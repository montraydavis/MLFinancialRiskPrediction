namespace MLFinancialRiskPrediction.AI.Plugins
{
    using System.ComponentModel;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ML;
    using Microsoft.SemanticKernel;

    using MLFinancialRiskPrediction.Configuration;
    using MLFinancialRiskPrediction.Extensions;
    using MLFinancialRiskPrediction.Models;
    using MLFinancialRiskPrediction.Services;

    public class EnsemblePredictorPlugin
    {
        public EnsemblePredictorPlugin(DataLoader dataLoader, [FromKeyedServices("FinancialRiskV2")] MLContext context, RiskScoreTrainer riskScoreTrainer, LoanApprovalTrainer loanApprovalTrainer, ModelEvaluator modelEvaluator, FeatureEngineer featureEngineer)
        {
            this._loanData = new LoanData
            {
                ApplicationDate = "2019-10-11",
                Age = 37f,
                AnnualIncome = 29503f,
                CreditScore = 576f,
                EmploymentStatus = "Employed",
                EducationLevel = "High School",
                Experience = 14f,
                LoanAmount = 18886f,
                LoanDuration = 60f,
                MaritalStatus = "Married",
                NumberOfDependents = 4f,
                HomeOwnershipStatus = "Mortgage",
                MonthlyDebtPayments = 527f,
                CreditCardUtilizationRate = 8.533589544452110f,
                NumberOfOpenCreditLines = 4f,
                NumberOfCreditInquiries = 1f,
                DebtToIncomeRatio = 2.515716286285740f,
                BankruptcyHistory = 0f,
                LoanPurpose = "Debt Consolidation",
                PreviousLoanDefaults = 0f,
                PaymentHistory = 27f,
                LengthOfCreditHistory = 27f,
                SavingsAccountBalance = 16163f,
                CheckingAccountBalance = 1695f,
                TotalAssets = 291811f,
                TotalLiabilities = 9046f,
                MonthlyIncome = 24.585833333333300f,
                UtilityBillsPaymentHistory = 8.251823539852960f,
                JobTenure = 5f,
                NetWorth = 282765f,
                BaseInterestRate = 23.588599999999900f,
                InterestRate = 23.602541509718300f,
                MonthlyLoanPayment = 5.389635595160010f,
                TotalDebtToIncomeRatio = 433.568203714606f,
                LoanApproved = false,
                RiskScore = -1.0f // Initial risk score, will be updated after prediction
            };

            // Load and split data
            Console.WriteLine("Loading data...");
            IEnumerable<Models.LoanData> allData = dataLoader.LoadData(ModelConfiguration.DataFilePath);
            (IEnumerable<Models.LoanData> trainData, IEnumerable<Models.LoanData> testData) = dataLoader.SplitData(allData, ModelConfiguration.TestDataFraction);

            Console.WriteLine($"Total records: {allData.Count()}");
            Console.WriteLine($"Training records: {trainData.Count()}");
            Console.WriteLine($"Test records: {testData.Count()}");
            Console.WriteLine();

            // Prepare features
            Console.WriteLine("Preparing features...");
            IDataView trainDataView = featureEngineer.PrepareFeatures(context, trainData);
            IDataView testDataView = featureEngineer.PrepareFeatures(context, testData);

            IDataView transformedTrainData = featureEngineer.ApplyTransformations(context, trainDataView);
            IDataView transformedTestData = featureEngineer.ApplyTransformations(context, testDataView);

            // Train Stage 1: RiskScore Model
            Console.WriteLine("=== Stage 1: Training RiskScore Model ===");
            ITransformer riskScoreModel = riskScoreTrainer.TrainModel(context, transformedTrainData);
            modelEvaluator.EvaluateRegressionModel(context, riskScoreModel, transformedTestData);

            // Train Stage 2: LoanApproval Model
            Console.WriteLine("=== Stage 2: Training LoanApproval Model ===");
            ITransformer loanApprovalModel = loanApprovalTrainer.TrainModel(context, transformedTrainData);
            modelEvaluator.EvaluateBinaryClassificationModel(context, loanApprovalModel, transformedTestData);

            // Create ensemble predictor
            ITransformer featureTransformer = context.CreateFeaturePipeline().Fit(trainDataView);
            this._predictor = new EnsemblePredictor(context, riskScoreModel, loanApprovalModel, featureTransformer);
        }

        #region Private Properties
        private readonly EnsemblePredictor _predictor;
        private readonly LoanData _loanData;
        private bool _isPredictionMade = false; // Flag to track if prediction has been made
        #endregion

        #region Private Methods
        private EnsemblePrediction MakePrediction()
        {
            EnsemblePrediction prediction = this._predictor.Predict(this._loanData);

            this._loanData.LoanApproved = prediction.LoanApproved; // Update the loan data with the prediction
            this._loanData.RiskScore = prediction.RiskScore; // Update the risk score in the loan data

            return prediction;
        }
        #endregion 

        #region Kernel Functions

        /// <summary>
        /// Kernel function to predict loan approval and risk score based on the current loan sample.
        /// </summary>
        /// <returns>
        /// The predicted risk score.
        /// </returns>
        [KernelFunction, Description("Predict the risk score for the current loan sample.")]
        public float PredictRiskScore()
        {
            this._isPredictionMade = true;

            EnsemblePrediction prediction = this.MakePrediction();

            return prediction.RiskScore;
        }

        /// <summary>
        /// Kernel function to predict loan approval based on the current loan sample.
        /// </summary>
        /// <returns>
        /// True if the loan is approved, otherwise false.
        /// </returns>
        [KernelFunction, Description("Predict if the loan is approved for the current loan sample.")]
        public bool PredictLoanApproval()
        {
            this._isPredictionMade = true;
            EnsemblePrediction prediction = this.MakePrediction();

            return prediction.LoanApproved;
        }

        /// <summary>
        /// Kernel function to get the current loan sample.
        /// </summary>
        /// <returns>
        /// A string representation of the current loan sample.
        /// </returns>
        [KernelFunction, Description("Get the current loan sample.")]
        public string GetCurrentSample()
        {
            return $"""
                Current Loan Sample:

                Application Date: {this._loanData.ApplicationDate}
                Age: {this._loanData.Age}
                Annual Income: {this._loanData.AnnualIncome}
                Credit Score: {this._loanData.CreditScore}
                Employment Status: {this._loanData.EmploymentStatus}
                Education Level: {this._loanData.EducationLevel}
                Experience: {this._loanData.Experience}
                Loan Amount: {this._loanData.LoanAmount}
                Loan Duration: {this._loanData.LoanDuration}
                Marital Status: {this._loanData.MaritalStatus}
                Number of Dependents: {this._loanData.NumberOfDependents}
                Home Ownership Status: {this._loanData.HomeOwnershipStatus}
                Monthly Debt Payments: {this._loanData.MonthlyDebtPayments}
                Credit Card Utilization Rate: {this._loanData.CreditCardUtilizationRate}
                Number of Open Credit Lines: {this._loanData.NumberOfOpenCreditLines}
                Number of Credit Inquiries: {this._loanData.NumberOfCreditInquiries}
                Debt to Income Ratio: {this._loanData.DebtToIncomeRatio}
                Bankruptcy History: {this._loanData.BankruptcyHistory}
                Loan Purpose: {this._loanData.LoanPurpose}
                Previous Loan Defaults: {this._loanData.PreviousLoanDefaults}
                Payment History: {this._loanData.PaymentHistory}
                Length of Credit History: {this._loanData.LengthOfCreditHistory}
                Savings Account Balance: {this._loanData.SavingsAccountBalance}
                Checking Account Balance: {this._loanData.CheckingAccountBalance}
                Total Assets: {this._loanData.TotalAssets}
                Total Liabilities: {this._loanData.TotalLiabilities}
                Monthly Income: {this._loanData.MonthlyIncome}
                Utility Bills Payment History: {this._loanData.UtilityBillsPaymentHistory}
                Job Tenure: {this._loanData.JobTenure}
                Net Worth: {this._loanData.NetWorth}
                Base Interest Rate: {this._loanData.BaseInterestRate}
                Interest Rate: {this._loanData.InterestRate}
                Monthly Loan Payment: {this._loanData.MonthlyLoanPayment}
                Total Debt to Income Ratio: {this._loanData.TotalDebtToIncomeRatio}
                Risk Score: {(this._isPredictionMade ? this._loanData.RiskScore.ToString() : "(Not Initialized)")}
                Loan Approved: {(this._isPredictionMade ? this._loanData.LoanApproved.ToString() : "(Not Initialized)")}
                """;

        }

        /// <summary>
        /// Kernel function to update the current loan sample with new values.
        /// </summary>
        /// <param name="applicationDate">The application date of the loan.</param>
        /// <param name="age">The age of the applicant.</param>
        /// <param name="annualIncome">The annual income of the applicant.</param>
        /// <param name="creditScore">The credit score of the applicant.</param>
        /// <param name="employmentStatus">The employment status of the applicant.</param>
        /// <param name="educationLevel">The education level of the applicant.</param>
        /// <param name="experience">The years of experience of the applicant.</param>
        /// <param name="loanAmount">The amount of the loan.</param>
        /// <param name="loanDuration">The duration of the loan in months.</param>
        /// <param name="maritalStatus">The marital status of the applicant.</param>
        /// <param name="numberOfDependents">The number of dependents of the applicant.</param>
        /// <param name="homeOwnershipStatus">The home ownership status of the applicant.</param>
        /// <param name="monthlyDebtPayments">The monthly debt payments of the applicant.</param>
        /// <param name="creditCardUtilizationRate">The credit card utilization rate of the applicant.</param>
        /// <param name="numberOfOpenCreditLines">The number of open credit lines of the applicant.</param>
        /// <param name="numberOfCreditInquiries">The number of credit inquiries of the applicant.</param>
        /// <param name="debtToIncomeRatio">The debt to income ratio of the applicant.</param>
        /// <param name="bankruptcyHistory">The bankruptcy history of the applicant.</param>
        /// <param name="loanPurpose">The purpose of the loan.</param>
        /// <param name="previousLoanDefaults">The number of previous loan defaults.</param>
        /// <param name="paymentHistory">The payment history of the applicant.</param>
        /// <param name="lengthOfCreditHistory">The length of credit history of the applicant.</param>
        /// <param name="savingsAccountBalance">The savings account balance of the applicant.</param>
        /// <param name="checkingAccountBalance">The checking account balance of the applicant.</param>
        /// <param name="totalAssets">The total assets of the applicant.</param>
        /// <param name="totalLiabilities">The total liabilities of the applicant.</param>
        /// <param name="monthlyIncome">The monthly income of the applicant.</param>
        /// <param name="utilityBillsPaymentHistory">The utility bills payment history of the applicant.</param>
        /// <param name="jobTenure">The job tenure of the applicant.</param>
        /// <param name="netWorth">The net worth of the applicant.</param>
        /// <param name="baseInterestRate">The base interest rate for the loan.</param>
        /// <param name="interestRate">The interest rate for the loan.</param>
        /// <param name="monthlyLoanPayment">The monthly loan payment amount.</param>
        /// <param name="totalDebtToIncomeRatio">The total debt to income ratio of the applicant.</param>
        [KernelFunction, Description("Update the current loan sample.")]
        public void UpdateSampleLoan(
            string? applicationDate = null,
            float? age = null,
            float? annualIncome = null,
            float? creditScore = null,
            string? employmentStatus = null,
            string? educationLevel = null,
            float? experience = null,
            float? loanAmount = null,
            float? loanDuration = null,
            string? maritalStatus = null,
            float? numberOfDependents = null,
            string? homeOwnershipStatus = null,
            float? monthlyDebtPayments = null,
            float? creditCardUtilizationRate = null,
            float? numberOfOpenCreditLines = null,
            float? numberOfCreditInquiries = null,
            float? debtToIncomeRatio = null,
            float? bankruptcyHistory = null,
            string? loanPurpose = null,
            float? previousLoanDefaults = null,
            float? paymentHistory = null,
            float? lengthOfCreditHistory = null,
            float? savingsAccountBalance = null,
            float? checkingAccountBalance = null,
            float? totalAssets = null,
            float? totalLiabilities = null,
            float? monthlyIncome = null,
            float? utilityBillsPaymentHistory = null,
            float? jobTenure = null,
            float? netWorth = null,
            float? baseInterestRate = null,
            float? interestRate = null,
            float? monthlyLoanPayment = null,
            float? totalDebtToIncomeRatio = null)
        {
            if (applicationDate != null)
            {
                this._loanData.ApplicationDate = applicationDate;
            }

            if (age.HasValue)
            {
                this._loanData.Age = age.Value;
            }

            if (annualIncome.HasValue)
            {
                this._loanData.AnnualIncome = annualIncome.Value;
            }

            if (creditScore.HasValue)
            {
                this._loanData.CreditScore = creditScore.Value;
            }

            if (employmentStatus != null)
            {
                this._loanData.EmploymentStatus = employmentStatus;
            }

            if (educationLevel != null)
            {
                this._loanData.EducationLevel = educationLevel;
            }

            if (experience.HasValue)
            {
                this._loanData.Experience = experience.Value;
            }

            if (loanAmount.HasValue)
            {
                this._loanData.LoanAmount = loanAmount.Value;
            }

            if (loanDuration.HasValue)
            {
                this._loanData.LoanDuration = loanDuration.Value;
            }

            if (maritalStatus != null)
            {
                this._loanData.MaritalStatus = maritalStatus;
            }

            if (numberOfDependents.HasValue)
            {
                this._loanData.NumberOfDependents = numberOfDependents.Value;
            }

            if (homeOwnershipStatus != null)
            {
                this._loanData.HomeOwnershipStatus = homeOwnershipStatus;
            }

            if (monthlyDebtPayments.HasValue)
            {
                this._loanData.MonthlyDebtPayments = monthlyDebtPayments.Value;
            }

            if (creditCardUtilizationRate.HasValue)
            {
                this._loanData.CreditCardUtilizationRate = creditCardUtilizationRate.Value;
            }

            if (numberOfOpenCreditLines.HasValue)
            {
                this._loanData.NumberOfOpenCreditLines = numberOfOpenCreditLines.Value;
            }

            if (numberOfCreditInquiries.HasValue)
            {
                this._loanData.NumberOfCreditInquiries = numberOfCreditInquiries.Value;
            }

            if (debtToIncomeRatio.HasValue)
            {
                this._loanData.DebtToIncomeRatio = debtToIncomeRatio.Value;
            }

            if (bankruptcyHistory.HasValue)
            {
                this._loanData.BankruptcyHistory = bankruptcyHistory.Value;
            }

            if (loanPurpose != null)
            {
                this._loanData.LoanPurpose = loanPurpose;
            }

            if (previousLoanDefaults.HasValue)
            {
                this._loanData.PreviousLoanDefaults = previousLoanDefaults.Value;
            }

            if (paymentHistory.HasValue)
            {
                this._loanData.PaymentHistory = paymentHistory.Value;
            }

            if (lengthOfCreditHistory.HasValue)
            {
                this._loanData.LengthOfCreditHistory = lengthOfCreditHistory.Value;
            }

            if (savingsAccountBalance.HasValue)
            {
                this._loanData.SavingsAccountBalance = savingsAccountBalance.Value;
            }

            if (checkingAccountBalance.HasValue)
            {
                this._loanData.CheckingAccountBalance = checkingAccountBalance.Value;
            }

            if (totalAssets.HasValue)
            {
                this._loanData.TotalAssets = totalAssets.Value;
            }

            if (totalLiabilities.HasValue)
            {
                this._loanData.TotalLiabilities = totalLiabilities.Value;
            }

            if (monthlyIncome.HasValue)
            {
                this._loanData.MonthlyIncome = monthlyIncome.Value;
            }

            if (utilityBillsPaymentHistory.HasValue)
            {
                this._loanData.UtilityBillsPaymentHistory = utilityBillsPaymentHistory.Value;
            }

            if (jobTenure.HasValue)
            {
                this._loanData.JobTenure = jobTenure.Value;
            }

            if (netWorth.HasValue)
            {
                this._loanData.NetWorth = netWorth.Value;
            }

            if (baseInterestRate.HasValue)
            {
                this._loanData.BaseInterestRate = baseInterestRate.Value;
            }

            if (interestRate.HasValue)
            {
                this._loanData.InterestRate = interestRate.Value;
            }

            if (monthlyLoanPayment.HasValue)
            {
                this._loanData.MonthlyLoanPayment = monthlyLoanPayment.Value;
            }

            if (totalDebtToIncomeRatio.HasValue)
            {
                this._loanData.TotalDebtToIncomeRatio = totalDebtToIncomeRatio.Value;
            }
        }
        #endregion
    }
}

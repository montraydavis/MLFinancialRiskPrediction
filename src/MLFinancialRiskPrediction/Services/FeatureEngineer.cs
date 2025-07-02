namespace MLFinancialRiskPrediction.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.ML;

    using MLFinancialRiskPrediction.Core;
    using MLFinancialRiskPrediction.Models;

    public class FeatureEngineer : IFeatureEngineer
    {
        /// <summary>
        /// Prepares the features for the machine learning model by loading data from an enumerable collection.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="data">The loan data to be loaded.</param>
        /// <returns>An <see cref="IDataView"/> containing the loaded loan data.</returns>
        public IDataView PrepareFeatures(MLContext context, IEnumerable<LoanData> data)
        {
            return context.Data.LoadFromEnumerable(data);
        }

        /// <summary>
        /// Applies transformations to the data to prepare it for training the machine learning model.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="data">The data to be transformed.</param>
        /// <returns>An <see cref="IDataView"/> containing the transformed data.</returns>
        public IDataView ApplyTransformations(MLContext context, IDataView data)
        {
            Microsoft.ML.Data.EstimatorChain<Microsoft.ML.Data.ColumnConcatenatingTransformer> pipeline = context.Transforms.Categorical.OneHotEncoding(
                    new[]
                    {
                    new InputOutputColumnPair("EmploymentStatusEncoded", "EmploymentStatus"),
                    new InputOutputColumnPair("EducationLevelEncoded", "EducationLevel"),
                    new InputOutputColumnPair("MaritalStatusEncoded", "MaritalStatus"),
                    new InputOutputColumnPair("HomeOwnershipStatusEncoded", "HomeOwnershipStatus"),
                    new InputOutputColumnPair("LoanPurposeEncoded", "LoanPurpose")
                    })
                .Append(context.Transforms.NormalizeMeanVariance(
                    new[]
                    {
                    new InputOutputColumnPair("AgeNormalized", "Age"),
                    new InputOutputColumnPair("AnnualIncomeNormalized", "AnnualIncome"),
                    new InputOutputColumnPair("CreditScoreNormalized", "CreditScore"),
                    new InputOutputColumnPair("LoanAmountNormalized", "LoanAmount"),
                    new InputOutputColumnPair("MonthlyIncomeNormalized", "MonthlyIncome")
                    }))
                .Append(context.Transforms.Concatenate("Features",
                    "AgeNormalized", "AnnualIncomeNormalized", "CreditScoreNormalized", "EmploymentStatusEncoded",
                    "EducationLevelEncoded", "Experience", "LoanAmountNormalized", "LoanDuration",
                    "MaritalStatusEncoded", "NumberOfDependents", "HomeOwnershipStatusEncoded",
                    "MonthlyDebtPayments", "CreditCardUtilizationRate", "NumberOfOpenCreditLines",
                    "NumberOfCreditInquiries", "DebtToIncomeRatio", "BankruptcyHistory", "LoanPurposeEncoded",
                    "PreviousLoanDefaults", "PaymentHistory", "LengthOfCreditHistory", "SavingsAccountBalance",
                    "CheckingAccountBalance", "TotalAssets", "TotalLiabilities", "MonthlyIncomeNormalized",
                    "UtilityBillsPaymentHistory", "JobTenure", "NetWorth", "BaseInterestRate",
                    "InterestRate", "MonthlyLoanPayment", "TotalDebtToIncomeRatio"));

            return pipeline.Fit(data).Transform(data);
        }
    }
}

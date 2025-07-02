namespace MLFinancialRiskPrediction.UI.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ML;

    using MLFinancialRiskPrediction.AI.Extensions;
    using MLFinancialRiskPrediction.Core;
    using MLFinancialRiskPrediction.Services;
    using MLFinancialRiskPrediction.UI.Services;
    using MLFinancialRiskPrediction.UI.ViewModeels;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUIServices(this IServiceCollection services)
        {
            // Register AI services from the AI project
            services.AddMLServices()
                   .AddSemanticKernel();

            // Register ML Core Services
            services.AddSingleton<MLContext>();
            services.AddSingleton<DataLoader>();
            services.AddSingleton<IFeatureEngineer, FeatureEngineer>();
            services.AddSingleton<IRiskScoreTrainer, RiskScoreTrainer>();
            services.AddSingleton<ILoanApprovalTrainer, LoanApprovalTrainer>();
            services.AddSingleton<IModelEvaluator, ModelEvaluator>();

            // Register EnsemblePredictor with factory pattern
            services.AddSingleton<IEnsemblePredictor>(serviceProvider =>
            {
                MLContext context = serviceProvider.GetRequiredService<MLContext>();
                DataLoader dataLoader = serviceProvider.GetRequiredService<DataLoader>();
                IFeatureEngineer featureEngineer = serviceProvider.GetRequiredService<IFeatureEngineer>();
                IRiskScoreTrainer riskScoreTrainer = serviceProvider.GetRequiredService<IRiskScoreTrainer>();
                ILoanApprovalTrainer loanApprovalTrainer = serviceProvider.GetRequiredService<ILoanApprovalTrainer>();

                try
                {
                    // Load training data
                    IEnumerable<MLFinancialRiskPrediction.Models.LoanData> data = dataLoader.LoadData("loans.csv");
                    (IEnumerable<MLFinancialRiskPrediction.Models.LoanData> trainData, IEnumerable<MLFinancialRiskPrediction.Models.LoanData> testData) = dataLoader.SplitData(data);

                    // Prepare features and get the feature transformer
                    IDataView trainDataView = featureEngineer.PrepareFeatures(context, trainData);

                    // Create the feature transformation pipeline
                    Microsoft.ML.Data.EstimatorChain<Microsoft.ML.Data.ColumnConcatenatingTransformer> featureTransformationPipeline = context.Transforms.Categorical.OneHotEncoding(
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

                    // Fit the feature transformer and transform data
                    Microsoft.ML.Data.TransformerChain<Microsoft.ML.Data.ColumnConcatenatingTransformer> featureTransformer = featureTransformationPipeline.Fit(trainDataView);
                    IDataView transformedTrainData = featureTransformer.Transform(trainDataView);

                    // Train models
                    ITransformer riskScoreModel = riskScoreTrainer.TrainModel(context, transformedTrainData);
                    ITransformer loanApprovalModel = loanApprovalTrainer.TrainModel(context, transformedTrainData);

                    return new EnsemblePredictor(context, riskScoreModel, loanApprovalModel, featureTransformer);
                }
                catch (Exception ex)
                {
                    // Log the error and return a dummy predictor or throw
                    System.Diagnostics.Debug.WriteLine($"Failed to initialize EnsemblePredictor: {ex.Message}");
                    throw new InvalidOperationException("Failed to initialize ML models. Ensure loans.csv exists and is accessible.", ex);
                }
            });

            // Register UI-specific services
            services.AddSingleton<IAIChatService, AIChatService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IPredictionService, PredictionService>();

            // Register ViewModels
            services.AddTransient<ChatViewModel>();
            services.AddTransient<LoanApplicationViewModel>();
            services.AddTransient<SinglePredictionViewModel>();

            return services;
        }
    }
}
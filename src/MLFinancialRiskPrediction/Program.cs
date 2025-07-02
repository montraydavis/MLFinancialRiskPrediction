namespace MLFinancialRiskPrediction
{
    using Microsoft.ML;

    using MLFinancialRiskPrediction.Configuration;
    using MLFinancialRiskPrediction.Extensions;
    using MLFinancialRiskPrediction.Services;

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ML Financial Risk Prediction - Ensemble Model");
            Console.WriteLine("============================================");

            MLContext context = new MLContext(ModelConfiguration.RandomSeed);

            try
            {
                // Initialize services
                DataLoader dataLoader = new DataLoader();
                FeatureEngineer featureEngineer = new FeatureEngineer();
                RiskScoreTrainer riskScoreTrainer = new RiskScoreTrainer();
                LoanApprovalTrainer loanApprovalTrainer = new LoanApprovalTrainer();
                ModelEvaluator modelEvaluator = new ModelEvaluator();

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
                EnsemblePredictor ensemblePredictor = new EnsemblePredictor(context, riskScoreModel, loanApprovalModel, featureTransformer);

                // Demonstrate predictions
                Console.WriteLine("=== Sample Predictions ===");
                IEnumerable<Models.LoanData> sampleData = testData.Take(3);

                foreach (Models.LoanData? sample in sampleData)
                {
                    Models.EnsemblePrediction prediction = ensemblePredictor.Predict(sample);

                    Console.WriteLine($"Input: Age={sample.Age}, Income=${sample.AnnualIncome:N0}, Credit={sample.CreditScore}");
                    Console.WriteLine($"Prediction: RiskScore={prediction.RiskScore:F1}, " +
                                    $"LoanApproved={prediction.LoanApproved}, " +
                                    $"Probability={prediction.ApprovalProbability:F2}");
                    Console.WriteLine($"Actual: RiskScore={sample.RiskScore:F1}, LoanApproved={sample.LoanApproved}");
                    Console.WriteLine();
                }

                // Dispose resources
                ensemblePredictor.Dispose();

                // Save models
                Console.WriteLine("Saving models...");
                context.SaveModel(riskScoreModel, transformedTrainData, ModelConfiguration.ModelPaths.RiskScoreModelPath);
                context.SaveModel(loanApprovalModel, transformedTrainData, ModelConfiguration.ModelPaths.LoanApprovalModelPath);
                context.SaveModel(featureTransformer, trainDataView, ModelConfiguration.ModelPaths.FeatureTransformerPath);

                Console.WriteLine("Training completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

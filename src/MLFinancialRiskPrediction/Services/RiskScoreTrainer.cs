namespace MLFinancialRiskPrediction.Services
{
    using System;

    using Microsoft.ML;

    using MLFinancialRiskPrediction.Core;

    public class RiskScoreTrainer : IRiskScoreTrainer
    {
        /// <summary>
        /// Trains a LightGBM regression model for predicting risk scores.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="trainData">The training data.</param>
        /// <returns>
        /// An <see cref="ITransformer"/> representing the trained model.
        /// </returns>
        public ITransformer TrainModel(MLContext context, IDataView trainData)
        {
            Microsoft.ML.Trainers.LightGbm.LightGbmRegressionTrainer pipeline = context.Regression.Trainers.LightGbm(
                labelColumnName: "RiskScore",
                featureColumnName: "Features",
                numberOfLeaves: 31,
                learningRate: 0.1,
                minimumExampleCountPerLeaf: 20,
                numberOfIterations: 100);

            Console.WriteLine("Training RiskScore model...");
            Microsoft.ML.Data.RegressionPredictionTransformer<Microsoft.ML.Trainers.LightGbm.LightGbmRegressionModelParameters> model = pipeline.Fit(trainData);
            Console.WriteLine("RiskScore model training completed.");

            return model;
        }

        /// <summary>
        /// Evaluates the regression model for predicting risk scores.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="model">The trained model.</param>
        /// <param name="testData">The test data.</param>
        public void EvaluateModel(MLContext context, ITransformer model, IDataView testData)
        {
            IDataView predictions = model.Transform(testData);
            Microsoft.ML.Data.RegressionMetrics metrics = context.Regression.Evaluate(predictions, labelColumnName: "RiskScore");

            Console.WriteLine($"RiskScore Model Metrics:");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F2}");
            Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:F2}");
            Console.WriteLine($"R²: {metrics.RSquared:F2}");
        }
    }
}

namespace MLFinancialRiskPrediction.Services
{
    using System;

    using Microsoft.ML;

    using MLFinancialRiskPrediction.Core;

    public class LoanApprovalTrainer : ILoanApprovalTrainer
    {
        /// <summary>
        /// Trains a LightGBM binary classification model for loan approval prediction.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="trainData">The training data.</param>
        /// <returns>
        /// An <see cref="ITransformer"/> representing the trained model.
        /// </returns>
        public ITransformer TrainModel(MLContext context, IDataView trainData)
        {
            Microsoft.ML.Trainers.LightGbm.LightGbmBinaryTrainer pipeline = context.BinaryClassification.Trainers.LightGbm(
                labelColumnName: "LoanApproved",
                featureColumnName: "Features",
                numberOfLeaves: 31,
                learningRate: 0.1,
                minimumExampleCountPerLeaf: 20,
                numberOfIterations: 100);

            Console.WriteLine("Training LoanApproval model...");
            Microsoft.ML.Data.BinaryPredictionTransformer<Microsoft.ML.Calibrators.CalibratedModelParametersBase<Microsoft.ML.Trainers.LightGbm.LightGbmBinaryModelParameters, Microsoft.ML.Calibrators.PlattCalibrator>> model = pipeline.Fit(trainData);
            Console.WriteLine("LoanApproval model training completed.");

            return model;
        }

        /// <summary>
        /// Evaluates the trained loan approval model using the provided test data.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="model">The trained model.</param>
        /// <param name="testData">The test data.</param>
        public void EvaluateModel(MLContext context, ITransformer model, IDataView testData)
        {
            IDataView predictions = model.Transform(testData);
            Microsoft.ML.Data.CalibratedBinaryClassificationMetrics metrics = context.BinaryClassification.Evaluate(predictions, labelColumnName: "LoanApproved");

            Console.WriteLine($"LoanApproval Model Metrics:");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:F2}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision:F2}");
            Console.WriteLine($"Recall: {metrics.PositiveRecall:F2}");
            Console.WriteLine($"F1 Score: {metrics.F1Score:F2}");
            Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:F2}");
        }
    }
}

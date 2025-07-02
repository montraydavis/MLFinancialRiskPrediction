namespace MLFinancialRiskPrediction.Services
{
    using System;

    using Microsoft.ML;

    using MLFinancialRiskPrediction.Core;

    public class ModelEvaluator : IModelEvaluator
    {
        /// <summary>
        /// Evaluates the regression model for predicting risk scores.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="model">The trained model.</param>
        /// <param name="testData">The test data.</param>
        /// <remarks>
        /// This method transforms the test data using the model and evaluates the regression metrics such as Root
        /// Mean Squared Error, Mean Absolute Error, Mean Squared Error, R-Squared, and Loss Function.
        /// It prints the evaluation results to the console.
        /// </remarks>
        public void EvaluateRegressionModel(MLContext context, ITransformer model, IDataView testData)
        {
            IDataView predictions = model.Transform(testData);
            Microsoft.ML.Data.RegressionMetrics metrics = context.Regression.Evaluate(predictions, labelColumnName: "RiskScore");

            Console.WriteLine("=== RiskScore Regression Model Evaluation ===");
            Console.WriteLine($"Root Mean Squared Error: {metrics.RootMeanSquaredError:F4}");
            Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError:F4}");
            Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError:F4}");
            Console.WriteLine($"R-Squared: {metrics.RSquared:F4}");
            Console.WriteLine($"Loss Function: {metrics.LossFunction:F4}");
            Console.WriteLine();
        }

        /// <summary>
        /// Evaluates the binary classification model for predicting loan approval.
        /// </summary>
        /// <param name="context">The ML.NET context.</param>
        /// <param name="model">The trained model.</param>
        /// <param name="testData">The test data.</param>
        /// <remarks>
        /// This method transforms the test data using the model and evaluates the binary classification metrics such as accuracy, precision, recall, F1 score, and AUC (Area Under the ROC Curve).
        /// </remarks>
        public void EvaluateBinaryClassificationModel(MLContext context, ITransformer model, IDataView testData)
        {
            IDataView predictions = model.Transform(testData);
            Microsoft.ML.Data.CalibratedBinaryClassificationMetrics metrics = context.BinaryClassification.Evaluate(predictions, labelColumnName: "LoanApproved");

            Console.WriteLine("=== LoanApproval Binary Classification Model Evaluation ===");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:F4}");
            Console.WriteLine($"Area Under ROC Curve: {metrics.AreaUnderRocCurve:F4}");
            Console.WriteLine($"Area Under Precision-Recall Curve: {metrics.AreaUnderPrecisionRecallCurve:F4}");
            Console.WriteLine($"F1 Score: {metrics.F1Score:F4}");
            Console.WriteLine($"Positive Precision: {metrics.PositivePrecision:F4}");
            Console.WriteLine($"Positive Recall: {metrics.PositiveRecall:F4}");
            Console.WriteLine($"Negative Precision: {metrics.NegativePrecision:F4}");
            Console.WriteLine($"Negative Recall: {metrics.NegativeRecall:F4}");
            Console.WriteLine();
        }
    }
}

namespace MLFinancialRiskPrediction.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MLFinancialRiskPrediction.Core;
    using MLFinancialRiskPrediction.Models;

    public static class EnsembleEvaluationExtensions
    {
        public static void EvaluateEnsembleAccuracy(this IEnsemblePredictor predictor, IEnumerable<LoanData> testData)
        {
            List<LoanData> testList = testData.ToList();
            List<EnsemblePrediction> predictions = predictor.PredictBatch(testList).ToList();

            Console.WriteLine("=== Manual Ensemble Evaluation ===");

            EvaluateRiskScoreAccuracy(testList, predictions);
            Console.WriteLine();
            EvaluateLoanApprovalAccuracy(testList, predictions);
            Console.WriteLine();
            EvaluateEnsembleBusinessMetrics(testList, predictions);
        }

        private static void EvaluateRiskScoreAccuracy(List<LoanData> actual, List<EnsemblePrediction> predictions)
        {
            Console.WriteLine("--- Risk Score Regression Metrics ---");

            int n = actual.Count;
            List<float> actualScores = actual.Select(x => x.RiskScore).ToList();
            List<float> predictedScores = predictions.Select(x => x.RiskScore).ToList();

            // Mean Absolute Error
            float mae = actualScores.Zip(predictedScores, (a, p) => Math.Abs(a - p)).Average();

            // Root Mean Square Error
            double rmse = Math.Sqrt(actualScores.Zip(predictedScores, (a, p) => Math.Pow(a - p, 2)).Average());

            // Mean Absolute Percentage Error
            float mape = actualScores.Zip(predictedScores, (a, p) =>
                a != 0 ? Math.Abs((a - p) / a) * 100 : 0).Average();

            // R-Squared
            float actualMean = actualScores.Average();
            double totalSumSquares = actualScores.Sum(a => Math.Pow(a - actualMean, 2));
            double residualSumSquares = actualScores.Zip(predictedScores, (a, p) => Math.Pow(a - p, 2)).Sum();
            double rSquared = 1 - (residualSumSquares / totalSumSquares);

            // Accuracy within tolerance ranges
            int within5Points = actualScores.Zip(predictedScores, (a, p) => Math.Abs(a - p) <= 5).Count(x => x);
            int within10Points = actualScores.Zip(predictedScores, (a, p) => Math.Abs(a - p) <= 10).Count(x => x);

            Console.WriteLine($"Sample Size: {n}");
            Console.WriteLine($"Mean Absolute Error: {mae:F2}");
            Console.WriteLine($"Root Mean Square Error: {rmse:F2}");
            Console.WriteLine($"Mean Absolute Percentage Error: {mape:F2}%");
            Console.WriteLine($"R-Squared: {rSquared:F4}");
            Console.WriteLine($"Predictions within ±5 points: {within5Points}/{n} ({(double)within5Points / n * 100:F1}%)");
            Console.WriteLine($"Predictions within ±10 points: {within10Points}/{n} ({(double)within10Points / n * 100:F1}%)");
        }

        private static void EvaluateLoanApprovalAccuracy(List<LoanData> actual, List<EnsemblePrediction> predictions)
        {
            Console.WriteLine("--- Loan Approval Classification Metrics ---");

            int n = actual.Count;
            List<bool> actualApprovals = actual.Select(x => x.LoanApproved).ToList();
            List<bool> predictedApprovals = predictions.Select(x => x.LoanApproved).ToList();

            // Confusion Matrix
            int tp = actualApprovals.Zip(predictedApprovals, (a, p) => a && p).Count(x => x); // True Positive
            int tn = actualApprovals.Zip(predictedApprovals, (a, p) => !a && !p).Count(x => x); // True Negative
            int fp = actualApprovals.Zip(predictedApprovals, (a, p) => !a && p).Count(x => x); // False Positive
            int fn = actualApprovals.Zip(predictedApprovals, (a, p) => a && !p).Count(x => x); // False Negative

            // Calculate metrics
            double accuracy = (double)(tp + tn) / n;
            double precision = tp > 0 ? (double)tp / (tp + fp) : 0;
            double recall = tp > 0 ? (double)tp / (tp + fn) : 0;
            double f1Score = precision + recall > 0 ? 2 * (precision * recall) / (precision + recall) : 0;
            double specificity = tn > 0 ? (double)tn / (tn + fp) : 0;

            Console.WriteLine($"Sample Size: {n}");
            Console.WriteLine($"Confusion Matrix:");
            Console.WriteLine($"  True Positives (Approved correctly): {tp}");
            Console.WriteLine($"  True Negatives (Rejected correctly): {tn}");
            Console.WriteLine($"  False Positives (Approved incorrectly): {fp}");
            Console.WriteLine($"  False Negatives (Rejected incorrectly): {fn}");
            Console.WriteLine($"Overall Accuracy: {accuracy:F4} ({accuracy * 100:F1}%)");
            Console.WriteLine($"Precision (Approved predictions that were correct): {precision:F4} ({precision * 100:F1}%)");
            Console.WriteLine($"Recall (Actual approvals that were predicted): {recall:F4} ({recall * 100:F1}%)");
            Console.WriteLine($"F1 Score: {f1Score:F4}");
            Console.WriteLine($"Specificity (Rejections that were correct): {specificity:F4} ({specificity * 100:F1}%)");
        }

        private static void EvaluateEnsembleBusinessMetrics(List<LoanData> actual, List<EnsemblePrediction> predictions)
        {
            Console.WriteLine("--- Business Impact Metrics ---");

            int n = actual.Count;

            // Risk assessment accuracy by approval outcome
            var approvedLoans = actual.Zip(predictions, (a, p) => new { Actual = a, Predicted = p })
                .Where(x => x.Predicted.LoanApproved).ToList();

            var rejectedLoans = actual.Zip(predictions, (a, p) => new { Actual = a, Predicted = p })
                .Where(x => !x.Predicted.LoanApproved).ToList();

            if (approvedLoans.Any())
            {
                float avgRiskOfApproved = approvedLoans.Average(x => x.Actual.RiskScore);
                float avgPredictedRiskOfApproved = approvedLoans.Average(x => x.Predicted.RiskScore);
                Console.WriteLine($"Approved Loans - Average Actual Risk: {avgRiskOfApproved:F1}, Predicted: {avgPredictedRiskOfApproved:F1}");
            }

            if (rejectedLoans.Any())
            {
                float avgRiskOfRejected = rejectedLoans.Average(x => x.Actual.RiskScore);
                float avgPredictedRiskOfRejected = rejectedLoans.Average(x => x.Predicted.RiskScore);
                Console.WriteLine($"Rejected Loans - Average Actual Risk: {avgRiskOfRejected:F1}, Predicted: {avgPredictedRiskOfRejected:F1}");
            }

            // Risk categories analysis
            float highRiskThreshold = 55.0f;
            int actualHighRisk = actual.Count(x => x.RiskScore >= highRiskThreshold);
            int predictedHighRisk = predictions.Count(x => x.RiskScore >= highRiskThreshold);

            Console.WriteLine($"High Risk Loans (>={highRiskThreshold}):");
            Console.WriteLine($"  Actual: {actualHighRisk}/{n} ({(double)actualHighRisk / n * 100:F1}%)");
            Console.WriteLine($"  Predicted: {predictedHighRisk}/{n} ({(double)predictedHighRisk / n * 100:F1}%)");

            // Probability distribution analysis
            int highConfidenceApprovals = predictions.Count(x => x.LoanApproved && x.ApprovalProbability >= 0.8);
            int lowConfidenceApprovals = predictions.Count(x => x.LoanApproved && x.ApprovalProbability < 0.6);

            Console.WriteLine($"Approval Confidence Distribution:");
            Console.WriteLine($"  High Confidence Approvals (≥80%): {highConfidenceApprovals}");
            Console.WriteLine($"  Low Confidence Approvals (<60%): {lowConfidenceApprovals}");
        }

        public static void EvaluateIndividualLayer(this IEnsemblePredictor predictor, IEnumerable<LoanData> testData, string layerName)
        {
            List<LoanData> testList = testData.ToList();
            List<EnsemblePrediction> predictions = predictor.PredictBatch(testList).ToList();

            Console.WriteLine($"=== {layerName} Layer Evaluation ===");

            if (layerName.Contains("Risk"))
            {
                EvaluateRiskScoreAccuracy(testList, predictions);
            }
            else if (layerName.Contains("Approval"))
            {
                EvaluateLoanApprovalAccuracy(testList, predictions);
            }
        }
    }
}

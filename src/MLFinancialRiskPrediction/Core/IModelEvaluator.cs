namespace MLFinancialRiskPrediction.Core
{
    using Microsoft.ML;

    public interface IModelEvaluator
    {
        void EvaluateRegressionModel(MLContext context, ITransformer model, IDataView testData);
        void EvaluateBinaryClassificationModel(MLContext context, ITransformer model, IDataView testData);
    }
}

namespace MLFinancialRiskPrediction.Core
{
    using Microsoft.ML;

    public interface IRiskScoreTrainer
    {
        ITransformer TrainModel(MLContext context, IDataView trainData);
        void EvaluateModel(MLContext context, ITransformer model, IDataView testData);
    }
}

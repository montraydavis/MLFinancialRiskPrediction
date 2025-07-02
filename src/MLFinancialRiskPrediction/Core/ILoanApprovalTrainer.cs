namespace MLFinancialRiskPrediction.Core
{
    using Microsoft.ML;

    public interface ILoanApprovalTrainer
    {
        ITransformer TrainModel(MLContext context, IDataView trainData);
        void EvaluateModel(MLContext context, ITransformer model, IDataView testData);
    }
}

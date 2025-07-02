namespace MLFinancialRiskPrediction.Core
{
    using System.Collections.Generic;

    using MLFinancialRiskPrediction.Models;

    public interface IEnsemblePredictor : IDisposable
    {
        EnsemblePrediction Predict(LoanData input);
        IEnumerable<EnsemblePrediction> PredictBatch(IEnumerable<LoanData> inputs);
    }
}

namespace MLFinancialRiskPrediction.Core
{
    using System.Collections.Generic;

    using Microsoft.ML;

    using MLFinancialRiskPrediction.Models;

    public interface IFeatureEngineer
    {
        IDataView PrepareFeatures(MLContext context, IEnumerable<LoanData> data);
        IDataView ApplyTransformations(MLContext context, IDataView data);
    }
}

namespace MLFinancialRiskPrediction.Models
{
    using Microsoft.ML.Data;

    public class RiskScorePrediction
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
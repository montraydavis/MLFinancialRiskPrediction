namespace MLFinancialRiskPrediction.Models
{
    public class EnsemblePrediction
    {
        public float RiskScore { get; set; }
        public bool LoanApproved { get; set; }
        public float ApprovalProbability { get; set; }
        public float ApprovalScore { get; set; }
    }
}
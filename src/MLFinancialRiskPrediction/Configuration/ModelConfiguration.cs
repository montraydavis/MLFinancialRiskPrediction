namespace MLFinancialRiskPrediction.Configuration
{
    public static class ModelConfiguration
    {
        public const string DataFilePath = "loans.csv";
        public const double TestDataFraction = 0.3;
        public const int RandomSeed = 42;

        public static class LightGBM
        {
            public const int NumberOfLeaves = 31;
            public const double LearningRate = 0.1;
            public const int MinimumExampleCountPerLeaf = 20;
            public const int NumberOfIterations = 100;
            public const double FeatureFraction = 0.8;
            public const double BaggingFraction = 0.8;
            public const int EarlyStoppingRounds = 10;
        }

        public static class Features
        {
            public const string InputColumnName = "Features";
            public const string RiskScoreLabelColumn = "RiskScore";
            public const string LoanApprovedLabelColumn = "LoanApproved";
        }

        public static class ModelPaths
        {
            public const string RiskScoreModelPath = "risk_score_model.zip";
            public const string LoanApprovalModelPath = "loan_approval_model.zip";
            public const string FeatureTransformerPath = "feature_transformer.zip";
        }
    }
}

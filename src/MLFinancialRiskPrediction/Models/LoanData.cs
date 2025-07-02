namespace MLFinancialRiskPrediction.Models
{
    using Microsoft.ML.Data;

    public class LoanData
    {
        [LoadColumn(0)]
        public string ApplicationDate { get; set; } = string.Empty;

        [LoadColumn(1)]
        public float Age { get; set; }

        [LoadColumn(2)]
        public float AnnualIncome { get; set; }

        [LoadColumn(3)]
        public float CreditScore { get; set; }

        [LoadColumn(4)]
        public string EmploymentStatus { get; set; } = string.Empty;

        [LoadColumn(5)]
        public string EducationLevel { get; set; } = string.Empty;

        [LoadColumn(6)]
        public float Experience { get; set; }

        [LoadColumn(7)]
        public float LoanAmount { get; set; }

        [LoadColumn(8)]
        public float LoanDuration { get; set; }

        [LoadColumn(9)]
        public string MaritalStatus { get; set; } = string.Empty;

        [LoadColumn(10)]
        public float NumberOfDependents { get; set; }

        [LoadColumn(11)]
        public string HomeOwnershipStatus { get; set; } = string.Empty;

        [LoadColumn(12)]
        public float MonthlyDebtPayments { get; set; }

        [LoadColumn(13)]
        public float CreditCardUtilizationRate { get; set; }

        [LoadColumn(14)]
        public float NumberOfOpenCreditLines { get; set; }

        [LoadColumn(15)]
        public float NumberOfCreditInquiries { get; set; }

        [LoadColumn(16)]
        public float DebtToIncomeRatio { get; set; }

        [LoadColumn(17)]
        public float BankruptcyHistory { get; set; }

        [LoadColumn(18)]
        public string LoanPurpose { get; set; } = string.Empty;

        [LoadColumn(19)]
        public float PreviousLoanDefaults { get; set; }

        [LoadColumn(20)]
        public float PaymentHistory { get; set; }

        [LoadColumn(21)]
        public float LengthOfCreditHistory { get; set; }

        [LoadColumn(22)]
        public float SavingsAccountBalance { get; set; }

        [LoadColumn(23)]
        public float CheckingAccountBalance { get; set; }

        [LoadColumn(24)]
        public float TotalAssets { get; set; }

        [LoadColumn(25)]
        public float TotalLiabilities { get; set; }

        [LoadColumn(26)]
        public float MonthlyIncome { get; set; }

        [LoadColumn(27)]
        public float UtilityBillsPaymentHistory { get; set; }

        [LoadColumn(28)]
        public float JobTenure { get; set; }

        [LoadColumn(29)]
        public float NetWorth { get; set; }

        [LoadColumn(30)]
        public float BaseInterestRate { get; set; }

        [LoadColumn(31)]
        public float InterestRate { get; set; }

        [LoadColumn(32)]
        public float MonthlyLoanPayment { get; set; }

        [LoadColumn(33)]
        public float TotalDebtToIncomeRatio { get; set; }

        [LoadColumn(34)]
        public bool LoanApproved { get; set; } = false;

        [LoadColumn(35)]
        public float RiskScore { get; set; } = -1;
    }
}
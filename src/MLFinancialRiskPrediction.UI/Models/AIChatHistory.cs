namespace MLFinancialRiskPrediction.UI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using MLFinancialRiskPrediction.UI.Services;
    public class CategoryOptions
    {
        public List<string> EmploymentStatuses { get; set; } = [];
        public List<string> EducationLevels { get; set; } = [];
        public List<string> MaritalStatuses { get; set; } = [];
        public List<string> HomeOwnershipStatuses { get; set; } = [];
        public List<string> LoanPurposes { get; set; } = [];
    }
    public class SinglePredictionResult
    {
        public float RiskScore { get; set; }
        public bool LoanApproved { get; set; }
        public float ApprovalProbability { get; set; }
        public float ApprovalScore { get; set; }
        public RiskCategory RiskCategory { get; set; }
        public string FormattedRiskScore => $"{this.RiskScore:F1}";
        public string FormattedApprovalProbability => $"{this.ApprovalProbability * 100:F1}%";
        public string ApprovalStatusText => this.LoanApproved ? "APPROVED" : "REJECTED";
        public string RiskCategoryText => this.RiskCategory.ToString().ToUpper();
        public string ApprovalStatusColor => this.LoanApproved ? "#FF4CAF50" : "#FFFF5722";
        public string RiskCategoryColor => this.RiskCategory switch
        {
            RiskCategory.Low => "#FF4CAF50",
            RiskCategory.Medium => "#FFFFC107",
            RiskCategory.High => "#FFFF9800",
            RiskCategory.VeryHigh => "#FFFF5722",
            RiskCategory.Critical => "#FF9C27B0",
            _ => "#FFCCCCCC"
        };
    }

    public enum RiskCategory
    {
        Low,
        Medium,
        High,
        VeryHigh,
        Critical
    }
    public class SinglePredictionRequest
    {
        public float Age { get; set; }
        public float AnnualIncome { get; set; }
        public float CreditScore { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public float Experience { get; set; }
        public float LoanAmount { get; set; }
        public float LoanDuration { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public float NumberOfDependents { get; set; }
        public string HomeOwnershipStatus { get; set; } = string.Empty;
        public float MonthlyDebtPayments { get; set; }
        public float CreditCardUtilizationRate { get; set; }
        public float NumberOfOpenCreditLines { get; set; }
        public float NumberOfCreditInquiries { get; set; }
        public float DebtToIncomeRatio { get; set; }
        public float BankruptcyHistory { get; set; }
        public string LoanPurpose { get; set; } = string.Empty;
        public float PreviousLoanDefaults { get; set; }
        public float PaymentHistory { get; set; }
        public float LengthOfCreditHistory { get; set; }
        public float SavingsAccountBalance { get; set; }
        public float CheckingAccountBalance { get; set; }
        public float TotalAssets { get; set; }
        public float TotalLiabilities { get; set; }
        public float MonthlyIncome { get; set; }
        public float UtilityBillsPaymentHistory { get; set; }
        public float JobTenure { get; set; }
        public float NetWorth { get; set; }
        public float BaseInterestRate { get; set; }
        public float InterestRate { get; set; }
        public float MonthlyLoanPayment { get; set; }
        public float TotalDebtToIncomeRatio { get; set; }
    }
    public class AIChatHistory
    {
        public AIChatHistory()
        {
            this.Messages = [];
        }

        public ObservableCollection<AIChatMessage> Messages { get; }

        public void AddMessage(AIChatMessage message)
        {
            this.Messages.Add(message);
        }

        public void AddMessage(string content, bool isFromUser, AIChatMessageType messageType = AIChatMessageType.Text)
        {
            AIChatMessage message = new AIChatMessage
            {
                Content = content,
                IsFromUser = isFromUser,
                Timestamp = DateTime.Now,
                MessageType = messageType
            };

            this.AddMessage(message);
        }

        public void Clear()
        {
            this.Messages.Clear();
        }

        public AIChatMessage? GetLastMessage()
        {
            return this.Messages.LastOrDefault();
        }

        public IEnumerable<AIChatMessage> GetUserMessages()
        {
            return this.Messages.Where(m => m.IsFromUser);
        }

        public IEnumerable<AIChatMessage> GetAIMessages()
        {
            return this.Messages.Where(m => !m.IsFromUser);
        }
    }
}

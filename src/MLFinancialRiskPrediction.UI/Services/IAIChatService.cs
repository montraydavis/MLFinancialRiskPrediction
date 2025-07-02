namespace MLFinancialRiskPrediction.UI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Connectors.OpenAI;

    using MLFinancialRiskPrediction.Core;
    using MLFinancialRiskPrediction.Models;
    using MLFinancialRiskPrediction.Services;
    using MLFinancialRiskPrediction.UI.Models;

    public class PredictionService : IPredictionService
    {
        public PredictionService(IEnsemblePredictor ensemblePredictor)
        {
            this._ensemblePredictor = ensemblePredictor;
        }

        private readonly IEnsemblePredictor _ensemblePredictor;

        public bool IsModelReady => this._ensemblePredictor != null;

        public SinglePredictionResult Predict(SinglePredictionRequest request)
        {
            LoanData loanData = this.MapToLoanData(request);
            EnsemblePrediction prediction = this._ensemblePredictor.Predict(loanData);

            // Normalize and clamp the risk score to 0-100 range
            float normalizedRiskScore = this.NormalizeRiskScore(prediction.RiskScore);

            // Ensure approval probability is between 0 and 1
            float normalizedApprovalProbability = Math.Max(0f, Math.Min(1f, prediction.ApprovalProbability));

            return new SinglePredictionResult
            {
                RiskScore = normalizedRiskScore,
                LoanApproved = prediction.LoanApproved,
                ApprovalProbability = normalizedApprovalProbability,
                ApprovalScore = prediction.ApprovalScore,
                RiskCategory = this.DetermineRiskCategory(normalizedRiskScore)
            };
        }

        private LoanData MapToLoanData(SinglePredictionRequest request)
        {
            return new LoanData
            {
                Age = request.Age,
                AnnualIncome = request.AnnualIncome,
                CreditScore = request.CreditScore,
                EmploymentStatus = request.EmploymentStatus,
                EducationLevel = request.EducationLevel,
                Experience = request.Experience,
                LoanAmount = request.LoanAmount,
                LoanDuration = request.LoanDuration,
                MaritalStatus = request.MaritalStatus,
                NumberOfDependents = request.NumberOfDependents,
                HomeOwnershipStatus = request.HomeOwnershipStatus,
                MonthlyDebtPayments = request.MonthlyDebtPayments,
                CreditCardUtilizationRate = request.CreditCardUtilizationRate,
                NumberOfOpenCreditLines = request.NumberOfOpenCreditLines,
                NumberOfCreditInquiries = request.NumberOfCreditInquiries,
                DebtToIncomeRatio = request.DebtToIncomeRatio,
                BankruptcyHistory = request.BankruptcyHistory,
                LoanPurpose = request.LoanPurpose,
                PreviousLoanDefaults = request.PreviousLoanDefaults,
                PaymentHistory = request.PaymentHistory,
                LengthOfCreditHistory = request.LengthOfCreditHistory,
                SavingsAccountBalance = request.SavingsAccountBalance,
                CheckingAccountBalance = request.CheckingAccountBalance,
                TotalAssets = request.TotalAssets,
                TotalLiabilities = request.TotalLiabilities,
                MonthlyIncome = request.MonthlyIncome,
                UtilityBillsPaymentHistory = request.UtilityBillsPaymentHistory,
                JobTenure = request.JobTenure,
                NetWorth = request.NetWorth,
                BaseInterestRate = request.BaseInterestRate,
                InterestRate = request.InterestRate,
                MonthlyLoanPayment = request.MonthlyLoanPayment,
                TotalDebtToIncomeRatio = request.TotalDebtToIncomeRatio
            };
        }

        private float NormalizeRiskScore(float rawRiskScore)
        {
            // Handle extreme values and normalize to 0-100 range
            if (float.IsNaN(rawRiskScore) || float.IsInfinity(rawRiskScore))
            {
                return 50f; // Default to medium risk if invalid
            }

            // If the raw score is already in reasonable range (0-100), use it
            if (rawRiskScore >= 0 && rawRiskScore <= 100)
            {
                return rawRiskScore;
            }

            // For very large positive values, use sigmoid normalization
            if (rawRiskScore > 100)
            {
                // Sigmoid function to map large values to 0-100
                float sigmoid = 1f / (1f + (float)Math.Exp(-rawRiskScore / 1000000f)); // Scale down large numbers
                return sigmoid * 100f;
            }

            // For negative values, map to lower risk scores
            if (rawRiskScore < 0)
            {
                // Map negative values to 0-50 range
                float normalizedNegative = (float)(50 * (1 - Math.Exp(rawRiskScore / 1000000f)));
                return Math.Max(0f, normalizedNegative);
            }

            return Math.Max(0f, Math.Min(100f, rawRiskScore));
        }

        private RiskCategory DetermineRiskCategory(float riskScore)
        {
            return riskScore switch
            {
                < 25 => RiskCategory.Low,
                < 45 => RiskCategory.Medium,
                < 65 => RiskCategory.High,
                < 80 => RiskCategory.VeryHigh,
                _ => RiskCategory.Critical
            };
        }
    }

    public interface IPredictionService
    {
        SinglePredictionResult Predict(SinglePredictionRequest request);
        bool IsModelReady { get; }
    }
    public class CategoryService : ICategoryService
    {
        public CategoryService(DataLoader dataLoader)
        {
            this._dataLoader = dataLoader;
        }

        private readonly DataLoader _dataLoader;

        public CategoryOptions GetCategoryOptions()
        {
            try
            {
                IEnumerable<LoanData> data = this._dataLoader.LoadData("loans.csv");

                return new CategoryOptions
                {
                    EmploymentStatuses = data
                        .Select(x => x.EmploymentStatus)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),

                    EducationLevels = data
                        .Select(x => x.EducationLevel)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),

                    MaritalStatuses = data
                        .Select(x => x.MaritalStatus)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),

                    HomeOwnershipStatuses = data
                        .Select(x => x.HomeOwnershipStatus)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),

                    LoanPurposes = data
                        .Select(x => x.LoanPurpose)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList()
                };
            }
            catch
            {
                return new CategoryOptions();
            }
        }
    }
    public interface ICategoryService
    {
        CategoryOptions GetCategoryOptions();
    }
    public interface IAIChatService
    {
        Task<AIChatMessage> SendMessageAsync(string message);
        Task<IEnumerable<AIChatMessage>> GetChatHistoryAsync();
        Task ClearHistoryAsync();
    }

    public class AIChatMessage
    {
        public string Content { get; set; } = string.Empty;
        public bool IsFromUser { get; set; }
        public DateTime Timestamp { get; set; }
        public AIChatMessageType MessageType { get; set; }
    }

    public enum AIChatMessageType
    {
        Text,
        Error,
        System,
        Data
    }

    public class AIChatService : IAIChatService
    {
        public AIChatService(Kernel kernel)
        {
            this._kernel = kernel;
            this._chatHistory = [];
            this._kernelArguments = new KernelArguments(new OpenAIPromptExecutionSettings
            {
                ModelId = "gpt-4o-mini",
                Temperature = 0.3,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            });
        }

        #region Private Fields
        private readonly Kernel _kernel;
        private readonly List<AIChatMessage> _chatHistory;
        private readonly KernelArguments _kernelArguments;
        #endregion

        public async Task<AIChatMessage> SendMessageAsync(string message)
        {
            AIChatMessage userMessage = new AIChatMessage
            {
                Content = message,
                IsFromUser = true,
                Timestamp = DateTime.Now,
                MessageType = AIChatMessageType.Text
            };

            this._chatHistory.Add(userMessage);

            try
            {
                FunctionResult response = await this._kernel.InvokePromptAsync(message, this._kernelArguments);
                string aiResponse = response.GetValue<string>() ?? "I apologize, but I couldn't process your request at this time.";

                AIChatMessage aiMessage = new AIChatMessage
                {
                    Content = aiResponse,
                    IsFromUser = false,
                    Timestamp = DateTime.Now,
                    MessageType = AIChatMessageType.Text
                };

                this._chatHistory.Add(aiMessage);
                return aiMessage;
            }
            catch (Exception ex)
            {
                AIChatMessage errorMessage = new AIChatMessage
                {
                    Content = $"Error: {ex.Message}",
                    IsFromUser = false,
                    Timestamp = DateTime.Now,
                    MessageType = AIChatMessageType.Error
                };

                this._chatHistory.Add(errorMessage);
                return errorMessage;
            }
        }

        public Task<IEnumerable<AIChatMessage>> GetChatHistoryAsync()
        {
            return Task.FromResult<IEnumerable<AIChatMessage>>(this._chatHistory.AsReadOnly());
        }

        public Task ClearHistoryAsync()
        {
            this._chatHistory.Clear();
            return Task.CompletedTask;
        }
    }
}

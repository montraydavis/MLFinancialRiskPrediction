namespace MLFinancialRiskPrediction.UI.ViewModeels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using MLFinancialRiskPrediction.UI.Models;
    using MLFinancialRiskPrediction.UI.Services;

    public class ChatViewModel : INotifyPropertyChanged
    {
        public ChatViewModel(IAIChatService chatService)
        {
            this._chatService = chatService;
            this.ChatHistory = new AIChatHistory();
            this.SendMessageCommand = new RelayCommand(this.SendMessage, this.CanSendMessage);
            this.ClearHistoryCommand = new RelayCommand(this.ClearHistory);
            this.PopulateSampleDataCommand = new RelayCommand(this.PopulateSampleData, this.CanPopulateSampleData);
            this.ExplainResultCommand = new RelayCommand(this.ExplainResult, this.CanExplainResult);

            this.InitializeWelcomeMessage();
        }

        #region Private Fields
        private readonly IAIChatService _chatService;
        private string _currentMessage = string.Empty;
        private bool _isSending;
        private SinglePredictionViewModel? _linkedPredictionViewModel;
        #endregion

        #region Properties
        public AIChatHistory ChatHistory { get; }

        public string CurrentMessage
        {
            get => this._currentMessage;
            set
            {
                this._currentMessage = value;
                this.OnPropertyChanged();
                this.SendMessageCommand.NotifyCanExecuteChanged();
            }
        }

        public bool IsSending
        {
            get => this._isSending;
            private set
            {
                this._isSending = value;
                this.OnPropertyChanged();
                this.SendMessageCommand.NotifyCanExecuteChanged();
            }
        }

        public SinglePredictionViewModel? LinkedPredictionViewModel
        {
            get => this._linkedPredictionViewModel;
            set
            {
                if (this._linkedPredictionViewModel != null)
                {
                    this._linkedPredictionViewModel.PropertyChanged -= this.OnPredictionViewModelPropertyChanged;
                }

                this._linkedPredictionViewModel = value;

                if (this._linkedPredictionViewModel != null)
                {
                    this._linkedPredictionViewModel.PropertyChanged += this.OnPredictionViewModelPropertyChanged;
                }

                this.OnPropertyChanged();
                this.PopulateSampleDataCommand.NotifyCanExecuteChanged();
                this.ExplainResultCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand SendMessageCommand { get; }
        public RelayCommand ClearHistoryCommand { get; }
        public RelayCommand PopulateSampleDataCommand { get; }
        public RelayCommand ExplainResultCommand { get; }
        #endregion

        #region Private Methods
        private async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(this.CurrentMessage) || this.IsSending)
            {
                return;
            }

            string messageToSend = this.CurrentMessage.Trim();
            this.CurrentMessage = string.Empty;
            this.IsSending = true;

            try
            {
                this.ChatHistory.AddMessage(messageToSend, isFromUser: true);

                // Check for special commands
                if (this.ProcessSpecialCommands(messageToSend))
                {
                    return;
                }

                AIChatMessage response = await this._chatService.SendMessageAsync(messageToSend);
                this.ChatHistory.AddMessage(response);
            }
            catch (Exception ex)
            {
                this.ChatHistory.AddMessage($"Error: {ex.Message}", isFromUser: false, AIChatMessageType.Error);
            }
            finally
            {
                this.IsSending = false;
            }
        }

        private bool CanSendMessage()
        {
            return !string.IsNullOrWhiteSpace(this.CurrentMessage) && !this.IsSending;
        }

        private async void ClearHistory()
        {
            this.ChatHistory.Clear();
            await this._chatService.ClearHistoryAsync();
            this.InitializeWelcomeMessage();
        }

        private void PopulateSampleData()
        {
            if (this.LinkedPredictionViewModel == null)
            {
                return;
            }

            SinglePredictionViewModel vm = this.LinkedPredictionViewModel;

            // High-risk scenario
            vm.Age = 22;
            vm.AnnualIncome = 28000;
            vm.CreditScore = 580;
            vm.Experience = 1;
            vm.LoanAmount = 45000;
            vm.LoanDuration = 72;
            vm.NumberOfDependents = 2;
            vm.MonthlyDebtPayments = 800;
            vm.CreditCardUtilizationRate = 85;
            vm.NumberOfOpenCreditLines = 8;
            vm.NumberOfCreditInquiries = 5;
            vm.BankruptcyHistory = 1;
            vm.PreviousLoanDefaults = 2;
            vm.PaymentHistory = 65;
            vm.LengthOfCreditHistory = 3;
            vm.SavingsAccountBalance = 500;
            vm.CheckingAccountBalance = 200;
            vm.UtilityBillsPaymentHistory = 70;
            vm.JobTenure = 0.5f;
            vm.InterestRate = 8.5f;

            if (vm.EmploymentStatuses.Contains("Part-time"))
            {
                vm.SelectedEmploymentStatus = "Part-time";
            }

            if (vm.EducationLevels.Contains("High School"))
            {
                vm.SelectedEducationLevel = "High School";
            }

            if (vm.MaritalStatuses.Contains("Single"))
            {
                vm.SelectedMaritalStatus = "Single";
            }

            if (vm.HomeOwnershipStatuses.Contains("Rent"))
            {
                vm.SelectedHomeOwnershipStatus = "Rent";
            }

            if (vm.LoanPurposes.Contains("Personal"))
            {
                vm.SelectedLoanPurpose = "Personal";
            }

            this.ChatHistory.AddMessage(
                "I've populated the form with a high-risk loan scenario. This applicant has limited income, poor credit history, and high debt utilization.",
                isFromUser: false,
                AIChatMessageType.System
            );
        }

        private bool CanPopulateSampleData()
        {
            return this.LinkedPredictionViewModel != null && !this.IsSending;
        }

        private void ExplainResult()
        {
            if (this.LinkedPredictionViewModel?.PredictionResult == null)
            {
                return;
            }

            SinglePredictionResult result = this.LinkedPredictionViewModel.PredictionResult;
            string explanation = this.GenerateResultExplanation(result);

            this.ChatHistory.AddMessage(
                explanation,
                isFromUser: false,
                AIChatMessageType.System
            );
        }

        private bool CanExplainResult()
        {
            return this.LinkedPredictionViewModel?.PredictionResult != null && !this.IsSending;
        }

        private bool ProcessSpecialCommands(string message)
        {
            string lowerMessage = message.ToLower();

            return false;
        }

        private string GenerateResultExplanation(SinglePredictionResult result)
        {
            string explanation = $"**Prediction Analysis Results:**\n\n";
            explanation += $"🎯 **Risk Score:** {result.FormattedRiskScore} ({result.RiskCategoryText})\n";
            explanation += $"📊 **Loan Decision:** {result.ApprovalStatusText}\n";
            explanation += $"🔍 **Confidence:** {result.FormattedApprovalProbability}\n\n";

            explanation += "**Risk Assessment:**\n";
            switch (result.RiskCategory)
            {
                case RiskCategory.Low:
                    explanation += "✅ This applicant presents low risk with strong financial indicators.";
                    break;
                case RiskCategory.Medium:
                    explanation += "⚠️ Moderate risk level - some areas of concern but generally acceptable.";
                    break;
                case RiskCategory.High:
                    explanation += "🔶 High risk applicant - careful consideration required.";
                    break;
                case RiskCategory.VeryHigh:
                    explanation += "🔴 Very high risk - significant concerns identified.";
                    break;
                case RiskCategory.Critical:
                    explanation += "⛔ Critical risk level - approval not recommended.";
                    break;
            }

            explanation += "\n\n**Key Factors to Consider:**\n";
            explanation += "• Credit score and payment history\n";
            explanation += "• Debt-to-income ratio\n";
            explanation += "• Employment stability\n";
            explanation += "• Previous loan defaults\n";
            explanation += "• Available assets and savings";

            return explanation;
        }

        private void OnPredictionViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SinglePredictionViewModel.PredictionResult))
            {
                this.ExplainResultCommand.NotifyCanExecuteChanged();

                // Auto-explain results when prediction completes
                if (this.LinkedPredictionViewModel?.PredictionResult != null)
                {
                    SinglePredictionResult result = this.LinkedPredictionViewModel.PredictionResult;
                    string autoMessage = $"Prediction completed! Risk Score: {result.FormattedRiskScore}, Status: {result.ApprovalStatusText}. Would you like a detailed explanation?";

                    this.ChatHistory.AddMessage(
                        autoMessage,
                        isFromUser: false,
                        AIChatMessageType.System
                    );
                }
            }
        }

        private void InitializeWelcomeMessage()
        {
            this.ChatHistory.AddMessage(
                "Hello! I can help you analyze loan applications and explain risk scores. Try commands like:\n" +
                "• 'Show me a sample' - Load example data\n" +
                "• 'Explain result' - Analyze current prediction\n" +
                "• 'Reset form' - Clear all inputs\n" +
                "• 'Predict' - Run analysis\n\n" +
                "What would you like to know?",
                isFromUser: false,
                AIChatMessageType.System
            );
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class RelayCommand : ICommand
    {
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        #region Private Fields
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;
        #endregion

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return this._canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            this._execute();
        }

        public void NotifyCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
namespace MLFinancialRiskPrediction.UI.ViewModeels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using MLFinancialRiskPrediction.UI.Models;
    using MLFinancialRiskPrediction.UI.Services;

    public class SinglePredictionViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public SinglePredictionViewModel(IPredictionService predictionService, ICategoryService categoryService)
        {
            this._predictionService = predictionService;
            this._categoryService = categoryService;

            this.InitializeCategories();
            this.InitializeCommands();
            this.ResetForm();
        }

        #region Private Fields
        private readonly IPredictionService _predictionService;
        private readonly ICategoryService _categoryService;
        private bool _isPredicting;
        private SinglePredictionResult? _predictionResult;
        private bool _isUpdatingCalculatedFields; // Add this flag

        // Input Fields
        private float _age = 35;
        private float _annualIncome = 50000;
        private float _creditScore = 650;
        private string _selectedEmploymentStatus = string.Empty;
        private string _selectedEducationLevel = string.Empty;
        private float _experience = 5;
        private float _loanAmount = 25000;
        private float _loanDuration = 36;
        private string _selectedMaritalStatus = string.Empty;
        private float _numberOfDependents = 0;
        private string _selectedHomeOwnershipStatus = string.Empty;
        private float _monthlyDebtPayments = 500;
        private float _creditCardUtilizationRate = 30;
        private float _numberOfOpenCreditLines = 3;
        private float _numberOfCreditInquiries = 1;
        private float _debtToIncomeRatio = 0.3f;
        private float _bankruptcyHistory = 0;
        private string _selectedLoanPurpose = string.Empty;
        private float _previousLoanDefaults = 0;
        private float _paymentHistory = 95;
        private float _lengthOfCreditHistory = 10;
        private float _savingsAccountBalance = 5000;
        private float _checkingAccountBalance = 2000;
        private float _totalAssets = 50000;
        private float _totalLiabilities = 15000;
        private float _monthlyIncome = 4167;
        private float _utilityBillsPaymentHistory = 98;
        private float _jobTenure = 3;
        private float _netWorth = 35000;
        private float _baseInterestRate = 3.5f;
        private float _interestRate = 5.5f;
        private float _monthlyLoanPayment = 750;
        private float _totalDebtToIncomeRatio = 0.35f;
        #endregion

        #region Properties
        public bool IsPredicting
        {
            get => this._isPredicting;
            private set
            {
                this._isPredicting = value;
                this.OnPropertyChanged();
                this.PredictCommand.NotifyCanExecuteChanged();
            }
        }

        public SinglePredictionResult? PredictionResult
        {
            get => this._predictionResult;
            private set
            {
                this._predictionResult = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.HasResult));
            }
        }

        public bool HasResult => this.PredictionResult != null;

        // Input Properties
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public float Age
        {
            get => this._age;
            set
            {
                this._age = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        [Range(1000, 10000000, ErrorMessage = "Annual income must be between $1,000 and $10,000,000")]
        public float AnnualIncome
        {
            get => this._annualIncome;
            set
            {
                this._annualIncome = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        [Range(300, 850, ErrorMessage = "Credit score must be between 300 and 850")]
        public float CreditScore
        {
            get => this._creditScore;
            set
            {
                this._creditScore = value;
                this.OnPropertyChanged();
            }
        }

        public string SelectedEmploymentStatus
        {
            get => this._selectedEmploymentStatus;
            set
            {
                this._selectedEmploymentStatus = value;
                this.OnPropertyChanged();
            }
        }

        public string SelectedEducationLevel
        {
            get => this._selectedEducationLevel;
            set
            {
                this._selectedEducationLevel = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
        public float Experience
        {
            get => this._experience;
            set
            {
                this._experience = value;
                this.OnPropertyChanged();
            }
        }

        [Range(1000, 5000000, ErrorMessage = "Loan amount must be between $1,000 and $5,000,000")]
        public float LoanAmount
        {
            get => this._loanAmount;
            set
            {
                this._loanAmount = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        [Range(6, 360, ErrorMessage = "Loan duration must be between 6 and 360 months")]
        public float LoanDuration
        {
            get => this._loanDuration;
            set
            {
                this._loanDuration = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        public string SelectedMaritalStatus
        {
            get => this._selectedMaritalStatus;
            set
            {
                this._selectedMaritalStatus = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 20, ErrorMessage = "Number of dependents must be between 0 and 20")]
        public float NumberOfDependents
        {
            get => this._numberOfDependents;
            set
            {
                this._numberOfDependents = value;
                this.OnPropertyChanged();
            }
        }

        public string SelectedHomeOwnershipStatus
        {
            get => this._selectedHomeOwnershipStatus;
            set
            {
                this._selectedHomeOwnershipStatus = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 50000, ErrorMessage = "Monthly debt payments must be between $0 and $50,000")]
        public float MonthlyDebtPayments
        {
            get => this._monthlyDebtPayments;
            set
            {
                this._monthlyDebtPayments = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        [Range(0, 100, ErrorMessage = "Credit card utilization rate must be between 0% and 100%")]
        public float CreditCardUtilizationRate
        {
            get => this._creditCardUtilizationRate;
            set
            {
                this._creditCardUtilizationRate = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 50, ErrorMessage = "Number of open credit lines must be between 0 and 50")]
        public float NumberOfOpenCreditLines
        {
            get => this._numberOfOpenCreditLines;
            set
            {
                this._numberOfOpenCreditLines = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 20, ErrorMessage = "Number of credit inquiries must be between 0 and 20")]
        public float NumberOfCreditInquiries
        {
            get => this._numberOfCreditInquiries;
            set
            {
                this._numberOfCreditInquiries = value;
                this.OnPropertyChanged();
            }
        }

        public float DebtToIncomeRatio
        {
            get => this._debtToIncomeRatio;
            set
            {
                this._debtToIncomeRatio = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 5, ErrorMessage = "Bankruptcy history must be between 0 and 5")]
        public float BankruptcyHistory
        {
            get => this._bankruptcyHistory;
            set
            {
                this._bankruptcyHistory = value;
                this.OnPropertyChanged();
            }
        }

        public string SelectedLoanPurpose
        {
            get => this._selectedLoanPurpose;
            set
            {
                this._selectedLoanPurpose = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 10, ErrorMessage = "Previous loan defaults must be between 0 and 10")]
        public float PreviousLoanDefaults
        {
            get => this._previousLoanDefaults;
            set
            {
                this._previousLoanDefaults = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 100, ErrorMessage = "Payment history must be between 0% and 100%")]
        public float PaymentHistory
        {
            get => this._paymentHistory;
            set
            {
                this._paymentHistory = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 50, ErrorMessage = "Length of credit history must be between 0 and 50 years")]
        public float LengthOfCreditHistory
        {
            get => this._lengthOfCreditHistory;
            set
            {
                this._lengthOfCreditHistory = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 10000000, ErrorMessage = "Savings account balance must be between $0 and $10,000,000")]
        public float SavingsAccountBalance
        {
            get => this._savingsAccountBalance;
            set
            {
                this._savingsAccountBalance = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        [Range(0, 1000000, ErrorMessage = "Checking account balance must be between $0 and $1,000,000")]
        public float CheckingAccountBalance
        {
            get => this._checkingAccountBalance;
            set
            {
                this._checkingAccountBalance = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        public float TotalAssets
        {
            get => this._totalAssets;
            set
            {
                this._totalAssets = value;
                this.OnPropertyChanged();
                if (!this._isUpdatingCalculatedFields)
                {
                    this.UpdateCalculatedFields();
                }
            }
        }

        public float TotalLiabilities
        {
            get => this._totalLiabilities;
            set
            {
                this._totalLiabilities = value;
                this.OnPropertyChanged();
                if (!this._isUpdatingCalculatedFields)
                {
                    this.UpdateCalculatedFields();
                }
            }
        }

        public float MonthlyIncome
        {
            get => this._monthlyIncome;
            set
            {
                this._monthlyIncome = value;
                this.OnPropertyChanged();
                if (!this._isUpdatingCalculatedFields)
                {
                    this.UpdateCalculatedFields();
                }
            }
        }

        [Range(0, 100, ErrorMessage = "Utility bills payment history must be between 0% and 100%")]
        public float UtilityBillsPaymentHistory
        {
            get => this._utilityBillsPaymentHistory;
            set
            {
                this._utilityBillsPaymentHistory = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 50, ErrorMessage = "Job tenure must be between 0 and 50 years")]
        public float JobTenure
        {
            get => this._jobTenure;
            set
            {
                this._jobTenure = value;
                this.OnPropertyChanged();
            }
        }

        public float NetWorth
        {
            get => this._netWorth;
            set
            {
                this._netWorth = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 20, ErrorMessage = "Base interest rate must be between 0% and 20%")]
        public float BaseInterestRate
        {
            get => this._baseInterestRate;
            set
            {
                this._baseInterestRate = value;
                this.OnPropertyChanged();
            }
        }

        [Range(0, 30, ErrorMessage = "Interest rate must be between 0% and 30%")]
        public float InterestRate
        {
            get => this._interestRate;
            set
            {
                this._interestRate = value;
                this.OnPropertyChanged();
                this.UpdateCalculatedFields();
            }
        }

        public float MonthlyLoanPayment
        {
            get => this._monthlyLoanPayment;
            set
            {
                this._monthlyLoanPayment = value;
                this.OnPropertyChanged();
            }
        }

        public float TotalDebtToIncomeRatio
        {
            get => this._totalDebtToIncomeRatio;
            set
            {
                this._totalDebtToIncomeRatio = value;
                this.OnPropertyChanged();
            }
        }

        // Category Collections
        public ObservableCollection<string> EmploymentStatuses { get; } = [];
        public ObservableCollection<string> EducationLevels { get; } = [];
        public ObservableCollection<string> MaritalStatuses { get; } = [];
        public ObservableCollection<string> HomeOwnershipStatuses { get; } = [];
        public ObservableCollection<string> LoanPurposes { get; } = [];
        #endregion

        #region Commands
        public RelayCommand PredictCommand { get; private set; } = null!;
        public RelayCommand ResetCommand { get; private set; } = null!;
        public RelayCommand ClearResultCommand { get; private set; } = null!;
        #endregion

        #region Private Methods
        private void InitializeCategories()
        {
            CategoryOptions options = this._categoryService.GetCategoryOptions();

            foreach (string status in options.EmploymentStatuses)
            {
                this.EmploymentStatuses.Add(status);
            }

            foreach (string level in options.EducationLevels)
            {
                this.EducationLevels.Add(level);
            }

            foreach (string status in options.MaritalStatuses)
            {
                this.MaritalStatuses.Add(status);
            }

            foreach (string status in options.HomeOwnershipStatuses)
            {
                this.HomeOwnershipStatuses.Add(status);
            }

            foreach (string purpose in options.LoanPurposes)
            {
                this.LoanPurposes.Add(purpose);
            }
        }

        private void InitializeCommands()
        {
            this.PredictCommand = new RelayCommand(this.Predict, this.CanPredict);
            this.ResetCommand = new RelayCommand(this.ResetForm);
            this.ClearResultCommand = new RelayCommand(this.ClearResult);
        }

        private async void Predict()
        {
            if (!this._predictionService.IsModelReady)
            {
                return;
            }

            this.IsPredicting = true;
            this.PredictionResult = null;

            try
            {
                SinglePredictionRequest request = this.CreatePredictionRequest();
                SinglePredictionResult result = await Task.Run(() => this._predictionService.Predict(request));
                this.PredictionResult = result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Prediction error: {ex.Message}");
            }
            finally
            {
                this.IsPredicting = false;
            }
        }

        private bool CanPredict()
        {
            return !this.IsPredicting &&
                   this._predictionService.IsModelReady &&
                   !string.IsNullOrWhiteSpace(this.SelectedEmploymentStatus) &&
                   !string.IsNullOrWhiteSpace(this.SelectedEducationLevel) &&
                   !string.IsNullOrWhiteSpace(this.SelectedMaritalStatus) &&
                   !string.IsNullOrWhiteSpace(this.SelectedHomeOwnershipStatus) &&
                   !string.IsNullOrWhiteSpace(this.SelectedLoanPurpose);
        }

        private void ResetForm()
        {
            this.Age = 35;
            this.AnnualIncome = 50000;
            this.CreditScore = 650;
            this.SelectedEmploymentStatus = this.EmploymentStatuses.FirstOrDefault() ?? string.Empty;
            this.SelectedEducationLevel = this.EducationLevels.FirstOrDefault() ?? string.Empty;
            this.Experience = 5;
            this.LoanAmount = 25000;
            this.LoanDuration = 36;
            this.SelectedMaritalStatus = this.MaritalStatuses.FirstOrDefault() ?? string.Empty;
            this.NumberOfDependents = 0;
            this.SelectedHomeOwnershipStatus = this.HomeOwnershipStatuses.FirstOrDefault() ?? string.Empty;
            this.MonthlyDebtPayments = 500;
            this.CreditCardUtilizationRate = 30;
            this.NumberOfOpenCreditLines = 3;
            this.NumberOfCreditInquiries = 1;
            this.BankruptcyHistory = 0;
            this.SelectedLoanPurpose = this.LoanPurposes.FirstOrDefault() ?? string.Empty;
            this.PreviousLoanDefaults = 0;
            this.PaymentHistory = 95;
            this.LengthOfCreditHistory = 10;
            this.SavingsAccountBalance = 5000;
            this.CheckingAccountBalance = 2000;
            this.UtilityBillsPaymentHistory = 98;
            this.JobTenure = 3;
            this.BaseInterestRate = 3.5f;
            this.InterestRate = 5.5f;

            this.UpdateCalculatedFields();
            this.ClearResult();
        }

        private void ClearResult()
        {
            this.PredictionResult = null;
        }

        private void UpdateCalculatedFields()
        {
            if (this._isUpdatingCalculatedFields)
            {
                return; // Prevent recursion
            }

            this._isUpdatingCalculatedFields = true;

            try
            {
                this.MonthlyIncome = this.AnnualIncome / 12;
                this.TotalAssets = this.SavingsAccountBalance + this.CheckingAccountBalance + (this.LoanAmount * 0.1f);
                this.TotalLiabilities = this.MonthlyDebtPayments * 12;
                this.NetWorth = this.TotalAssets - this.TotalLiabilities;
                this.DebtToIncomeRatio = this.MonthlyIncome > 0 ? this.MonthlyDebtPayments / this.MonthlyIncome : 0;
                this.TotalDebtToIncomeRatio = this.DebtToIncomeRatio;

                if (this.LoanDuration > 0 && this.InterestRate > 0)
                {
                    float monthlyRate = this.InterestRate / 100 / 12;
                    float payments = this.LoanDuration;
                    this.MonthlyLoanPayment = this.LoanAmount * (monthlyRate * (float)Math.Pow(1 + monthlyRate, payments)) /
                                             ((float)Math.Pow(1 + monthlyRate, payments) - 1);
                }
            }
            finally
            {
                this._isUpdatingCalculatedFields = false;
            }
        }

        private SinglePredictionRequest CreatePredictionRequest()
        {
            return new SinglePredictionRequest
            {
                Age = this.Age,
                AnnualIncome = this.AnnualIncome,
                CreditScore = this.CreditScore,
                EmploymentStatus = this.SelectedEmploymentStatus,
                EducationLevel = this.SelectedEducationLevel,
                Experience = this.Experience,
                LoanAmount = this.LoanAmount,
                LoanDuration = this.LoanDuration,
                MaritalStatus = this.SelectedMaritalStatus,
                NumberOfDependents = this.NumberOfDependents,
                HomeOwnershipStatus = this.SelectedHomeOwnershipStatus,
                MonthlyDebtPayments = this.MonthlyDebtPayments,
                CreditCardUtilizationRate = this.CreditCardUtilizationRate,
                NumberOfOpenCreditLines = this.NumberOfOpenCreditLines,
                NumberOfCreditInquiries = this.NumberOfCreditInquiries,
                DebtToIncomeRatio = this.DebtToIncomeRatio,
                BankruptcyHistory = this.BankruptcyHistory,
                LoanPurpose = this.SelectedLoanPurpose,
                PreviousLoanDefaults = this.PreviousLoanDefaults,
                PaymentHistory = this.PaymentHistory,
                LengthOfCreditHistory = this.LengthOfCreditHistory,
                SavingsAccountBalance = this.SavingsAccountBalance,
                CheckingAccountBalance = this.CheckingAccountBalance,
                TotalAssets = this.TotalAssets,
                TotalLiabilities = this.TotalLiabilities,
                MonthlyIncome = this.MonthlyIncome,
                UtilityBillsPaymentHistory = this.UtilityBillsPaymentHistory,
                JobTenure = this.JobTenure,
                NetWorth = this.NetWorth,
                BaseInterestRate = this.BaseInterestRate,
                InterestRate = this.InterestRate,
                MonthlyLoanPayment = this.MonthlyLoanPayment,
                TotalDebtToIncomeRatio = this.TotalDebtToIncomeRatio
            };
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDataErrorInfo
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                System.Reflection.PropertyInfo? property = this.GetType().GetProperty(columnName);
                if (property != null)
                {
                    object[] validationAttributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);
                    foreach (ValidationAttribute attribute in validationAttributes)
                    {
                        if (!attribute.IsValid(property.GetValue(this)))
                        {
                            return attribute.ErrorMessage ?? $"Invalid value for {columnName}";
                        }
                    }
                }
                return string.Empty;
            }
        }
        #endregion
    }
}
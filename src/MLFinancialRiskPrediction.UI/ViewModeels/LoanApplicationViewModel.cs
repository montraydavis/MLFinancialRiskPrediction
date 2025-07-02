namespace MLFinancialRiskPrediction.UI.ViewModeels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using MLFinancialRiskPrediction.Models;
    using MLFinancialRiskPrediction.Services;

    public class LoanApplicationViewModel : INotifyPropertyChanged
    {
        public LoanApplicationViewModel(DataLoader dataLoader)
        {
            this._dataLoader = dataLoader;
            this.RecentApplications = [];

            // Fire and forget - but properly handle exceptions
            _ = this.LoadRecentApplicationsAsync();
        }

        #region Private Fields
        private readonly DataLoader _dataLoader;
        private bool _isLoading;
        #endregion

        #region Properties
        public ObservableCollection<LoanApplicationSummary> RecentApplications { get; }

        public bool IsLoading
        {
            get => this._isLoading;
            private set
            {
                this._isLoading = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Private Methods
        private async Task LoadRecentApplicationsAsync()
        {
            this.IsLoading = true;

            try
            {
                // Run data loading on background thread
                List<LoanApplicationSummary> recentApps = await Task.Run(() =>
                {
                    // Load data from your existing configuration
                    IEnumerable<LoanData> allLoans = this._dataLoader.LoadData("loans.csv");

                    // Parse and sort by actual ApplicationDate, then take most recent 10
                    return allLoans
                        .Where(loan => !string.IsNullOrEmpty(loan.ApplicationDate))
                        .Select(loan => new
                        {
                            Loan = loan,
                            ParsedDate = this.TryParseApplicationDate(loan.ApplicationDate)
                        })
                        .Where(x => x.ParsedDate.HasValue)
                        .OrderByDescending(x => x.ParsedDate.Value) // Most recent first
                        .Take(50)
                        .Select(x => new LoanApplicationSummary
                        {
                            ApplicantName = this.GenerateRandomName(),
                            LoanAmount = x.Loan.LoanAmount,
                            RiskScore = x.Loan.RiskScore,
                            IsApproved = x.Loan.LoanApproved,
                            ApplicationTime = x.ParsedDate.Value,
                            Status = x.Loan.LoanApproved ? LoanStatus.Approved :
                                    x.Loan.RiskScore > 60 ? LoanStatus.Rejected : LoanStatus.Pending
                        })
                        .ToList();
                }).ConfigureAwait(false);

                // Explicitly marshal to UI thread for ObservableCollection updates
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    this.RecentApplications.Clear();
                    foreach (LoanApplicationSummary app in recentApps)
                    {
                        this.RecentApplications.Add(app);
                    }
                });
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Loan data file not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Access denied loading loan data: {ex.Message}");
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"IO error loading loan data: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error loading recent applications: {ex.Message}");
            }
            finally
            {
                // IsLoading update also needs to be on UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    this.IsLoading = false;
                });
            }
        }

        private DateTime? TryParseApplicationDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            // Try common date formats
            string[] formats = {
                "yyyy-MM-dd",        // 2024-01-15
                "MM/dd/yyyy",        // 01/15/2024
                "dd/MM/yyyy",        // 15/01/2024
                "yyyy/MM/dd",        // 2024/01/15
                "MM-dd-yyyy",        // 01-15-2024
                "dd-MM-yyyy",        // 15-01-2024
                "yyyy.MM.dd",        // 2024.01.15
                "dd.MM.yyyy"         // 15.01.2024
            };

            foreach (string format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }

            // Try general parsing as fallback
            if (DateTime.TryParse(dateString, out DateTime generalResult))
            {
                return generalResult;
            }

            System.Diagnostics.Debug.WriteLine($"Could not parse date: {dateString}");
            return null;
        }

        private string GenerateRandomName()
        {
            string[] firstNames = { "John", "Sarah", "Mike", "Emma", "David", "Lisa", "Robert", "Maria", "James", "Anna" };
            string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson" };

            return $"{firstNames[Random.Shared.Next(firstNames.Length)]} {lastNames[Random.Shared.Next(lastNames.Length)]}";
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

    public class LoanApplicationSummary
    {
        public string ApplicantName { get; set; } = string.Empty;
        public float LoanAmount { get; set; }
        public float RiskScore { get; set; }
        public bool IsApproved { get; set; }
        public DateTime ApplicationTime { get; set; }
        public LoanStatus Status { get; set; }

        public string FormattedAmount => $"${this.LoanAmount:N0}";
        public string FormattedRiskScore => $"{this.RiskScore:F1}";
        public string TimeAgo => this.GetTimeAgo();
        public string StatusText => this.Status.ToString().ToUpper();
        public string StatusColor => this.Status switch
        {
            LoanStatus.Approved => "#FF4CAF50",
            LoanStatus.Rejected => "#FFFF5722",
            LoanStatus.Pending => "#FFFFC107",
            _ => "#FFCCCCCC"
        };

        private string GetTimeAgo()
        {
            TimeSpan timeDiff = DateTime.Now - this.ApplicationTime;

            if (timeDiff.TotalDays < 0)
            {
                // Future date - should not happen but handle gracefully
                return "Future date";
            }

            if (timeDiff.TotalMinutes < 1)
            {
                return "Just now";
            }

            if (timeDiff.TotalMinutes < 60)
            {
                return $"{(int)timeDiff.TotalMinutes} min ago";
            }

            if (timeDiff.TotalHours < 24)
            {
                return $"{(int)timeDiff.TotalHours} hour{((int)timeDiff.TotalHours != 1 ? "s" : "")} ago";
            }

            if (timeDiff.TotalDays < 30)
            {
                return $"{(int)timeDiff.TotalDays} day{((int)timeDiff.TotalDays != 1 ? "s" : "")} ago";
            }

            if (timeDiff.TotalDays < 365)
            {
                int months = (int)(timeDiff.TotalDays / 30);
                return $"{months} month{(months != 1 ? "s" : "")} ago";
            }

            int years = (int)(timeDiff.TotalDays / 365);
            return $"{years} year{(years != 1 ? "s" : "")} ago";
        }
    }

    public enum LoanStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
namespace MLFinancialRiskPrediction.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using MLFinancialRiskPrediction.UI.ViewModeels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindow_Loaded;
        }

        public ChatViewModel? ChatViewModel { get; private set; }
        public LoanApplicationViewModel? LoanApplicationViewModel { get; private set; }
        public SinglePredictionViewModel? SinglePredictionViewModel { get; private set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            try
            {
                this.ChatViewModel = App.GetRequiredService<ChatViewModel>();
                this.LoanApplicationViewModel = App.GetRequiredService<LoanApplicationViewModel>();
                this.SinglePredictionViewModel = App.GetRequiredService<SinglePredictionViewModel>();

                // Link chat and prediction viewmodels for integration
                this.ChatViewModel.LinkedPredictionViewModel = this.SinglePredictionViewModel;

                // Set ViewModels for controls
                this.ChatControlInstance.ViewModel = this.ChatViewModel;
                this.SinglePredictionControlInstance.ViewModel = this.SinglePredictionViewModel;
                this.ResultDisplayControlInstance.ViewModel = this.SinglePredictionViewModel;

                this.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize dashboard: {ex.Message}", "Initialization Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string section)
            {
                this.NavigateToSection(section);
            }
        }

        private void NavigateToSection(string section)
        {
            switch (section)
            {
                case "Prediction":
                    // Already on prediction view - focus the form
                    this.SinglePredictionControlInstance.Focus();
                    break;
                case "Dashboard":
                    this.SendChatMessage("Show me an overview of recent loan applications and risk metrics");
                    break;
                case "Analytics":
                    this.SendChatMessage("Show me detailed analytics and trends for loan approvals");
                    break;
                case "Chat Assistant":
                    this.ChatControlInstance.Focus();
                    break;
                case "Risk Reports":
                    this.SendChatMessage("Generate a comprehensive risk assessment report");
                    break;
                case "Settings":
                    this.SendChatMessage("What settings and configurations are available?");
                    break;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            this.SendChatMessage("Help me search for specific loan applications or risk patterns");
        }

        private void QuickAction_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string action)
            {
                string message = action switch
                {
                    "📊 Risk Analysis" => "Analyze the current loan application's risk factors and provide insights",
                    "💰 Sample Data" => "show me a sample",
                    "📈 Explain Result" => "explain result",
                    _ => action
                };

                this.SendChatMessage(message);
            }
        }

        private void SendChatMessage(string message)
        {
            if (this.ChatViewModel != null && !string.IsNullOrWhiteSpace(message))
            {
                this.ChatViewModel.CurrentMessage = message;
                this.ChatViewModel.SendMessageCommand.Execute(null);
            }
        }
    }
}
namespace MLFinancialRiskPrediction.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    using MLFinancialRiskPrediction.UI.ViewModeels;

    /// <summary>
    /// Interaction logic for SinglePredictionControl.xaml
    /// </summary>
    public partial class SinglePredictionControl : UserControl
    {
        public SinglePredictionControl()
        {
            this.InitializeComponent();
        }

        #region Dependency Properties
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(SinglePredictionViewModel), typeof(SinglePredictionControl),
                new PropertyMetadata(null, OnViewModelChanged));

        public SinglePredictionViewModel? ViewModel
        {
            get => (SinglePredictionViewModel?)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }
        #endregion

        #region Private Methods
        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SinglePredictionControl control)
            {
                control.DataContext = e.NewValue;
            }
        }
        #endregion
    }
}
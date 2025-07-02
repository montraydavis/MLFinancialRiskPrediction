namespace MLFinancialRiskPrediction.UI.Controls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using MLFinancialRiskPrediction.UI.ViewModeels;

    /// <summary>
    /// Interaction logic for ResultDisplayControl.xaml
    /// </summary>
    public partial class ResultDisplayControl : UserControl
    {
        public ResultDisplayControl()
        {
            this.InitializeComponent();
        }

        #region Dependency Properties
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(SinglePredictionViewModel), typeof(ResultDisplayControl),
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
            if (d is ResultDisplayControl control)
            {
                control.DataContext = e.NewValue;
            }
        }
        #endregion
    }

    #region Value Converters
    public class BoolToStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool approved ? approved ? "✅" : "❌" : "❓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RiskScoreToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float riskScore)
            {
                // Convert risk score (0-100) to point on circle
                double angle = (riskScore / 100.0) * 270; // 270 degrees for 3/4 circle
                double radians = (angle - 90) * Math.PI / 180; // Start from top (-90 degrees)

                double x = 35 + 30 * Math.Cos(radians); // 35 = center, 30 = radius
                double y = 35 + 30 * Math.Sin(radians);

                return new Point(x, y);
            }
            return new Point(35, 5); // Default to top
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RiskScoreToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float riskScore)
            {
                // Convert risk score (0-100) to width (0-200 pixels)
                return Math.Min(200, (riskScore / 100.0) * 200);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? boolValue ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
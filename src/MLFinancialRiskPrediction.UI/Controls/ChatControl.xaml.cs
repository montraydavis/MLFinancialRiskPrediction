namespace MLFinancialRiskPrediction.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    using MLFinancialRiskPrediction.UI.ViewModeels;

    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        public ChatControl()
        {
            this.InitializeComponent();
            this.Loaded += this.ChatControl_Loaded;
        }

        #region Dependency Properties
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ChatViewModel), typeof(ChatControl),
                new PropertyMetadata(null, OnViewModelChanged));

        public ChatViewModel? ViewModel
        {
            get => (ChatViewModel?)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }
        #endregion

        #region Event Handlers
        private void ChatControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.MessageTextBox.Focus();
            this.StartTypingIndicatorAnimation();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                e.Handled = true;
                this.ViewModel?.SendMessageCommand.Execute(null);
            }
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatControl control)
            {
                control.DataContext = e.NewValue;

                if (e.NewValue is ChatViewModel viewModel)
                {
                    viewModel.ChatHistory.Messages.CollectionChanged += control.OnMessagesCollectionChanged;
                }

                if (e.OldValue is ChatViewModel oldViewModel)
                {
                    oldViewModel.ChatHistory.Messages.CollectionChanged -= control.OnMessagesCollectionChanged;
                }
            }
        }

        private void OnMessagesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() => this.ScrollToBottom());
        }
        #endregion

        #region Private Methods
        private void ScrollToBottom()
        {
            this.MessagesScrollViewer.ScrollToEnd();
        }

        private void StartTypingIndicatorAnimation()
        {
            Storyboard storyboard = new Storyboard { RepeatBehavior = RepeatBehavior.Forever };

            this.CreateDotAnimation(storyboard, this.Dot1, 0);
            this.CreateDotAnimation(storyboard, this.Dot2, 0.2);
            this.CreateDotAnimation(storyboard, this.Dot3, 0.4);

            storyboard.Begin();
        }

        private void CreateDotAnimation(Storyboard storyboard, FrameworkElement dot, double delay)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0.3,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.6),
                BeginTime = TimeSpan.FromSeconds(delay),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(animation, dot);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            storyboard.Children.Add(animation);
        }
        #endregion
    }
}
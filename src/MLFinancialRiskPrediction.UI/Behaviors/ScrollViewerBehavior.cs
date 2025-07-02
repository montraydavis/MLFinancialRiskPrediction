namespace MLFinancialRiskPrediction.UI.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty BubbleScrollEventsProperty =
            DependencyProperty.RegisterAttached(
                "BubbleScrollEvents",
                typeof(bool),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(false, OnBubbleScrollEventsChanged));

        public static readonly DependencyProperty SmartScrollBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "SmartScrollBehavior",
                typeof(bool),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(false, OnSmartScrollBehaviorChanged));

        public static bool GetBubbleScrollEvents(DependencyObject obj)
        {
            return (bool)obj.GetValue(BubbleScrollEventsProperty);
        }

        public static void SetBubbleScrollEvents(DependencyObject obj, bool value)
        {
            obj.SetValue(BubbleScrollEventsProperty, value);
        }

        public static bool GetSmartScrollBehavior(DependencyObject obj)
        {
            return (bool)obj.GetValue(SmartScrollBehaviorProperty);
        }

        public static void SetSmartScrollBehavior(DependencyObject obj, bool value)
        {
            obj.SetValue(SmartScrollBehaviorProperty, value);
        }

        private static void OnBubbleScrollEventsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.PreviewMouseWheel += OnPreviewMouseWheel;
                }
                else
                {
                    element.PreviewMouseWheel -= OnPreviewMouseWheel;
                }
            }
        }

        private static void OnSmartScrollBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                if ((bool)e.NewValue)
                {
                    scrollViewer.PreviewMouseWheel += OnSmartScrollViewerPreviewMouseWheel;
                }
                else
                {
                    scrollViewer.PreviewMouseWheel -= OnSmartScrollViewerPreviewMouseWheel;
                }
            }
        }

        private static void OnSmartScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                bool canScrollUp = scrollViewer.VerticalOffset > 0;
                bool canScrollDown = scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight;

                // If we can't scroll in the requested direction, bubble to parent
                if ((e.Delta > 0 && !canScrollUp) || (e.Delta < 0 && !canScrollDown))
                {
                    ScrollViewer? parentScrollViewer = FindParentScrollViewer(scrollViewer);
                    if (parentScrollViewer != null)
                    {
                        e.Handled = true;

                        MouseWheelEventArgs newArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                        {
                            RoutedEvent = UIElement.MouseWheelEvent,
                            Source = scrollViewer
                        };

                        parentScrollViewer.RaiseEvent(newArgs);
                    }
                }
                // If we can scroll, let this ScrollViewer handle it (don't set e.Handled)
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is UIElement element)
            {
                // Check if this element or any child has a ScrollViewer that can handle scrolling
                ScrollViewer? directScrollViewer = FindDirectScrollViewer(element);

                if (directScrollViewer != null)
                {
                    // This element has its own ScrollViewer - let it handle scrolling first
                    bool canScrollUp = directScrollViewer.VerticalOffset > 0;
                    bool canScrollDown = directScrollViewer.VerticalOffset < directScrollViewer.ScrollableHeight;

                    // Only bubble if we can't scroll in the requested direction
                    if ((e.Delta > 0 && !canScrollUp) || (e.Delta < 0 && !canScrollDown))
                    {
                        ScrollViewer? parentScrollViewer = FindParentScrollViewer(element);
                        if (parentScrollViewer != null && parentScrollViewer != directScrollViewer)
                        {
                            e.Handled = true;

                            MouseWheelEventArgs newArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                            {
                                RoutedEvent = UIElement.MouseWheelEvent,
                                Source = element
                            };

                            parentScrollViewer.RaiseEvent(newArgs);
                        }
                    }
                    // If we can scroll, let the direct ScrollViewer handle it (don't set e.Handled)
                }
                else
                {
                    // No direct ScrollViewer - find parent and bubble
                    ScrollViewer? parentScrollViewer = FindParentScrollViewer(element);
                    if (parentScrollViewer != null)
                    {
                        e.Handled = true;

                        MouseWheelEventArgs newArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                        {
                            RoutedEvent = UIElement.MouseWheelEvent,
                            Source = element
                        };

                        parentScrollViewer.RaiseEvent(newArgs);
                    }
                }
            }
        }

        private static ScrollViewer? FindDirectScrollViewer(DependencyObject element)
        {
            // Check if this element is a ScrollViewer
            if (element is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            // Look for immediate child ScrollViewer in visual tree
            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is ScrollViewer childScrollViewer)
                {
                    return childScrollViewer;
                }

                // Recursively search one level deeper for nested ScrollViewer
                ScrollViewer? nestedScrollViewer = FindDirectScrollViewer(child);
                if (nestedScrollViewer != null)
                {
                    return nestedScrollViewer;
                }
            }

            return null;
        }

        private static ScrollViewer? FindParentScrollViewer(DependencyObject child)
        {
            DependencyObject? parent = LogicalTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is ScrollViewer scrollViewer)
                {
                    return scrollViewer;
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }

            // If not found in logical tree, try visual tree
            parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is ScrollViewer scrollViewer)
                {
                    return scrollViewer;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}
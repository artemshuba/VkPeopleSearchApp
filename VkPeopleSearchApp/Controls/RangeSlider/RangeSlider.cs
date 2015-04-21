using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace VkPeopleSearchApp.Controls.RangeSlider
{
    [TemplatePart(Name = LeftThumbPartName, Type = typeof(Thumb))]
    [TemplatePart(Name = RightThumbPartName, Type = typeof(Thumb))]
    [TemplatePart(Name = LowerBarPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = UpperBarPartName, Type = typeof(FrameworkElement))]
    public class RangeSlider : Control
    {
        private const string LeftThumbPartName = "PART_LeftThumb";
        private const string RightThumbPartName = "PART_RightThumb";
        private const string LowerBarPartName = "PART_LowerBar";
        private const string UpperBarPartName = "PART_UpperBar";

        private Thumb _leftThumb, _rightThumb;
        private FrameworkElement _lowerBar, _upperBar;

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(RangeSlider), new PropertyMetadata(100d, MaximumPropertyChanged));

        private static void MaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (RangeSlider)d;
            c.UpperValue = Math.Min(c.UpperValue, c.Maximum);
            c.UpdateThumbsPosition();
        }

        /// <summary>
        /// Maximum value
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(double), typeof(RangeSlider), new PropertyMetadata(default(double), MinimumPropertyChanged));

        private static void MinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (RangeSlider)d;
            c.LowerValue = Math.Max(c.Minimum, c.LowerValue);
            c.UpdateThumbsPosition();
        }

        /// <summary>
        /// Minimum value
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register(
            "LowerValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(default(double), LowerValuePropertyChanged));

        private static void LowerValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (RangeSlider)d;
            c.LowerValue = Math.Max(c.Minimum, (double)e.NewValue);
            c.UpdateOutput();
            c.UpdateThumbsPosition();
        }

        /// <summary>
        /// Lower value
        /// </summary>
        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public static readonly DependencyProperty UpperValueProperty = DependencyProperty.Register(
            "UpperValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(100d, UpperValuePropertyChanged));

        private static void UpperValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (RangeSlider)d;
            c.UpperValue = Math.Min((double)e.NewValue, c.Maximum);
            c.UpdateOutput();
            c.UpdateThumbsPosition();
        }

        /// <summary>
        /// Upper value
        /// </summary>
        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public static readonly DependencyProperty ValueTextBlockProperty = DependencyProperty.Register(
            "ValueTextBlock", typeof (TextBlock), typeof (RangeSlider), new PropertyMetadata(default(TextBlock), ValueTextBlockPropertyChanged));

        private static void ValueTextBlockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (RangeSlider) d;
            c.UpdateOutput();
        }

        /// <summary>
        /// TextBlock for showing current values
        /// </summary>
        public TextBlock ValueTextBlock
        {
            get { return (TextBlock) GetValue(ValueTextBlockProperty); }
            set { SetValue(ValueTextBlockProperty, value); }
        }

        public RangeSlider()
        {
            this.DefaultStyleKey = typeof(RangeSlider);

            IsEnabledChanged += OnIsEnabledChanged;
            SizeChanged += OnSizeChanged;
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", false);
            else
                VisualStateManager.GoToState(this, "Normal", false);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdateThumbsPosition();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_leftThumb != null)
                _leftThumb.DragDelta -= LeftThumb_OnDragDelta;

            if (_rightThumb != null)
                _rightThumb.DragDelta -= RightThumb_OnDragDelta;

            _leftThumb = (Thumb)GetTemplateChild(LeftThumbPartName);
            _rightThumb = (Thumb)GetTemplateChild(RightThumbPartName);

            _lowerBar = (FrameworkElement)GetTemplateChild(LowerBarPartName);
            _upperBar = (FrameworkElement)GetTemplateChild(UpperBarPartName);

            _leftThumb.DragDelta += LeftThumb_OnDragDelta;
            _rightThumb.DragDelta += RightThumb_OnDragDelta;

            if (UpperValue <= Minimum)
                UpperValue = Maximum;

            UpdateOutput();
            UpdateThumbsPosition();

            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", false);
        }

        private void LeftThumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var currentLeft = _lowerBar.ActualWidth;

            if (currentLeft + e.HorizontalChange < 0)
                LowerValue = Minimum;
            else if ((currentLeft + e.HorizontalChange + GetThumbWidth(_leftThumb)) > ActualWidth)
                LowerValue = Maximum;
            else
            {
                var ratioDiff = e.HorizontalChange / (ActualWidth - GetThumbWidth(_rightThumb) - GetThumbWidth(_leftThumb));
                var rangeSize = Maximum - Minimum;
                var rangeDiff = rangeSize * ratioDiff;
                LowerValue = Math.Min(UpperValue, LowerValue + rangeDiff);
            }
        }

        private void RightThumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var currentLeft = ActualWidth - _upperBar.ActualWidth - GetThumbWidth(_rightThumb);

            if ((currentLeft + e.HorizontalChange) < 0)
                UpperValue = Minimum;
            else if ((currentLeft + e.HorizontalChange + GetThumbWidth(_rightThumb)) > ActualWidth)
                UpperValue = Maximum;
            else
            {
                var ratioDiff = e.HorizontalChange / (ActualWidth - GetThumbWidth(_rightThumb) - GetThumbWidth(_leftThumb));
                var rangeSize = Maximum - Minimum;
                var rangeDiff = rangeSize * ratioDiff;
                UpperValue = Math.Max(LowerValue, UpperValue + rangeDiff);
            }
        }

        private void UpdateThumbsPosition()
        {
            if (_leftThumb == null || _rightThumb == null || _upperBar == null || _lowerBar == null)
                return;

            double totalSize = ActualWidth - GetThumbWidth(_leftThumb) - GetThumbWidth(_rightThumb);

            double ratio = totalSize / (Maximum - Minimum);

            _lowerBar.Width = Math.Max(0, Math.Min(ratio * (LowerValue - Minimum), ActualWidth - _upperBar.Width - GetThumbWidth(_rightThumb) - GetThumbWidth(_leftThumb)));

            _upperBar.Width = Math.Max(0, Math.Min(ratio * (Maximum - UpperValue), ActualWidth - _lowerBar.Width - GetThumbWidth(_rightThumb) - GetThumbWidth(_leftThumb)));
        }

        private void UpdateOutput()
        {
            if (ValueTextBlock == null)
                return;

            ValueTextBlock.Text = UpperValue == LowerValue
                ? Math.Round(UpperValue).ToString()
                : string.Format("{0} – {1}", Math.Round(LowerValue), Math.Round(UpperValue));
        }

        private double GetThumbWidth(Thumb thumb)
        {
            return thumb.ActualWidth - thumb.Padding.Left - thumb.Padding.Right;
        }
    }
}

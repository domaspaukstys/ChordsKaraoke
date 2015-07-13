using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ChordsKaraoke.Creator.Views
{
    /// <summary>
    ///     Interaction logic for TimeTextView.xaml
    /// </summary>
    public partial class TimeTextView
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(TimeTextView), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(TimeTextView), new PropertyMetadata(default(bool), IsSelectedPropertyChangedCallback));

        private static void IsSelectedPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            TimeTextView view = dependencyObject as TimeTextView;
            if (view != null)
            {
                view.BorderBrush = new SolidColorBrush((bool)args.NewValue ? Colors.MediumBlue : Colors.Transparent);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private bool _mouseDown;

        public TimeTextView()
        {
            InitializeComponent();
        }

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set
            {
                if (Math.Abs(X - value) > 0.0001)
                {
                    SetValue(XProperty, value);
                }
            }
        }
//
//        public event RoutedEventHandler Click;
//
//        protected virtual void OnClick()
//        {
//            if (Click != null)
//            {
//                Click(this, new RoutedEventArgs());
//            }
//        }

        private void Move(object sender, DragDeltaEventArgs e)
        {
            if (X + e.HorizontalChange >= 0)
            {
                X += e.HorizontalChange;
            }
            else
            {
                X = 0;
            }
        }

        private void Resize(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange >= MinWidth)
            {
                Width += e.HorizontalChange;
            }
            else
            {
                Width = MinWidth;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDown = true;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseDown && e.LeftButton == MouseButtonState.Released)
            {
//                OnClick();
                IsSelected = !IsSelected;
            }
            _mouseDown = false;
        }
    }
}
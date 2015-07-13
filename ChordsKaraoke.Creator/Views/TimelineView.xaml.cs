using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ChordsKaraoke.Data.ViewModels;

namespace ChordsKaraoke.Creator.Views
{
    /// <summary>
    ///     Interaction logic for TimelineView.xaml
    /// </summary>
    public partial class TimelineView
    {

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom", typeof (double), typeof (TimelineView), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register(
            "CurrentPosition", typeof (double), typeof (TimelineView),
            new PropertyMetadata(default(double), CurrentPositionPropertyChangedCallback));

        public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.Register(
            "MultiSelect", typeof (bool), typeof (TimelineView), new PropertyMetadata(default(bool)));

        public bool MultiSelect
        {
            get { return (bool) GetValue(MultiSelectProperty); }
            set { SetValue(MultiSelectProperty, value); }
        }

        private bool _manualOffset;
        private bool _mouseButtonIsDown;

        public TimelineView()
        {
            InitializeComponent();
            CompositionTarget.Rendering += WhileRendering;
        }

        private void WhileRendering(object sender, EventArgs eventArgs)
        {
            MultiSelect = Keyboard.IsKeyDown(Key.LeftCtrl);
        }

        public double CurrentPosition
        {
            get { return (double) GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        public double Zoom
        {
            get { return (double) GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        private static void CurrentPositionPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var view = dependencyObject as TimelineView;
            if (view != null && !view._manualOffset)
            {
                view.ScrollViewer.ScrollToHorizontalOffset((double) args.NewValue - view.ScrollViewer.ViewportWidth/2);
            }
        }

        private void MouseWheelChange(object sender, MouseWheelEventArgs e)
        {
            bool zoom = Keyboard.IsKeyDown(Key.LeftCtrl);
            if (zoom)
            {
                Zoom += e.Delta > 0 ? 0.5 : -0.5;
            }
            else
            {
                var viewer = sender as ScrollViewer;
                if (viewer != null)
                {
                    viewer.ScrollToHorizontalOffset(viewer.ContentHorizontalOffset - e.Delta);
                }
            }
            e.Handled = true;
        }

        private void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var viewer = e.OriginalSource as ScrollViewer;
                if (viewer != null)
                {
                    _mouseButtonIsDown = true;
                }
            }
        }

        private void MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _mouseButtonIsDown = false;
                RecalculateTime(e);
            }
        }

        private void MouseStateChanges(object sender, MouseEventArgs e)
        {
            if (_mouseButtonIsDown && e.LeftButton == MouseButtonState.Pressed)
            {
                RecalculateTime(e);
            }
        }

        private void RecalculateTime(MouseEventArgs e)
        {
            var viewer = e.OriginalSource as ScrollViewer;
            if (viewer != null)
            {
                Point point = e.GetPosition(viewer);
                _manualOffset = true;
                CurrentPosition = point.X + viewer.HorizontalOffset;
                _manualOffset = false;
            }
        }
    }
}
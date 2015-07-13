using System.Windows;
using System.Windows.Controls.Primitives;

namespace ChordsKaraoke.Editor.Views
{
    /// <summary>
    /// Interaction logic for ResizibleTextControl.xaml
    /// </summary>
    public partial class ResizibleTextControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ResizibleTextControl), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TimestampProperty = DependencyProperty.Register(
            "Timestamp", typeof(double), typeof(ResizibleTextControl), new PropertyMetadata(default(double)));

        public double Timestamp
        {
            get { return (double)GetValue(TimestampProperty); }
            set { SetValue(TimestampProperty, value); }
        }

        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(ResizibleTextControl), new PropertyMetadata(default(double)));

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public ResizibleTextControl()
        {
            InitializeComponent();
        }

        private void HorizontalDragDeltaRight(object sender, DragDeltaEventArgs e)
        {
            if (Length > MinWidth)
            {
                Length += e.HorizontalChange;
            }
            else
            {
                Length = MinWidth + 4;
                Thumb t = sender as Thumb;
                if (t != null)
                {
                    t.ReleaseMouseCapture();
                }
            }
        }

        private void DragMove(object sender, DragDeltaEventArgs e)
        {
            if (Timestamp >= 0)
            {
                Timestamp += e.HorizontalChange;
            }
            else
            {
                Timestamp = 0;

                Thumb t = sender as Thumb;
                if (t != null)
                {
                    t.ReleaseMouseCapture();
                }
            }
        }
    }
}
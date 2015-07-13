using System.Collections.ObjectModel;
using System.Windows;
using ChordsKaraoke.Data.ViewModels;

namespace ChordsKaraoke.Creator.Views
{
    /// <summary>
    ///     Interaction logic for TimelineRowView.xaml
    /// </summary>
    public partial class TimelineRowView
    {
        public static readonly DependencyProperty RowItemTemplateProperty = DependencyProperty.Register(
            "RowItemTemplate", typeof (DataTemplate), typeof (TimelineRowView),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof (ObservableCollection<TimeTextViewModel>), typeof (TimelineRowView),
            new PropertyMetadata(default(ObservableCollection<TimeTextViewModel>)));

        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register("MaxX", typeof (double),
            typeof (TimelineRowView), new PropertyMetadata(default(double)));


        public TimelineRowView()
        {
            InitializeComponent();
        }

        public DataTemplate RowItemTemplate
        {
            get { return (DataTemplate) GetValue(RowItemTemplateProperty); }
            set { SetValue(RowItemTemplateProperty, value); }
        }

        public ObservableCollection<TimeTextViewModel> ItemsSource
        {
            get { return (ObservableCollection<TimeTextViewModel>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public double MaxX
        {
            get { return (double) GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }
    }
}
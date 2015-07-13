using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChordsKaraoke.Creator.Views
{
    public class IsChordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(value as bool? == true ? Colors.Orchid : Colors.LimeGreen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

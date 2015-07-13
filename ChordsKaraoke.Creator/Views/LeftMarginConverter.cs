using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChordsKaraoke.Creator.Views
{
    public class LeftMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new Thickness();
            var left = value as double?;
            if (left.HasValue)
            {
                result.Left = left.Value;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0d;
            var t = value as Thickness?;
            if (t.HasValue)
            {
                result = t.Value.Left;
            }
            return result;
        }
    }
}
using System;
using System.Windows.Data;
using System.Globalization;

namespace NetworkMonitor.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    class BoolInvertorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }
}

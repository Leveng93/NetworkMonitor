using System;
using System.Windows.Data;
using System.Globalization;

namespace NetworkMonitor.Converters
{
    /// <summary>
    /// Преобразует значение bool, отображающее статус сетевого монитора, в строку.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    class MonitorStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? "Capture in progress" : "Stopped";
        }

        /// <summary>
        /// Не реализовано - обратное преобразование не требуется.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

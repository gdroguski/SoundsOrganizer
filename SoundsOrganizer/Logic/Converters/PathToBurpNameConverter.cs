using System;
using System.Globalization;
using System.Windows.Data;

namespace SoundsOrganizer.Logic.Converters
{
    internal class PathToBurpNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (string)value;
            return input?.Substring(input.LastIndexOf("Burp", StringComparison.Ordinal));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return  null;
        }
    }
}

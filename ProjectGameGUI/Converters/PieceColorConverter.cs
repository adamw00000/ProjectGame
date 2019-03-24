using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup;
using Avalonia.Media;

namespace ProjectGameGUI.Converters
{
    public class PieceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool valid = (bool)value;
            return valid ? Colors.Gold : Colors.YellowGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
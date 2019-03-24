using System;
using System.Globalization;
using Avalonia.Markup;
using GameLib;
using Avalonia.Media;
using Avalonia.Data.Converters;

namespace ProjectGameGUI.Converters
{
    public class TeamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Team team = (Team)value;
            return team == Team.Blue ? Colors.Blue : Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
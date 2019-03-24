using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup;
using Avalonia.Media;
using ProjectGameGUI.Models;

namespace ProjectGameGUI.Converters
{
    public class FieldColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Field field = (Field)value;
            if(field.Team == null)
            {
                return Colors.White;
            }
            else
            {
                Color color = field.Team.Value == GameLib.Team.Blue ? Colors.Blue : Colors.Red;
                if(field.IsUndiscoveredGoal)
                {
                    color = new Color(100, color.R, color.G, color.B);
                }
                else
                {
                    color = new Color(50, color.R, color.G, color.B);
                }
                return color;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
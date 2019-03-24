
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using ProjectGameGUI.Models;
using ProjectGameGUI;
using Avalonia.Controls.Shapes;

namespace ProjectGameGUI.Converters
{
    public class GameObjectConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            List<Control> result = new List<Control>();

            IEnumerable<Field> fields = values[0] as IEnumerable<Field>;
            IEnumerable<Piece> pieces = values[1] as IEnumerable<Piece>;
            IEnumerable<Player> players = values[2] as IEnumerable<Player>;

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    Border border = DisplaySettings.GetFieldControl(field);
                    result.Add(border);
                }
            }

            if (pieces != null)
            {
                foreach (var piece in pieces)
                {
                    Ellipse ellipse = DisplaySettings.GetPieceControl(piece);
                    result.Add(ellipse);
                }
            }

            if (players != null)
            {
                foreach (var player in players)
                {
                    Canvas canvas = DisplaySettings.GetPlayerControl(player);
                    result.Add(canvas);
                }
            }

            return result;
        }
    }
}
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectGameGUI
{
    internal class DisplaySettings
    {
        public readonly int FieldWidth = 100;
        public readonly int FieldHeight = 100;

        public readonly float AgentWidthFactor = 0.5F;
        public readonly float AgentHeightFactor = 0.5F;

        public readonly ISolidColorBrush RedTeamColor = Brushes.IndianRed;
        public readonly ISolidColorBrush RedTeamColorActive = Brushes.Red;
        public readonly ISolidColorBrush BlueTeamColor = Brushes.DodgerBlue;
        public readonly ISolidColorBrush BlueTeamColorActive = Brushes.Blue;

        public readonly int BorderThickness = 1;
        public readonly ISolidColorBrush BorderBrush = Brushes.Black;

        public readonly float NormalFieldOpacityFactor = 0.2F;
        public readonly float GoalFieldOpacityFactor = 0.4F;

        public readonly float PieceWidthFactor = 0.8F;
        public readonly float PieceHeightFactor = 0.8F;
        public readonly float PlayerPieceFactor = 0.5F;
        public readonly ISolidColorBrush ValidPieceColor = Brushes.Gold;
        public readonly ISolidColorBrush InvalidPieceColor = Brushes.YellowGreen;


        public Rectangle GetPlayer(ISolidColorBrush teamColor)
        {
            Rectangle rect = new Rectangle();
            rect.Width = FieldWidth * AgentWidthFactor;
            rect.Height = FieldHeight * AgentHeightFactor;
            rect.Fill = teamColor;
            return rect;
        }

        public Border GetBorder(ISolidColorBrush backgound, GameLib.GameMasterField field)
        {
            Border border = new Border();
            border.BorderBrush = BorderBrush;
            border.BorderThickness = BorderThickness;
            border.Background = backgound;
            if(field.IsGoal)
            {
                border.Opacity = GoalFieldOpacityFactor;
            }
            else
            {
                border.Opacity = NormalFieldOpacityFactor;
            }
            return border;
        }

        public Ellipse GetPlayerPiece(bool valid)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = PieceWidthFactor * FieldWidth * PlayerPieceFactor;
            ellipse.Height = PieceHeightFactor * FieldHeight * PlayerPieceFactor;
            ellipse.Fill = valid ? ValidPieceColor : InvalidPieceColor;
            return ellipse;
        }

        public Ellipse GetPiece(bool valid)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = PieceWidthFactor * FieldWidth;
            ellipse.Height = PieceHeightFactor * FieldHeight;
            ellipse.Fill = valid ? ValidPieceColor : InvalidPieceColor;
            return ellipse;
        }

    }
}

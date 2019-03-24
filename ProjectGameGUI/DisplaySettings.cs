﻿using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using ProjectGameGUI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectGameGUI
{
    internal static class DisplaySettings
    {
        public static readonly int FieldWidth = 100;
        public static readonly int FieldHeight = 100;

        public static readonly float AgentWidthFactor = 0.5F;
        public static readonly float AgentHeightFactor = 0.5F;

        public static readonly ISolidColorBrush RedTeamGoalAreaColor = Brushes.Red;
        public static readonly ISolidColorBrush BlueTeamGoalAreaColor = Brushes.Blue;
        public static readonly ISolidColorBrush RedTeamColor = Brushes.IndianRed;
        public static readonly ISolidColorBrush RedTeamColorActive = Brushes.Red;
        public static readonly ISolidColorBrush BlueTeamColor = Brushes.DodgerBlue;
        public static readonly ISolidColorBrush BlueTeamColorActive = Brushes.Blue;
        public static readonly ISolidColorBrush TaskAreaColor = Brushes.White;

        public static readonly int BorderThickness = 1;
        public static readonly ISolidColorBrush BorderBrush = Brushes.Black;

        public static readonly float NormalFieldOpacityFactor = 0.2F;
        public static readonly float GoalFieldOpacityFactor = 0.4F;

        public static readonly float PieceWidthFactor = 0.8F;
        public static readonly float PieceHeightFactor = 0.8F;
        public static readonly float PlayerPieceFactor = 0.5F;
        public static readonly ISolidColorBrush ValidPieceColor = Brushes.Gold;
        public static readonly ISolidColorBrush InvalidPieceColor = Brushes.YellowGreen;
        public static readonly ISolidColorBrush TextColor = Brushes.Black;


        public static Canvas GetPlayerControl(Player player)
        {
            Canvas canvas = new Canvas();
            Grid.SetRow(canvas, player.X);
            Grid.SetColumn(canvas, player.Y);
            canvas.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            canvas.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

            Rectangle rect = new Rectangle();          
            rect.ZIndex = 2;
            rect.Width = FieldWidth * AgentWidthFactor;
            rect.Height = FieldHeight * AgentHeightFactor;
            if (player.Team == GameLib.Team.Blue)
            {
                rect.Fill = player.IsActive ? BlueTeamColorActive : BlueTeamColor;
            }
            else
            {
                rect.Fill = player.IsActive ? RedTeamColorActive : RedTeamColor;
            }
            
            canvas.Children.Add(rect);
            TextBlock text = new TextBlock();
            text.Text = player.Name;
            text.Foreground = TextColor;
            text.ZIndex = 4;
            text.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            text.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            canvas.Children.Add(text);

            if(player.HeldPiece != null)
            {
                canvas.Children.Add(GetPlayerPieceControl(player.HeldPiece));
            }

            return canvas;
        }

        public static Border GetFieldControl(Field field)
        {
            Border border = new Border();
            border.BorderBrush = BorderBrush;
            Grid.SetRow(border, field.X);
            Grid.SetColumn(border, field.Y);
            border.ZIndex = 0;
            border.BorderThickness = new Avalonia.Thickness(BorderThickness);
            switch(field.Team)
            {
                case GameLib.Team.Blue:
                    border.Background = BlueTeamGoalAreaColor;
                    break;
                case GameLib.Team.Red:
                    border.Background = RedTeamGoalAreaColor;
                    break;
                default:
                    border.Background = TaskAreaColor;
                    return border;
            }
            if(field.IsUndiscoveredGoal)
            {
                border.Opacity = GoalFieldOpacityFactor;
            }
            else
            {
                border.Opacity = NormalFieldOpacityFactor;
            }
            TextBlock text = new TextBlock();
            text.Text = field.Name;
            text.VerticalAlignment = VerticalAlignment.Top;
            text.HorizontalAlignment = HorizontalAlignment.Left;
            border.Child = text;
            return border;
        }

        public static Ellipse GetPlayerPieceControl(Piece piece)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = PieceWidthFactor * FieldWidth * PlayerPieceFactor;
            ellipse.Height = PieceHeightFactor * FieldHeight * PlayerPieceFactor;
            ellipse.Fill = piece.IsValid ? ValidPieceColor : InvalidPieceColor;
            ellipse.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            ellipse.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            ellipse.ZIndex = 3;
            return ellipse;
        }

        public static Ellipse GetPieceControl(Piece piece)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = PieceWidthFactor * FieldWidth;
            ellipse.Height = PieceHeightFactor * FieldHeight;
            ellipse.Fill = piece.IsValid ? ValidPieceColor : InvalidPieceColor;
            ellipse.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            ellipse.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            Grid.SetRow(ellipse, piece.X);
            Grid.SetColumn(ellipse, piece.Y);
            ellipse.ZIndex = 1;
            return ellipse;
        }

    }
}

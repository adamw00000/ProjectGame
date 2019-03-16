using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using GameLib;
using Avalonia.Controls.Shapes;

namespace ProjectGameGUI.Views
{
    public class MainWindow : Window
    {
        public Grid MainGrid { get; set; }
        private GameMasterState gameMasterState;
        private int width => gameMasterState.Board.Width;
        private int height => gameMasterState.Board.Height;
        private int goalAreaHeight => gameMasterState.Board.GoalAreaHeight;
        private DisplaySettings displaySettings = new DisplaySettings();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            MainGrid = this.Get<Grid>("MainGrid");

            var rules = GUIhelper.GetOddSizeBoardRules();
            gameMasterState = GUIhelper.GetGameMasterState(rules);
            gameMasterState.InitializePlayerPositions(width, height, 12);

            gameMasterState.PlayerStates.TryGetValue(0, out PlayerState playerState);
            playerState.Piece = new Piece(0.4);
            gameMasterState.PlayerStates[0] = playerState;

            this.Width = width * displaySettings.FieldWidth;
            this.Height = height * displaySettings.FieldHeight;
            InitializeBoard();
        }
        private void InitializeBoard()
        {
            PrepareGrid();
            SetBorders();
            SetPlayers();
            gameMasterState.GeneratePiece();
            VisualizePieces();
        }
        private void PrepareGrid()
        {
            List<ColumnDefinition> columns = new List<ColumnDefinition>(width);
            List<RowDefinition> rows = new List<RowDefinition>(height);

            for (int i = 0; i < width; i++)
            {
                columns.Add(new ColumnDefinition());
            }
            for (int i = 0; i < height; i++)
            {
                rows.Add(new RowDefinition());
            }
            MainGrid.ColumnDefinitions.AddRange(columns);
            MainGrid.RowDefinitions.AddRange(rows);
        }
        private void SetBorders()
        {
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < goalAreaHeight; i++)
                {
                    Border border = displaySettings.GetBorder(Avalonia.Media.Brushes.Red,gameMasterState.Board[i,j]);
                    TextBlock text = GetTextBlock(i, j);
                    Grid.SetColumn(border, j);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(text, j);
                    Grid.SetRow(text, i);
                    MainGrid.Children.Add(border);
                    MainGrid.Children.Add(text);
                }
                for (int i = goalAreaHeight; i < height - goalAreaHeight; i++)
                {
                    Border border = displaySettings.GetBorder(Avalonia.Media.Brushes.White, gameMasterState.Board[i, j]);
                    TextBlock text = GetTextBlock(i, j);
                    Grid.SetColumn(border, j);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(text, j);
                    Grid.SetRow(text, i);
                    MainGrid.Children.Add(border);
                    MainGrid.Children.Add(text);
                }
                for (int i = height - goalAreaHeight; i < height; i++)
                {
                    Border border = displaySettings.GetBorder(Avalonia.Media.Brushes.Blue, gameMasterState.Board[i, j]);
                    TextBlock text = GetTextBlock(i, j);
                    Grid.SetColumn(border, j);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(text, j);
                    Grid.SetRow(text, i);
                    MainGrid.Children.Add(border);
                    MainGrid.Children.Add(text);
                }
            }
            TextBlock GetTextBlock(int i, int j)
            {
                TextBlock text = new TextBlock();
                text.Text = (i, j).ToString();
                text.Foreground = Avalonia.Media.Brushes.Black;
                return text;
            }
        }
        private void SetPlayers()
        {
            foreach(var player in gameMasterState.PlayerStates)
            {
                Rectangle r = null;

                if(player.Value.Team == Team.Blue)
                {
                    r = displaySettings.GetPlayer(displaySettings.RedTeamColor);
                }
                else
                {
                    r = displaySettings.GetPlayer(displaySettings.BlueTeamColor);
                }

                Grid.SetColumn(r, player.Value.Position.Y);
                Grid.SetRow(r, player.Value.Position.X);
                MainGrid.Children.Add(r);

                if (player.Value.Piece != null)
                {
                    Ellipse ellipse = displaySettings.GetPlayerPiece();
                    Grid.SetColumn(ellipse, player.Value.Position.Y);
                    Grid.SetRow(ellipse, player.Value.Position.X);
                    ellipse.ZIndex = 2;
                    MainGrid.Children.Add(ellipse);
                }
            }
        }
        private void VisualizePieces()
        {
            foreach(var piece in gameMasterState.Board.PiecesPositions)
            {
                Ellipse p = displaySettings.GetPiece();
                Grid.SetColumn(p, piece.y);
                Grid.SetRow(p, piece.x);
                MainGrid.Children.Add(p);
            }
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

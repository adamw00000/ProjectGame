using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using GameLib;
using Avalonia.Controls.Shapes;
using System.Threading.Tasks;
using Avalonia.Threading;
using System;

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
            InitializeDummyGM();
            this.Width = width * displaySettings.FieldWidth;
            this.Height = height * displaySettings.FieldHeight;

            InitializeBoard();
            TestUpdateLoop();
        }

        private void InitializeDummyGM()
        {
            var rules = GUIhelper.GetOddSizeBoardRules();
            gameMasterState = GUIhelper.GetGameMasterState(rules);
            gameMasterState.InitializePlayerPositions(width, height, 12);

            gameMasterState.PlayerStates.TryGetValue(0, out PlayerState playerState);
            playerState.Piece = new Piece(0.4);
            gameMasterState.PlayerStates[0] = playerState;

            //gameMasterState.GeneratePiece();
        }

        private async Task TestUpdateLoop()
        {
            Random random = new Random();

            while (true)
            {
                await Task.Delay(50);
                switch(random.Next() % 4)
                {
                    case 0:
                        {
                            gameMasterState.GeneratePiece();
                            break;
                        }
                    case 2:
                    case 3:
                    case 1:
                        {
                            try
                            {
                                gameMasterState.Move(random.Next() % gameMasterState.PlayerStates.Count, (MoveDirection)(random.Next() % 4));
                            }
                            catch { }
                            break;
                        }
                }
                await Update();
            }
        }

        public async Task Update()
        {
            await Dispatcher.UIThread.InvokeAsync(() => UpdateBoard());
        }

        private void InitializeBoard()
        {
            MainGrid = this.Get<Grid>("MainGrid");
            InitializeGrid();
            VisualizeBorders();
            VisualizePlayers();
            VisualizePieces();
        }

        private void UpdateBoard()
        {
            MainGrid.Children.Clear();
            VisualizeBorders();
            VisualizePlayers();
            VisualizePieces();
        }

        private void InitializeGrid()
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

        private void VisualizeBorders()
        {
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < goalAreaHeight; i++)
                {
                    Border border = displaySettings.GetBorder(Avalonia.Media.Brushes.Red, gameMasterState.Board[i, j]);
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

        private void VisualizePlayers()
        {
            foreach(var player in gameMasterState.PlayerStates)
            {
                Rectangle r = null;

                if(player.Value.Team == Team.Blue)
                {
                    r = displaySettings.GetPlayer(displaySettings.BlueTeamColor);
                }
                else
                {
                    r = displaySettings.GetPlayer(displaySettings.RedTeamColor);
                }

                Grid.SetColumn(r, player.Value.Position.Y);
                Grid.SetRow(r, player.Value.Position.X);
                r.ZIndex = 2;
                MainGrid.Children.Add(r);

                if (player.Value.Piece != null)
                {
                    Ellipse ellipse = displaySettings.GetPlayerPiece(player.Value.Piece.IsValid);
                    Grid.SetColumn(ellipse, player.Value.Position.Y);
                    Grid.SetRow(ellipse, player.Value.Position.X);
                    ellipse.ZIndex = 3;
                    MainGrid.Children.Add(ellipse);
                }
            }
        }

        private void VisualizePieces()
        {
            foreach(var piecePos in gameMasterState.Board.PiecesPositions)
            {
                var piece = gameMasterState.Board[piecePos.x, piecePos.y].Piece;
                Ellipse p = displaySettings.GetPiece(piece.IsValid);
                Grid.SetColumn(p, piecePos.y);
                Grid.SetRow(p, piecePos.x);
                p.ZIndex = 1;
                MainGrid.Children.Add(p);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using GameLib;
using Avalonia.Controls.Shapes;
using System.Threading.Tasks;
using Avalonia.Threading;
using System;
using ConnectionLib;

namespace ProjectGameGUI.Views
{
    public class MainWindow : Window
    {
        public Grid MainGrid { get; set; }
        private GameMaster gameMaster;
        private GameMasterState gameMasterState => gameMaster.state;
        private int width => gameMasterState.Board.Width;
        private int height => gameMasterState.Board.Height;
        private int goalAreaHeight => gameMasterState.Board.GoalAreaHeight;
        private DisplaySettings displaySettings = new DisplaySettings();

        Task[] agentTasks;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            int[] actionPriorities = new int[] { 2, 1, 10, 20, 5, 10, 2, 2 };

            LocalCommunicationServer cs = new LocalCommunicationServer();
            GMLocalConnection gMLocalConnection = new GMLocalConnection(cs);
            GameRules rules = new GameRules(teamSize: 1, baseTimePenalty: 100, boardHeight: 7, boardWidth: 7);
            gameMaster = new GameMaster(rules, gMLocalConnection);

            this.Width = width * displaySettings.FieldWidth;
            this.Height = height * displaySettings.FieldHeight;
            
            //Agent interactiveAgent1 = new Agent(0, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
            //Agent interactiveAgent2 = new Agent(1, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
            //Task interactiveAgentTask1 = interactiveAgent1.Run();
            //Task interactiveAgentTask2 = interactiveAgent2.Run();


            Task.Run(() => { gameMaster.ListenJoiningAndStart(); });

            agentTasks = new Task[2 * rules.TeamSize];

            for (int i = 0; i < rules.TeamSize; ++i)
            {
                Agent Agent1 = new Agent(2 * i + 2, new RandomDecisionModule(actionPriorities, 10), new AgentLocalConnection(cs));
                //Agent Agent1 = new Agent(2 * i + 2, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
                agentTasks[2 * i] = Task.Run(async () => await Agent1.Run(Team.Red));

                Agent Agent2 = new Agent(2 * i + 3, new RandomDecisionModule(actionPriorities, 5), new AgentLocalConnection(cs));
                //Agent Agent2 = new Agent(2 * i + 3, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
                agentTasks[2 * i + 1] = Task.Run(async () => await Agent2.Run(Team.Blue));
            }

            while (!gameMaster.gameStarted) { }

            InitializeBoard();
            Task.Run(() => UpdateLoop());
        }

        private async Task Reader()
        {
            Task inputReaderTask = InteractiveInputProvider.ReadInput();
            await inputReaderTask;
        }
        private async Task UpdateLoop()
        {
            while (true)
            {
                await Update();
                System.Threading.Thread.Sleep(100);
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

        private async void UpdateBoard()
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
            var states = new Dictionary<int, PlayerState>(gameMasterState.PlayerStates);

            foreach (var (id, player) in states)
            {
                if (player.Piece != null)
                {
                    Ellipse ellipse = displaySettings.GetPlayerPiece(player.Piece.IsValid);
                    Grid.SetColumn(ellipse, player.Position.Y);
                    Grid.SetRow(ellipse, player.Position.X);
                    ellipse.ZIndex = 3;
                    MainGrid.Children.Add(ellipse);
                }
                TextBlock textId = new TextBlock();

                textId.Text = "Id: " + id.ToString() + "\n";

                char c;

                if (InteractiveInputProvider.RegisteredAgents.TryGetValue(id, out c))
                {
                    textId.Text += "['" + c.ToString() + "']";
                }

                Rectangle r = null;

                if (player.Team == Team.Blue)
                {
                    if(InteractiveInputProvider.ActiveAgents.Contains(c))
                        r = displaySettings.GetPlayer(displaySettings.BlueTeamColorActive);
                    else
                        r = displaySettings.GetPlayer(displaySettings.BlueTeamColor);
                }
                else
                {
                    if (InteractiveInputProvider.ActiveAgents.Contains(c))
                        r = displaySettings.GetPlayer(displaySettings.RedTeamColorActive);
                    else
                        r = displaySettings.GetPlayer(displaySettings.RedTeamColor);
                }

                Grid.SetColumn(r, player.Position.Y);
                Grid.SetRow(r, player.Position.X);
                r.ZIndex = 2;
                MainGrid.Children.Add(r);


                textId.Foreground = Avalonia.Media.Brushes.Black;
                textId.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                textId.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                textId.Foreground = Avalonia.Media.Brushes.White;
                Grid.SetColumn(textId, player.Position.Y);
                Grid.SetRow(textId, player.Position.X);
                textId.ZIndex = 4;
                MainGrid.Children.Add(textId);
            }
        }

        private void VisualizePieces()
        {
            var states = new List<(int x, int y)>(gameMasterState.Board.PiecesPositions);

            foreach (var piecePos in states)
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

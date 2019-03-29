using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;
using GameLib;
using ConnectionLib;
using System.Threading.Tasks;
using ProjectGameGUI.ViewModels;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System.Collections.ObjectModel;

namespace ProjectGameGUI.Models
{
    public class GameModel
    {
        private Task[] agentTasks;
        private GameMaster gameMaster;
        private GameMasterState gameMasterState => gameMaster.state;
        private int width => gameMasterState.Board.Width;
        private int height => gameMasterState.Board.Height;
        private int goalAreaHeight => gameMasterState.Board.GoalAreaHeight;
        private MainWindowViewModel mainWindowViewModel;
        private List<Field> fields = new List<Field>();
        private List<Player> players = new List<Player>();
        private List<Piece> pieces = new List<Piece>();

        public GameModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public void InitializeGame()
        {
            int[] actionPriorities = new int[] { 2, 1, 10, 20, 5, 10, 2, 2 };

            LocalCommunicationServer cs = new LocalCommunicationServer();
            GMLocalConnection gMLocalConnection = new GMLocalConnection(cs);
            GameRules rules = new GameRules(teamSize: 2, baseTimePenalty: 50, boardHeight: 8, boardWidth: 4);
            gameMaster = new GameMaster(rules, gMLocalConnection);

            this.mainWindowViewModel.WindowWidth = width * DisplaySettings.FieldWidth;
            this.mainWindowViewModel.WindowHeight = height * DisplaySettings.FieldHeight;

            this.mainWindowViewModel.BoardWidth = width;
            this.mainWindowViewModel.BoardHeight = height;

            Task.Run(() => { gameMaster.ListenJoiningAndStart(); });

            agentTasks = new Task[2 * rules.TeamSize];

            for (int i = 0; i < rules.TeamSize; ++i)
            {
                //Agent Agent1 = new Agent(2 * i + 2, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                //Agent Agent1 = new Agent(2 * i + 2, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
                Agent Agent1 = new Agent(2 * i + 2, new BasicDecisionModule(), new AgentLocalConnection(cs));
                agentTasks[2 * i] = Task.Run(async () => await Agent1.Run(Team.Red));

                //Agent Agent2 = new Agent(2 * i + 3, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                //Agent Agent2 = new Agent(2 * i + 3, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
                Agent Agent2 = new Agent(2 * i + 3, new BasicDecisionModule(), new AgentLocalConnection(cs));
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
                System.Threading.Thread.Sleep(34);
            }
        }

        public async Task Update()
        {
            await Dispatcher.UIThread.InvokeAsync(() => UpdateBoard());
        }

        private void InitializeBoard()
        {
            InitializeGrid();
            UpdateBoard();
        }

        private async void UpdateBoard()
        {
            PrepareFields();
            PreparePlayers();
            PreparePieces();
            mainWindowViewModel.Fields = new ObservableCollection<Field>(fields);
            mainWindowViewModel.Players = new ObservableCollection<Player>(players);
            mainWindowViewModel.Pieces = new ObservableCollection<Piece>(pieces);
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
        }

        private void PrepareFields()
        {
            fields.Clear();
            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < goalAreaHeight; i++)
                {
                    Field field = new Field();
                    field.X = i;
                    field.Y = j;
                    field.Team = Team.Red;
                    field.IsUndiscoveredGoal = gameMasterState.Board[i, j].IsGoal;
                    field.Name = $"({i},{j})";
                    fields.Add(field);
                }
                for (int i = goalAreaHeight; i < height - goalAreaHeight; i++)
                {
                    Field field = new Field();
                    field.X = i;
                    field.Y = j;
                    field.Team = null;
                    field.IsUndiscoveredGoal = false;
                    field.Name = $"({i},{j})";
                    fields.Add(field);
                }
                for (int i = height - goalAreaHeight; i < height; i++)
                {
                    Field field = new Field();
                    field.X = i;
                    field.Y = j;
                    field.Team = Team.Blue;
                    field.IsUndiscoveredGoal = gameMasterState.Board[i, j].IsGoal;
                    field.Name = $"({i},{j})";
                    fields.Add(field);
                }
            }
        }

        private void PreparePlayers()
        {
            players.Clear();
            var states = new Dictionary<int, PlayerState>(gameMasterState.PlayerStates);

            foreach (var (id, playerstate) in states)
            {
                Player player = new Player();

                if (playerstate.Piece != null)
                {
                    Piece piece = new Piece();
                    piece.IsValid = playerstate.Piece.IsValid;
                    (piece.X, piece.Y) = playerstate.Position;
                    player.HeldPiece = piece;
                }
                TextBlock textId = new TextBlock();

                player.Name = "Id: " + id.ToString() + "\n";

                char c;

                if (InteractiveInputProvider.RegisteredAgents.TryGetValue(id, out c))
                {
                    player.Name += "['" + c.ToString() + "']";
                }

                player.Team = playerstate.Team;
                (player.X, player.Y) = playerstate.Position;
                player.IsActive = InteractiveInputProvider.ActiveAgents.Contains(c);
                players.Add(player);
            }
        }

        private void PreparePieces()
        {
            pieces.Clear();
            var states = new List<(int x, int y)>(gameMasterState.Board.PiecesPositions);

            foreach (var piecePos in states)
            {
                Piece piece = new Piece();
                (piece.X, piece.Y) = piecePos;
                piece.IsValid = gameMasterState.Board[piecePos.x, piecePos.y].Piece.IsValid;
                pieces.Add(piece);
            }
        }
    }
}

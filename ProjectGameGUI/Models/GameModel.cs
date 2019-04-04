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
        private GameMasterStateSnapshot gameMasterStateSnapShot => gameMaster.GameMasterStateSnapshot;
        private int width => gameMasterStateSnapShot.Board.Width;
        private int height => gameMasterStateSnapShot.Board.Height;
        private int goalAreaHeight => gameMasterStateSnapShot.Board.GoalAreaHeight;
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
            gameMaster = new GameMaster(rules, gMLocalConnection, new GameMasterMessageFactory());

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
                Agent Agent1 = new Agent(2 * i + 2, new BasicDecisionModule(), new AgentLocalConnection(cs), new AgentMessageFactory());
                agentTasks[2 * i] = Task.Run(async () => await Agent1.Run(Team.Red));

                //Agent Agent2 = new Agent(2 * i + 3, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                //Agent Agent2 = new Agent(2 * i + 3, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
                Agent Agent2 = new Agent(2 * i + 3, new BasicDecisionModule(), new AgentLocalConnection(cs), new AgentMessageFactory());
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
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < goalAreaHeight; y++)
                {
                    Field field = new Field();
                    field.X = x;
                    field.Y = height - y - 1; //avalonia displays it upside down
                    field.Team = Team.Blue;
                    field.IsUndiscoveredGoal = gameMasterStateSnapShot.Board[x, y].IsGoal;
                    field.Name = $"({x},{y})";
                    fields.Add(field);
                }
                for (int y = goalAreaHeight; y < height - goalAreaHeight; y++)
                {
                    Field field = new Field();
                    field.X = x;
                    field.Y = height - y - 1; //avalonia displays it upside down
                    field.Team = null;
                    field.IsUndiscoveredGoal = false;
                    field.Name = $"({x},{y})";
                    fields.Add(field);
                }
                for (int y = height - goalAreaHeight; y < height; y++)
                {
                    Field field = new Field();
                    field.X = x;
                    field.Y = height - y - 1; //avalonia displays it upside down
                    field.Team = Team.Red;
                    field.IsUndiscoveredGoal = gameMasterStateSnapShot.Board[x, y].IsGoal;
                    field.Name = $"({x},{y})";
                    fields.Add(field);
                }
            }
        }

        private void PreparePlayers()
        {
            players.Clear();
            
            foreach (var (id, playerstate) in gameMasterStateSnapShot.PlayerStates)
            {
                Player player = new Player();
                if (playerstate.Piece != null)
                {
                    Piece piece = new Piece();
                    piece.IsValid = playerstate.Piece.IsValid;
                    //(piece.X, piece.Y) = playerstate.Position; // not useful
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
                player.X = playerstate.Position.X;
                player.Y = height - playerstate.Position.Y - 1; //avalonia displays it upside down
                player.IsActive = InteractiveInputProvider.ActiveAgents.Contains(c);
                players.Add(player);
            }
        }

        private void PreparePieces()
        {
            pieces.Clear();

            foreach (var piecePos in gameMasterStateSnapShot.Board.PiecesPositions)
            {
                Piece piece = new Piece();
                piece.X = piecePos.x;
                piece.Y = height - piecePos.y - 1;
                piece.IsValid = gameMasterStateSnapShot.Board[piecePos.x, piecePos.y].Piece.IsValid;
                pieces.Add(piece);
            }
        }
    }
}

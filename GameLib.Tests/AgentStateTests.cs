using Shouldly;
using System;
using Xunit;

namespace GameLib.Tests
{
    public class AgentStateTests
    {
        //factory methods - in order to abstract sometimes complex object creation
        private GameRules GetDefaultRules()
        {
            return new GameRules(boardWidth: 8, boardHeight: 8, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5);
        }

        private AgentState GetState()
        {
            return new AgentState();
        }

        private AgentState GetSetUpState(GameRules rules)
        {
            var state = new AgentState();
            state.Setup(rules);

            return state;
        }

        private static AgentField[,] GetFields()
        {
            return new AgentField[3, 3]
                {
                    {
                        new AgentField { Distance = 2, IsGoal = AgentFieldState.Unknown, Timestamp = DateTime.UtcNow.AddMilliseconds(-100) },
                        new AgentField { Distance = 1, IsGoal = AgentFieldState.DiscoveredGoal, Timestamp = DateTime.UtcNow.AddMilliseconds(-200) },
                        new AgentField { Distance = 2, IsGoal = AgentFieldState.DiscoveredNotGoal, Timestamp = DateTime.UtcNow.AddMilliseconds(-300) }
                    },
                    {
                        new AgentField { Distance = 1, IsGoal = AgentFieldState.NA, Timestamp = DateTime.UtcNow.AddMilliseconds(-100) },
                        new AgentField { Distance = 0, IsGoal = AgentFieldState.NA, Timestamp = DateTime.UtcNow.AddMilliseconds(-200) },
                        new AgentField { Distance = 1, IsGoal = AgentFieldState.NA, Timestamp = DateTime.UtcNow.AddMilliseconds(-300) }
                    },
                    {
                        new AgentField { Distance = 2, IsGoal = AgentFieldState.NA, Timestamp = DateTime.UtcNow.AddMilliseconds(-100) },
                        new AgentField { Distance = 1, IsGoal = AgentFieldState.NA, Timestamp = DateTime.UtcNow.AddMilliseconds(-200) },
                        new AgentField { Distance = 2, IsGoal = AgentFieldState.NA, Timestamp = DateTime.UtcNow.AddMilliseconds(-300) }
                    }
                };
        }

        [Theory]
        [InlineData(4, 4)]
        [InlineData(1, 8)]
        public void Setup_WhenCalled_SetsAgentsPosition(int X, int Y)
        {
            var state = GetState();
            var rules = new GameRules(boardWidth: 8, boardHeight: 8, agentStartX: X, agentStartY: Y);

            state.Setup(rules);

            state.Position.X.ShouldBe(X);
            state.Position.Y.ShouldBe(Y);
        }

        [Theory]
        [InlineData(8, 8, 2)]
        [InlineData(4, 6, 1)]
        public void Setup_WhenCalled_InitializesBoard(int width, int height, int goalAreaHeight)
        {
            var state = GetState();
            var rules = new GameRules(boardWidth: width, boardHeight: height, goalAreaHeight: goalAreaHeight, agentStartX: 0, agentStartY: 0);

            state.Setup(rules);

            state.Board.ShouldNotBe(null);
            state.Board.Width.ShouldBe(width);
            state.Board.Height.ShouldBe(height);
            state.Board.GoalAreaHeight.ShouldBe(goalAreaHeight);
        }

        [Theory]
        [InlineData(MoveDirection.Up)]
        [InlineData(MoveDirection.Down)]
        [InlineData(MoveDirection.Left)]
        [InlineData(MoveDirection.Right)]
        public void Move_WhenCalled_ChangesAgentsPosition(MoveDirection direction)
        {
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            var distance = 1;

            state.Move(direction, distance);

            var expectedX = rules.AgentStartX;
            var expectedY = rules.AgentStartY;
            switch (direction)
            {
                case MoveDirection.Left:
                    expectedY--;
                    break;

                case MoveDirection.Right:
                    expectedY++;
                    break;

                case MoveDirection.Up:
                    expectedX--;
                    break;

                case MoveDirection.Down:
                    expectedX++;
                    break;
            }
            state.Position.X.ShouldBe(expectedX);
            state.Position.Y.ShouldBe(expectedY);
        }

        [Theory]
        [InlineData(MoveDirection.Left)]
        [InlineData(MoveDirection.Up)]
        public void Move_WhenMoveIsInvalid_ThrowsInvalidMoveException(MoveDirection direction)
        {
            var rules = new GameRules(boardWidth: 8, boardHeight: 8, agentStartX: 0, agentStartY: 0);
            var state = GetSetUpState(rules);
            var distance = 1;

            Should.Throw<InvalidMoveException>(() => state.Move(direction, distance));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(5)]
        public void MoveAgent_WhenCalled_UpdatesBoard(int distance)
        {
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);

            var callTime = DateTime.UtcNow.AddMilliseconds(-1);
            state.Move(MoveDirection.Up, distance);

            var field = state.Board[state.Position.X, state.Position.Y];
            field.Distance.ShouldBe(distance);
            field.Timestamp.ShouldBeGreaterThan(callTime);
        }

        [Fact]
        public void PickUpPiece_WhenAgentDoesntHavePiece_UpdatesPieceState()
        {
            var state = GetState();
            state.HoldsPiece = false;

            state.PickUpPiece();

            state.HoldsPiece.ShouldBe(true);
            state.PieceState.ShouldBe(PieceState.Unknown);
        }

        [Fact]
        public void PickUpPiece_WhenAgentHasPiece_ThrowsPieceOperationException()
        {
            var state = GetState();
            state.HoldsPiece = true;

            Should.Throw<PieceOperationException>(() => state.PickUpPiece(), "Picking up piece when agent has one already");
        }

        [Theory]
        [InlineData(PieceState.Invalid)]
        [InlineData(PieceState.Valid)]
        public void SetPieceState_WhenCalled_UpdatesPieceState(PieceState newState)
        {
            var state = GetState();

            state.SetPieceState(newState);

            state.PieceState.ShouldBe(newState);
        }

        [Fact]
        public void PlaceOrDestroyPiece_WhenAgentHasPiece_UpdatesPieceState()
        {
            var state = GetState();
            state.HoldsPiece = true;

            state.PlaceOrDestroyPiece();

            state.HoldsPiece.ShouldBe(false);
            state.PieceState.ShouldBe(PieceState.Unknown);
        }

        [Fact]
        public void PlaceOrDestroyPiece_WhenAgentDoesntHavePiece_ThrowsPieceOperationException()
        {
            var state = GetState();
            state.HoldsPiece = false;

            Should.Throw<PieceOperationException>(() => state.PlaceOrDestroyPiece(), "Placing or destroying piece when agent doesn't have it");
        }

        [Fact]
        public void ApplyDiscoveryResult_WhenDiscoveryResultIsNot3x3_ThrowsInvalidDiscoveryResultException()
        {
            var state = GetState();
            var fields = new AgentField[3, 4];
            var discoveryResult = new AgentDiscoveryResult(1, 1, fields);

            Should.Throw<InvalidDiscoveryResultException>(() => state.Discover(discoveryResult));
        }

        [Fact]
        public void ApplyDiscoveryResult_WhenDiscoveryPositionIsNotOnTheBoard_ThrowsInvalidDiscoveryResultException()
        {
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            var fields = new AgentField[3, 3];
            var discoveryResult = new AgentDiscoveryResult(-1, -1, fields);

            Should.Throw<InvalidDiscoveryResultException>(() => state.Discover(discoveryResult));
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(7, 7)]
        [InlineData(0, 0)]
        public void ApplyDiscoveryResult_WhenDiscoveryResultIsValid_UpdatesBoard(int x, int y)
        {
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            AgentField[,] fields = GetFields();
            var discoveryResult = new AgentDiscoveryResult(x, y, fields);

            state.Discover(discoveryResult);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (IsInBounds(state.Board, x + i, y + j))
                        state.Board[x + i, y + j].Distance.ShouldBe(fields[i + 1, j + 1].Distance);
                }
            }
        }

        private bool IsInBounds(AgentBoard board, int x, int y)
        {
            return x >= 0 && x < board.Width &&
                y >= 0 && y < board.Height;
        }

        [Fact]
        public void UpdateBoardWithCommunicationData_WhenBoardsAreDifferentSizes_ThrowsInvalidDiscoveryResultException()
        {
            var rules = new GameRules(boardWidth: 8);
            var state = GetSetUpState(rules);
            var resultRules = new GameRules(boardWidth: 10);
            var resultBoard = new AgentBoard(resultRules);

            Should.Throw<InvalidCommunicationResultException>(() => state.UpdateBoardWithCommunicationData(resultBoard));
        }

        [Fact]
        public void UpdateBoardWithCommunicationData_WhenResultIsNewer_UpdatesBoard()
        {
            int distance = 1;
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            var resultBoard = new AgentBoard(rules);
            SetupCommunicationBoards(state.Board, resultBoard, DateTime.MaxValue, distance);

            state.UpdateBoardWithCommunicationData(resultBoard);

            for (int i = 0; i < state.Board.Height; i++)
            {
                for (int j = 0; j < state.Board.Width; j++)
                {
                    state.Board[i, j].Distance.ShouldBe(distance);
                }
            }
        }

        [Fact]
        public void UpdateBoardWithCommunicationData_WhenResultIsOlder_DoesntUpdatesBoard()
        {
            int distance = 1;
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            var resultBoard = new AgentBoard(rules);
            SetupCommunicationBoards(state.Board, resultBoard, DateTime.MinValue, distance);

            state.UpdateBoardWithCommunicationData(resultBoard);

            for (int i = 0; i < state.Board.Height; i++)
            {
                for (int j = 0; j < state.Board.Width; j++)
                {
                    state.Board[i, j].Distance.ShouldBe(-1);
                }
            }
        }

        private static void SetupCommunicationBoards(AgentBoard agentBoard, AgentBoard resultBoard, in DateTime value, int distance)
        {
            for (int i = 0; i < resultBoard.Height; i++)
            {
                for (int j = 0; j < resultBoard.Width; j++)
                {
                    agentBoard.BoardTable[i, j].Distance = -1;
                    resultBoard.BoardTable[i, j].Distance = distance;
                    resultBoard.BoardTable[i, j].Timestamp = value;
                }
            }
        }
    }
}
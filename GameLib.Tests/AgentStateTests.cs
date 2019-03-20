using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void PlacePiece_WhenAgentHasPiece_UpdatesPieceState()
        {
            var state = GetState();
            state.HoldsPiece = true;

            state.PlacePiece(PutPieceResult.PieceInTaskArea);

            state.HoldsPiece.ShouldBe(false);
            state.PieceState.ShouldBe(PieceState.Unknown);
        }

        [Fact]
        public void DestroyPiece_WhenAgentHasPiece_UpdatesPieceState()
        {
            var state = GetState();
            state.HoldsPiece = true;

            state.DestroyPiece();

            state.HoldsPiece.ShouldBe(false);
            state.PieceState.ShouldBe(PieceState.Unknown);
        }

        [Fact]
        public void PutPiece_WhenAgentDoesntHavePiece_ThrowsPieceOperationException()
        {
            var state = GetState();
            state.HoldsPiece = false;

            Should.Throw<PieceOperationException>(() => state.PlacePiece(PutPieceResult.PieceGoalRealized), "Placing piece when agent doesn't have it");
        }

        [Fact]
        public void DestroyPiece_WhenAgentDoesntHavePiece_ThrowsPieceOperationException()
        {
            var state = GetState();
            state.HoldsPiece = false;

            Should.Throw<PieceOperationException>(() => state.DestroyPiece(), "Destroying piece when agent doesn't have it");
        }

        [Fact]
        public void ApplyDiscoveryResult_WhenDiscoveryResultIsValid_UpdatesBoard()
        {
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            List<(int x, int y, int distance)> fields = new List<(int x, int y, int distance)>
            {(5,3,4),(5,4,3),(5,5,2),(4,3,3),(4,4,2),(4,5,1),(3,3,2),(3,4,1),(3,5,0)};
            var discoveryResult = new DiscoveryResult(fields);

            state.Discover(discoveryResult, 0);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (IsInBounds(state.Board, 4 + i, 4 + j))
                        state.Board[4 + i, 4 + j].Distance.ShouldBe(
                            fields.First(t => t.x == 4 + i && t.y == 4 + j).distance);
                }
            }
        }

        private bool IsInBounds(AgentBoard board, int x, int y)
        {
            return x >= 0 && x < board.Width &&
                y >= 0 && y < board.Height;
        }

        [Fact]
        public void ApplyCommunicationResult_WhenBoardAreDifferentSizes_ThrowsInvalidDiscoveryResultException()
        {
            var rules = new GameRules(boardWidth: 8);
            var state = GetSetUpState(rules);
            var resultRules = new GameRules(boardWidth: 10);
            var resultBoard = new AgentBoard(resultRules);
            var communicationResult = new CommunicationResult(resultBoard);

            Should.Throw<InvalidCommunicationResultException>(() => state.ApplyCommunicationResult(communicationResult));
        }

        [Fact]
        public void ApplyCommunicationResult_WhenResultIsNewer_UpdatesBoard()
        {
            int distance = 1;
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            var resultBoard = new AgentBoard(rules);
            SetupCommunicationBoards(state.Board, resultBoard, DateTime.MaxValue, distance);
            var communicationResult = new CommunicationResult(resultBoard);

            state.ApplyCommunicationResult(communicationResult);

            for (int i = 0; i < state.Board.Height; i++)
            {
                for (int j = 0; j < state.Board.Width; j++)
                {
                    state.Board[i, j].Distance.ShouldBe(distance);
                }
            }
        }

        [Fact]
        public void ApplyCommunicationResult_WhenResultIsOlder_DoesntUpdatesBoard()
        {
            int distance = 1;
            var rules = GetDefaultRules();
            var state = GetSetUpState(rules);
            var resultBoard = new AgentBoard(rules);
            SetupCommunicationBoards(state.Board, resultBoard, DateTime.MinValue, distance);
            var communicationResult = new CommunicationResult(resultBoard);

            state.ApplyCommunicationResult(communicationResult);

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
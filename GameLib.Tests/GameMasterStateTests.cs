using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using Shouldly;
using Moq;

namespace GameLib.Tests
{
    public class GameMasterStateTests
    {
        private void Initialize()
        {
            RandomGenerator.Initialize();
        }

        private GameMasterState GetState(GameRules rules)
        {
            Initialize();

            return new GameMasterState(rules);
        }
        
        [Fact]
        public void GeneratePiece_WhenCalled_PlacesPieceOnTheRandomBoardField()
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            var previousNumberOfPieces = state.Board.PieceCount;

            state.GeneratePiece();
            
            state.Board.PieceCount.ShouldBe(previousNumberOfPieces + 1);
            state.IsStateCorrect().ShouldBe(true);
        }

        [Fact]
        public void GeneratePiece_WhenMaximumIsReached_DoesNotPlaceAnotherPiece()
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            for (int i = 0; i < rules.MaxPiecesOnBoard; i++)
            {
                state.GeneratePiece();
            }
            var previousNumberOfPieces = state.Board.PieceCount;

            state.GeneratePiece();
            
            state.Board.PieceCount.ShouldBe(previousNumberOfPieces);
            state.IsStateCorrect().ShouldBe(true);
        }
        
        [Theory]
        [InlineData(1, 1, Direction.Up)]
        [InlineData(1, 1, Direction.Down)]
        [InlineData(1, 1, Direction.Left)]
        [InlineData(1, 1, Direction.Right)]
        public void Move_WhenCalled_ChangesAgentsPositionOnBoard(int agentX, int agentY, Direction direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            var agentId = 0;
            state.PlayerStates.Clear();
            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));

            state.Move(agentId, direction);

            var expectedX = agentX;
            var expectedY = agentY;
            switch (direction)
            {
                case Direction.Left:
                    expectedX--;
                    break;
                case Direction.Right:
                    expectedX++;
                    break;
                //os Y z dolu do gory
                case Direction.Up:
                    expectedY--;
                    break;
                case Direction.Down:
                    expectedY++;
                    break;
            }

            PlayerState playerState = state.PlayerStates.First().Value;
            playerState.Position.X.ShouldBe(expectedX);
            playerState.Position.Y.ShouldBe(expectedY);
        }

        [Theory]
        [InlineData(0, 0, Direction.Left)]
        [InlineData(7, 7, Direction.Right)]
        public void Move_WhenAgentMovesOutsideBoard_ThrowsInvalidMoveException(int agentX, int agentY, Direction direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            var agentId = 0;
            state.PlayerStates.Clear();
            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));

            Should.Throw<InvalidMoveException>(() => state.Move(agentId, direction));
        }

        [Theory]
        [InlineData(1, 1, Direction.Up)]
        [InlineData(1, 1, Direction.Down)]
        [InlineData(1, 1, Direction.Left)]
        [InlineData(1, 1, Direction.Right)]
        public void Move_WhenAgentIsNotEligible_ThrowsDelayException(int agentX, int agentY, Direction direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            var agentId = 0;
            state.PlayerStates.Clear();

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY) { LastRequestTimestamp = DateTime.MaxValue, LastActionDelay = 0 });

            Should.Throw<DelayException>(() => state.Move(agentId, direction));
        }

        [Fact]
        public void Move_WhenAgentMovesOnAnotherAgent_ThrowsInvalidMoveException()
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            var agentId = 0;
            var agentX = 1;
            var agentY = 1;
            var direction = Direction.Left;
            state.PlayerStates.Clear();
            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));
            state.PlayerStates.Add(agentId + 1, new PlayerState(agentX - 1, agentY));

            Should.Throw<InvalidMoveException>(() => state.Move(agentId, direction));
        }

        [Fact]
        public void PickUpPiece_WhenAgentIsNotEligible_ThrowsDelayException()
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            var agentId = 0;
            state.PlayerStates.Clear();

            state.PlayerStates.Add(agentId, new PlayerState(0, 0) { LastRequestTimestamp = DateTime.MaxValue, LastActionDelay = 0 });

            Should.Throw<DelayException>(() => state.PickUpPiece(agentId));
        }

        [Theory]
        [InlineData(3, 5)]
        [InlineData(4, 4)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public void PickUpPiece_WhenNotOnPiece_ThrowsPieceOperationException(int agentX, int agentY)
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);
            state.GeneratePiece();
            state.GeneratePiece();

            var agentId = 0;
            state.PlayerStates.Clear();
            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                if (x != agentX || y != agentY)
                {
                    Should.Throw<PieceOperationException>(() => state.PickUpPiece(agentId));
                }
            }
        }

        [Fact]
        public void PickUpPiece_WhenAnotherPlayerHasPiece_ThrowsPieceOperationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = GetState(rules);

            state.GeneratePiece();
            state.PlayerStates.Clear();
            (int x, int y) piecePosition = state.Board.PiecesPositions[0];

            state.PlayerStates.Add(0, new PlayerState(piecePosition.x, piecePosition.y) { LastActionDelay = 0 });
            state.PlayerStates.Add(1, new PlayerState(1, 1) { LastActionDelay = 0 });

            state.PickUpPiece(0);

            Should.Throw<PieceOperationException>(() => state.PickUpPiece(1));
        }

        [Fact]
        public void CorrectPickUpPiece_ShouldCauseRecalculationOfDistances()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = GetState(rules);

            state.GeneratePieceOnCoordinates(4,5);
            state.GeneratePieceOnCoordinates(1,2);
            state.GeneratePieceOnCoordinates(7,3);

            state.PlayerStates.Clear();
            (int x, int y) firstPiecePosition = state.Board.PiecesPositions[0];

            int agentId = 0;
            state.PlayerStates.Add(agentId, new PlayerState(firstPiecePosition.x, firstPiecePosition.y) { LastActionDelay = 0 });

            state.PickUpPiece(agentId);

            state.Board.Board[0, 0].Distance.ShouldBe(3);
            state.Board.Board[0, 1].Distance.ShouldBe(2);
            state.Board.Board[0, 2].Distance.ShouldBe(1);
            state.Board.Board[0, 3].Distance.ShouldBe(2);
            state.Board.Board[0, 4].Distance.ShouldBe(3);
            state.Board.Board[0, 5].Distance.ShouldBe(4);
            state.Board.Board[0, 6].Distance.ShouldBe(5);
            state.Board.Board[0, 7].Distance.ShouldBe(6);
            state.Board.Board[1, 0].Distance.ShouldBe(2);
            state.Board.Board[1, 1].Distance.ShouldBe(1);
            state.Board.Board[1, 2].Distance.ShouldBe(0);
            state.Board.Board[1, 3].Distance.ShouldBe(1);
            state.Board.Board[1, 4].Distance.ShouldBe(2);
            state.Board.Board[1, 5].Distance.ShouldBe(3);
            state.Board.Board[1, 6].Distance.ShouldBe(4);
            state.Board.Board[1, 7].Distance.ShouldBe(5);
            state.Board.Board[2, 0].Distance.ShouldBe(3);
            state.Board.Board[2, 1].Distance.ShouldBe(2);
            state.Board.Board[2, 2].Distance.ShouldBe(1);
            state.Board.Board[2, 3].Distance.ShouldBe(2);
            state.Board.Board[2, 4].Distance.ShouldBe(3);
            state.Board.Board[2, 5].Distance.ShouldBe(4);
            state.Board.Board[2, 6].Distance.ShouldBe(5);
            state.Board.Board[2, 7].Distance.ShouldBe(6);
            state.Board.Board[3, 0].Distance.ShouldBe(4);
            state.Board.Board[3, 1].Distance.ShouldBe(3);
            state.Board.Board[3, 2].Distance.ShouldBe(2);
            state.Board.Board[3, 3].Distance.ShouldBe(3);
            state.Board.Board[3, 4].Distance.ShouldBe(4);
            state.Board.Board[3, 5].Distance.ShouldBe(5);
            state.Board.Board[3, 6].Distance.ShouldBe(6);
            state.Board.Board[3, 7].Distance.ShouldBe(7);
            state.Board.Board[4, 0].Distance.ShouldBe(5);
            state.Board.Board[4, 1].Distance.ShouldBe(4);
            state.Board.Board[4, 2].Distance.ShouldBe(3);
            state.Board.Board[4, 3].Distance.ShouldBe(3);
            state.Board.Board[4, 4].Distance.ShouldBe(4);
            state.Board.Board[4, 5].Distance.ShouldBe(5);
            state.Board.Board[4, 6].Distance.ShouldBe(6);
            state.Board.Board[4, 7].Distance.ShouldBe(7);
            state.Board.Board[5, 0].Distance.ShouldBe(5);
            state.Board.Board[5, 1].Distance.ShouldBe(4);
            state.Board.Board[5, 2].Distance.ShouldBe(3);
            state.Board.Board[5, 3].Distance.ShouldBe(2);
            state.Board.Board[5, 4].Distance.ShouldBe(3);
            state.Board.Board[5, 5].Distance.ShouldBe(4);
            state.Board.Board[5, 6].Distance.ShouldBe(5);
            state.Board.Board[5, 7].Distance.ShouldBe(6);
            state.Board.Board[6, 0].Distance.ShouldBe(4);
            state.Board.Board[6, 1].Distance.ShouldBe(3);
            state.Board.Board[6, 2].Distance.ShouldBe(2);
            state.Board.Board[6, 3].Distance.ShouldBe(1);
            state.Board.Board[6, 4].Distance.ShouldBe(2);
            state.Board.Board[6, 5].Distance.ShouldBe(3);
            state.Board.Board[6, 6].Distance.ShouldBe(4);
            state.Board.Board[6, 7].Distance.ShouldBe(5);
            state.Board.Board[7, 0].Distance.ShouldBe(3);
            state.Board.Board[7, 1].Distance.ShouldBe(2);
            state.Board.Board[7, 2].Distance.ShouldBe(1);
            state.Board.Board[7, 3].Distance.ShouldBe(0);
            state.Board.Board[7, 4].Distance.ShouldBe(1);
            state.Board.Board[7, 5].Distance.ShouldBe(2);
            state.Board.Board[7, 6].Distance.ShouldBe(3);
            state.Board.Board[7, 7].Distance.ShouldBe(4);
        }

        [Fact]
        public void Piece_WhenGenerated_ShouldBeInTaskArea()
        {
            GameRules rules = Helper.GetDefaultRules();
            var state = GetState(rules);

            state.GeneratePiece();
            state.GeneratePiece();
            state.GeneratePiece();

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                bool inTaskArea = x >= rules.GoalAreaHeight && x < rules.BoardHeight - rules.GoalAreaHeight;
                state.Board.Board[x, y].HasPiece.ShouldBe(inTaskArea);
            }
        }

        [Fact]
        public void PieceIsValid_WhenProbabilityIs_1()
        {
            GameRules rules = Helper.GetAlwaysValidPieceRules();
            var state = GetState(rules);

            state.GeneratePiece();

            foreach(var (x, y) in state.Board.PiecesPositions)
            {
                state.Board.Board[x, y].IsGoal.ShouldBe(true);
            }

        }

        [Fact]
        public void PieceIsInvalid_WhenProbabilityIs_0()
        {
            GameRules rules = Helper.GetAlwaysInvalidPieceRules();
            var state = GetState(rules);

            state.GeneratePiece();

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                state.Board.Board[x, y].IsGoal.ShouldBe(false);
            }
        }
    }
}

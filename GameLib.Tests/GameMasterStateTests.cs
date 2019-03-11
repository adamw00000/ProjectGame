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

            int[,] results = { {3,2,1,2,3,4,5,6}, 
                               {2,1,0,1,2,3,4,5}, 
                               {3,2,1,2,3,4,5,6}, 
                               {4,3,2,3,4,5,6,7},
                               {5,4,3,3,4,5,6,7},
                               {5,4,3,2,3,4,5,6},
                               {4,3,2,1,2,3,4,5},
                               {3,2,1,0,1,2,3,4}};

            for (int x = 0; x < state.Board.Width; x++)
            {
                for (int y = 0; y < state.Board.Height; y++)
                {
                    state.Board.Board[x, y].Distance.ShouldBe(results[x,y]);
                }
            }
        }

        [Fact]
        public void AfterPickingUpOnlyPieceOnBoard_DistancesShouldBeNegative_AndPieceCountShouldBeOne()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = GetState(rules);

            state.GeneratePieceOnCoordinates(4, 5);


            state.PlayerStates.Clear();
            (int x, int y) firstPiecePosition = state.Board.PiecesPositions[0];

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(firstPiecePosition.x, firstPiecePosition.y) { LastActionDelay = 0 });

            state.PickUpPiece(agentId);

            for (int x = 0; x < state.Board.Width; x++)
            {
                for (int y = 0; y < state.Board.Height; y++)
                {
                    state.Board.Board[x, y].Distance.ShouldBe(-1);
                }
            }
            state.Board.PieceCount.ShouldBe(1);
        }

        [Fact]
        public void PutPieceWithoutHavingOne_ThrowsPieceOperationException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = GetState(rules);

            state.GeneratePiece();


            state.PlayerStates.Clear();
            (int x, int y) firstPiecePosition = state.Board.PiecesPositions[0];

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(firstPiecePosition.x, firstPiecePosition.y) { LastActionDelay = 0 });

            Should.Throw<PieceOperationException>(() => state.PutPiece(agentId));
        }

        [Fact]
        public void PutPieceOnAnotherPiece_ThrowsPieceOperationException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = GetState(rules);

            state.GeneratePieceOnCoordinates(4, 5);


            state.PlayerStates.Clear();
            (int x, int y) firstPiecePosition = state.Board.PiecesPositions[0];

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(firstPiecePosition.x, firstPiecePosition.y) { LastActionDelay = 0, Piece = new Piece(0.5) });

            Should.Throw<PieceOperationException>(() => state.PutPiece(agentId)); 
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

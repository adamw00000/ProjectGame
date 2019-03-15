using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace GameLib.Tests
{
    public class GameMasterStateTests
    {
        #region --Generate--

        [Fact]
        public void GeneratePiece_WhenSucceeded_PlacesPieceOnTheRandomBoardField()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var previousNumberOfPieces = state.Board.PieceCount;

            state.GeneratePiece();

            state.Board.PieceCount.ShouldBe(previousNumberOfPieces + 1);
        }

        [Fact]
        public void GeneratePiece_WhenMaximumIsReached_DoesNotPlaceAnotherPiece()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            for (int i = 0; i < rules.MaxPiecesOnBoard; i++)
            {
                state.GeneratePiece();
            }
            var previousNumberOfPieces = state.Board.PieceCount;

            state.GeneratePiece();

            state.Board.PieceCount.ShouldBe(previousNumberOfPieces);
        }

        [Fact]
        public void Piece_WhenGenerated_ShouldBeInTaskArea()
        {
            GameRules rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();
            state.GeneratePiece();
            state.GeneratePiece();

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                bool inTaskArea = x >= rules.GoalAreaHeight && x < rules.BoardHeight - rules.GoalAreaHeight;
                state.Board.BoardTable[x, y].HasPiece.ShouldBe(inTaskArea);
            }
        }

        [Fact]
        public void GeneratedPiece_WhenProbabilityIs1_IsValid()
        {
            GameRules rules = Helper.GetAlwaysValidPieceRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                state.Board.BoardTable[x, y].HasValidPiece.ShouldBe(true);
            }
        }

        [Fact]
        public void GeneratedPiece_WhenProbabilityIs0_IsNotValid()
        {
            GameRules rules = Helper.GetAlwaysInvalidPieceRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                state.Board.BoardTable[x, y].HasValidPiece.ShouldBe(false);
            }
        }

        #endregion --Generate--

        #region --Move--

        [Theory]
        [InlineData(1, 1, MoveDirection.Up)]
        [InlineData(1, 1, MoveDirection.Down)]
        [InlineData(1, 1, MoveDirection.Left)]
        [InlineData(1, 1, MoveDirection.Right)]
        public void Move_WhenSucceeded_ChangesAgentsPositionOnBoard(int agentX, int agentY, MoveDirection direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));

            state.Move(agentId, direction);

            var expectedX = agentX;
            var expectedY = agentY;
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

            PlayerState playerState = state.PlayerStates.First().Value;
            playerState.Position.X.ShouldBe(expectedX);
            playerState.Position.Y.ShouldBe(expectedY);
        }

        [Fact]
        public void Move_WhenSucceeded_AppliesMoveDelay()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var agentId = 0;
            int agentX = 2;
            int agentY = 2;
            var direction = MoveDirection.Left;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));
            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.Move(agentId, direction);

            var expectedDelay = rules.BaseTimePenalty * rules.MoveMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        [Theory]
        [InlineData(0, 0, MoveDirection.Left)]
        [InlineData(7, 7, MoveDirection.Right)]
        public void Move_WhenAgentMovesOutsideBoard_ThrowsInvalidMoveException(int agentX, int agentY, MoveDirection direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));

            Should.Throw<InvalidMoveException>(() => state.Move(agentId, direction), "Agent tried to move out of board!");
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Fact]
        public void Move_WhenAgentMovesOnAnotherAgent_ThrowsInvalidMoveException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;
            var agentX = 1;
            var agentY = 1;
            var direction = MoveDirection.Up;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));
            state.PlayerStates.Add(agentId + 1, new PlayerState(agentX - 1, agentY));

            Should.Throw<InvalidMoveException>(() => state.Move(agentId, direction), "Agent tried to on the space occupied by another agent!");
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Theory]
        [InlineData(5, 0, Team.Blue, MoveDirection.Down)]
        [InlineData(2, 2, Team.Red, MoveDirection.Up)]
        public void Move_WhenAgentMovesToEnemyGoalArea_ThrowsInvalidMoveException(int agentX, int agentY, Team team, MoveDirection direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY, team));

            Should.Throw<InvalidMoveException>(() => state.Move(agentId, direction), "Agent tried to move onto enemy goal area!");
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Theory]
        [InlineData(1, 1, MoveDirection.Up)]
        [InlineData(1, 1, MoveDirection.Down)]
        [InlineData(1, 1, MoveDirection.Left)]
        [InlineData(1, 1, MoveDirection.Right)]
        public void Move_WhenAgentIsNotEligible_ThrowsDelayException(int agentX, int agentY, MoveDirection direction)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY) { LastRequestTimestamp = DateTime.MaxValue, LastActionDelay = 0 });

            Should.Throw<DelayException>(() => state.Move(agentId, direction));
        }

        #endregion --Move--

        #region --PickUpPiece--

        [Fact]
        public void PickUpPiece_WhenAgentIsNotEligible_ThrowsDelayException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

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
            var state = Helper.GetGameMasterState(rules);
            state.GeneratePieceAt(3, 4);
            state.GeneratePieceAt(5, 4);

            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY));

            foreach (var (x, y) in state.Board.PiecesPositions)
            {
                if (x != agentX || y != agentY)
                {
                    Should.Throw<PieceOperationException>(() => state.PickUpPiece(agentId), "No piece on this field!");
                }
            }
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Fact]
        public void PickUpPiece_WhenSucceeded_ShouldCauseRecalculationOfDistances()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePieceAt(4, 5);
            state.GeneratePieceAt(1, 2);
            state.GeneratePieceAt(7, 3);

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
                    state.Board.BoardTable[x, y].Distance.ShouldBe(results[x, y]);
                }
            }
        }

        [Fact]
        public void PickUpPiece_WhenSucceeded_AssignsPieceToAgent()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();
            (int x, int y) = state.Board.PiecesPositions[0];
            var expectedPiece = state.Board[x, y].Piece;

            int agentId = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0 });

            state.PickUpPiece(agentId);

            var playerState = state.PlayerStates[0];
            playerState.Piece.ShouldBe(expectedPiece);
        }

        [Fact]
        public void PickUpPiece_WhenSucceeded_RemovesPieceFromBoard()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();
            (int x, int y) = state.Board.PiecesPositions[0];
            var expectedPiece = state.Board[x, y].Piece;

            int agentId = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0 });

            state.PickUpPiece(agentId);

            state.Board[x, y].HasPiece.ShouldBe(false);
        }

        [Fact]
        public void PickUpPiece_WhenSucceeded_AppliesPickUpDelay()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();
            (int x, int y) = state.Board.PiecesPositions[0];

            int agentId = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0 });
            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.PickUpPiece(agentId);

            var expectedDelay = rules.BaseTimePenalty * rules.PickUpMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        [Fact]
        public void PickUpPiece_WhenPickingUpOnlyPieceOnBoard_DistancesShouldBeNegativeAndPieceCountShouldBeOne()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePieceAt(4, 5);

            (int x, int y) firstPiecePosition = state.Board.PiecesPositions[0];

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(firstPiecePosition.x, firstPiecePosition.y) { LastActionDelay = 0 });

            state.PickUpPiece(agentId);

            for (int x = 0; x < state.Board.Width; x++)
            {
                for (int y = 0; y < state.Board.Height; y++)
                {
                    state.Board.BoardTable[x, y].Distance.ShouldBe(-1);
                }
            }
            state.Board.PieceCount.ShouldBe(1);
        }

        #endregion --PickUpPiece--

        #region --PutPiece--

        [Fact]
        public void PutPiece_WhenAgentDoesntHaveOne_ThrowsPieceOperationException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePiece();

            (int x, int y) = state.Board.PiecesPositions[0];

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0 });

            Should.Throw<PieceOperationException>(() => state.PutPiece(agentId), "Player doesn't have a piece!");
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Fact]
        public void PutPiece_WhenOnAnotherPiece_ThrowsPieceOperationException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.GeneratePieceAt(4, 5);

            (int x, int y) = state.Board.PiecesPositions[0];

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0, Piece = new Piece(0.5) });

            Should.Throw<PieceOperationException>(() => state.PutPiece(agentId), "Cannot put another piece on this field!");
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Fact]
        public void PutPieceInGoalArea_WhenSucceeded_DestroysPiece()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(0, 0) { LastActionDelay = 0, Piece = new Piece(0.5) });

            state.PutPiece(agentId);

            state.PlayerStates[agentId].Piece.ShouldBe(null);
            state.Board[0, 0].HasPiece.ShouldBe(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PutPieceInGoalArea_WhenPieceIsValid_ReturnsGoalInformation(bool isGoal)
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.Board[0, 0] = new GameMasterField() { IsGoal = isGoal };
            state.PlayerStates.Add(agentId, new PlayerState(0, 0) { LastActionDelay = 0, Piece = new Piece(1) });
            var expectedResult = isGoal ? PutPieceResult.PieceGoalRealized : PutPieceResult.PieceGoalUnrealized;

            var result = state.PutPiece(agentId);

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void PutPieceInGoalArea_WhenPieceIsInvalid_ReturnsNoInformationAboutGoal()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.Board[0, 0] = new GameMasterField() { IsGoal = true };
            state.PlayerStates.Add(agentId, new PlayerState(0, 0) { LastActionDelay = 0, Piece = new Piece(0) });

            var result = state.PutPiece(agentId);

            result.ShouldBe(PutPieceResult.PieceWasFake);
        }

        [Theory]
        [InlineData(4, 4)]
        [InlineData(3, 5)]
        [InlineData(2, 0)]
        public void PutPieceInTaskArea_PutsPieceBackOnBoard_AndPlayerDoesntHaveIt(int x, int y)
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0, Piece = new Piece(0.5) });

            var result = state.PutPiece(agentId);

            result.ShouldBe(PutPieceResult.PieceInTaskArea);
            state.Board[x, y].HasPiece.ShouldBe(true);
            state.PlayerStates[agentId].Piece.ShouldBe(null);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 2)]
        [InlineData(4, 4)]
        public void PutPiece_WhenSucceeded_AppliesPutPieceDelay(int x, int y)
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(x, y, Team.Blue) { LastActionDelay = 0, Piece = new Piece(0.5) });

            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            var result = state.PutPiece(agentId);

            var expectedDelay = rules.BaseTimePenalty * rules.PutPieceMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        #endregion --PutPiece--

        #region --DestroyPiece--

        [Fact]
        public void DestroyPiece_IfAgentHasDelay_ShouldThrowDelayException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastRequestTimestamp = DateTime.UtcNow, LastActionDelay = 1000 * 3600 * 24, Piece = new Piece(0.5) });

            Should.Throw<DelayException>(() => state.DestroyPiece(agentId), "Player doesn't have a piece!");
        }

        [Fact]
        public void DestroyPiece_IfAgentDoesntHavePiece_ShouldThrowPieceOperationException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0, Piece = null });

            Should.Throw<PieceOperationException>(() => state.DestroyPiece(agentId));
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Fact]
        public void DestroyPiece_WhenSucceeded_ShouldDestroyPiece()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0, Piece = new Piece(0.5) });
            int piecesCount = state.Board.PieceCount;

            state.DestroyPiece(agentId);

            PlayerState player = state.PlayerStates[agentId];
            player.Piece.ShouldBeNull();
            state.Board[x, y].HasPiece.ShouldBe(false);
            state.Board.PieceCount.ShouldBe(piecesCount - 1);
        }

        [Fact]
        public void DestroyPiece_WhenSucceeded_AppliesDestroyDelay()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0, Piece = new Piece(0.5) });
            int piecesCount = state.Board.PieceCount;
            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.DestroyPiece(agentId);

            var expectedDelay = rules.BaseTimePenalty * rules.DestroyPieceMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        #endregion --DestroyPiece--

        #region --CheckPiece--

        [Fact]
        public void CheckPiece_IfAgentHasDelay_ShouldThrowDelayException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastRequestTimestamp = DateTime.UtcNow, LastActionDelay = 1000 * 3600 * 24, Piece = new Piece(0.5) });

            Should.Throw<DelayException>(() => state.CheckPiece(agentId), "Player doesn't have a piece!");
        }

        [Fact]
        public void CheckPiece_IfAgentDoesntHavePiece_ShouldThrowPieceOperationException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0, Piece = null });

            Should.Throw<PieceOperationException>(() => state.CheckPiece(agentId));
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CheckPiece_WhenSucceeded_ReturnsCorrectPieceInformation(bool isValid)
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            double validProbability = isValid ? 1 : 0;
            state.PlayerStates.Add(agentId, new PlayerState(0, 0) { LastActionDelay = 0, Piece = new Piece(validProbability) });

            var result = state.CheckPiece(agentId);

            result.ShouldBe(isValid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CheckPiece_WhenSucceeded_AppliesCheckDelay(bool isValid)
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            double validProbability = isValid ? 1 : 0;
            state.PlayerStates.Add(agentId, new PlayerState(0, 0) { LastActionDelay = 0, Piece = new Piece(validProbability) });
            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.CheckPiece(agentId);

            var expectedDelay = rules.BaseTimePenalty * rules.CheckMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        #endregion --CheckPiece--

        #region --Discover--

        [Fact]
        public void Discover_IfAgentHasDelay_ShouldThrowDelayException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;
            int x = 0;
            int y = 0;
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastRequestTimestamp = DateTime.UtcNow, LastActionDelay = 1000 * 3600 * 24 });

            Should.Throw<DelayException>(() => state.Discover(agentId));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 2)]
        [InlineData(0, 7)]
        [InlineData(5, 7)]
        [InlineData(7, 7)]
        [InlineData(7, 6)]
        [InlineData(7, 0)]
        [InlineData(3, 0)]
        [InlineData(2, 1)]
        [InlineData(4, 4)]
        public void Discover_WhenSucceeded_ReturnsInformationsAboutSurroundingFields(int x, int y)
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.GeneratePieceAt(1, 2);
            state.GeneratePieceAt(7, 3);
            state.PlayerStates.Add(agentId, new PlayerState(x, y) { LastActionDelay = 0 });

            int[,] discoveryResult = state.Discover(agentId);
            int[,] distances = { {3,2,1,2,3,4,5,6},
                                 {2,1,0,1,2,3,4,5},
                                 {3,2,1,2,3,4,5,6},
                                 {4,3,2,3,4,5,6,7},
                                 {5,4,3,3,4,5,6,7},
                                 {5,4,3,2,3,4,5,6},
                                 {4,3,2,1,2,3,4,5},
                                 {3,2,1,0,1,2,3,4}};

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 && x + i < rules.BoardWidth && y + j >= 0 && y + j < rules.BoardHeight)
                    {
                        discoveryResult[i + 1, j + 1].ShouldBe(distances[x + i, y + j]);
                    }
                    else
                    {
                        discoveryResult[i + 1, j + 1].ShouldBe(int.MaxValue);
                    }
                }
            }
        }

        [Fact]
        public void Discover_WhenSucceeded_AppliesDiscoverDelay()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int agentId = 0;

            state.GeneratePieceAt(1, 2);
            state.GeneratePieceAt(7, 3);
            state.PlayerStates.Add(agentId, new PlayerState(4, 4) { LastActionDelay = 0 });
            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.Discover(agentId);

            var expectedDelay = rules.BaseTimePenalty * rules.DiscoverMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        #endregion --Discover--

        #region --Communicate--

        [Fact]
        public void Communicate_WhenAgentHasDelay_ThrowsDelayException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var agentId = 0;
            state.PlayerStates.Add(agentId, new PlayerState() { LastRequestTimestamp = DateTime.UtcNow, LastActionDelay = 1000 * 3600 * 24 });

            Should.Throw<DelayException>(() => state.Communicate(agentId));
        }

        [Fact]
        public void Communicate_WhenSucceeded_AppliesCommunicationDelay()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var agentId = 0;
            state.PlayerStates.Add(agentId, new PlayerState());

            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.Communicate(agentId);

            var expectedDelay = rules.BaseTimePenalty * rules.CommunicationMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        #endregion --Communicate--

        #region --Initialization--

        [Fact]
        public void InitializingPlayersPositionsOnEvenSizedBoard_WhenCalled_SetsInitialPositions()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, rules.BoardWidth);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[0, i].ShouldBe(true);
                positions[rules.BoardHeight - 1, i].ShouldBe(true);
            }
        }

        [Fact]
        public void InitializingPlayersPositionsOnOddSizedBoard_WhenCalled_SetsInitialPositions()
        {
            var rules = Helper.GetOddSizeBoardRules();
            var state = Helper.GetGameMasterState(rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, rules.BoardWidth);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[0, i].ShouldBe(true);
                positions[rules.BoardHeight - 1, i].ShouldBe(true);
            }
        }

        [Fact]
        public void InitializingPositionsWithWeirdNumberOfPlayers_WhenCalled_SetsInitialPositions()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, 12);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[0, i].ShouldBe(true);
                positions[rules.BoardHeight - 1, i].ShouldBe(true);
            }
            for (int i = 0; i < 2; i++)
            {
                positions[1, i].ShouldBe(false);
                positions[rules.BoardHeight - 2, i].ShouldBe(false);
            }
            for (int i = 2; i < 6; i++)
            {
                positions[1, i].ShouldBe(true);
                positions[rules.BoardHeight - 2, i].ShouldBe(true);
            }
            for (int i = 6; i < 8; i++)
            {
                positions[1, i].ShouldBe(false);
                positions[rules.BoardHeight - 2, i].ShouldBe(false);
            }
        }

        [Fact]
        public void InitializingPlayersPositions_WhenCalled_SetsInitialPositionsInTwoRows()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, 2 * rules.BoardWidth);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[0, i].ShouldBe(true);
                positions[1, i].ShouldBe(true);
                positions[rules.BoardHeight - 1, i].ShouldBe(true);
                positions[rules.BoardHeight - 2, i].ShouldBe(true);
            }
        }

        #endregion --Initialization--
    }
}
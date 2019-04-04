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
                bool inTaskArea = y >= rules.GoalAreaHeight && y < rules.BoardHeight - rules.GoalAreaHeight;
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

            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY, Team.Blue));

            state.Move(agentId, direction);

            var expectedX = agentX;
            var expectedY = agentY;
            switch (direction)
            {
                case MoveDirection.Left:
                    expectedX--;
                    break;

                case MoveDirection.Right:
                    expectedX++;
                    break;

                case MoveDirection.Up:
                    expectedY++;
                    break;

                case MoveDirection.Down:
                    expectedY--;
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
        [InlineData(0, 0, MoveDirection.Down)]
        [InlineData(7, 7, MoveDirection.Right)]
        [InlineData(7, 7, MoveDirection.Up)]
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
            state.PlayerStates.Add(agentId + 1, new PlayerState(agentX, agentY + 1));

            Should.Throw<InvalidMoveException>(() => state.Move(agentId, direction), "Agent tried to on the space occupied by another agent!");
            state.PlayerStates[0].LastActionDelay.ShouldBe(0);
        }

        [Theory]
        [InlineData(0, 2, Team.Red, MoveDirection.Down)]
        [InlineData(2, 5, Team.Blue, MoveDirection.Up)]
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

        [Theory]
        [InlineData(0, 5, MoveDirection.Right, Team.Blue, 0)]
        [InlineData(1, 3, MoveDirection.Right, Team.Red, 2)]
        [InlineData(1, 7, MoveDirection.Down, Team.Red, 1)]
        [InlineData(2, 5, MoveDirection.Up, Team.Red, 2)]
        [InlineData(4, 3, MoveDirection.Right, Team.Blue, 1)]
        [InlineData(4, 5, MoveDirection.Down, Team.Blue, 1)]
        [InlineData(7, 7, MoveDirection.Left, Team.Red, 6)]
        public void Move_WhenSucceded_ReturnsDistanceToClosestPiece(int agentX, int agentY, MoveDirection direction, Team agentTeam, int expected)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.GeneratePieceAt(4, 3);
            state.GeneratePieceAt(1, 5);
            state.PlayerStates.Add(agentId, new PlayerState(agentX, agentY, agentTeam));

            int result = state.Move(agentId, direction);

            result.ShouldBe(expected);
        }

        [Fact]
        public void Move_WhenAgentHavePendingLeaderCommunication_ThrowsPendingLeaderCommunicationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;
            var direction = MoveDirection.Down;

            state.PlayerStates.Add(agentId, new PlayerState(1, 1) { PendingLeaderCommunication = true });

            Should.Throw<PendingLeaderCommunicationException>(() => state.Move(agentId, direction));
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

        [Fact]
        public void PickUpPiece_WhenPlayerAlreadyHasPiece_ThrowsPieceOperationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int pieceX = 3;
            int pieceY = 4;
            state.GeneratePieceAt(pieceX, pieceY);

            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(pieceX, pieceY) { Piece = new Piece(0.5) });
            Should.Throw<PieceOperationException>(() => state.PickUpPiece(agentId), "Cannot pick up piece if you already have one!");
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

            // [x, y] := Goal Areas are on left and right
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

            var expectedDelay = rules.BaseTimePenalty * rules.PickUpPieceMultiplier;
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

        [Fact]
        public void PickUpPiece_WhenAgentHavePendingLeaderCommunication_ThrowsPendingLeaderCommunicationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;
            state.Board.BoardTable[1, 1].Piece = new Piece(1.0);

            state.PlayerStates.Add(agentId, new PlayerState(1, 1) { PendingLeaderCommunication = true });

            Should.Throw<PendingLeaderCommunicationException>(() => state.PickUpPiece(agentId));
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

            state.PlayerStates.Add(agentId, new PlayerState(0, 0, Team.Blue) { LastActionDelay = 0, Piece = new Piece(0.5) });

            state.PutPiece(agentId);

            state.PlayerStates[agentId].Piece.ShouldBe(null);
            state.Board[0, 0].HasPiece.ShouldBe(false);
        }

        [Fact]
        public void PutPieceInGoalArea_WhenSucceeded_LowersNumberOfUndiscoveredGoals()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            int undiscoveredBlueGoals = state.UndiscoveredBlueGoalsLeft;

            int agentId = 0;

            state.Board[0, 0] = new GameMasterField() { IsGoal = true };
            state.PlayerStates.Add(agentId, new PlayerState(0, 0, Team.Blue) { LastActionDelay = 0, Piece = new Piece(1) });

            state.PutPiece(agentId);

            state.UndiscoveredBlueGoalsLeft.ShouldBe(undiscoveredBlueGoals - 1);
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
            state.PlayerStates.Add(agentId, new PlayerState(0, 0, Team.Blue) { LastActionDelay = 0, Piece = new Piece(1) });
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
            state.PlayerStates.Add(agentId, new PlayerState(0, 0, Team.Blue) { LastActionDelay = 0, Piece = new Piece(0) });

            var result = state.PutPiece(agentId);

            result.ShouldBe(PutPieceResult.PieceWasFake);
        }

        [Fact]
        public void DiscoveringAllGoals_EndsGame()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.UndiscoveredRedGoalsLeft.ShouldBe(rules.GoalCount);
            state.UndiscoveredBlueGoalsLeft.ShouldBe(rules.GoalCount);

            int idCounter = 0;

            for (int y = 0; y < state.Board.GoalAreaHeight; y++) //sets red players where red goals are
            {
                for (int x = 0; x < state.Board.Width; x++)
                {
                    if (state.Board[x, y].IsGoal)
                    {
                        state.PlayerStates.Add(idCounter++, new PlayerState(x, y, Team.Blue) { LastActionDelay = 0, Piece = new Piece(1) });
                    }
                }
            }
            for (int y = state.Board.Height - state.Board.GoalAreaHeight; y < state.Board.Height; y++) //sets blue players where blue goals are
            {
                for (int x = 0; x < state.Board.Width; x++)
                {
                    if (state.Board[x, y].IsGoal)
                    {
                        state.PlayerStates.Add(idCounter++, new PlayerState(x, y, Team.Red) { LastActionDelay = 0, Piece = new Piece(1) });
                    }
                }
            }

            for (int i = 0; i < state.PlayerStates.Count; i++) //every player puts valid piece on goal
            {
                var result = state.PutPiece(i);
                result.ShouldBe(PutPieceResult.PieceGoalRealized);
                state.Board[state.PlayerStates[i].Position.X, state.PlayerStates[i].Position.Y].IsGoal.ShouldBe(false);
            }

            state.UndiscoveredRedGoalsLeft.ShouldBe(0);
            state.UndiscoveredBlueGoalsLeft.ShouldBe(0);
            state.GameEnded.ShouldBe(true);
        }

        [Theory]
        [InlineData(4, 4)]
        [InlineData(5, 3)]
        [InlineData(0, 2)]
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

        [Fact]
        public void PutPiece_WhenAgentHavePendingLeaderCommunication_ThrowsPendingLeaderCommunicationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(1, 1) { PendingLeaderCommunication = true, Piece = new Piece(1.0) });

            Should.Throw<PendingLeaderCommunicationException>(() => state.PutPiece(agentId));
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

        [Fact]
        public void DestroyPiece_WhenAgentHavePendingLeaderCommunication_ThrowsPendingLeaderCommunicationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(1, 1) { PendingLeaderCommunication = true, Piece = new Piece(1.0) });

            Should.Throw<PendingLeaderCommunicationException>(() => state.DestroyPiece(agentId));
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

            var expectedDelay = rules.BaseTimePenalty * rules.CheckPieceMultiplier;
            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBe(expectedDelay);
        }

        [Fact]
        public void CheckPiece_WhenAgentHavePendingLeaderCommunication_ThrowsPendingLeaderCommunicationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(1, 1) { PendingLeaderCommunication = true, Piece = new Piece(1.0) });

            Should.Throw<PendingLeaderCommunicationException>(() => state.CheckPiece(agentId));
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

            DiscoveryResult discoveryResult = state.Discover(agentId);
            // [x, y]
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
                        discoveryResult.Fields.Single(t => t.x == x + i && t.y == y + j).distance.ShouldBe(distances[x + i, y + j]);
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

        [Fact]
        public void Discover_WhenAgentHavePendingLeaderCommunication_ThrowsPendingLeaderCommunicationException()
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);
            var agentId = 0;

            state.PlayerStates.Add(agentId, new PlayerState(1, 1) { PendingLeaderCommunication = true });

            Should.Throw<PendingLeaderCommunicationException>(() => state.Discover(agentId));
        }

        #endregion --Discover--

        #region --Communicate--

        [Fact]
        public void DelayCommunicationPartners_WhenCalled_AppliesCommunicationDelayToBothAgents()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var senderId = 0;
            var targetId = 1;
            var previousSenderDelay = 1000 * 3600 * 24;
            state.PlayerStates.Add(senderId, new PlayerState(-1, -1) { LastRequestTimestamp = DateTime.UtcNow, LastActionDelay = previousSenderDelay });
            state.PlayerStates.Add(targetId, new PlayerState(-1, -1));

            var beforeTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

            state.DelayCommunicationPartners(senderId, targetId);

            var expectedTargetDelay = rules.BaseTimePenalty * rules.CommunicationMultiplier;
            var expectedSenderDelayMin = expectedTargetDelay;
            var expectedSenderDelayMax = previousSenderDelay + expectedTargetDelay;

            state.PlayerStates[0].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[0].LastActionDelay.ShouldBeGreaterThanOrEqualTo(expectedSenderDelayMin);
            state.PlayerStates[0].LastActionDelay.ShouldBeLessThanOrEqualTo(expectedSenderDelayMax);

            state.PlayerStates[1].LastRequestTimestamp.ShouldBeGreaterThan(beforeTimestamp);
            state.PlayerStates[1].LastActionDelay.ShouldBe(expectedTargetDelay);
        }

        [Fact]
        public void SaveCommunicationData_WhenSenderIsDelayed_ThrowsDelayException()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var senderId = 0;
            var targetId = 1;
            var previousSenderDelay = 1000 * 3600 * 24;
            state.PlayerStates.Add(senderId, new PlayerState(-1, -1) { LastRequestTimestamp = DateTime.UtcNow, LastActionDelay = previousSenderDelay });
            state.PlayerStates.Add(targetId, new PlayerState(-1, -1));

            object message1 = 15;

            Should.Throw<DelayException>(() => state.SaveCommunicationData(senderId, targetId, message1, ""));
        }

        [Fact]
        public void GetCommunicationData_WhenCalled_ReturnsMostRecentDataForThePair()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.JoinGame(0, 0, false);
            state.JoinGame(1, 0, false);

            var senderId = 0;
            var targetId = 1;

            object message1 = 15;
            object message2 = 125;

            state.SaveCommunicationData(senderId, targetId, message1, "1");
            state.SaveCommunicationData(senderId, targetId, message2, "2");

            var result = state.GetCommunicationData(senderId, targetId);
            result.ShouldBe((message2, "2"));
        }

        [Fact]
        public void SaveCommunicationData_WhenCalled_SetsPendingCommunicationWithLeader()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var senderId = 0;
            var targetId = 1;
            state.PlayerStates.Add(senderId, new PlayerState(-1, -1, isLeader: true));
            state.PlayerStates.Add(targetId, new PlayerState(-1, -1));

            state.SaveCommunicationData(senderId, targetId, "", "");

            state.PlayerStates[targetId].PendingLeaderCommunication.ShouldBe(true);
        }

        [Fact]
        public void DelayCommunicationPartners_WhenCalled_RemovesPendingCommunicationWithLeader()
        {
            var rules = Helper.GetStaticDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            var senderId = 0;
            var targetId = 1;
            state.PlayerStates.Add(senderId, new PlayerState(-1, -1, isLeader: true));
            state.PlayerStates.Add(targetId, new PlayerState(-1, -1));

            state.SaveCommunicationData(senderId, targetId, "", "");
            state.DelayCommunicationPartners(senderId, targetId);

            state.PlayerStates[targetId].PendingLeaderCommunication.ShouldBe(false);
        }

        #endregion --Communicate--

        #region --Initialization--

        [Fact]
        public void InitializingPlayersPositionsOnEvenSizedBoard_WhenCalled_SetsInitialPositions()
        {
            var rules = Helper.GetDefaultRules(8);
            var state = Helper.GetGameMasterState(rules);
            Helper.AddPlayers(state, rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, teamSize: rules.BoardWidth);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[i, 0].ShouldBe(true);
                positions[i, rules.BoardHeight - 1].ShouldBe(true);
            }
        }

        [Fact]
        public void InitializingPlayersPositionsOnOddSizedBoard_WhenCalled_SetsInitialPositions()
        {
            var rules = Helper.GetOddSizeBoardRules(7);
            var state = Helper.GetGameMasterState(rules);
            Helper.AddPlayers(state, rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, teamSize: rules.BoardWidth);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[i, 0].ShouldBe(true);
                positions[i, rules.BoardHeight - 1].ShouldBe(true);
            }
        }

        [Fact]
        public void InitializingPositionsWithWeirdNumberOfPlayers_WhenCalled_SetsInitialPositions()
        {
            var rules = Helper.GetDefaultRules(12);
            var state = Helper.GetGameMasterState(rules);
            Helper.AddPlayers(state, rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, 12);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[i, 0].ShouldBe(true);
                positions[i, rules.BoardHeight - 1].ShouldBe(true);
            }
            for (int i = 0; i < 2; i++)
            {
                positions[i, 1].ShouldBe(false);
                positions[i, rules.BoardHeight - 2].ShouldBe(false);
            }
            for (int i = 2; i < 6; i++)
            {
                positions[i, 1].ShouldBe(true);
                positions[i, rules.BoardHeight - 2].ShouldBe(true);
            }
            for (int i = 6; i < 8; i++)
            {
                positions[i, 1].ShouldBe(false);
                positions[i, rules.BoardHeight - 2].ShouldBe(false);
            }
        }

        [Fact]
        public void InitializingPlayersPositions_WhenCalled_SetsInitialPositionsInTwoRows()
        {
            var rules = Helper.GetDefaultRules(16);
            var state = Helper.GetGameMasterState(rules);
            Helper.AddPlayers(state, rules);

            state.InitializePlayerPositions(rules.BoardWidth, rules.BoardHeight, rules.TeamSize);

            bool[,] positions = new bool[rules.BoardWidth, rules.BoardHeight];

            for (int i = 0; i < state.PlayerStates.Count; i++)
            {
                (int x, int y) = state.PlayerStates[i].Position;
                positions[x, y] = true;
            }
            for (int i = 0; i < rules.BoardWidth; i++)
            {
                positions[i, 0].ShouldBe(true);
                positions[i, 1].ShouldBe(true);
                positions[i, rules.BoardHeight - 1].ShouldBe(true);
                positions[i, rules.BoardHeight - 2].ShouldBe(true);
            }
        }

        [Theory]
        [InlineData(0, 0, 0)]
        public void JoinGame_WithAlreadyConnectedId_ThrowsGameSetupException(int agent1Id, int agent2Id, int teamId)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            state.JoinGame(agent1Id, teamId, false);
            Should.Throw<GameSetupException>(() => state.JoinGame(agent2Id, teamId, false), $"Agent with Id {agent2Id} is already connected");
        }

        [Theory]
        [InlineData(0, 3)]
        [InlineData(2, -1)]
        public void JoinAgent_WithNonExistingTeamId_ThrowsGameSetupException(int agentId, int teamId)
        {
            var rules = Helper.GetDefaultRules();
            var state = Helper.GetGameMasterState(rules);

            Should.Throw<GameSetupException>(() => state.JoinGame(agentId, teamId, false), $"No team with Id {teamId}");
        }

        [Theory]
        [InlineData(0, 1, 0)]
        public void JoinAgent_WithFullTeam_ThrowsGameSetupException(int agent1Id, int agent2Id, int teamId)
        {
            var rules = Helper.GetRulesWithSmallTeamSize();
            var state = Helper.GetGameMasterState(rules);

            state.JoinGame(agent1Id, teamId, false);
            Should.Throw<GameSetupException>(() => state.JoinGame(agent2Id, teamId, false), $"Team {teamId} is full");
        }

        [Theory]
        [InlineData(0, 1, 2, 0, false)]
        [InlineData(0, 1, 2, 0, true)]
        public void JoinAgent_WhenNotExactlyOneAgentWantsToBeLeader_ConnectsAllAgentsAndChoosesOneLeader(int agent1Id, int agent2Id, int agent3Id, int teamId, bool wantToBeLeader)
        {
            var rules = Helper.GetRulesWithMediumTeamSize();
            var state = Helper.GetGameMasterState(rules);

            state.JoinGame(agent1Id, teamId, wantToBeLeader);
            state.JoinGame(agent2Id, teamId, wantToBeLeader);
            state.JoinGame(agent3Id, teamId, wantToBeLeader);

            int leaderCount = 0;
            if (state.PlayerStates[agent1Id].IsLeader)
                leaderCount++;
            if (state.PlayerStates[agent2Id].IsLeader)
                leaderCount++;
            if (state.PlayerStates[agent3Id].IsLeader)
                leaderCount++;

            (state.PlayerStates.Count((g) => g.Value.Team == (Team)teamId), leaderCount).ShouldBe((3, 1));
        }

        #endregion --Initialization--
    }
}
using System;
using System.Collections.Generic;

namespace GameLib
{
    public class AgentState
    {
        public AgentBoard Board { get; private set; }
        public bool IsInGame { get; set; } = false;
        public bool GameStarted { get; set; } = false;
        public bool GameEnded { get; set; } = false;
        public bool IsLeader { get; private set; }
        public int[] TeamIds { get; private set; }
        public int TeamLeaderId { get; private set; }

        public bool IsInTaskArea => Position.X >= Board.GoalAreaHeight &&
                                    Position.X < Board.Height - Board.GoalAreaHeight &&
                                    Position.X >= 0 && Position.X < Board.Height;
        public bool IsInGoalArea
        {
            get
            {
                if (Team == Team.Red)
                    return Position.X < Board.GoalAreaHeight && Position.X >= 0;
                else
                    return Position.X >= Board.Height - Board.GoalAreaHeight && Position.X < Board.Height;
            }
        }

        public List<MoveDirection> PossibleMoves
        {
            get
            {
                List<MoveDirection> possibleMoves = new List<MoveDirection>();
                if ((Team == Team.Red && Position.X > 0) || (Team == Team.Blue && Position.X > Board.GoalAreaHeight))
                    possibleMoves.Add(MoveDirection.Up);
                if ((Team == Team.Red && Position.X < Board.Height - Board.GoalAreaHeight - 1) || (Team == Team.Blue && Position.X < Board.Height - 1))
                    possibleMoves.Add(MoveDirection.Down);

                if (Position.Y > 0)
                    possibleMoves.Add(MoveDirection.Left);
                if (Position.Y < Board.Width - 1)
                    possibleMoves.Add(MoveDirection.Right);

                return possibleMoves;
            }
        }

        public AgentField CurrentField => Board[Position.X, Position.Y];
        public AgentField GetFieldAt((int X, int Y) position) => Board[position.X, position.Y];

        public bool HoldsPiece = false;
        public PieceState PieceState = PieceState.Unknown;
        public (int X, int Y) Position;
        public int WaitUntilTime;

        public Team Team;
        public DateTime Start;

        private bool wantsToBeLeader;

        public AgentState()
        {
        }

        public void Setup(AgentGameRules rules)
        {
            Position = (rules.AgentStartX, rules.AgentStartY);
            Board = new AgentBoard(rules);
        }

        public void Move(MoveDirection direction, int distance, int timestamp)
        {
            var oldPosition = Position;

            switch (direction)
            {
                case MoveDirection.Left:
                    Position.Y--;
                    break;

                case MoveDirection.Right:
                    Position.Y++;
                    break;

                case MoveDirection.Up:
                    Position.X--;
                    break;

                case MoveDirection.Down:
                    Position.X++;
                    break;
            }

            if (Position.X >= Board.Height || Position.X < 0 ||
                Position.Y >= Board.Width || Position.Y < 0)
            {
                Position = oldPosition;
                throw new OutOfBoardMoveException("Agent went out of board");
            }

            Board.SetDistance(Position.X, Position.Y, distance, timestamp);
        }

        public void PickUpPiece(int timestamp)
        {
            if (HoldsPiece)
                throw new PieceOperationException("Picking up piece when agent has one already");

            HoldsPiece = true;
            Board.SetDistance(Position.X, Position.Y, -1, timestamp);
            PieceState = PieceState.Unknown;
        }

        public void SetPieceState(PieceState newState)
        {
            PieceState = newState;
        }

        public void DestroyPiece()
        {
            if (!HoldsPiece)
                throw new PieceOperationException("Destroying piece when agent doesn't have it");

            HoldsPiece = false;
            PieceState = PieceState.Unknown;
        }

        public void PlacePiece(PutPieceResult putResult)
        {
            if (!HoldsPiece)
            {
                throw new PieceOperationException("Agent is not holding a piece");
            }
            switch (putResult)
            {
                case PutPieceResult.PieceGoalRealized:
                    Board.BoardTable[Position.X, Position.Y].IsGoal = AgentFieldState.DiscoveredGoal;
                    break;
                case PutPieceResult.PieceGoalUnrealized:
                    Board.BoardTable[Position.X, Position.Y].IsGoal = AgentFieldState.DiscoveredNotGoal;
                    break;
                case PutPieceResult.PieceInTaskArea:
                    break;
                case PutPieceResult.PieceWasFake:
                    break;
            }
            HoldsPiece = false;
            PieceState = PieceState.Unknown;
        }

        public void Discover(DiscoveryResult discoveryResult, int timestamp)
        {
            Board.ApplyDiscoveryResult(discoveryResult, timestamp);
        }

        public void UpdateBoardWithCommunicationData(AgentBoard partnerBoard)
        {
            bool IsValid(AgentBoard board) => Board.Width == board.Width && Board.Height == board.Height;

            if (!IsValid(partnerBoard))
                throw new InvalidCommunicationResultException();

            Board.ApplyCommunicationResult(partnerBoard);
        }

        public void JoinGame(Team choosenTeam, bool wantsToBeLeader)
        {
            this.Team = choosenTeam;
            this.wantsToBeLeader = wantsToBeLeader;
        }

        public int CurrentTimestamp()
        {
            return (int)(DateTime.UtcNow - Start).TotalMilliseconds;
        }

        public void HandleStartGameMessage(int agentId, AgentGameRules rules, long absoluteStart)
        {
            Setup(rules);

            this.IsLeader = agentId == rules.TeamLeaderId;
            this.TeamIds = (int[])rules.AgentIdsFromTeam.Clone();
            this.TeamLeaderId = rules.TeamLeaderId;
            this.Start = (new DateTime(1970, 1, 1)).AddMilliseconds(absoluteStart);
            this.GameStarted = true;
        }
    }
}
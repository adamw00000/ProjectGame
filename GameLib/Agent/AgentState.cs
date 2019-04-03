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

        public bool IsInTaskArea => Position.Y >= Board.GoalAreaHeight &&
                                    Position.Y < Board.Height - Board.GoalAreaHeight; //changed
        public bool IsInGoalArea
        {
            get
            {
                if (Team == Team.Blue)
                    return Position.Y < Board.GoalAreaHeight && Position.Y >= 0;
                else
                    return Position.Y >= Board.Height - Board.GoalAreaHeight && Position.Y < Board.Height;
            }
        }

        public List<MoveDirection> PossibleMoves
        {
            get
            {
                List<MoveDirection> possibleMoves = new List<MoveDirection>();

                if ((Team == Team.Red && Position.Y < Board.Height - 1) || (Team == Team.Blue && Position.Y < Board.Height - Board.GoalAreaHeight - 1))
                    possibleMoves.Add(MoveDirection.Up);

                if ((Team == Team.Red && Position.Y > Board.GoalAreaHeight) || (Team == Team.Blue && Position.Y > 0))
                    possibleMoves.Add(MoveDirection.Down);

                if (Position.X > 0)
                    possibleMoves.Add(MoveDirection.Left);

                if (Position.X < Board.Width - 1)
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

        public void Move(MoveDirection direction, int distance)
        {
            var oldPosition = Position;

            switch (direction)
            {
                case MoveDirection.Left:
                    Position.X--;
                    break;

                case MoveDirection.Right:
                    Position.X++;
                    break;

                case MoveDirection.Up:
                    Position.Y++;
                    break;

                case MoveDirection.Down:
                    Position.Y--;
                    break;
            }

            if (Position.Y >= Board.Height || Position.Y < 0 ||
                Position.X >= Board.Width || Position.X < 0)
            {
                Position = oldPosition;
                throw new InvalidMoveException();
            }

            if ((Team == Team.Blue && Position.Y > Board.Height - Board.GoalAreaHeight - 1) ||
                (Team == Team.Red && Position.Y < Board.GoalAreaHeight))
            {
                Position = oldPosition;
                throw new InvalidMoveException();
            }

            Board.SetDistance(Position.X, Position.Y, distance);
        }

        public void PickUpPiece()
        {
            if (HoldsPiece)
                throw new PieceOperationException("Picking up piece when agent has one already");

            HoldsPiece = true;
            Board.SetDistance(Position.X, Position.Y, -1);
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

        public void HandleStartGameMessage(int agentId, AgentGameRules rules, int timestamp, long absoluteStart)
        {
            Setup(rules);

            this.IsLeader = agentId == rules.TeamLeaderId;
            this.TeamIds = (int[])rules.AgentIdsFromTeam.Clone();
            this.TeamLeaderId = rules.TeamLeaderId;
            this.Start = (new DateTime()).AddMilliseconds(absoluteStart);
            this.GameStarted = true;
        }
    }
}
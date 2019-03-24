using System;

namespace GameLib
{
    public class AgentState
    {
        public AgentBoard Board { get; private set; }
        public bool IsInGame { get; set; } = false;
        public bool GameStarted { get; set; } = false;
        public bool GameEnded { get; set; } = false;
        public bool IsLeader { get; private set; }

        public bool HoldsPiece = false;
        public PieceState PieceState = PieceState.Unknown;
        public (int X, int Y) Position;
        public int WaitUntilTime;

        public Team Team;
        public DateTime Start;

        private bool wantsToBeLeader;
        private int[] teamIds; // We did add teamIds here becouse DecisionModule will use them for communication and it does get AgentState
        private int teamLeaderId;


        public AgentState()
        {
        }

        public void Setup(GameRules rules)
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

            if (Position.X >= Board.Width || Position.X < 0 ||
                Position.Y >= Board.Height || Position.Y < 0)
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
            if(!HoldsPiece)
            {
                throw new PieceOperationException("Agent is not holding a piece");
            }
            switch(putResult)
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

        public void HandleStartGameMessage(int agentId, GameRules rules, int timestamp, long absoluteStart)
        {
            Setup(rules);

            this.IsLeader = agentId == rules.TeamLeaderId;
            this.teamIds = (int[])rules.AgentIdsFromTeam.Clone();
            this.teamLeaderId = rules.TeamLeaderId;
            this.Start = (new DateTime()).AddMilliseconds(absoluteStart);
            this.GameStarted = true;
        }
    }
}
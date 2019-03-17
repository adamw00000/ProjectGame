namespace GameLib
{
    public class AgentState
    {
        public AgentBoard Board { get; private set; }

        public bool HoldsPiece = false;
        public PieceState PieceState = PieceState.Unknown;
        public (int X, int Y) Position;
        public int waitUntilTime;

        public AgentState()
        {
        }

        // Ta metoda została wyodrębniona z konstruktora bo nie chcemy tworzyć całego obiektu w momencie rozpoczęcia gry
        // tylko uzupełnić planszę (która zależy od infromacji otrzymanych na początku gry.
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
                    Board.BoardTable[Position.X, Position.Y].IsGoal = FieldState.DiscoveredGoal;
                    break;
                case PutPieceResult.PieceGoalUnrealized:
                    Board.BoardTable[Position.X, Position.Y].IsGoal = FieldState.DiscoveredNotGoal;
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

        public void ApplyCommunicationResult(CommunicationResult communicationResult)
        {
            if (!communicationResult.IsValid(Board))
                throw new InvalidCommunicationResultException();

            Board.ApplyCommunicationResult(communicationResult.Board);
        }
    }
}
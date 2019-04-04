using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameLib
{
    public class BasicDecisionModule : DecisionModuleBase
    {
        private (int x, int y) previousPosition = (-1, -1);
        private int previousDistance = int.MaxValue;
        private MoveDirection lastMove;
        private bool movingBackWasNeeded = false;

        Random random = RandomGenerator.GetGenerator();

        public override Task<Action> ChooseAction(int agentId, AgentState agentState)
        {
            Action action = null;

            if (isLeaderRequestPending) //leader request
            {
                isLeaderRequestPending = false;
                var data = DataProcessor.CreateCommunicationDataForCommunicationWith(agentState.TeamLeaderId, agentState);
                action = new ActionCommunicationAgreement(agentState.TeamLeaderId, true, data); //always respond
                previousAction = action;
                return Task.FromResult(action); //return
            }

            if (communicationQueue.Count != 0) //another agent request
            {
                var teammate = communicationQueue[0];
                communicationQueue.RemoveAt(0);
                bool agreement = random.Next(2) == 0 ? true : false; //50% chance
                //bool agreement = true; // do zmiany

                if (agreement)
                {
                    var data = DataProcessor.CreateCommunicationDataForCommunicationWith(teammate, agentState);
                    action = new ActionCommunicationAgreement(teammate, true, data);
                }
                else
                {
                    action = new ActionCommunicationAgreement(teammate, false, null);
                }
                previousAction = action;
                return Task.FromResult(action); //return
            }

            if (random.Next(20) == 0) //random communication (one in 20)
            {
                var randomTeammate = agentState.TeamIds[random.Next(agentState.TeamIds.Length)];
                var data = DataProcessor.CreateCommunicationDataForCommunicationWith(randomTeammate, agentState);
                action = new ActionCommunicate(randomTeammate, data);
                previousAction = action;
                return Task.FromResult(action); //return
            }

            (int X, int Y) = agentState.Position;

            if (agentState.HoldsPiece)
            {
                if (agentState.IsInGoalArea) //holds piece in goal area
                {
                    if (agentState.CurrentField.IsGoal == AgentFieldState.Unknown) //if can discover
                    {
                        action = new ActionPutPiece();
                    }
                    else //if any of the neighbouring fields can be discovered, go there, else - random move
                    {
                        var selectedDirection = GenerateGoalAreaMoveWithPiece(agentState);
                        lastMove = selectedDirection;
                        action = new ActionMove(selectedDirection);
                    }
                }
                else //holds piece in task area
                {
                    if (previousAction is ActionPickPiece)
                    {
                        action = new ActionCheckPiece();
                    }
                    else if (agentState.PieceState == PieceState.Invalid)
                    {
                        action = new ActionDestroyPiece();
                    }
                    else //move to goal area
                    {
                        var direction = MoveToGoalArea(agentState, agentId, X, Y);
                        action = new ActionMove(direction);
                    }
                }
            }
            else //doesn't have piece
            {
                if (agentState.CurrentField.Distance == 0) //standing on piece
                {
                    action = new ActionPickPiece();
                    previousAction = action;
                    return Task.FromResult(action); //return
                }
                if (random.Next(20) == 0) //random discovery (one in 20)
                {
                    action = new ActionDiscovery();
                }
                else
                {
                    //move to task area
                    var direction = MoveToTaskArea(agentState, agentId, X, Y);
                    action = new ActionMove(direction);
                }
            }

            previousPosition = (X, Y);
            previousDistance = agentState.Board[X, Y].Distance;
            previousAction = action;
            return Task.FromResult(action);
        }

        private MoveDirection GenerateGoalAreaMoveWithPiece(AgentState agentState)
        {
            List<MoveDirection> goalAreaMoves = new List<MoveDirection>();

            foreach (var direction in agentState.PossibleMoves)
            {
                var newPosition = SimulateMove(agentState, direction);
                if (IsInGoalArea(agentState, newPosition))
                {
                    goalAreaMoves.Add(direction);
                    if (agentState.GetFieldAt(newPosition).IsGoal == AgentFieldState.Unknown)
                    {
                        return direction;
                    }
                }
            }

            return goalAreaMoves[random.Next(goalAreaMoves.Count)];
        }

        private MoveDirection MoveToGoalArea(AgentState agentState, int agentId, int X, int Y)
        {
            if (previousPosition == (X, Y) && previousAction is ActionMove) //last move unsuccessful
            {
                if (agentState.Team == Team.Blue)
                {
                    return ChooseThisOrRandomValidDirection(agentState, MoveDirection.Right);
                }
                else
                {
                    return ChooseThisOrRandomValidDirection(agentState, MoveDirection.Left);
                }
            }
            else
            {
                if (agentState.Team == Team.Blue)
                {
                    return ChooseThisOrRandomValidDirection(agentState, MoveDirection.Down);
                }
                else
                {
                    return ChooseThisOrRandomValidDirection(agentState, MoveDirection.Up);
                }
            }
        }

        private MoveDirection ChooseThisOrRandomValidDirection(AgentState agentState, MoveDirection moveDirection)
        {
            if (agentState.PossibleMoves.Contains(moveDirection))
            {
                return moveDirection;
            }
            else
            {
                var possibleMoves = agentState.PossibleMoves;
                return possibleMoves[random.Next(possibleMoves.Count)];
            }
        }

        private MoveDirection MoveToTaskArea(AgentState agentState, int agentId, int X, int Y)
        {
            if (previousPosition == (X, Y) && previousAction is ActionMove) //last move unsuccessful
            {
                if (agentState.Team == Team.Blue)
                {
                    return ChooseThisOrRandomValidDirection(agentState, MoveDirection.Left);
                }
                else
                {
                    return ChooseThisOrRandomValidDirection(agentState, MoveDirection.Right);
                }
            }
            else
            {
                if (previousDistance == int.MaxValue)
                {
                    if (agentState.Team == Team.Red)
                    {
                        return MoveDirection.Down;
                    }
                    else
                    {
                        return MoveDirection.Up;
                    }
                }
                else
                {
                    return SeekPiece(agentState, agentState.Board[X, Y].Distance, X, Y, agentState.Board.Width, agentState.Board.Height);
                }
            }
        }

        private MoveDirection SeekPiece(AgentState agentState, int distance, int x, int y, int boardWidth, int boardHeight)
        {
            if (previousDistance > distance)
            {
                if (movingBackWasNeeded)
                {
                    movingBackWasNeeded = false;
                    return MoveToTheSide(agentState);
                }
                else return lastMove;
            }
            else if (previousDistance < distance)
            {
                movingBackWasNeeded = true;
                switch (lastMove)
                {
                    case MoveDirection.Up:
                        return MoveDirection.Down;
                    case MoveDirection.Right:
                        return MoveDirection.Left;
                    case MoveDirection.Down:
                        return MoveDirection.Up;
                    case MoveDirection.Left:
                        return MoveDirection.Right;
                    default:
                        return MoveDirection.Right; //won't happen
                }
            }
            else
            {
                if (agentState.PossibleMoves.Contains(lastMove))
                    return lastMove;
                else
                    return MoveToTheSide(agentState);
            }
        }

        private MoveDirection MoveToTheSide(AgentState agentState)
        {
            if (lastMove == MoveDirection.Up || lastMove == MoveDirection.Down)
            {
                List<MoveDirection> sideMoves = new List<MoveDirection>();
                if (agentState.PossibleMoves.Contains(MoveDirection.Left))
                    sideMoves.Add(MoveDirection.Left);
                if (agentState.PossibleMoves.Contains(MoveDirection.Right))
                    sideMoves.Add(MoveDirection.Right);
                return sideMoves[random.Next(sideMoves.Count)];
            }
            else
            {
                List<MoveDirection> sideMoves = new List<MoveDirection>();
                if (agentState.PossibleMoves.Contains(MoveDirection.Up))
                    sideMoves.Add(MoveDirection.Up);
                if (agentState.PossibleMoves.Contains(MoveDirection.Down))
                    sideMoves.Add(MoveDirection.Down);
                return sideMoves[random.Next(sideMoves.Count)];
            }
        }
    }
}

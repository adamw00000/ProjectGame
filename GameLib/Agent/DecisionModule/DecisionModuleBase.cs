using GameLib.Actions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLib
{
    public abstract class DecisionModuleBase
    {
        protected List<int> communicationQueue = new List<int>();
        protected bool isLeaderRequestPending;
        protected IAction previousAction;
        
        public CommunicationDataProcessor DataProcessor { get; } = new CommunicationDataProcessor();

        public abstract Task<IAction> ChooseAction(int agentId, AgentState agentState);

        public void SaveCommunicationResult(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState)
        {
            DataProcessor.ExtractCommunicationData(senderId, agreement, timestamp, data, agentState);
        }

        protected int CurrentTimestamp(AgentState agentState)
        {
            return (int)(DateTime.UtcNow - agentState.Start).TotalMilliseconds;
        }

        internal void AddSenderToCommunicationQueue(AgentState agentState, int senderId)
        {
            if (senderId == agentState.TeamLeaderId)
            {
                isLeaderRequestPending = true;
                return;
            }

            if (!communicationQueue.Contains(senderId))
            {
                communicationQueue.Remove(senderId);
                communicationQueue.Add(senderId);
            }
        }

        // Not used, not CHANGED TO NEW COORDINATES
        //public bool IsInTaskArea(AgentState agentState, (int X, int Y) position) =>
        //                            agentState.Position.X >= agentState.Board.GoalAreaHeight &&
        //                            agentState.Position.X < agentState.Board.Height - agentState.Board.GoalAreaHeight &&
        //                            agentState.Position.X >= 0 && agentState.Position.X < agentState.Board.Height;

        public bool IsInGoalArea(AgentState agentState, (int X, int Y) position)
        {
            if (agentState.Team == Team.Blue)
                return position.Y < agentState.Board.GoalAreaHeight && position.Y >= 0;
            else
                return position.Y >= agentState.Board.Height - agentState.Board.GoalAreaHeight && position.Y < agentState.Board.Height;
        }

        public (int X, int Y) SimulateMove(AgentState state, MoveDirection direction)
        {
            var newPosition = state.Position;

            switch (direction)
            {
                case MoveDirection.Left:
                    newPosition.X--;
                    break;

                case MoveDirection.Right:
                    newPosition.X++;
                    break;

                case MoveDirection.Up:
                    newPosition.Y++;
                    break;

                case MoveDirection.Down:
                    newPosition.Y--;
                    break;
            }

            return newPosition;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameLib.Actions;

namespace GameLib
{
    public class InteractiveDecisionModule : IDecisionModule
    {
        private bool registered = false;
        public CommunicationDataProcessor DataProcessor { get; } = new CommunicationDataProcessor();

        public async Task<IAction> ChooseAction(int agentId, AgentState agentState)
        {
            if (!registered)
            {
                InteractiveInputProvider.Register(agentId);
                registered = true;
            }

            ConsoleKey key = await InteractiveInputProvider.GetKey(agentId);

            IAction action = ParseInput(key, agentId, agentState);
            Console.WriteLine(action);

            return action;
        }

        private IAction ParseInput(ConsoleKey key, int agentId, AgentState agentState)
        {
            IAction action;
            var random = RandomGenerator.GetGenerator();

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    action = new ActionMove(agentId, MoveDirection.Up);
                    break;
                case ConsoleKey.DownArrow:
                    action = new ActionMove(agentId, MoveDirection.Down);
                    break;
                case ConsoleKey.LeftArrow:
                    action = new ActionMove(agentId, MoveDirection.Left);
                    break;
                case ConsoleKey.RightArrow:
                    action = new ActionMove(agentId, MoveDirection.Right);
                    break;
                case ConsoleKey.Q:
                    action = new ActionPickPiece(agentId);
                    break;
                case ConsoleKey.W:
                    action = new ActionCheckPiece(agentId);
                    break;
                case ConsoleKey.E:
                    action = new ActionPutPiece(agentId);
                    break;
                case ConsoleKey.R:
                    action = new ActionDestroyPiece(agentId);
                    break;
                case ConsoleKey.A:
                    action = new ActionCommunicationRequestWithData(agentId, agentId, 
                        DataProcessor.CreateCommunicationDataForCommunicationWith(agentId, agentState));
                    break;
                case ConsoleKey.S:
                    action = new ActionCommunicationAgreementWithData(agentId, agentId, 
                        random.Next(2) == 1 ? true : false, DataProcessor.CreateCommunicationDataForCommunicationWith(agentId, agentState));
                    break;
                case ConsoleKey.D:
                    action = new ActionDiscovery(agentId);
                    break;
                default:
                    throw new ArgumentException("Invalid input");
            }

            Console.WriteLine(action.ToString());

            return action;
        }

        public void SaveCommunicationResult(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState)
        {
            DataProcessor.ExtractCommunicationData(senderId, agreement, timestamp, data, agentState);
        }
    }
}

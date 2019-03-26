using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameLib.Actions;

namespace GameLib
{
    public class InteractiveDecisionModule : DecisionModuleBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private bool registered = false;

        public async override Task<IAction> ChooseAction(int agentId, AgentState agentState)
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

            Random random = RandomGenerator.GetGenerator(); 

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    action = new ActionMove(agentId, MoveDirection.Up);
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Up}");
                    break;
                case ConsoleKey.DownArrow:
                    action = new ActionMove(agentId, MoveDirection.Down);
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Down}");
                    break;
                case ConsoleKey.LeftArrow:
                    action = new ActionMove(agentId, MoveDirection.Left);
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Left}");
                    break;
                case ConsoleKey.RightArrow:
                    action = new ActionMove(agentId, MoveDirection.Right);
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Right}");
                    break;
                case ConsoleKey.Q:
                    action = new ActionPickPiece(agentId);
                    logger.Debug($"Agent {agentId} chose action ActionPickPiece");
                    break;
                case ConsoleKey.W:
                    action = new ActionCheckPiece(agentId);
                    logger.Debug($"Agent {agentId} chose action ActionCheckPiece");
                    break;
                case ConsoleKey.E:
                    action = new ActionPutPiece(agentId);
                    logger.Debug($"Agent {agentId} chose action ActionPutPiece");
                    break;
                case ConsoleKey.R:
                    action = new ActionDestroyPiece(agentId);
                    logger.Debug($"Agent {agentId} chose action ActionDestroyPiece");
                    break;
                case ConsoleKey.A:
                    var requestData = DataProcessor.CreateCommunicationDataForCommunicationWith(agentId, agentState);
                    action = new ActionCommunicationRequestWithData(agentId, agentId, requestData);
                    logger.Debug($"Agent {agentId} chose action ActionCommunicationRequestWithData with data {requestData}");
                    break;
                case ConsoleKey.S:
                    bool agreement = (random.Next(2) == 1) ? true : false;
                    var responseData = DataProcessor.CreateCommunicationDataForCommunicationWith(agentId, agentState);
                    action = new ActionCommunicationAgreementWithData(agentId, agentId, agreement, responseData);
                    logger.Debug($"Agent {agentId} chose action ActionCommunicationAgreementWithData with data {responseData} - he {(agreement ? "agrees" : "doesn't agree")} for the communication");
                    break;
                case ConsoleKey.D:
                    action = new ActionDiscovery(agentId);
                    logger.Debug($"Agent {agentId} chose action ActionDiscovery");
                    break;
                default:
                    logger.Error($"Agent {agentId} received invalid input");
                    throw new ArgumentException("Invalid input");
            }

            return action;
        }
    }
}

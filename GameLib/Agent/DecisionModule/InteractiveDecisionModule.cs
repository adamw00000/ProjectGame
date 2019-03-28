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

        private int CurrentTimestamp(AgentState agentState)
        {
            return (int)(DateTime.UtcNow - agentState.Start).TotalMilliseconds;
        }

        private IAction ParseInput(ConsoleKey key, int agentId, AgentState agentState)
        {
            IAction action;

            Random random = RandomGenerator.GetGenerator(); 

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    action = new ActionMove(agentId, MoveDirection.Up, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Up}");
                    break;
                case ConsoleKey.DownArrow:
                    action = new ActionMove(agentId, MoveDirection.Down, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Down}");
                    break;
                case ConsoleKey.LeftArrow:
                    action = new ActionMove(agentId, MoveDirection.Left, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Left}");
                    break;
                case ConsoleKey.RightArrow:
                    action = new ActionMove(agentId, MoveDirection.Right, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionMove with direction {MoveDirection.Right}");
                    break;
                case ConsoleKey.Q:
                    action = new ActionPickPiece(agentId, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionPickPiece");
                    break;
                case ConsoleKey.W:
                    action = new ActionCheckPiece(agentId, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionCheckPiece");
                    break;
                case ConsoleKey.E:
                    action = new ActionPutPiece(agentId, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionPutPiece");
                    break;
                case ConsoleKey.R:
                    action = new ActionDestroyPiece(agentId, CurrentTimestamp(agentState));
                    logger.Debug($"Agent {agentId} chose action ActionDestroyPiece");
                    break;
                case ConsoleKey.A:
                    var teammate = agentState.TeamIds[random.Next(agentState.TeamIds.Length)];
                    var requestData = DataProcessor.CreateCommunicationDataForCommunicationWith(teammate, agentState);
                    action = new ActionCommunicationRequestWithData(agentId, CurrentTimestamp(agentState), teammate, requestData);
                    logger.Debug($"Agent {agentId} chose action ActionCommunicationRequestWithData with agent {teammate} with data {requestData}");
                    break;
                case ConsoleKey.S:
                    var randomTeammate = agentState.TeamIds[random.Next(agentState.TeamIds.Length)];
                    bool agreement = (random.Next(2) == 1) ? true : false;
                    var responseData = DataProcessor.CreateCommunicationDataForCommunicationWith(randomTeammate, agentState);
                    action = new ActionCommunicationAgreementWithData(agentId, CurrentTimestamp(agentState), randomTeammate, agreement, responseData);
                    logger.Debug($"Agent {agentId} chose action ActionCommunicationAgreementWithData with agent {randomTeammate} with data {responseData} - he {(agreement ? "agrees" : "doesn't agree")} for the communication");
                    break;
                case ConsoleKey.D:
                    action = new ActionDiscovery(agentId, CurrentTimestamp(agentState));
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

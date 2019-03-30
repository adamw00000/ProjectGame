using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameLib.Actions;

namespace GameLib
{
    public class RandomDecisionModule : DecisionModuleBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Random random;

        private readonly int[] prefixSumArray = new int[actionCount];
        private const int actionCount = 8;

        public RandomDecisionModule(int[] weightArray) : this(weightArray, RandomGenerator.GetGenerator().Next())
        { }

        public RandomDecisionModule(int[] weightArray, int randSeed)
        {
            if (weightArray.Length != actionCount)
                throw new ArgumentException($"Weight array's length must be {actionCount}!");

            CalculatePrefixWeightArray(weightArray);
            
            random = new Random(randSeed);
        }

        private void CalculatePrefixWeightArray(int[] weightArray)
        {
            for (int i = 0; i < actionCount; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    prefixSumArray[i] += weightArray[j];
                }
            }
        }

        public override Task<Action> ChooseAction(int agentId, AgentState agentState)
        {
            Action action;

            int value = random.Next(1, prefixSumArray[actionCount - 1] + 1);
            if (value <= prefixSumArray[0])
            {
                action = new ActionCheckPiece();
                logger.Debug($"Agent {agentId} chose action ActionCheckPieceMessage");
            }
            else if (value <= prefixSumArray[1])
            {
                action = new ActionDestroyPiece();
                logger.Debug($"Agent {agentId} chose action ActionDestroyPiece");
            }
            else if (value <= prefixSumArray[2])
            {
                action = new ActionPickPiece();
                logger.Debug($"Agent {agentId} chose action ActionPickPiece");
            }
            else if (value <= prefixSumArray[3])
            {
                var direction = (MoveDirection)random.Next(4);
                action = new ActionMove(direction);
                logger.Debug($"Agent {agentId} chose action ActionMove with direction {direction}");
            }
            else if (value <= prefixSumArray[4])
            {
                action = new ActionDiscovery();
                logger.Debug($"Agent {agentId} chose action ActionDiscovery");
            }
            else if (value <= prefixSumArray[5])
            {
                action = new ActionPutPiece();
                logger.Debug($"Agent {agentId} chose action ActionPutPiece");
            }
            else if (value <= prefixSumArray[6])
            {
                var teammate = agentState.TeamIds[random.Next(agentState.TeamIds.Length)];
                var requestData = DataProcessor.CreateCommunicationDataForCommunicationWith(teammate, agentState);
                action = new ActionCommunicate(teammate,requestData);
                logger.Debug($"Agent {agentId} chose action ActionCommunicationRequestWithData with agent {teammate} with data {requestData}");
            }
            else
            {
                var randomTeammate = agentState.TeamIds[random.Next(agentState.TeamIds.Length)];
                var responseData = DataProcessor.CreateCommunicationDataForCommunicationWith(randomTeammate, agentState);
                bool agreement = (random.Next(2) == 1) ? true : false;
                action = new ActionCommunicationAgreement(randomTeammate, agreement, responseData);
                logger.Debug($"Agent {agentId} chose action ActionCommunicationAgreementWithData with agent {randomTeammate} with data {responseData} - he {(agreement ? "agrees" : "doesn't agree")} for the communication");
            }
            System.Threading.Thread.Sleep(1000); //necessary for GUI
            Console.WriteLine(action.ToString());

            return Task.FromResult(action);
        }
    }
}

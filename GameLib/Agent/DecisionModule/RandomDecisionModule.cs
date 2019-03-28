using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameLib.Actions;

namespace GameLib
{
    public class RandomDecisionModule : IDecisionModule
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Random random;

        private readonly int[] prefixSumArray = new int[actionCount];
        private const int actionCount = 8;

        public CommunicationDataProcessor DataProcessor { get; } = new CommunicationDataProcessor();

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

        private int CurrentTimestamp(AgentState agentState)
        {
            return (int)(DateTime.UtcNow - agentState.Start).TotalMilliseconds;
        }

        public Task<IAction> ChooseAction(int agentId, AgentState agentState)
        {
            IAction action;

            int value = random.Next(1, prefixSumArray[actionCount - 1] + 1);
            if (value <= prefixSumArray[0])
            {
                action = new ActionCheckPiece(agentId, CurrentTimestamp(agentState));
                logger.Debug($"Agent {agentId} chose action ActionCheckPiece");
            }
            else if (value <= prefixSumArray[1])
            {
                action = new ActionDestroyPiece(agentId, CurrentTimestamp(agentState));
                logger.Debug($"Agent {agentId} chose action ActionDestroyPiece");
            }
            else if (value <= prefixSumArray[2])
            {
                action = new ActionPickPiece(agentId, CurrentTimestamp(agentState));
                logger.Debug($"Agent {agentId} chose action ActionPickPiece");
            }
            else if (value <= prefixSumArray[3])
            {
                var direction = (MoveDirection)random.Next(4);
                action = new ActionMove(agentId, direction, CurrentTimestamp(agentState)); 
                logger.Debug($"Agent {agentId} chose action ActionMove with direction {direction}");
            }
            else if (value <= prefixSumArray[4])
            {
                action = new ActionDiscovery(agentId, CurrentTimestamp(agentState));
                logger.Debug($"Agent {agentId} chose action ActionDiscovery");
            }
            else if (value <= prefixSumArray[5])
            {
                action = new ActionPutPiece(agentId, CurrentTimestamp(agentState));
                logger.Debug($"Agent {agentId} chose action ActionPutPiece");
            }
            else if (value <= prefixSumArray[6])
            {
                var requestData = DataProcessor.CreateCommunicationDataForCommunicationWith(agentId, agentState);
                action = new ActionCommunicationRequestWithData(agentId, CurrentTimestamp(agentState), agentId, requestData);
                logger.Debug($"Agent {agentId} chose action ActionCommunicationRequestWithData with data {requestData}");
            }
            else
            {
                var responseData = DataProcessor.CreateCommunicationDataForCommunicationWith(agentId, agentState);
                bool agreement = (random.Next(2) == 1) ? true : false;
                action = new ActionCommunicationAgreementWithData(agentId, CurrentTimestamp(agentState), agentId, agreement, responseData);
                logger.Debug($"Agent {agentId} chose action ActionCommunicationAgreementWithData with data {responseData} - he {(agreement ? "agrees" : "doesn't agree")} for the communication");
            }
            System.Threading.Thread.Sleep(1000); //necessary for GUI
            Console.WriteLine(action.ToString());

            return Task.FromResult(action);
        }

        public void SaveCommunicationResult(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState)
        {
            DataProcessor.ExtractCommunicationData(senderId, agreement, timestamp, data, agentState);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameLib.Actions;

namespace GameLib
{
    public class RandomDecisionModule : IDecisionModule
    {
        private readonly int[] prefixSumArray = new int[actionCount];
        private const int actionCount = 8;

        public RandomDecisionModule(int[] weightArray)
        {
            if (weightArray.Length != actionCount)
                throw new ArgumentException($"Weight array's length must be {actionCount}!");

            CalculatePrefixWeightArray(weightArray);
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

        public Task<IAction> ChooseAction(int agentId, AgentState agentState)
        {
            IAction action;
            Random random = RandomGenerator.GetGenerator();

            int value = random.Next(1, prefixSumArray[actionCount - 1] + 1);
            if (value <= prefixSumArray[0])
            {
                action = new ActionCheckPiece(agentId);
            }
            else if (value <= prefixSumArray[1])
            {
                action = new ActionDestroyPiece(agentId);
            }
            else if (value <= prefixSumArray[2])
            {
                action = new ActionPickPiece(agentId);
            }
            else if (value <= prefixSumArray[3])
            {
                action = new ActionMove(agentId, (MoveDirection)random.Next(4));
            }
            else if (value <= prefixSumArray[4])
            {
                action = new ActionDiscovery(agentId);
            }
            else if (value <= prefixSumArray[5])
            {
                action = new ActionPutPiece(agentId);
            }
            else if (value <= prefixSumArray[6])
            {
                action = new ActionCommunicationRequestWithData(agentId, agentId, agentState);
            }
            else
            {
                action = new ActionCommunicationAgreementWithData(agentId, agentId, random.Next(2) == 1 ? true : false, agentState);
            }

            Console.WriteLine(action.ToString());

            return Task.FromResult(action);
        }
    }
}

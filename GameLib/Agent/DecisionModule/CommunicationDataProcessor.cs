using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class CommunicationDataProcessor
    {
        public Dictionary<int, (DateTime timestamp, CommunicationData data)> CurrentAgentCommunicationData { get; } =
            new Dictionary<int, (DateTime timestamp, CommunicationData data)>();
        public Dictionary<int, (DateTime timestamp, CommunicationState state)> LastCommunicationState = 
            new Dictionary<int, (DateTime timestamp, CommunicationState state)>();

        CommunicationData Unpack(object data)
        {
            return (CommunicationData)data;
        }

        public void ExtractCommunicationData(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState)
        {
            if (!agreement)
            {
                LastCommunicationState[senderId] = (timestamp, CommunicationState.Rejected);
                return;
            }

            CommunicationData deserializedData;
            try
            {
                if (data == null)
                    throw new NullReferenceException();
                deserializedData = Unpack(data);
            }
            catch
            {
                throw new InvalidCommunicationDataException("Received data is not a valid CommunicationData object");
            }

            LastCommunicationState[senderId] = (timestamp, CommunicationState.Accepted);
            (DateTime oldTimestamp, CommunicationData oldData) = CurrentAgentCommunicationData[senderId];
            if (oldTimestamp > timestamp)
                return;
            agentState.UpdateBoardWithCommunicationData(deserializedData.AgentState.Board);
            CurrentAgentCommunicationData[senderId] = (timestamp, deserializedData);
        }

        public object CreateCommunicationDataForCommunicationWith(int targetId, AgentState state)
        {
            LastCommunicationState[targetId] = (DateTime.UtcNow, CommunicationState.Pending);
            return new CommunicationData(state);
        }
    }

    public class CommunicationData
    {
        public AgentState AgentState { get; }

        public CommunicationData(AgentState state)
        {
            AgentState = state;
        }
    }

    public enum CommunicationState { Accepted, Rejected, Pending }
}
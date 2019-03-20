using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class CommunicationDataProcessor
    {
        public Dictionary<int, (DateTime timestamp, CommunicationData data)> CurrentAgentCommunicationData { get; } =
            new Dictionary<int, (DateTime timestamp, CommunicationData data)>();

        object Serialize(CommunicationData data)
        {
            return data;
        }

        CommunicationData Deserialize(object data)
        {
            return (CommunicationData)data;
        }

        public void ExtractCommunicationData(int senderId, bool agreement, DateTime timestamp, object data, AgentState agentState)
        {
            (DateTime oldTimestamp, CommunicationData oldData) = CurrentAgentCommunicationData[senderId];
            if (oldTimestamp > timestamp)
                return;

            CommunicationData deserializedData = Deserialize(data);
            agentState.UpdateBoardWithCommunicationData(deserializedData.AgentState.Board);
            CurrentAgentCommunicationData[senderId] = (timestamp, deserializedData);
        }

        public object CreateCommunicationData(AgentState state)
        {
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
}

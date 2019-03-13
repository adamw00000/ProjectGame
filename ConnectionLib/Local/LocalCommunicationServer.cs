using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConnectionLib
{
    public static class LocalCommunicationServer
    {
        #region Private fields
        private static int lastAgentId = 0;

        private static GMLocalConnection gameMaster;
        private static ConcurrentDictionary<AgentLocalConnection, int> agentToId = new ConcurrentDictionary<AgentLocalConnection, int>();
        private static ConcurrentDictionary<int, AgentLocalConnection> idToAgent = new ConcurrentDictionary<int, AgentLocalConnection>();
        #endregion

        #region Public methods
        public static void Clear()
        {
            lastAgentId = 0;
            gameMaster = null;
            agentToId = new ConcurrentDictionary<AgentLocalConnection, int>();
            idToAgent = new ConcurrentDictionary<int, AgentLocalConnection>();
    }

        public static void ConnectAgent(AgentLocalConnection agent)
        {
            if (gameMaster == null)
            {
                // Tu powinno być wysłanie odpowiedniej wiadomości w odpowiedzi a nie Exception
                throw new Exception("GM is not connected");
            }
            if (agentToId.ContainsKey(agent))
            {
                throw new Exception("This agent is already connected");
            }

            int id = System.Threading.Interlocked.Increment(ref lastAgentId);
            agentToId.GetOrAdd(agent, id);
            idToAgent.GetOrAdd(id, agent);
        }

        public static void ConnectGM(GMLocalConnection gameMaster)
        {
            if (LocalCommunicationServer.gameMaster != null)
            {
                throw new Exception("GM is already connected");
            }

            LocalCommunicationServer.gameMaster = gameMaster;
        }

        public static void DisconnectAgent(AgentLocalConnection agent)
        {
            if (!agentToId.ContainsKey(agent))
            {
                throw new Exception("Agent wasn't connected to GM");
            }

            agentToId.Remove(agent, out int id);
            idToAgent.Remove(id, out AgentLocalConnection a);
        }

        public static void DisconnectGM(GMLocalConnection gameMaster)
        {
            if (LocalCommunicationServer.gameMaster != gameMaster)
            {
                throw new Exception("Different GM is connected");
            }

            LocalCommunicationServer.gameMaster = null;
        }

        public static void SendMessage<M>(AgentLocalConnection agent, M message)
        {
            // tu CS musi dodawać Id żeby potem GM wiedział komu odsyłać
            gameMaster.Messages.Add(message);
        }

        public static void SendMessage<M>(GMLocalConnection gameMaster, M message)
        {
            // tutaj musimy jakoś wydobyć z message id agenta i tam przesłać
            // i to poniżej jest pięknym rozwiązniem (nie)
            //int id = ((dynamic)message).AgentId;
            //int id = (int)message.GetType().GetProperty("AgentId").GetValue(message, null);

            int id = 1;

            // jeśli id jest złe to co??? bo zgodnie ze specyfikacją jest wiadomość Invalid JSON... ale nie mamy jeszcze implementacji
            if (idToAgent.TryGetValue(id, out AgentLocalConnection agent))
            {
                agent.Messages.Add(message);
            }
        }
        #endregion
    }
}

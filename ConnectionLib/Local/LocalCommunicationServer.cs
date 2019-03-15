using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConnectionLib
{
    public class LocalCommunicationServer
    {
        #region Fields and properties
        private int lastAgentId = 0;

        private GMLocalConnection gameMaster;
        private ConcurrentDictionary<AgentLocalConnection, int> agentToId = new ConcurrentDictionary<AgentLocalConnection, int>();
        private ConcurrentDictionary<int, AgentLocalConnection> idToAgent = new ConcurrentDictionary<int, AgentLocalConnection>();
        #endregion

        #region Constructors
        public LocalCommunicationServer()
        {

        }
        #endregion

        #region Methods
        public void ConnectAgent(AgentLocalConnection agent)
        {
            //if (GameMaster == null)
            //{
            //    // Tu powinno być wysłanie odpowiedniej wiadomości w odpowiedzi a nie Exception
            //    throw new Exception("GM is not connected");
            //}
            if (agentToId.ContainsKey(agent))
            {
                throw new Exception("This agent is already connected");
            }

            int id = System.Threading.Interlocked.Increment(ref lastAgentId);
            agentToId.GetOrAdd(agent, id);
            idToAgent.GetOrAdd(id, agent);
        }

        public void ConnectGM(GMLocalConnection gameMaster)
        {
            if (this.gameMaster != null)
            {
                throw new Exception("GM is already connected");
            }

            this.gameMaster = gameMaster;
        }

        public void DisconnectAgent(AgentLocalConnection agent)
        {
            if (!agentToId.ContainsKey(agent))
            {
                throw new Exception("Agent wasn't connected to GM");
            }

            agentToId.Remove(agent, out int id);
            idToAgent.Remove(id, out AgentLocalConnection a);
        }

        public void DisconnectGM(GMLocalConnection gameMaster)
        {
            if (this.gameMaster != gameMaster)
            {
                throw new Exception("Different GM is connected");
            }

            this.gameMaster = null;
        }

        public void SendMessage(AgentLocalConnection agent, Message message)
        {
            if (agentToId.TryGetValue(agent, out int id))
            {
                message.AgentId = id;
                if (gameMaster != null)
                {
                    gameMaster.Messages.Add(message);
                }
                else
                {
                    throw new Exception("GM is not connected");
                }
            }
        }

        public void SendMessage(GMLocalConnection gameMaster, Message message)
        {
            int id = message.AgentId;
            // jeśli id jest złe to zgodnie ze specyfikacją jest wiadomość Invalid JSON... ale to będzie dopiero w "prawdziwym CSie"

            if (idToAgent.TryGetValue(id, out AgentLocalConnection agent))
            {
                agent.Messages.Add(message);
            }
        }
        #endregion
    }
}
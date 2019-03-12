using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionCommunicationRequest : IGameMasterAction
    {
        public ActionCommunicationRequest(int requesterId)
        {
            this.requesterId = requesterId;
        }

        public void Handle(Agent agent)
        {
            agent.ServeCommunicationRequest(requesterId);
        }

        int requesterId;
    }
}

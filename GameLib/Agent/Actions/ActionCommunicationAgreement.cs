using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionCommunicationAgreement : Action
    {
        int RequesterId;
        bool Agreement;
        object Data;

        public ActionCommunicationAgreement(int requesteerId, bool agreement, object data)
        {
            RequesterId = requesteerId;
            Agreement = agreement;
            Data = data;
        }

        public override void Execute(Agent agent)
        {
            agent.AgreeOnCommunication(RequesterId, Agreement, Data);
        }
    }
}

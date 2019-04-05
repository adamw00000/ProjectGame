using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionCommunicationAgreement : Action
    {
        public int RequesterId;
        public bool Agreement;
        public object Data;

        public ActionCommunicationAgreement(int requesterId, bool agreement, object data)
        {
            RequesterId = requesterId;
            Agreement = agreement;
            Data = data;
        }

        public override void Execute(Agent agent)
        {
            agent.AgreeOnCommunication(this);
        }

        public override string ToString()
        {
            return $"ActionCommunicationAgreement (sender agent: {RequesterId}, agreement status: {Agreement}, data: {Data})";
        }
    }
}

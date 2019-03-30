using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class ActionCommunicate : Action
    {
        public int TargetId;
        public object Data;

        public ActionCommunicate(int targetId, object data)
        {
            TargetId = targetId;
            Data = data;
        }

        public override void Execute(Agent agent)
        {
            agent.Communicate(TargetId, Data);
        }
    }
}

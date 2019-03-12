using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    class ActionCommunicationResponseWithData : IActionResponse
    {
        public ActionCommunicationResponseWithData(int waitUntilTime, object data)
        {
            this.WaitUntilTime = waitUntilTime;
            this.data = data;
        }

        public int WaitUntilTime { get; }

        public void Handle(Agent agent)
        {
            throw new NotImplementedException();
        }

        object data;
    }
}

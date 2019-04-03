using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Actions
{
    interface IActionError
    {
        int RequestTimestamp { get; }
        void Handle(Agent agent);
    }
}

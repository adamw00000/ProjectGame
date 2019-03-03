using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectionLib
{
    public interface IConnectionListener
    {
        IConnection Listen();
    }
}
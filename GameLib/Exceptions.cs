using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public class Exceptions
    {
        public class DelayException : Exception { }
        public class InvalidMoveException : Exception { }
        public class InvalidDiscoveryResultException : Exception { }
        public class InvalidCommunicationResultException : Exception { }
        public class PieceOperationException : Exception
        {
            public PieceOperationException(string message = "") : base(message) { }
        }
    }
}

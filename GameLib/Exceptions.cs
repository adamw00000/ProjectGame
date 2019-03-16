using System;

namespace GameLib
{
    public class DelayException : Exception
    {
        public DelayException() : base()
        {
        }

        public DelayException(string message) : base(message)
        {
        }

        public DelayException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() : base()
        {
        }

        public InvalidMoveException(string message) : base(message)
        {
        }

        public InvalidMoveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidDiscoveryResultException : Exception
    {
        public InvalidDiscoveryResultException() : base()
        {
        }

        public InvalidDiscoveryResultException(string message) : base(message)
        {
        }

        public InvalidDiscoveryResultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidCommunicationResultException : Exception
    {
        public InvalidCommunicationResultException() : base()
        {
        }

        public InvalidCommunicationResultException(string message) : base(message)
        {
        }

        public InvalidCommunicationResultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class PieceOperationException : Exception
    {
        public PieceOperationException(string message = "") : base(message)
        {
        }

        public PieceOperationException() : base()
        {
        }

        public PieceOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class CommunicationException : Exception
    {
        public CommunicationException() : base()
        {
        }

        public CommunicationException(string message = "") : base(message)
        {
        }

        public CommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
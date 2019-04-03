using System;
using System.Runtime.Serialization;

namespace GameLib
{
    public class DelayException : Exception
    {
        public DelayException() : base("Penalty hasn't finished yet")
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

    public class InteractiveModuleException : Exception
    {
        public InteractiveModuleException() : base()
        {
        }

        public InteractiveModuleException(string message = "") : base(message)
        {
        }

        public InteractiveModuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class GameSetupException : Exception
    {
        public GameSetupException()
        {
        }

        public GameSetupException(string message) : base(message)
        {
        }

        public GameSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidCommunicationDataException : Exception
    {
        public InvalidCommunicationDataException() : base()
        {
        }

        public InvalidCommunicationDataException(string message = "") : base(message)
        {
        }

        public InvalidCommunicationDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class InvalidRulesException : Exception
    {
        public InvalidRulesException() : base()
        {
        }

        public InvalidRulesException(string message) : base(message)
        {
        }

        public InvalidRulesException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class PendingLeaderCommunicationException : Exception
    {
        public PendingLeaderCommunicationException() : base()
        {
        }

        public PendingLeaderCommunicationException(string message) : base(message)
        {
        }

        public PendingLeaderCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class CommunicationInProgressException : Exception
    {
        public CommunicationInProgressException()
        {
        }

        public CommunicationInProgressException(string message) : base(message)
        {
        }

        public CommunicationInProgressException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
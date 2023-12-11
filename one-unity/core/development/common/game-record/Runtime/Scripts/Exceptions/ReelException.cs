using System;
using System.Runtime.Serialization;

namespace TPFive.Game.Record
{
    [Serializable]
    public class ReelException : Exception
    {
        public ReelException()
        {
        }

        public ReelException(string message)
            : base(message)
        {
        }

        public ReelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ReelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

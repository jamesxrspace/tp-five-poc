using System;

namespace TPFive.Room
{
    public class SpaceException : Exception
    {
        public SpaceException(string msg)
            : base(msg)
        {
        }
    }
}

using System.Collections.Generic;

namespace TPFive.Game.Mocap
{
    public class MismatchBlendShapeException : System.Exception
    {
        public MismatchBlendShapeException(List<ARKitBlendShapeLocation> locations, string message)
            : base(message)
        {
            Locations = locations;
        }

        public List<ARKitBlendShapeLocation> Locations { get; private set; }

        public override string ToString()
        {
            return $"{Message} {string.Join(", ", Locations)}";
        }
    }
}
using System;

namespace TPFive.Cross.Editor
{
    public class OrderedInitializeOnLoadAttribute : Attribute
    {
        public OrderedInitializeOnLoadAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}

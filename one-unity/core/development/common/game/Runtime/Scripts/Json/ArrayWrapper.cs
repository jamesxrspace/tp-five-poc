using System;

namespace TPFive.Game.Json
{
    [Serializable]
    public struct ArrayWarpper<T>
    {
        public T[] Items;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Extensions
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> target)
        {
            if (target == null)
            {
                return;
            }

            var n = target.Count;
            while (n > 1)
            {
                --n;
                var random = Random.Range(0, n);
                (target[random], target[n]) =
                    (target[n], target[random]);
            }
        }
    }
}
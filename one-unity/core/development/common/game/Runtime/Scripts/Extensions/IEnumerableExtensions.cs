using System.Collections.Generic;

namespace TPFive.Game.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void DisposeAll<T>(this IEnumerable<T> enumerable)
            where T : System.IDisposable
        {
            foreach (var item in enumerable)
            {
                item?.Dispose();
            }
        }
    }
}
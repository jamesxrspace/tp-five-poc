using System;

namespace TPFive.SCG.Utility
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string source, string value)
            => !source.EndsWith(value) ? source : source.Remove(source.LastIndexOf(value));
    }
}

using System.Globalization;

namespace TPFive.Game.Login.Entry.Extensions
{
    internal static class StringExtensions
    {
        public static string ToTitleCase(this string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }
    }
}

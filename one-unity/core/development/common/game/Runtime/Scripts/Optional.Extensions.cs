namespace TPFive.Game
{
    public static class OptionalExtensions
    {
        /// <summary>
        /// Convert an optional value to a nullable value.
        /// </summary>
        public static T? ToNullable<T>(this Optional<T> value)
            where T : struct
        {
            return value.HasValue ? value.Value : default(T?);
        }
    }
}

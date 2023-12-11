namespace TPFive.Game.Profile
{
    public enum MemAllocKind
    {
        /// <summary>
        /// Memory allocated by Unity engine.
        /// </summary>
        Unity,

        /// <summary>
        /// Memory allocated for Mono scripts/
        /// </summary>
        Mono,
    }
}

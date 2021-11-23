namespace ScoreboardToJSON.Enums
{
    /// <summary>
    /// Represents a teams warnings, if any
    /// </summary>
    public enum Warning
    {
        /// <summary>
        /// Team received a multiple instances warning
        /// </summary>
        MultiImage = 1 << 0,

        /// <summary>
        /// Team recieved an overtime warning
        /// </summary>
        TimeOver = 1 << 1,

        /// <summary>
        /// Team recieved a Withheld warning
        /// </summary>
        Withdrawn = 1 << 2
    }
}

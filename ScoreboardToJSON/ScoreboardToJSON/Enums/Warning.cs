namespace ScoreboardToJSON.Enums
{
    /// <summary>
    /// Represents a teams warnings, if any
    /// </summary>
    public enum Warning
    {
        /// <summary>
        /// Team has received no warning
        /// </summary>
        None = 0,

        /// <summary>
        /// Team received a multiple instances warning
        /// </summary>
        M,

        /// <summary>
        /// Team recieved an overtime warning
        /// </summary>
        T,

        /// <summary>
        /// Team recieved both a multiple instances warning and an overtime warning
        /// </summary>
        MT
    }
}

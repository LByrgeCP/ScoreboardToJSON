namespace ScoreboardToJSON
{
    /// <summary>
    /// Represents a teams Tier
    /// </summary>
    public enum Tier
    {
        /// <summary>
        /// Team is in silver tier
        /// </summary>
        Silver = 0,

        /// <summary>
        /// Team is in gold tier
        /// </summary>
        Gold,

        /// <summary>
        /// Team is in platinum tier
        /// </summary>
        Platinum,

        /// <summary>
        /// Team is in middle school or could not have its tier parsed.
        /// </summary>
        None
    }
}

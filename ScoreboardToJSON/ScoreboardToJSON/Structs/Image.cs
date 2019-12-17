using ScoreboardToJSON.Enums;
namespace ScoreboardToJSON.Structs
{
    /// <summary>
    /// A structure to represent an Image summary
    /// </summary>
    public struct Image
    {
        /// <summary>
        /// The name of the image
        /// </summary>
        public string imagename;

        /// <summary>
        /// How long the image has been opened
        /// </summary>
        public string playtime;

        /// <summary>
        /// How many vulnerabilities that the team found
        /// </summary>
        public int vulnsfound;

        /// <summary>
        /// How many vulnerabilities that the team has remaining 
        /// </summary>
        public int vulnsremaining;

        /// <summary>
        /// How many penalties that the team has received
        /// </summary>
        public int penalties;

        /// <summary>
        /// The total score that the team has earned
        /// </summary>
        public int score;

        /// <summary>
        /// The points possible to be earned on the image (usually 100)
        /// </summary>
        public int pointspossible;

        /// <summary>
        /// Warnings the team has recieved on the specific image.
        /// </summary>
        public Warning warnings;
    }
}

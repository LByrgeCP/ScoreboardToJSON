using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreboardToJSON
{
    public class ScoreboardTable
    {
        public int rankColumn;
        public int teamColumn;
        public int locationColumn;
        public int divisionColumn;
        public int tierColumn;
        public int imageCountColumn;
        public int playTimeColumn;
        public int warnColumn;
        public int ccsScoreColumn;
        public int adminAdjustColumn;
        public int challengeColumn;
        public int ciscoColumn;
        public int TotalScoreColumn;

        /// <summary>
        /// Load the ScoreboardTable configuration
        /// </summary>
        /// <param name="config">string of the line of the configuration for the scoreboard table</param>
        public ScoreboardTable(string config)
        {
            string[] confSplit = config.Split(',');

            // There should be twelve entries
            if (confSplit.Length != 13)
            {
                Console.WriteLine("Error: Invalied Scoreboard Configuration");
                Environment.Exit(1);
            }
            try
            {
                rankColumn = int.Parse(confSplit[0]);
                teamColumn = int.Parse(confSplit[1]);
                locationColumn = int.Parse(confSplit[2]);
                divisionColumn = int.Parse(confSplit[3]);
                tierColumn = int.Parse(confSplit[4]);
                imageCountColumn = int.Parse(confSplit[5]);
                playTimeColumn = int.Parse(confSplit[6]);
                warnColumn = int.Parse(confSplit[7]);
                ccsScoreColumn = int.Parse(confSplit[8]);
                adminAdjustColumn = int.Parse(confSplit[9]);
                challengeColumn = int.Parse(confSplit[10]);
                ciscoColumn = int.Parse(confSplit[11]);
                TotalScoreColumn = int.Parse(confSplit[12]);
            }
            catch
            {
                Console.WriteLine("Error: Could not parse ScoreboardTable config");
                Environment.Exit(1);
            }
        }
    }
}

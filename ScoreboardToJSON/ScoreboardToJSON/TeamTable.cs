﻿using System;

namespace ScoreboardToJSON
{
    public class TeamTable
    {
        public int teamColumn;
        public int locationColumn;
        public int divisionColumn;
        public int tierColumn;
        public int imageCountColumn;
        public int playTimeColumn;
        public int scoreTimeColumn;
        public int warnColumn;
        public int ccsScoreColumn;
        public int adminAdjustColumn;
        public int challengeColumn;
        public int ciscoColumn;
        public int TotalScoreColumn;
        /// <summary>
        /// Load the TeamTable configuration
        /// </summary>
        /// <param name="config">string of the line of the configuration for the team table</param>
        public TeamTable(string config)
        {
            string[] confSplit = config.Split(',');

            // There should be twelve entries
            if (confSplit.Length != 13)
            {
                Console.WriteLine("Error: Invalied TeamTable Configuration");
                Environment.Exit(1);
            }
            try
            {

                teamColumn = int.Parse(confSplit[0]);
                locationColumn = int.Parse(confSplit[1]);
                divisionColumn = int.Parse(confSplit[2]);
                tierColumn = int.Parse(confSplit[3]);
                imageCountColumn = int.Parse(confSplit[4]);
                playTimeColumn = int.Parse(confSplit[5]);
                scoreTimeColumn = int.Parse(confSplit[6]);
                warnColumn = int.Parse(confSplit[7]);
                ccsScoreColumn = int.Parse(confSplit[8]);
                adminAdjustColumn = int.Parse(confSplit[9]);
                challengeColumn = int.Parse(confSplit[10]);
                ciscoColumn = int.Parse(confSplit[11]);
                TotalScoreColumn = int.Parse(confSplit[12]);
            }
            catch
            {
                Console.WriteLine("Error: Could not parse TeamTable config");
                Environment.Exit(1);
            }
        }
    }
}

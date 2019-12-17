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
        public int ciscoColumn;
        public int TotalScoreColumn;
        public ScoreboardTable(string config)
        {
            string[] confSplit = config.Split(',');
            if (confSplit.Length != 12)
            {
                Console.WriteLine("Error: Invalied Scoreboard Configuration");
                Environment.Exit(1);
            }
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
            ciscoColumn = int.Parse(confSplit[10]);
            TotalScoreColumn = int.Parse(confSplit[11]);
        }
    }
}

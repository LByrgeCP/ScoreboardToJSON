using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreboardToJSON
{
    public class TeamTable
    {
        public int teamColumn;
        public int divisionColumn;
        public int tierColumn;
        public int imageCountColumn;
        public int playTimeColumn;
        public int scoreTimeColumn;
        public int warnColumn;
        public int ccsScoreColumn;
        public int adminAdjustColumn;
        public int ciscoColumn;
        public int TotalScoreColumn;
        public TeamTable(string config)
        {
            string[] confSplit = config.Split(',');
            teamColumn = int.Parse(confSplit[0]);
            divisionColumn = int.Parse(confSplit[1]);
            tierColumn = int.Parse(confSplit[2]);
            imageCountColumn = int.Parse(confSplit[3]);
            playTimeColumn = int.Parse(confSplit[4]);
            scoreTimeColumn = int.Parse(confSplit[5]);
            warnColumn = int.Parse(confSplit[6]);
            ccsScoreColumn = int.Parse(confSplit[7]);
            adminAdjustColumn = int.Parse(confSplit[8]);
            ciscoColumn = int.Parse(confSplit[9]);
            TotalScoreColumn = int.Parse(confSplit[10]);
        }
    }
}

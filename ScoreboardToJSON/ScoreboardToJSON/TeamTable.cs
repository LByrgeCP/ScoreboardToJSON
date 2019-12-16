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
        public int ciscoColumn;
        public int TotalScoreColumn;
        public TeamTable(string config)
        {
            string[] confSplit = config.Split(',');
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
            ciscoColumn = int.Parse(confSplit[10]);
            TotalScoreColumn = int.Parse(confSplit[11]);
        }
    }
}

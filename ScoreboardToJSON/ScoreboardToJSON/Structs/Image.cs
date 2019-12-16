using ScoreboardToJSON.Enums;
namespace ScoreboardToJSON.Structs
{
    public struct Image
    {
        public string imagename;
        public string playtime;
        public int vulnsfound;
        public int vulnsremaining;
        public int penalties;
        public int score;
        public int pointspossible;
        public Warning warnings;
    }
}

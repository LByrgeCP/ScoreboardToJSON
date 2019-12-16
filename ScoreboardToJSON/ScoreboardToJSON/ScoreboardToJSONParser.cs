using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScoreboardToJSON
{
    public class ScoreboardToJSONParser
    {
        private string ConfigFile;
        private string baseURL;
        private string OutputFile;
        public string ScoreboardHTML;
        public string[] TeamsStrArr;
        public ScoreboardToJSONParser(string fileInput, string urlInput, string fileOutput)
        {
            // Set needed variables
            ConfigFile = fileInput;
            baseURL = urlInput;
            OutputFile = fileOutput;

            // Download the html for mass parsing
            DownloadScoreboardHTML();

            // Find all teams on scoreboard
            GetTeamsFromScoreboard();
        }

        /// <summary>
        /// Downloads html from baseURL.
        /// </summary>
        public void DownloadScoreboardHTML()
        {
            WebClient client = new WebClient();
            ScoreboardHTML = client.DownloadString(baseURL);
            client.Dispose();
        }

        /// <summary>
        /// Finds all teams on scoreboard
        /// </summary>
        public void GetTeamsFromScoreboard()
        {
            MatchCollection teamsonscoreboard = Regex.Matches(ScoreboardHTML, @"12\-[0-9]{4}[^']");
            List<string> teamlist = new List<string>();
            foreach (Match match in teamsonscoreboard)
            {
                string matchstring = match.Value.Replace("<", "").Replace("12-", "");
                teamlist.Add(matchstring);
            }
            TeamsStrArr = teamlist.ToArray();
        }

        /// <summary>
        /// Get a List of Teams for output
        /// </summary>
        /// <param name="configline">
        /// Line from config file containing comma-separated 
        /// strings representing each row in the scoreboard table.
        /// </param>
        /// <returns>A List of Teans for the JSON</returns>
        public List<Team> GetTeams(string configline)
        {
            int currentteam = 1;
            int totalteams = TeamsStrArr.Length;
            List<Team> teamlist = new List<Team>();
            foreach (string team in TeamsStrArr)
            {
                double percentdone = (double)currentteam / (double)totalteams * 100;
                Console.WriteLine($"{currentteam}/{totalteams} 12-{team}   {Math.Round(percentdone, 2)}% finished with Team Parsing");
                teamlist.Add(new Team(ScoreboardHTML, configline, team, currentteam));
                currentteam++;
            }
            return teamlist;
        }
    }
}


//static void Main(string[] args)
//{
//    // Download Scoreboard HTML
//    WebClient client = new WebClient();
//    string ScoreboardHTML = client.DownloadString("http://scoreboard.uscyberpatriot.org");
//    client.Dispose();
//    // Grab all teams on the scoreboard for later parsing
//    MatchCollection teamsonscoreboard = Regex.Matches(ScoreboardHTML, @"12\-[0-9]{4}[^']");
//    List<Team> teamlist = new List<Team>();
//    int totalteams = teamsonscoreboard.Count;
//    int currentteam = 1;
//    foreach (Match match in teamsonscoreboard)
//    {
//        string matchstring = match.Value.Replace("<", "").Replace("12-", "");
//        double percentdone = (double)currentteam / (double)totalteams * 100;
//        Console.WriteLine($"{currentteam}/{totalteams} 12-{matchstring}   {Math.Round(percentdone, 2)}% finished with Team Parsing");
//        teamlist.Add(new Team(ScoreboardHTML, matchstring));
//        currentteam++;
//    }

//    // testing
//    //teamlist.Add(new Team(ScoreboardHTML, "0169"));
//    //teamlist.Add(new Team(ScoreboardHTML, "5675"));
//    //teamlist.Add(new Team(ScoreboardHTML, "7293"));
//    JArray JTeams = new JArray();

//    foreach (Team team in teamlist)
//    {
//        JObject tempjteam = new JObject(
//            new JProperty("TeamId", team.teamid),
//            new JProperty("Location", team.location),
//            new JProperty("Category", null),
//            new JProperty("Division", (int)team.division),
//            new JProperty("ImageCount", team.imagecount),
//            new JProperty("PlayTime", team.playtime),
//            new JProperty("TotalScore", team.totalscore),
//            new JProperty("Warnings", (int)team.warning),
//            new JProperty("Advancement", null)
//            );

//        if (team.tier == Tier.None)
//            tempjteam.Add(new JProperty("Tier", null));
//        else
//            tempjteam.Add(new JProperty("Tier", (int)team.tier));
//        JTeams.Add(tempjteam);
//    }
//    teamlist.Clear();
//    JProperty JteamsProperty = new JProperty("TeamList", JTeams);
//    JProperty Snapshot = new JProperty("SnapshotTimestamp", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null));
//    JProperty OriginURI = new JProperty("OriginUri", "http://scoreboard.uscyberpatriot.org/");
//    JProperty Filter = new JProperty(
//        "Filter",
//        new JObject(
//            new JProperty("Division", null),
//            new JProperty("Tier", null),
//            new JProperty("Category", null),
//            new JProperty("Location", null))
//        );
//    JProperty Summary = new JProperty(
//        "summary",
//        new JObject(
//            JteamsProperty,
//            Snapshot,
//            OriginURI,
//            Filter
//        ));
//    List<TeamAdvanced> AdvTeamList = new List<TeamAdvanced>();
//    currentteam = 1;
//    foreach (Match match in teamsonscoreboard)
//    {
//        string matchstring = match.Value.Replace("<", "").Replace("12-", "");
//        double percentdone = (double)currentteam / (double)totalteams * 100;
//        Console.WriteLine($"{currentteam}/{totalteams} 12-{matchstring}   {Math.Round(percentdone, 2)}% finished with AdvTeam Parsing");
//        AdvTeamList.Add(new TeamAdvanced(matchstring));
//        Thread.Sleep(2000);
//        currentteam++;
//    }

//    // testing
//    //AdvTeamList.Add(new TeamAdvanced("0169"));
//    //AdvTeamList.Add(new TeamAdvanced("5675"));
//    //AdvTeamList.Add(new TeamAdvanced("7293"));

//    JObject teams = new JObject();
//    foreach (TeamAdvanced team in AdvTeamList)
//    {
//        JProperty TeamSummary = new JProperty(
//            "Summary",
//            new JObject(
//                new JProperty("TeamId", team.teamid),
//                new JProperty("Location", team.location),
//                new JProperty("Category", null),
//                new JProperty("Division", (int)team.division),
//                new JProperty("ImageCount", team.imagecount),
//                new JProperty("PlayTime", team.playtime),
//                new JProperty("TotalScore", team.totalscore),
//                new JProperty("Warnings", (int)team.warning),
//                new JProperty("Advancement", null)
//            ));
//        if (team.tier == Tier.None)
//            TeamSummary.Add(new JProperty("Tier", null));
//        else
//            TeamSummary.Add(new JProperty("Tier", (int)team.tier));
//        JProperty ScoreTime = new JProperty("ScoreTime", team.scoretime);
//        JArray Images = new JArray();
//        foreach (Image image in team.images)
//        {
//            JObject imageobj = new JObject
//            {
//                new JProperty("ImageName", image.imagename),
//                new JProperty("PlayTime", image.playtime),
//                new JProperty("VulnerabilitiesFound", image.vulnsfound),
//                new JProperty("VulnerabilitiesRemaining", image.vulnsremaining),
//                new JProperty("Penalties", image.penalties),
//                new JProperty("Score", image.score),
//                new JProperty("PointsPossible", image.pointspossible),
//                new JProperty("Warnings", image.warnings)
//            };
//            Images.Add(imageobj);
//        }
//        JObject Ciscoobj = new JObject
//        {
//            new JProperty("ImageName", "Cisco (Total)"),
//            new JProperty("PlayTime", "00:00:00"),
//            new JProperty("VulnerabilitiesFound", 0),
//            new JProperty("VulnerabilitiesRemaining", 0),
//            new JProperty("Penalties", 0),
//            new JProperty("Score", team.ciscoScore),
//            new JProperty("PointsPossible", -1),
//            new JProperty("Warnings", 0)
//        };
//        Images.Add(Ciscoobj);
//        JObject AdminAdjust = new JObject
//        {
//            new JProperty("ImageName", "Administrative Adjustment"),
//            new JProperty("PlayTime", "00:00:00"),
//            new JProperty("VulnerabilitiesFound", 0),
//            new JProperty("VulnerabilitiesRemaining", 0),
//            new JProperty("Penalties", 0),
//            new JProperty("Score", team.AdministrativeAdjustment),
//            new JProperty("PointsPossible", 0),
//            new JProperty("Warnings", 0)
//        };
//        Images.Add(AdminAdjust);
//        JProperty originuri = new JProperty("OriginUri", team.originuri);
//        JProperty Snapshottime = new JProperty("SnapshotTimestamp", team.snapshottimestamp.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null));
//        JObject ImageScoresOverTime = new JObject();

//        foreach (KeyValuePair<string, SortedDictionary<DateTimeOffset, int?>> image in team.ImageScoresOverTime)
//        {
//            JObject imageobj = new JObject();
//            foreach (KeyValuePair<DateTimeOffset, int?> image1 in image.Value)
//            {
//                if (image1.Value == null)
//                    imageobj.Add(new JProperty(image1.Key.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null), null)); //2019-12-14T04:56:31+00:00
//                else

//                    imageobj.Add(new JProperty(image1.Key.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null), (int)image1.Value));
//            }
//            JProperty imageprop = new JProperty(image.Key, imageobj);
//            ImageScoresOverTime.Add(imageprop);
//        }
//        JProperty ImageScoresOverTimeProp = new JProperty("ImageScoresOverTime", ImageScoresOverTime);
//        JProperty Comment = new JProperty("Comment", null);
//        JProperty teamjprop = new JProperty(
//            team.teamid,
//            new JObject(
//                TeamSummary,
//                ScoreTime,
//                new JProperty("Images", Images),
//                originuri,
//                Snapshottime,
//                ImageScoresOverTimeProp,
//                Comment
//            ));
//        teams.Add(teamjprop);
//    }
//    JProperty teamsprop = new JProperty("teams", teams);
//    JProperty round = new JProperty("round", 0);
//    JObject jObject = new JObject(
//        Summary,
//        teamsprop,
//        round
//        );
//    File.WriteAllText(@"C:\Users\router\Desktop\out.json", jObject.ToString(Newtonsoft.Json.Formatting.None));
//    Console.Write("Press any enter to continue... ");
//    Console.ReadLine();
//}
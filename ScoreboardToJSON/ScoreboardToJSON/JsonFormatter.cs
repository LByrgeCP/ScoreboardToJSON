using Newtonsoft.Json.Linq;
using ScoreboardToJSON.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreboardToJSON
{
    public class JsonFormatter
    {
        public JObject OutJSON = new JObject();

        public void CreateOutJSON(List<Team> teamlist, List<TeamAdvanced> advteamlist, string outfile)
        {
            Console.WriteLine("Adding team summaries to json");
            AddSummaryJSON(teamlist);
            Console.WriteLine("Adding team details to json");
            AddDetailedJSON(advteamlist);
            Console.WriteLine("Adding round to json");
            AddRoundJSON();
            Console.WriteLine($"Writing output to {outfile}");
            File.WriteAllText(outfile, OutJSON.ToString(Newtonsoft.Json.Formatting.None));
        }

        public void AddSummaryJSON(List<Team> teamlist)
        {
            JArray JTeams = new JArray();
            foreach (Team team in teamlist)
            {
                JObject tempjteam = new JObject(
                    new JProperty("TeamId", team.teamid),
                    new JProperty("Location", team.location),
                    new JProperty("Category", null),
                    new JProperty("Division", (int)team.division),
                    new JProperty("ImageCount", team.imagecount),
                    new JProperty("PlayTime", team.playtime),
                    new JProperty("TotalScore", team.totalscore),
                    new JProperty("Warnings", (int)team.warning),
                    new JProperty("Advancement", null)
                    );

                if (team.tier == Tier.None)
                    tempjteam.Add(new JProperty("Tier", null));
                else
                    tempjteam.Add(new JProperty("Tier", (int)team.tier));
                JTeams.Add(tempjteam);
            }
            teamlist.Clear();


            JProperty JteamsProperty = new JProperty("TeamList", JTeams);
            JProperty Snapshot = new JProperty("SnapshotTimestamp", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null));
            JProperty OriginURI = new JProperty("OriginUri", "http://scoreboard.uscyberpatriot.org/");
            JProperty Filter = new JProperty(
                "Filter",
                new JObject(
                    new JProperty("Division", null),
                    new JProperty("Tier", null),
                    new JProperty("Category", null),
                    new JProperty("Location", null))
                );
            JProperty Summary = new JProperty(
                "summary",
                new JObject(
                    JteamsProperty,
                    Snapshot,
                    OriginURI,
                    Filter
                ));
            OutJSON.Add(Summary);
        }

        public void AddDetailedJSON(List<TeamAdvanced> AdvTeamList)
        {
            JObject teams = new JObject();
            foreach (TeamAdvanced team in AdvTeamList)
            {
                JObject teamjobj = new JObject(
                     new JProperty("TeamId", team.teamid),
                     new JProperty("Location", team.location),
                     new JProperty("Category", null),
                     new JProperty("Division", (int)team.division),
                     new JProperty("ImageCount", team.imagecount),
                     new JProperty("PlayTime", team.playtime),
                     new JProperty("TotalScore", team.totalscore),
                     new JProperty("Warnings", (int)team.warning),
                     new JProperty("Advancement", null));
                if (team.tier == Tier.None)
                    teamjobj.Add(new JProperty("Tier", null));
                else
                    teamjobj.Add(new JProperty("Tier", (int)team.tier));
                JProperty TeamSummary = new JProperty(
                    "Summary",
                    teamjobj
                    );
                JProperty ScoreTime = new JProperty("ScoreTime", team.scoretime);
                JArray Images = new JArray();
                foreach (Image image in team.images)
                {
                    JObject imageobj = new JObject
                        {
                            new JProperty("ImageName", image.imagename),
                            new JProperty("PlayTime", image.playtime),
                            new JProperty("VulnerabilitiesFound", image.vulnsfound),
                            new JProperty("VulnerabilitiesRemaining", image.vulnsremaining),
                            new JProperty("Penalties", image.penalties),
                            new JProperty("Score", image.score),
                            new JProperty("PointsPossible", image.pointspossible),
                            new JProperty("Warnings", image.warnings)
                        };
                    Images.Add(imageobj);
                }
                JObject Ciscoobj = new JObject
                    {
                        new JProperty("ImageName", "Cisco (Total)"),
                        new JProperty("PlayTime", "00:00:00"),
                        new JProperty("VulnerabilitiesFound", 0),
                        new JProperty("VulnerabilitiesRemaining", 0),
                        new JProperty("Penalties", 0),
                        new JProperty("Score", team.ciscoScore),
                        new JProperty("PointsPossible", -1),
                        new JProperty("Warnings", 0)
                    };
                Images.Add(Ciscoobj);
                JObject AdminAdjust = new JObject
                    {
                        new JProperty("ImageName", "Administrative Adjustment"),
                        new JProperty("PlayTime", "00:00:00"),
                        new JProperty("VulnerabilitiesFound", 0),
                        new JProperty("VulnerabilitiesRemaining", 0),
                        new JProperty("Penalties", 0),
                        new JProperty("Score", team.AdministrativeAdjustment),
                        new JProperty("PointsPossible", 0),
                        new JProperty("Warnings", 0)
                    };
                Images.Add(AdminAdjust);
                JProperty originuri = new JProperty("OriginUri", team.originuri);
                JProperty Snapshottime = new JProperty("SnapshotTimestamp", team.snapshottimestamp.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null));
                JObject ImageScoresOverTime = new JObject();

                foreach (KeyValuePair<string, SortedDictionary<DateTimeOffset, int?>> image in team.ImageScoresOverTime)
                {
                    JObject imageobj = new JObject();
                    foreach (KeyValuePair<DateTimeOffset, int?> image1 in image.Value)
                    {
                        if (image1.Value == null)
                            imageobj.Add(new JProperty(image1.Key.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null), null)); //2019-12-14T04:56:31+00:00
                        else

                            imageobj.Add(new JProperty(image1.Key.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null), (int)image1.Value));
                    }
                    JProperty imageprop = new JProperty(image.Key, imageobj);
                    ImageScoresOverTime.Add(imageprop);
                }
                JProperty ImageScoresOverTimeProp = new JProperty("ImageScoresOverTime", ImageScoresOverTime);
                JProperty Comment = new JProperty("Comment", null);
                JProperty teamjprop = new JProperty(
                    team.teamid,
                    new JObject(
                        TeamSummary,
                        ScoreTime,
                        new JProperty("Images", Images),
                        originuri,
                        Snapshottime,
                        ImageScoresOverTimeProp,
                        Comment
                    ));
                teams.Add(teamjprop);
            }
            OutJSON.Add(new JProperty("teams", teams));
        }
        public void AddRoundJSON()
        {
            Console.Write("Input the round number (1, 2, 3, 4): ");
            string input = Console.ReadLine();
            int round;
            try
            {
                round = int.Parse(input);
            }
            catch
            {
                round = 0;
            }
            JProperty roundprop = new JProperty("round", round);
            OutJSON.Add(roundprop);
        }
    }
}

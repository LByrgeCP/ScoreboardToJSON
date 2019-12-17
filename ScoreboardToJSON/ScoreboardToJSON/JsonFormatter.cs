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
        /// <summary>
        /// The JSON that will be returned via file
        /// </summary>
        public JObject OutJSON = new JObject();

        /// <summary>
        /// Create and write the JSON to a file
        /// </summary>
        /// <param name="teamlist">The list of Team's to add to the json "summary" section</param>
        /// <param name="advteamlist">The list of TeamAdvanced's to add to the json "teams" section</param>
        /// <param name="outfile">The file to write the json to</param>
        public void CreateOutJSON(List<Team> teamlist, List<TeamAdvanced> advteamlist, string outfile)
        {
            // Add summary section of the json
            Console.WriteLine("Adding team summaries to json");
            AddSummaryJSON(teamlist);

            // Add the detailed "teams" section of the json
            Console.WriteLine("Adding team details to json");
            AddDetailedJSON(advteamlist);

            // Add the round number to the json
            Console.WriteLine("Adding round to json");
            AddRoundJSON();

            // Write the JSON to the output file
            Console.WriteLine($"Writing output to {outfile}");
            File.WriteAllText(outfile, OutJSON.ToString(Newtonsoft.Json.Formatting.None));
        }

        /// <summary>
        /// Add the summary section of the jsons
        /// </summary>
        /// <param name="teamlist">List of Team's to add</param>
        public void AddSummaryJSON(List<Team> teamlist)
        {
            // For every team in the team list, add them to
            // a JSON Array
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

            // Add the JTeams JSON Array to a new JSON Property
            JProperty JteamsProperty = new JProperty("TeamList", JTeams);

            // Add the current time to a new JSON Property
            JProperty Snapshot = new JProperty("SnapshotTimestamp", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00", null));

            // Add the scoreboard url to a new JSON Property
            JProperty OriginURI = new JProperty("OriginUri", "http://scoreboard.uscyberpatriot.org/");

            // Create a JSON Property with the filter
            JProperty Filter = new JProperty(
                "Filter",
                new JObject(
                    new JProperty("Division", null),
                    new JProperty("Tier", null),
                    new JProperty("Category", null),
                    new JProperty("Location", null))
                );

            // Add all the JSON Properties to a new JSON Properties
            JProperty Summary = new JProperty(
                "summary",
                new JObject(
                    JteamsProperty,
                    Snapshot,
                    OriginURI,
                    Filter
                ));

            // Add the summary Property to the output JSON
            OutJSON.Add(Summary);
        }
        
        /// <summary>
        /// Add Team's detailed data to the json
        /// </summary>
        /// <param name="AdvTeamList">List of TeamAdvanced's to add</param>
        public void AddDetailedJSON(List<TeamAdvanced> AdvTeamList)
        {
            // TODO: Finish commenting
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

        /// <summary>
        /// Add the current round (via user input) to the JSON
        /// </summary>
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

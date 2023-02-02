using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        public string boeingJSON;
        public ScoreboardToJSONParser(string fileInput, string urlInput, string fileOutput, string year, string boeing, string[] customTeamlist = null)
        {
            // Set needed variables
            ConfigFile = fileInput;
            baseURL = urlInput;
            OutputFile = fileOutput;

            if(boeing != null)
            {
                // download contents of the url in boeing to boeingJSON
                try
                {
                    // Create a new webclient
                    WebClient client = new WebClient();

                    // boeingJSON webclient data
                    boeingJSON = client.DownloadString(boeing);

                    // Dispose of client just in case
                    client.Dispose();
                }
                catch
                {
                    Console.WriteLine($"Downloading URL {baseURL} Failed");
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            // Download the html for mass parsing
            DownloadScoreboardHTML();
            
            if (customTeamlist == null)
                // Find all teams on scoreboard
                GetTeamsFromScoreboard(year);
            else
                // Set custom input teams to TeamsStrArr
                TeamsStrArr = customTeamlist;
            // Grab config file and remove comments
            string[] conflines = GetConfigLines(fileInput);

            // Exit if more than two config lines
            if (conflines.Length != 2)
            {
                Console.WriteLine($"Invald configuration {fileInput}");
                Environment.Exit(1);
            }

            // Grab all Teams summary data as List<Team>
            List<Team> teamList = GetTeams(conflines[0], year);

            // Grab all Teams descriptive data as List<TeamAdvanced>
            List<TeamAdvanced> teamAdvList = GetAdvTeams(conflines[1], year);

            JsonFormatter jsonFormatter = new JsonFormatter();
            jsonFormatter.CreateOutJSON(teamList, teamAdvList, fileOutput);
        }

        /// <summary>
        /// Downloads html from baseURL.
        /// </summary>
        public void DownloadScoreboardHTML()
        {
            try
            {
                // Create a new webclient
                WebClient client = new WebClient();

                // Download webclient data
                ScoreboardHTML = client.DownloadString(baseURL);

                // Dispose of client just in case
                client.Dispose();
            } catch
            {
                Console.WriteLine($"Downloading URL {baseURL} Failed, trying again in 5 seconds");
                DownloadScoreboardHTML();
            }
        }

        public bool DoesTeamExist(string team, string year)
        {
            try
            {

                return ScoreboardHTML.Contains($"{year}-{team}");
            }
            catch
            {
                Console.WriteLine($"Downloading URL {baseURL} Failed, trying again in 5 seconds");
                Thread.Sleep(5000);
                return DoesTeamExist(team, year);
            }
        }

        public string[] GetConfigLines(string filename)
        {
            // Exit if config file doesnt exist
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Error: File {filename} does not exist.");
                Environment.Exit(1);
            }

            // Loop through all lines in file and return non-comment lines
            List<string> linesOut = new List<string>();
            string[] filelines = File.ReadAllLines(filename);
            foreach (string line in filelines)
                if (!Regex.IsMatch(line, "^\\s*#"))
                    linesOut.Add(line);
            return linesOut.ToArray();
        }

        /// <summary>
        /// Finds all teams on scoreboard
        /// </summary>
        public void GetTeamsFromScoreboard(string year)
        {
            // Regex all teams and set TeamsStrArr to an array of all team numbers (not includng {year}-)
            MatchCollection teamsonscoreboard = Regex.Matches(ScoreboardHTML, $"{year}\\-[0-9]{{4}}[^']");
            List<string> teamlist = new List<string>();
            foreach (Match match in teamsonscoreboard)
            {
                string matchstring = match.Value.Replace("<", "").Replace($"{year}-", "");
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
        public List<Team> GetTeams(string configline, string year)
        {
            // For printing how many teams are parsed
            int currentteam = 1;
            int totalteams = TeamsStrArr.Length;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(ScoreboardHTML);
            // loop through all teams and add each one to teamlist
            List<Team> teamlist = new List<Team>();
            foreach (string team in TeamsStrArr)
            {
                double percentdone = (double)currentteam / (double)totalteams * 100;
                // Console.WriteLine($"{currentteam}/{totalteams} {year}-{team}   {Math.Round(percentdone, 2)}% finished with Team Summaries Parsing");
                if (DoesTeamExist(team, year))
                    teamlist.Add(new Team(configline, team, GetTeamRank(team), year, doc));
                else
                    Console.WriteLine($"Team {year}-{team} does not exist, skipping...");
                currentteam++;
            }
            return teamlist;
        }
        /// <summary>
        /// Get a List of TeamAdvanced for output
        /// </summary>
        /// <param name="configline">
        /// Line from config file containing comma-separated 
        /// strings representing each row in a teams table.
        /// </param>
        /// <returns>List of TeamAdvanced</returns>
        public List<TeamAdvanced> GetAdvTeams(string configline, string year)
        {
            // For printing how many teams are parsed
            int currentteam = 1;
            int totalteams = TeamsStrArr.Length;
            
            // loop through all teams and add each one to teamlist
            List<TeamAdvanced> teamlist = new List<TeamAdvanced>();
            foreach (string team in TeamsStrArr)
            {
                double percentdone = (double)currentteam / (double)totalteams * 100;
                Console.WriteLine($"{currentteam}/{totalteams} {year}-{team}   {Math.Round(percentdone, 2)}% finished with Team Details Parsing");
                if (DoesTeamExist(team, year))
                    teamlist.Add(new TeamAdvanced(configline, team, year, baseURL, boeingJSON));
                else
                    Console.WriteLine($"Team {year}-{team} does not exist, skipping...");
                currentteam++;
                Thread.Sleep(2000);
            }
            return teamlist;
        }

        private int GetTeamRank(string team)
        {
            Match match = Regex.Match(ScoreboardHTML, $"{team}'><td>[0-9]{{1,4}}");
            int rank = int.Parse(match.Value.Replace($"{team}'><td>", ""));
            return rank;
        }
    }
}
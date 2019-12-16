using HtmlAgilityPack;
using ScoreboardToJSON.Enums;
using ScoreboardToJSON.Structs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace ScoreboardToJSON
{
    public class TeamAdvanced
    {
        public string TeamscoreboardHtml;
        public string teamnumber;
        public string teamid;
        public string location;
        public Division division;
        public Tier tier;
        public int imagecount;
        public string playtime;
        public double totalscore;
        public Warning warning;

        public string scoretime;
        public Image[] images;
        public string originuri;
        public DateTimeOffset snapshottimestamp;
        public Dictionary<string, SortedDictionary<DateTimeOffset, int?>> ImageScoresOverTime = new Dictionary<string, SortedDictionary<DateTimeOffset, int?>>();
        public double ciscoScore;
        public int AdministrativeAdjustment;

        public TeamTable teamtable;

        public TeamAdvanced(string configline, string Teamnumber)
        {
            teamnumber = Teamnumber;
            teamid = $"12-{teamnumber}";
            originuri = $"http://scoreboard.uscyberpatriot.org/team.php?team={teamid}";
            teamtable = new TeamTable(configline);
            GetTeamScoreboardHtml();
            location = GetLocationFromScoreboard();
            tier = GetTierFromScoreboard();
            division = GetDivisionFromScoreboard();
            playtime = GetPlayTimeFromScoreboard();
            totalscore = GetTotalScoreFromScoreboard();
            warning = GetWarningFromScoreboard();
            ciscoScore = GetCiscoScore();
            scoretime = GetScoreTimeFromScoreboard();
            GetAdvancedImages();
            GetImages();
            snapshottimestamp = DateTimeOffset.UtcNow;
            AdministrativeAdjustment = GetAdminAdjust();
            imagecount = images.Length;
        }
        public void GetTeamScoreboardHtml()
        {
            try
            {
                WebClient client = new WebClient();
                TeamscoreboardHtml = client.DownloadString(originuri);
                client.Dispose();
            }
            catch
            {
                Console.WriteLine($"Downloading URL {originuri} Failed, trying again in 5 seconds");
                Thread.Sleep(5000);
                GetTeamScoreboardHtml();
            }
        }
        public string GetLocationFromScoreboard()
        {
            if (teamtable.locationColumn == -1)
                return "N/A";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.locationColumn}]");
            return node.InnerText;
        }
        public Division GetDivisionFromScoreboard()
        {
            if (teamtable.divisionColumn == -1)
                return Division.None;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.divisionColumn}]");
            string line = node.InnerText;
            if (line.Contains("Open"))
                return Division.Open;
            else if (line.Contains("Middle"))
                return Division.Middle;
            else
                return Division.AS;
        }
        public Tier GetTierFromScoreboard()
        {
            if (teamtable.tierColumn == -1)
                return Tier.None;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.tierColumn}]");
            string line = node.InnerText;
            if (line.Contains("Platinum"))
                return Tier.Platinum;
            else if (line.Contains("Gold"))
                return Tier.Gold;
            else if (line.Contains("Silver"))
                return Tier.Silver;
            else
                return Tier.None;
        }
        public Warning GetWarningFromScoreboard()
        {
            if (teamtable.warnColumn == -1)
                return Warning.None;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.warnColumn}]");
            string line = node.InnerText;
            if (line.Contains("MT"))
                return Warning.MT;
            else if (line == "M")
                return Warning.M;
            else if (line == "T")
                return Warning.T;
            else
                return Warning.None;
        }
        public string GetPlayTimeFromScoreboard()
        {
            if (teamtable.playTimeColumn == -1)
                return "00:00:00";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.playTimeColumn}]");
            string line = node.InnerText;
            string[] timesplit = line.Split(':');
            return new TimeSpan(int.Parse(timesplit[0]),    // hours
                           int.Parse(timesplit[1]),         // minutes
                           0).ToString();
        }

        public double GetTotalScoreFromScoreboard()
        {
            if (teamtable.TotalScoreColumn == -1)
                return 0;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.TotalScoreColumn}]");
            string line = node.InnerText;
            return double.Parse(line);
        }

        public string GetScoreTimeFromScoreboard()
        {
            if (teamtable.scoreTimeColumn == -1)
                return "00:00:00";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.scoreTimeColumn}]");
            string line = node.InnerText;
            string[] timesplit = line.Split(':');
            return new TimeSpan(int.Parse(timesplit[0]),    // hours
                           int.Parse(timesplit[1]),         // minutes
                           0).ToString();
        }

        public void GetAdvancedImages()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/script[1]");
            MatchCollection matches = Regex.Matches(node.InnerText, @"\[.*\]");
            string timeline = matches[1].Value;
            List<Line> lines = new List<Line>();
            foreach (Match match in matches)
            {
                string val = match.Value;
                if (val.Contains("corechart") || val.Contains("Time")) continue;
                string time = Regex.Match(val, "'.*'").Value.Replace("'", "");
                List<int?> scores = new List<int?>();
                MatchCollection intmatches = Regex.Matches(val, ", (-*[0-9]{1,3}|null)");
                foreach (Match intmatch in intmatches)
                {
                    string valueint = intmatch.Value.Replace(", ", "");
                    if (valueint == "null")
                        scores.Add(null);
                    else
                        scores.Add(int.Parse(valueint));
                }
                Line line = new Line();
                line.scores = scores.ToArray();
                line.time = time;
                lines.Add(line);
            }
            MatchCollection images = Regex.Matches(timeline, "'[^']*'");
            for (int l = 1; l < images.Count; l++)
            {
                string imagename = images[l].Value.Replace("'", "");
                SortedDictionary<DateTimeOffset, int?> keyValuePairs = new SortedDictionary<DateTimeOffset, int?>();
                foreach (Line lin in lines)
                {
                    DateTimeOffset dto = default(DateTimeOffset);
                    try
                    {
                        // MM/dd hh:mm
                        string dateStr = lin.time;
                        string[] dateStrComponents = dateStr.Split(' ');
                        string[] dateComponents = dateStrComponents[0].Split('/');
                        string[] timeComponents = dateStrComponents[1].Split(':');
                        dto = new DateTimeOffset(DateTimeOffset.UtcNow.Year, int.Parse(dateComponents[0]), int.Parse(dateComponents[1]), int.Parse(timeComponents[0]), int.Parse(timeComponents[1]), 0, TimeSpan.Zero);
                    }
                    catch
                    {
                        continue;
                    }
                    keyValuePairs.Add(dto, lin.scores[l - 1]);
                }
                ImageScoresOverTime.Add(imagename, keyValuePairs);
            }
        }

        public void GetImages()
        {
            List<Image> imagelist = new List<Image>();
            foreach (KeyValuePair<string, SortedDictionary<DateTimeOffset, int?>> keyValuePair in ImageScoresOverTime)
            {
                Match match = Regex.Match(TeamscoreboardHtml, "^.*" + keyValuePair.Key + ".*$", RegexOptions.Multiline);
                string line = match.Value.Replace("<tr><td>", "").Replace("</td></tr>", "");
                string[] Values = line.Split(new[] { "</td><td>" }, StringSplitOptions.None);
                Image image = new Image();
                image.imagename = Values[0];
                image.playtime = GetCorrectPlayTime(Values[1].Trim());
                image.vulnsfound = int.Parse(Values[2]);
                image.vulnsremaining = int.Parse(Values[3]);
                image.penalties = int.Parse(Values[4]);
                image.score = int.Parse(Values[5]);
                image.warnings = StringToWarning(Values[6]);
                image.pointspossible = 100;
                imagelist.Add(image);
            }
            images = imagelist.ToArray();
        }

        public string GetCorrectPlayTime(string play1)
        {
            string[] timesplit = play1.Split(':');
            return new TimeSpan(int.Parse(timesplit[0]),    // hours
                           int.Parse(timesplit[1]),         // minutes
                           0).ToString();
        }

        public Warning StringToWarning(string str)
        {
            if (str == "M")
                return Warning.M;
            else if (str == "T")
                return Warning.T;
            else if (str == "MT")
                return Warning.MT;
            else
                return Warning.None;
        }
        public double GetCiscoScore()
        {
            if (teamtable.ciscoColumn == -1)
                return 0;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.ciscoColumn}]");
            string line = node.InnerText;
            return double.Parse(line);
        }
        public int GetAdminAdjust()
        {
            if (teamtable.adminAdjustColumn == -1)
                return 0;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.adminAdjustColumn}]");
            string line = node.InnerText;
            return (int)double.Parse(line);
        }

        public string comment = null;
    }
}

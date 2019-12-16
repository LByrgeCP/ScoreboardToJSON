using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreboardToJSON.Structs;
using ScoreboardToJSON.Enums;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;
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
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table[1]/tr[2]/td[{teamtable.locationColumn}]");
            return node.InnerText;
        }
        public Division GetDivisionFromScoreboard()
        {
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
            if (TeamscoreboardHtml.Contains("<td>Platinum"))
                return Tier.Platinum;
            else if (TeamscoreboardHtml.Contains("<td>Gold"))
                return Tier.Gold;
            else if (TeamscoreboardHtml.Contains("<td>Silver"))
                return Tier.Silver;
            else
                return Tier.None;
        }
        public Warning GetWarningFromScoreboard()
        {
            if (TeamscoreboardHtml.Contains(">MT<"))
                return Warning.MT;
            else if (TeamscoreboardHtml.Contains("td><b>M</b"))
                return Warning.M;
            else if (TeamscoreboardHtml.Contains("td><b>T</b"))
                return Warning.T;
            else
                return Warning.None;
        }
        public string GetPlayTimeFromScoreboard()
        {
            Match teammatch = Regex.Match(TeamscoreboardHtml, "<td>[1-9]*</td><td>[0-9]{2,}:[0-9]{2,}", RegexOptions.Multiline);
            string timestr =  Regex.Replace(teammatch.Value, "<td>[1-9]*</td><td>", "");
            string[] timesplit = timestr.Split(':');
            return new TimeSpan(int.Parse(timesplit[0]),    // hours
                           int.Parse(timesplit[1]),         // minutes
                           0).ToString();
        }

        public double GetTotalScoreFromScoreboard()
        {
            try
            {
                Match scorematch = Regex.Match(TeamscoreboardHtml, "[0-9.]+</td></tr>");
                return double.Parse(scorematch.Value.Replace("</td></tr>", ""));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public string GetScoreTimeFromScoreboard()
        {
            Match teammatch = Regex.Match(TeamscoreboardHtml, "[0-9]{2,}:[0-9]{2,}</td><td><", RegexOptions.Multiline);
            string timestr = teammatch.Value.Replace("</td><td><", "");
            string[] timesplit = timestr.Split(':');
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
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/table/tr[2]/td[11]");
            try
            {
                return double.Parse(node.InnerText);
            }
            catch
            {
                return 0;
            }
        }
        public int AdministrativeAdjustment;
        public int GetAdminAdjust()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TeamscoreboardHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/table/tr[2]/td[10]");
            try
            {
                return (int)double.Parse(node.InnerText);
            }
            catch
            {
                return 0;
            }
        }

        public string comment = null;
    }
}

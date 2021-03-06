﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreboardToJSON.Structs;
using ScoreboardToJSON.Enums;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ScoreboardToJSON
{
    public class Team
    {
        string ScoreboardHTML;
        public string teamnumber;
        public string teamid;
        public string location;
        public Division division;
        public Tier tier;
        public int imagecount;
        public string playtime;
        public double totalscore;
        public Warning warning;
        public string configLine;
        public ScoreboardTable scoreboardTable;
        public int rank;
        public HtmlDocument doc;
        public Team(string html, string config, string Teamnumber, int teamplace, HtmlDocument document)
        {
            doc = document;
            rank = teamplace;
            ScoreboardHTML = html;
            configLine = config;
            scoreboardTable = new ScoreboardTable(configLine);
            teamnumber = Teamnumber;
            html = null;
            teamid = "13-" + Teamnumber;
            location = GetLocationFromScoreboard();
            division = GetDivisionFromScoreboard();
            tier = GetTierFromScoreboard();
            imagecount = GetImageCount();
            playtime = GetPlaytime();
            totalscore = GetTotalScore();
            warning = GetWarningFromScoreboard();
        }
        public double GetTotalScore()
        {
            if (scoreboardTable.TotalScoreColumn == -1)
                return 0;
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.TotalScoreColumn}]");
            return double.Parse(node.InnerText);
        }
        public string GetPlaytime()
        {
            if (scoreboardTable.playTimeColumn == -1)
                return "00:00:00";
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.playTimeColumn}]");

            // So teams >24 hours dont break bot
            string[] timesplit = node.InnerText.Split(':');
            return new TimeSpan(int.Parse(timesplit[0]),    // hours
                           int.Parse(timesplit[1]),         // minutes
                           int.Parse(timesplit[2])).ToString();
        }

        public int GetImageCount()
        {
            if (scoreboardTable.imageCountColumn == -1)
                return 0;
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.imageCountColumn}]");
            return int.Parse(node.InnerText);
        }
        public Division GetDivisionFromScoreboard()
        {
            if (scoreboardTable.divisionColumn == -1)
                return Division.None;
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.divisionColumn}]");
            string line = node.InnerText;
            if (line.Contains("Open"))
                return Division.Open;
            else if (line.Contains("Middle"))
                return Division.Middle;
            else if (line.Contains("High"))
                return Division.Open;
            else
                return Division.AS;
        }
        public string GetLocationFromScoreboard()
        {
            if (scoreboardTable.locationColumn == -1)
                return "N/A";
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.locationColumn}]");
            if (node.InnerText == "")
                return "N/A";
            return node.InnerText;
        }
        public Tier GetTierFromScoreboard()
        {
            if (scoreboardTable.tierColumn == -1)
                return Tier.None;
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.tierColumn}]");
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
            if (scoreboardTable.warnColumn == -1)
                return Warning.None;
            HtmlNode node = doc.DocumentNode.SelectSingleNode($"/html/body/div[2]/div/table/tr[{rank + 1}]/td[{scoreboardTable.warnColumn}]");
            string line = node.InnerText;
            if (line.Contains("MT") || line.Contains("TM"))
                return Warning.MT;
            else if (line.Contains("M"))
                return Warning.M;
            else if (line.Contains("T"))
                return Warning.T;
            else
                return Warning.None;
        }
    }
}

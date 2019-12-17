using Newtonsoft.Json.Linq;
using ScoreboardToJSON.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace ScoreboardToJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Show_Error("No input");
                Show_Help();
                return;
            }
            if(args[0] == "-h")
            {
                Show_Help();
                return;
            }
            if(args[0] != "-f")
            {
                Show_Error($"Incorrect Input {args[0]}, expected -f");
                Show_Help();
                return;
            }
            string fileInput = args[1];
            string urlInput = "http://scoreboard.uscyberpatriot.org/";
            string fileOutput = "Scoreboard.json";
            string[] customteamsinput = null;
            if (args.Length > 2)
            {
                if(args[2] == "-u")
                {
                    urlInput = args[3];
                    if (!Uri.IsWellFormedUriString(urlInput, UriKind.Absolute))
                    {
                        Show_Error($"Invalid url {urlInput}, using default.");
                        urlInput = "http://scoreboard.uscyberpatriot.org/";
                    }
                } else if (args[2] == "-o")
                {
                    fileOutput = args[3];
                }
                else if (args[2] == "-t")
                {
                    customteamsinput = ParseCustomTeams(args[3]);
                }
                else
                {
                    Show_Error($"Incorrect Input {args[2]}");
                    Show_Help();
                    return;
                }
            }
            if (args.Length > 4)
            {
                if (args[4] == "-u")
                {
                    urlInput = args[5];
                    if (!Uri.IsWellFormedUriString(urlInput, UriKind.Absolute))
                    {
                        Show_Error($"Invalid url {urlInput}, using default.");
                        urlInput = "http://scoreboard.uscyberpatriot.org/";
                    }
                }
                else if (args[4] == "-o")
                {
                    fileOutput = args[5];
                }
                else if (args[4] == "-t")
                {
                    customteamsinput = ParseCustomTeams(args[5]);
                }
                else
                {
                    Show_Error($"Incorrect Input {args[4]}");
                    Show_Help();
                    return;
                }
            }
            if (args.Length > 6)
            {
                if (args[6] == "-u")
                {
                    urlInput = args[5];
                    if (!Uri.IsWellFormedUriString(urlInput, UriKind.Absolute))
                    {
                        Show_Error($"Invalid url {urlInput}, using default.");
                        urlInput = "http://scoreboard.uscyberpatriot.org/";
                    }
                }
                else if (args[6] == "-o")
                {
                    fileOutput = args[5];
                } else if (args[6] == "-t")
                {
                    customteamsinput = ParseCustomTeams(args[7]);
                }
                else
                {
                    Show_Error($"Incorrect Input {args[5]}");
                    Show_Help();
                    return;
                }
            }
            if(args.Length > 8)
            {
                Show_Error($"Incorrect Input");
                Show_Help();
                return; 
            }
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            new ScoreboardToJSONParser(fileInput, urlInput, fileOutput, customteamsinput);
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.Elapsed}");
            Console.WriteLine("Press any enter to exit...");
            Console.ReadLine();
        }
        public static void Show_Help()
        {
            Console.WriteLine("Parse the scoreboard for CyPat score bot JSON");
            Console.WriteLine();
            Console.WriteLine("Usage: ScoreboardToJSON.exe -f <file> [options]");
            Console.WriteLine("\t -h            show this menu");
            Console.WriteLine("\t -f <file>     file containing scoreboard format.");
            Console.WriteLine("\t               \tview example file on github repo...");
            Console.WriteLine("\t -u <url>      Use your own url of a scoreboard");
            Console.WriteLine("\t               Default: http://scoreboard.uscyberpatriot.org");
            Console.WriteLine("\t -o <file>     File to output data");
            Console.WriteLine("\t               Default: Scoreboard.json");
            Console.WriteLine("\t -t <file>     File containing custom team numbers to parse seperated by commas");
            Console.WriteLine("\t               NOTE: make sure the team numbers do not include the year and dash");
            Console.WriteLine("\n\n\n");
            Console.WriteLine("View the source code at http://github.com/LByrgeCP/ScoreboardToJSON");
        }
        public static void Show_Error(string error)
        {
            Console.WriteLine($"ERROR: {error}");
        }
        public static string[] ParseCustomTeams(string filename)
        {
            if(!File.Exists(filename))
            {
                Show_Error($"{filename} does not exist, parsing all teams");
                return null;
            }
            string filecontents = File.ReadAllText(filename);
            string[] teamsarr = filecontents.Split(',');
            foreach (string team in teamsarr)
            {
                if (!Regex.IsMatch(team, "^[0-9]{4}$"))
                {
                    Show_Error($"{team} is in an incorrect format, parsing all teams");
                    return null;
                }
            }
            return teamsarr;
        }
    }
}

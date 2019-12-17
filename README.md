# ScoreboardToJSON
Parses the CyberPatriot Scoreboard to produce JSON for the CyberScores discord bot
# How to
First off, if you are going to parse all the teams, you need a configuration file.
The configuration for this program is very basic.
Create a text file with 2 lines:
	First line- the format of the scoreboard
	Second line- the format of each teams panel
Look inside example.conf for more.

Next run the program, some examples are below:

Run the program with only the configuration and output it to Semis.txt:
`ScoreboardToJSON.exe -f example.conf -o Semis.txt`

Run the program to parse only specific teams:
`ScoreboardToJSON.exe -f example.conf -o Semis.json -t teams.csv`

This program is specifically for Glen3b's CyberScores bot to store backup round data.

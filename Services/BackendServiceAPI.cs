using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace backend.Services {

    public class MatchService { //make it public so that we can create an instance/call on this object from main
        //for now lets do only Eng teams, perhaps we could change this later
        private static readonly List<string> Teams = new()
        {
            "Manchester United", "Arsenal", "Liverpool", "Chelsea", "Manchester City", "Tottenham Hotspur",
            "Newcastle United", "West Ham United", "Brighton & Hove Albion", "Aston Villa", "Brentford", "Crystal Palace",
            "Everton", "Fulham", "Nottingham Forest", "Sheffield United", "Wolverhampton Wanderers", "Luton Town",
            "Burnley", "Bournemouth"
        };

        private static readonly Random Random = new();
        private static List<Match> Matches = new();
        private static DateTime lastUpdateTime = DateTime.Now;
        private readonly CancellationTokenSource _cancellationTokenSource = new(); //we need this because we will create a service and run it from a side thread, it would be blocking the main thread..

        public void StartServer() {
            Task.Run(() => RunServer(_cancellationTokenSource.Token));
        }

        private async Task RunServer(CancellationToken cancellationToken) {

            GenerateMatches(10); // lets gen 10
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:9999/");
            listener.Start();
            Console.WriteLine("yo! server running on http://localhost:9999/matches");

            while(!cancellationToken.IsCancellationRequested) {
                while (true) {
                    var context = await listener.GetContextAsync();
                    if (context.Request.Url?.AbsolutePath == "/matches") {
                        UpdateMatchScores(); // Update the scorese when requested
                        var response = GetMatchesJson();
                        byte[] buffer = Encoding.UTF8.GetBytes(response);
                        context.Response.ContentType = "application/json";
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Close();
                    }
                    else {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                    }
                }
            };
        }

        private static void GenerateMatches(int numMatches) {
            Matches.Clear();
            DateTime now = DateTime.Now;

            for (int i = 0; i < numMatches; i++) {
                string team1 = Teams[Random.Next(Teams.Count)];
                string team2;
                do { team2 = Teams[Random.Next(Teams.Count)]; } while (team1 == team2);

                DateTime matchDate = now.AddDays(Random.Next(-5, 5)); // Past & future matches
                string score = matchDate < now ? $"{Random.Next(5)}-{Random.Next(5)}" : "Upcoming";

                Matches.Add(new Match(i + 1, team1, team2, matchDate, score));
            }
        }

        private static void UpdateMatchScores() {
            DateTime now = DateTime.Now;
            if ((now - lastUpdateTime).TotalMinutes < 1) return; 
            foreach (var match in Matches) {
                if (match.Date < now && match.Score == "Upcoming")
                {
                    match.Score = $"{Random.Next(5)}-{Random.Next(5)}";
                }
            }

            lastUpdateTime = now;
        }

        private static string GetMatchesJson() {
            return JsonSerializer.Serialize(new { matches = Matches }, new JsonSerializerOptions { WriteIndented = true });
        }

        private class Match { //private is fine here, we will only use this internally
            public int MatchId { get; }
            public string Team1 { get; }
            public string Team2 { get; }
            public DateTime Date { get; }
            public string Score { get; set; }

            public Match(int matchId, string team1, string team2, DateTime date, string score)
            {
                MatchId = matchId;
                Team1 = team1;
                Team2 = team2;
                Date = date;
                Score = score;
            }
        }
    }
}

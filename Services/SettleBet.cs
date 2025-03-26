using FootballBettingApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FootballBettingApi.Services
{
    public class BettingSettlementService
    {
        private readonly ApplicationDbContext ctx;
        private readonly FootballApiService _footballApiService;

        public BettingSettlementService(ApplicationDbContext db, FootballApiService api)
        {
            ctx = db;
            _footballApiService = api;
        }

        public async Task SetleBetsCallBack()
        {
            var apidat = await _footballApiService.GetAPIData();
            var matches = apidat.RootElement.GetProperty("matches");

            var unsettledBets = await ctx.Bets
                .Where(b => b.WinnerTeam == null)
                .ToListAsync();

            foreach (var bet in unsettledBets)
            {
                var match = matches.EnumerateArray()
                    .FirstOrDefault(m => m.GetProperty("MatchId").GetInt32() == bet.MatchId);

                if (match.ValueKind == JsonValueKind.Undefined)
                    continue;

                var score = match.GetProperty("Score").GetString();
                if (score == "Upcoming")
                    continue;

                var t1 = match.GetProperty("Team1").GetString();
                var t2 = match.GetProperty("Team2").GetString();

                var scores = score.Split('-');
                if (scores.Length != 2) continue;

                int Firstscore = int.Parse(scores[0]);
                int Secondscore = int.Parse(scores[1]);

                string winner = "";
                if (Firstscore > Secondscore)
                {
                    winner = t1;
                }
                else if (Secondscore > Firstscore)
                {
                    winner = t2;
                }
                else
                {
                    winner = "Draw";
                }

                bet.WinnerTeam = winner;
                //okl ets debug if this is acutally scheduled as a callback and it actually runs..
                Console.WriteLine($"winner - {winner}");

                if (string.Equals(winner, bet.TeamChosen, StringComparison.OrdinalIgnoreCase))
                {
                    var user = await ctx.Users.FindAsync(bet.UserId);
                    user.Coins += bet.Amount * 2; // Win = double your bet
                    Console.WriteLine("User won and coins dubbled!!");
                }
            }

            await ctx.SaveChangesAsync();
        }
    }
}

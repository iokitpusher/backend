using FootballBettingApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FootballBettingApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using FootballBettingApi.Data;
using FootballBettingApi.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;    

    [Route("api/[controller]")]
    [ApiController]
    public class FootballController : ControllerBase
    {
        private readonly FootballApiService _footballApiService;
        private readonly ApplicationDbContext ctx;
        public FootballController(FootballApiService footballApiService,ApplicationDbContext dbContext)
        {
            _footballApiService = footballApiService;
            ctx = dbContext;
        }

        [HttpGet("yolo-test")]
        public async Task<IActionResult> yolo_test()
        {
            var result = await _footballApiService.GetAPIData();
            return Ok(result.RootElement);
        }

        [HttpGet("searchmatches")]
        public async Task<IActionResult> searchMatches([FromQuery] string team)
        {
            if (string.IsNullOrWhiteSpace(team))
                return BadRequest("Team name is required");

            var res = await _footballApiService.GetAPIData();
            var matches = res.RootElement.GetProperty("matches");

            List<Match> MatchesList = new List<Match>();

            foreach (var match in matches.EnumerateArray())
            {
                var t1 = match.GetProperty("Team1").GetString();
                var t2 = match.GetProperty("Team2").GetString();

                if (!string.IsNullOrEmpty(t1) && !string.IsNullOrEmpty(t2))
                {
                    if (t1.ToLower().Contains(team.ToLower()) || t2.ToLower().Contains(team.ToLower()))
                    {
                        MatchesList.Add(new Match(
                            match.GetProperty("MatchId").GetInt32(),
                            t1,
                            t2,
                            match.GetProperty("Date").GetDateTime(),
                            match.GetProperty("Score").GetString()
                        ));
                    }
                }
            }

        return Ok(MatchesList);
        }

        [Authorize]
        [HttpPost("placebet")]
        public async Task<IActionResult> bet([FromBody] UserBetResponse req)
        {
            var fullData = await _footballApiService.GetAPIData();
            var allMatches = fullData.RootElement.GetProperty("matches");

            var match = allMatches.EnumerateArray()
                .FirstOrDefault(mem => mem.GetProperty("MatchId").GetInt32() == req.MatchId);

            if (match.ValueKind == JsonValueKind.Undefined)
                return NotFound("?? match doesnt exists");

            var score = match.GetProperty("Score").GetString();
            if (score != "Upcoming")
            {
                return BadRequest("matchBetTooLate");
            } 
            var t1 = match.GetProperty("Team1").GetString();
            var t2 = match.GetProperty("Team2").GetString();

            if (req.TeamChosen.ToLower() != t1.ToLower() && req.TeamChosen.ToLower() != t2.ToLower())
            {
                return BadRequest("seleced team is not in the match!!");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await ctx.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            if (req.Amount <= 0)
                return BadRequest("you must bet more than 0");

            if (user.Coins < req.Amount)
                return BadRequest("Not enough coins");

            var betDTO = new Bet
            {
                MatchId = req.MatchId,
                TeamChosen = req.TeamChosen,
                Amount = req.Amount,
                UserId = userId
            };

            user.Coins -= req.Amount;
            ctx.Bets.Add(betDTO);
            await ctx.SaveChangesAsync();

            return Ok(new { message = "Bet placed successfully", coinsLetf = user.Coins });
        }


        [Authorize]
        [HttpGet("mybets")]
        public async Task<IActionResult> getBets()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bets = await ctx.Bets
                .Where(b => b.UserId == userId)
               .ToListAsync();

            var res = bets.Select(bet =>
            {
                string stat;

                if (bet.WinnerTeam == null)
                {
                    stat = "Pending";
                }
                else if (bet.TeamChosen == bet.WinnerTeam)
                {
                    stat = "Won";
                }
                else
                {
                    stat = "Lost";
                }
                return new UserBetResponse
                {
                    MatchId = bet.MatchId,
                    TeamChosen = bet.TeamChosen,
                    Amount = bet.Amount,
                    Status = stat,
                    WinnerTeam = bet.WinnerTeam
                };
            });

            return Ok(res);
        }

        [HttpGet("coinboard")]
        public async Task<IActionResult> GetCoinsForBoard()
        {
            var users = await ctx.Users.ToListAsync();
            List<int> coinsToStore = new List<int>();
            foreach(var user in users)
            {
                coinsToStore.Add(user.Coins);
            }

            //ok for now I'll just inline the bubble sort, perphaps later we will switch to a different algorithm, also I think we can just use .Sort()...
            for (int i = 0; i < coinsToStore.Count - 1; i++)
            {
                for (int j = 0; j < coinsToStore.Count - i - 1; j++)
                {
                    if (coinsToStore[j] > coinsToStore[j + 1])
                    {
                        int temp = coinsToStore[j];
                        coinsToStore[j] = coinsToStore[j + 1];
                        coinsToStore[j + 1] = temp;
                    }
                }
            }


            return Ok(coinsToStore);

        }
        
    }
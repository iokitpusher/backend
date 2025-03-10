using FootballBettingApi.Data;
using FootballBettingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace FootballBettingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/bets/place
        
        [HttpPost("place")]
        public async Task<IActionResult> PlaceBet([FromBody] Bet bet)
        {
          
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            if (bet.UserId != userId)
            {
                return Forbid("You can only place bets for yourself.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == bet.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Coins < bet.BetAmount)
            {
                return BadRequest("Not enough coins.");
            }


            user.Coins -= bet.BetAmount;
            _context.Bets.Add(bet);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Bet placed successfully.");
        }
        
        // POST: api/bets/settle
        
        [HttpPost("settle")]
        public async Task<IActionResult> SettleBets([FromBody] SettleRequest request)
        {
            
            var bets = await _context.Bets
                .Where(b => b.MatchId == request.MatchId && !b.IsSettled)
                .ToListAsync();
                
            if (!bets.Any())
            {
                return NotFound("No unsettled bets for this match.");
            }
            
            // Process each bet.
            foreach (var bet in bets)
            {
                if (bet.Prediction.Equals(request.ActualResult, System.StringComparison.OrdinalIgnoreCase))
                {
                    bet.Result = "win";
                  
                    var user = await _context.Users.FindAsync(bet.UserId);
                    if (user != null)
                    {
                        user.Coins += bet.BetAmount * 2;
                        _context.Users.Update(user);
                    }
                }
                else
                {
                    bet.Result = "loss";
                }
                bet.IsSettled = true;
                _context.Bets.Update(bet);
            }
            
            await _context.SaveChangesAsync();
            return Ok("Bets settled successfully.");
        }
    }


    public class SettleRequest
    {
        public string MatchId { get; set; }
        // Expected: "home", "draw", or "away".
        public string ActualResult { get; set; }
    }
}

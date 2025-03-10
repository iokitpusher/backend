using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBettingApi.Models
{
    public class Bet
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        // The external match ID from the football API.
        public string MatchId { get; set; }
        
        public int BetAmount { get; set; }
        
        // For example: "home", "draw", or "away"
        public string Prediction { get; set; }
        
        public DateTime BetTime { get; set; } = DateTime.UtcNow;
        
        // Settlement fields:
        public bool IsSettled { get; set; } = false;
        // "win", "loss", or null if not yet settled.
        public string Result { get; set; }
        
        public User User { get; set; }
    }
}

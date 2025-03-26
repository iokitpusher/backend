using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBettingApi.Models
{
    public class Bet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MatchId { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public string TeamChosen { get; set; }

        public DateTime DatePlaced { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string? WinnerTeam { get; set; } //make it NULLable ?
    }
}

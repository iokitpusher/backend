using System;
using System.ComponentModel.DataAnnotations;

namespace FootballBettingApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; } 
        
        public int Coins { get; set; } = 0;
        
        
        public DateTime LastCoinReset { get; set; } = DateTime.UtcNow;
    }
}

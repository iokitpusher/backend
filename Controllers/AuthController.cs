using FootballBettingApi.Data;
using FootballBettingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FootballBettingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
           
            if (await _db.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

           
            user.Coins = 10; // nake the new users get 10 coins.
            user.LastCoinReset = DateTime.UtcNow.Date;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null || user.Password != login.Password)
            {
                return Unauthorized("Invalid username or password");
            }

           
            if (user.LastCoinReset.Date < DateTime.UtcNow.Date)
            {
                user.Coins += 10;
                user.LastCoinReset = DateTime.UtcNow.Date;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
            }

           
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString, userId = user.Id, coins = user.Coins });
        }
    }
}

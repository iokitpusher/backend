using FootballBettingApi.Data;
using FootballBettingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext ctx;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext db, IConfiguration config)
        {
            ctx = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
           
            if (await ctx.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

           
            user.Coins = 10;
            user.LastCoinReset = DateTime.UtcNow.Date;
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            
            var user = await ctx.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null || user.Password != login.Password)
            {
                return Unauthorized("Invalid username or password");
            }

           
            if (user.LastCoinReset.Date < DateTime.UtcNow.Date)
            {
                user.Coins += 5;
                user.LastCoinReset = DateTime.UtcNow.Date;
                ctx.Users.Update(user);
                await ctx.SaveChangesAsync();
            }

           
            var jwt_handler = new JwtSecurityTokenHandler();
            var secretKey = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwt_handler.CreateToken(tokenDesc);
            var tokenStr = jwt_handler.WriteToken(token);

            return Ok(new { token = tokenStr, userId = user.Id, coins = user.Coins });
        }
    }

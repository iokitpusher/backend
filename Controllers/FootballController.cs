using FootballBettingApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FootballBettingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FootballController : ControllerBase
    {
        private readonly FootballApiService _footballApiService;
        public FootballController(FootballApiService footballApiService)
        {
            _footballApiService = footballApiService;
        }

        // GET: api/football/search-team?name=Arsenal
        [HttpGet("search-team")]
        public async Task<IActionResult> SearchTeam([FromQuery] string name)
        {
            var result = await _footballApiService.SearchTeamByName(name);
            return Ok(result.RootElement);
        }

        // GET: api/football/upcoming-matches?leagueId=4328&season=2020-2021
        [HttpGet("upcoming-matches")]
        public async Task<IActionResult> GetUpcomingMatches([FromQuery] string leagueId, [FromQuery] string season)
        {
            
            var upcomingEvents = new List<JsonElement>();
            return Ok(upcomingEvents);
        }

        // GET: api/football/previous-scores?teamId=133602
        [HttpGet("previous-scores")]
        public async Task<IActionResult> GetPreviousScores([FromQuery] string teamId)
        {
            var doc = await _footballApiService.GetLastEventsByTeam(teamId);
            return Ok(doc.RootElement);
        }
    }
}

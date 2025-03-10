using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace FootballBettingApi.Services
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
       
        private const string BaseUrl = "https://www.thesportsdb.com";

        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

       
        public async Task<JsonDocument> SearchTeamByName(string teamName)
        {
            var url = $"{BaseUrl}/api/v1/json/3/searchteams.php?t={teamName}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(stream);
        }

       
        public async Task<JsonDocument> GetEventsSeason(string leagueId, string season)
        {
            var url = $"{BaseUrl}/api/v1/json/3/eventsseason.php?id={leagueId}&s={season}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(stream);
        }

        
        public async Task<JsonDocument> GetLastEventsByTeam(string teamId)
        {
            var url = $"{BaseUrl}/api/v1/json/3/eventslast.php?id={teamId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(stream);
        }
    }
}

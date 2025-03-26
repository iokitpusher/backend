using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;


    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
       
        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<JsonDocument> GetAPIData() {
            var res = await _httpClient.GetAsync("http://localhost:9999/matches");
            res.EnsureSuccessStatusCode();
            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(stream);
        }
    }


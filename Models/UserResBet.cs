namespace FootballBettingApi.DTOs
{
    public class UserBetResponse
    {
        public int MatchId { get; set; }
        public string TeamChosen { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string? WinnerTeam { get; set; }
    }
}
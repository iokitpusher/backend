    public class Match 
    {
        public int MatchId { get; }
        public string Team1 { get; }
        public string Team2 { get; }
        public DateTime Date { get; }
        public string Score { get; set; }

        public Match(int matchId, string team1, string team2, DateTime date, string score)
        {
            MatchId = matchId;
            Team1 = team1;
            Team2 = team2;
            Date = date;
            Score = score;
            }
    }
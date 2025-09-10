namespace BoxrecScraper.Models
{
    // Model for API
    public class Team
    {
        public string? competitorId { get; set; } = "";
        public string? title { get; set; } = "";
    }

    public class Match
    {
        public string? eventId { get; set; } = "";
        public string? sportType { get; set; } = "";
        public string? startTime { get; set; } = "";
        public string? status { get; set; } = "";
        public bool? isAmericanSport { get; set; }
        public Team? homeTeam { get; set; }
        public Team? awayTeam { get; set; }
    }

    public class ApiResponse
    {
        public List<Match>? matches { get; set; }
        public List<object>? _links { get; set; }  // keep as object if we don't need it
    }


    // Model for Event
    class Event
    {
        public string location { get; set; } = "";
        public string date { get; set; } = "";
    }

    // Model for Fighter
    class Fighter
    {
        public string bouts { get; set; } = "";
        public string rounds { get; set; } = "";
    }

    // Model for PushService API
    public class Result
    {
        public string? winner { get; set; } = "";
        public string? method { get; set; } = "";
        public string? round_finish { get; set; } = "";
        public string? round_time { get; set; } = "";
        public string? round_scheduled { get; set; } = "";
    }

    public class Status
    {
        public Team? fighter_1 { get; set; }
        public Team? fighter_2 { get; set; }
        public Result? result { get; set; }
    }
    public class PSMatch
    {
        public string? palmerbet_id { get; set; } = "";
        public string? boxrec_id { get; set; } = "";
        public string? division { get; set; } = "";
        public string? location { get; set; } = "";
        public string? date { get; set; } = "";
        public Status? status { get; set; }
    }


    // Model for Login credentials
    class LoginCredentials
    {
        public string email { get; set; } = "";
        public string password { get; set; } = "";
    }
}

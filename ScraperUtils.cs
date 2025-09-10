using System;
using System.Threading.Tasks;
using BoxrecScraper.Models;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Playwright;

namespace BoxrecScraper.Utils
{
    public static class ScraperUtils
    {
        public static List<string> DestructData(List<Match> apiData)
        {
            var playerNames = apiData.Select(u => u.homeTeam.title + " - " + u.awayTeam.title).ToList();
            return playerNames;
        }

        private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateRandomString(int length = 8) // default 8 chars
        {
            if (length < 6) length = 6; // enforce minimum length of 6

            var result = new StringBuilder(length);
            byte[] buffer = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }

            for (int i = 0; i < length; i++)
            {
                int idx = buffer[i] % chars.Length;
                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static List<Match> FilterRecentMatches(List<Match> matches)
        {
            var filteredMatches = matches
            .Where(m =>
            {
                if (DateTime.TryParse(m.startTime, out var date))
                {
                    return date.Date <= DateTime.UtcNow.Date; // keep today + past
                }
                return false; // skip if invalid date
            })
            .ToList();
            return filteredMatches;
        }

        public static Proxy GetProxy()
        {
            var proxy = new Proxy  // dataimpulse mobile proxy
            {
                Server = Environment.GetEnvironmentVariable("PROXY_SERVER"),
                Username = Environment.GetEnvironmentVariable("PROXY_USERNAME"),
                Password = Environment.GetEnvironmentVariable("PROXY_PASSWORD")
            };


            return proxy;
        }

    }

}

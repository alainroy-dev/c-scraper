using System;
using System.IO;
using System.Text.Json;
using BoxrecScraper.Models;

namespace BoxrecScraper.Utils
{
    public class JsonStorage<T>
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;
        
        public JsonStorage(string filePath)
        {
            _filePath = filePath;
            _options = new JsonSerializerOptions
            {
                WriteIndented = true // pretty print JSON
            };
        }

        // Save data to JSON file
        public async Task SaveAsync(T data)
        {
            string json = JsonSerializer.Serialize(data, _options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        // Save data to JSON file
        public async Task SaveAsyncWithOld(T data)
        {
            List<Match>? existingData = new();

            if (data is List<Match> matches)
            {
                existingData = await LoadAsync() as List<Match>;
                foreach (var match in matches)
                {
                    if (existingData != null && !existingData.Any(m => m.eventId == match.eventId))
                    {
                        existingData.Add(match);
                    }
                }
            }

            string json = JsonSerializer.Serialize(existingData, _options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task SaveAsyncDone(T data)
        {
            List<Match>? existingData = [];

            if (data is List<Match> matchData)
            {
                existingData = await LoadAsync() as List<Match> ?? new List<Match>();
                if (existingData != null)
                {
                    if (existingData.Count == 0 || !existingData.Any(m => m.eventId == matchData[0].eventId))
                    {
                        existingData.Add(matchData[0]);
                    }
                    
                }
                else
                {
                    existingData = new List<Match> { matchData[0] };
                }
            }
            
            string json = JsonSerializer.Serialize(existingData, _options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        // find matches not populated in fixturesDone.json
        public async Task<List<Match>>? filterSync(T data)
        {
            List<Match>? filterData = new();

            if (data is List<Match> matches)
            {
                List<Match>? existingData = await LoadAsync() as List<Match>;
                foreach (var match in matches)
                {
                    if (existingData!= null && !existingData.Any(m => m.eventId == match.eventId))
                    {
                        filterData.Add(match);
                    }
                }
            }
            return filterData;
        }

        // Load data from JSON file
        public async Task<T?> LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return default;
            }

            string json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return Activator.CreateInstance<T>()!;
            }
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        // internal async Task saveToFileAsync(string v, List<Match> matches)
        // {
        //     throw new NotImplementedException();
        // }
    }

}

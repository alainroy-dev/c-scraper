using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BoxrecScraper.Utils
{
    public class APIClient
    {
        private readonly HttpClient _httpClient;

        public APIClient(string? baseUrl = null)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        // Generic GET request
        public async Task<string> GetAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Generic POST request (JSON body)
        public async Task<string> PostAsync(string endpoint, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Generic PUT request
        public async Task<string> PutAsync(string endpoint, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Generic DELETE request
        public async Task<string> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
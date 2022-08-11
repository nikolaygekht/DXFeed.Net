using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace DXFeed.Demo
{
    internal class GehtsoftTokenFactory : ITokenFactory
    {
        private sealed class DxFeedToken
        {
            [JsonPropertyName("tkn")]
            public string? Token { get; set; }
            [JsonPropertyName("url")]
            public string? Url { get; set; }
        }

        public string? GetToken(string url, string name) => GetTokenAsync(url, name).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<string?> GetTokenAsync(string url, string name)
        {
            var client = new HttpClient();
            var responseText = await client.GetStringAsync($"{url}{HttpUtility.UrlEncode(name)}");
            var response = JsonSerializer.Deserialize<DxFeedToken>(responseText);
            return response?.Token;
        }
    }
}

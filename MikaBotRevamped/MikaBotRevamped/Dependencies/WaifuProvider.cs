using MikaBotRevamped.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MikaBotRevamped.Dependencies
{
    internal class WaifuProvider : IWaifuProvider
    {
        private readonly HttpClient httpClient1;
        private readonly HttpClient httpClient2;

        public WaifuProvider(string baseUrl1, string baseUrl2)
        {
            httpClient1 = new HttpClient
            {
                BaseAddress = new Uri(baseUrl1)
            };
            httpClient1.DefaultRequestHeaders.Accept.Clear();
            httpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpClient2 = new HttpClient
            {
                BaseAddress = new Uri(baseUrl2)
            };
            httpClient2.DefaultRequestHeaders.Accept.Clear();
            httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetRandomWaifuImageUrl()
        {
            Random r = new Random();
            if (r.Next(2) == 1)
            {
                HttpResponseMessage response2 = await httpClient2.GetAsync("https://api.waifu.pics/sfw/waifu");
                response2.EnsureSuccessStatusCode();
                string responseContent2 = await response2.Content.ReadAsStringAsync();
                string url2 = JsonSerializer.Deserialize<JsonElement>(responseContent2).GetProperty("url").GetString();
                return url2;
            } else
            {
                HttpResponseMessage response = await httpClient1.GetAsync("https://api.waifu.im/search");
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();

                JsonDocument document = JsonDocument.Parse(responseContent);
                string url = document.RootElement
                    .GetProperty("images")[0]
                    .GetProperty("url").GetString();

                return url;
            }
        }

        public async Task<string> GetWaifuImageUrlByTag(string tag)
        {
            HttpResponseMessage response = await httpClient1.GetAsync("https://api.waifu.pics/sfw/" + tag);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            string url = JsonSerializer.Deserialize<JsonElement>(responseContent).GetProperty("url").GetString();

            return url;
        }
    }
}

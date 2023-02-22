using Grupp15.Shared.Models;
using System.Net.Http.Json;

namespace Grupp15.Client.Services
{
    public class NewsManager : INewsManager
    {
        private readonly HttpClient _httpClient;
        public NewsManager(HttpClient httpclient)
        {
            _httpClient = httpclient;
        }
        public async Task PostNews(string news)
        {
            await _httpClient.PostAsJsonAsync("api/news/news", news);
        }
        public async Task<List<NewsModel>?> GetNews()
        {
            return await _httpClient.GetFromJsonAsync<List<NewsModel>?>("api/news/getnews");
        }
        public async Task DeleteNews(int id)
        {
            await _httpClient.DeleteAsync($"api/news/NewsDelete/{id}");
        }
        public async Task UpdateNews(string news, int id)
        {
            await _httpClient.PutAsJsonAsync($"api/news/editnews/{id}", news);
        }
        public async Task<NewsModel?> GetSpecificNews(int id)
        {
            return await _httpClient.GetFromJsonAsync<NewsModel?>($"api/news/getnews/{id}");
        }
    }

    public interface INewsManager
    {
        Task PostNews(string news);
        Task<List<NewsModel>?> GetNews();
        Task DeleteNews(int id);
        Task UpdateNews(string news, int id);
        Task<NewsModel?> GetSpecificNews(int id);
    }
}

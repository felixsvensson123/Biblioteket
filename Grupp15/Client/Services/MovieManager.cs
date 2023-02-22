using System.Net.Http.Json;
using System.Net.Http.Headers;
using Grupp15.Shared.Models;


namespace Grupp15.Client.Services
{
    public class MovieManager : IMovieManager
    {
        private readonly HttpClient _httpClient;

        public MovieManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MovieModel> GetMovie(int id)
        {
            return await _httpClient.GetFromJsonAsync<MovieModel>($"api/movies/{id}");
        }

        public async Task BulkAdd(Stream fileStream, string fileName)
        {
            var content = new MultipartFormDataContent();

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            //reset stream position as it is incorrect
            fileStream.Position = 0;

            content.Add(new StreamContent(fileStream, (int)fileStream.Length), "csv", fileName);

            await _httpClient.PostAsync("/api/movies/bulkadd", content);
        }

        public async Task EditMovie(MovieModel movieModel, int id)
        {
            await _httpClient.PutAsJsonAsync($"api/movies/Edit/{id}", movieModel);
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/movies/{id}");

            if (result.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }

    public interface IMovieManager
    {
        Task<MovieModel> GetMovie(int id);
        Task BulkAdd(Stream fileStream, string fileName);
        Task EditMovie(MovieModel movieModel, int id);
        Task<bool> DeleteMovie(int id);
    }
}
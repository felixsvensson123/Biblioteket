using System.Net.Http.Json;
using System.Net.Http.Headers;
using Grupp15.Shared.Models;

namespace Grupp15.Client.Services
{
    public class EBookManager : IEBookManager
    {
        private readonly HttpClient _httpClient;

        public EBookManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EBookModel?> GetEBook(int id)
        {
            return await _httpClient.GetFromJsonAsync<EBookModel>($"api/Ebooks/{id}");
        }

        public async Task BulkAdd(Stream fileStream, string fileName)
        {
            var content = new MultipartFormDataContent();

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            //reset stream position as it is incorrect
            fileStream.Position = 0;

            content.Add(new StreamContent(fileStream, (int)fileStream.Length), "csv", fileName);

            await _httpClient.PostAsync("/api/ebooks/bulkadd", content);
        }

        public async Task UpdateEBook(EBookModel newEbook, int id)
        {
            await _httpClient.PutAsJsonAsync($"api/ebooks/edit/{id}", newEbook);
        }

        public async Task DeleteEbook(int id)
        {
            await _httpClient.DeleteAsync($"api/ebooks/delete");
        }
    }

    public interface IEBookManager
    {
        Task<EBookModel?> GetEBook(int id);
        Task BulkAdd(Stream fileStream, string fileName);
        Task UpdateEBook(EBookModel newEbook, int id);
        Task DeleteEbook(int id);
    }
}

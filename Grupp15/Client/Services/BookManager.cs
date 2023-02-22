using System.Net.Http.Json;
using System.Net.Http.Headers;
using Grupp15.Shared.Models;

namespace Grupp15.Client.Services
{


    public class BookManager : IBookManager
    {
        private readonly HttpClient _httpClient;

        public BookManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BookModel>> GetBooks()
        {
            return await _httpClient.GetFromJsonAsync<List<BookModel>>($"api/books");
        }

        public async Task<BookModel> GetBook(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookModel>($"api/books/{id}");
        }
        public async Task BulkAdd(Stream fileStream, string fileName)
        {
            var content = new MultipartFormDataContent();

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            //reset stream position as it is incorrect
            fileStream.Position = 0;

            content.Add(new StreamContent(fileStream, (int)fileStream.Length), "csv", fileName);

            await _httpClient.PostAsync("/api/books/bulkadd", content);
        }

        public async Task<BookModel> AddNewBook(BookModel bookModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/books/", bookModel);
            if (result.IsSuccessStatusCode)
            {
                return bookModel;
            }
            else
            {
                return null!;
            }
        }

        public async Task EditBook(BookModel newbook, int id)
        {
            await _httpClient.PutAsJsonAsync($"api/books/edit/{id}", newbook);
        }

        public async Task<bool> DeleteBook(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/books/{id}");

            if (result.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }

    public interface IBookManager
    {
        Task<BookModel> AddNewBook(BookModel bookModel);
        Task<BookModel> GetBook(int id);
        Task BulkAdd(Stream fileStream, string fileName);
        Task<List<BookModel>> GetBooks();
        Task EditBook(BookModel newbook, int id);
        Task<bool> DeleteBook(int id);
    }
}
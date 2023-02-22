using Grupp15.Shared.Models;
using System.Net.Http.Json;

namespace Grupp15.Client.Services
{
    public class BorrowingManager : IBorrowingManager
    {
        private readonly HttpClient _httpClient;

        public BorrowingManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Loan(int id)
        {
            var result = await _httpClient.PutAsJsonAsync("api/borrowing/loan", id);

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<List<BorrowingModel>> GetLoaned(string id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<BorrowingModel>>($"api/borrowing/loaned/{id}");

            return result ?? new List<BorrowingModel>();
        }

        public async Task<List<BorrowingModel>> GetLoaned()
        {
            var result = await _httpClient.GetFromJsonAsync<List<BorrowingModel>>("api/borrowing/loaned");

            return result ?? new List<BorrowingModel>();
        }

        public async Task Return(int id)
        {
            await _httpClient.PutAsJsonAsync($"api/borrowing/return", id);
        }
    }

    public interface IBorrowingManager
    {
        Task<bool> Loan(int id);
        Task<List<BorrowingModel>> GetLoaned(string id);
        Task<List<BorrowingModel>> GetLoaned();
        Task Return(int id);
    }
}

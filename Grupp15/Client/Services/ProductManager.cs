using Grupp15.Shared.Models;
using System.Net.Http.Json;

namespace Grupp15.Client.Services
{
    public class ProductManager : IProductManager
    {
        private readonly HttpClient _httpClient;
        public ProductManager(HttpClient httpclient)
        {
            _httpClient = httpclient;
        }
        public async Task<List<ProductBase>?> LatestsProducts()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductBase>?>("api/product/prods");
        }
        public async Task<List<ProductBase>?> GetAllProducts()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductBase>?>("api/product/AllProds");
        } 
        public async Task<List<BorrowingModel>?> GetAllLoaned()
        {
            return await _httpClient.GetFromJsonAsync<List<BorrowingModel>?>("api/product/GetLoaned");
        }
        public async Task<ProductBase?> GetProduct(int id)
        {
            return await _httpClient.GetFromJsonAsync<ProductBase>($"api/product/GetProduct/{id}");
        }

        public async Task<List<ProductBase>> SearchProduct(string searchText)
        {
            return await _httpClient.GetFromJsonAsync<List<ProductBase>>($"api/product/search/{searchText}");
        }
    }

    public interface IProductManager
    {
        Task<List<ProductBase>?> LatestsProducts();
        Task<List<ProductBase>?> GetAllProducts();
        Task<List<BorrowingModel>?> GetAllLoaned();
        Task<ProductBase?> GetProduct(int id);
        Task<List<ProductBase>> SearchProduct(string searchText);
    }
}

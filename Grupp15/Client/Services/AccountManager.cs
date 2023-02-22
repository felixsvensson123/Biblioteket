using Blazored.LocalStorage;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace Grupp15.Client.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authProvider;

        public AccountManager(HttpClient httpclient, ILocalStorageService localStorage, NavigationManager navigationManager, AuthenticationStateProvider authProvider)
        {
            _httpClient = httpclient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authProvider = authProvider;
        }
        public async Task<string> CreateUser(RegisterModel user)
        {
           var result = await _httpClient.PostAsJsonAsync<RegisterModel>("api/users/Register", user);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }
        public async Task<string> LoginUser(LoginModel user)
        {
            var result = await _httpClient.PostAsJsonAsync<LoginModel>("api/users/login", user);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            return null;
        }
        public async Task DeleteUser()
        {
            var result = await _httpClient.DeleteAsync("api/users/delete");

            if (result.IsSuccessStatusCode)
            {
                await _localStorage.RemoveItemAsync("token");
                await _authProvider.GetAuthenticationStateAsync();
                _navigationManager.NavigateTo("/");
            }
        }
        public async Task<bool> CheckUserRole(string id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/users/getuserrole/{id}");
        }
        
        public async Task<List<ApplicationUser>?> GetAllUsers()
        {
            return await _httpClient.GetFromJsonAsync<List<ApplicationUser>>("api/users/getusers");
        }
        public async Task<ApplicationUser?> GetCurrentUser()
        {
            return await _httpClient.GetFromJsonAsync<ApplicationUser>("api/users/getcurrentuser");
        }
        public async Task<ApplicationUser?> GetUser(string Id)
        {
            return await _httpClient.GetFromJsonAsync<ApplicationUser>($"api/users/finduser/{Id}");
        }
        public async Task HireLibrarian(string id, string user)
        {
            await _httpClient.PutAsJsonAsync($"api/users/HireLibrarian/{id}", user);
        }
        public async Task FireLibrarian(string id, string user)
        {
            await _httpClient.PutAsJsonAsync($"api/users/FireLibrarian/{id}", user);
        }
    }

    public interface IAccountManager
    {
        Task<string> CreateUser(RegisterModel user);
        Task<string> LoginUser(LoginModel user);
        Task DeleteUser();
        Task<bool> CheckUserRole(string id);
        Task<List<ApplicationUser>?> GetAllUsers();
        Task<ApplicationUser?> GetCurrentUser();
        Task<ApplicationUser?> GetUser(string Id);
        Task HireLibrarian(string id, string user);
        Task FireLibrarian(string id, string user);
    }
}

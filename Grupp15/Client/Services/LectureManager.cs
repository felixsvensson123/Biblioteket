using Grupp15.Shared.Models;
using System.Net.Http.Json;

namespace Grupp15.Client.Services
{
    public class LectureManager : ILectureManager
    {
        private readonly HttpClient _httpClient;

        public LectureManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LectureModel>?> GetAll()
        {
            return await _httpClient.GetFromJsonAsync<List<LectureModel>>("api/lecture");
        }

        public async Task<LectureModel?> GetById(int id)
        {
            return await _httpClient.GetFromJsonAsync<LectureModel>($"api/lecture/{id}");
        }

        public async Task<List<AttendModel>> GetAttending()
        {
            var result = await _httpClient.GetFromJsonAsync<List<AttendModel>>("api/lecture/attending");

            return result ?? new List<AttendModel>(); //if null return empty list
        }

        public async Task<List<AttendModel>> GetAttending(string id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<AttendModel>>($"api/lecture/attending/user/{id}");

            return result ?? new List<AttendModel>();
        }

        public async Task<List<AttendModel>> GetAttending(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<List<AttendModel>>($"api/lecture/attending/lecture/{id}");

            return result ?? new List<AttendModel>();
        }

        public async Task<bool> SignUp(int id)
        {
            var result = await _httpClient.PostAsJsonAsync("api/lecture/signup", id);

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> Unregister(int id)
        {
            var result = await _httpClient.PostAsJsonAsync("api/lecture/unregister", id);

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }
        public async Task<bool> AddLecture(LectureModel model)
        {
            var result = await _httpClient.PostAsJsonAsync("api/lecture/add", model);

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> DeleteLecture(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/lecture/{id}");

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> RemoveStudent(int lectureId, string studentId)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/lecture/RemoveStudent/{lectureId}", studentId);

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> EditLecture(LectureModel model, int id)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/lecture/{id}", model);

            if (result.IsSuccessStatusCode)
                return true;

            return false;
        }
    }

    public interface ILectureManager
    {
        Task<List<LectureModel>?> GetAll();
        Task<LectureModel?> GetById(int id);
        Task<bool> SignUp(int id);
        Task<bool> Unregister(int id);
        Task<bool> AddLecture(LectureModel model);
        Task<bool> DeleteLecture(int id);
        Task<bool> EditLecture(LectureModel model, int id);
        Task<List<AttendModel>> GetAttending();
        Task<List<AttendModel>> GetAttending(string id);
        Task<List<AttendModel>> GetAttending(int id);
        Task<bool> RemoveStudent(int lectureId, string studentId);
    }
}

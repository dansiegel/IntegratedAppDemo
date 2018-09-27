using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IntegratedTodoClient.Models;
using IntegratedTodoClient.Services;
using Newtonsoft.Json;

namespace IntegratedTodoClient.Todo.Services
{
    public class ApiClient : IApiClient
    {
        private IAuthStore _authStore { get; }
        private IApiSettings _apiSettings { get; }

        public ApiClient(IAuthStore authStore, IApiSettings apiSettings)
        {
            _authStore = authStore;
            _apiSettings = apiSettings;
        }

        public async Task<UserProfile> GetUserProfileAsync()
        {
            using (var client = await CreateClientAsync())
            {
                var response = await client.GetAsync("api/userprofiles/me");
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserProfile>(jsonContent);
            }
        }

        public async Task SaveTodoItemAsync(TodoItem item)
        {
            using (var client = await CreateClientAsync())
            {
                var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                await client.PostAsync("api/todoitems", content);
            }
        }

        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync()
        {
            using (var client = await CreateClientAsync())
            {
                var response = await client.GetAsync("api/todoitems/list");
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<TodoItem>>(jsonContent);
            }
        }

        private async Task<HttpClient> CreateClientAsync()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_apiSettings.ApiEndpoint)
            };
            var token = await _authStore.GetTokenAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BEARER", token);
            client.DefaultRequestHeaders.Add("X-Platform", Xamarin.Forms.Device.RuntimePlatform);
            client.DefaultRequestHeaders.Add("X-DeviceId", _apiSettings.InstallId);

            return client;
        }
    }
}

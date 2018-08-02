using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using IntegratedTodoClient.Helpers;
using IntegratedTodoClient.Models;

namespace IntegratedTodoClient.Services
{
    public class ApiClient : IApiClient
    {
        private IPublicClientApplication _pca { get; }

        public ApiClient(IPublicClientApplication publicClientApplication)
        {
            _pca = publicClientApplication;
        }

        public async Task<UserProfile> GetUserProfileAsync()
        {
            using(var client = await CreateClientAsync())
            {
                var response = await client.GetAsync("api/userprofiles/me");
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserProfile>(jsonContent);
            }
        }

        public async Task SaveTodoItemAsync(TodoItem item)
        {
            using(var client = await CreateClientAsync())
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

        public async Task<bool> LoginAsync()
        {
            var result = await SilentLoginAsync();
            if(result == null)
            {
                result = await _pca.AcquireTokenAsync(App.Scopes, GetUserByPolicy(_pca.Users, App.PolicySignUpSignIn), null);
            }

            return result != null;
        }

        private async Task<AuthenticationResult> SilentLoginAsync()
        {
            return await _pca.AcquireTokenSilentAsync(App.Scopes, GetUserByPolicy(_pca.Users, App.PolicySignUpSignIn), App.Authority, false);
        }

        private async Task<HttpClient> CreateClientAsync()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(Secrets.AppServiceEndpoint)
            };
            var authResult = await SilentLoginAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BEARER", authResult.AccessToken);
            client.DefaultRequestHeaders.Add("X-Platform", Xamarin.Forms.Device.RuntimePlatform);
            client.DefaultRequestHeaders.Add("X-DeviceId", (await AppCenter.GetInstallIdAsync())?.ToString());

            return client;
        }

        private IUser GetUserByPolicy(IEnumerable<IUser> users, string policy)
        {
            foreach (var user in users)
            {
                string userIdentifier = Base64UrlDecode(user.Identifier.Split('.')[0]);
                if (userIdentifier.EndsWith(policy.ToLower(), StringComparison.InvariantCultureIgnoreCase)) return user;
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

    }
}

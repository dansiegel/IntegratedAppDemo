using System.Linq;
using System.Threading.Tasks;
using IntegratedTodoClient.Services;
using Microsoft.Identity.Client;

namespace IntegratedTodoClient.Identity.Services
{
    public class AuthClient : IAuthClient, IAuthStore
    {
        private IPublicClientApplication _pca { get; }
        private UIParent _uiParent { get; }
        private IAuthOptions _options { get; }

        public AuthClient(IPublicClientApplication pca, UIParent uiParent, IAuthOptions options)
        {
            _pca = pca;
            _uiParent = uiParent;
            _options = options;
        }

        public async Task<bool> LoginAsync()
        {
            var result = await SilentLoginAsync();
            if (result == null)
            {
                var accounts = await _pca.GetAccountsAsync();
                await _pca.AcquireTokenAsync(_options.Scopes, accounts.FirstOrDefault(), _uiParent);
            }

            return result != null;
        }

        private async Task<AuthenticationResult> SilentLoginAsync()
        {
            var accounts = await _pca.GetAccountsAsync();
            return await _pca.AcquireTokenSilentAsync(_options.Scopes, accounts.FirstOrDefault(), $"{_options.AuthorityBase}{_options.PolicySignUpSignIn}", false);
        }

        public async Task Logout()
        {
            var accounts = await _pca.GetAccountsAsync();
            if(accounts != null && accounts.Any())
            {
                foreach(var account in accounts)
                {
                    await _pca.RemoveAsync(account);
                }
            }
        }

        public async Task<string> GetTokenAsync()
        {
            var result = await SilentLoginAsync();
            return result?.AccessToken;
        }
    }
}

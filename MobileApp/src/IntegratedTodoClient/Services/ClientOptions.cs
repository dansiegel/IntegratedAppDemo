using IntegratedTodoClient.Helpers;
using IntegratedTodoClient.Identity.Services;
using IntegratedTodoClient.Todo.Services;
using Microsoft.AppCenter;

namespace IntegratedTodoClient.Services
{
    public class ClientOptions : IAuthOptions, IApiSettings
    {
        public ClientOptions()
        {
            InstallId = AppCenter.GetInstallIdAsync().GetAwaiter().GetResult()?.ToString();
        }

        public string PolicySignUpSignIn => AppConstants.PolicySignUpSignIn;
        public string PolicyEditProfile => AppConstants.PolicyEditProfile;
        public string PolicyResetPassword => AppConstants.PolicyResetPassword;
        public string[] Scopes => AppConstants.Scopes;
        public string AuthorityBase => AppConstants.AuthorityBase;

        public string ApiEndpoint => Secrets.AppServiceEndpoint;
        public string InstallId { get; }
    }
}

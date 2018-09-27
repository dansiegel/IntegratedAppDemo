using System.Threading.Tasks;
using IntegratedTodoClient.Todo.Services;
using Microsoft.AppCenter;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;

namespace IntegratedTodoClient.Todo.ViewModels
{
    public class UserFooterViewModel : TodoViewModelBase
    {
        public UserFooterViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ILogger logger, IApiClient apiClient) : base(navigationService, pageDialogService, logger, apiClient)
        {
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string UUID { get; set; }

        private async Task CheckAppCenterValuesAsync()
        {
            var id = await AppCenter.GetInstallIdAsync();

            if (id.HasValue)
            {
                UUID = id.ToString();
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await CheckAppCenterValuesAsync();
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                var user = await _apiClient.GetUserProfileAsync();
                Name = $"{user.FirstName} {user.LastName}";
                Email = user.Email;
            }
        }
    }
}

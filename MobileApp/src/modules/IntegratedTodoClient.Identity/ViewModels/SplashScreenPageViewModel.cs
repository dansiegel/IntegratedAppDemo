using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Navigation;
using Prism.Services;
using IntegratedTodoClient.Services;
using Prism.Logging;
using IntegratedTodoClient.ViewModels;

namespace IntegratedTodoClient.Identity.ViewModels
{
    public class SplashScreenPageViewModel : ViewModelBase
    {
        public SplashScreenPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ILogger logger, IApiClient apiClient)
            : base(navigationService, pageDialogService, logger, apiClient)
        {
        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            // TODO: Implement any initialization logic you need here. Example would be to handle automatic user login

            // Simulated long running task. You should remove this in your app.
            await _apiClient.LoginAsync();

            // After performing the long running task we perform an absolute Navigation to remove the SplashScreen from
            // the Navigation Stack.
            await _navigationService.NavigateAsync("/NavigationPage/MainPage");
        }
    }
}
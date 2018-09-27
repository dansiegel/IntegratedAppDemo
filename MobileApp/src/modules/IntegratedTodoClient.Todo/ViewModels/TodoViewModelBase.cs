using IntegratedTodoClient.Services;
using IntegratedTodoClient.Todo.Services;
using IntegratedTodoClient.ViewModels;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;

namespace IntegratedTodoClient.Todo.ViewModels
{
    public class TodoViewModelBase : ViewModelBase
    {
        protected IApiClient _apiClient { get; }

        public TodoViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, ILogger logger, IApiClient apiClient) : base(navigationService, pageDialogService, logger)
        {
            _apiClient = apiClient;
        }
    }
}

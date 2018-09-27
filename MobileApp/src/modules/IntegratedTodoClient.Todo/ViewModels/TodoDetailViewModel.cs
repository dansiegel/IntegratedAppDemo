using System;
using Prism.Navigation;
using Prism.Services;
using IntegratedTodoClient.Services;
using IntegratedTodoClient.Models;
using Prism.Commands;
using Prism.Logging;
using IntegratedTodoClient.ViewModels;
using IntegratedTodoClient.Todo.Services;

namespace IntegratedTodoClient.Todo.ViewModels
{
    public class TodoDetailViewModel : TodoViewModelBase
    {
        public TodoDetailViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ILogger logger, IApiClient apiClient) 
            : base(navigationService, pageDialogService, logger, apiClient)
        {
            
        }

        public TodoItem Model { get; set; }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand CancelCommand { get; }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            Model = parameters.GetValue<TodoItem>("item") ?? new TodoItem();
        }

        private async void OnCancelCommandExecuted() =>
            await _navigationService.GoBackAsync();

        private async void OnSaveCommandExecuted()
        {
            IsBusy = true;
            await _apiClient.SaveTodoItemAsync(Model);
            await _navigationService.GoBackAsync();
            IsBusy = false;
        }
    }
}

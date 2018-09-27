using System.Collections.ObjectModel;
using System.Threading.Tasks;
using IntegratedTodoClient.Models;
using IntegratedTodoClient.Services;
using IntegratedTodoClient.Todo.Resources;
using IntegratedTodoClient.Todo.Services;
using IntegratedTodoClient.ViewModels;
using Microsoft.AppCenter;
using Prism.Commands;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;

namespace IntegratedTodoClient.Todo.ViewModels
{
    public class MainPageViewModel : TodoViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ILogger logger, IApiClient apiClient)
            : base(navigationService, pageDialogService, logger, apiClient)
        {
            Title = AppResources.MainPageTitle;
            TodoItems = new ObservableCollection<TodoItem>();

            AddItemCommand = new DelegateCommand(OnAddItemCommandExecuted);
            RefreshCommand = new DelegateCommand(OnRefreshCommandExecuted);
            TodoItemTappedCommand = new DelegateCommand<TodoItem>(OnTodoItemTappedCommandExecuted);
        }

        public ObservableCollection<TodoItem> TodoItems { get; set; }

        public DelegateCommand AddItemCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public DelegateCommand<TodoItem> TodoItemTappedCommand { get; }

        public override void OnAppearing()
        {
            OnRefreshCommandExecuted();
        }

        private async void OnAddItemCommandExecuted()
        {
            await _navigationService.NavigateAsync("TodoDetail");
        }

        private async void OnRefreshCommandExecuted()
        {
            TodoItems.Clear();
            foreach(var item in await _apiClient.GetTodoItemsAsync())
            {
                TodoItems.Add(item);
            }
        }

        private async void OnTodoItemTappedCommandExecuted(TodoItem item)
        {
            await _navigationService.NavigateAsync("TodoDetail", new NavigationParameters{
                { "item", item }
            });
        }
    }
}
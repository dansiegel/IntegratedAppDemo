using Plugin.Multilingual;
using IntegratedTodoClient.Todo.Resources;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Xamarin.Forms.Xaml;
using IntegratedTodoClient.Todo.Views;
using IntegratedTodoClient.Todo.ViewModels;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IntegratedTodoClient.Todo
{
    public class TodoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Handle post initialization tasks like resolving IEventAggregator to subscribe events
            AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<TodoDetail, TodoDetailViewModel>();

            // Will work by convention. This eliminates the need for location through reflection.
            ViewModelLocationProvider.Register<UserFooter, UserFooterViewModel>();
        }
    }
}
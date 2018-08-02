using Plugin.Multilingual;
using IntegratedTodoClient.Todo.Resources;
using Prism.Ioc;
using Prism.Modularity;
using Xamarin.Forms.Xaml;

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
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<TodoDetail>();
        }
    }
}
using Plugin.Multilingual;
using Prism.Ioc;
using Prism.Modularity;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IntegratedTodoClient.Identity
{
    public class IdentityModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Handle post initialization tasks like resolving IEventAggregator to subscribe events
            // AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
using System;
using System.Threading.Tasks;
using IntegratedTodoClient.Resources;
using IntegratedTodoClient.Services;
using Plugin.Multilingual;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using DryIoc;
using Prism.DryIoc;
using IntegratedTodoClient.Helpers;
using FFImageLoading.Helpers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Prism.Logging;
using Xamarin.Forms;
using Prism.Services;
using Microsoft.Identity.Client;

using DebugLogger = IntegratedTodoClient.Services.DebugLogger;
using System.Collections.Generic;
using Prism.Modularity;
using IntegratedTodoClient.Todo;
using IntegratedTodoClient.Identity;
using System.Linq;

namespace IntegratedTodoClient
{
    public partial class App : PrismApplication
    {
        /* 
         * NOTE: 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App()
            : this(null)
        {
        }

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {

        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            // https://docs.microsoft.com/en-us/mobile-center/sdk/push/xamarin-forms
            Push.PushNotificationReceived += OnPushNotificationReceived;

            AppCenter.Start(AppConstants.AppCenterStart,
                            typeof(Analytics), typeof(Crashes), typeof(Push));
            LogUnobservedTaskExceptions();
            AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;

            Logger.LogCallback = delegate (Microsoft.Identity.Client.LogLevel level, string message, bool containsPii)
            {
                Console.WriteLine($"[{level}] - {message}");
            };

            Logger.Level = Microsoft.Identity.Client.LogLevel.Verbose;
            Logger.PiiLoggingEnabled = true;

            await NavigationService.NavigateAsync("SplashScreenPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register the Popup Plugin Navigation Service
            containerRegistry.RegisterPopupNavigationService();

#if DEBUG
            containerRegistry.GetContainer().RegisterMany<DebugLogger>(Reuse.Singleton, serviceTypeCondition:
                t => typeof(DebugLogger).GetInterfaces().Any(i => i == t));
#else
            containerRegistry.GetContainer().RegisterMany<AppCenterLogger>(Reuse.Singleton, serviceTypeCondition:
                t => typeof(AppCenterLogger).GetInterfaces().Any(i => i == t));
#endif

            containerRegistry.RegisterInstance<IPublicClientApplication>(
                new PublicClientApplication(Secrets.ClientId, AppConstants.Authority)
                {
                    RedirectUri = $"msal{Secrets.ClientId}://auth",
                    ValidateAuthority = true
                });
            containerRegistry.GetContainer().RegisterMany<ClientOptions>(Reuse.Singleton, serviceTypeCondition:
                t => typeof(ClientOptions).GetInterfaces().Any(i => i == t));

            // Navigating to "TabbedPage?createTab=ViewA&createTab=ViewB&createTab=ViewC will generate a TabbedPage
            // with three tabs for ViewA, ViewB, & ViewC
            // Adding `selectedTab=ViewB` will set the current tab to ViewB
            containerRegistry.RegisterForNavigation<TabbedPage>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<IdentityModule>();
            moduleCatalog.AddModule<TodoModule>(InitializationMode.OnDemand);
        }

        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
#if DEBUG
                Console.WriteLine(e.Exception);
#else
                Crashes.TrackError(e.Exception, new Dictionary<string, string> { { "UnobservedException", "true" } });
#endif
            };
        }

        private async void OnPushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            // Add the notification message and title to the message
            var summary = $"Push notification received:" +
                $"\n\tNotification title: {e.Title}" +
                $"\n\tMessage: {e.Message}";

            // If there is custom data associated with the notification,
            // print the entries
            if (e.CustomData != null)
            {
                summary += "\n\tCustom data:\n";
                foreach (var key in e.CustomData.Keys)
                {
                    summary += $"\t\t{key} : {e.CustomData[key]}\n";
                }
            }

            // Send the notification summary to debug output
            Console.WriteLine(summary);
            //Container.Resolve<ILoggerFacade>().Log(summary);
            await Container.Resolve<IPageDialogService>().DisplayAlertAsync(e.Title, e.Message, "Ok");
        }
    }
}

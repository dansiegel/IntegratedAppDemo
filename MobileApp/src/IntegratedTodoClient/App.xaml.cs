using System;
using System.Threading.Tasks;
using IntegratedTodoClient.Resources;
using IntegratedTodoClient.Services;
using IntegratedTodoClient.Views;
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

using DebugLogger = IntegratedTodoClient.Services.DebugLogger;
using Prism.Services;
using Microsoft.Identity.Client;

namespace IntegratedTodoClient
{
    public partial class App : PrismApplication
    {
        //public static string Tenant = "fabrikamb2c.onmicrosoft.com";
        public static string PolicySignUpSignIn = "b2c_1_susi";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] Scopes = { $"https://{Secrets.Tenant}/demoapi/demo.read" };
        //public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public static string AuthorityBase = $"https://login.microsoftonline.com/tfp/{Secrets.Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";

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
            // https://docs.microsoft.com/en-us/mobile-center/sdk/push/xamarin-forms
            Push.PushNotificationReceived += OnPushNotificationReceived;

        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            AppCenter.Start(AppConstants.AppCenterStart,
                            typeof(Analytics), typeof(Crashes), typeof(Push));
            LogUnobservedTaskExceptions();
            AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;

            await NavigationService.NavigateAsync("SplashScreenPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register the Popup Plugin Navigation Service
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterInstance(CreateLogger());

            var pca = new PublicClientApplication(Secrets.ClientId, Authority)
            {
                RedirectUri = $"msal{Secrets.ClientId}://auth"
            };
            containerRegistry.RegisterInstance<IPublicClientApplication>(pca);
                             

            // Navigating to "TabbedPage?createTab=ViewA&createTab=ViewB&createTab=ViewC will generate a TabbedPage
            // with three tabs for ViewA, ViewB, & ViewC
            // Adding `selectedTab=ViewB` will set the current tab to ViewB
            containerRegistry.RegisterForNavigation<TabbedPage>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<TodoDetail>();
            containerRegistry.RegisterForNavigation<SplashScreenPage>();
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            if (await Analytics.IsEnabledAsync())
            {
                System.Diagnostics.Debug.WriteLine("Analytics is enabled");
                FFImageLoading.ImageService.Instance.Config.Logger = (IMiniLogger)Container.Resolve<ILoggerFacade>();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Analytics is disabled");
            }
        }

        protected override void OnSleep()
        {
            // Handle IApplicationLifecycle
            base.OnSleep();

            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle IApplicationLifecycle
            base.OnResume();

            // Handle when your app resumes
        }

        private ILoggerFacade CreateLogger()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case "iOS":
                    if (!string.IsNullOrWhiteSpace(Secrets.AppCenter_iOS_Secret))
                        return CreateAppCenterLogger();
                    break;
            }
            return new DebugLogger();
        }

        private MCAnalyticsLogger CreateAppCenterLogger()
        {
            var logger = new MCAnalyticsLogger();
            FFImageLoading.ImageService.Instance.Config.Logger = (IMiniLogger)logger;
            return logger;
        }

        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Console.WriteLine(e.Exception);
                //Container.Resolve<ILoggerFacade>().Log(e.Exception);
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

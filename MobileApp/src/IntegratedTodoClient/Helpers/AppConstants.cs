using System;

namespace IntegratedTodoClient.Helpers
{
    public static class AppConstants
    {
        // Put constants here that are not of a sensitive nature
        //public static string Tenant = "fabrikamb2c.onmicrosoft.com";
        public const string PolicySignUpSignIn = "b2c_1_susi";
        public const string PolicyEditProfile = "b2c_1_edit_profile";
        public const string PolicyResetPassword = "b2c_1_reset";

        public static string[] Scopes => new string[] { $"https://{Secrets.Tenant}/demoapi/demo.read" };
        //public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public static string AuthorityBase => $"https://login.microsoftonline.com/tfp/{Secrets.Tenant}/";
        public static string Authority => $"{AuthorityBase}{PolicySignUpSignIn}";

        public static string AppCenterStart
        {
            get
            {
                string startup = string.Empty;

                if (Guid.TryParse(Secrets.AppCenter_iOS_Secret, out Guid iOSSecret))
                {
                    startup += $"ios={iOSSecret};";
                }

                return startup;
            }
        }
    }
}

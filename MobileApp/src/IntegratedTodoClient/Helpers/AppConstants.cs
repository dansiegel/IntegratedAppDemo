using System;

namespace IntegratedTodoClient.Helpers
{
    public static class AppConstants
    {
        // Put constants here that are not of a sensitive nature

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

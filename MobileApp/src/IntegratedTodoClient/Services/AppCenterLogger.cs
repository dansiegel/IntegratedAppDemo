using System;
using System.Collections.Generic;
using FFImageLoading.Helpers;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism.Logging;

namespace IntegratedTodoClient.Services
{
    public class AppCenterLogger : ILoggerFacade, IMiniLogger, ILogger
    {
        public void Debug(string message)
        {
            Log("Debug", new Dictionary<string, string>
            {
                { "logger", nameof(IMiniLogger) },
                { "message", message }
            });
        }

        public void Error(string errorMessage)
        {
            Log("Error", new Dictionary<string, string>
            {
                { "logger", nameof(IMiniLogger) },
                { "message", errorMessage }
            });
        }

        public void Error(string errorMessage, Exception ex)
        {
            Log("Error", new Dictionary<string, string>
            {
                { "logger", nameof(IMiniLogger) },
                { "message", errorMessage },
                { "errorType", ex.GetType().Name },
                { "error", ex.ToString() }
            });
        }

        public void Log(string message, Category category, Priority priority)
        {
            Log($"{category}", new Dictionary<string, string>
            {
                { "logger", nameof(ILoggerFacade) },
                { "priority", $"{priority}" },
                { "message", message }
            });
        }

        public void Log(string message, IDictionary<string, string> properties)
        {
            Analytics.TrackEvent(message, properties);
        }

        public void Report(Exception ex, IDictionary<string, string> properties)
        {
            Crashes.TrackError(ex, properties);
        }
    }
}
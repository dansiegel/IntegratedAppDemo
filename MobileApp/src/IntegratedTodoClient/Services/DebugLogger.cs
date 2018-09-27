using System;
using System.Collections.Generic;
using FFImageLoading.Helpers;
using Prism.Logging;
using static System.Diagnostics.Debug;

namespace IntegratedTodoClient.Services
{
    public class DebugLogger : ILoggerFacade, IMiniLogger, ILogger
    {
        public void Debug(string message) =>
            WriteLine($"Debug: {message}");

        public void Error(string errorMessage) =>
            WriteLine($"Error: {errorMessage}");

        public void Error(string errorMessage, Exception ex) =>
            WriteLine($"Error: {errorMessage}\n{ex.GetType().Name}: {ex}");

        public void Log(string message, Category category, Priority priority) =>
            WriteLine($"{category} - {priority}: {message}");

        public void Log(string message, IDictionary<string, string> properties)
        {
            Console.WriteLine(message);
            Console.WriteLine("--------- Begin Properties ----------");
            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    Console.WriteLine($"    {prop.Key}: {prop.Value}");
                }
            }
            Console.WriteLine("-------- End Properties ----------");
        }

        public void Report(Exception ex, IDictionary<string, string> properties)
        {
            Console.WriteLine($"Exception Reported: {ex.GetType().Name}");
            Console.WriteLine("--------- Begin Properties ----------");
            if(properties != null)
            {
                foreach(var prop in properties)
                {
                    Console.WriteLine($"    {prop.Key}: {prop.Value}");
                }
            }
            Console.WriteLine("-------- End Properties ----------");
            Console.WriteLine(ex);
        }
    }
}
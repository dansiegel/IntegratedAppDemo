using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppCenterPushNotificationRelay.DAL;
using AppCenterPushRelay.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace AppCenterPushNotificationRelay
{
    public static class PushRelay
    {
        [FunctionName("PushForUsers")]
        public static async Task<HttpResponseMessage> PushForUsersAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var appCenterToken = GetEnvironmentVariable("AppCenterToken");
            var ownerName = GetEnvironmentVariable("OwnerName");
            if (!IsProperlyConfigured(appCenterToken, ownerName, log))
            {
                return req.CreateResponse(HttpStatusCode.ExpectationFailed, new
                {
                    Error = "Server Fault.The Server is missing critical configuration settings."
                });
            }

            try
            {
                string requestBody = await req.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "The request has no content");
                }

                var request = JsonConvert.DeserializeObject<UserNotificationRequest>(requestBody);
                var db = new AppCenterUserTrackingContext();

                var registrations = db.UserRegistrations.Where(reg => request.Users.Contains(reg.UserId));

                var targets = new Dictionary<string, NotificationTarget>();
                foreach (var platform in registrations.Select(r => r.Platform).Distinct())
                {
                    var target = new NotificationTarget
                    {
                        Type = TargetType.Devices,
                        Devices = registrations.Where(r => r.Platform == platform).Select(r => r.DeviceId)
                    };
                    targets.Add(platform, target);
                }

                var notificationRequest = new NotificationRequest
                {
                    Targets = targets,
                    Content = request.Content
                };
                var responses = await ProcessTargetsAsync(notificationRequest, ownerName, appCenterToken, log);
                return GenerateResponse(req, responses);
            }
            catch (Exception ex)
            {
                log.Error("Unexpected Error occurred", ex);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            
        }

        [FunctionName("PushRelay")]
        public static async Task<HttpResponseMessage> PushRelayAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var appCenterToken = GetEnvironmentVariable("AppCenterToken");
            var ownerName = GetEnvironmentVariable("OwnerName");
            if(!IsProperlyConfigured(appCenterToken, ownerName, log))
            {
                return req.CreateResponse(HttpStatusCode.ExpectationFailed, new
                {
                    Error = "Server Fault.The Server is missing critical configuration settings."
                });
            }

            try
            {
                string requestBody = await req.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "The request has no content");
                }

                var request = JsonConvert.DeserializeObject<NotificationRequest>(requestBody);
                var responses = await ProcessTargetsAsync(request, ownerName, appCenterToken, log);

                return GenerateResponse(req, responses);
            }
            catch (Exception ex)
            {
                log.Error("Unexpected Error occurred", ex);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private static bool IsProperlyConfigured(string appCenterToken, string ownerName, TraceWriter log)
        {
            if (string.IsNullOrWhiteSpace(appCenterToken))
            {
                log.Error($"The App Center Token has not been configured in the application settings");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ownerName))
            {
                log.Error($"The Owner Name has not been configured in the application settings");
                return false;
            }

            return true;
        }

        private static HttpResponseMessage GenerateResponse(HttpRequestMessage req, IDictionary<string, NotificationResponse> responses)
        {
            var responseCode = HttpStatusCode.Accepted;
            if (responses.All(r => string.IsNullOrWhiteSpace(r.Value.NotificationId)))
            {
                responseCode = HttpStatusCode.BadRequest;
            }
            else if (responses.Any(r => !string.IsNullOrWhiteSpace(r.Value.Error?.Message)))
            {
                responseCode = HttpStatusCode.OK;
            }

            return req.CreateResponse(responseCode, responses);
        }

        private static async Task<IDictionary<string, NotificationResponse>> ProcessTargetsAsync(NotificationRequest request, string ownerName, string appCenterToken, TraceWriter log)
        {
            var responses = new Dictionary<string, NotificationResponse>();
            foreach (var target in request.Targets)
            {
                var appName = GetEnvironmentVariable($"{target.Key}AppName");

                if (string.IsNullOrWhiteSpace(appName))
                {
                    log.Error($"The App Name has not been configured in the application settings for {target.Key}");
                    responses.Add(target.Key, new NotificationResponse
                    {
                        Error = new ErrorDetails
                        {
                            Code = "BadRequest",
                            Message = $"Server Fault. The Server has is missing critical configuration settings for {target.Key}."
                        }
                    });
                    continue;
                }

                log.Info($"Sending message {request.Content.Name} for {target.Key}");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.appcenter.ms");
                    client.DefaultRequestHeaders.Add("X-API-Token", appCenterToken);

                    var notificationMessage = new NotificationMessage
                    {
                        Target = target.Value,
                        Content = request.Content
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(notificationMessage), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"/v0.1/apps/{ownerName}/{appName}/push/notifications", content);
                    responses.Add(target.Key, await response.Content.ReadAsAsync<NotificationResponse>());
                }
            }

            return responses;
        }

        private static string GetEnvironmentVariable(string name)
        {
            try
            {
                return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

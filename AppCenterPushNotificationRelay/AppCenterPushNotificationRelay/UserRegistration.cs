using System.Linq;
using AppCenterPushNotificationRelay.DAL;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace AppCenterPushNotificationRelay
{
    public static class UserRegistration
    {
        [FunctionName("UserRegistration")]
        public static void Run([ServiceBusTrigger("knownuser", AccessRights.Listen, Connection = "")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var user = JsonConvert.DeserializeObject<AppCenterPushRelay.Models.UserRegistration>(myQueueItem);

            var db = new AppCenterUserTrackingContext();

            if(!db.UserRegistrations.Any(ur => ur.UserId == user.UserId && ur.DeviceId == user.DeviceId))
            {
                db.UserRegistrations.Add(user);
                db.SaveChanges();
            }
        }
    }
}

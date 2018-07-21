using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMobileApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace DemoMobileApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private IQueueClient _queueClient { get; }

        public UserProfilesController(IQueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        // GET api/values
        [HttpGet("me")]
        public async Task<UserProfile> GetUserProfileAsyc()
        {
            var notification = new UserRecordNotification
            {
                Platform = HttpContext.Request.Headers["X-Platform"],
                DeviceId = HttpContext.Request.Headers["X-DeviceId"],
                UserId = HttpContext.User.FindFirst("objectId").Value
            };
            var jsonPayload = JsonConvert.SerializeObject(notification);

            var message = new Message(Encoding.ASCII.GetBytes(jsonPayload));
            await _queueClient.SendAsync(message);

            return HttpContext.User;
        }
    }
}

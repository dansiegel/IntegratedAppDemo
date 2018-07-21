using System;
using System.ComponentModel.DataAnnotations;

namespace AppCenterPushRelay.Models
{
    public class UserRegistration
    {
        [Key]
        public Guid UserId { get; set; }

        [Key]
        public Guid DeviceId { get; set; }

        public string Platform { get; set; }
    }
}

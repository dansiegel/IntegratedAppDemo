using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoMobileApi.Models
{
    public class UserProfile
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public static implicit operator UserProfile(ClaimsPrincipal principal)
        {
            return new UserProfile
            {
                FirstName = principal.FindFirst("givenName").Value,
                LastName = principal.FindFirst("surname").Value,
                Email = principal.FindFirst("emails").Value
            };
        }
    }
}

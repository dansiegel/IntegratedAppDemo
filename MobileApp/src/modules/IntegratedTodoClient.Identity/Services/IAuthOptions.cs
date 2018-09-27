using System;
using System.Collections.Generic;
using System.Text;

namespace IntegratedTodoClient.Identity.Services
{
    public interface IAuthOptions
    {
        string PolicySignUpSignIn { get; }
        string PolicyEditProfile { get; }
        string PolicyResetPassword { get; }

        string[] Scopes { get; }

        string AuthorityBase { get; }
    }
}

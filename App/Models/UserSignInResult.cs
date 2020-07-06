using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models
{
    public class UserSignInResult<TUser>
    {
        public TUser User { get; set; }
        public SignInResult SignInResult { get; }

        public UserSignInResult(SignInResult result)
        {
            SignInResult = result ?? throw new NullReferenceException("SignInResult can't be null");
        }
    }
}

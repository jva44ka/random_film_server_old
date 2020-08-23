using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels.ResultModels
{
    public class LoginResult
    {
        public bool LoggedIn { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserId { get; set; }
    }
}

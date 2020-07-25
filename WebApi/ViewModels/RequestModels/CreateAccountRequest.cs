using System;

namespace WebApi.ViewModels.RequestModels
{
    public class CreateAccountRequest
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool SignInAfter { get; set; }
    }
}

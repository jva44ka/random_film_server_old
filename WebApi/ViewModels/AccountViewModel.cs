using System;
using System.Collections.Generic;

namespace WebApi.ViewModels
{
    public class AccountViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime? LastSeenOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public AccountViewModel CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public AccountViewModel ModifiedBy { get; set; }

        public virtual ICollection<CommentViewModel> Comments { get; set; }
        public virtual ICollection<LikeViewModel> Likes { get; set; }
    }
}

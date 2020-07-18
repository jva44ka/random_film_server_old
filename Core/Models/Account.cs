using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Accounts")]
    public class Account : IdentityUser, IDataModel
    {
        [Key, Required]
        public override string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public override string UserName { get; set; }
        public bool IsMainAdmin { get; set; }
        public DateTime? LastSeenOn { get; set; }
        [Required]
        public DateTime? CreatedOn { get; set; }
        public Account CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Account ModifiedBy { get; set; }

        public virtual Image Avatar { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<UserFilm> Likes { get; set; }
    }
}

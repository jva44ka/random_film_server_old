using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Likes")]
    public class Like : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        [Required]
        public bool IsLike { get; set; }
        [Required]
        public DateTime? CreatedOn { get; set; }


        public virtual Account Owner { get; set; }
        public virtual Film Film { get; set; }
    }
}

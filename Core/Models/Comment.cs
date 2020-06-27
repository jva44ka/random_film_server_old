using Core.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Comments")]
    public class Comment : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        public string Text { get; set; }
        [Required]
        public DateTime? CreatedOn { get; set; }
        public Account CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Account ModifiedBy { get; set; }

        public Account Owner { get; set; }
        public virtual Film Film { get; set; }
    }
}

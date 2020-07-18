using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Images")]
    public class Image : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [Required]
        public string Data { get; set; }
    }
}

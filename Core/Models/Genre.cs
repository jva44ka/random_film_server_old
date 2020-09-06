using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Genres")]
    public class Genre : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

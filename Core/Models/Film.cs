using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Films")]
    public class Film : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Description { get; set; }
        public DateTime? Year { get; set; }
        public string Director { get; set; }
        public string UrlImg { get; set; }
        public string UrlTrailer { get; set; }

        [Required]
        public DateTime? CreatedOn { get; set; }
        public Account CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Account ModifiedBy { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FilmsGenres> FilmsGenres { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}

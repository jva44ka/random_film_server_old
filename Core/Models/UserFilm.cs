using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("UserFilms")]
    public class UserFilm : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }
        public bool? IsLike { get; set; }
        [Required]
        public DateTime? ViewedOn { get; set; }
        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [Required, ForeignKey(nameof(Film))]
        public Guid FilmId { get; set; }

        [Required]
        public virtual Account User { get; set; }
        [Required]
        public virtual Film Film { get; set; }
    }
}

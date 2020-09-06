using Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("FilmGenres")]
    public class FilmGenre : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }

        [Required, ForeignKey(nameof(Film))]
        public Guid FilmId { get; set; }
        [Required, ForeignKey(nameof(Genre))]
        public  Guid GenreId { get; set; }

        [Required]
        public virtual Film Film { get; set; }
        [Required]
        public virtual Genre Genre { get; set; }
    }
}

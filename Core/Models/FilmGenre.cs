using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("FilmsGenres")]
    public class FilmGenre : IDataModel
    {
        [Key, Required]
        public Guid Id { get; set; }

        public virtual Film Film { get; set; }
        public virtual Genre Genre { get; set; }
    }
}

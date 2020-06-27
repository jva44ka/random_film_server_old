using System;
using System.Collections.Generic;

namespace randomfilm_backend.Models.Entities
{
    public partial class FilmsGenres
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public int GenreId { get; set; }

        public virtual Film Film { get; set; }
        public virtual Genre Genre { get; set; }
    }
}

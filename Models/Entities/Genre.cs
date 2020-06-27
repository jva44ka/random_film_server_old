using System;
using System.Collections.Generic;

namespace randomfilm_backend.Models.Entities
{
    public partial class Genre
    {
        public Genre()
        {
            FilmsGenres = new HashSet<FilmsGenres>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FilmsGenres> FilmsGenres { get; set; }
    }
}

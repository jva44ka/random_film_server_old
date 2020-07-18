using System;
using System.Collections.Generic;

namespace WebApi.ViewModels
{
    public class FilmViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Description { get; set; }
        public DateTime? Year { get; set; }
        public string Director { get; set; }
        public string UrlTrailer { get; set; }
        public ICollection<GenreViewModel> Genres { get; set; }
    }
}

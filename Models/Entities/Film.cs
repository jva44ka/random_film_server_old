using System;
using System.Collections.Generic;

namespace randomfilm_backend.Models.Entities
{
    public partial class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Description { get; set; }
        public DateTime? Year { get; set; }
        public string Director { get; set; }
        public string UrlImg { get; set; }
        public string UrlTrailer { get; set; }


        public Film()
        {
            Comments = new HashSet<Comment>();
            FilmsGenres = new HashSet<FilmsGenres>();
            Likes = new HashSet<Like>();
        }


        /// <param name="id">ID</param>
        /// <param name="title">Название</param>
        /// <param name="duration">Длительность</param>
        /// <param name="description">Описание</param>
        /// <param name="year">Год выхода</param>
        /// <param name="director">Режиссер</param>
        /// <param name="urlImg">Ссылка на превью</param>
        /// <param name="urlTrailer">Ссылка на трейлер youtube</param>
        public Film(int id, string title, TimeSpan? duration, string description,
            DateTime? year, string director, string urlImg, string urlTrailer)
        {
            this.Id = id;
            this.Title = title;
            this.Duration = duration;
            this.Description = description;
            this.Year = year;
            this.Director = director;
            this.UrlImg = urlImg;
            this.UrlTrailer = urlTrailer;

            Comments = new HashSet<Comment>();
            FilmsGenres = new HashSet<FilmsGenres>();
            Likes = new HashSet<Like>();
        }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FilmsGenres> FilmsGenres { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}

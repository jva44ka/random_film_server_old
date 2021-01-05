using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Managers.Interfaces
{
    public interface IFilmManager
    {
        IList<Film> GetAllFilms();
        Film GetFilmById(Guid id);
        IList<Genre> GetGenres(Guid id);
        Task<bool?> IsLiked(string userId, Guid filmId);
        Task<Film> CreateAsync(Film film);
        Task<Film> UpdateAsync(Guid id, Film film);
        Task<Image> UpdatePreview(Guid filmId, Image newPreview);
        Task<bool> DeleteAsync(Guid id);

        // Подбор фильмов 
        Task<IList<Film>> GetRandomShakedFilms(string userId = null);
        Task<IList<Film>> GetSameUsersFilms(string userId);
        Task<IList<Film>> GetPopularFilms(string userId = null);
    }
}

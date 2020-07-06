using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Managers.Interfaces
{
    public interface IFilmManager
    {
        Task<IList<Film>> GetAllFilms();
        Task<Film> GetFilmById(Guid id);
        Task<IList<Film>> GetRandomShakedFilms();
        Task<IList<Film>> GetSpicifityFilms(string userName);
        Task<Film> CreateAsync(Film film);
        Task<Film> UpdateAsync(Guid id, Film film);
        Task<bool> DeleteAsync(Guid id);
    }
}

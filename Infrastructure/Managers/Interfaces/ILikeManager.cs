using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Managers.Interfaces
{
    public interface ILikeManager
    {
        IList<UserFilm> GetLikes();
        UserFilm GetLikeById(Guid id);
        UserFilm GetLikeByFilm(string userName, Guid filmId);
        Task<UserFilm> CreateAsync(UserFilm like);
        Task<bool> DeleteByFilmAsync(string userName, Guid filmId);
        Task<bool> DeleteAsync(Guid id);
    }
}

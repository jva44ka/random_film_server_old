using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Managers.Interfaces
{
    public interface IUserFilmManager
    {
        IList<UserFilm> GetLikes();
        UserFilm GetLikeById(Guid id);
        UserFilm GetLikeByFilm(string userName, Guid filmId);
        Task<bool?> LikeOrDislike(Guid filmId, string userId, bool? likeOrDislike);
        Task<bool> DeleteByFilmAsync(string userName, Guid filmId);
        Task<bool> DeleteAsync(Guid id);
    }
}

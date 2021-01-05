using Core.Models;
using Services.Models;
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
        Task<RemoveResult<UserFilm>> DeleteByFilmAsync(string userName, Guid filmId);
        Task<RemoveResult<UserFilm>> DeleteAsync(Guid id);
    }
}

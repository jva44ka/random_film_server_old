using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Managers.Interfaces
{
    public interface ILikeManager
    {
        IList<Like> GetLikes();
        Like GetLikeById(Guid id);
        Like GetLikeByFilm(string userName, Guid filmId);
        Task<Like> CreateAsync(Like like);
        Task<bool> DeleteByFilmAsync(string userName, Guid filmId);
        Task<bool> DeleteAsync(Guid id);
    }
}

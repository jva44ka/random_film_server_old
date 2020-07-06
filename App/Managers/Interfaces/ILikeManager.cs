using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Managers.Interfaces
{
    public interface ILikeManager
    {
        Task<IList<Like>> GetLikes();
        Task<Like> GetLikeById(Guid id);
        Task<Like> CreateAsync(Like film);
        Task<Like> UpdateAsync(Guid id, Like film);
        Task<bool> DeleteByFilmAsync(string userName, Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}

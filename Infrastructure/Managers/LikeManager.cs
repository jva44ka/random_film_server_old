using Core.Interfaces;
using Core.Models;
using Infrastructure.Exceptions;
using Infrastructure.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Managers
{
    public class LikeManager : ILikeManager
    {
        private readonly IRepository<Like> _likeRepo;
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Film> _filmRepo;

        public LikeManager(IRepository<Like> likeRepo,
                            IRepository<Account> accountRepo,
                            IRepository<Film> filmRepo)
        {
            this._likeRepo = likeRepo;
            this._accountRepo = accountRepo;
            this._filmRepo = filmRepo;
        }
        public IList<Like> GetLikes() 
        {
            return (IList<Like>)_likeRepo.GetAll().ToList();
        }

        public Like GetLikeById(Guid id) 
        {
            return _likeRepo.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public Like GetLikeByFilm(string userName, Guid filmId)
        {
            return _likeRepo.GetAll()
                            .Include(x => x.Film)
                            .Include(x => x.Owner)
                            .FirstOrDefault(x => x.Owner.UserName == userName && 
                                                x.Film.Id == filmId);
        }

        public async Task<Like> CreateAsync(Like like)
        {
            var owner = _accountRepo.GetAll().FirstOrDefault(x => x.Id == like.Owner.Id);
            var film = _filmRepo.GetAll().FirstOrDefault(x => x.Id == like.Film.Id);
            like.Owner = owner;
            like.Film = film;

            var sameLike = _likeRepo.GetAll().Include(x => x.Film)
                                            .Include(x => x.Owner)
                                            .FirstOrDefault(x => x.Film.Id == like.Film.Id && x.Owner.Id == like.Owner.Id);

            if(sameLike != null)
            {
                if (sameLike.IsLike == like.IsLike)
                    //Пользователь пытается поставить существующий лайк/дизлайк
                    throw new AlreadyExistsException("That like is already exists");
                else
                    //Пользователь ставит оценку противоположную уже поставленой
                    return _likeRepo.Update(like);
            }

            like.CreatedOn = DateTime.UtcNow;
            return await _likeRepo.CreateAsync(like);
        }

        public async Task<bool> DeleteByFilmAsync(string userName, Guid filmId) 
        {
            var owner = _accountRepo.GetAll().FirstOrDefault(x => x.UserName == userName);
            var like = _likeRepo.GetAll()
                                .Include(x => x.Film)
                                .Include(x => x.Owner)
                                .FirstOrDefault(x => (x.Film.Id == filmId) &&
                                                    (x.Owner.Id == owner.Id));
            if (like == null)
                throw new NotExistsException("Like not exists for delete");

            return _likeRepo.Delete(like.Id);
        }

        public async Task<bool> DeleteAsync(Guid id) 
        {
            var like = await _likeRepo.GetByIdAsync(id);
            if (like == null)
                throw new NotExistsException("Like not exists for delete");

            return _likeRepo.Delete(like.Id);
        }
    }
}

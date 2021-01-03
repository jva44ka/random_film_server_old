using Core.Interfaces;
using Core.Models;
using Infrastructure.Exceptions;
using Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Services.Managers
{
    public class UserFilmManager : IUserFilmManager
    {
        private readonly IRepository<UserFilm> _userFilmsRepo;
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Film> _filmsRepo;

        public UserFilmManager(IRepository<UserFilm> userFilmsRepo,
                            IRepository<Account> accountRepo,
                            IRepository<Film> filmsRepo)
        {
            this._userFilmsRepo = userFilmsRepo;
            this._accountRepo = accountRepo;
            this._filmsRepo = filmsRepo;
        }
        public IList<UserFilm> GetLikes() 
        {
            return _userFilmsRepo.Get().ToList();
        }

        public UserFilm GetLikeById(Guid id) 
        {
            return _userFilmsRepo.Get().FirstOrDefault(x => x.Id == id);
        }

        public UserFilm GetLikeByFilm(string userName, Guid filmId)
        {
            return _userFilmsRepo.Get()
                            .Include(x => x.Film)
                            .Include(x => x.User)
                            .FirstOrDefault(x => x.User.UserName == userName && 
                                                x.Film.Id == filmId);
        }

        public async Task<bool?> LikeOrDislike(Guid filmId, string userId, bool? likeOrDislike)
        {
            var film = _filmsRepo.Get().AsNoTracking().Single(x => x.Id == filmId);
            var user = _accountRepo.Get().AsNoTracking().Single(x => x.Id == userId);

            if (film == null || user == null)
                throw new NotExistsException("User or Film with that id is not exists");

            var react = _userFilmsRepo
                .Get()
                .AsTracking()
                .FirstOrDefault(x => x.FilmId == filmId && x.UserId == userId);

            if (react?.IsLike == likeOrDislike)
                throw new AlreadyExistsException("This like/dislike already exists");

            if (react != null)
            {
                react.IsLike = likeOrDislike;
            }
            else
            {
                react = new UserFilm();
                react.FilmId = filmId;
                react.UserId = userId;
                react.IsLike = likeOrDislike;
                react.ViewedOn = DateTime.UtcNow;
                _userFilmsRepo.Create(react);
            }
            
            await _userFilmsRepo.SaveAsync();
            return react.IsLike;
        }

        public async Task<bool> DeleteByFilmAsync(string userName, Guid filmId) 
        {
            var owner = _accountRepo.Get().FirstOrDefault(x => x.UserName == userName);
            var like = _userFilmsRepo.Get()
                                .Include(x => x.Film)
                                .Include(x => x.User)
                                .FirstOrDefault(x => (x.Film.Id == filmId) &&
                                                    (x.User.Id == owner.Id));
            if (like == null)
                throw new NotExistsException("Like not exists for delete");

            return _userFilmsRepo.Delete(like.Id);
        }

        public async Task<bool> DeleteAsync(Guid id) 
        {
            var like = await _userFilmsRepo.GetByIdAsync(id);
            if (like == null)
                throw new NotExistsException("Like not exists for delete");

            return _userFilmsRepo.Delete(like.Id);
        }
    }
}

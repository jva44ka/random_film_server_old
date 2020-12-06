using Core.Interfaces;
using Core.Models;
using Services.Algorithms.Interfaces;
using Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Managers
{
    public class FilmManager : IFilmManager
    {
        private IRepository<Film> _filmsRepo;
        private IRepository<Account> _usersRepo;
        private IRepository<UserFilm> _userFilmsRepo;
        private ISameUsersAlgorithm _specifityFilmSelector;

        public FilmManager(IRepository<Film> films,
                            IRepository<Account> users, 
                            IRepository<UserFilm> userFilms,
                            ISameUsersAlgorithm specifityFilmSelector)
        {
            this._filmsRepo = films;
            this._usersRepo = users;
            this._userFilmsRepo = userFilms;
            this._specifityFilmSelector = specifityFilmSelector;
        }

        public IList<Film> GetAllFilms()
        {
            return this._filmsRepo
                .Get()
                .Include(x => x.Preview)
                .Include(x => x.FilmsGenres)
                    .ThenInclude(x => x.Genre)
                .ToList();
        }

        public Film GetFilmById(Guid id)
        {
            return _filmsRepo
                .Get()
                .Include(x => x.Preview)
                .Include(x => x.FilmsGenres)
                    .ThenInclude(x => x.Genre)
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<IList<Film>> GetRandomShakedFilms()
        {
            //Вытаскиваем бд в кеш
            List<Film> filmsCache = await this._filmsRepo.Get()
                                .Include(x => x.Likes)
                                .Include(x => x.FilmsGenres)
                                    .ThenInclude(x => x.Genre)
                                .Include(x => x.Preview)
                                .Where(x => x.FilmsGenres.FirstOrDefault(y => y.Film.Id == x.Id) != null)
                                .ToListAsync();
            Film[] result = new Film[filmsCache.Count];

            //Буферные переменные для работы с рандомной выборкой и переброса из коллекции в коллекцию
            int filmsCacheCount = filmsCache.Count;
            Random random = new Random();
            Film selectedFilm;

            //Заполнение массива рандомными фильмами
            for (int i = 0; i < filmsCacheCount; i++)
            {
                selectedFilm = filmsCache[random.Next(0, filmsCache.Count)];
                result[i] = selectedFilm;
                filmsCache.Remove(selectedFilm);
            }

            return result.ToList();
        }

        public IList<Genre> GetGenres(Guid id)
        {
            return _filmsRepo.Get()
                                .Include(x => x.FilmsGenres)
                                    .ThenInclude(x => x.Genre)
                                .SelectMany(x => x.FilmsGenres)
                                .Select(x => x.Genre)
                                .ToList();
        }

        public async Task<IList<Film>> GetSpicifityFilms(string userId)
        {
            var user = this._usersRepo.Get().First(x => x.Id == userId);
            return await _specifityFilmSelector.GetFilmsAsync(user);
        }

        public Task<bool?> IsLiked(string userId, Guid filmId)
        {
            var filmReact = this._userFilmsRepo
                .Get()
                .FirstOrDefault(x => x.UserId == userId && x.FilmId == filmId);

            return Task.FromResult(filmReact?.IsLike);
        }

        public async Task<Film> CreateAsync(Film film)
        {
            film.CreatedOn = DateTime.UtcNow;
            var newFilm = await _filmsRepo.CreateAsync(film);
            await _filmsRepo.SaveAsync();
            return newFilm;
        }

        public async Task<Film> UpdateAsync(Guid id, Film changedFilm)
        {
            _filmsRepo.Untrack(changedFilm);

            var film = _filmsRepo
                .Get()
                .AsTracking()
                .Single(x => x.Id == id);

            film.ModifiedOn = DateTime.UtcNow;
            film.Title = changedFilm.Title;
            film.UrlTrailer = changedFilm.UrlTrailer;
            film.Year = changedFilm.Year;
            film.Description = changedFilm.Description;
            film.Director = changedFilm.Director;
            film.Duration = changedFilm.Duration;

            await _filmsRepo.SaveAsync();
            return film;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = _filmsRepo.Delete(id);
            await _filmsRepo.SaveAsync();
            return result;
        }
    }
}

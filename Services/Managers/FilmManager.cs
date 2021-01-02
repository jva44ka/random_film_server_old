using Core.Interfaces;
using Core.Models;
using Services.Algorithms.Interfaces;
using Services.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;

namespace Services.Managers
{
    public class FilmManager : IFilmManager
    {
        private IRepository<Film> _filmsRepo;
        private IRepository<Account> _usersRepo;
        private IRepository<UserFilm> _userFilmsRepo;
        private readonly IRepository<SelectionList> _selectionListRepo;
        private ISameUsersAlgorithm _specifityFilmSelector;
        private readonly IRandomFilmsAlgorithm _randomFilmsAlgorithm;
        private readonly IPopularFilmsAlgorithm _popularFilmsAlgorithm;

        public FilmManager(IRepository<Film> films,
                            IRepository<Account> users, 
                            IRepository<UserFilm> userFilms,
                            IRepository<SelectionList> selectionListRepo,
                            ISameUsersAlgorithm specifityFilmSelector,
                            IRandomFilmsAlgorithm randomFilmsAlgorithm, 
                            IPopularFilmsAlgorithm popularFilmsAlgorithm)
        {
            _filmsRepo = films;
            _usersRepo = users;
            _userFilmsRepo = userFilms;
            _selectionListRepo = selectionListRepo;
            _specifityFilmSelector = specifityFilmSelector;
            _randomFilmsAlgorithm = randomFilmsAlgorithm;
            _popularFilmsAlgorithm = popularFilmsAlgorithm;
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

        public async Task<IList<Film>> GetRandomShakedFilms(string userId = null)
        {
            //Проверяем существует ли подборка и актуальна ли она (например не больше чем месячной давности)
            var selection = _selectionListRepo
                .Get()
                .Include(sl => sl.FilmSelectionLists)
                    .ThenInclude(fsl => fsl.Film)
                        .ThenInclude(f => f.Preview)
                .FirstOrDefault(fl => fl.UserId == userId
                    && fl.AlgorithmType == AlgorithmType.RandomAlgorithm);

            //Подборка существует и она нынешнего месяца
            if (selection != null && DateTime.UtcNow.Month == selection.CreatedOn.Month)
                return selection.FilmSelectionLists.Select(fsl => fsl.Film).ToList();

            //Подборка существует но она просрочена
            else if (selection != null)
            {
                _selectionListRepo.Delete(selection.Id);
                await _selectionListRepo.SaveAsync();
            }

            // Не существует никакой
            var films = await _randomFilmsAlgorithm.GetFilms(userId);
            if (!string.IsNullOrEmpty(userId))
                await CreateList(userId, films, AlgorithmType.RandomAlgorithm);
            return films;
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

        public async Task<IList<Film>> GetSameUsersFilms(string userId)
        {
            //Проверяем существует ли подборка и актуальна ли она (например не больше чем месячной давности)
            var selection = _selectionListRepo
                .Get()
                .Include(sl => sl.FilmSelectionLists)
                    .ThenInclude(fsl => fsl.Film)
                        .ThenInclude(f => f.Preview)
                .FirstOrDefault(fl => fl.UserId == userId
                    && fl.AlgorithmType == AlgorithmType.SameUsersAlgorithm);

            //Подборка существует и она нынешнего месяца
            if (selection != null && DateTime.UtcNow.Month == selection.CreatedOn.Month)
                return selection.FilmSelectionLists.Select(fsl => fsl.Film).ToList();

            //Подборка существует но она просрочена
            else if (selection != null)
            {
                _selectionListRepo.Delete(selection.Id);
                await _selectionListRepo.SaveAsync();
            }

            // Не существует никакой
            var films = await _specifityFilmSelector.GetFilms(userId);
            if (!string.IsNullOrEmpty(userId))
                await CreateList(userId, films, AlgorithmType.SameUsersAlgorithm);
            return films;
        }

        public async Task<IList<Film>> GetPopularFilms(string userId = null)
        {
            //Проверяем существует ли подборка и актуальна ли она (например не больше чем месячной давности)
            var selection = _selectionListRepo
                .Get()
                .Include(sl => sl.FilmSelectionLists)
                    .ThenInclude(fsl => fsl.Film)
                        .ThenInclude(f => f.Preview)
                .FirstOrDefault(fl => fl.UserId == userId
                    && fl.AlgorithmType == AlgorithmType.PopularFilmsAlgorithm);

            //Подборка существует и она нынешнего месяца
            if (selection != null && DateTime.UtcNow.Month == selection.CreatedOn.Month)
                return selection.FilmSelectionLists.Select(fsl => fsl.Film).ToList();

            //Подборка существует но она просрочена
            else if (selection != null)
            {
                _selectionListRepo.Delete(selection.Id);
                await _selectionListRepo.SaveAsync();
            }

            // Не существует никакой
            var films = await _popularFilmsAlgorithm.GetFilms(userId);
            if (!string.IsNullOrEmpty(userId))
                await CreateList(userId, films, AlgorithmType.PopularFilmsAlgorithm);
            return films;
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

        private async Task CreateList(string userId, IList<Film> films, AlgorithmType algorithmType)
        {
            var filmSelectionLists = films.Select((film, index) => new FilmSelectionList
            {
                FilmId = film.Id,
                Order = index
            }).ToList();

            var selectionList = new SelectionList
            {
                UserId = userId,
                AlgorithmType = algorithmType,
                CreatedOn = DateTime.UtcNow,
                FilmSelectionLists = filmSelectionLists
            };
            await _selectionListRepo.CreateAsync(selectionList);
            await _selectionListRepo.SaveAsync();
        }
    }
}

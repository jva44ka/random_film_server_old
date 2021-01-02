using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Services.Algorithms.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Algorithms
{
    public class RandomFilmsAlgorithm : IRandomFilmsAlgorithm
    {
        private readonly IRepository<Film> _filmsRepo;
        private readonly IRepository<UserFilm> _userFilmRepo;

        public RandomFilmsAlgorithm(IRepository<Film> filmsRepo, IRepository<UserFilm> userFilmRepo)
        {
            _filmsRepo = filmsRepo;
            _userFilmRepo = userFilmRepo;
        }

        public Task<IList<Film>> GetFilms(string userId = null)
        {
            //Вытаскиваем бд в кеш
            List<Film> filmsCache = _filmsRepo.Get()
                .Include(x => x.Likes)
                .Include(x => x.FilmsGenres)
                    .ThenInclude(x => x.Genre)
                .Include(x => x.Preview)
                .Where(x => x.FilmsGenres.FirstOrDefault(y => y.Film.Id == x.Id) != null)
                .ToList();
            Film[] resultArr = new Film[filmsCache.Count];

            //Буферные переменные для работы с рандомной выборкой и переброса из коллекции в коллекцию
            int filmsCacheCount = filmsCache.Count;
            Random random = new Random();
            Film selectedFilm;

            //Заполнение массива рандомными фильмами
            for (int i = 0; i < filmsCacheCount; i++)
            {
                selectedFilm = filmsCache[random.Next(0, filmsCache.Count)];
                resultArr[i] = selectedFilm;
                filmsCache.Remove(selectedFilm);
            }

            var result = resultArr.ToList();

            //Если userId != null, значит удалить из результирующей коллекции просмотренные фильмы
            if (!string.IsNullOrEmpty(userId))
            {
                var watchedFilmIds = _userFilmRepo.Get()
                    .Where(uf => uf.UserId == userId && uf.IsLike != null)
                    .Select(uf => uf.FilmId)
                    .ToList();

                result = result.Where(f => !watchedFilmIds.Contains(f.Id))
                    .ToList();
            }

            return Task.FromResult((IList<Film>)result);
        }
    }
}

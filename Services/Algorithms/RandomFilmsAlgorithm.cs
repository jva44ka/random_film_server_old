using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Services.Algorithms.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Algorithms
{
    public class RandomFilmsAlgorithm : IRandomFilmsAlgorithm
    {
        private readonly IRepository<Film> _filmsRepo;
        private readonly IRepository<SelectionList> _selectionListRepo;

        public RandomFilmsAlgorithm(IRepository<Film> filmsRepo, 
            IRepository<SelectionList> selectionListRepo)
        {
            _filmsRepo = filmsRepo;
            _selectionListRepo = selectionListRepo;
        }
        public Task<IEnumerable<Film>> GetFilms(string userId = null)
        {
            //Проверяем существует ли подборка и актуальна ли она (например не больше чем месячной давности)
            var list = _selectionListRepo
                .Get()
                .Include(sl => sl.FilmSelectionLists)
                    .ThenInclude(fsl => fsl.Film)
                .FirstOrDefault(fl => fl.UserId == userId
                    && DateTime.UtcNow.Month != fl.CreatedOn.Month 
                    && fl.AlgorithmType == AlgorithmType.RandomAlgorithm);

            if (list != null)
                return Task.FromResult(list.FilmSelectionLists.Select(fsl => fsl.Film).AsEnumerable());

            //Вытаскиваем бд в кеш
            List<Film> filmsCache = _filmsRepo.Get()
                                .Include(x => x.Likes)
                                .Include(x => x.FilmsGenres)
                                    .ThenInclude(x => x.Genre)
                                .Include(x => x.Preview)
                                .Where(x => x.FilmsGenres.FirstOrDefault(y => y.Film.Id == x.Id) != null)
                                .ToList();
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

            return Task.FromResult(result.AsEnumerable());
        }
    }
}

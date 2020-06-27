using Microsoft.EntityFrameworkCore;
using randomfilm_backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace randomfilm_backend.Models.Algorithms
{
    /// <summary>
    /// Алгоритм выдачи фильма с учетом предпочтений пользователя
    /// Кратко: алгоритм ищет пользователей с такими же лайкнутыми фильмами. Затем сортирует по количеству совпадений и берет 
    /// какое-то число соседей (k) и смотрит какие у этих соседей общие лайкнутые фильмы, которые не лайкнул исходный пользователь.
    /// Если не находит берет рандомный из лайкнутых соседями но не лайкнутый пользователем.
    /// </summary>
    public class RandomAlgorithm : IFilmSelection
    {
        RandomFilmDBContext db;
        public RandomAlgorithm(RandomFilmDBContext db)
        {
            this.db = db;
        }

        public async Task<List<Film>> GetFilmsAsync(Account user)
        {
            //Вытаскиваем бд в кеш
            List<Film> filmsCache = await this.db.Films
                                .Include(x => x.Likes)
                                .Include(x => x.FilmsGenres)
                                    .ThenInclude(x => x.Genre)
                                .Where(x => x.FilmsGenres.FirstOrDefault(y => y.FilmId == x.Id) != null)
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
    }
}

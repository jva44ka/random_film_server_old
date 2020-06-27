using Microsoft.EntityFrameworkCore;
using randomfilm_backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace randomfilm_backend.Models.Algorithms
{
    /// <summary>
    /// Алгоритм выдачи фильма с учетом предпочтений пользователя (на основе метода ближайших k соседей)
    /// Кратко: алгоритм высчитывает средние значения пользователя по каждому жанру (1 - все лайки, -1 - все дизлайки), высчитывает
    /// также средние значения у всех пользователей пользователей, находит ближайших соседей по этим значениям и смотрит
    /// какие у этих соседей общие лайкнутые фильмы.
    /// </summary>
    public class KnnAlgorithm : IFilmSelection
    {
        // Количество ближайших соседей
        private const int k = 3;

        private RandomFilmDBContext db;

        private Account[] accountsCache;
        private Genre[] genresCache;
        private FilmsGenres[] filmsGenresCache;
        private Film[] filmsCache;
        private Like[] likesCache;

        public KnnAlgorithm(RandomFilmDBContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Публичный метод для использования данного алгоритма 
        /// </summary>
        /// <param name="user">Пользователь, под которого подбирается фильм</param>
        /// <returns>Фильм</returns>
        public async Task<List<Film>> GetFilmsAsync(Account user)
        {
            List<Film> result;

            // 0. Вытаскивыние базы в кеш
            accountsCache = await db.Accounts.Include(x => x.Likes)
                                        .ToArrayAsync();
            genresCache = await db.Genres.ToArrayAsync();
            filmsCache = await db.Films.Include(x => x.Likes)
                                    .ToArrayAsync();
            filmsGenresCache = await db.FilmsGenres.ToArrayAsync();
            likesCache = await db.Likes.Include(x => x.Film)
                                    .ToArrayAsync(); ;

            /* 1. Нахождение для каждого пользователя (кроме того, для которого подбираем фильм) 
                 среднего значения в каждой метрике (жанр 1, жанр 2, ..., жанр n)*/

            Dictionary<Account, double[]> vectors = new Dictionary<Account, double[]>();
            for (int i = 0; i < accountsCache.Length; i++)
            {
                if (user.Id == accountsCache[i].Id)
                    continue;
                vectors.Add(accountsCache[i], GetAveregeLikes(accountsCache[i]));
            }

            /* 2. Нахождение для нашего пользователя средних оценок (жанр 1, жанр 2, ..., жанр n)*/
            double[] userVector = GetAveregeLikes(user);

            /* 3. Расчет расстояния других пользователей до нашего пользователя в 
             * многомерной системе жанров, сортировка по дистанциям */
            Dictionary<Account, double> distances = GetDistances(user, userVector, vectors).
                OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            /* 4. Выбор ближайших соседей */
            Dictionary<Account, double> nearestToUser = distances.Take(KnnAlgorithm.k).ToDictionary(x => x.Key, x => x.Value);

            /* 5. Выборка фильма для пользователя */
            result = GetResultFilms(user, nearestToUser);

            return result;
        }

        /// <summary>
        /// Подсчитывает среднюю оценку каждой метрике (все жанры) поставленую конкретным пользователем
        /// </summary>
        /// <param name="account">Пользователь, для которого расчитываются средние значения оценок</param>
        /// <returns></returns>
        private double[] GetAveregeLikes(Account account)
        {
            double[] methrics = new double[genresCache.Length + 1];
            // Посчитываем средние значения оценки для каждого жанра
            for (int i = 0; i < genresCache.Length; i++)
            {
                methrics[i] = GetAveregeLikeInGenre(account, genresCache[i]);
            }
            return methrics;
        }

        /// <summary>
        /// Подсчитывает среднюю оценку конкретного пользователя в конкретном жанре фильмов
        /// </summary>
        /// <param name="account">Пользователь</param>
        /// <param name="genre">Жанр</param>
        /// <returns></returns>
        private double GetAveregeLikeInGenre(Account account, Genre genre)
        {
            double result;
            double summOfLikes = 0;
            double numberOfLikes = 0;

            //Находим фильмы с нужным нам жанром
            FilmsGenres[] filmsGenres = filmsGenresCache.Where(x => x.GenreId == genre.Id).ToArray();
            List<Film> filmsWithNeededGenre = new List<Film>();
            foreach (var item in filmsGenres)
            {
                filmsWithNeededGenre.Add(filmsCache.FirstOrDefault(x => x.Id == item.FilmId));
            }

            //Находим все лайки пользователя фильмам данного жанра
            List<Like> likes = likesCache.Where(x => x.AccountId == account.Id).ToList();
            Like like;
            foreach (var film in filmsWithNeededGenre)
            {
                like = likes.FirstOrDefault(x => x.FilmId == film.Id);
                if (like != null)
                {
                    likes.Add(like);
                }
            }

            /// Ищем средний балл по жанру
            // Ищем число лайков
            numberOfLikes = likes.Count;

            // Ищем сумму лайков
            foreach (var item in likes)
            {
                if (item.LikeOrDislike)
                    summOfLikes++;
                else
                    summOfLikes--;
            }

            // Ищем среднее
            if (numberOfLikes != 0)
                result = summOfLikes / numberOfLikes;
            else
                result = summOfLikes;

            return result;
        }

        /// <summary>
        /// Расчитывает расстояния других пользователей до конкретного в многомерном пространстве лайков
        /// </summary>
        /// <param name="user">Пользователь, расстояние до которого ищется от других пользователей</param>
        /// <param name="userMethrics">Средние значения (метрики) пользователя, до которого исчется расстояния</param>
        /// <param name="methrics">Пары "пользователь-средние оценки" других пользователей</param>
        /// <returns>Пары "пользователь-дистанция до исходного пользователя"</returns>
        private Dictionary<Account, double> GetDistances(Account user, double[] userMethrics, Dictionary<Account, double[]> methrics)
        {
            Dictionary<Account, double> result = new Dictionary<Account, double>();
            //Буферная переменная
            double summDistances = 0;
            for (int i = 0; i < methrics.Count; i++)
            {
                summDistances = 0;
                for (int k = 0; k < methrics.Values.ElementAt(i).Length ; k++)
                {
                    summDistances += (userMethrics[k] - methrics.Values.ElementAt(i).ElementAt(k)) *
                                        (userMethrics[k] - methrics.Values.ElementAt(i).ElementAt(k));
                }
                result.Add(methrics.Keys.ElementAt(i), Math.Sqrt(summDistances));
            }
            return result;
        }

        /// <summary>
        /// Выборка фильмов: среди ближайших соседей, исключая фильмы которые оценивал наш пользователь, состовляется
        /// список фильмов и рейтингов этих фильмов. Рейтинг формируется количеством лайков/дизлайков от соседей.
        /// </summary>
        /// <param name="user">Пользователь, для которого подбирается фильм</param>
        /// <param name="nearestToUser">Ближайшие соседи</param>
        /// <returns>Подобраный фильм</returns>
        private List<Film> GetResultFilms(Account user, Dictionary<Account, double> nearestToUser)
        {
            // Выясняем какие фильмы еще не оценивал (соответственно не смотрел) пользователь (посмотрел)
            Film[] notLikedFilmsByUser = GetNotLikedFilmsByUser(user);

            // Вводим счетчик лайков у этих фильмов ближайшими соседями этого пользователя
            Dictionary<Film, int> filmsLikes = new Dictionary<Film, int>();

            // Проходим по фильмам. На каждой итерации собираем рейтинг фильма соседями
            int rating;
            for (int i = 0; i < notLikedFilmsByUser.Length; i++)
            {
                rating = 0;
                for (int k = 0; k < nearestToUser.Keys.Count; k++)
                {
                    Like like = likesCache.FirstOrDefault(x => (x.FilmId == notLikedFilmsByUser[i].Id) &&
                                    x.AccountId == nearestToUser.Keys.ElementAt(k).Id);
                    if (like != null)
                    {
                        if (like.LikeOrDislike)
                            rating++;

                        else
                            rating--;
                    }
                }
                filmsLikes.Add(notLikedFilmsByUser[i], rating);
            }

            // Сортируем получившийся словарь по убыванию рейтинга и даем первый элемент как результат
            List<Film> result;
            try
            {
                result = filmsLikes.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.ToList();
            }
            catch(System.InvalidOperationException ex)
            {
                result = null;
            }
            return result;

        }

        /// <summary>
        /// Выдает массив фильмов, которым ставил оценки конкретный пользователь
        /// </summary>
        /// <param name="user">конкретный пользовател</param>
        /// <returns>Коллекция оцененных фильмов</returns>
        private Film[] GetLikedFilmsByUser(Account user)
        {
            Like[] likesByUser = likesCache.Where(x => x.AccountId == user.Id).ToArray();
            List<Film> filmsLikedByUser = new List<Film>();
            foreach (var item in likesByUser)
                filmsLikedByUser.Add(filmsCache.FirstOrDefault(x => x.Id == item.FilmId));

            return filmsLikedByUser.ToArray();
        }

        /// <summary>
        /// Выдает массив фильмов, которым НЕ ставил оценки конкретный пользователь
        /// </summary>
        /// <param name="user">конкретный пользовател</param>
        /// <returns>Коллекция оцененных фильмов</returns>
        private Film[] GetNotLikedFilmsByUser(Account user)
        {
            Like[] likesByUser = likesCache.Where(x => x.AccountId == user.Id).ToArray();
            List<Film> filmsNotLikedByUser = new List<Film>(filmsCache);
            foreach (var item in likesByUser)
                filmsNotLikedByUser.Remove(filmsCache.FirstOrDefault(x => x.Id == item.FilmId));

            return filmsNotLikedByUser.ToArray();
        }
    }
}

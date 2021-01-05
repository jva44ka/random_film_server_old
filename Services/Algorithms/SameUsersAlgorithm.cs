using Services.Algorithms.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using System;

namespace Services.Algorithms
{
    /// <summary>
    /// Алгоритм выдачи фильма с учетом предпочтений пользователя
    /// Кратко: алгоритм ищет пользователей с такими же лайкнутыми фильмами. Затем сортирует по количеству совпадений и берет 
    /// какое-то число похожих по вкусу юзеров (k) и смотрит какие у них общие лайкнутые фильмы, которые не лайкнул исходный пользователь.
    /// Если не находит берет рандомный из лайкнутых ближайшими по вкусу юзерами но не лайкнутый пользователем.
    /// </summary>
    public class SameUsersAlgorithm : ISameUsersAlgorithm
    {
        // Количество похожих по вкусу юзеров
        private static readonly int k = 1;

        private IRepository<Account> _accountsRepo;
        private IRepository<Film> _filmsRepo;
        private IRepository<UserFilm> _likesRepo;

        public SameUsersAlgorithm(IRepository<Account> accounts,
                                    IRepository<Film> films,
                                    IRepository<UserFilm> likes)
        {
            _accountsRepo = accounts;
            _filmsRepo = films;
            _likesRepo = likes;
        }

        /// <summary>
        /// Публичный метод для использования данного алгоритма 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<Film>> GetFilms(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Incorrect userId: " + userId);

            List<Film> result;

            /* 1. Нахождение для каждого пользователя общих лайков с нашим пользователем*/
            Dictionary<string, int> usersMatches = GetUsersWithSameLikes(userId);

            /* 2. Сортировка по совпадениям лайков и выбор ближайших по вкусу юзеров */
            Dictionary<string, int> nearestToUser = usersMatches
                .OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value)
                .Take(SameUsersAlgorithm.k).ToDictionary(x => x.Key, x => x.Value);

            /* 3. Выборка фильма для пользователя */
            result = SelectFilms(userId, nearestToUser);

            return result;
        }

        /// <summary>
        /// Находит рейтинги похожести (общие лайкнутые фильмы) для каждого пользователя относительно искомого пользователя
        /// </summary>
        /// <param name="userId">Пользователь, для которого подбирается фильм</param>
        /// <returns>Словарь вида "id пользователя"-"количество общих лайков с пользователем для которого подбирается фильм"</returns>
        private Dictionary<string, int> GetUsersWithSameLikes(string userId)
        {
            // Ищем лайкнутые фильмы пользователя
            List<Guid> userLikesFilmIds = _likesRepo
                .Get()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(ul => ul.FilmId)
                .ToList();

            List<Film> filmsLikedByUser = _filmsRepo
                .Get()
                .AsNoTracking()
                .Where(f => userLikesFilmIds.Contains(f.Id))
                .ToList();

            // Подготовка буферов для поиска совпадений лайков с другими пользователями
            Dictionary<string, int> result = new Dictionary<string, int>();
            int matches = 0;
            List<UserFilm> iterUserLikes;
            UserFilm sameLike;

            // Вытаскиваем из бд для анализа только тех юзеров, которые имеют лайк (и не наш юзер)
            var accountsToAnalysis = _accountsRepo
                .Get()
                .Include(a => a.UserFilms)
                .AsNoTracking()
                .Where(a => a.Id != userId && a.UserFilms.Where(uf => uf.IsLike != null).Any())
                .ToList();

            // Непосредственно поиск совпадений лайков с другими пользователями
            foreach (Account iterUser in accountsToAnalysis)
            {
                iterUserLikes = _likesRepo.Get()
                    .AsNoTracking()
                    .Where(l => l.UserId == iterUser.Id && l.IsLike != null)
                    .ToList();

                foreach(UserFilm iterUserFilm in iterUserLikes)
                {
                    sameLike = iterUserLikes.FirstOrDefault(l => l.FilmId == iterUserFilm.FilmId && l.IsLike == iterUserFilm.IsLike);
                    if (sameLike != null)
                        matches++;
                }

                result.Add(iterUser.Id, matches);
                matches = 0;
            }
            return result;
        }

        /// <summary>
        /// Выборка фильмов: среди ближайших по лайкам юзеров, исключая фильмы которые оценивал наш пользователь, состовляется
        /// список фильмов и рейтингов этих фильмов. Рейтинг формируется количеством лайков/дизлайков от ближайших юзеров.
        /// </summary>
        /// <param name="user">Пользователь, для которого подбирается фильм</param>
        /// <param name="nearestToUser">id ближайших по вкусу юзеров / рейтинг их близости к нашему</param>
        /// <returns>Подобраные фильмы</returns>
        private List<Film> SelectFilms(string userId, Dictionary<string, int> nearestToUser)
        {
            // Выясняем какие фильмы еще не оценивал (соответственно не смотрел) пользователь (посмотрел)
            Film[] notLikedFilmsByUser = GetNotLikedFilmsByUser(userId);

            // Вводим счетчик лайков у этих фильмов ближайшими юзерами к этому пользователю
            Dictionary<Film, int> filmsLikes = new Dictionary<Film, int>();

            // Проходим по фильмам. На каждой итерации собираем рейтинг фильма ближайшими юзерами
            int rating;
            for (int i = 0; i < notLikedFilmsByUser.Length; i++)
            {
                rating = 0;
                for (int k = 0; k < nearestToUser.Keys.Count; k++)
                {
                    UserFilm like = notLikedFilmsByUser[i].Likes
                        .FirstOrDefault(l => l.IsLike != null && l.UserId == nearestToUser.Keys.ElementAt(k));
                    if (like != null && like.IsLike != null)
                    {
                        if ((bool)like.IsLike)
                            rating++;

                        else
                            rating--;
                    }
                }
                filmsLikes.Add(notLikedFilmsByUser[i], rating);
            }

            // Сортируем получившийся словарь по убыванию рейтинга и даем отсортированную коллекцию как результат
            return filmsLikes.OrderByDescending(x => x.Value)
                                .ToDictionary(x => x.Key, x => x.Value)
                                .Keys
                                .ToList();

        }

        /// <summary>
        /// Выдает массив фильмов, которым НЕ ставил оценки конкретный пользователь
        /// </summary>
        /// <param name="userId">конкретный пользовател</param>
        /// <returns>Коллекция оцененных фильмов</returns>
        private Film[] GetNotLikedFilmsByUser(string userId)
        {
            return _filmsRepo
                .Get()
                .Include(f => f.Likes)
                .Include(f => f.Preview) // ДЛЯ РАСЧЕТОВ НЕ НУЖНО И ОЧЕНЬ ЖИРНОЕ
                .AsNoTracking()
                .Where(f => !f.Likes.Where(l => l.UserId == userId && l.IsLike != null).Any())
                .ToArray();
        }
    }
}

using Services.Algorithms.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Services.Algorithms
{
    /// <summary>
    /// Алгоритм выдачи фильма с учетом предпочтений пользователя
    /// Кратко: алгоритм ищет пользователей с такими же лайкнутыми фильмами. Затем сортирует по количеству совпадений и берет 
    /// какое-то число соседей (k) и смотрит какие у этих соседей общие лайкнутые фильмы, которые не лайкнул исходный пользователь.
    /// Если не находит берет рандомный из лайкнутых соседями но не лайкнутый пользователем.
    /// </summary>
    public class SameUsersAlgorithm : IFilmSelector
    {
        // Количество ближайших соседей
        private const int k = 1;

        private Account[] _accountsCache;
        private Film[] _filmsCache;
        private UserFilm[] _likesCache;

        private IRepository<Account> _accountsRepo;
        private IRepository<Film> _filmsRepo;
        private IRepository<UserFilm> _likesRepo;

        public SameUsersAlgorithm(IRepository<Account> accounts,
                                    IRepository<Film> films,
                                    IRepository<UserFilm> likes)
        {
            this._accountsRepo = accounts;
            this._filmsRepo = films;
            this._likesRepo = likes;
        }

        /// <summary>
        /// Публичный метод для использования данного алгоритма 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<Film>> GetFilmsAsync(Account user)
        {
            List<Film> result;

            // 0. Вытаскивыние базы в кеш
            _accountsCache = await _accountsRepo.Get()
                                        .Include(x => x.UserFilms)
                                        .ToArrayAsync();
            _filmsCache = await _filmsRepo.Get()
                                    .Include(x => x.Likes)
                                    .Include(x => x.FilmsGenres)
                                        .ThenInclude(x => x.Genre)
                                    .Include(x => x.Preview)
                                    .ToArrayAsync();
            _likesCache = await _likesRepo.Get().Include(x => x.Film)
                                                    .Include(x => x.User)
                                                    .ToArrayAsync();

            /* 1. Нахождение для каждого пользователя общих лайков с нашим пользователем*/
            Dictionary<Account, int> usersMatches = GetUsersWithSameLakes(user);

            /* 2. Сортировка по совпадениям лайков и выбор ближайших соседей */
            Dictionary<Account, int> nearestToUser = usersMatches
                .OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value)
                .Take(SameUsersAlgorithm.k).ToDictionary(x => x.Key, x => x.Value);

            /* 3. Выборка фильма для пользователя */
            result = SelectFilms(user, nearestToUser);

            /* 4. Удаление из переменных класса ссылок на объекты таблиц для удаления сборщиком мусора*/
            _accountsCache = null;
            _filmsCache = null;
            _likesCache = null;

            return result;
        }

        /// <summary>
        /// Находит рейтинги похожести (общие лайкнутые фильмы) для каждого пользователя относительно искомого пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого подбирается фильм</param>
        /// <returns>Словарь вида "пользователь"-"количество общих лайков с пользователем для которого подбирается фильм"</returns>
        private Dictionary<Account, int> GetUsersWithSameLakes(Account user)
        {
            // Ищем лайкнутые фильмы пользователя
            UserFilm[] userLikes = _likesCache.Where(x => x.User.Id == user.Id).ToArray();
            List<Film> filmsLikedByUser = new List<Film>();
            for (int i = 0; i < userLikes.Length; i++)
            {
                filmsLikedByUser.Add(_filmsCache.FirstOrDefault(x => x.Id == userLikes[i].Film.Id));
            }

            // Ищем совпадения лайков с другими пользователями
            Dictionary<Account, int> result = new Dictionary<Account, int>();
            int matches = 0;
            UserFilm sameLike;
            for (int i = 0; i < _accountsCache.Length; i++)
            {
                if (_accountsCache[i].Id == user.Id)
                    continue;
                for (int k = 0; k < filmsLikedByUser.Count; k++)
                {
                    sameLike = _likesCache.FirstOrDefault(x => (x.User.Id == _accountsCache[i].Id) && (x.Film.Id == filmsLikedByUser[k].Id));
                    if ((sameLike != null) && (sameLike.IsLike == userLikes[k].IsLike))
                        matches++;
                }
                result.Add(_accountsCache[i], matches);
                matches = 0;
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
        private List<Film> SelectFilms(Account user, Dictionary<Account, int> nearestToUser)
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
                    UserFilm like = _likesCache.FirstOrDefault(x => (x.Film.Id == notLikedFilmsByUser[i].Id) &&
                                    (x.User.Id == nearestToUser.Keys.ElementAt(k).Id));
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

            // Сортируем получившийся словарь по убыванию рейтинга и даем первый элемент как результат
            List<Film> result;
            try
            {
                result = filmsLikes.OrderByDescending(x => x.Value)
                                    .ToDictionary(x => x.Key, x => x.Value)
                                    .Keys
                                    .ToList();
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
            UserFilm[] likesByUser = _likesCache.Where(x => x.User.Id == user.Id).ToArray();
            List<Film> filmsLikedByUser = new List<Film>();
            foreach (var item in likesByUser)
                filmsLikedByUser.Add(_filmsCache.FirstOrDefault(x => x.Id == item.Film.Id));

            return filmsLikedByUser.ToArray();
        }

        /// <summary>
        /// Выдает массив фильмов, которым НЕ ставил оценки конкретный пользователь
        /// </summary>
        /// <param name="user">конкретный пользовател</param>
        /// <returns>Коллекция оцененных фильмов</returns>
        private Film[] GetNotLikedFilmsByUser (Account user)
        {
            UserFilm[] likesByUser = _likesCache.Where(x => x.User.Id == user.Id).ToArray();
            List<Film> filmsNotLikedByUser = new List<Film>(_filmsCache);
            foreach (var item in likesByUser)
                filmsNotLikedByUser.Remove(_filmsCache.FirstOrDefault(x => x.Id == item.Film.Id));

            return filmsNotLikedByUser.ToArray();
        }
    }
}

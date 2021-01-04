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
    public class PopularFilmsAlgorithm : IPopularFilmsAlgorithm
    {
        private readonly IRepository<Film> _filmsRepo;
        private readonly IRepository<Account> _accountRepository;

        public PopularFilmsAlgorithm(IRepository<Film> filmsRepo, IRepository<Account> accountRepository)
        {
            _filmsRepo = filmsRepo;
            _accountRepository = accountRepository;
        }
        public async Task<IList<Film>> GetFilms(string userId)
        {
            var films = _filmsRepo.Get().Include(f => f.Likes).OrderByDescending(f => f.Likes.Count).AsQueryable();
            var user = await _accountRepository.Get().AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
                films = films.Where(f => f.Likes.FirstOrDefault(l => l.UserId == userId && l.IsLike != null) == null);

            return await films.ToListAsync();
        }
    }
}

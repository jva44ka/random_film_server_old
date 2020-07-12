using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Infrastructure.Algorithms.Interfaces
{
    public interface IFilmSelector
    {
        Task<List<Film>> GetFilmsAsync(Account user);
    }
}

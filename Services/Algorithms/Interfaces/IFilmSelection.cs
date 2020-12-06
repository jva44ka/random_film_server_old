using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Services.Algorithms.Interfaces
{
    public interface IFilmSelector
    {
        Task<IEnumerable<Film>> GetFilms(string userId = null);
    }
}

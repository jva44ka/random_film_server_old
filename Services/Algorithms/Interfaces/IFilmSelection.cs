using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Algorithms.Interfaces
{
    public interface IFilmSelector
    {
        Task<IList<Guid>> GetFilmIds(string userId = null);
    }
}

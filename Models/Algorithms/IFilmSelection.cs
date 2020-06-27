using Microsoft.EntityFrameworkCore;
using randomfilm_backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace randomfilm_backend.Models.Algorithms
{
    interface IFilmSelection
    {
        public Task<List<Film>> GetFilmsAsync(Account user);
    }
}

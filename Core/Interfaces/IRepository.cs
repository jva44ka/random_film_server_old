using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(Guid id, bool asUnmodified = false);
        Task<T> GetByIdAsync(Guid id, bool asUnmodified = false);
        Task<T> GetByIdAsync(string id, bool asUnmodified = false);
        T Create(T item);
        void CreateRange(IEnumerable<T> items);
        Task<T> CreateAsync(T item);
        Task CreateRangeAsync(IEnumerable<T> item);
        T Update(T item);
        bool Delete(Guid id);
        void Delete(T item);
        void Save();
        Task SaveAsync();
    }
}

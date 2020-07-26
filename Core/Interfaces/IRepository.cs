using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Get();
        T GetById(Guid id, bool asUnmodified = false);
        T GetById(string id);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(string id);
        T Create(T item);
        void CreateRange(IEnumerable<T> items);
        Task<T> CreateAsync(T item);
        Task CreateRangeAsync(IEnumerable<T> items);
        T Update(T item);
        bool Delete(Guid id);
        void Delete(T item);
        void DeleteRange(IEnumerable<T> items);
        void Save();
        Task SaveAsync();
    }
}

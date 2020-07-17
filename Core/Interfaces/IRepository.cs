using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
        T GetById(Guid id, bool asUnmodified = false);
        Task<T> GetByIdAsync(Guid id, bool asUnmodified = false);
        Task<T> GetByIdAsync(string id, bool asUnmodified = false);
        void CreateRange(IEnumerable<T> items);
        T Create(T item);
        Task<T> CreateAsync(T item);
        Task CreateRangeAsync(IEnumerable<T> items);
        T Update(T item);
        bool Delete(Guid id);
        void Delete(T item);
        void Save();
        Task SaveAsync();
    }
}

using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IDataModel
    {
        protected readonly DbMainContext _context;
        protected DbSet<TEntity> DbEntities { get; }

        public GenericRepository(DbMainContext context)
        {
            _context = context;
            DbEntities = context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbEntities.AsNoTracking();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression)
        {
            return DbEntities.AsNoTracking().Where(expression);
        }

        public TEntity GetById(Guid id, bool asUnmodified = false)
        {
            var result = DbEntities.Find(id);
            if (asUnmodified)
                _context.Entry(result).State = EntityState.Unchanged;
            return result;
        }

        public async Task<TEntity> GetByIdAsync(Guid id, bool asUnmodified = false)
        {
            var result = await DbEntities.FindAsync(id);
            if (asUnmodified)
                _context.Entry(result).State = EntityState.Unchanged;
            return result;
        }

        public async Task<TEntity> GetByIdAsync(string id, bool asUnmodified = false)
        {
            var result = await DbEntities.FindAsync(id);
            if (asUnmodified)
                _context.Entry(result).State = EntityState.Unchanged;
            return result;
        }

        public void CreateRange(IEnumerable<TEntity> items)
        {
            DbEntities.AddRange(items);
        }

        public TEntity Create(TEntity item)
        {
            return DbEntities.Add(item).Entity;
        }

        public async Task<TEntity> CreateAsync(TEntity item)
        {
            return (await DbEntities.AddAsync(item)).Entity;
        }
        public async Task CreateRangeAsync(IEnumerable<TEntity> items)
        {
            await DbEntities.AddRangeAsync(items);
        }

        public TEntity Update(TEntity item)
        {
            var result = DbEntities.Update(item).Entity;
            //_context.Entry(item).State = EntityState.Modified;
            return result;
        }

        public bool Delete(Guid id)
        {
            var item = GetById(id);
            if (item == null)
                return false;
            Delete(item);
            return true;
        }

        public void Delete(TEntity item)
        {
            DbEntities.Remove(item);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

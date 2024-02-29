using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbset;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbset.FindAsync(id);

            if(entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public T Updade(T entity)
        {
           _context.Entry(entity).State |= EntityState.Modified;
            return entity; 
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate);
        }
    }
}

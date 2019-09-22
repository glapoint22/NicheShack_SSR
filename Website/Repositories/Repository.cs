using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Website.Interfaces;

namespace Website.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        // Set the context
        private readonly DbContext context;
        public Repository(DbContext context)
        {
            this.context = context;
        }



        // Get overloads
        public async Task<T> Get(int id)
        {
            return await context.Set<T>()
                .FindAsync(id);
        }

        public async Task<T> Get(string id)
        {
            return await context.Set<T>()
                .FindAsync(id);
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public async Task<TOut> Get<TOut>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select(select)
                .FirstOrDefaultAsync();
        }

        public async Task<TOut> Get<TOut>(Expression<Func<T, bool>> predicate, ISelect<T, TOut> dto) where TOut : class
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select(dto.SetSelect())
                .FirstOrDefaultAsync();
        }





        // GetCollection overloads
        public async Task<IEnumerable<TOut>> GetCollection<TOut>(ISelect<T, TOut> dto) where TOut : class
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Select(dto.SetSelect())
                .ToListAsync();
        }

        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, ISelect<T, TOut> dto) where TOut : class
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select(dto.SetSelect())
                .ToListAsync();
        }

        public async Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select)
        {
            return await context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .Select(select)
                .ToListAsync();
        }




        public async Task<IEnumerable<T>> GetCollection()
        {
            return await context.Set<T>().ToListAsync();
                
        }




        // Add
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }



        // Update
        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }



        // Remove
        public void Remove(T entity)
        {
            context.Set<T>().Remove(entity);
        }


        // Any
        public async Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }
    }
}

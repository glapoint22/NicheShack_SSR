using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Website.Interfaces;

namespace Website.Repositories
{
    public interface IRepository<T> where T: class
    {
        // Get overloads
        Task<T> Get(int id);
        Task<T> Get(string id);
        Task<T> Get(Expression<Func<T, bool>> predicate);
        Task<TOut> Get<TOut>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select);
        Task<TOut> Get<TOut>(Expression<Func<T, bool>> predicate, ISelect<T, TOut> dto) where TOut : class;



        // GetCollection overloads
        Task<IEnumerable<TOut>> GetCollection<TOut>(ISelect<T, TOut> dto) where TOut : class;
        Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, ISelect<T, TOut> dto) where TOut : class;
        Task<IEnumerable<TOut>> GetCollection<TOut>(Expression<Func<T, bool>> predicate, Expression<Func<T, TOut>> select);
        

        // Add
        void Add(T entity);

        // Update
        void Update(T entity);


        // Remove
        void Remove(T entity);


        // Any
        Task<bool> Any(Expression<Func<T, bool>> predicate);
    }
}

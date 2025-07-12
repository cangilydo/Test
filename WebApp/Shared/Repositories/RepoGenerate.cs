using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Shared.EF;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Shared.Repositories
{
    public interface IRepoGenerate<T>
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includeProperties);
        Task UpdateAsync(T entity);
        bool Any(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
    }
    public abstract class RepoGenerate<T> : IRepoGenerate<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        protected readonly AppDbContext _dbContext;
        private IDbContextTransaction _transaction;
        public RepoGenerate(AppDbContext context)
        {            
            _dbSet = context.Set<T>();
            _dbContext = context;
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }
        public virtual async Task AddAsync(T entity)
        {
            _dbSet.Entry(entity).State = EntityState.Added;

            await _dbContext.SaveChangesAsync();
        }
        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
                        
            await _dbContext.SaveChangesAsync();            
        }
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (predicate == null)
            {
                return query.FirstOrDefault();
            }
            return query.FirstOrDefault(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public virtual void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = _dbContext.Database.BeginTransaction();
            }
        }

        public virtual void Commit()
        {
            try
            {
                _dbContext.SaveChanges();
                _transaction?.Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public virtual void Rollback()
        {
            _transaction?.Rollback();
            DisposeTransaction();
        }

        private void DisposeTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

}

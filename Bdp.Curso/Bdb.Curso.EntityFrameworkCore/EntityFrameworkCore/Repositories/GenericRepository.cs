namespace Bdb.Curso.EntityFrameworkCore.Repositories
{
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using System.Linq.Expressions;

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            }
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate, bool asNoTracking = true)
        {
            var query = _dbSet.Where(predicate);
            return asNoTracking ? query.AsNoTracking() : query;
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public IQueryable<TResult> GroupBy<TKey, TResult>(
            Expression<Func<T, TKey>> keySelector,
            Expression<Func<IGrouping<TKey, T>, TResult>> resultSelector)
        {
            return _dbSet.GroupBy(keySelector).Select(resultSelector);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ? await _dbSet.AnyAsync() : await _dbSet.AnyAsync(predicate);
        }

        public async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
        {
            return await _dbSet.SumAsync(selector);
        }

        public async Task<T> MaxAsync(Expression<Func<T, decimal>> selector)
        {
            return await _dbSet.OrderByDescending(selector).FirstOrDefaultAsync();
        }

        public async Task<T> MinAsync(Expression<Func<T, decimal>> selector)
        {
            return await _dbSet.OrderBy(selector).FirstOrDefaultAsync();
        }

        public IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return _dbSet.Select(selector);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true)
        {
            var query = _dbSet.Where(predicate);
            return asNoTracking ? await query.AsNoTracking().FirstOrDefaultAsync() : await query.FirstOrDefaultAsync();
        }


        // Método para ejecutar procedimientos almacenados que no devuelven resultados
        public async Task ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] parameters)
        {
            await _context.Database.ExecuteSqlRawAsync(procedureName, parameters);
        }

        // Método para ejecutar procedimientos almacenados que devuelven resultados
        public async Task<List<T>> ExecuteStoredProcedureWithResultsAsync(string procedureName,
            params SqlParameter[] parameters)
        {                                    
            return await _context.Set<T>().FromSqlRaw(procedureName, parameters).ToListAsync();
        }




    }

}

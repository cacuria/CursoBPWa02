namespace Bdb.Curso.EntityFrameworkCore
{
    using Microsoft.Data.SqlClient;
    using System.Linq.Expressions;

    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        // Soporte para búsquedas con Where
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, bool asNoTracking = true);

        // Soporte para incluir entidades relacionadas
        IQueryable<T> Include(params Expression<Func<T, object>>[] includes);

        // Soporte para agrupaciones
        IQueryable<TResult> GroupBy<TKey, TResult>(
            Expression<Func<T, TKey>> keySelector,
            Expression<Func<IGrouping<TKey, T>, TResult>> resultSelector);

        // Soporte para conteo de elementos
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        // Verificar si existen registros con Any
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null);

        // Operaciones de agregación
        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector);
        Task<T> MaxAsync(Expression<Func<T, decimal>> selector);
        Task<T> MinAsync(Expression<Func<T, decimal>> selector);

        // Proyecciones personalizadas
        IQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        // Soporte para obtener un solo elemento
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true);



        Task<List<T>> ExecuteStoredProcedureWithResultsAsync(string procedureName, params SqlParameter[] parameters);
        Task ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] parameters);





    }

}

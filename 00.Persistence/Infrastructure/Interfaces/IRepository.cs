using System.Linq.Expressions;

namespace Infrastructure.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    void Delete(object id);

    void Delete(TEntity entityToDelete);

    TEntity? FindSingleBy(
        Expression<Func<TEntity, bool>>? filter = null);

    IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");

    Task<IEnumerable<TEntity>> GetAll();

    IEnumerable<TEntity>? GetAllBy(Expression<Func<TEntity, bool>>? filter = null);

    TEntity? GetByID(object? id);

    void Insert(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);

    void Update(TEntity entityToUpdate);

    void Update(object id, TEntity entityToUpdate);
}

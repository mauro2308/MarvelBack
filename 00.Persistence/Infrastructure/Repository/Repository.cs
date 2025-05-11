using System.Linq.Expressions;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class Repository<TEntity>(AppDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    internal AppDbContext Context = context;
    internal DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "")
    {
        IQueryable<TEntity> query = DbSet;

        try
        {
            if (filter != null) query = query.Where(filter);

            query = includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex.InnerException);
        }
    }

    public virtual TEntity? FindSingleBy(
        Expression<Func<TEntity, bool>>? filter = null)
    {
        TEntity? query = null;
        if (filter != null) query = DbSet.Where(filter).SingleOrDefault();

        return query;
    }

    public virtual IEnumerable<TEntity>? GetAllBy(
        Expression<Func<TEntity, bool>>? filter = null)
    {
        IEnumerable<TEntity>? query = null;
        if (filter != null) query = DbSet.Where(filter);

        return query;
    }

    public virtual TEntity? GetByID(object? id)
    {
        return DbSet.Find(id);
    }

    public virtual void Insert(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
        var entityToDelete = DbSet.Find(id);
        if (entityToDelete != null) Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (Context.Entry(entityToDelete).State == EntityState.Detached) DbSet.Attach(entityToDelete);

        DbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        DbSet.Update(entityToUpdate);
        Context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public virtual void Update(object id, TEntity entityToUpdate)
    {
        var entity = DbSet.Find(id);
        Context.Entry(entity).CurrentValues.SetValues(entityToUpdate);
        if (entity != null) Update(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        return await DbSet.ToListAsync();
    }
}

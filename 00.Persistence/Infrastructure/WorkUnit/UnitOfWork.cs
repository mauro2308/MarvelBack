using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.WorkUnit;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;
    private IDbContextTransaction _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<T>? Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T))) return _repositories[typeof(T)] as IRepository<T>;

        IRepository<T>? repo = new Repository<T>(_context);
        _repositories.Add(typeof(T), repo);
        return repo;
    }

    public bool Save<T>() where T : class
    {
        var returnValue = true;
        try
        {
            var entries = _context.ChangeTracker.Entries<T>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added ||
                            e.State == EntityState.Deleted);

            foreach (var entry in entries) entry.State = entry.State;

            _context.SaveChanges();
        }
        catch (Exception)
        {
            returnValue = false;
        }

        return returnValue;
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
        if (_transaction != null)
            await _transaction.CommitAsync();
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            _transaction?.Dispose();
        }

        _disposed = true;
    }
}

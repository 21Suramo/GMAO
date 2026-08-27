using System.Collections.Concurrent;
using GMAO.Application.Common.Interfaces;
using GMAO.Domain.Common;
using GMAO.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace GMAO.Persistence.Repositories;

/// <summary>
/// Implémentation de l'unité de travail : partage un même <see cref="AppDbContext"/>
/// entre les dépôts et gère les transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IRepository<T> Repository<T>() where T : EntiteBase
        => (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;
        await _context.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _transaction?.Dispose();
        _context.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

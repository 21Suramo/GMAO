using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Domain.Common;
using GMAO.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GMAO.Persistence.Repositories;

/// <summary>
/// Implémentation EF Core générique du Repository Pattern.
/// </summary>
public class Repository<T> : IRepository<T> where T : EntiteBase
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _set;

    public Repository(AppDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _set.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _set.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.AnyAsync(predicate, cancellationToken);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null
            ? await _set.CountAsync(cancellationToken)
            : await _set.CountAsync(predicate, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _set.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public IQueryable<T> Query() => _set.AsQueryable();

    public async Task<IReadOnlyList<TResultat>> ListerAsync<TResultat>(
        Expression<Func<T, bool>>? filtre,
        Expression<Func<T, TResultat>> projection,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> requete = _set.AsNoTracking();
        if (filtre is not null)
            requete = requete.Where(filtre);

        return await requete.Select(projection).ToListAsync(cancellationToken);
    }
}

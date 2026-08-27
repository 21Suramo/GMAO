using GMAO.Domain.Common;

namespace GMAO.Application.Common.Interfaces;

/// <summary>
/// Unité de travail (Unit of Work) : coordonne les dépôts et valide les changements
/// au sein d'une même transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Retourne le dépôt générique pour le type d'entité demandé.</summary>
    IRepository<T> Repository<T>() where T : EntiteBase;

    /// <summary>Persiste tous les changements en attente.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Démarre une transaction explicite.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Valide la transaction courante.</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>Annule la transaction courante.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

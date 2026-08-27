using System.Linq.Expressions;
using GMAO.Domain.Common;

namespace GMAO.Application.Common.Interfaces;

/// <summary>
/// Dépôt générique (Repository Pattern) pour l'accès aux entités persistées.
/// Abstrait la technologie de persistance vis-à-vis de la couche Application.
/// </summary>
/// <typeparam name="T">Type d'entité héritant de <see cref="EntiteBase"/>.</typeparam>
public interface IRepository<T> where T : EntiteBase
{
    /// <summary>Récupère une entité par son identifiant, ou null si absente.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Récupère toutes les entités.</summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Récupère les entités satisfaisant un prédicat.</summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Récupère la première entité satisfaisant un prédicat, ou null.</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Indique s'il existe au moins une entité satisfaisant le prédicat.</summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Compte les entités satisfaisant le prédicat (toutes si null).</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>Ajoute une entité au contexte.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Marque une entité comme modifiée.</summary>
    void Update(T entity);

    /// <summary>Marque une entité pour suppression.</summary>
    void Remove(T entity);

    /// <summary>Expose un IQueryable pour les requêtes composées (recherche multicritère, projections).</summary>
    IQueryable<T> Query();

    /// <summary>
    /// Exécute une projection typée (avec filtre optionnel) côté base de données.
    /// Permet de matérialiser des DTO sans exposer EF Core à la couche Application.
    /// </summary>
    Task<IReadOnlyList<TResultat>> ListerAsync<TResultat>(
        Expression<Func<T, bool>>? filtre,
        Expression<Func<T, TResultat>> projection,
        CancellationToken cancellationToken = default);
}

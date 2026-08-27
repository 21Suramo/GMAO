using GMAO.Application.DTOs;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Cas d'usage liés au parc des respirateurs.</summary>
public interface IRespirateurService
{
    /// <summary>Liste tous les respirateurs du parc.</summary>
    Task<IReadOnlyList<RespirateurDto>> ListerAsync(CancellationToken cancellationToken = default);

    /// <summary>Obtient la fiche d'un respirateur.</summary>
    Task<RespirateurDto?> ObtenirAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Déclare un respirateur « hors service » (RG-03) avec motif et auteur.</summary>
    Task<Result> DeclarerHorsServiceAsync(int id, string motif, string auteur, CancellationToken cancellationToken = default);

    /// <summary>Remet un respirateur en service.</summary>
    Task<Result> RemettreEnServiceAsync(int id, string auteur, CancellationToken cancellationToken = default);
}

using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Cas d'usage liés aux pièces détachées et au stock.</summary>
public interface IPieceService
{
    /// <summary>Liste les pièces du stock.</summary>
    Task<IReadOnlyList<PieceDto>> ListerAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enregistre un mouvement de stock (entrée, sortie, ajustement) et met à jour le stock.
    /// </summary>
    Task<Result> EnregistrerMouvementAsync(int pieceId, TypeMouvement type, int quantite, string? motif, string auteur, CancellationToken cancellationToken = default);
}

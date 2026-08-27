using GMAO.Application.DTOs;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Fournit les statistiques avancées (analyses multi-mois, comparaisons).</summary>
public interface IStatistiquesService
{
    /// <summary>
    /// Calcule les statistiques avancées sur la fenêtre demandée.
    /// Réservé aux rôles disposant de la vue globale (<c>ConsulterTableauBordGlobal</c>).
    /// </summary>
    Task<Result<StatistiquesDto>> ObtenirAsync(StatistiquesFiltre filtre, CancellationToken cancellationToken = default);
}

using GMAO.Application.DTOs;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Fournit les indicateurs agrégés du tableau de bord.</summary>
public interface ITableauBordService
{
    /// <summary>
    /// Calcule les indicateurs pour la période et le périmètre demandés.
    /// Exige la permission <c>ConsulterTableauBord</c> (et <c>ConsulterTableauBordGlobal</c> pour la vue globale).
    /// </summary>
    Task<Result<TableauBordDto>> ObtenirAsync(TableauBordFiltre filtre, CancellationToken cancellationToken = default);
}

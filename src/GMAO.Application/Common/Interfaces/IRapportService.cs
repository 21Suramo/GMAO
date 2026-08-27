using GMAO.Application.DTOs;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Génération et archivage des rapports d'intervention.</summary>
public interface IRapportService
{
    /// <summary>
    /// Génère le rapport PDF d'une intervention, l'enregistre sur disque et l'archive.
    /// </summary>
    /// <returns>Le chemin du fichier PDF généré.</returns>
    Task<Result<string>> GenererRapportInterventionAsync(int interventionId, CancellationToken cancellationToken = default);

    /// <summary>Liste les interventions et l'état de leur rapport PDF (récentes d'abord).</summary>
    Task<IReadOnlyList<RapportDto>> ListerAsync(CancellationToken cancellationToken = default);
}

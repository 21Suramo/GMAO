using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Affectation (automatique et manuelle) des interventions aux ingénieurs.</summary>
public interface IAffectationService
{
    /// <summary>
    /// Choisit le meilleur ingénieur disponible pour intervenir sur un respirateur à une date donnée.
    /// Action système appelée pendant la création d'une DI critique (non soumise à autorisation).
    /// </summary>
    /// <returns>L'identifiant de l'ingénieur retenu, ou null si aucun n'est disponible.</returns>
    Task<int?> ChoisirIngenieurAsync(int respirateurId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Affecte (ou réaffecte) manuellement une intervention à un ingénieur donné, ou à défaut au
    /// meilleur candidat proposé par le moteur. Exige la permission <c>AffecterIntervention</c>.
    /// </summary>
    Task<Result> AffecterAsync(int interventionId, int? ingenieurId, string auteur, CancellationToken cancellationToken = default);
}

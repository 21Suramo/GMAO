using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Cas d'usage liés aux interventions de maintenance corrective.</summary>
public interface IInterventionService
{
    /// <summary>Liste toutes les interventions (récentes d'abord).</summary>
    Task<IReadOnlyList<InterventionDto>> ListerAsync(CancellationToken cancellationToken = default);

    /// <summary>Obtient le détail complet d'une intervention.</summary>
    Task<InterventionDetailDto?> ObtenirAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée une intervention à partir d'une déclaration.
    /// Applique RG-01 : si patient connecté ⇒ priorité Critique, état Affectée, affectation automatique.
    /// </summary>
    Task<Result<InterventionDto>> CreerAsync(CreerInterventionRequete requete, string auteur, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change l'état d'une intervention (workflow) et journalise la transition.
    /// Le passage à « Clôturée » exige une check-list complète (RG-02).
    /// </summary>
    Task<Result> ChangerEtatAsync(int id, EtatIntervention nouvelEtat, string auteur, CancellationToken cancellationToken = default);

    /// <summary>Met à jour la check-list de clôture d'une intervention.</summary>
    Task<Result> MettreAJourCheckListAsync(int id, CheckListDto checkList, CancellationToken cancellationToken = default);

    /// <summary>Liste les symptômes prédéfinis déclarables.</summary>
    Task<IReadOnlyList<SymptomeDto>> ListerSymptomesAsync(CancellationToken cancellationToken = default);
}

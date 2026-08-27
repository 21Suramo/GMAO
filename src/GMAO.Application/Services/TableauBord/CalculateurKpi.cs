using GMAO.Domain.Enums;

namespace GMAO.Application.Services.TableauBord;

/// <summary>
/// Calculs d'indicateurs (MTTR, délai d'affectation, disponibilité, SLA) isolés dans une classe
/// pure et sans effet de bord, afin d'être unitairement testables indépendamment de l'accès aux données.
/// </summary>
public static class CalculateurKpi
{
    /// <summary>Seuils SLA (en heures) de résolution attendue selon la priorité.</summary>
    public static double SeuilSlaHeures(Priorite priorite) => priorite switch
    {
        Priorite.Critique => 4,
        Priorite.Haute => 24,
        Priorite.Normale => 72,
        Priorite.Basse => 168,
        _ => 72
    };

    /// <summary>
    /// MTTR : moyenne, en heures, du délai création → clôture sur les interventions clôturées.
    /// Renvoie 0 si aucune intervention clôturée.
    /// </summary>
    public static double Mttr(IEnumerable<(DateTime Creation, DateTime? Cloture)> interventions)
    {
        var delais = interventions
            .Where(i => i.Cloture.HasValue && i.Cloture.Value >= i.Creation)
            .Select(i => (i.Cloture!.Value - i.Creation).TotalHours)
            .ToList();

        return delais.Count == 0 ? 0d : Math.Round(delais.Average(), 1);
    }

    /// <summary>
    /// Délai moyen d'affectation, en heures : moyenne du délai création → prise en charge
    /// (première affectation). Renvoie 0 si aucune affectation.
    /// </summary>
    public static double DelaiMoyenAffectation(IEnumerable<(DateTime Creation, DateTime? Affectation)> interventions)
    {
        var delais = interventions
            .Where(i => i.Affectation.HasValue && i.Affectation.Value >= i.Creation)
            .Select(i => (i.Affectation!.Value - i.Creation).TotalHours)
            .ToList();

        return delais.Count == 0 ? 0d : Math.Round(delais.Average(), 1);
    }

    /// <summary>
    /// Taux de disponibilité (%) d'un équipement sur la période : part de temps non immobilisé,
    /// estimée à partir du temps total d'intervention (déplacement + réparation).
    /// </summary>
    public static double DisponibilitePourcent(int immobilisationMinutes, double periodeMinutes)
    {
        if (periodeMinutes <= 0) return 100d;
        var indispo = Math.Min(100d, immobilisationMinutes / periodeMinutes * 100d);
        return Math.Round(100d - indispo, 1);
    }

    /// <summary>
    /// Indique si une intervention ouverte a dépassé son délai SLA compte tenu de sa priorité
    /// et de son ancienneté (âge en heures depuis la création).
    /// </summary>
    public static bool EstEnDepassementSla(Priorite priorite, double ageHeures)
        => ageHeures > SeuilSlaHeures(priorite);
}

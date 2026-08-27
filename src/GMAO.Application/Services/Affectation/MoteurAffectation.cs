namespace GMAO.Application.Services.Affectation;

/// <summary>Période d'indisponibilité (congé) d'un ingénieur.</summary>
public class Periode
{
    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }
}

/// <summary>Candidat à l'affectation d'une intervention.</summary>
public class IngenieurCandidat
{
    public int Id { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string? Zone { get; set; }

    /// <summary>Disponible à la date cible (hors congé et marqué disponible).</summary>
    public bool EstDisponible { get; set; }

    /// <summary>Modèles maîtrisés par l'ingénieur.</summary>
    public IReadOnlyList<string> CompetencesModeles { get; set; } = Array.Empty<string>();

    /// <summary>Nombre d'interventions en cours (équilibrage de charge).</summary>
    public int NbInterventionsOuvertes { get; set; }
}

/// <summary>Contexte d'une demande d'affectation.</summary>
public class ContexteAffectation
{
    public string ModeleNom { get; set; } = string.Empty;
    public string? VilleHopital { get; set; }
}

/// <summary>
/// Moteur d'affectation automatique (Strategy) : choisit le meilleur ingénieur
/// disponible selon ses compétences, sa zone géographique et sa charge.
/// </summary>
public static class MoteurAffectation
{
    public const int PointsCompetence = 50;
    public const int PointsZone = 30;
    public const int PenaliteParInterventionOuverte = 5;

    /// <summary>Calcule le score d'adéquation d'un candidat (plus élevé = meilleur).</summary>
    public static int Score(IngenieurCandidat candidat, ContexteAffectation contexte)
    {
        var score = 0;

        if (candidat.CompetencesModeles.Any(m => string.Equals(m, contexte.ModeleNom, StringComparison.OrdinalIgnoreCase)))
            score += PointsCompetence;

        if (!string.IsNullOrWhiteSpace(contexte.VilleHopital)
            && string.Equals(candidat.Zone, contexte.VilleHopital, StringComparison.OrdinalIgnoreCase))
            score += PointsZone;

        score -= candidat.NbInterventionsOuvertes * PenaliteParInterventionOuverte;
        return score;
    }

    /// <summary>
    /// Sélectionne le meilleur ingénieur disponible, ou null si aucun n'est disponible.
    /// </summary>
    public static IngenieurCandidat? Choisir(IEnumerable<IngenieurCandidat> candidats, ContexteAffectation contexte)
        => candidats
            .Where(c => c.EstDisponible)
            .OrderByDescending(c => Score(c, contexte))
            .ThenBy(c => c.NbInterventionsOuvertes)
            .ThenBy(c => c.Id)
            .FirstOrDefault();
}

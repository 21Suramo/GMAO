namespace GMAO.Application.DTOs;

/// <summary>
/// Filtre du tableau de bord : période analysée et périmètre (global ou personnel).
/// </summary>
public class TableauBordFiltre
{
    /// <summary>Début de la période (inclus). Par défaut : 30 jours avant la fin.</summary>
    public DateTime? Debut { get; set; }

    /// <summary>Fin de la période (inclus). Par défaut : maintenant.</summary>
    public DateTime? Fin { get; set; }

    /// <summary>
    /// Si renseigné, restreint les indicateurs aux interventions de cet ingénieur
    /// (vue personnelle d'un technicien / ingénieur).
    /// </summary>
    public int? IngenieurId { get; set; }

    /// <summary>Vrai pour la vue globale (responsable/administrateur), faux pour la vue personnelle.</summary>
    public bool VueGlobale { get; set; } = true;
}

/// <summary>Indicateurs synthétiques affichés sur le tableau de bord.</summary>
public class TableauBordDto
{
    /// <summary>Libellé de la période analysée (ex. « 14/06/2026 → 14/07/2026 »).</summary>
    public string PeriodeLibelle { get; set; } = string.Empty;

    /// <summary>Périmètre affiché (« Vue globale » ou « Mon activité »).</summary>
    public string PerimetreLibelle { get; set; } = "Vue globale";

    // --- Parc ---
    public int NombreRespirateurs { get; set; }
    public int RespirateursEnService { get; set; }
    public int RespirateursHorsService { get; set; }

    /// <summary>Disponibilité globale du parc en pourcentage (part en service).</summary>
    public double DisponibiliteGlobale { get; set; }

    // --- Flux d'interventions ---
    public int InterventionsActives { get; set; }
    public int EnAttenteAffectation { get; set; }
    public int InterventionsCloturees { get; set; }
    public int InterventionsUrgentes { get; set; }

    /// <summary>Nombre de pannes (interventions) déclarées aujourd'hui.</summary>
    public int PannesAujourdhui { get; set; }

    /// <summary>Interventions ouvertes ayant dépassé leur délai SLA (selon priorité).</summary>
    public int EnDepassementSla { get; set; }

    // --- Indicateurs de performance ---
    /// <summary>MTTR : temps moyen de réparation (création → clôture) en heures, sur la période.</summary>
    public double MttrHeures { get; set; }

    /// <summary>Délai moyen d'affectation (création → prise en charge) en heures.</summary>
    public double DelaiMoyenAffectationHeures { get; set; }

    /// <summary>Coût cumulé des interventions clôturées de la période (main d'œuvre + pièces).</summary>
    public decimal CoutCumule { get; set; }

    // --- Stock ---
    public int PiecesEnAlerte { get; set; }

    // --- Contexte ---
    public int NombreHopitaux { get; set; }
    public int NombreIngenieurs { get; set; }

    // --- Séries pour graphiques / listes ---
    /// <summary>Répartition des interventions par état (camembert).</summary>
    public List<CategorieValeur> RepartitionParEtat { get; set; } = new();

    /// <summary>Répartition des interventions par modèle de respirateur (histogramme).</summary>
    public List<CategorieValeur> InterventionsParModele { get; set; } = new();

    /// <summary>Pareto des pannes / symptômes récurrents (les plus fréquents d'abord).</summary>
    public List<CategorieValeur> ParetoPannes { get; set; } = new();

    /// <summary>Taux de disponibilité estimé par équipement sur la période (%).</summary>
    public List<CategorieValeur> DisponibiliteParEquipement { get; set; } = new();

    /// <summary>Top 5 des respirateurs les plus défaillants (nombre d'interventions sur la période).</summary>
    public List<CategorieValeur> Top5Respirateurs { get; set; } = new();

    /// <summary>Top 5 des hôpitaux générant le plus d'interventions sur la période.</summary>
    public List<CategorieValeur> Top5Hopitaux { get; set; } = new();

    /// <summary>Charge de travail par technicien (interventions en cours / terminées).</summary>
    public List<ChargeTechnicien> ChargeParTechnicien { get; set; } = new();
}

/// <summary>Couple (catégorie, valeur) pour les graphiques.</summary>
public class CategorieValeur
{
    public string Libelle { get; set; } = string.Empty;
    public int Valeur { get; set; }
}

/// <summary>Charge de travail d'un technicien / ingénieur.</summary>
public class ChargeTechnicien
{
    public string Nom { get; set; } = string.Empty;
    public int EnCours { get; set; }
    public int Terminees { get; set; }
}

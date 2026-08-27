namespace GMAO.Application.DTOs;

/// <summary>Filtre des statistiques avancées (fenêtre d'analyse).</summary>
public class StatistiquesFiltre
{
    /// <summary>Début de la fenêtre. Par défaut : 12 mois avant la fin.</summary>
    public DateTime? Debut { get; set; }

    /// <summary>Fin de la fenêtre. Par défaut : maintenant.</summary>
    public DateTime? Fin { get; set; }
}

/// <summary>Valeur mensuelle (entière) — ex. nombre de pannes.</summary>
public class PointMensuel
{
    public string Mois { get; set; } = string.Empty;
    public int Valeur { get; set; }
}

/// <summary>Coût mensuel des interventions.</summary>
public class CoutMensuel
{
    public string Mois { get; set; } = string.Empty;
    public decimal Cout { get; set; }
}

/// <summary>Durée moyenne d'intervention d'un ingénieur (en heures).</summary>
public class DureeIngenieur
{
    public string Nom { get; set; } = string.Empty;
    public double DureeMoyenneHeures { get; set; }
}

/// <summary>Ligne de comparaison entre modèles de respirateurs.</summary>
public class ComparaisonModele
{
    public string Modele { get; set; } = string.Empty;
    public int NombreInterventions { get; set; }
    public double MttrHeures { get; set; }
    public decimal CoutMoyen { get; set; }
}

/// <summary>Jeu de données des statistiques avancées.</summary>
public class StatistiquesDto
{
    public string PeriodeLibelle { get; set; } = string.Empty;

    /// <summary>Évolution du nombre de pannes (interventions) par mois.</summary>
    public List<PointMensuel> PannesParMois { get; set; } = new();

    /// <summary>Coût des interventions clôturées par mois.</summary>
    public List<CoutMensuel> CoutParMois { get; set; } = new();

    /// <summary>Taux de disponibilité estimé par hôpital (%).</summary>
    public List<CategorieValeur> DisponibiliteParHopital { get; set; } = new();

    /// <summary>Durée moyenne d'intervention par ingénieur.</summary>
    public List<DureeIngenieur> DureeMoyenneParIngenieur { get; set; } = new();

    /// <summary>Comparaison entre modèles de respirateurs.</summary>
    public List<ComparaisonModele> ComparaisonModeles { get; set; } = new();
}

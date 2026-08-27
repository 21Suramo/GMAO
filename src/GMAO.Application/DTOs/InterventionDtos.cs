using GMAO.Domain.Enums;

namespace GMAO.Application.DTOs;

/// <summary>Représentation d'une intervention pour les listes.</summary>
public class InterventionDto
{
    public int Id { get; set; }
    public string NumeroDI { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public string RespirateurSerie { get; set; } = string.Empty;
    public string ModeleNom { get; set; } = string.Empty;
    public string HopitalNom { get; set; } = string.Empty;
    public string? IngenieurNom { get; set; }

    public string Description { get; set; } = string.Empty;

    public EtatIntervention Etat { get; set; }
    public Priorite Priorite { get; set; }
    public bool PatientConnecte { get; set; }
    public bool Urgence { get; set; }

    public string EtatLibelle => LibelleEtat(Etat);
    public string PrioriteLibelle => Priorite switch
    {
        Priorite.Basse => "Basse",
        Priorite.Normale => "Normale",
        Priorite.Haute => "Haute",
        Priorite.Critique => "Critique",
        _ => Priorite.ToString()
    };
    public bool EstCritique => PatientConnecte || Priorite == Priorite.Critique;

    /// <summary>Libellé lisible d'un état du workflow.</summary>
    public static string LibelleEtat(EtatIntervention etat) => etat switch
    {
        EtatIntervention.Nouvelle => "Nouvelle",
        EtatIntervention.Affectee => "Affectée",
        EtatIntervention.EnDeplacement => "En déplacement",
        EtatIntervention.Diagnostic => "Diagnostic",
        EtatIntervention.Reparation => "Réparation",
        EtatIntervention.EnAttentePiece => "En attente pièce",
        EtatIntervention.Test => "Test fonctionnel",
        EtatIntervention.Validation => "Validation",
        EtatIntervention.Cloturee => "Clôturée",
        EtatIntervention.Annulee => "Annulée",
        _ => etat.ToString()
    };
}

/// <summary>Détail complet d'une intervention (fiche + check-list + historique).</summary>
public class InterventionDetailDto : InterventionDto
{
    public string? Diagnostic { get; set; }
    public string? Cause { get; set; }
    public string? Commentaires { get; set; }
    public DateTime? DateCloture { get; set; }
    public int TempsDeplacement { get; set; }
    public int TempsReparation { get; set; }

    public List<string> Symptomes { get; set; } = new();
    public CheckListDto? CheckList { get; set; }
    public List<HistoriqueEtatDto> Historique { get; set; } = new();
}

/// <summary>Check-list de clôture (toutes les cases obligatoires — RG-02).</summary>
public class CheckListDto
{
    public bool AutotestOk { get; set; }
    public bool TestEtancheite { get; set; }
    public bool CalibrationDebit { get; set; }
    public bool CalibrationO2 { get; set; }
    public bool Batterie { get; set; }
    public bool Alimentation { get; set; }
    public bool Alarmes { get; set; }
    public bool ValidationFinale { get; set; }

    public bool EstComplete => AutotestOk && TestEtancheite && CalibrationDebit && CalibrationO2
                               && Batterie && Alimentation && Alarmes && ValidationFinale;
}

/// <summary>Entrée d'historique de changement d'état.</summary>
public class HistoriqueEtatDto
{
    public DateTime Date { get; set; }
    public EtatIntervention AncienEtat { get; set; }
    public EtatIntervention NouvelEtat { get; set; }
    public string? Auteur { get; set; }
    public string Libelle => $"{InterventionDto.LibelleEtat(AncienEtat)} → {InterventionDto.LibelleEtat(NouvelEtat)}";
}

/// <summary>Symptôme prédéfini sélectionnable lors d'une déclaration.</summary>
public class SymptomeDto
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public bool Selectionne { get; set; }
}

/// <summary>Données de déclaration d'une nouvelle intervention (DI).</summary>
public class CreerInterventionRequete
{
    public int RespirateurId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Commentaires { get; set; }
    public Priorite Priorite { get; set; } = Priorite.Normale;
    public bool PatientConnecte { get; set; }
    public bool Urgence { get; set; }
    public List<int> SymptomeIds { get; set; } = new();
}

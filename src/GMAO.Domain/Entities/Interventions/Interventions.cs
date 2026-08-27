using GMAO.Domain.Common;
using GMAO.Domain.Enums;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Pieces;
using GMAO.Domain.Entities.Planning;

namespace GMAO.Domain.Entities.Interventions;

/// <summary>
/// Intervention de maintenance corrective (Demande d'Intervention — DI).
/// Entité centrale du workflow SAV.
/// </summary>
public class Intervention : EntiteBase
{
    /// <summary>Numéro de DI lisible (unique), ex. « DI-2026-0001 ».</summary>
    public string NumeroDI { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;
    public string? Diagnostic { get; set; }
    public string? Cause { get; set; }

    public EtatIntervention Etat { get; set; } = EtatIntervention.Nouvelle;
    public Priorite Priorite { get; set; } = Priorite.Normale;

    /// <summary>Patient connecté au respirateur lors de la déclaration (déclencheur de criticité — RG-01).</summary>
    public bool PatientConnecte { get; set; }
    public bool Urgence { get; set; }

    /// <summary>Temps de déplacement en minutes.</summary>
    public int TempsDeplacement { get; set; }

    /// <summary>Temps de réparation en minutes.</summary>
    public int TempsReparation { get; set; }

    /// <summary>Coût de la main d'œuvre.</summary>
    public decimal MainOeuvre { get; set; }

    public string? Commentaires { get; set; }

    /// <summary>Signature de validation (image base64 ou chemin de fichier).</summary>
    public string? Signature { get; set; }

    public DateTime? DateCloture { get; set; }

    // Relations
    public int RespirateurId { get; set; }
    public Respirateur? Respirateur { get; set; }

    public int HopitalId { get; set; }
    public Hopital? Hopital { get; set; }

    public int? IngenieurId { get; set; }
    public Ingenieur? Ingenieur { get; set; }

    public int? PanneId { get; set; }
    public Panne? Panne { get; set; }

    public CheckListCloture? CheckList { get; set; }
    public Rapport? Rapport { get; set; }

    public ICollection<Symptome> Symptomes { get; set; } = new List<Symptome>();
    public ICollection<LignePieceIntervention> PiecesUtilisees { get; set; } = new List<LignePieceIntervention>();
    public ICollection<PhotoDocument> PiecesJointes { get; set; } = new List<PhotoDocument>();
    public ICollection<HistoriqueEtatIntervention> HistoriqueEtats { get; set; } = new List<HistoriqueEtatIntervention>();

    /// <summary>Coût des pièces consommées.</summary>
    public decimal CoutPieces() => PiecesUtilisees?.Sum(p => p.PrixUnitaire * p.Quantite) ?? 0m;

    /// <summary>Coût total = main d'œuvre + pièces.</summary>
    public decimal CoutTotal() => MainOeuvre + CoutPieces();

    /// <summary>Temps total d'immobilisation (déplacement + réparation) en minutes.</summary>
    public int TempsTotal() => TempsDeplacement + TempsReparation;

    /// <summary>Indique si la DI relève d'un cas critique (patient connecté ou priorité critique).</summary>
    public bool EstCritique() => PatientConnecte || Priorite == Priorite.Critique;

    /// <summary>
    /// Vrai si l'intervention peut être clôturée : check-list complète et non déjà clôturée/annulée (RG-02).
    /// </summary>
    public bool PeutCloturer()
        => CheckList is not null
           && CheckList.EstComplete()
           && Etat is not (EtatIntervention.Cloturee or EtatIntervention.Annulee);
}

/// <summary>Symptôme prédéfini déclarable lors d'une DI.</summary>
public class Symptome : EntiteBase
{
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}

/// <summary>Type de panne / défaillance catalogué.</summary>
public class Panne : EntiteBase
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();

    /// <summary>Pièces susceptibles d'être à l'origine de cette panne (association panne ↔ pièce).</summary>
    public ICollection<PanneePiece> PiecesSuspectes { get; set; } = new List<PanneePiece>();
}

/// <summary>
/// Check-list de contrôle obligatoire avant clôture d'une intervention.
/// Toutes les cases doivent être validées (RG-02).
/// </summary>
public class CheckListCloture : EntiteBase
{
    public int InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public bool AutotestOk { get; set; }
    public bool TestEtancheite { get; set; }
    public bool CalibrationDebit { get; set; }
    public bool CalibrationO2 { get; set; }
    public bool Batterie { get; set; }
    public bool Alimentation { get; set; }
    public bool Alarmes { get; set; }
    public bool ValidationFinale { get; set; }

    public string? Commentaire { get; set; }

    /// <summary>Vrai si toutes les vérifications obligatoires sont validées.</summary>
    public bool EstComplete()
        => AutotestOk && TestEtancheite && CalibrationDebit && CalibrationO2
           && Batterie && Alimentation && Alarmes && ValidationFinale;
}

/// <summary>Ligne de consommation d'une pièce lors d'une intervention.</summary>
public class LignePieceIntervention : EntiteBase
{
    public int InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public int PieceId { get; set; }
    public Piece? Piece { get; set; }

    public int Quantite { get; set; } = 1;

    /// <summary>Prix unitaire au moment de la consommation (instantané).</summary>
    public decimal PrixUnitaire { get; set; }
}

/// <summary>Rapport PDF généré et archivé à la clôture d'une intervention.</summary>
public class Rapport : EntiteBase
{
    public int InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public string NumeroRapport { get; set; } = string.Empty;
    public string CheminPdf { get; set; } = string.Empty;
    public DateTime DateGeneration { get; set; } = DateTime.UtcNow;
}

/// <summary>Pièce jointe (photo, vidéo, document) rattachée à une intervention.</summary>
public class PhotoDocument : EntiteBase
{
    public int InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public string CheminFichier { get; set; } = string.Empty;
    public string? Legende { get; set; }

    /// <summary>Type de contenu : « Photo », « Video », « Document ».</summary>
    public string TypeContenu { get; set; } = "Photo";
}

/// <summary>Trace d'un changement d'état du workflow d'une intervention (RG-07).</summary>
public class HistoriqueEtatIntervention : EntiteBase
{
    public int InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public EtatIntervention AncienEtat { get; set; }
    public EtatIntervention NouvelEtat { get; set; }
    public DateTime DateChangement { get; set; } = DateTime.UtcNow;
    public string? Auteur { get; set; }
    public string? Commentaire { get; set; }
}

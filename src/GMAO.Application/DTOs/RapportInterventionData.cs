namespace GMAO.Application.DTOs;

/// <summary>Données nécessaires à la génération du rapport PDF d'une intervention.</summary>
public class RapportInterventionData
{
    public string NumeroDI { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? DateCloture { get; set; }

    public string ClientNom { get; set; } = string.Empty;
    public string ClientVille { get; set; } = string.Empty;

    public string AppareilSerie { get; set; } = string.Empty;
    public string AppareilModele { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string? Diagnostic { get; set; }
    public string? Cause { get; set; }
    public string Etat { get; set; } = string.Empty;
    public string? Ingenieur { get; set; }

    public int TempsDeplacement { get; set; }
    public int TempsReparation { get; set; }
    public decimal MainOeuvre { get; set; }
    public decimal CoutPieces { get; set; }
    public decimal CoutTotal => MainOeuvre + CoutPieces;

    public List<LignePieceData> Pieces { get; set; } = new();
    public List<string> CheckListValidee { get; set; } = new();

    /// <summary>Texte encodé dans le QR Code du rapport.</summary>
    public string QrContenu { get; set; } = string.Empty;
}

/// <summary>Ligne de pièce remplacée dans le rapport.</summary>
public class LignePieceData
{
    public string Reference { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal Total => PrixUnitaire * Quantite;
}

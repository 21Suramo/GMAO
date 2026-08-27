using GMAO.Domain.Common;
using GMAO.Domain.Enums;
using GMAO.Domain.Entities.Interventions;

namespace GMAO.Domain.Entities.Pieces;

/// <summary>Catégorie de pièce détachée.</summary>
public class CategoriePiece : EntiteBase
{
    public string Nom { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
}

/// <summary>Fournisseur de pièces détachées.</summary>
public class Fournisseur : EntiteBase
{
    public string Nom { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }

    public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
}

/// <summary>Pièce détachée gérée en stock.</summary>
public class Piece : EntiteBase
{
    /// <summary>Référence catalogue (unique).</summary>
    public string Reference { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;

    /// <summary>Modèles compatibles (texte libre ou liste).</summary>
    public string? Compatible { get; set; }

    public int Stock { get; set; }
    public int StockMinimum { get; set; }
    public string? Emplacement { get; set; }
    public decimal Prix { get; set; }
    public DateTime? DatePeremption { get; set; }

    public int? CategoriePieceId { get; set; }
    public CategoriePiece? Categorie { get; set; }

    public int? FournisseurId { get; set; }
    public Fournisseur? Fournisseur { get; set; }

    public ICollection<MouvementStock> Mouvements { get; set; } = new List<MouvementStock>();
    public ICollection<PanneePiece> PannesAssociees { get; set; } = new List<PanneePiece>();

    /// <summary>Vrai si le stock a atteint le seuil minimum (RG-04).</summary>
    public bool EnAlerte() => Stock <= StockMinimum;

    /// <summary>Vrai si le stock est nul.</summary>
    public bool EnRupture() => Stock <= 0;

    /// <summary>Vrai si la pièce est périmée.</summary>
    public bool EstPerime() => DatePeremption.HasValue && DatePeremption.Value.Date < DateTime.UtcNow.Date;
}

/// <summary>Mouvement de stock (entrée, sortie, ajustement) d'une pièce.</summary>
public class MouvementStock : EntiteBase
{
    public int PieceId { get; set; }
    public Piece? Piece { get; set; }

    public TypeMouvement Type { get; set; }
    public int Quantite { get; set; }
    public DateTime DateMouvement { get; set; } = DateTime.UtcNow;
    public string? Motif { get; set; }
    public string? Auteur { get; set; }
}

/// <summary>
/// Association entre une panne et une pièce probablement responsable
/// (permet d'identifier les pièces à l'origine des défaillances).
/// </summary>
public class PanneePiece : EntiteBase
{
    public int PanneId { get; set; }
    public Panne? Panne { get; set; }

    public int PieceId { get; set; }
    public Piece? Piece { get; set; }

    /// <summary>Probabilité indicative (0 à 100 %) que la pièce soit responsable.</summary>
    public int Probabilite { get; set; }
}

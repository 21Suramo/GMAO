namespace GMAO.Application.DTOs;

/// <summary>Représentation d'une pièce détachée (stock + alertes).</summary>
public class PieceDto
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Compatible { get; set; }

    public int Stock { get; set; }
    public int StockMinimum { get; set; }
    public string? Emplacement { get; set; }
    public decimal Prix { get; set; }
    public DateTime? DatePeremption { get; set; }

    public string? CategorieNom { get; set; }
    public string? FournisseurNom { get; set; }

    /// <summary>Pannes associées à cette pièce (analyse des défaillances).</summary>
    public List<string> PannesAssociees { get; set; } = new();

    public bool EnRupture => Stock <= 0;
    public bool EnAlerte => Stock <= StockMinimum;
    public bool EstPerime => DatePeremption.HasValue && DatePeremption.Value.Date < DateTime.UtcNow.Date;

    /// <summary>Niveau d'alerte synthétique (pour l'affichage).</summary>
    public string NiveauAlerte =>
        EnRupture ? "Rupture" :
        EnAlerte ? "Stock bas" :
        EstPerime ? "Périmé" : "OK";
}

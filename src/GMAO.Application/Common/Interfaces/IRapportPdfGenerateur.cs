using GMAO.Application.DTOs;

namespace GMAO.Application.Common.Interfaces;

/// <summary>
/// Port de génération du document PDF (implémenté avec iText7 dans l'Infrastructure).
/// </summary>
public interface IRapportPdfGenerateur
{
    /// <summary>Produit le PDF du rapport d'intervention sous forme d'octets.</summary>
    byte[] Generer(RapportInterventionData data);
}

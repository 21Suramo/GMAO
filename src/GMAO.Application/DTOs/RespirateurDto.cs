using GMAO.Domain.Enums;

namespace GMAO.Application.DTOs;

/// <summary>Représentation d'un respirateur pour l'affichage (liste & fiche).</summary>
public class RespirateurDto
{
    public int Id { get; set; }
    public string NumeroSerie { get; set; } = string.Empty;
    public string CodeInterne { get; set; } = string.Empty;
    public Guid CodeQr { get; set; }

    public string ModeleNom { get; set; } = string.Empty;
    public string? ModeleGamme { get; set; }
    public string? VersionLogicielle { get; set; }
    public string? VersionMaterielle { get; set; }

    public EtatRespirateur Etat { get; set; }
    public DateTime DateMiseEnService { get; set; }

    public bool SousContrat { get; set; }
    public string? NumeroContrat { get; set; }
    public DateTime? DateFinGarantie { get; set; }

    /// <summary>Localisation lisible « Hôpital · Service · Bloc ».</summary>
    public string Localisation { get; set; } = string.Empty;

    public string? MotifHorsService { get; set; }
    public DateTime? DateHorsService { get; set; }
    public string? AuteurHorsService { get; set; }

    /// <summary>Libellé lisible de l'état.</summary>
    public string EtatLibelle => Etat switch
    {
        EtatRespirateur.EnService => "En service",
        EtatRespirateur.EnMaintenance => "En maintenance",
        EtatRespirateur.HorsService => "Hors service",
        EtatRespirateur.EnAttente => "En attente",
        _ => Etat.ToString()
    };

    public bool EstHorsService => Etat == EtatRespirateur.HorsService;

    public bool SousGarantie => DateFinGarantie.HasValue && DateFinGarantie.Value.Date >= DateTime.UtcNow.Date;

    /// <summary>Contenu encodé dans le QR Code de l'appareil.</summary>
    public string CodeQrTexte => $"GMAO-RESP:{CodeQr}";
}

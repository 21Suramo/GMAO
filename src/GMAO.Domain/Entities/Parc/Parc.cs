using GMAO.Domain.Common;
using GMAO.Domain.Enums;
using GMAO.Domain.Entities.Interventions;

namespace GMAO.Domain.Entities.Parc;

/// <summary>Établissement client (hôpital).</summary>
public class Hopital : EntiteBase
{
    public string Nom { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Telephone { get; set; }
    public string? Contact { get; set; }
    public string? Email { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
}

/// <summary>Service hospitalier rattaché à un hôpital (ex. Bloc central, Réanimation).</summary>
public class Service : EntiteBase
{
    public string Nom { get; set; } = string.Empty;

    public int HopitalId { get; set; }
    public Hopital? Hopital { get; set; }

    public ICollection<BlocOperatoire> BlocsOperatoires { get; set; } = new List<BlocOperatoire>();
}

/// <summary>Bloc opératoire hébergeant des respirateurs.</summary>
public class BlocOperatoire : EntiteBase
{
    public string Nom { get; set; } = string.Empty;

    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    public ICollection<Respirateur> Respirateurs { get; set; } = new List<Respirateur>();
}

/// <summary>Modèle / gamme de respirateur Datex-Ohmeda (Aespire, Avance, Aisys…).</summary>
public class ModeleRespirateur : EntiteBase
{
    public string Nom { get; set; } = string.Empty;
    public string? Gamme { get; set; }
    public string Constructeur { get; set; } = "Datex-Ohmeda";
    public string? Description { get; set; }

    public ICollection<Respirateur> Respirateurs { get; set; } = new List<Respirateur>();
    public ICollection<DocumentTechnique> Documents { get; set; } = new List<DocumentTechnique>();
}

/// <summary>Respirateur d'anesthésie individuel du parc (équipement tracé).</summary>
public class Respirateur : EntiteBase
{
    /// <summary>Numéro de série constructeur (unique).</summary>
    public string NumeroSerie { get; set; } = string.Empty;

    /// <summary>Code interne MEDICANA.</summary>
    public string CodeInterne { get; set; } = string.Empty;

    /// <summary>Identifiant encodé dans le QR Code (unique).</summary>
    public Guid CodeQr { get; set; } = Guid.NewGuid();

    public string? VersionLogicielle { get; set; }
    public string? VersionMaterielle { get; set; }

    public EtatRespirateur Etat { get; set; } = EtatRespirateur.EnService;
    public DateTime DateMiseEnService { get; set; } = DateTime.UtcNow;

    // Contrat & garantie
    public string? NumeroContrat { get; set; }
    public bool SousContrat { get; set; }
    public DateTime? DateFinGarantie { get; set; }

    // Localisation
    public int ModeleRespirateurId { get; set; }
    public ModeleRespirateur? Modele { get; set; }
    public int? BlocOperatoireId { get; set; }
    public BlocOperatoire? BlocOperatoire { get; set; }

    // Informations « Hors service »
    public string? MotifHorsService { get; set; }
    public DateTime? DateHorsService { get; set; }
    public string? AuteurHorsService { get; set; }

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
    public ICollection<DocumentTechnique> Documents { get; set; } = new List<DocumentTechnique>();

    /// <summary>Vrai si le respirateur est déclaré hors service.</summary>
    public bool EstHorsService() => Etat == EtatRespirateur.HorsService;

    /// <summary>Vrai si le respirateur peut recevoir une intervention médicale programmée (RG-03).</summary>
    public bool EstProgrammable() => Etat != EtatRespirateur.HorsService;

    /// <summary>Vrai si l'appareil est encore sous garantie à la date du jour.</summary>
    public bool SousGarantie() => DateFinGarantie.HasValue && DateFinGarantie.Value.Date >= DateTime.UtcNow.Date;
}

/// <summary>Document technique (manuel, schéma, procédure…) lié à un modèle ou un respirateur.</summary>
public class DocumentTechnique : EntiteBase
{
    public string Titre { get; set; } = string.Empty;
    public TypeDocument Type { get; set; }

    /// <summary>Chemin relatif du fichier (PDF, image…).</summary>
    public string CheminFichier { get; set; } = string.Empty;

    public int? ModeleRespirateurId { get; set; }
    public ModeleRespirateur? Modele { get; set; }

    public int? RespirateurId { get; set; }
    public Respirateur? Respirateur { get; set; }
}

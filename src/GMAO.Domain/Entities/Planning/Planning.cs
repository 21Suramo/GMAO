using GMAO.Domain.Common;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Entities.Interventions;

namespace GMAO.Domain.Entities.Planning;

/// <summary>Ingénieur biomédical / technicien réalisant les interventions.</summary>
public class Ingenieur : EntiteBase
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Email { get; set; }

    /// <summary>Zone géographique d'intervention (critère d'affectation).</summary>
    public string? Zone { get; set; }

    /// <summary>Disponibilité globale (hors congés).</summary>
    public bool Disponible { get; set; } = true;

    public int? UtilisateurId { get; set; }
    public Utilisateur? Utilisateur { get; set; }

    public ICollection<Conge> Conges { get; set; } = new List<Conge>();
    public ICollection<Competence> Competences { get; set; } = new List<Competence>();
    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();

    public string NomComplet => $"{Prenom} {Nom}".Trim();

    /// <summary>Vrai si l'ingénieur est disponible à la date donnée (ni indisponible, ni en congé).</summary>
    public bool EstDisponible(DateTime date)
        => Disponible
           && (Conges is null || !Conges.Any(c => date.Date >= c.DateDebut.Date && date.Date <= c.DateFin.Date));
}

/// <summary>Période de congé / indisponibilité d'un ingénieur.</summary>
public class Conge : EntiteBase
{
    public int IngenieurId { get; set; }
    public Ingenieur? Ingenieur { get; set; }

    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string? Motif { get; set; }
}

/// <summary>Compétence d'un ingénieur (souvent liée à la maîtrise d'un modèle).</summary>
public class Competence : EntiteBase
{
    public string Libelle { get; set; } = string.Empty;

    public int? ModeleRespirateurId { get; set; }
    public ModeleRespirateur? Modele { get; set; }

    public ICollection<Ingenieur> Ingenieurs { get; set; } = new List<Ingenieur>();
}

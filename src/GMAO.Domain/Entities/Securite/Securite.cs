using GMAO.Domain.Common;
using GMAO.Domain.Enums;

namespace GMAO.Domain.Entities.Securite;

/// <summary>
/// Compte utilisateur de l'application (authentification + RBAC).
/// </summary>
public class Utilisateur : EntiteBase
{
    /// <summary>Identifiant de connexion (unique).</summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>Empreinte BCrypt du mot de passe (jamais le mot de passe en clair).</summary>
    public string MotDePasseHash { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }

    /// <summary>Rôle déterminant les droits d'accès.</summary>
    public RoleType Role { get; set; } = RoleType.Invite;

    /// <summary>Compte actif (un compte désactivé ne peut pas se connecter).</summary>
    public bool Actif { get; set; } = true;

    public DateTime? DerniereConnexion { get; set; }

    /// <summary>Journal des tentatives de connexion de cet utilisateur.</summary>
    public ICollection<HistoriqueConnexion> HistoriqueConnexions { get; set; } = new List<HistoriqueConnexion>();

    /// <summary>Nom complet « Prénom Nom » (calculé, non persisté).</summary>
    public string NomComplet => $"{Prenom} {Nom}".Trim();
}

/// <summary>
/// Entrée du journal d'audit des connexions (réussies ou échouées).
/// </summary>
public class HistoriqueConnexion : EntiteBase
{
    public int UtilisateurId { get; set; }
    public Utilisateur? Utilisateur { get; set; }

    public DateTime DateConnexion { get; set; } = DateTime.UtcNow;
    public string? AdresseIp { get; set; }

    /// <summary>Vrai si l'authentification a réussi.</summary>
    public bool Succes { get; set; }

    /// <summary>Précision (ex. « mot de passe invalide », « compte désactivé »).</summary>
    public string? Detail { get; set; }
}

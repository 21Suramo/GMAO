using GMAO.Domain.Enums;

namespace GMAO.Application.DTOs;

/// <summary>Ligne de la liste d'administration des comptes utilisateurs.</summary>
public class UtilisateurListeDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public RoleType Role { get; set; }
    public string RoleLibelle { get; set; } = string.Empty;
    public bool Actif { get; set; }
    public DateTime? DerniereConnexion { get; set; }
}

/// <summary>Données de création d'un compte utilisateur (saisies par l'administrateur).</summary>
public class CreerUtilisateurRequete
{
    public string Login { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public RoleType Role { get; set; } = RoleType.Technicien;

    /// <summary>Mot de passe en clair : haché (BCrypt) par le service, jamais persisté tel quel.</summary>
    public string MotDePasse { get; set; } = string.Empty;
}

/// <summary>Données de modification d'un compte utilisateur existant.</summary>
public class ModifierUtilisateurRequete
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public RoleType Role { get; set; }
    public bool Actif { get; set; }
}

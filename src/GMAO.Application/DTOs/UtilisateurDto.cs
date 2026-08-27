using GMAO.Domain.Enums;

namespace GMAO.Application.DTOs;

/// <summary>Représentation d'un utilisateur exposée hors du domaine (sans secret).</summary>
public class UtilisateurDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string? Email { get; set; }
    public RoleType Role { get; set; }

    /// <summary>Libellé lisible du rôle (ex. « Responsable SAV »).</summary>
    public string RoleLibelle { get; set; } = string.Empty;
}

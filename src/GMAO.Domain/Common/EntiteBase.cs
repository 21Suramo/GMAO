namespace GMAO.Domain.Common;

/// <summary>
/// Classe de base de toutes les entités persistées.
/// Fournit l'identifiant et les champs d'audit communs (création/modification, soft-delete).
/// </summary>
public abstract class EntiteBase
{
    /// <summary>Identifiant technique (clé primaire auto-incrémentée).</summary>
    public int Id { get; set; }

    /// <summary>Date de création de l'enregistrement (UTC).</summary>
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    /// <summary>Date de dernière modification (UTC), nulle si jamais modifiée.</summary>
    public DateTime? DateModification { get; set; }

    /// <summary>Identifiant ou nom de l'utilisateur ayant créé l'enregistrement.</summary>
    public string? CreePar { get; set; }

    /// <summary>Identifiant ou nom de l'utilisateur ayant modifié l'enregistrement en dernier.</summary>
    public string? ModifiePar { get; set; }

    /// <summary>Indique si l'entité est logiquement supprimée (soft-delete).</summary>
    public bool EstSupprime { get; set; }
}

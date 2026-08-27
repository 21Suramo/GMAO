using GMAO.Application.DTOs;
using GMAO.Domain.Enums;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Conserve l'utilisateur authentifié pour la session applicative courante.</summary>
public interface ICurrentUserService
{
    /// <summary>Utilisateur connecté, ou null si aucune session active.</summary>
    UtilisateurDto? Utilisateur { get; }

    /// <summary>Vrai si un utilisateur est authentifié.</summary>
    bool EstAuthentifie { get; }

    /// <summary>Définit l'utilisateur de la session après une connexion réussie.</summary>
    void Definir(UtilisateurDto utilisateur);

    /// <summary>Termine la session (déconnexion).</summary>
    void Effacer();

    /// <summary>Indique si l'utilisateur courant possède l'un des rôles donnés.</summary>
    bool ADroit(params RoleType[] roles);
}

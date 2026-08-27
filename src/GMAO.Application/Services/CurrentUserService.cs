using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Enums;

namespace GMAO.Application.Services;

/// <summary>Implémentation en mémoire de la session utilisateur courante (singleton).</summary>
public class CurrentUserService : ICurrentUserService
{
    public UtilisateurDto? Utilisateur { get; private set; }

    public bool EstAuthentifie => Utilisateur is not null;

    public void Definir(UtilisateurDto utilisateur) => Utilisateur = utilisateur;

    public void Effacer() => Utilisateur = null;

    public bool ADroit(params RoleType[] roles)
        => Utilisateur is not null && roles.Contains(Utilisateur.Role);
}

using GMAO.Application.Common.Interfaces;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>
/// Implémentation du contrôle d'accès par action.
/// La logique de rôle vit déjà dans la couche Application (<see cref="ICurrentUserService"/>,
/// <c>RoleLibelle</c>) et la matrice des droits dans le Domaine (<see cref="MatricePermissions"/>) ;
/// ce service se place donc naturellement ici plutôt qu'en Persistence/Infrastructure.
/// </summary>
public class AutorisationService : IAutorisationService
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AutorisationService> _logger;

    public AutorisationService(
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ILogger<AutorisationService> logger)
    {
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> AutoriserAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        var session = _currentUser.Utilisateur;
        if (session is null)
        {
            _logger.LogWarning("Accès refusé à {Permission} : aucune session active.", permission);
            return Result.Echec("Accès refusé : aucune session active. Veuillez vous reconnecter.");
        }

        // Revérification en base : le compte doit toujours exister et être actif
        // (le filtre global de soft-delete écarte déjà les comptes supprimés).
        var utilisateur = await _unitOfWork.Repository<Utilisateur>()
            .FirstOrDefaultAsync(u => u.Id == session.Id, cancellationToken);

        if (utilisateur is null || !utilisateur.Actif)
        {
            _logger.LogWarning(
                "Accès refusé à {Permission} : le compte {Login} n'est plus valide (supprimé ou désactivé).",
                permission, session.Login);
            return Result.Echec("Accès refusé : votre compte n'est plus actif. Veuillez vous reconnecter.");
        }

        if (!MatricePermissions.Possede(utilisateur.Role, permission))
        {
            _logger.LogWarning(
                "Accès refusé : {Login} (rôle {Role}) a tenté l'action {Permission}.",
                utilisateur.Login, utilisateur.Role, permission);
            return Result.Echec($"Accès refusé : votre rôle ne permet pas cette action ({permission}).");
        }

        return Result.Succes();
    }

    public bool ADroit(Permission permission)
    {
        var session = _currentUser.Utilisateur;
        return session is not null && MatricePermissions.Possede(session.Role, permission);
    }
}

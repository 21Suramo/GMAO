using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>
/// Vérifie, action par action, que l'utilisateur courant a le droit d'exécuter une opération métier.
/// Chaque service applicatif appelle <see cref="AutoriserAsync"/> en tout début de méthode métier :
/// le contrôle ne dépend donc plus de l'écran depuis lequel l'action est déclenchée.
/// </summary>
public interface IAutorisationService
{
    /// <summary>
    /// Autorise (ou refuse) l'exécution d'une action nécessitant <paramref name="permission"/>.
    /// Revérifie l'identité de l'utilisateur courant en base (compte toujours existant et actif),
    /// ce qui couvre le cas d'une session périmée ou d'un compte désactivé entre l'affichage et le clic.
    /// </summary>
    /// <returns><see cref="Result.Succes()"/> si l'accès est accordé, sinon un échec explicite « Accès refusé ».</returns>
    Task<Result> AutoriserAsync(Permission permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Contrôle rapide, en mémoire (sans accès base), du rôle de l'utilisateur courant.
    /// Réservé au pilotage de l'UI (activation/masquage) ; ne remplace pas <see cref="AutoriserAsync"/>.
    /// </summary>
    bool ADroit(Permission permission);
}

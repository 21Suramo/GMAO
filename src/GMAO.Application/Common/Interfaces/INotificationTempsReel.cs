using GMAO.Application.DTOs;

namespace GMAO.Application.Common.Interfaces;

/// <summary>
/// Client de notifications temps réel (connexion au serveur Node.js via WebSocket).
/// </summary>
public interface INotificationTempsReel : IAsyncDisposable
{
    /// <summary>Déclenché à la réception d'une notification du serveur.</summary>
    event Action<NotificationMessage>? NotificationRecue;

    /// <summary>Indique si la connexion temps réel est active.</summary>
    bool EstConnecte { get; }

    /// <summary>Établit la connexion au serveur de notifications.</summary>
    Task ConnecterAsync(CancellationToken cancellationToken = default);

    /// <summary>Émet une notification (diffusée à tous les clients connectés).</summary>
    Task EnvoyerAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}

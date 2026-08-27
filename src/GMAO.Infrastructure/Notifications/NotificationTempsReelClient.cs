using System.Net.Http;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace GMAO.Infrastructure.Notifications;

/// <summary>
/// Implémentation du client de notifications temps réel basée sur WebSocket
/// (réception) et HTTP REST (émission) vers le serveur Node.js.
/// </summary>
public class NotificationTempsReelClient : INotificationTempsReel
{
    private const string BaseHttp = "http://localhost:4000";
    private const string UrlWebSocket = "ws://localhost:4000/ws";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ILogger<NotificationTempsReelClient> _logger;
    private readonly HttpClient _http = new();
    private ClientWebSocket? _socket;
    private CancellationTokenSource? _cts;

    public NotificationTempsReelClient(ILogger<NotificationTempsReelClient> logger) => _logger = logger;

    public event Action<NotificationMessage>? NotificationRecue;

    public bool EstConnecte => _socket?.State == WebSocketState.Open;

    public async Task ConnecterAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _socket = new ClientWebSocket();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await _socket.ConnectAsync(new Uri(UrlWebSocket), cancellationToken);
            _ = Task.Run(() => BoucleReceptionAsync(_cts.Token));
            _logger.LogInformation("Connecté au serveur de notifications ({Url})", UrlWebSocket);
        }
        catch (Exception ex)
        {
            // Le serveur de notifications est optionnel : on n'empêche pas l'application de fonctionner.
            _logger.LogWarning("Serveur de notifications indisponible : {Message}", ex.Message);
        }
    }

    private async Task BoucleReceptionAsync(CancellationToken cancellationToken)
    {
        var tampon = new byte[8192];
        try
        {
            while (_socket is { State: WebSocketState.Open } && !cancellationToken.IsCancellationRequested)
            {
                var resultat = await _socket.ReceiveAsync(new ArraySegment<byte>(tampon), cancellationToken);
                if (resultat.MessageType == WebSocketMessageType.Close) break;

                var json = Encoding.UTF8.GetString(tampon, 0, resultat.Count);
                var notification = JsonSerializer.Deserialize<NotificationMessage>(json, JsonOptions);
                if (notification is not null)
                    NotificationRecue?.Invoke(notification);
            }
        }
        catch (OperationCanceledException) { /* arrêt normal */ }
        catch (Exception ex)
        {
            _logger.LogWarning("Réception des notifications interrompue : {Message}", ex.Message);
        }
    }

    public async Task EnvoyerAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _http.PostAsJsonAsync($"{BaseHttp}/notify", message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Échec d'émission de la notification : {Message}", ex.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            _cts?.Cancel();
            if (_socket is { State: WebSocketState.Open })
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fermeture", CancellationToken.None);
        }
        catch { /* ignore */ }
        finally
        {
            _socket?.Dispose();
            _http.Dispose();
            _cts?.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}

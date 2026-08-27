using GMAO.Domain.Common;
using GMAO.Domain.Enums;
using GMAO.Domain.Entities.Securite;

namespace GMAO.Domain.Entities.Notifications;

/// <summary>Notification émise par le système (relayée en temps réel via SignalR).</summary>
public class Notification : EntiteBase
{
    public TypeNotification Type { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Vrai une fois la notification lue par son destinataire.</summary>
    public bool Lu { get; set; }

    public int? DestinataireUtilisateurId { get; set; }
    public Utilisateur? Destinataire { get; set; }

    /// <summary>Type de l'entité référencée (« Intervention », « Piece »…).</summary>
    public string? ReferenceType { get; set; }

    /// <summary>Identifiant de l'entité référencée.</summary>
    public int? ReferenceId { get; set; }
}

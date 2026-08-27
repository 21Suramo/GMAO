namespace GMAO.Application.DTOs;

/// <summary>Message de notification temps réel échangé avec le serveur Node.js.</summary>
public class NotificationMessage
{
    public string Type { get; set; } = "Info";
    public string Titre { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}

namespace GMAO.Application.DTOs;

/// <summary>Données d'une demande de connexion.</summary>
public class ConnexionRequete
{
    public string Login { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;

    /// <summary>Adresse IP / poste à l'origine de la connexion (journalisé).</summary>
    public string? AdresseIp { get; set; }
}

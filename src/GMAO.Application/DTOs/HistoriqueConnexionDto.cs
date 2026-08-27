namespace GMAO.Application.DTOs;

/// <summary>Entrée d'historique de connexion exposée à l'interface.</summary>
public class HistoriqueConnexionDto
{
    public DateTime DateConnexion { get; set; }
    public string? AdresseIp { get; set; }
    public bool Succes { get; set; }
    public string? Detail { get; set; }

    public string Statut => Succes ? "Réussie" : "Échouée";
}

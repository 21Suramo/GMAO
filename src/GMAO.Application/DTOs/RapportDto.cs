using GMAO.Domain.Enums;

namespace GMAO.Application.DTOs;

/// <summary>Synthèse d'une intervention et de son rapport PDF éventuel.</summary>
public class RapportDto
{
    public int InterventionId { get; set; }
    public string NumeroDI { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string ClientNom { get; set; } = string.Empty;
    public string AppareilSerie { get; set; } = string.Empty;
    public EtatIntervention Etat { get; set; }

    public bool ARapport { get; set; }
    public string? NumeroRapport { get; set; }
    public string? CheminPdf { get; set; }
    public DateTime? DateGeneration { get; set; }

    public string EtatLibelle => InterventionDto.LibelleEtat(Etat);
    public bool EstCloturee => Etat == EtatIntervention.Cloturee;
}

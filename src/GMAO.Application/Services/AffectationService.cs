using GMAO.Application.Common.Interfaces;
using GMAO.Application.Services.Affectation;
using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Planning;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Services;

/// <summary>
/// Affectation automatique : charge les candidats depuis la base et délègue
/// la décision au <see cref="MoteurAffectation"/>.
/// </summary>
public class AffectationService : IAffectationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAutorisationService _autorisation;

    public AffectationService(IUnitOfWork unitOfWork, IAutorisationService autorisation)
    {
        _unitOfWork = unitOfWork;
        _autorisation = autorisation;
    }

    public async Task<Result> AffecterAsync(int interventionId, int? ingenieurId, string auteur, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.AffecterIntervention, cancellationToken);
        if (acces.EstEchec)
            return acces;

        var depot = _unitOfWork.Repository<Intervention>();
        var intervention = await depot.GetByIdAsync(interventionId, cancellationToken);
        if (intervention is null)
            return Result.Echec("Intervention introuvable.");
        if (intervention.Etat is EtatIntervention.Cloturee or EtatIntervention.Annulee)
            return Result.Echec("Impossible d'affecter une intervention clôturée ou annulée.");

        // À défaut d'ingénieur imposé, on laisse le moteur choisir le meilleur candidat.
        var cible = ingenieurId ?? await ChoisirIngenieurAsync(intervention.RespirateurId, DateTime.UtcNow, cancellationToken);
        if (cible is null)
            return Result.Echec("Aucun ingénieur disponible pour cette affectation.");

        var existe = await _unitOfWork.Repository<Ingenieur>().AnyAsync(i => i.Id == cible.Value, cancellationToken);
        if (!existe)
            return Result.Echec("Ingénieur introuvable.");

        var ancienEtat = intervention.Etat;
        intervention.IngenieurId = cible;
        if (intervention.Etat == EtatIntervention.Nouvelle)
            intervention.Etat = EtatIntervention.Affectee;

        await _unitOfWork.Repository<HistoriqueEtatIntervention>().AddAsync(new HistoriqueEtatIntervention
        {
            InterventionId = interventionId,
            AncienEtat = ancienEtat,
            NouvelEtat = intervention.Etat,
            Auteur = auteur,
            Commentaire = $"Affectation à l'ingénieur #{cible}"
        }, cancellationToken);

        depot.Update(intervention);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Succes();
    }

    public async Task<int?> ChoisirIngenieurAsync(int respirateurId, DateTime date, CancellationToken cancellationToken = default)
    {
        // Contexte : modèle de l'appareil et ville de l'hôpital.
        var infos = await _unitOfWork.Repository<Respirateur>().ListerAsync(
            r => r.Id == respirateurId,
            r => new ContexteBrut
            {
                ModeleNom = r.Modele!.Nom,
                VilleHopital = r.BlocOperatoire != null ? r.BlocOperatoire.Service.Hopital.Ville : null
            },
            cancellationToken);

        var contexteBrut = infos.FirstOrDefault();
        if (contexteBrut is null) return null;

        var contexte = new ContexteAffectation
        {
            ModeleNom = contexteBrut.ModeleNom,
            VilleHopital = contexteBrut.VilleHopital
        };

        // Candidats : ingénieurs avec compétences, congés et charge.
        var bruts = await _unitOfWork.Repository<Ingenieur>().ListerAsync(
            null,
            i => new CandidatBrut
            {
                Id = i.Id,
                NomComplet = i.Prenom + " " + i.Nom,
                Zone = i.Zone,
                DisponibleBase = i.Disponible,
                Conges = i.Conges.Select(c => new Periode { Debut = c.DateDebut, Fin = c.DateFin }).ToList(),
                Competences = i.Competences.Select(c => c.Modele!.Nom).ToList(),
                NbInterventionsOuvertes = i.Interventions.Count(x =>
                    x.Etat != EtatIntervention.Cloturee && x.Etat != EtatIntervention.Annulee)
            },
            cancellationToken);

        var candidats = bruts.Select(b => new IngenieurCandidat
        {
            Id = b.Id,
            NomComplet = b.NomComplet,
            Zone = b.Zone,
            EstDisponible = b.DisponibleBase
                            && !b.Conges.Any(p => date.Date >= p.Debut.Date && date.Date <= p.Fin.Date),
            CompetencesModeles = b.Competences,
            NbInterventionsOuvertes = b.NbInterventionsOuvertes
        });

        return MoteurAffectation.Choisir(candidats, contexte)?.Id;
    }

    private sealed class ContexteBrut
    {
        public string ModeleNom { get; set; } = string.Empty;
        public string? VilleHopital { get; set; }
    }

    private sealed class CandidatBrut
    {
        public int Id { get; set; }
        public string NomComplet { get; set; } = string.Empty;
        public string? Zone { get; set; }
        public bool DisponibleBase { get; set; }
        public List<Periode> Conges { get; set; } = new();
        public List<string> Competences { get; set; } = new();
        public int NbInterventionsOuvertes { get; set; }
    }
}

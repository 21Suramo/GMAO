using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Planning;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>Implémentation des cas d'usage des interventions.</summary>
public class InterventionService : IInterventionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAffectationService _affectation;
    private readonly IAutorisationService _autorisation;
    private readonly ILogger<InterventionService> _logger;

    public InterventionService(
        IUnitOfWork unitOfWork,
        IAffectationService affectation,
        IAutorisationService autorisation,
        ILogger<InterventionService> logger)
    {
        _unitOfWork = unitOfWork;
        _affectation = affectation;
        _autorisation = autorisation;
        _logger = logger;
    }

    private static readonly Expression<Func<Intervention, InterventionDto>> ProjectionListe = i => new InterventionDto
    {
        Id = i.Id,
        NumeroDI = i.NumeroDI,
        Date = i.Date,
        RespirateurSerie = i.Respirateur!.NumeroSerie,
        ModeleNom = i.Respirateur.Modele!.Nom,
        HopitalNom = i.Hopital!.Nom,
        IngenieurNom = i.Ingenieur != null ? i.Ingenieur.Prenom + " " + i.Ingenieur.Nom : null,
        Description = i.Description,
        Etat = i.Etat,
        Priorite = i.Priorite,
        PatientConnecte = i.PatientConnecte,
        Urgence = i.Urgence
    };

    public async Task<IReadOnlyList<InterventionDto>> ListerAsync(CancellationToken cancellationToken = default)
    {
        var liste = await _unitOfWork.Repository<Intervention>().ListerAsync(null, ProjectionListe, cancellationToken);
        return liste.OrderByDescending(i => i.Date).ToList();
    }

    public async Task<InterventionDetailDto?> ObtenirAsync(int id, CancellationToken cancellationToken = default)
    {
        var liste = await _unitOfWork.Repository<Intervention>().ListerAsync(
            i => i.Id == id,
            i => new InterventionDetailDto
            {
                Id = i.Id,
                NumeroDI = i.NumeroDI,
                Date = i.Date,
                RespirateurSerie = i.Respirateur!.NumeroSerie,
                ModeleNom = i.Respirateur.Modele!.Nom,
                HopitalNom = i.Hopital!.Nom,
                IngenieurNom = i.Ingenieur != null ? i.Ingenieur.Prenom + " " + i.Ingenieur.Nom : null,
                Description = i.Description,
                Etat = i.Etat,
                Priorite = i.Priorite,
                PatientConnecte = i.PatientConnecte,
                Urgence = i.Urgence,
                Diagnostic = i.Diagnostic,
                Cause = i.Cause,
                Commentaires = i.Commentaires,
                DateCloture = i.DateCloture,
                TempsDeplacement = i.TempsDeplacement,
                TempsReparation = i.TempsReparation,
                Symptomes = i.Symptomes.Select(s => s.Libelle).ToList(),
                CheckList = i.CheckList == null ? null : new CheckListDto
                {
                    AutotestOk = i.CheckList.AutotestOk,
                    TestEtancheite = i.CheckList.TestEtancheite,
                    CalibrationDebit = i.CheckList.CalibrationDebit,
                    CalibrationO2 = i.CheckList.CalibrationO2,
                    Batterie = i.CheckList.Batterie,
                    Alimentation = i.CheckList.Alimentation,
                    Alarmes = i.CheckList.Alarmes,
                    ValidationFinale = i.CheckList.ValidationFinale
                },
                Historique = i.HistoriqueEtats
                    .OrderBy(h => h.DateChangement)
                    .Select(h => new HistoriqueEtatDto
                    {
                        Date = h.DateChangement,
                        AncienEtat = h.AncienEtat,
                        NouvelEtat = h.NouvelEtat,
                        Auteur = h.Auteur
                    }).ToList()
            },
            cancellationToken);

        return liste.FirstOrDefault();
    }

    public async Task<Result<InterventionDto>> CreerAsync(CreerInterventionRequete requete, string auteur, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.CreerIntervention, cancellationToken);
        if (acces.EstEchec)
            return Result.Echec<InterventionDto>(acces.Erreur);

        if (string.IsNullOrWhiteSpace(requete.Description))
            return Result.Echec<InterventionDto>("La description de la panne est obligatoire.");

        // Localisation du respirateur → hôpital (FK obligatoire de l'intervention).
        var infos = await _unitOfWork.Repository<Respirateur>().ListerAsync(
            r => r.Id == requete.RespirateurId,
            r => new { r.Id, r.NumeroSerie, HopitalId = (int?)r.BlocOperatoire!.Service.HopitalId },
            cancellationToken);

        var info = infos.FirstOrDefault();
        if (info is null)
            return Result.Echec<InterventionDto>("Respirateur introuvable.");
        if (info.HopitalId is null)
            return Result.Echec<InterventionDto>("Le respirateur n'est rattaché à aucun hôpital (bloc non défini).");

        // Numéro de DI séquentiel par année.
        var annee = DateTime.UtcNow.Year;
        var nbAnnee = await _unitOfWork.Repository<Intervention>().CountAsync(i => i.Date.Year == annee, cancellationToken);
        var numeroDI = $"DI-{annee}-{(nbAnnee + 1):D4}";

        // RG-01 : patient connecté ⇒ criticité maximale + affectation immédiate.
        var priorite = requete.PatientConnecte ? Priorite.Critique : requete.Priorite;
        var etat = EtatIntervention.Nouvelle;
        int? ingenieurId = null;

        if (requete.PatientConnecte)
        {
            // Affectation automatique (compétences, zone, disponibilité, charge).
            ingenieurId = await _affectation.ChoisirIngenieurAsync(requete.RespirateurId, DateTime.UtcNow, cancellationToken);
            if (ingenieurId is not null)
                etat = EtatIntervention.Affectee;
        }

        var intervention = new Intervention
        {
            NumeroDI = numeroDI,
            Date = DateTime.UtcNow,
            Description = requete.Description,
            Commentaires = requete.Commentaires,
            RespirateurId = requete.RespirateurId,
            HopitalId = info.HopitalId.Value,
            IngenieurId = ingenieurId,
            Priorite = priorite,
            PatientConnecte = requete.PatientConnecte,
            Urgence = requete.Urgence || requete.PatientConnecte,
            Etat = etat,
            CheckList = new CheckListCloture(),
            HistoriqueEtats =
            {
                new HistoriqueEtatIntervention
                {
                    AncienEtat = EtatIntervention.Nouvelle,
                    NouvelEtat = etat,
                    Auteur = auteur,
                    Commentaire = "Création de la DI"
                }
            }
        };

        // Liaison des symptômes déclarés (entités existantes, suivies).
        foreach (var symptomeId in requete.SymptomeIds.Distinct())
        {
            var symptome = await _unitOfWork.Repository<Symptome>().GetByIdAsync(symptomeId, cancellationToken);
            if (symptome is not null)
                intervention.Symptomes.Add(symptome);
        }

        await _unitOfWork.Repository<Intervention>().AddAsync(intervention, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (priorite == Priorite.Critique)
            _logger.LogWarning("DI CRITIQUE {Numero} créée (patient connecté) sur {Serie}", numeroDI, info.NumeroSerie);
        else
            _logger.LogInformation("DI {Numero} créée sur {Serie}", numeroDI, info.NumeroSerie);

        return Result.Succes(new InterventionDto
        {
            Id = intervention.Id,
            NumeroDI = numeroDI,
            Date = intervention.Date,
            RespirateurSerie = info.NumeroSerie,
            Description = requete.Description,
            Etat = etat,
            Priorite = priorite,
            PatientConnecte = requete.PatientConnecte,
            Urgence = intervention.Urgence
        });
    }

    public async Task<Result> ChangerEtatAsync(int id, EtatIntervention nouvelEtat, string auteur, CancellationToken cancellationToken = default)
    {
        // La clôture est une action plus sensible : elle exige sa propre permission.
        var permissionRequise = nouvelEtat == EtatIntervention.Cloturee
            ? Permission.ClorerIntervention
            : Permission.ChangerEtatIntervention;
        var acces = await _autorisation.AutoriserAsync(permissionRequise, cancellationToken);
        if (acces.EstEchec)
            return acces;

        var depot = _unitOfWork.Repository<Intervention>();
        var intervention = await depot.GetByIdAsync(id, cancellationToken);
        if (intervention is null)
            return Result.Echec("Intervention introuvable.");

        if (intervention.Etat == nouvelEtat)
            return Result.Succes();

        // RG-02 : clôture impossible sans check-list complète.
        if (nouvelEtat == EtatIntervention.Cloturee)
        {
            var checkList = await _unitOfWork.Repository<CheckListCloture>()
                .FirstOrDefaultAsync(c => c.InterventionId == id, cancellationToken);
            if (checkList is null || !checkList.EstComplete())
                return Result.Echec("Clôture impossible : la check-list de contrôle doit être entièrement validée.");
        }

        var ancien = intervention.Etat;
        intervention.Etat = nouvelEtat;
        if (nouvelEtat == EtatIntervention.Cloturee)
            intervention.DateCloture = DateTime.UtcNow;

        await _unitOfWork.Repository<HistoriqueEtatIntervention>().AddAsync(new HistoriqueEtatIntervention
        {
            InterventionId = id,
            AncienEtat = ancien,
            NouvelEtat = nouvelEtat,
            Auteur = auteur
        }, cancellationToken);

        depot.Update(intervention);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("DI {Numero} : {Ancien} → {Nouveau} ({Auteur})",
            intervention.NumeroDI, ancien, nouvelEtat, auteur);
        return Result.Succes();
    }

    public async Task<Result> MettreAJourCheckListAsync(int id, CheckListDto dto, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.ChangerEtatIntervention, cancellationToken);
        if (acces.EstEchec)
            return acces;

        var depot = _unitOfWork.Repository<CheckListCloture>();
        var checkList = await depot.FirstOrDefaultAsync(c => c.InterventionId == id, cancellationToken);

        if (checkList is null)
        {
            checkList = new CheckListCloture { InterventionId = id };
            await depot.AddAsync(checkList, cancellationToken);
        }

        checkList.AutotestOk = dto.AutotestOk;
        checkList.TestEtancheite = dto.TestEtancheite;
        checkList.CalibrationDebit = dto.CalibrationDebit;
        checkList.CalibrationO2 = dto.CalibrationO2;
        checkList.Batterie = dto.Batterie;
        checkList.Alimentation = dto.Alimentation;
        checkList.Alarmes = dto.Alarmes;
        checkList.ValidationFinale = dto.ValidationFinale;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Succes();
    }

    public Task<IReadOnlyList<SymptomeDto>> ListerSymptomesAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.Repository<Symptome>().ListerAsync(
            null,
            s => new SymptomeDto { Id = s.Id, Libelle = s.Libelle },
            cancellationToken);
}

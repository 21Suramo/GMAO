using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>Implémentation des cas d'usage du parc des respirateurs.</summary>
public class RespirateurService : IRespirateurService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RespirateurService> _logger;

    public RespirateurService(IUnitOfWork unitOfWork, ILogger<RespirateurService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>Projection partagée entité → DTO (exécutée côté base).</summary>
    private static readonly Expression<Func<Respirateur, RespirateurDto>> Projection = r => new RespirateurDto
    {
        Id = r.Id,
        NumeroSerie = r.NumeroSerie,
        CodeInterne = r.CodeInterne,
        CodeQr = r.CodeQr,
        ModeleNom = r.Modele!.Nom,
        ModeleGamme = r.Modele.Gamme,
        VersionLogicielle = r.VersionLogicielle,
        VersionMaterielle = r.VersionMaterielle,
        Etat = r.Etat,
        DateMiseEnService = r.DateMiseEnService,
        SousContrat = r.SousContrat,
        NumeroContrat = r.NumeroContrat,
        DateFinGarantie = r.DateFinGarantie,
        Localisation = r.BlocOperatoire == null
            ? "Non affecté"
            : r.BlocOperatoire.Service.Hopital.Nom + " · " + r.BlocOperatoire.Service.Nom + " · " + r.BlocOperatoire.Nom,
        MotifHorsService = r.MotifHorsService,
        DateHorsService = r.DateHorsService,
        AuteurHorsService = r.AuteurHorsService
    };

    public Task<IReadOnlyList<RespirateurDto>> ListerAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.Repository<Respirateur>().ListerAsync(null, Projection, cancellationToken);

    public async Task<RespirateurDto?> ObtenirAsync(int id, CancellationToken cancellationToken = default)
    {
        var liste = await _unitOfWork.Repository<Respirateur>().ListerAsync(r => r.Id == id, Projection, cancellationToken);
        return liste.FirstOrDefault();
    }

    public async Task<Result> DeclarerHorsServiceAsync(int id, string motif, string auteur, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(motif))
            return Result.Echec("Le motif de mise hors service est obligatoire.");

        var depot = _unitOfWork.Repository<Respirateur>();
        var respirateur = await depot.GetByIdAsync(id, cancellationToken);
        if (respirateur is null)
            return Result.Echec("Respirateur introuvable.");

        respirateur.Etat = EtatRespirateur.HorsService;
        respirateur.MotifHorsService = motif;
        respirateur.DateHorsService = DateTime.UtcNow;
        respirateur.AuteurHorsService = auteur;
        depot.Update(respirateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("Respirateur {Serie} déclaré HORS SERVICE par {Auteur} : {Motif}",
            respirateur.NumeroSerie, auteur, motif);
        return Result.Succes();
    }

    public async Task<Result> RemettreEnServiceAsync(int id, string auteur, CancellationToken cancellationToken = default)
    {
        var depot = _unitOfWork.Repository<Respirateur>();
        var respirateur = await depot.GetByIdAsync(id, cancellationToken);
        if (respirateur is null)
            return Result.Echec("Respirateur introuvable.");

        respirateur.Etat = EtatRespirateur.EnService;
        respirateur.MotifHorsService = null;
        respirateur.DateHorsService = null;
        respirateur.AuteurHorsService = null;
        depot.Update(respirateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Respirateur {Serie} remis en service par {Auteur}", respirateur.NumeroSerie, auteur);
        return Result.Succes();
    }
}

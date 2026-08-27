using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>Génère et archive les rapports PDF d'intervention.</summary>
public class RapportService : IRapportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRapportPdfGenerateur _generateur;
    private readonly IAutorisationService _autorisation;
    private readonly ILogger<RapportService> _logger;
    private readonly string _dossierRapports;

    public RapportService(IUnitOfWork unitOfWork, IRapportPdfGenerateur generateur, IAutorisationService autorisation, ILogger<RapportService> logger)
    {
        _unitOfWork = unitOfWork;
        _generateur = generateur;
        _autorisation = autorisation;
        _logger = logger;
        _dossierRapports = Path.Combine(AppContext.BaseDirectory, "reports", "generated");
    }

    public async Task<Result<string>> GenererRapportInterventionAsync(int interventionId, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GenererRapport, cancellationToken);
        if (acces.EstEchec)
            return Result.Echec<string>(acces.Erreur);

        var liste = await _unitOfWork.Repository<Intervention>().ListerAsync(i => i.Id == interventionId, ProjectionBrut, cancellationToken);
        var brut = liste.FirstOrDefault();
        if (brut is null)
            return Result.Echec<string>("Intervention introuvable.");

        var data = ConstruireData(brut);

        byte[] pdf;
        try
        {
            pdf = _generateur.Generer(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Échec de génération du PDF pour {Numero}", data.NumeroDI);
            return Result.Echec<string>($"Erreur de génération du PDF : {ex.Message}");
        }

        Directory.CreateDirectory(_dossierRapports);
        var chemin = Path.Combine(_dossierRapports, $"Rapport_{data.NumeroDI}.pdf");
        await File.WriteAllBytesAsync(chemin, pdf, cancellationToken);

        await ArchiverAsync(interventionId, data.NumeroDI, chemin, cancellationToken);

        _logger.LogInformation("Rapport PDF généré : {Chemin}", chemin);
        return Result.Succes(chemin);
    }

    public async Task<IReadOnlyList<RapportDto>> ListerAsync(CancellationToken cancellationToken = default)
    {
        var liste = await _unitOfWork.Repository<Intervention>().ListerAsync(
            null,
            i => new RapportDto
            {
                InterventionId = i.Id,
                NumeroDI = i.NumeroDI,
                Date = i.Date,
                ClientNom = i.Hopital!.Nom,
                AppareilSerie = i.Respirateur!.NumeroSerie,
                Etat = i.Etat,
                ARapport = i.Rapport != null,
                NumeroRapport = i.Rapport != null ? i.Rapport.NumeroRapport : null,
                CheminPdf = i.Rapport != null ? i.Rapport.CheminPdf : null,
                DateGeneration = i.Rapport != null ? (DateTime?)i.Rapport.DateGeneration : null
            },
            cancellationToken);

        return liste.OrderByDescending(r => r.Date).ToList();
    }

    private async Task ArchiverAsync(int interventionId, string numeroDI, string chemin, CancellationToken cancellationToken)
    {
        var depot = _unitOfWork.Repository<Rapport>();
        var existant = await depot.FirstOrDefaultAsync(r => r.InterventionId == interventionId, cancellationToken);
        if (existant is null)
        {
            await depot.AddAsync(new Rapport
            {
                InterventionId = interventionId,
                NumeroRapport = $"RAP-{numeroDI}",
                CheminPdf = chemin,
                DateGeneration = DateTime.UtcNow
            }, cancellationToken);
        }
        else
        {
            existant.CheminPdf = chemin;
            existant.DateGeneration = DateTime.UtcNow;
            depot.Update(existant);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static RapportInterventionData ConstruireData(BrutRapport b)
    {
        var pieces = b.Pieces ?? new List<LignePieceData>();
        var checklist = new List<string>();
        if (b.CkAutotest) checklist.Add("Autotest OK");
        if (b.CkEtancheite) checklist.Add("Test d'étanchéité");
        if (b.CkCalibrationDebit) checklist.Add("Calibration débit");
        if (b.CkCalibrationO2) checklist.Add("Calibration O2");
        if (b.CkBatterie) checklist.Add("Batterie");
        if (b.CkAlimentation) checklist.Add("Alimentation");
        if (b.CkAlarmes) checklist.Add("Alarmes");
        if (b.CkValidationFinale) checklist.Add("Validation finale");

        return new RapportInterventionData
        {
            NumeroDI = b.NumeroDI,
            Date = b.Date,
            DateCloture = b.DateCloture,
            ClientNom = b.ClientNom,
            ClientVille = b.ClientVille,
            AppareilSerie = b.AppareilSerie,
            AppareilModele = b.AppareilModele,
            Description = b.Description,
            Diagnostic = b.Diagnostic,
            Cause = b.Cause,
            Etat = InterventionDto.LibelleEtat(b.Etat),
            Ingenieur = b.Ingenieur,
            TempsDeplacement = b.TempsDeplacement,
            TempsReparation = b.TempsReparation,
            MainOeuvre = b.MainOeuvre,
            CoutPieces = pieces.Sum(p => p.Total),
            Pieces = pieces,
            CheckListValidee = checklist,
            QrContenu = $"GMAO-DI:{b.NumeroDI}"
        };
    }

    private static readonly Expression<Func<Intervention, BrutRapport>> ProjectionBrut = i => new BrutRapport
    {
        NumeroDI = i.NumeroDI,
        Date = i.Date,
        DateCloture = i.DateCloture,
        ClientNom = i.Hopital!.Nom,
        ClientVille = i.Hopital.Ville,
        AppareilSerie = i.Respirateur!.NumeroSerie,
        AppareilModele = i.Respirateur.Modele!.Nom,
        Description = i.Description,
        Diagnostic = i.Diagnostic,
        Cause = i.Cause,
        Etat = i.Etat,
        Ingenieur = i.Ingenieur != null ? i.Ingenieur.Prenom + " " + i.Ingenieur.Nom : null,
        TempsDeplacement = i.TempsDeplacement,
        TempsReparation = i.TempsReparation,
        MainOeuvre = i.MainOeuvre,
        Pieces = i.PiecesUtilisees.Select(l => new LignePieceData
        {
            Reference = l.Piece!.Reference,
            Nom = l.Piece.Nom,
            Quantite = l.Quantite,
            PrixUnitaire = l.PrixUnitaire
        }).ToList(),
        CkAutotest = i.CheckList != null && i.CheckList.AutotestOk,
        CkEtancheite = i.CheckList != null && i.CheckList.TestEtancheite,
        CkCalibrationDebit = i.CheckList != null && i.CheckList.CalibrationDebit,
        CkCalibrationO2 = i.CheckList != null && i.CheckList.CalibrationO2,
        CkBatterie = i.CheckList != null && i.CheckList.Batterie,
        CkAlimentation = i.CheckList != null && i.CheckList.Alimentation,
        CkAlarmes = i.CheckList != null && i.CheckList.Alarmes,
        CkValidationFinale = i.CheckList != null && i.CheckList.ValidationFinale
    };

    /// <summary>Forme brute projetée depuis la base avant mise en forme.</summary>
    private sealed class BrutRapport
    {
        public string NumeroDI { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime? DateCloture { get; set; }
        public string ClientNom { get; set; } = string.Empty;
        public string ClientVille { get; set; } = string.Empty;
        public string AppareilSerie { get; set; } = string.Empty;
        public string AppareilModele { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Diagnostic { get; set; }
        public string? Cause { get; set; }
        public EtatIntervention Etat { get; set; }
        public string? Ingenieur { get; set; }
        public int TempsDeplacement { get; set; }
        public int TempsReparation { get; set; }
        public decimal MainOeuvre { get; set; }
        public List<LignePieceData>? Pieces { get; set; }
        public bool CkAutotest { get; set; }
        public bool CkEtancheite { get; set; }
        public bool CkCalibrationDebit { get; set; }
        public bool CkCalibrationO2 { get; set; }
        public bool CkBatterie { get; set; }
        public bool CkAlimentation { get; set; }
        public bool CkAlarmes { get; set; }
        public bool CkValidationFinale { get; set; }
    }
}

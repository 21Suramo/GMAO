using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Application.Services.TableauBord;
using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Pieces;
using GMAO.Domain.Entities.Planning;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Services;

/// <summary>Calcule les indicateurs du tableau de bord à partir des dépôts (projections côté base).</summary>
public class TableauBordService : ITableauBordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAutorisationService _autorisation;

    public TableauBordService(IUnitOfWork unitOfWork, IAutorisationService autorisation)
    {
        _unitOfWork = unitOfWork;
        _autorisation = autorisation;
    }

    public async Task<Result<TableauBordDto>> ObtenirAsync(TableauBordFiltre filtre, CancellationToken cancellationToken = default)
    {
        // Contrôle d'accès : consultation du tableau de bord (et vue globale le cas échéant).
        var acces = await _autorisation.AutoriserAsync(Permission.ConsulterTableauBord, cancellationToken);
        if (acces.EstEchec)
            return Result.Echec<TableauBordDto>(acces.Erreur);

        var fin = filtre.Fin ?? DateTime.UtcNow;
        var debut = filtre.Debut ?? fin.AddDays(-30);
        if (debut > fin) (debut, fin) = (fin, debut);
        var debutJour = DateTime.UtcNow.Date;

        var vueGlobale = filtre.VueGlobale;
        if (vueGlobale)
        {
            var accesGlobal = await _autorisation.AutoriserAsync(Permission.ConsulterTableauBordGlobal, cancellationToken);
            if (accesGlobal.EstEchec)
                return Result.Echec<TableauBordDto>(accesGlobal.Erreur);
        }
        int? ing = vueGlobale ? null : filtre.IngenieurId;

        var interventions = _unitOfWork.Repository<Intervention>();

        // --- Instantané des interventions ouvertes (indépendant de la période) ---
        Expression<Func<Intervention, bool>> filtreOuvert = ing.HasValue
            ? i => i.Etat != EtatIntervention.Cloturee && i.Etat != EtatIntervention.Annulee && i.IngenieurId == ing
            : i => i.Etat != EtatIntervention.Cloturee && i.Etat != EtatIntervention.Annulee;

        var ouvertes = await interventions.ListerAsync(filtreOuvert, i => new LigneOuverte
        {
            Date = i.Date,
            Priorite = i.Priorite,
            AIngenieur = i.IngenieurId != null,
            IngenieurNom = i.Ingenieur != null ? i.Ingenieur.Prenom + " " + i.Ingenieur.Nom : null
        }, cancellationToken);

        // --- Interventions de la période (créées OU clôturées dans la fenêtre) ---
        Expression<Func<Intervention, bool>> filtrePeriode = ing.HasValue
            ? i => i.IngenieurId == ing &&
                   ((i.Date >= debut && i.Date <= fin) ||
                    (i.DateCloture != null && i.DateCloture >= debut && i.DateCloture <= fin))
            : i => (i.Date >= debut && i.Date <= fin) ||
                   (i.DateCloture != null && i.DateCloture >= debut && i.DateCloture <= fin);

        var periode = await interventions.ListerAsync(filtrePeriode, i => new LignePeriode
        {
            Date = i.Date,
            DateCloture = i.DateCloture,
            Etat = i.Etat,
            Priorite = i.Priorite,
            ModeleNom = i.Respirateur!.Modele!.Nom,
            RespirateurSerie = i.Respirateur.NumeroSerie,
            HopitalNom = i.Hopital!.Nom,
            IngenieurNom = i.Ingenieur != null ? i.Ingenieur.Prenom + " " + i.Ingenieur.Nom : null,
            MainOeuvre = i.MainOeuvre,
            CoutPieces = i.PiecesUtilisees.Sum(l => l.PrixUnitaire * l.Quantite),
            ImmobilisationMinutes = i.TempsDeplacement + i.TempsReparation,
            Symptomes = i.Symptomes.Select(s => s.Libelle).ToList(),
            DatesAffectation = i.HistoriqueEtats
                .Where(h => h.NouvelEtat == EtatIntervention.Affectee)
                .Select(h => h.DateChangement).ToList()
        }, cancellationToken);

        var creesPeriode = periode.Where(p => p.Date >= debut && p.Date <= fin).ToList();
        var cloturesPeriode = periode.Where(p => p.DateCloture.HasValue && p.DateCloture >= debut && p.DateCloture <= fin).ToList();

        // Pannes déclarées aujourd'hui (snapshot indépendant de la période sélectionnée).
        Expression<Func<Intervention, bool>> filtreAujourdhui = ing.HasValue
            ? i => i.IngenieurId == ing && i.Date >= debutJour
            : i => i.Date >= debutJour;
        var pannesAujourdhui = await interventions.CountAsync(filtreAujourdhui, cancellationToken);

        var dto = new TableauBordDto
        {
            PeriodeLibelle = $"{debut.ToLocalTime():dd/MM/yyyy} → {fin.ToLocalTime():dd/MM/yyyy}",
            PerimetreLibelle = vueGlobale ? "Vue globale" : "Mon activité",

            // Flux
            InterventionsActives = ouvertes.Count,
            EnAttenteAffectation = ouvertes.Count(o => !o.AIngenieur),
            InterventionsUrgentes = ouvertes.Count(o => o.Priorite is Priorite.Critique or Priorite.Haute),
            EnDepassementSla = ouvertes.Count(o =>
                CalculateurKpi.EstEnDepassementSla(o.Priorite, (fin - o.Date).TotalHours)),
            InterventionsCloturees = cloturesPeriode.Count,
            PannesAujourdhui = pannesAujourdhui,

            // Performance
            MttrHeures = CalculateurKpi.Mttr(cloturesPeriode.Select(p => (p.Date, p.DateCloture))),
            DelaiMoyenAffectationHeures = CalculateurKpi.DelaiMoyenAffectation(
                creesPeriode.Select(p => (p.Date, p.DatesAffectation.Count == 0 ? (DateTime?)null : p.DatesAffectation.Min()))),
            CoutCumule = cloturesPeriode.Sum(p => p.MainOeuvre + p.CoutPieces),

            // Répartitions
            RepartitionParEtat = creesPeriode
                .GroupBy(p => p.Etat)
                .Select(g => new CategorieValeur { Libelle = InterventionDto.LibelleEtat(g.Key), Valeur = g.Count() })
                .ToList(),
            InterventionsParModele = creesPeriode
                .GroupBy(p => p.ModeleNom)
                .Select(g => new CategorieValeur { Libelle = g.Key, Valeur = g.Count() })
                .OrderByDescending(c => c.Valeur).ToList(),
            ParetoPannes = creesPeriode
                .SelectMany(p => p.Symptomes)
                .GroupBy(s => s)
                .Select(g => new CategorieValeur { Libelle = g.Key, Valeur = g.Count() })
                .OrderByDescending(c => c.Valeur).Take(8).ToList(),
            DisponibiliteParEquipement = ConstruireDisponibilites(periode, debut, fin),
            Top5Respirateurs = creesPeriode
                .GroupBy(p => p.RespirateurSerie)
                .Select(g => new CategorieValeur { Libelle = g.Key, Valeur = g.Count() })
                .OrderByDescending(c => c.Valeur).Take(5).ToList(),
            Top5Hopitaux = creesPeriode
                .GroupBy(p => p.HopitalNom)
                .Select(g => new CategorieValeur { Libelle = g.Key, Valeur = g.Count() })
                .OrderByDescending(c => c.Valeur).Take(5).ToList(),
            ChargeParTechnicien = ConstruireCharge(ouvertes, cloturesPeriode)
        };

        // --- Contexte parc / stock (uniquement en vue globale) ---
        if (vueGlobale)
        {
            var respirateurs = _unitOfWork.Repository<Respirateur>();
            int total = await respirateurs.CountAsync(null, cancellationToken);
            int enService = await respirateurs.CountAsync(r => r.Etat == EtatRespirateur.EnService, cancellationToken);
            int horsService = await respirateurs.CountAsync(r => r.Etat == EtatRespirateur.HorsService, cancellationToken);

            dto.NombreRespirateurs = total;
            dto.RespirateursEnService = enService;
            dto.RespirateursHorsService = horsService;
            dto.DisponibiliteGlobale = total == 0 ? 100d : Math.Round((double)enService / total * 100d, 1);
            dto.PiecesEnAlerte = await _unitOfWork.Repository<Piece>().CountAsync(p => p.Stock <= p.StockMinimum, cancellationToken);
            dto.NombreHopitaux = await _unitOfWork.Repository<Hopital>().CountAsync(null, cancellationToken);
            dto.NombreIngenieurs = await _unitOfWork.Repository<Ingenieur>().CountAsync(null, cancellationToken);
        }

        return Result.Succes(dto);
    }

    private static List<CategorieValeur> ConstruireDisponibilites(IReadOnlyList<LignePeriode> periode, DateTime debut, DateTime fin)
    {
        var periodeMinutes = (fin - debut).TotalMinutes;
        return periode
            .GroupBy(p => p.RespirateurSerie)
            .Select(g => new CategorieValeur
            {
                Libelle = g.Key,
                Valeur = (int)Math.Round(CalculateurKpi.DisponibilitePourcent(g.Sum(x => x.ImmobilisationMinutes), periodeMinutes))
            })
            .OrderBy(c => c.Valeur)
            .Take(10)
            .ToList();
    }

    private static List<ChargeTechnicien> ConstruireCharge(IReadOnlyList<LigneOuverte> ouvertes, IReadOnlyList<LignePeriode> clotures)
    {
        var noms = ouvertes.Where(o => o.IngenieurNom != null).Select(o => o.IngenieurNom!)
            .Concat(clotures.Where(c => c.IngenieurNom != null).Select(c => c.IngenieurNom!))
            .Distinct();

        return noms
            .Select(nom => new ChargeTechnicien
            {
                Nom = nom,
                EnCours = ouvertes.Count(o => o.IngenieurNom == nom),
                Terminees = clotures.Count(c => c.IngenieurNom == nom)
            })
            .OrderByDescending(c => c.EnCours + c.Terminees)
            .ToList();
    }

    private sealed class LigneOuverte
    {
        public DateTime Date { get; set; }
        public Priorite Priorite { get; set; }
        public bool AIngenieur { get; set; }
        public string? IngenieurNom { get; set; }
    }

    private sealed class LignePeriode
    {
        public DateTime Date { get; set; }
        public DateTime? DateCloture { get; set; }
        public EtatIntervention Etat { get; set; }
        public Priorite Priorite { get; set; }
        public string ModeleNom { get; set; } = string.Empty;
        public string RespirateurSerie { get; set; } = string.Empty;
        public string HopitalNom { get; set; } = string.Empty;
        public string? IngenieurNom { get; set; }
        public decimal MainOeuvre { get; set; }
        public decimal CoutPieces { get; set; }
        public int ImmobilisationMinutes { get; set; }
        public List<string> Symptomes { get; set; } = new();
        public List<DateTime> DatesAffectation { get; set; } = new();
    }
}

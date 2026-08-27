using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Application.Services.TableauBord;
using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Services;

/// <summary>Calcule les statistiques avancées à partir de projections côté base.</summary>
public class StatistiquesService : IStatistiquesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAutorisationService _autorisation;

    public StatistiquesService(IUnitOfWork unitOfWork, IAutorisationService autorisation)
    {
        _unitOfWork = unitOfWork;
        _autorisation = autorisation;
    }

    public async Task<Result<StatistiquesDto>> ObtenirAsync(StatistiquesFiltre filtre, CancellationToken cancellationToken = default)
    {
        // Les statistiques avancées sont des analyses globales : vue globale requise.
        var acces = await _autorisation.AutoriserAsync(Permission.ConsulterTableauBordGlobal, cancellationToken);
        if (acces.EstEchec)
            return Result.Echec<StatistiquesDto>(acces.Erreur);

        var fin = filtre.Fin ?? DateTime.UtcNow;
        var debut = filtre.Debut ?? fin.AddMonths(-12);
        if (debut > fin) (debut, fin) = (fin, debut);

        // Interventions créées OU clôturées dans la fenêtre (projection légère).
        var lignes = await _unitOfWork.Repository<Intervention>().ListerAsync(
            i => (i.Date >= debut && i.Date <= fin) ||
                 (i.DateCloture != null && i.DateCloture >= debut && i.DateCloture <= fin),
            i => new Ligne
            {
                Date = i.Date,
                DateCloture = i.DateCloture,
                ModeleNom = i.Respirateur!.Modele!.Nom,
                HopitalNom = i.Hopital!.Nom,
                IngenieurNom = i.Ingenieur != null ? i.Ingenieur.Prenom + " " + i.Ingenieur.Nom : null,
                MainOeuvre = i.MainOeuvre,
                CoutPieces = i.PiecesUtilisees.Sum(l => l.PrixUnitaire * l.Quantite),
                ImmobilisationMinutes = i.TempsDeplacement + i.TempsReparation
            }, cancellationToken);

        var creees = lignes.Where(l => l.Date >= debut && l.Date <= fin).ToList();
        var cloturees = lignes.Where(l => l.DateCloture.HasValue && l.DateCloture >= debut && l.DateCloture <= fin).ToList();

        // Squelette des mois de la fenêtre (pour ne pas « trouer » les courbes).
        var mois = MoisEntre(debut, fin);

        var dto = new StatistiquesDto
        {
            PeriodeLibelle = $"{debut.ToLocalTime():MM/yyyy} → {fin.ToLocalTime():MM/yyyy}",

            // 1. Évolution des pannes par mois (interventions créées).
            PannesParMois = mois.Select(m => new PointMensuel
            {
                Mois = m.Libelle,
                Valeur = creees.Count(l => Cle(l.Date) == m.Cle)
            }).ToList(),

            // 2. Coût mensuel des interventions (clôturées, main d'œuvre + pièces).
            CoutParMois = mois.Select(m => new CoutMensuel
            {
                Mois = m.Libelle,
                Cout = cloturees.Where(l => Cle(l.DateCloture!.Value) == m.Cle).Sum(l => l.MainOeuvre + l.CoutPieces)
            }).ToList(),

            // 3. Disponibilité par hôpital (immobilisation cumulée sur la période).
            DisponibiliteParHopital = creees
                .GroupBy(l => l.HopitalNom)
                .Select(g => new CategorieValeur
                {
                    Libelle = g.Key,
                    Valeur = (int)Math.Round(CalculateurKpi.DisponibilitePourcent(
                        g.Sum(x => x.ImmobilisationMinutes), (fin - debut).TotalMinutes))
                })
                .OrderBy(c => c.Valeur).ToList(),

            // 4. Durée moyenne d'intervention par ingénieur (déplacement + réparation).
            DureeMoyenneParIngenieur = creees
                .Where(l => l.IngenieurNom != null && l.ImmobilisationMinutes > 0)
                .GroupBy(l => l.IngenieurNom!)
                .Select(g => new DureeIngenieur
                {
                    Nom = g.Key,
                    DureeMoyenneHeures = Math.Round(g.Average(x => x.ImmobilisationMinutes) / 60d, 1)
                })
                .OrderByDescending(d => d.DureeMoyenneHeures).ToList(),

            // 5. Comparaison entre modèles de respirateurs.
            ComparaisonModeles = creees
                .GroupBy(l => l.ModeleNom)
                .Select(g => new ComparaisonModele
                {
                    Modele = g.Key,
                    NombreInterventions = g.Count(),
                    MttrHeures = CalculateurKpi.Mttr(g.Select(x => (x.Date, x.DateCloture))),
                    CoutMoyen = CoutMoyen(cloturees.Where(l => l.ModeleNom == g.Key).ToList())
                })
                .OrderByDescending(c => c.NombreInterventions).ToList()
        };

        return Result.Succes(dto);
    }

    private static decimal CoutMoyen(IReadOnlyList<Ligne> lignes)
        => lignes.Count == 0 ? 0m : Math.Round(lignes.Average(l => l.MainOeuvre + l.CoutPieces), 0);

    private static string Cle(DateTime date) => $"{date.Year:D4}-{date.Month:D2}";

    private static List<(string Cle, string Libelle)> MoisEntre(DateTime debut, DateTime fin)
    {
        var resultat = new List<(string, string)>();
        var curseur = new DateTime(debut.Year, debut.Month, 1);
        var borne = new DateTime(fin.Year, fin.Month, 1);
        while (curseur <= borne)
        {
            resultat.Add(($"{curseur.Year:D4}-{curseur.Month:D2}", $"{curseur.Month:D2}/{curseur.Year}"));
            curseur = curseur.AddMonths(1);
        }
        return resultat;
    }

    private sealed class Ligne
    {
        public DateTime Date { get; set; }
        public DateTime? DateCloture { get; set; }
        public string ModeleNom { get; set; } = string.Empty;
        public string HopitalNom { get; set; } = string.Empty;
        public string? IngenieurNom { get; set; }
        public decimal MainOeuvre { get; set; }
        public decimal CoutPieces { get; set; }
        public int ImmobilisationMinutes { get; set; }
    }
}

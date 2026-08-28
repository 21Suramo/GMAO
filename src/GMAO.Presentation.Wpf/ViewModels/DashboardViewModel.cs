using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Planning;
using GMAO.Domain.Enums;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>Une période prédéfinie sélectionnable pour le tableau de bord.</summary>
public record PeriodeOption(string Libelle, int Jours);

/// <summary>Ligne d'un classement (top N) avec barre proportionnelle pré-calculée.</summary>
public record ClassementItem(int Rang, string Libelle, int Valeur, double LargeurBarre);

/// <summary>Ligne de disponibilité d'un équipement avec barre et couleur pré-calculées.</summary>
public record DisponibiliteItem(string Libelle, int Pourcentage, double LargeurBarre, string Couleur);

/// <summary>ViewModel du tableau de bord : charge et expose les indicateurs (KPI) et graphiques.</summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty] private TableauBordDto? _kpi;
    [ObservableProperty] private bool _chargement;
    [ObservableProperty] private bool _vueGlobale;

    [ObservableProperty] private ISeries[] _seriesEtats = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] _seriesModeles = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _axesModeles = Array.Empty<Axis>();

    private const double LargeurBarreMax = 170d;

    public ObservableCollection<ClassementItem> ParetoPannes { get; } = new();
    public ObservableCollection<ChargeTechnicien> ChargeParTechnicien { get; } = new();
    public ObservableCollection<ClassementItem> Top5Respirateurs { get; } = new();
    public ObservableCollection<ClassementItem> Top5Hopitaux { get; } = new();
    public ObservableCollection<DisponibiliteItem> DisponibiliteParEquipement { get; } = new();

    public PeriodeOption[] Periodes { get; } =
    {
        new("30 derniers jours", 30),
        new("90 derniers jours", 90),
        new("12 derniers mois", 365)
    };

    [ObservableProperty] private PeriodeOption _periodeSelectionnee;

    /// <summary>
    /// ViewModel des statistiques avancées, exposé comme onglet « Analytique » du tableau de bord
    /// (l'accès a été fusionné ici ; la vue est résolue via le DataTemplate VM→View habituel).
    /// </summary>
    public StatistiquesViewModel Statistiques { get; }

    public DashboardViewModel(IServiceScopeFactory scopeFactory, ICurrentUserService currentUser, StatistiquesViewModel statistiques)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
        Statistiques = statistiques;
        _periodeSelectionnee = Periodes[0];
        _ = ChargerAsync();
    }

    partial void OnPeriodeSelectionneeChanged(PeriodeOption value) => _ = ChargerAsync();

    [RelayCommand]
    private async Task ChargerAsync()
    {
        Chargement = true;
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var autorisation = scope.ServiceProvider.GetRequiredService<IAutorisationService>();
            var service = scope.ServiceProvider.GetRequiredService<ITableauBordService>();

            // Périmètre : vue globale si le rôle le permet, sinon activité personnelle.
            VueGlobale = autorisation.ADroit(Permission.ConsulterTableauBordGlobal);

            int? ingenieurId = null;
            if (!VueGlobale)
            {
                var uid = _currentUser.Utilisateur?.Id;
                if (uid is not null)
                {
                    var lien = await scope.ServiceProvider.GetRequiredService<IRepository<Ingenieur>>()
                        .ListerAsync(i => i.UtilisateurId == uid, i => i.Id);
                    ingenieurId = lien.Count > 0 ? lien[0] : -1; // -1 : aucune intervention liée
                }
            }

            var filtre = new TableauBordFiltre
            {
                Debut = DateTime.UtcNow.AddDays(-PeriodeSelectionnee.Jours),
                Fin = DateTime.UtcNow,
                VueGlobale = VueGlobale,
                IngenieurId = ingenieurId
            };

            var resultat = await service.ObtenirAsync(filtre);
            if (resultat.EstEchec || resultat.Valeur is null)
                return;

            Kpi = resultat.Valeur;

            // Chargement progressif : listes puis graphiques.
            RemplirClassement(ParetoPannes, Kpi.ParetoPannes);
            RemplirClassement(Top5Respirateurs, Kpi.Top5Respirateurs);
            RemplirClassement(Top5Hopitaux, Kpi.Top5Hopitaux);
            RemplirDisponibilites(Kpi.DisponibiliteParEquipement);
            ChargeParTechnicien.Clear();
            foreach (var c in Kpi.ChargeParTechnicien) ChargeParTechnicien.Add(c);

            ConstruireGraphiques(Kpi);
        }
        finally
        {
            Chargement = false;
        }
    }

    private static void RemplirClassement(ObservableCollection<ClassementItem> cible, IReadOnlyList<CategorieValeur> source)
    {
        cible.Clear();
        var max = source.Count == 0 ? 1 : source.Max(c => c.Valeur);
        if (max == 0) max = 1;
        for (var i = 0; i < source.Count; i++)
            cible.Add(new ClassementItem(i + 1, source[i].Libelle, source[i].Valeur,
                Math.Max(4d, source[i].Valeur / (double)max * LargeurBarreMax)));
    }

    private void RemplirDisponibilites(IReadOnlyList<CategorieValeur> source)
    {
        DisponibiliteParEquipement.Clear();
        foreach (var d in source)
        {
            var couleur = d.Valeur >= 90 ? "#2E7D32" : d.Valeur >= 70 ? "#F9A825" : "#C62828";
            DisponibiliteParEquipement.Add(new DisponibiliteItem(
                d.Libelle, d.Valeur, Math.Max(4d, d.Valeur / 100d * LargeurBarreMax), couleur));
        }
    }

    private void ConstruireGraphiques(TableauBordDto kpi)
    {
        SeriesEtats = kpi.RepartitionParEtat
            .Select(c => (ISeries)new PieSeries<int>
            {
                Values = new[] { c.Valeur },
                Name = c.Libelle
            })
            .ToArray();

        SeriesModeles = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Interventions",
                Values = kpi.InterventionsParModele.Select(c => c.Valeur).ToArray()
            }
        };
        AxesModeles = new[]
        {
            new Axis { Labels = kpi.InterventionsParModele.Select(c => c.Libelle).ToArray() }
        };
    }
}

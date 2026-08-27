using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel des statistiques avancées (analyses multi-mois et comparaisons).</summary>
public partial class StatistiquesViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    [ObservableProperty] private bool _chargement;
    [ObservableProperty] private string? _message;
    [ObservableProperty] private string _periodeLibelle = string.Empty;

    // 1. Évolution des pannes par mois
    [ObservableProperty] private ISeries[] _seriesPannes = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _axesPannes = Array.Empty<Axis>();

    // 2. Coût mensuel des interventions
    [ObservableProperty] private ISeries[] _seriesCout = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _axesCout = Array.Empty<Axis>();

    // 3. Disponibilité par hôpital
    [ObservableProperty] private ISeries[] _seriesDispoHopital = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _axesDispoHopital = Array.Empty<Axis>();

    // 4. Durée moyenne d'intervention par ingénieur
    [ObservableProperty] private ISeries[] _seriesDuree = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _axesDuree = Array.Empty<Axis>();

    // 5. Comparaison entre modèles (graphique + tableau)
    [ObservableProperty] private ISeries[] _seriesModeles = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _axesModeles = Array.Empty<Axis>();
    public ObservableCollection<ComparaisonModele> ComparaisonModeles { get; } = new();

    public StatistiquesViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _ = ChargerAsync();
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        Chargement = true;
        Message = null;
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IStatistiquesService>();
            var resultat = await service.ObtenirAsync(new StatistiquesFiltre());

            if (resultat.EstEchec || resultat.Valeur is null)
            {
                Message = resultat.Erreur;
                return;
            }

            Construire(resultat.Valeur);
        }
        finally
        {
            Chargement = false;
        }
    }

    private void Construire(StatistiquesDto s)
    {
        PeriodeLibelle = s.PeriodeLibelle;

        // 1. Pannes par mois (colonnes).
        SeriesPannes = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Pannes",
                Values = s.PannesParMois.Select(m => m.Valeur).ToArray(),
                Fill = new SolidColorPaint(new SKColor(0xE5, 0x39, 0x35))
            }
        };
        AxesPannes = AxeLabels(s.PannesParMois.Select(m => m.Mois));

        // 2. Coût mensuel (ligne).
        SeriesCout = new ISeries[]
        {
            new LineSeries<double>
            {
                Name = "Coût (MAD)",
                Values = s.CoutParMois.Select(m => (double)m.Cout).ToArray(),
                Stroke = new SolidColorPaint(new SKColor(0x37, 0x47, 0x4F)) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(new SKColor(0x37, 0x47, 0x4F, 0x22)),
                GeometrySize = 8
            }
        };
        AxesCout = AxeLabels(s.CoutParMois.Select(m => m.Mois));

        // 3. Disponibilité par hôpital (colonnes, %).
        SeriesDispoHopital = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Disponibilité (%)",
                Values = s.DisponibiliteParHopital.Select(h => h.Valeur).ToArray(),
                Fill = new SolidColorPaint(new SKColor(0x02, 0x77, 0xBD))
            }
        };
        AxesDispoHopital = AxeLabels(s.DisponibiliteParHopital.Select(h => h.Libelle));

        // 4. Durée moyenne par ingénieur (colonnes, heures).
        SeriesDuree = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "Durée moyenne (h)",
                Values = s.DureeMoyenneParIngenieur.Select(d => d.DureeMoyenneHeures).ToArray(),
                Fill = new SolidColorPaint(new SKColor(0x6A, 0x1B, 0x9A))
            }
        };
        AxesDuree = AxeLabels(s.DureeMoyenneParIngenieur.Select(d => d.Nom));

        // 5. Comparaison entre modèles (colonnes = nb interventions + tableau détaillé).
        SeriesModeles = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Interventions",
                Values = s.ComparaisonModeles.Select(m => m.NombreInterventions).ToArray(),
                Fill = new SolidColorPaint(new SKColor(0x00, 0x83, 0x8F))
            }
        };
        AxesModeles = AxeLabels(s.ComparaisonModeles.Select(m => m.Modele));

        ComparaisonModeles.Clear();
        foreach (var m in s.ComparaisonModeles) ComparaisonModeles.Add(m);
    }

    private static Axis[] AxeLabels(IEnumerable<string> labels) => new[]
    {
        new Axis
        {
            Labels = labels.ToArray(),
            LabelsRotation = 15,
            TextSize = 11
        }
    };
}

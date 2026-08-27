using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel du module « Rapports » : liste, génération et ouverture des PDF.</summary>
public partial class RapportsViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<RapportDto> Rapports { get; } = new();

    [ObservableProperty] private RapportDto? _selection;
    [ObservableProperty] private string? _message;
    [ObservableProperty] private bool _chargement;

    public RapportsViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _ = ChargerAsync();
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        Chargement = true;
        try
        {
            var idCourant = Selection?.InterventionId;
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IRapportService>();
            var liste = await service.ListerAsync();

            Rapports.Clear();
            foreach (var r in liste) Rapports.Add(r);

            Selection = idCourant is null
                ? Rapports.FirstOrDefault()
                : Rapports.FirstOrDefault(r => r.InterventionId == idCourant) ?? Rapports.FirstOrDefault();
        }
        finally
        {
            Chargement = false;
        }
    }

    [RelayCommand]
    private async Task GenererAsync()
    {
        if (Selection is null) return;

        string? chemin = null;
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IRapportService>();
            var resultat = await service.GenererRapportInterventionAsync(Selection.InterventionId);
            if (resultat.EstEchec) { Message = resultat.Erreur; return; }
            chemin = resultat.Valeur;
        }

        Message = $"Rapport généré : {Path.GetFileName(chemin)}";
        await ChargerAsync();
        Ouvrir(chemin);
    }

    [RelayCommand]
    private void Ouvrir()
    {
        if (Selection?.CheminPdf is { } chemin) Ouvrir(chemin);
        else Message = "Aucun rapport à ouvrir pour cette intervention.";
    }

    private void Ouvrir(string? chemin)
    {
        if (string.IsNullOrWhiteSpace(chemin) || !File.Exists(chemin))
        {
            Message = "Fichier PDF introuvable.";
            return;
        }
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(chemin) { UseShellExecute = true });
        }
        catch
        {
            Message = "Impossible d'ouvrir le fichier.";
        }
    }
}

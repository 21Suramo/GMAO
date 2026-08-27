using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel du module « Parc des respirateurs » (liste + fiche + QR Code).</summary>
public partial class ParcViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserService _currentUser;

    public ObservableCollection<RespirateurDto> Respirateurs { get; } = new();

    [ObservableProperty] private RespirateurDto? _selection;
    [ObservableProperty] private ImageSource? _qrCodeImage;
    [ObservableProperty] private string _motifHorsService = string.Empty;
    [ObservableProperty] private bool _chargement;
    [ObservableProperty] private string? _message;

    public ParcViewModel(IServiceScopeFactory scopeFactory, ICurrentUserService currentUser)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
        _ = ChargerAsync();
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        Chargement = true;
        Message = null;
        try
        {
            var idCourant = Selection?.Id;
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IRespirateurService>();
            var liste = await service.ListerAsync();

            Respirateurs.Clear();
            foreach (var r in liste)
                Respirateurs.Add(r);

            Selection = idCourant is null
                ? Respirateurs.FirstOrDefault()
                : Respirateurs.FirstOrDefault(r => r.Id == idCourant) ?? Respirateurs.FirstOrDefault();
        }
        finally
        {
            Chargement = false;
        }
    }

    partial void OnSelectionChanged(RespirateurDto? value)
    {
        MotifHorsService = string.Empty;
        Message = null;
        GenererQrCode(value);
    }

    private void GenererQrCode(RespirateurDto? respirateur)
    {
        if (respirateur is null)
        {
            QrCodeImage = null;
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var qr = scope.ServiceProvider.GetRequiredService<IQrCodeService>();
        var png = qr.GenererPng(respirateur.CodeQrTexte, pixelsParModule: 10);
        QrCodeImage = VersImage(png);
    }

    [RelayCommand]
    private async Task DeclarerHorsServiceAsync()
    {
        if (Selection is null) return;

        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IRespirateurService>();
            var auteur = _currentUser.Utilisateur?.NomComplet ?? "Inconnu";
            var resultat = await service.DeclarerHorsServiceAsync(Selection.Id, MotifHorsService, auteur);
            if (resultat.EstEchec)
            {
                Message = resultat.Erreur;
                return;
            }
        }

        Message = "Respirateur déclaré hors service.";
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task RemettreEnServiceAsync()
    {
        if (Selection is null) return;

        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IRespirateurService>();
            var auteur = _currentUser.Utilisateur?.NomComplet ?? "Inconnu";
            var resultat = await service.RemettreEnServiceAsync(Selection.Id, auteur);
            if (resultat.EstEchec)
            {
                Message = resultat.Erreur;
                return;
            }
        }

        Message = "Respirateur remis en service.";
        await ChargerAsync();
    }

    private static ImageSource VersImage(byte[] png)
    {
        var image = new BitmapImage();
        using var flux = new MemoryStream(png);
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = flux;
        image.EndInit();
        image.Freeze();
        return image;
    }
}

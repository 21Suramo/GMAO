using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel du module « Pièces détachées » (stock, alertes, mouvements).</summary>
public partial class PiecesViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserService _currentUser;

    public ObservableCollection<PieceDto> Pieces { get; } = new();
    public TypeMouvement[] TypesMouvement { get; } = { TypeMouvement.Entree, TypeMouvement.Sortie, TypeMouvement.Ajustement };

    [ObservableProperty] private PieceDto? _selection;
    [ObservableProperty] private TypeMouvement _typeMouvement = TypeMouvement.Entree;
    [ObservableProperty] private int _quantite = 1;
    [ObservableProperty] private string? _motif;
    [ObservableProperty] private string? _message;
    [ObservableProperty] private int _nombreAlertes;

    public PiecesViewModel(IServiceScopeFactory scopeFactory, ICurrentUserService currentUser)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
        _ = ChargerAsync();
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        var idCourant = Selection?.Id;
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPieceService>();
        var liste = await service.ListerAsync();

        Pieces.Clear();
        foreach (var p in liste) Pieces.Add(p);
        NombreAlertes = Pieces.Count(p => p.EnAlerte || p.EstPerime);

        Selection = idCourant is null
            ? Pieces.FirstOrDefault()
            : Pieces.FirstOrDefault(p => p.Id == idCourant) ?? Pieces.FirstOrDefault();
    }

    partial void OnSelectionChanged(PieceDto? value) => Message = null;

    [RelayCommand]
    private async Task EnregistrerMouvementAsync()
    {
        if (Selection is null) return;

        var auteur = _currentUser.Utilisateur?.NomComplet ?? "Inconnu";
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IPieceService>();
            var resultat = await service.EnregistrerMouvementAsync(Selection.Id, TypeMouvement, Quantite, Motif, auteur);
            if (resultat.EstEchec) { Message = resultat.Erreur; return; }
        }

        Message = $"Mouvement enregistré ({TypeMouvement}, {Quantite}).";
        Motif = null;
        Quantite = 1;
        await ChargerAsync();
    }
}

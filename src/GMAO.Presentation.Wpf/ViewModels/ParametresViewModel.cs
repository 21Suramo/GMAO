using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.Common.Validation;
using GMAO.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel du module « Paramètres » : profil, sécurité, historique, à propos.</summary>
public partial class ParametresViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserService _currentUser;

    public ObservableCollection<HistoriqueConnexionDto> Historique { get; } = new();

    [ObservableProperty] private string _ancienMotDePasse = string.Empty;
    [ObservableProperty] private string _nouveauMotDePasse = string.Empty;
    [ObservableProperty] private string _confirmationMotDePasse = string.Empty;
    [ObservableProperty] private string? _messageMotDePasse;

    public ParametresViewModel(IServiceScopeFactory scopeFactory, ICurrentUserService currentUser)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
        _ = ChargerHistoriqueAsync();
    }

    // Profil
    public string UtilisateurNom => _currentUser.Utilisateur?.NomComplet ?? "—";
    public string Login => _currentUser.Utilisateur?.Login ?? "—";
    public string RoleLibelle => _currentUser.Utilisateur?.RoleLibelle ?? "—";
    public string Email => _currentUser.Utilisateur?.Email ?? "—";

    // À propos
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
    public string CheminBase => Path.Combine(AppContext.BaseDirectory, "gmao.db");
    public string ServeurNotifications => "ws://localhost:4000/ws";

    private async Task ChargerHistoriqueAsync()
    {
        var id = _currentUser.Utilisateur?.Id;
        if (id is null) return;

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();
        var liste = await service.HistoriqueConnexionsAsync(id.Value);

        Historique.Clear();
        foreach (var h in liste) Historique.Add(h);
    }

    [RelayCommand]
    private async Task ChangerMotDePasseAsync()
    {
        MessageMotDePasse = null;
        var id = _currentUser.Utilisateur?.Id;
        if (id is null) return;

        if (NouveauMotDePasse != ConfirmationMotDePasse)
        {
            MessageMotDePasse = "La confirmation ne correspond pas au nouveau mot de passe.";
            return;
        }

        var erreurComplexite = RegleMotDePasse.Valider(NouveauMotDePasse);
        if (erreurComplexite is not null)
        {
            MessageMotDePasse = erreurComplexite;
            return;
        }

        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();
            var resultat = await service.ChangerMotDePasseAsync(id.Value, AncienMotDePasse, NouveauMotDePasse);
            if (resultat.EstEchec) { MessageMotDePasse = resultat.Erreur; return; }
        }

        MessageMotDePasse = "Mot de passe modifié avec succès.";
        AncienMotDePasse = string.Empty;
        NouveauMotDePasse = string.Empty;
        ConfirmationMotDePasse = string.Empty;
        await ChargerHistoriqueAsync();
    }
}

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>Option de rôle pour les listes déroulantes (avec libellé lisible).</summary>
public record RoleItem(string Libelle, RoleType? Role);

/// <summary>
/// ViewModel du module « Utilisateurs » (administration des comptes).
/// Chaque action passe par <see cref="IUtilisateurService"/>, lui-même protégé par la
/// permission <c>GererUtilisateurs</c> : un rôle non autorisé reçoit un message d'échec.
/// </summary>
public partial class UtilisateursViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<UtilisateurListeDto> Utilisateurs { get; } = new();

    /// <summary>Rôles proposés à la création/modification (sans « Tous »).</summary>
    public RoleItem[] Roles { get; }

    /// <summary>Rôles proposés au filtrage (avec « Tous les rôles »).</summary>
    public RoleItem[] RolesFiltre { get; }

    [ObservableProperty] private bool _chargement;
    [ObservableProperty] private string? _message;
    [ObservableProperty] private bool _messageEstErreur;

    // Filtres
    [ObservableProperty] private string _recherche = string.Empty;
    [ObservableProperty] private RoleItem? _roleFiltre;

    // Sélection / formulaire
    [ObservableProperty] private UtilisateurListeDto? _selection;
    [ObservableProperty] private int _editionId;
    [ObservableProperty] private string _login = string.Empty;
    [ObservableProperty] private string _nom = string.Empty;
    [ObservableProperty] private string _prenom = string.Empty;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _telephone;
    [ObservableProperty] private RoleType _role = RoleType.Technicien;
    [ObservableProperty] private string _motDePasse = string.Empty;

    public bool EstCreation => EditionId == 0;
    public string TitreFormulaire => EstCreation ? "Nouveau compte" : "Modifier le compte";

    public UtilisateursViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        Roles = Enum.GetValues<RoleType>()
            .Select(r => new RoleItem(RoleLibelle.Pour(r), r))
            .ToArray();
        RolesFiltre = new[] { new RoleItem("Tous les rôles", null) }.Concat(Roles).ToArray();
        _roleFiltre = RolesFiltre[0];

        _ = ChargerAsync();
    }

    partial void OnEditionIdChanged(int value)
    {
        OnPropertyChanged(nameof(EstCreation));
        OnPropertyChanged(nameof(TitreFormulaire));
    }

    partial void OnSelectionChanged(UtilisateurListeDto? value)
    {
        if (value is null) return;
        EditionId = value.Id;
        Login = value.Login;
        Nom = value.Nom;
        Prenom = value.Prenom;
        Email = value.Email;
        Telephone = value.Telephone;
        Role = value.Role;
        MotDePasse = string.Empty;
        Message = null;
    }

    [RelayCommand]
    private async Task ChargerAsync()
    {
        Chargement = true;
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();
            var resultat = await service.ListerAsync(Recherche, RoleFiltre?.Role);

            Utilisateurs.Clear();
            if (resultat.EstEchec)
            {
                DefinirMessage(resultat.Erreur, erreur: true);
                return;
            }
            foreach (var u in resultat.Valeur!) Utilisateurs.Add(u);
        }
        finally
        {
            Chargement = false;
        }
    }

    [RelayCommand]
    private void Nouveau()
    {
        Selection = null;
        EditionId = 0;
        Login = Nom = Prenom = string.Empty;
        Email = Telephone = null;
        Role = RoleType.Technicien;
        MotDePasse = string.Empty;
        Message = null;
    }

    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();

        if (EstCreation)
        {
            var resultat = await service.CreerAsync(new CreerUtilisateurRequete
            {
                Login = Login, Nom = Nom, Prenom = Prenom,
                Email = Email, Telephone = Telephone, Role = Role, MotDePasse = MotDePasse
            });
            if (resultat.EstEchec) { DefinirMessage(resultat.Erreur, erreur: true); return; }
            DefinirMessage($"Compte « {resultat.Valeur!.Login} » créé.", erreur: false);
        }
        else
        {
            var resultat = await service.ModifierAsync(new ModifierUtilisateurRequete
            {
                Id = EditionId, Nom = Nom, Prenom = Prenom,
                Email = Email, Telephone = Telephone, Role = Role,
                Actif = Selection?.Actif ?? true
            });
            if (resultat.EstEchec) { DefinirMessage(resultat.Erreur, erreur: true); return; }
            DefinirMessage("Compte mis à jour.", erreur: false);
        }

        Nouveau();
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task BasculerActifAsync(UtilisateurListeDto? cible)
    {
        if (cible is null) return;
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();
        var resultat = await service.DefinirActifAsync(cible.Id, !cible.Actif);
        if (resultat.EstEchec) { DefinirMessage(resultat.Erreur, erreur: true); return; }
        DefinirMessage(cible.Actif ? "Compte désactivé." : "Compte activé.", erreur: false);
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task SupprimerAsync(UtilisateurListeDto? cible)
    {
        if (cible is null) return;

        var confirmation = System.Windows.MessageBox.Show(
            $"Voulez-vous vraiment supprimer le compte « {cible.Login} » ? Cette action est irréversible.",
            "Supprimer le compte",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);
        if (confirmation != System.Windows.MessageBoxResult.Yes) return;

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();
        var resultat = await service.SupprimerAsync(cible.Id);
        if (resultat.EstEchec) { DefinirMessage(resultat.Erreur, erreur: true); return; }
        DefinirMessage("Compte supprimé.", erreur: false);
        if (Selection?.Id == cible.Id) Nouveau();
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task ReinitialiserMotDePasseAsync()
    {
        if (EstCreation) { DefinirMessage("Sélectionnez d'abord un compte existant.", erreur: true); return; }
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUtilisateurService>();
        var resultat = await service.ReinitialiserMotDePasseAsync(EditionId, MotDePasse);
        if (resultat.EstEchec) { DefinirMessage(resultat.Erreur, erreur: true); return; }
        DefinirMessage("Mot de passe réinitialisé.", erreur: false);
        MotDePasse = string.Empty;
    }

    partial void OnRechercheChanged(string value) => _ = ChargerAsync();
    partial void OnRoleFiltreChanged(RoleItem? value) => _ = ChargerAsync();

    private void DefinirMessage(string texte, bool erreur)
    {
        Message = texte;
        MessageEstErreur = erreur;
    }
}

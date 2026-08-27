using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>
/// ViewModel du shell principal : gère la navigation latérale, l'affichage
/// du module courant et les notifications temps réel.
/// </summary>
public partial class ShellViewModel : ObservableObject
{
    private readonly IServiceProvider _services;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationTempsReel _notifications;

    [ObservableProperty] private ObservableObject? _currentViewModel;
    [ObservableProperty] private NavItem? _selectedNav;
    [ObservableProperty] private bool _panneauNotificationsOuvert;

    public ObservableCollection<NotificationMessage> Notifications { get; } = new();

    public ObservableCollection<NavItem> MenuItems { get; }

    public string UtilisateurNom => _currentUser.Utilisateur?.NomComplet ?? "Utilisateur";
    public string UtilisateurRole => _currentUser.Utilisateur?.RoleLibelle ?? string.Empty;
    public string Initiales => CalculerInitiales(UtilisateurNom);

    public int NombreNotifications => Notifications.Count;

    /// <summary>Déclenché lorsque l'utilisateur demande à se déconnecter.</summary>
    public event EventHandler? DeconnexionDemandee;

    public ShellViewModel(IServiceProvider services, ICurrentUserService currentUser, INotificationTempsReel notifications)
    {
        _services = services;
        _currentUser = currentUser;
        _notifications = notifications;

        _notifications.NotificationRecue += OnNotificationRecue;
        _ = _notifications.ConnecterAsync();

        // Les icônes sont des points de code de la police « Segoe Fluent Icons ».
        // Chaque entrée peut exiger une permission : la navigation est filtrée selon le rôle
        // (le contrôle d'accès réel reste fait action par action dans les services).
        var tousLesItems = new[]
        {
            new NavItem("Tableau de bord", 0xE80F, typeof(DashboardViewModel), Permission.ConsulterTableauBord),
            new NavItem("Statistiques", 0xE9D9, typeof(StatistiquesViewModel), Permission.ConsulterTableauBordGlobal),
            new NavItem("Parc respirateurs", 0xE772, typeof(ParcViewModel), Permission.ConsulterParc),
            new NavItem("Interventions", 0xE90F, typeof(InterventionsViewModel), Permission.ConsulterInterventions),
            new NavItem("Pièces détachées", 0xE7B8, typeof(PiecesViewModel), Permission.ConsulterPieces),
            new NavItem("Rapports", 0xE9F9, typeof(RapportsViewModel), Permission.ConsulterInterventions),
            new NavItem("Utilisateurs", 0xE716, typeof(UtilisateursViewModel), Permission.GererUtilisateurs),
            new NavItem("Paramètres", 0xE713, typeof(ParametresViewModel))
        };

        var role = _currentUser.Utilisateur?.Role;
        MenuItems = new ObservableCollection<NavItem>(
            tousLesItems.Where(item =>
                item.PermissionRequise is null
                || (role is not null && MatricePermissions.Possede(role.Value, item.PermissionRequise.Value))));

        SelectedNav = MenuItems.FirstOrDefault();
    }

    partial void OnSelectedNavChanged(NavItem? value)
    {
        if (value is null) return;
        CurrentViewModel = (ObservableObject)_services.GetRequiredService(value.CibleViewModel);
    }

    [RelayCommand]
    private void Deconnexion() => DeconnexionDemandee?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void BasculerNotifications() => PanneauNotificationsOuvert = !PanneauNotificationsOuvert;

    [RelayCommand]
    private void EffacerNotifications()
    {
        Notifications.Clear();
        OnPropertyChanged(nameof(NombreNotifications));
    }

    private void OnNotificationRecue(NotificationMessage message)
    {
        // Réception sur un thread d'arrière-plan : on repasse sur le thread UI.
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is null) return;
        dispatcher.Invoke(() =>
        {
            Notifications.Insert(0, message);
            OnPropertyChanged(nameof(NombreNotifications));
        });
    }

    private static string CalculerInitiales(string nomComplet)
    {
        var parties = nomComplet.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parties.Length == 0) return "?";
        if (parties.Length == 1) return parties[0][..1].ToUpper();
        return $"{parties[0][0]}{parties[^1][0]}".ToUpper();
    }
}

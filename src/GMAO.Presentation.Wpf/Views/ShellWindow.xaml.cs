using System.ComponentModel;
using System.Linq;
using GMAO.Presentation.Wpf.ViewModels;
using Wpf.Ui.Controls;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>Fenêtre principale (shell) hébergeant la navigation et les modules.</summary>
public partial class ShellWindow : FluentWindowBase
{
    public ShellViewModel ViewModel { get; }

    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        PeuplerNavigation();

        // Le rail compact (PaneDisplayMode=Left, fermé) réaffiche parfois le pane à l'application
        // du template : on force le repli au chargement pour garantir un rail permanent (icônes seules).
        Loaded += (_, _) => Nav.IsPaneOpen = false;

        // La navigation passe par la commande NaviguerVers (SelectedNav), pas par le SelectedItem
        // interne du NavigationView : on synchronise donc la surbrillance du rail sur SelectedNav.
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShellViewModel.SelectedNav))
            SynchroniserSurbrillanceRail();
    }

    /// <summary>Aligne l'item actif du rail (surbrillance) sur ShellViewModel.SelectedNav,
    /// quelle que soit l'origine de la navigation (clic rail ou programmatique).</summary>
    private void SynchroniserSurbrillanceRail()
    {
        foreach (var navItem in Nav.MenuItems.OfType<NavigationViewItem>()
                     .Concat(Nav.FooterMenuItems.OfType<NavigationViewItem>()))
        {
            navItem.IsActive = ReferenceEquals(navItem.Tag, ViewModel.SelectedNav);
        }
    }

    /// <summary>
    /// Peuple le NavigationView à partir de ShellViewModel.MenuItems / FooterItems
    /// (la logique RBAC est déjà appliquée côté ViewModel). Chaque item route via la
    /// commande NaviguerVers ; le mécanisme VM→View (CurrentViewModel + DataTemplate) est conservé.
    /// </summary>
    private void PeuplerNavigation()
    {
        foreach (var item in ViewModel.MenuItems)
            Nav.MenuItems.Add(CreerNavItem(item));
        foreach (var item in ViewModel.FooterItems)
            Nav.FooterMenuItems.Add(CreerNavItem(item));

        // Surbrillance initiale alignée sur l'item courant du ShellViewModel
        // (SelectedItem est en lecture seule sur NavigationView → on active l'item).
        var courant = Nav.MenuItems.OfType<NavigationViewItem>()
            .FirstOrDefault(n => ReferenceEquals(n.Tag, ViewModel.SelectedNav));
        if (courant is not null)
            courant.IsActive = true;
    }

    private NavigationViewItem CreerNavItem(NavItem item)
    {
        var navItem = new NavigationViewItem
        {
            Content = item.Titre,
            Tag = item,
            Icon = new FontIcon
            {
                FontFamily = new System.Windows.Media.FontFamily("Segoe Fluent Icons, Segoe MDL2 Assets"),
                Glyph = item.Icone,
            },
            Command = ViewModel.NaviguerVersCommand,
            CommandParameter = item,
        };
        // Rail compact (icônes seules) : le libellé n'est visible qu'en survol via le ToolTip.
        navItem.ToolTip = item.Titre;
        System.Windows.Controls.ToolTipService.SetInitialShowDelay(navItem, 200);
        return navItem;
    }
}

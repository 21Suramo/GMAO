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

    private NavigationViewItem CreerNavItem(NavItem item) => new()
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
}

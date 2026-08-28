using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GMAO.Presentation.Wpf.ViewModels;
using FontIcon = global::Wpf.Ui.Controls.FontIcon;
using NavigationViewItem = global::Wpf.Ui.Controls.NavigationViewItem;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>Fenêtre principale (shell) hébergeant la navigation et les modules.</summary>
public partial class ShellWindow : FluentWindowBase
{
    private static readonly FontFamily PoliceIcones = new("Segoe Fluent Icons, Segoe MDL2 Assets");

    public ShellViewModel ViewModel { get; }

    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;

        ConstruireNavigation();

        // Le ContentFrame n'existe qu'une fois le template du NavigationView appliqué.
        Loaded += (_, _) => AttacherContenu();
    }

    /// <summary>
    /// Construit les entrées du volet à partir des <see cref="NavItem"/> déjà filtrés par le
    /// RBAC dans <see cref="ShellViewModel"/> : la règle d'accès n'est pas redéfinie ici.
    /// </summary>
    private void ConstruireNavigation()
    {
        foreach (var item in ViewModel.MenuItems)
        {
            Navigation.MenuItems.Add(CreerEntree(item));
        }

        foreach (var item in ViewModel.FooterItems)
        {
            Navigation.FooterMenuItems.Add(CreerEntree(item));
        }
    }

    /// <summary>
    /// Crée une entrée de volet pour un <see cref="NavItem"/>.
    ///
    /// <c>TargetPageType</c> reste volontairement nul : le NavigationView de WPF-UI ne
    /// déclenche alors aucune navigation interne (il se contenterait d'instancier un type de
    /// page, ce qui doublonnerait la résolution VM -> Vue). L'entrée se comporte en simple
    /// bouton — <c>NavigationViewItem</c> dérive de <c>ButtonBase</c> — et met à jour
    /// <see cref="ShellViewModel.SelectedNav"/>, seul pilote du contenu affiché.
    /// </summary>
    private NavigationViewItem CreerEntree(NavItem item)
    {
        var entree = new NavigationViewItem
        {
            Content = item.Titre,
            Icon = new FontIcon { FontFamily = PoliceIcones, Glyph = item.Icone },
            IsActive = ReferenceEquals(item, ViewModel.SelectedNav)
        };

        entree.Click += (_, _) =>
        {
            ViewModel.SelectedNav = item;
            ActualiserSelection(entree);
        };

        return entree;
    }

    /// <summary>
    /// Met à jour le surlignage du volet. Sans navigation interne, WPF-UI n'active pas
    /// l'entrée lui-même ; pour une entrée simple, son <c>Activate</c> se réduit d'ailleurs
    /// à positionner <c>IsActive</c>.
    /// </summary>
    private void ActualiserSelection(NavigationViewItem selectionnee)
    {
        foreach (var entree in Navigation.MenuItems.OfType<NavigationViewItem>()
                     .Concat(Navigation.FooterMenuItems.OfType<NavigationViewItem>()))
        {
            entree.IsActive = ReferenceEquals(entree, selectionnee);
        }
    }

    /// <summary>
    /// Place dans le ContentFrame du NavigationView le ContentControl lié à
    /// <see cref="ShellViewModel.CurrentViewModel"/> : le mécanisme DataTemplate VM -> Vue
    /// déclaré dans App.xaml reste la seule source de vérité, il est simplement rebranché
    /// sur le nouveau conteneur.
    ///
    /// Le DataContext est posé explicitement : un Frame ne propage pas celui de son parent.
    /// </summary>
    private void AttacherContenu()
    {
        var hote = new ContentControl { DataContext = ViewModel };
        hote.SetBinding(
            ContentControl.ContentProperty,
            new Binding(nameof(ShellViewModel.CurrentViewModel)) { Mode = BindingMode.OneWay });

        Navigation.ApplyTemplate();
        Navigation.ReplaceContent(hote);
    }
}

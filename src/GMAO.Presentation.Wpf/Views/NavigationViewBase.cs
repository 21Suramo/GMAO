using System.Windows;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>
/// Sous-classe locale de <c>Wpf.Ui.Controls.NavigationView</c>.
///
/// Même raison que <see cref="FluentWindowBase"/> : le namespace racine finit par « .Wpf »,
/// ce qui masque « Wpf.Ui » dans le code XAML généré. Un <c>ui:NavigationView</c> nommé
/// ferait générer un champ <c>Wpf.Ui.Controls.NavigationView</c> → collision CS0234.
/// On dérive ici via <c>global::</c> pour pouvoir nommer le contrôle sans collision.
/// </summary>
public class NavigationViewBase : global::Wpf.Ui.Controls.NavigationView
{
    static NavigationViewBase()
    {
        // Sans ceci, la sous-classe aurait DefaultStyleKey = typeof(NavigationViewBase),
        // pour lequel aucun style/template n'existe → le pane ne se rend pas du tout.
        // On réutilise le template par défaut de la NavigationView de WPF-UI.
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(NavigationViewBase),
            new FrameworkPropertyMetadata(typeof(global::Wpf.Ui.Controls.NavigationView)));
    }
}

namespace GMAO.Presentation.Wpf.Views;

/// <summary>
/// Sous-classe locale de <c>Wpf.Ui.Controls.NavigationView</c>, pour la même raison que
/// <see cref="FluentWindowBase"/> : le namespace racine du projet se termine par « .Wpf »,
/// ce qui masque le namespace « Wpf.Ui » dans le code XAML GÉNÉRÉ. Un <c>x:Name</c> sur un
/// contrôle WPF-UI y produit un champ dont le type est écrit en toutes lettres
/// (<c>Wpf.Ui.Controls.NavigationView</c>) : résolu depuis « GMAO.Presentation.Wpf.Views »,
/// l'identifiant « Wpf » pointe vers « GMAO.Presentation.Wpf » → « .Ui » introuvable (CS0234).
///
/// En dérivant ici via <c>global::</c>, le contrôle reste un NavigationView à part entière —
/// <c>DefaultStyleKey</c> vaut toujours <c>typeof(NavigationView)</c>, donc le style Fluent
/// s'applique — sans déclencher la collision.
/// </summary>
public class NavigationViewBase : global::Wpf.Ui.Controls.NavigationView
{
}

namespace GMAO.Presentation.Wpf.Views;

/// <summary>
/// Sous-classe locale de <c>Wpf.Ui.Controls.FluentWindow</c>.
///
/// Nécessaire car le namespace racine du projet se termine par « .Wpf », ce qui
/// masque le namespace « Wpf.Ui » de la librairie WPF-UI dans le code XAML GÉNÉRÉ :
/// le compilateur y écrit la classe de base en toutes lettres (`Wpf.Ui.Controls.FluentWindow`)
/// et, résolu depuis « GMAO.Presentation.Wpf.Views », l'identifiant « Wpf » pointe vers
/// « GMAO.Presentation.Wpf » → « .Ui » introuvable (CS0234).
///
/// En dérivant ici via <c>global::</c> (dans du code que l'on contrôle), la fenêtre reste
/// une FluentWindow à part entière (Mica, titlebar étendue, etc.) sans déclencher la collision.
/// </summary>
public class FluentWindowBase : global::Wpf.Ui.Controls.FluentWindow
{
}

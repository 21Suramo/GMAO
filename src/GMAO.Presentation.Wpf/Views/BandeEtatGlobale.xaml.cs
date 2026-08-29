using System.Windows.Controls;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>
/// Bande d'état globale du shell (statut de connexion, compteur d'interventions
/// critiques, cloche de notifications). Le DataContext est hérité du ShellWindow
/// (= ShellViewModel) — aucun code-behind métier.
/// </summary>
public partial class BandeEtatGlobale : UserControl
{
    public BandeEtatGlobale()
    {
        InitializeComponent();
    }
}

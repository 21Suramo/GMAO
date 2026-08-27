using System.Windows;
using System.Windows.Input;
using GMAO.Presentation.Wpf.ViewModels;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>Fenêtre de connexion. Relie le PasswordBox au ViewModel (non bindable directement).</summary>
public partial class LoginWindow : Window
{
    /// <summary>ViewModel typé de la fenêtre.</summary>
    public LoginViewModel ViewModel { get; }

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;

        // Synchronisation manuelle du mot de passe (le PasswordBox ne supporte pas le binding).
        ChampMotDePasse.PasswordChanged += (_, _) => ViewModel.MotDePasse = ChampMotDePasse.Password;

        // Validation au clavier (Entrée).
        ChampMotDePasse.KeyDown += SoumettreSurEntree;
        ChampLogin.KeyDown += SoumettreSurEntree;

        Loaded += (_, _) => ChampLogin.Focus();
    }

    private void SoumettreSurEntree(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        if (ViewModel.ConnexionCommand.CanExecute(null))
            ViewModel.ConnexionCommand.Execute(null);
    }
}

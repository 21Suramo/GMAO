using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel de l'écran de connexion.</summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserService _currentUser;

    [ObservableProperty] private string _login = "admin";
    [ObservableProperty] private string _motDePasse = string.Empty;
    [ObservableProperty] private string? _messageErreur;
    [ObservableProperty] private bool _enCours;

    /// <summary>Déclenché après une authentification réussie.</summary>
    public event EventHandler? ConnexionReussie;

    public LoginViewModel(IServiceScopeFactory scopeFactory, ICurrentUserService currentUser)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
    }

    [RelayCommand]
    private async Task ConnexionAsync()
    {
        MessageErreur = null;
        EnCours = true;
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var auth = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            var resultat = await auth.ConnecterAsync(new ConnexionRequete
            {
                Login = Login,
                MotDePasse = MotDePasse,
                AdresseIp = Environment.MachineName
            });

            if (resultat.EstSucces)
            {
                _currentUser.Definir(resultat.Valeur!);
                ConnexionReussie?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageErreur = resultat.Erreur;
            }
        }
        catch (Exception ex)
        {
            MessageErreur = $"Erreur inattendue : {ex.Message}";
        }
        finally
        {
            EnCours = false;
        }
    }
}

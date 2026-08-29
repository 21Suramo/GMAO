using System.IO;
using System.Windows;
using GMAO.Application;
using GMAO.Application.Common.Interfaces;
using GMAO.Infrastructure;
using GMAO.Persistence;
using GMAO.Persistence.Context;
using GMAO.Persistence.Seed;
using GMAO.Presentation.Wpf.ViewModels;
using GMAO.Presentation.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wpf.Ui.Appearance;

namespace GMAO.Presentation.Wpf;

/// <summary>
/// Point d'entrée WPF : compose le conteneur d'injection (Generic Host + Serilog),
/// initialise la base, puis orchestre l'enchaînement connexion → shell.
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    /// <summary>Mot de passe administrateur par défaut (à changer en production).</summary>
    private const string MotDePasseAdminParDefaut = "Admin@123";

    public App()
    {
        // Filet de sécurité : aucune exception non gérée ne doit fermer brutalement l'application.
        DispatcherUnhandledException += (_, e) =>
        {
            Log.Error(e.Exception, "Exception non gérée sur le thread UI");
            MessageBox.Show($"Une erreur inattendue s'est produite :\n\n{e.Exception.Message}",
                "MEDICANA — Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        };

        var cheminBase = Path.Combine(AppContext.BaseDirectory, "gmao.db");
        var connectionString = $"Data Source={cheminBase}";

        _host = Host.CreateDefaultBuilder()
            .UseSerilog((_, cfg) => cfg
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(AppContext.BaseDirectory, "logs", "gmao-.log"),
                    rollingInterval: RollingInterval.Day))
            .ConfigureServices((_, services) =>
            {
                services.AddPersistence(connectionString);
                services.AddInfrastructure();
                services.AddApplication();
                EnregistrerPresentation(services);
            })
            .Build();
    }

    /// <summary>Enregistre les ViewModels et les fenêtres de la couche présentation.</summary>
    private static void EnregistrerPresentation(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ShellViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<StatistiquesViewModel>();
        services.AddTransient<ParcViewModel>();
        services.AddTransient<InterventionsViewModel>();
        services.AddTransient<PiecesViewModel>();
        services.AddTransient<RapportsViewModel>();
        services.AddTransient<UtilisateursViewModel>();
        services.AddTransient<ParametresViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<ShellWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Thème Fluent sombre + backdrop Mica par défaut (WPF-UI)
        ApplicationThemeManager.Apply(ApplicationTheme.Dark, global::Wpf.Ui.Controls.WindowBackdropType.Mica);

        await _host.StartAsync();

        try
        {
            using var scope = _host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await DbSeeder.SeedAsync(context, hasher.Hacher(MotDePasseAdminParDefaut));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Échec de l'initialisation de la base de données");
            MessageBox.Show($"Erreur d'initialisation de la base :\n{ex.Message}",
                "MEDICANA", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        AfficherConnexion();
    }

    /// <summary>Affiche la fenêtre de connexion et câble la transition vers le shell.</summary>
    private void AfficherConnexion()
    {
        var login = _host.Services.GetRequiredService<LoginWindow>();

        login.ViewModel.ConnexionReussie += (_, _) =>
        {
            var shell = _host.Services.GetRequiredService<ShellWindow>();

            shell.ViewModel.DeconnexionDemandee += (_, _) =>
            {
                _host.Services.GetRequiredService<ICurrentUserService>().Effacer();
                AfficherConnexion();   // afficher la connexion avant de fermer le shell
                shell.Close();
            };

            shell.Show();
            login.Close();
        };

        login.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(2));
        }
        await Log.CloseAndFlushAsync();
        base.OnExit(e);
    }
}

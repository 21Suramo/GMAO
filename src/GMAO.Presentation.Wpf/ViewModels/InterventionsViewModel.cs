using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>ViewModel du module « Interventions » : liste, déclaration, workflow et check-list.</summary>
public partial class InterventionsViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationTempsReel _notifications;

    public ObservableCollection<InterventionDto> Interventions { get; } = new();
    public ObservableCollection<RespirateurDto> Respirateurs { get; } = new();
    public ObservableCollection<SymptomeDto> Symptomes { get; } = new();
    public ObservableCollection<HistoriqueEtatDto> Historique { get; } = new();

    public Priorite[] Priorites { get; } = { Priorite.Basse, Priorite.Normale, Priorite.Haute, Priorite.Critique };

    [ObservableProperty] private InterventionDto? _selection;
    [ObservableProperty] private InterventionDetailDto? _detail;
    [ObservableProperty] private bool _chargement;
    [ObservableProperty] private string? _message;

    // Formulaire de déclaration
    [ObservableProperty] private bool _creationVisible;
    [ObservableProperty] private RespirateurDto? _nouveauRespirateur;
    [ObservableProperty] private string _nouvelleDescription = string.Empty;
    [ObservableProperty] private Priorite _nouvellePriorite = Priorite.Normale;
    [ObservableProperty] private bool _nouveauPatientConnecte;

    // Check-list (8 contrôles obligatoires)
    [ObservableProperty] private bool _ckAutotest;
    [ObservableProperty] private bool _ckEtancheite;
    [ObservableProperty] private bool _ckCalibrationDebit;
    [ObservableProperty] private bool _ckCalibrationO2;
    [ObservableProperty] private bool _ckBatterie;
    [ObservableProperty] private bool _ckAlimentation;
    [ObservableProperty] private bool _ckAlarmes;
    [ObservableProperty] private bool _ckValidationFinale;

    public InterventionsViewModel(IServiceScopeFactory scopeFactory, ICurrentUserService currentUser, INotificationTempsReel notifications)
    {
        _scopeFactory = scopeFactory;
        _currentUser = currentUser;
        _notifications = notifications;
        _ = ChargerAsync();
    }

    private string Auteur => _currentUser.Utilisateur?.NomComplet ?? "Inconnu";

    [RelayCommand]
    private async Task ChargerAsync()
    {
        Chargement = true;
        Message = null;
        try
        {
            var idCourant = Selection?.Id;
            using var scope = _scopeFactory.CreateScope();
            var interventions = scope.ServiceProvider.GetRequiredService<IInterventionService>();
            var respirateurs = scope.ServiceProvider.GetRequiredService<IRespirateurService>();

            var liste = await interventions.ListerAsync();
            Interventions.Clear();
            foreach (var i in liste) Interventions.Add(i);

            if (Respirateurs.Count == 0)
            {
                foreach (var r in await respirateurs.ListerAsync()) Respirateurs.Add(r);
                foreach (var s in await interventions.ListerSymptomesAsync()) Symptomes.Add(s);
            }

            Selection = idCourant is null
                ? Interventions.FirstOrDefault()
                : Interventions.FirstOrDefault(i => i.Id == idCourant) ?? Interventions.FirstOrDefault();
        }
        finally
        {
            Chargement = false;
        }
    }

    partial void OnSelectionChanged(InterventionDto? value) => _ = ChargerDetailAsync(value);

    private async Task ChargerDetailAsync(InterventionDto? value)
    {
        Historique.Clear();
        if (value is null) { Detail = null; return; }

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IInterventionService>();
        Detail = await service.ObtenirAsync(value.Id);

        if (Detail?.CheckList is { } c)
        {
            CkAutotest = c.AutotestOk; CkEtancheite = c.TestEtancheite;
            CkCalibrationDebit = c.CalibrationDebit; CkCalibrationO2 = c.CalibrationO2;
            CkBatterie = c.Batterie; CkAlimentation = c.Alimentation;
            CkAlarmes = c.Alarmes; CkValidationFinale = c.ValidationFinale;
        }
        foreach (var h in Detail?.Historique ?? Enumerable.Empty<HistoriqueEtatDto>())
            Historique.Add(h);
    }

    [RelayCommand]
    private void BasculerCreation()
    {
        CreationVisible = !CreationVisible;
        Message = null;
    }

    [RelayCommand]
    private async Task CreerAsync()
    {
        if (NouveauRespirateur is null)
        {
            Message = "Sélectionnez un respirateur.";
            return;
        }

        var requete = new CreerInterventionRequete
        {
            RespirateurId = NouveauRespirateur.Id,
            Description = NouvelleDescription,
            Priorite = NouvellePriorite,
            PatientConnecte = NouveauPatientConnecte,
            SymptomeIds = Symptomes.Where(s => s.Selectionne).Select(s => s.Id).ToList()
        };

        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IInterventionService>();
            var resultat = await service.CreerAsync(requete, Auteur);
            if (resultat.EstEchec) { Message = resultat.Erreur; return; }
            Message = resultat.Valeur!.EstCritique
                ? $"DI CRITIQUE {resultat.Valeur.NumeroDI} créée et affectée automatiquement."
                : $"DI {resultat.Valeur.NumeroDI} créée.";

            // Notification temps réel pour les cas critiques (patient connecté).
            if (resultat.Valeur.EstCritique)
            {
                await _notifications.EnvoyerAsync(new NotificationMessage
                {
                    Type = "InterventionUrgente",
                    Titre = $"DI critique {resultat.Valeur.NumeroDI}",
                    Message = $"Patient connecté — {resultat.Valeur.RespirateurSerie}. Intervention immédiate requise.",
                    Reference = resultat.Valeur.NumeroDI
                });
            }
        }

        // Réinitialise le formulaire.
        NouvelleDescription = string.Empty;
        NouveauPatientConnecte = false;
        NouvellePriorite = Priorite.Normale;
        foreach (var s in Symptomes) s.Selectionne = false;
        CreationVisible = false;

        await ChargerAsync();
    }

    [RelayCommand]
    private async Task AvancerAsync()
    {
        if (Detail is null) return;
        var suivant = EtatSuivant(Detail.Etat);
        if (suivant is null) { Message = "Aucune étape suivante."; return; }
        await AppliquerEtatAsync(suivant.Value);
    }

    [RelayCommand]
    private async Task MettreEnAttentePieceAsync() => await AppliquerEtatAsync(EtatIntervention.EnAttentePiece);

    [RelayCommand]
    private async Task CloturerAsync() => await AppliquerEtatAsync(EtatIntervention.Cloturee);

    private async Task AppliquerEtatAsync(EtatIntervention etat)
    {
        if (Detail is null) return;
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IInterventionService>();
            var resultat = await service.ChangerEtatAsync(Detail.Id, etat, Auteur);
            if (resultat.EstEchec) { Message = resultat.Erreur; return; }
            Message = $"Intervention {Detail.NumeroDI} : {InterventionDto.LibelleEtat(etat)}.";
        }
        await ChargerAsync();
    }

    [RelayCommand]
    private async Task GenererRapportAsync()
    {
        if (Detail is null) return;
        string? chemin = null;
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IRapportService>();
            var resultat = await service.GenererRapportInterventionAsync(Detail.Id);
            if (resultat.EstEchec) { Message = resultat.Erreur; return; }
            chemin = resultat.Valeur;
        }

        Message = $"Rapport PDF généré : {chemin}";
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(chemin!) { UseShellExecute = true });
        }
        catch
        {
            // L'ouverture automatique peut échouer ; le fichier reste disponible sur disque.
        }
    }

    [RelayCommand]
    private async Task EnregistrerCheckListAsync()
    {
        if (Detail is null) return;
        var dto = new CheckListDto
        {
            AutotestOk = CkAutotest, TestEtancheite = CkEtancheite,
            CalibrationDebit = CkCalibrationDebit, CalibrationO2 = CkCalibrationO2,
            Batterie = CkBatterie, Alimentation = CkAlimentation,
            Alarmes = CkAlarmes, ValidationFinale = CkValidationFinale
        };
        using (var scope = _scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IInterventionService>();
            await service.MettreAJourCheckListAsync(Detail.Id, dto);
        }
        Message = dto.EstComplete
            ? "Check-list complète : l'intervention peut être clôturée."
            : "Check-list enregistrée (incomplète).";
        await ChargerDetailAsync(Selection);
    }

    private static EtatIntervention? EtatSuivant(EtatIntervention etat) => etat switch
    {
        EtatIntervention.Nouvelle => EtatIntervention.Affectee,
        EtatIntervention.Affectee => EtatIntervention.EnDeplacement,
        EtatIntervention.EnDeplacement => EtatIntervention.Diagnostic,
        EtatIntervention.Diagnostic => EtatIntervention.Reparation,
        EtatIntervention.Reparation => EtatIntervention.Test,
        EtatIntervention.EnAttentePiece => EtatIntervention.Reparation,
        EtatIntervention.Test => EtatIntervention.Validation,
        EtatIntervention.Validation => EtatIntervention.Cloturee,
        _ => null
    };
}

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GMAO.Presentation.Wpf.ViewModels;
using Wpf.Ui.Controls;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>
/// Vue d'administration des comptes. Le formulaire création/modification est présenté dans un
/// <see cref="ContentDialog"/> (WPF-UI) construit et attaché en code à l'hôte <c>HoteDialog</c>
/// (un ContentDialog déclaré dans l'arbre visuel ne peut pas être ré-attaché à un hôte). Code-behind
/// purement « vue » : orchestration d'ouverture/fermeture, aucune logique métier.
/// </summary>
public partial class UtilisateursView : UserControl
{
    private bool _dialogueOuvert;
    private bool _sauvegardeReussie;

    public UtilisateursView()
    {
        InitializeComponent();

        DataContextChanged += (_, e) =>
        {
            if (e.OldValue is UtilisateursViewModel ancien) ancien.PropertyChanged -= SurChangementVm;
            if (e.NewValue is UtilisateursViewModel vm) vm.PropertyChanged += SurChangementVm;
        };
    }

    private UtilisateursViewModel? Vm => DataContext as UtilisateursViewModel;

    /// <summary>Bouton « Nouvel utilisateur » : réinitialise le formulaire puis ouvre le dialogue en création.</summary>
    private void OuvrirCreation(object sender, RoutedEventArgs e)
    {
        Vm?.NouveauCommand.Execute(null);
        _ = AfficherDialogueAsync();
    }

    private async Task AfficherDialogueAsync()
    {
        if (_dialogueOuvert || Vm is null) return;
        _dialogueOuvert = true;
        _sauvegardeReussie = false;
        try
        {
            var dialogue = new ContentDialog(HoteDialog)
            {
                Title = Vm.TitreFormulaire,
                Content = new UtilisateurFormulaireDialog { DataContext = Vm },
                PrimaryButtonText = "Enregistrer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Primary,
                DataContext = Vm,
            };
            dialogue.Closing += SurFermetureDialogue;
            await dialogue.ShowAsync();
        }
        finally
        {
            _dialogueOuvert = false;
        }
    }

    // Bouton « Enregistrer » : on délègue la validation au ViewModel et on maintient le dialogue
    // ouvert tant que la sauvegarde échoue (le message d'erreur s'affiche dans le dialogue).
    private async void SurFermetureDialogue(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (args.Result != ContentDialogResult.Primary || _sauvegardeReussie || Vm is null) return;

        args.Cancel = true;
        await Vm.EnregistrerCommand.ExecuteAsync(null);
        if (!Vm.MessageEstErreur)
        {
            _sauvegardeReussie = true;
            sender.Hide(ContentDialogResult.Primary);
        }
    }

    private void SurChangementVm(object? sender, PropertyChangedEventArgs e)
    {
        if (Vm is null) return;

        // Sélection d'un compte dans la liste ⇒ ouverture du dialogue en modification.
        if (e.PropertyName == nameof(UtilisateursViewModel.Selection)
            && Vm.Selection is not null && !_dialogueOuvert)
            _ = AfficherDialogueAsync();
    }
}

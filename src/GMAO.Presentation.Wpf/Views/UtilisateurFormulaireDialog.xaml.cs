using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GMAO.Presentation.Wpf.ViewModels;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>
/// Contenu du formulaire de création/modification d'un compte, hébergé dans un ContentDialog.
/// Code-behind purement « vue » : relais de la PasswordBox (non bindable) vers le ViewModel.
/// </summary>
public partial class UtilisateurFormulaireDialog : UserControl
{
    public UtilisateurFormulaireDialog()
    {
        InitializeComponent();

        PbMotDePasse.PasswordChanged += (_, _) => { if (Vm is not null) Vm.MotDePasse = PbMotDePasse.Password; };

        DataContextChanged += (_, e) =>
        {
            if (e.OldValue is UtilisateursViewModel ancien) ancien.PropertyChanged -= SurChangementVm;
            if (e.NewValue is UtilisateursViewModel vm) vm.PropertyChanged += SurChangementVm;
        };
    }

    private UtilisateursViewModel? Vm => DataContext as UtilisateursViewModel;

    private void SurChangementVm(object? sender, PropertyChangedEventArgs e)
    {
        if (Vm is null) return;
        if (e.PropertyName == nameof(UtilisateursViewModel.MotDePasse)
            && Vm.MotDePasse.Length == 0 && PbMotDePasse.Password.Length > 0)
            PbMotDePasse.Clear();
    }
}

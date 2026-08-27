using System.ComponentModel;
using System.Windows.Controls;
using GMAO.Presentation.Wpf.ViewModels;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>Vue d'administration des comptes. Relie la PasswordBox (non bindable) au ViewModel.</summary>
public partial class UtilisateursView : UserControl
{
    public UtilisateursView()
    {
        InitializeComponent();

        PbMotDePasse.PasswordChanged += (_, _) => { if (Vm is not null) Vm.MotDePasse = PbMotDePasse.Password; };

        DataContextChanged += (_, e) =>
        {
            if (e.NewValue is UtilisateursViewModel vm)
                vm.PropertyChanged += SurChangementVm;
        };
    }

    private UtilisateursViewModel? Vm => DataContext as UtilisateursViewModel;

    // Vide le champ mot de passe lorsque le ViewModel le réinitialise (après enregistrement).
    private void SurChangementVm(object? sender, PropertyChangedEventArgs e)
    {
        if (Vm is null) return;
        if (e.PropertyName == nameof(UtilisateursViewModel.MotDePasse)
            && Vm.MotDePasse.Length == 0 && PbMotDePasse.Password.Length > 0)
            PbMotDePasse.Clear();
    }
}

using System.ComponentModel;
using System.Windows.Controls;
using GMAO.Presentation.Wpf.ViewModels;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>Vue du module « Paramètres ». Relie les PasswordBox (non bindables) au ViewModel.</summary>
public partial class ParametresView : UserControl
{
    public ParametresView()
    {
        InitializeComponent();

        PbAncien.PasswordChanged += (_, _) => { if (Vm is not null) Vm.AncienMotDePasse = PbAncien.Password; };
        PbNouveau.PasswordChanged += (_, _) => { if (Vm is not null) Vm.NouveauMotDePasse = PbNouveau.Password; };
        PbConfirme.PasswordChanged += (_, _) => { if (Vm is not null) Vm.ConfirmationMotDePasse = PbConfirme.Password; };

        DataContextChanged += (_, e) =>
        {
            if (e.NewValue is ParametresViewModel vm)
                vm.PropertyChanged += SurChangementVm;
        };
    }

    private ParametresViewModel? Vm => DataContext as ParametresViewModel;

    // Vide les champs lorsque le ViewModel réinitialise les mots de passe (après succès).
    private void SurChangementVm(object? sender, PropertyChangedEventArgs e)
    {
        if (Vm is null) return;
        if (e.PropertyName == nameof(ParametresViewModel.AncienMotDePasse) && Vm.AncienMotDePasse.Length == 0 && PbAncien.Password.Length > 0)
            PbAncien.Clear();
        if (e.PropertyName == nameof(ParametresViewModel.NouveauMotDePasse) && Vm.NouveauMotDePasse.Length == 0 && PbNouveau.Password.Length > 0)
            PbNouveau.Clear();
        if (e.PropertyName == nameof(ParametresViewModel.ConfirmationMotDePasse) && Vm.ConfirmationMotDePasse.Length == 0 && PbConfirme.Password.Length > 0)
            PbConfirme.Clear();
    }
}

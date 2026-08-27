using GMAO.Presentation.Wpf.ViewModels;

namespace GMAO.Presentation.Wpf.Views;

/// <summary>Fenêtre principale (shell) hébergeant la navigation et les modules.</summary>
public partial class ShellWindow : FluentWindowBase
{
    public ShellViewModel ViewModel { get; }

    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }
}

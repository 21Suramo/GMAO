using GMAO.Domain.Enums;

namespace GMAO.Presentation.Wpf.ViewModels;

/// <summary>Élément du menu de navigation latéral.</summary>
public class NavItem
{
    public string Titre { get; }

    /// <summary>Glyphe d'icône (police Segoe Fluent Icons / MDL2), issu d'un point de code.</summary>
    public string Icone { get; }

    /// <summary>Type du ViewModel à afficher lorsque l'élément est sélectionné.</summary>
    public Type CibleViewModel { get; }

    /// <summary>Permission requise pour voir cet élément (null = accessible à tous les rôles).</summary>
    public Permission? PermissionRequise { get; }

    /// <param name="iconeCodePoint">Point de code Unicode du glyphe (ex. 0xE80F).</param>
    public NavItem(string titre, int iconeCodePoint, Type cibleViewModel, Permission? permissionRequise = null)
    {
        Titre = titre;
        Icone = char.ConvertFromUtf32(iconeCodePoint);
        CibleViewModel = cibleViewModel;
        PermissionRequise = permissionRequise;
    }
}

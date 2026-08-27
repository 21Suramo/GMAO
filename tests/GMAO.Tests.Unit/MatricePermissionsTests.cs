using FluentAssertions;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Enums;
using Xunit;

namespace GMAO.Tests.Unit;

/// <summary>Tests de la matrice Rôle → Permissions (RBAC).</summary>
public class MatricePermissionsTests
{
    [Fact]
    public void Administrateur_PossedeToutesLesPermissions()
    {
        foreach (var permission in Enum.GetValues<Permission>())
            MatricePermissions.Possede(RoleType.Administrateur, permission)
                .Should().BeTrue($"l'administrateur doit avoir {permission}");
    }

    [Fact]
    public void GestionUtilisateurs_ReserveeAdministrateur()
    {
        MatricePermissions.Possede(RoleType.Administrateur, Permission.GererUtilisateurs).Should().BeTrue();
        MatricePermissions.Possede(RoleType.ResponsableSAV, Permission.GererUtilisateurs).Should().BeFalse();
        MatricePermissions.Possede(RoleType.Ingenieur, Permission.GererUtilisateurs).Should().BeFalse();
        MatricePermissions.Possede(RoleType.Technicien, Permission.GererUtilisateurs).Should().BeFalse();
    }

    [Fact]
    public void Technicien_NePeutPasCloturer()
    {
        MatricePermissions.Possede(RoleType.Technicien, Permission.ChangerEtatIntervention).Should().BeTrue();
        MatricePermissions.Possede(RoleType.Technicien, Permission.ClorerIntervention).Should().BeFalse();
    }

    [Fact]
    public void Invite_EnLectureSeule()
    {
        MatricePermissions.Possede(RoleType.Invite, Permission.ConsulterTableauBord).Should().BeTrue();
        MatricePermissions.Possede(RoleType.Invite, Permission.CreerIntervention).Should().BeFalse();
        MatricePermissions.Possede(RoleType.Invite, Permission.GererStock).Should().BeFalse();
    }

    [Fact]
    public void VueGlobale_ReserveeResponsableEtAdministrateur()
    {
        MatricePermissions.Possede(RoleType.Administrateur, Permission.ConsulterTableauBordGlobal).Should().BeTrue();
        MatricePermissions.Possede(RoleType.ResponsableSAV, Permission.ConsulterTableauBordGlobal).Should().BeTrue();
        MatricePermissions.Possede(RoleType.Ingenieur, Permission.ConsulterTableauBordGlobal).Should().BeFalse();
        MatricePermissions.Possede(RoleType.Technicien, Permission.ConsulterTableauBordGlobal).Should().BeFalse();
    }
}

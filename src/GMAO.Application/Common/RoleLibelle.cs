using GMAO.Domain.Enums;

namespace GMAO.Application.Common;

/// <summary>Fournit les libellés lisibles des rôles utilisateurs.</summary>
public static class RoleLibelle
{
    public static string Pour(RoleType role) => role switch
    {
        RoleType.Administrateur => "Administrateur",
        RoleType.ResponsableSAV => "Responsable SAV",
        RoleType.Ingenieur => "Ingénieur Biomédical",
        RoleType.Technicien => "Technicien",
        RoleType.Client => "Client (Hôpital)",
        RoleType.Invite => "Invité",
        _ => role.ToString()
    };
}

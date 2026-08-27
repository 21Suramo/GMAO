using GMAO.Domain.Enums;

namespace GMAO.Domain.Entities.Securite;

/// <summary>
/// Table de correspondance Rôle → Permissions (RBAC).
/// Règle de domaine pure et sans effet de bord : elle constitue la source de vérité
/// des droits, exploitée par le service d'autorisation de la couche Application.
/// </summary>
public static class MatricePermissions
{
    private static readonly IReadOnlyDictionary<RoleType, HashSet<Permission>> Matrice =
        new Dictionary<RoleType, HashSet<Permission>>
        {
            // L'administrateur dispose de toutes les permissions (y compris la gestion des comptes).
            [RoleType.Administrateur] = new HashSet<Permission>(Enum.GetValues<Permission>()),

            // Le responsable SAV pilote toute l'activité métier, sauf la gestion des utilisateurs.
            [RoleType.ResponsableSAV] = new HashSet<Permission>
            {
                Permission.ConsulterTableauBord, Permission.ConsulterTableauBordGlobal,
                Permission.ConsulterInterventions, Permission.CreerIntervention,
                Permission.AffecterIntervention, Permission.ChangerEtatIntervention,
                Permission.ClorerIntervention, Permission.GenererRapport,
                Permission.ConsulterParc, Permission.GererParc,
                Permission.ConsulterPieces, Permission.GererStock, Permission.SupprimerPiece
            },

            // L'ingénieur biomédical exécute les interventions et consomme des pièces.
            [RoleType.Ingenieur] = new HashSet<Permission>
            {
                Permission.ConsulterTableauBord,
                Permission.ConsulterInterventions, Permission.CreerIntervention,
                Permission.ChangerEtatIntervention, Permission.ClorerIntervention,
                Permission.GenererRapport,
                Permission.ConsulterParc,
                Permission.ConsulterPieces, Permission.GererStock
            },

            // Le technicien réalise des interventions plus simples (pas de clôture définitive).
            [RoleType.Technicien] = new HashSet<Permission>
            {
                Permission.ConsulterTableauBord,
                Permission.ConsulterInterventions, Permission.CreerIntervention,
                Permission.ChangerEtatIntervention, Permission.GenererRapport,
                Permission.ConsulterParc,
                Permission.ConsulterPieces, Permission.GererStock
            },

            // Le client (hôpital) déclare des pannes et suit ses interventions.
            [RoleType.Client] = new HashSet<Permission>
            {
                Permission.ConsulterTableauBord,
                Permission.ConsulterInterventions, Permission.CreerIntervention,
                Permission.ConsulterParc
            },

            // L'invité n'a qu'un accès en lecture au tableau de bord et aux interventions.
            [RoleType.Invite] = new HashSet<Permission>
            {
                Permission.ConsulterTableauBord,
                Permission.ConsulterInterventions
            }
        };

    /// <summary>Indique si un rôle possède la permission demandée.</summary>
    public static bool Possede(RoleType role, Permission permission)
        => Matrice.TryGetValue(role, out var permissions) && permissions.Contains(permission);

    /// <summary>Renvoie l'ensemble des permissions d'un rôle (vide si rôle inconnu).</summary>
    public static IReadOnlyCollection<Permission> Pour(RoleType role)
        => Matrice.TryGetValue(role, out var permissions)
            ? permissions
            : Array.Empty<Permission>();
}

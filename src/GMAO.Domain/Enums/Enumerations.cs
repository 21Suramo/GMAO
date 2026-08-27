namespace GMAO.Domain.Enums;

/// <summary>Rôles utilisateurs (contrôle d'accès basé sur les rôles — RBAC).</summary>
public enum RoleType
{
    Administrateur = 1,
    ResponsableSAV = 2,
    Ingenieur = 3,
    Technicien = 4,
    Client = 5,
    Invite = 6
}

/// <summary>
/// Permission élémentaire, associée à une action métier précise.
/// Le contrôle d'accès se fait action par action (et non plus seulement écran par écran) :
/// chaque service applicatif exige la permission correspondante avant d'exécuter la logique.
/// </summary>
public enum Permission
{
    // Tableau de bord
    ConsulterTableauBord = 1,
    ConsulterTableauBordGlobal = 2,

    // Interventions
    ConsulterInterventions = 10,
    CreerIntervention = 11,
    AffecterIntervention = 12,
    ChangerEtatIntervention = 13,
    ClorerIntervention = 14,
    GenererRapport = 15,

    // Parc
    ConsulterParc = 20,
    GererParc = 21,

    // Pièces
    ConsulterPieces = 30,
    GererStock = 31,
    SupprimerPiece = 32,

    // Administration
    GererUtilisateurs = 40
}

/// <summary>État d'un respirateur dans le parc.</summary>
public enum EtatRespirateur
{
    EnService = 1,
    EnMaintenance = 2,
    HorsService = 3,
    EnAttente = 4
}

/// <summary>États du workflow d'une intervention de maintenance corrective.</summary>
public enum EtatIntervention
{
    Nouvelle = 1,
    Affectee = 2,
    EnDeplacement = 3,
    Diagnostic = 4,
    Reparation = 5,
    EnAttentePiece = 6,
    Test = 7,
    Validation = 8,
    Cloturee = 9,
    Annulee = 10
}

/// <summary>Niveau de priorité d'une intervention.</summary>
public enum Priorite
{
    Basse = 1,
    Normale = 2,
    Haute = 3,
    Critique = 4
}

/// <summary>Type de document technique associé à un respirateur ou un modèle.</summary>
public enum TypeDocument
{
    ManuelUtilisateur = 1,
    ManuelMaintenance = 2,
    Schema = 3,
    Procedure = 4,
    Calibration = 5,
    GuideSAV = 6,
    Photo = 7,
    Autre = 99
}

/// <summary>Catégories de notifications temps réel.</summary>
public enum TypeNotification
{
    NouvelleDI = 1,
    InterventionUrgente = 2,
    PieceIndisponible = 3,
    StockFaible = 4,
    RespirateurCritique = 5,
    FinIntervention = 6,
    TempsDepasse = 7
}

/// <summary>Type de mouvement de stock d'une pièce détachée.</summary>
public enum TypeMouvement
{
    Entree = 1,
    Sortie = 2,
    Ajustement = 3
}

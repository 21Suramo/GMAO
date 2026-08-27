# Diagrammes UML — GMAO Datex-Ohmeda

> Diagrammes en notation Mermaid (rendus par GitHub/VS Code). Couvrent : cas d'utilisation, classes (domaine), séquence et états.

## 1. Diagramme de cas d'utilisation

```mermaid
graph LR
    subgraph Acteurs
        ADM([Administrateur])
        SAV([Responsable SAV])
        ING([Ingénieur Biomédical])
        TEC([Technicien])
        CLI([Client Hôpital])
        INV([Invité])
    end

    subgraph "Système GMAO"
        UC1[Gérer utilisateurs & rôles]
        UC2[Gérer le parc respirateurs]
        UC3[Déclarer une intervention - QR]
        UC4[Affecter une intervention]
        UC5[Réaliser le diagnostic & réparation]
        UC6[Gérer le stock de pièces]
        UC7[Clôturer via check-list]
        UC8[Générer rapport PDF]
        UC9[Consulter dashboard & KPI]
        UC10[Recevoir notifications]
        UC11[Rechercher - multicritère]
        UC12[Déclarer HORS SERVICE]
    end

    ADM --> UC1
    ADM --> UC2
    ADM --> UC12
    SAV --> UC4
    SAV --> UC9
    SAV --> UC6
    ING --> UC5
    ING --> UC7
    ING --> UC8
    TEC --> UC5
    CLI --> UC3
    CLI --> UC11
    INV --> UC9
    SAV --> UC10
    ING --> UC10
```

## 1 bis. Cas d'utilisation détaillés — relations «include» / «extend»

> Cette section formalise le **contrôle d'accès action par action** (chaque cas métier inclut la
> vérification de permission) et les cas optionnels (extend). Les rôles et permissions sont ceux
> réellement définis dans `GMAO.Domain` (`RoleType`, `Permission`, `MatricePermissions`).
> Mermaid ne dispose pas d'un type « use case » natif : les relations sont représentées par des
> flèches pointillées explicitement étiquetées, complétées par le tableau ci-dessous.

```mermaid
graph LR
    ADM([Administrateur])
    SAV([Responsable SAV])
    ING([Ingénieur])
    TEC([Technicien])
    CLI([Client Hôpital])

    %% Cas d'utilisation de base
    UcCreer[Créer une intervention]
    UcAffecter[Affecter une intervention]
    UcWorkflow[Faire avancer le workflow]
    UcCloturer[Clôturer une intervention]
    UcRapport[Générer le rapport PDF]
    UcStock[Enregistrer un mouvement de stock]
    UcUsers[Gérer les utilisateurs]
    UcDash[Consulter le tableau de bord]
    UcHisto[Consulter l'historique des interventions]

    %% Cas inclus (systématique) et cas d'extension (conditionnels)
    UcAuth{{S'authentifier / Vérifier la permission}}
    UcReaffecter[[Réaffecter à un autre ingénieur]]
    UcRecherche[[Filtrer / rechercher dans l'historique]]
    UcVueGlobale[[Afficher la vue globale du parc]]

    %% Associations acteur → cas
    ADM --> UcUsers
    SAV --> UcAffecter
    SAV --> UcDash
    SAV --> UcStock
    ING --> UcWorkflow
    ING --> UcCloturer
    ING --> UcRapport
    TEC --> UcWorkflow
    CLI --> UcCreer
    CLI --> UcHisto

    %% «include» : tout cas métier inclut la vérification de permission
    UcCreer -.->|«include»| UcAuth
    UcAffecter -.->|«include»| UcAuth
    UcWorkflow -.->|«include»| UcAuth
    UcCloturer -.->|«include»| UcAuth
    UcRapport -.->|«include»| UcAuth
    UcStock -.->|«include»| UcAuth
    UcUsers -.->|«include»| UcAuth
    UcDash -.->|«include»| UcAuth

    %% «extend» : cas optionnels qui étendent un cas de base sous condition
    UcReaffecter -.->|«extend»| UcAffecter
    UcRecherche -.->|«extend»| UcHisto
    UcVueGlobale -.->|«extend»| UcDash
```

### Relation «include» — vérification systématique de permission

Le cas **« S'authentifier / Vérifier la permission »** est **inclus** par tous les cas d'utilisation
métier : il matérialise l'appel, en tout début de chaque méthode de service, à
`IAutorisationService.AutoriserAsync(permission)`. La vérification revérifie l'identité en base
(compte existant et actif) puis consulte `MatricePermissions`. Un refus renvoie un `Result.Echec`
« Accès refusé » — jamais d'exception.

| Cas d'utilisation de base | Permission requise (incluse) | Service porteur |
|---|---|---|
| Créer une intervention | `CreerIntervention` | `InterventionService.CreerAsync` |
| Affecter une intervention | `AffecterIntervention` | `AffectationService.AffecterAsync` |
| Faire avancer le workflow | `ChangerEtatIntervention` | `InterventionService.ChangerEtatAsync` |
| Clôturer une intervention | `ClorerIntervention` | `InterventionService.ChangerEtatAsync` (état → Clôturée) |
| Générer le rapport PDF | `GenererRapport` | `RapportService.GenererRapportInterventionAsync` |
| Enregistrer un mouvement de stock | `GererStock` | `PieceService.EnregistrerMouvementAsync` |
| Gérer les utilisateurs | `GererUtilisateurs` | `UtilisateurService` (CRUD) |
| Consulter le tableau de bord | `ConsulterTableauBord` | `TableauBordService.ObtenirAsync` |

### Relation «extend» — cas optionnels conditionnels

| Cas d'extension | Étend le cas de base | Condition d'activation |
|---|---|---|
| Réaffecter à un autre ingénieur | Affecter une intervention | Ingénieur indisponible / changement de charge (exige aussi `AffecterIntervention`) |
| Filtrer / rechercher dans l'historique | Consulter l'historique des interventions | L'utilisateur saisit un critère de recherche |
| Afficher la vue globale du parc | Consulter le tableau de bord | Le rôle possède `ConsulterTableauBordGlobal` (responsable / administrateur) — sinon vue personnelle |

## 2. Diagramme de classes (domaine simplifié)

```mermaid
classDiagram
    class EntiteBase {
        +int Id
        +DateTime DateCreation
        +DateTime? DateModification
    }
    class Utilisateur {
        +string Login
        +string MotDePasseHash
        +RoleType Role
        +bool Actif
    }
    class Respirateur {
        +string NumeroSerie
        +string CodeInterne
        +Guid CodeQr
        +string VersionLogicielle
        +string VersionMaterielle
        +EtatRespirateur Etat
        +DateTime DateMiseEnService
        +bool EstHorsService()
    }
    class ModeleRespirateur {
        +string Nom
        +string Gamme
    }
    class Intervention {
        +string NumeroDI
        +DateTime Date
        +string Description
        +string Diagnostic
        +EtatIntervention Etat
        +Priorite Priorite
        +bool PatientConnecte
        +int TempsDeplacement
        +int TempsReparation
        +decimal MainOeuvre
        +bool PeutCloturer()
        +decimal CoutTotal()
    }
    class CheckListCloture {
        +bool AutotestOk
        +bool TestEtancheite
        +bool CalibrationDebit
        +bool CalibrationO2
        +bool Batterie
        +bool Alimentation
        +bool Alarmes
        +bool ValidationFinale
        +bool EstComplete()
    }
    class Piece {
        +string Reference
        +int Stock
        +int StockMinimum
        +decimal Prix
        +bool EnAlerte()
    }
    class Ingenieur {
        +string Nom
        +string Zone
        +bool EstDisponible(date)
    }
    class Panne {
        +string Libelle
    }

    EntiteBase <|-- Utilisateur
    EntiteBase <|-- Respirateur
    EntiteBase <|-- Intervention
    EntiteBase <|-- Piece
    EntiteBase <|-- Ingenieur
    ModeleRespirateur "1" --> "*" Respirateur
    Respirateur "1" --> "*" Intervention
    Intervention "1" --> "1" CheckListCloture
    Intervention "*" --> "1" Ingenieur
    Intervention "*" --> "1" Panne
    Intervention "*" --> "*" Piece : consomme
    Panne "*" --> "*" Piece : suspecte
```

## 3. Diagramme de séquence — Déclaration critique (patient connecté)

```mermaid
sequenceDiagram
    actor Client as Client (Hôpital)
    participant UI as Interface DI (WPF)
    participant App as InterventionService
    participant Affect as AffectationService
    participant DB as UnitOfWork/EF Core
    participant Notif as NotificationService
    participant Node as Serveur Node.js (SignalR)

    Client->>UI: Scan QR + symptômes + Patient connecté = Oui
    UI->>App: CreerDepuisDeclaration(dto)
    App->>App: Valider (FluentValidation)
    App->>App: Priorité = Critique, État = Affectée
    App->>Affect: ChoisirIngenieur(zone, compétences, dispo)
    Affect-->>App: Ingénieur sélectionné
    App->>DB: Enregistrer intervention + affectation
    DB-->>App: OK
    App->>Notif: Push("Intervention critique")
    Notif->>Node: Émettre via SignalR
    Node-->>UI: Notification desktop
    Node-->>Affect: Email à l'ingénieur + Responsable SAV
    App-->>UI: Result<InterventionDto>
    UI-->>Client: Confirmation (n° DI)
```

## 4. Diagramme d'états — Intervention

```mermaid
stateDiagram-v2
    [*] --> Nouvelle
    Nouvelle --> Affectee : affectation (auto/manuelle)
    Affectee --> EnDeplacement : départ ingénieur
    EnDeplacement --> Diagnostic : arrivée sur site
    Diagnostic --> Reparation : cause identifiée
    Reparation --> EnAttentePiece : rupture de stock
    EnAttentePiece --> Reparation : pièce reçue
    Reparation --> Test : réparation terminée
    Test --> Validation : test fonctionnel OK
    Validation --> Cloturee : check-list complète
    Nouvelle --> Annulee
    Affectee --> Annulee
    Cloturee --> [*]
    Annulee --> [*]
```

## 5. Diagramme de séquence — Clôture avec check-list

```mermaid
sequenceDiagram
    actor Ing as Ingénieur
    participant UI as Vue Intervention
    participant App as InterventionService
    participant Dom as Domaine (Intervention)
    participant Pdf as RapportPdfService
    participant DB as UnitOfWork

    Ing->>UI: Coche check-list + signature
    UI->>App: Cloturer(interventionId, checklist, signature)
    App->>Dom: PeutCloturer()
    alt check-list incomplète
        Dom-->>App: false
        App-->>UI: Result.Echec("Check-list obligatoire")
    else complète
        Dom-->>App: true
        App->>DB: État = Cloturee
        App->>Pdf: GenererRapport(intervention)
        Pdf-->>App: rapport.pdf
        App->>DB: Archiver rapport
        DB-->>App: OK
        App-->>UI: Result.Succes(rapport)
    end
```

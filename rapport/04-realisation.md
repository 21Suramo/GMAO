# Chapitre 4
# Réalisation, tests et déploiement

## 4.1 Introduction

Ce chapitre présente la mise en œuvre de la conception exposée au chapitre précédent. Nous décrivons l'environnement et les outils retenus, puis chaque module réalisé, en donnant pour les mécanismes centraux l'extrait de code correspondant. Nous exposons ensuite la stratégie de tests et sa couverture, les conditions de déploiement, et nous dressons enfin le bilan du travail au regard des vingt exigences fonctionnelles spécifiées au chapitre 2.

## 4.2 Environnement et outils

L'application est développée en C# sur la plateforme .NET 10, l'interface graphique reposant sur la technologie WPF. Le tableau 11 recense les bibliothèques employées et le rôle de chacune.

**Tableau 11 - Bibliothèques utilisées**

| Bibliothèque | Version | Rôle |
|---|---|---|
| Entity Framework Core (SQLite) | 10.0.9 | Correspondance objet-relationnel et migrations |
| CommunityToolkit.Mvvm | 8.4.2 | Support du modèle Modèle-Vue-ModèleDeVue |
| WPF-UI | 4.x | Composants d'interface de style Fluent |
| LiveChartsCore SkiaSharpView WPF | 2.1.0 | Graphiques du tableau de bord |
| AutoMapper | 13.0.1 | Mappage entre entités et objets de transfert |
| FluentValidation | 12.1.1 | Validation déclarative des requêtes |
| Serilog | 4.3.1 | Journalisation vers console et fichier |
| iText7 et son adaptateur Bouncy Castle | 9.6.0 | Génération des rapports au format PDF |
| QRCoder | 1.8.0 | Génération des QR Codes |
| BCrypt.Net-Next | 4.2.0 | Hachage des mots de passe |
| Express, ws, cors | — | Serveur de notifications sous Node.js |
| xUnit, FluentAssertions, Moq | 2.9.3 / 8.10.0 / 4.20.72 | Tests unitaires, assertions lisibles, doublures |

Deux contraintes de version méritent d'être signalées, car elles ont orienté des choix concrets. AutoMapper est figé en version 13.0.1 : les versions 15 et suivantes exigent une licence payante. iText7 en version 9 requiert la présence explicite du paquet `itext7.bouncy-castle-adapter` aux côtés du noyau, faute de quoi l'écriture du document échoue à l'exécution.

## 4.3 Mise en œuvre des modules

### 4.3.1 Authentification et gestion des comptes

L'application s'ouvre sur une fenêtre de connexion. Le service d'authentification recherche le compte, vérifie l'empreinte du mot de passe par BCrypt, contrôle que le compte est actif, puis consigne la tentative — réussie ou non — dans le journal des connexions.

`[Figure 12 - Fenêtre de connexion]`
`[CAPTURE À FOURNIR]`

Le module d'administration des comptes, réservé au rôle Administrateur, offre la création, la modification, l'activation et la désactivation, la suppression logique, la réinitialisation du mot de passe et la recherche parmi les comptes. Chaque compte porte l'auteur et la date de sa création et de sa dernière modification, renseignés automatiquement par le mécanisme d'audit décrit en 3.4.

`[Figure 13 - Écran d'administration des comptes]`
`[CAPTURE À FOURNIR]`

L'écran des paramètres donne à chaque utilisateur l'accès à son profil, au changement de son mot de passe et à l'historique de ses connexions.

### 4.3.2 Parc et QR Code

Le module de parc gère la hiérarchie hôpital, service, bloc opératoire, respirateur, et présente la fiche de chaque appareil : numéro de série, code interne, modèle, versions logicielle et matérielle, date de mise en service, état et rattachement.

`[Figure 14 - Écran de gestion du parc]`
`[CAPTURE À FOURNIR]`

Chaque respirateur porte un identifiant unique matérialisé par un QR Code, généré au format PNG à partir de cet identifiant. L'étiquette apposée sur l'appareil permet de l'identifier sans ambiguïté lors d'une déclaration de panne.

`[Figure 15 - QR Code généré pour un respirateur]`
`[CAPTURE À FOURNIR]`

La lecture du QR Code par caméra n'est pas réalisée dans cette version : l'identification passe par la sélection de l'appareil dans l'interface. Ce point est repris au bilan en 4.6.

### 4.3.3 Interventions et workflow

Le module d'interventions couvre la déclaration, le suivi et la progression dans le workflow. La déclaration présente la liste des symptômes prédéfinis, la saisie d'une description et la case déterminante indiquant si un patient est connecté à l'appareil.

`[Figure 16 - Écran des interventions]`
`[CAPTURE À FOURNIR]`

La progression s'effectue par une commande qui applique l'état suivant du workflow, complétée par deux commandes particulières : la mise en attente de pièce et la clôture. Chaque changement d'état est enregistré dans l'historique de l'intervention avec son auteur, conformément à la règle RG-07.

La figure 17 donne le cœur de la règle RG-01, telle qu'elle s'exécute lors de la création d'une intervention.

```csharp
// Numéro de DI séquentiel par année.
var annee = DateTime.UtcNow.Year;
var nbAnnee = await _unitOfWork.Repository<Intervention>()
    .CountAsync(i => i.Date.Year == annee, cancellationToken);
var numeroDI = $"DI-{annee}-{(nbAnnee + 1):D4}";

// RG-01 : patient connecté ⇒ criticité maximale + affectation immédiate.
var priorite = requete.PatientConnecte ? Priorite.Critique : requete.Priorite;
var etat = EtatIntervention.Nouvelle;
int? ingenieurId = null;

if (requete.PatientConnecte)
{
    // Affectation automatique (compétences, zone, disponibilité, charge).
    ingenieurId = await _affectation.ChoisirIngenieurAsync(
        requete.RespirateurId, DateTime.UtcNow, cancellationToken);
    if (ingenieurId is not null)
        etat = EtatIntervention.Affectee;
}
```

**Figure 17 - Application de la règle RG-01 à la création d'une intervention**

On observe que la priorité saisie par l'utilisateur est ignorée lorsqu'un patient est connecté, et que l'absence d'ingénieur disponible n'empêche pas la création : l'intervention demeure alors à l'état *Nouvelle* avec la priorité *Critique*, et le tableau de bord la compte parmi les demandes en attente d'affectation.

Le tableau Kanban avec déplacement des cartes, spécifié par l'exigence EF-09, n'a pas été retenu à la réalisation. Nous avons privilégié le workflow d'états, qui porte la même information tout en garantissant que chaque transition passe par le contrôle de permission et par l'historisation. Un déplacement libre de cartes aurait autorisé des transitions non prévues par le diagramme d'états de la figure 9.

### 4.3.4 Pièces détachées

Le module de pièces détachées présente le catalogue avec la référence, le nom, la catégorie, le stock, le stock minimum, le prix et le fournisseur, et signale les pièces en alerte conformément à la règle RG-04. Les mouvements de stock sont tracés en entrée, en sortie et en ajustement.

`[Figure 18 - Écran des pièces détachées]`
`[CAPTURE À FOURNIR]`

### 4.3.5 Rapports au format PDF

À la clôture d'une intervention, un rapport est généré au format PDF puis archivé. Le document comporte un en-tête portant l'identification de l'intervention et le QR Code de l'appareil, les informations du client et du respirateur, la description de la panne, le diagnostic, un tableau des pièces détachées consommées avec les quantités et les prix, les temps de déplacement et de réparation, le coût total et un emplacement de signature.

`[Figure 19 - Rapport d'intervention généré]`
`[CAPTURE À FOURNIR]`

L'écran des rapports permet de lister les rapports archivés, d'en générer de nouveaux et d'ouvrir un document existant.

### 4.3.6 Tableau de bord et indicateurs

Le tableau de bord restitue l'activité du service sur une période sélectionnable. Il présente le nombre de respirateurs, les appareils en service et hors service, la disponibilité globale, les interventions actives, celles en attente d'affectation, celles qui dépassent le délai convenu, le délai moyen de résolution, le délai moyen d'affectation, le coût cumulé et le nombre de pièces en alerte. Il fournit également la répartition des interventions par état, par modèle, le diagramme de Pareto des pannes, la disponibilité par équipement, les classements des respirateurs et des hôpitaux les plus sollicités, et la charge par technicien.

`[Figure 20 - Tableau de bord]`
`[CAPTURE À FOURNIR]`

Les indicateurs sont calculés par une classe dédiée, sans effet de bord. Le délai de résolution est la moyenne des écarts entre la création et la clôture des interventions closes ; le délai d'affectation, la moyenne des écarts entre la création et l'affectation ; la disponibilité, le complément du rapport du temps d'immobilisation à la durée de la période. Le dépassement du délai convenu s'apprécie au regard d'un seuil dépendant de la priorité : quatre heures pour une intervention critique, vingt-quatre heures pour une priorité haute, soixante-douze heures pour une priorité normale et une semaine pour une priorité basse.

Le tableau de bord s'adapte au rôle : les utilisateurs disposant de la permission de consultation globale voient l'ensemble du parc, les autres une vue limitée à leur périmètre. Un écran de statistiques complète ce dispositif par des analyses sur des périodes plus longues.

`[Figure 21 - Écran de statistiques]`
`[CAPTURE À FOURNIR]`

Le temps moyen entre pannes, prévu par la règle RG-06, n'est pas implémenté dans cette version. Son calcul suppose de connaître le temps de service cumulé de chaque appareil, donnée que le modèle actuel ne collecte pas.

### 4.3.7 Notifications temps réel

Le serveur de notifications est un programme Node.js autonome, extérieur à la solution .NET. Il expose une interface REST comportant trois points d'entrée — l'émission d'une notification, un contrôle de bon fonctionnement et la consultation de l'historique — ainsi qu'un canal WebSocket sur lequel il diffuse chaque notification à tous les clients connectés. Le port d'écoute vaut 4000 par défaut et se configure par variable d'environnement.

Côté application, un client WebSocket se connecte à ce canal et alimente une cloche de notification accompagnée d'un panneau déroulant. L'émission est déclenchée notamment lors de la création d'une intervention critique.

`[Figure 22 - Panneau de notifications]`
`[CAPTURE À FOURNIR]`

Conformément au choix de couplage faible exposé en 3.3.5, l'indisponibilité du serveur n'empêche pas l'application de fonctionner. Le relais des notifications par courriel n'est pas réalisé dans cette version.

### 4.3.8 Moteur d'affectation automatique

Le moteur d'affectation constitue la réponse au quatrième constat de l'analyse de l'existant : l'affectation empirique des ingénieurs. La figure 23 en donne l'intégralité de la logique de décision.

```csharp
public const int PointsCompetence = 50;
public const int PointsZone = 30;
public const int PenaliteParInterventionOuverte = 5;

/// <summary>Calcule le score d'adéquation d'un candidat (plus élevé = meilleur).</summary>
public static int Score(IngenieurCandidat candidat, ContexteAffectation contexte)
{
    var score = 0;

    if (candidat.CompetencesModeles.Any(m =>
            string.Equals(m, contexte.ModeleNom, StringComparison.OrdinalIgnoreCase)))
        score += PointsCompetence;

    if (!string.IsNullOrWhiteSpace(contexte.VilleHopital)
        && string.Equals(candidat.Zone, contexte.VilleHopital, StringComparison.OrdinalIgnoreCase))
        score += PointsZone;

    score -= candidat.NbInterventionsOuvertes * PenaliteParInterventionOuverte;
    return score;
}

public static IngenieurCandidat? Choisir(
    IEnumerable<IngenieurCandidat> candidats, ContexteAffectation contexte)
    => candidats
        .Where(c => c.EstDisponible)
        .OrderByDescending(c => Score(c, contexte))
        .ThenBy(c => c.NbInterventionsOuvertes)
        .ThenBy(c => c.Id)
        .FirstOrDefault();
```

**Figure 23 - Moteur d'affectation : calcul du score et sélection du candidat**

Trois points méritent d'être relevés. D'abord, la pondération traduit une hiérarchie de priorités explicite : la compétence sur le modèle concerné pèse cinquante points, la présence dans la zone géographique trente, et chaque intervention déjà ouverte retranche cinq points. La compétence prime donc sur la proximité, et un ingénieur compétent devient moins intéressant qu'un autre à partir de quatre interventions ouvertes d'écart. Ensuite, l'indisponibilité est un filtre et non une pénalité : un ingénieur en congé est écarté, jamais choisi en dernier recours. Enfin, les deux critères de départage successifs — la charge puis l'identifiant — rendent la sélection **déterministe** : à situation identique, le moteur choisit toujours le même ingénieur, ce qui rend la décision reproductible et testable.

## 4.4 Tests

### 4.4.1 Stratégie

La testabilité obtenue par l'architecture est mise à profit sur les mécanismes dont une défaillance aurait les conséquences les plus lourdes : le contrôle d'accès, la décision d'affectation, le calcul des indicateurs, la validation des saisies et la génération des rapports. Les tests s'exécutent en mémoire, les dépendances étant remplacées par des doublures.

### 4.4.2 Couverture

Le projet de tests comporte trente-sept cas répartis sur six classes. Le tableau 12 en donne le détail.

**Tableau 12 - Couverture des tests unitaires**

| Classe de tests | Cas | Objet de la vérification |
|---|:---:|---|
| `AutorisationServiceTests` | 6 | Revérification du compte en base, compte inactif, refus, autorisation accordée |
| `CalculateurKpiTests` | 10 | Délai de résolution, délai d'affectation, disponibilité, seuils et dépassement des délais convenus |
| `CreerUtilisateurRequeteValidatorTests` | 10 | Règles de validation à la création d'un compte |
| `MatricePermissionsTests` | 5 | Droits de chaque rôle et étanchéité entre rôles |
| `MoteurAffectationTests` | 5 | Pondération du score, exclusion des indisponibles, départage |
| `RapportPdfGenerateurTests` | 1 | Production effective d'un document PDF valide |
| **Total** | **37** | |

`[Figure 24 - Exécution de la campagne de tests]`
`[CAPTURE À FOURNIR : sortie de la commande dotnet test sur le poste de développement]`

Les tests du calcul des indicateurs et de la validation des saisies emploient des jeux de données paramétrés, ce qui permet de couvrir plusieurs combinaisons de valeurs avec une seule méthode de test.

Il faut énoncer clairement la limite de cette campagne : elle porte sur la logique métier pure et ne couvre ni la couche de persistance, ni l'interface graphique. Les tests d'intégration sur base et les tests d'interface constituent une extension nécessaire, reprise en perspectives.

## 4.5 Déploiement

### 4.5.1 Production du livrable

Un script unique enchaîne la restauration des dépendances, la compilation en configuration Release, l'exécution des tests, la publication de l'application pour l'architecture Windows 64 bits et l'installation des dépendances du serveur de notifications.

La publication produit par défaut un livrable dépendant du cadriciel : le poste cible doit disposer du runtime .NET 10 Desktop. Une publication autonome, embarquant le runtime dans un exécutable unique, est également prévue pour les postes qui n'en disposent pas.

### 4.5.2 Installation et premier démarrage

L'installation consiste à copier le dossier publié sur le poste, à copier le serveur de notifications, à démarrer ce dernier puis à lancer l'application. Un fichier de commande à la racine du dépôt enchaîne ces deux derniers gestes.

Au premier démarrage, la base de données est créée automatiquement à côté de l'exécutable et alimentée avec un jeu de données de démonstration comportant un hôpital, quatre respirateurs, deux ingénieurs, cinq comptes utilisateurs couvrant les différents rôles, cinq pièces détachées, quatre pannes catalogues, cinq symptômes et une intervention. Ce jeu permet de parcourir l'ensemble des écrans dès l'installation.

Le tableau 13 récapitule l'emplacement des données produites par l'application.

**Tableau 13 - Emplacement des données**

| Élément | Emplacement |
|---|---|
| Base de données | `gmao.db`, à côté de l'exécutable |
| Journaux applicatifs | `logs/` |
| Rapports générés | `reports/generated/` |
| Serveur de notifications | `http://localhost:4000` |

### 4.5.3 Exploitation

La sauvegarde se réduit à la copie du fichier de base de données, accompagné de son fichier de journalisation anticipée lorsqu'il est présent. Le compte administrateur initial est créé avec un mot de passe par défaut qu'il convient de changer à la mise en service.

## 4.6 Bilan au regard des exigences

Le tableau 14 confronte les vingt exigences fonctionnelles spécifiées en 2.3.2 à l'état réel de la réalisation.

**Tableau 14 - Bilan de réalisation des exigences fonctionnelles**

| Identifiant | Exigence | État | Observation |
|---|---|---|---|
| EF-01 | Authentification avec hachage | Réalisée | BCrypt, facteur de travail 12 |
| EF-02 | Rôles et droits | Réalisée | Quatorze permissions, contrôle action par action |
| EF-03 | Historique des connexions | Réalisée | Tentatives réussies et échouées |
| EF-04 | Parc et fiche de vie | Réalisée | |
| EF-05 | QR Code | Partielle | Génération réalisée, lecture par caméra non réalisée |
| EF-06 | Déclaration simplifiée | Réalisée | |
| EF-07 | Détection du patient connecté | Réalisée | Règle RG-01 |
| EF-08 | Workflow complet | Réalisée | Dix états, transitions tracées |
| EF-09 | Tableau Kanban | Non réalisée | Écartée au profit du workflow d'états (voir 4.3.3) |
| EF-10 | Check-list de clôture | Réalisée | Règle RG-02 |
| EF-11 | Blocage des appareils hors service | Réalisée | |
| EF-12 | Stock et alertes | Réalisée | Règle RG-04 |
| EF-13 | Association panne et pièces | Réalisée | Modélisée et exploitée |
| EF-14 | Documentation technique | Non réalisée | Entité présente au modèle, module d'interface non développé |
| EF-15 | Rapports PDF | Réalisée | |
| EF-16 | Notifications temps réel | Partielle | Notification sur poste réalisée, relais par courriel non réalisé |
| EF-17 | Tableau de bord et indicateurs | Réalisée | Carte de chaleur et temps moyen entre pannes non réalisés |
| EF-18 | Affectation automatique | Partielle | Moteur réalisé et testé, interface de planning non développée |
| EF-19 | Recherche multicritère | Partielle | Filtres présents sur le tableau de bord, les statistiques et les comptes ; recherche transverse non généralisée |
| EF-20 | Thèmes et navigation | Partielle | Navigation latérale filtrée par rôle réalisée, thèmes clair et sombre non réalisés |

Sur vingt exigences, treize sont pleinement réalisées, cinq le sont partiellement et deux ne le sont pas. Les exigences de priorité haute portant le cœur du métier — traçabilité, criticité, workflow, check-list, rapports, indicateurs — sont couvertes. Les manques concernent des fonctions de confort ou d'ergonomie, à une exception près : la lecture du QR Code par caméra, qui appartenait au scénario de déclaration initialement envisagé.

## 4.7 Conclusion

Ce chapitre a exposé la réalisation des huit modules de l'application, en détaillant les deux mécanismes qui portent les objectifs du projet : l'application de la règle de criticité à la création d'une intervention et le moteur d'affectation automatique. Il a présenté une campagne de trente-sept tests unitaires ciblant la logique métier, décrit les conditions de production et d'installation du livrable, et dressé le bilan des vingt exigences spécifiées. Ce bilan, favorable sur le cœur métier et incomplet sur plusieurs fonctions périphériques, alimente directement la critique et les perspectives de la conclusion générale.

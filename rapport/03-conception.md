# Chapitre 3
# Conception

## 3.1 Introduction

Ce chapitre présente la conception de la solution. Nous exposons d'abord l'architecture générale et la règle de dépendance qui la gouverne, puis nous justifions les cinq décisions de conception structurantes du projet. Nous décrivons ensuite les patrons de conception appliqués, le modèle du domaine, le modèle de données, les comportements dynamiques du système et la conception du contrôle d'accès.

## 3.2 Architecture générale

### 3.2.1 Organisation en couches

Nous retenons une architecture en couches de type Clean Architecture, organisée en sept projets. La figure 6 en donne la vue d'ensemble.

`[Figure 6 - Architecture en couches de la solution]`
`[SOURCE : docs/03-architecture.md §1, à redessiner proprement]`

Le principe directeur est la **règle de dépendance** : les dépendances pointent toujours vers l'intérieur, c'est-à-dire vers le domaine.

- `GMAO.Domain` ne dépend de rien. Il contient les entités, les énumérations et les règles métier pures.
- `GMAO.Application` dépend du domaine. Il orchestre les cas d'utilisation et **déclare les interfaces** dont il a besoin — entrepôts, générateur de rapports, service de QR Code, notification.
- `GMAO.Persistence` et `GMAO.Infrastructure` dépendent de l'application et **implémentent** ces interfaces. Ce sont les adaptateurs techniques.
- `GMAO.Presentation.Wpf` dépend de l'application et compose les implémentations concrètes au démarrage.
- `GMAO.Shared` porte les types transverses et est référencé partout.

L'inversion à retenir est celle-ci : la couche applicative ne connaît pas Entity Framework Core, ni iText7, ni la bibliothèque de QR Code. Elle connaît des interfaces qu'elle a elle-même définies. Ce sont les couches techniques qui viennent s'y brancher.

Le tableau 7 détaille la responsabilité de chaque projet.

**Tableau 7 - Responsabilité des projets de la solution**

| Projet | Nature | Responsabilité |
|---|---|---|
| `GMAO.Domain` | Bibliothèque | Entités, énumérations, règles métier, matrice des permissions. Aucune dépendance externe |
| `GMAO.Application` | Bibliothèque | Services de cas d'utilisation, objets de transfert, validation, mappage, interfaces d'infrastructure |
| `GMAO.Persistence` | Bibliothèque | Contexte de persistance, configurations, entrepôts, unité de travail, migrations, jeu de données de démonstration |
| `GMAO.Infrastructure` | Bibliothèque | Génération PDF, QR Code, client de notification, hachage des mots de passe |
| `GMAO.Shared` | Bibliothèque | Type `Result`, constantes et utilitaires transverses |
| `GMAO.Presentation.Wpf` | Application WPF | Coquille, navigation, vues et modèles de vue, thèmes, composition des dépendances |
| `notification-server` | Serveur Node.js | Diffusion des notifications temps réel |

### 3.2.2 Pourquoi cette architecture

Le besoin non fonctionnel de maintenabilité, énoncé en 2.4, justifie à lui seul ce choix, mais trois conséquences concrètes méritent d'être explicitées.

D'abord, **la testabilité**. Les règles métier résidant dans un projet sans dépendance externe, leur vérification ne demande ni base de données, ni interface graphique. Le chapitre 4 montre que les règles les plus sensibles du projet — moteur d'affectation, matrice des permissions, calcul des indicateurs — se testent en mémoire.

Ensuite, **la substituabilité des choix techniques**. Le remplacement de la technologie de notification, décrit en 3.3.5, a effectivement eu lieu en cours de projet. Il n'a affecté que la couche d'infrastructure : la couche applicative continue d'appeler la même interface. Cette architecture a donc prouvé son intérêt sur un cas réel du projet, et non en théorie.

Enfin, **la lisibilité du code métier**. Un service applicatif se lit comme l'énoncé d'un cas d'utilisation, sans requête de base de données ni détail d'affichage.

Le coût de ce choix est réel et il faut l'assumer : sept projets et un jeu d'interfaces représentent une charge de structure supérieure à celle d'une application monolithique. Sur un projet de cette taille, la contrepartie en testabilité et en évolutivité la justifie.

## 3.3 Décisions de conception

### 3.3.1 Lecture par projection

Une difficulté apparaît dès lors que la couche applicative ne référence pas Entity Framework Core : comment interroger la base sans en dépendre ?

Nous résolvons ce point par une méthode de lecture générique qui accepte un filtre et une **projection**, tous deux exprimés sous forme d'arbres d'expression. L'entrepôt applique le filtre puis la projection dans la même requête, et renvoie directement des objets de transfert. La traduction en SQL a lieu dans la couche de persistance ; la couche applicative n'a manipulé que des expressions du langage.

Ce choix apporte un gain de performance non recherché initialement : la projection s'exécutant côté base, seules les colonnes utiles transitent, et le chargement d'entités complètes suivi d'un mappage en mémoire est évité.

### 3.3.2 Le patron Result plutôt que les exceptions

Les services applicatifs renvoient un type `Result` ou `Result<T>` plutôt que de lever une exception lorsqu'un cas d'échec attendu se produit : identifiants incorrects, accès refusé, check-list incomplète, entité introuvable.

Nous distinguons deux natures d'échec. L'échec **attendu** fait partie du fonctionnement normal : un utilisateur qui saisit un mauvais mot de passe n'est pas une anomalie du système. L'échec **exceptionnel** — base inaccessible, disque plein — relève, lui, de l'exception.

Traiter le premier cas par une exception présente trois inconvénients : le coût d'exécution du mécanisme d'exception sur un chemin fréquent, la perte de la signature de méthode comme documentation des cas d'échec, et le risque d'un traitement d'exception trop large qui masque une véritable anomalie. Le type `Result` rend au contraire les cas d'échec visibles dans la signature et impossibles à ignorer silencieusement.

### 3.3.3 Le patron Strategy pour l'affectation

Le moteur d'affectation est conçu comme une fonction pure : il reçoit une liste de candidats et un contexte, et renvoie le meilleur candidat. Il n'accède à aucune base de données, n'écrit aucun journal et ne produit aucun effet de bord.

Cette pureté est un choix délibéré. Elle permet d'exprimer les cas de test directement, sans construire de contexte de persistance, et elle isole la politique d'affectation du mécanisme qui la déclenche. Modifier la pondération relative des compétences, de la zone et de la charge ne demande de toucher qu'à cette classe.

La séparation des responsabilités est la suivante : le service d'affectation collecte les données depuis la base et enregistre le résultat ; le moteur décide.

### 3.3.4 SQLite comme base de données

L'application est destinée à un poste de bureau du service après-vente. Nous retenons SQLite, base embarquée dans un fichier unique, pour trois raisons : l'installation ne demande aucun serveur de base de données sur le poste client, la sauvegarde se réduit à la copie d'un fichier, et l'accès à l'ensemble du modèle en mode Code First reste identique à celui d'un serveur relationnel classique.

Cette décision a une limite qu'il faut énoncer : SQLite ne convient pas à un déploiement multi-postes avec écritures concurrentes soutenues. Le passage à un serveur relationnel constituerait une évolution naturelle, et le choix d'Entity Framework Core en mode Code First le rend accessible sans réécrire la couche applicative.

### 3.3.5 Un serveur WebSocket natif plutôt que SignalR

La spécification initiale prévoyait SignalR pour les notifications temps réel, adossé à un serveur Node.js. Cette combinaison s'est révélée impraticable : SignalR est une technologie de l'écosystème ASP.NET Core, et son auto-hébergement sous Node.js n'est pas supporté de façon satisfaisante.

Nous avons donc remplacé SignalR par un **serveur WebSocket natif** : un serveur Node.js fondé sur Express pour l'interface REST et sur la bibliothèque `ws` pour la diffusion. L'application cliente s'y connecte au moyen d'un client WebSocket standard.

Deux conséquences méritent d'être relevées. D'une part, le changement n'a touché que la couche d'infrastructure, comme annoncé en 3.2.2. D'autre part, nous avons retenu un **couplage faible** : si le serveur de notifications n'est pas démarré, l'application fonctionne normalement et les notifications sont simplement ignorées. Une fonction de confort ne doit pas conditionner la disponibilité d'un outil dont dépend la maintenance d'équipements critiques.

## 3.4 Patrons de conception appliqués

Outre le patron Strategy déjà décrit, la solution applique cinq patrons.

**Entrepôt et unité de travail.** L'entrepôt abstrait l'accès aux données par type d'entité ; l'unité de travail regroupe les modifications d'un cas d'utilisation en une seule validation, garantissant la cohérence transactionnelle demandée en 2.4.

**Modèle-Vue-ModèleDeVue.** L'interface graphique sépare la vue déclarative de son modèle de vue. Aucune logique métier ne figure dans le code accompagnant les vues. La navigation résout la vue à partir du modèle de vue par correspondance de type.

**Injection de dépendances.** La composition a lieu en un point unique, au démarrage de l'application. Un point d'attention particulier concerne la durée de vie : le contexte de persistance ayant une durée de vie limitée à une opération, les services applicatifs sont résolus par portée à l'intérieur des modèles de vue, et non injectés comme instances uniques.

**Suppression logique et audit centralisés.** Toutes les entités héritent d'une classe de base portant les champs d'audit et un indicateur de suppression. Le contexte de persistance renseigne automatiquement les dates et les auteurs lors de l'enregistrement, et applique un filtre global qui exclut les entités supprimées de toute requête. Ni l'audit ni la suppression logique ne sont donc écrits à la main dans les services : ils ne peuvent pas être oubliés.

**Validation aux frontières.** Les requêtes entrantes sont validées par des règles déclaratives, à l'entrée de la couche applicative.

## 3.5 Modèle du domaine

La figure 7 présente les classes principales du domaine et leurs relations.

`[Figure 7 - Diagramme de classes du domaine]`
`[SOURCE : docs/05-diagrammes-uml.md §2, à exporter en image]`

Le point à retenir de ce diagramme est la présence de **méthodes de comportement** sur les entités, et non de simples propriétés. L'intervention sait dire si elle peut être clôturée, si elle relève d'un cas critique, quel est son coût total et son temps d'immobilisation. La check-list sait dire si elle est complète. Le respirateur sait dire s'il est hors service. La pièce détachée sait dire si elle est en alerte de stock.

Ce choix place les règles de gestion au plus près des données qu'elles concernent. La règle RG-02, par exemple, s'exprime dans la méthode qui répond à la question « cette intervention peut-elle être clôturée ? » : la check-list existe, elle est complète, et l'intervention n'est ni déjà clôturée ni annulée. Le service applicatif se contente d'interroger le domaine, il ne réimplémente pas la règle.

## 3.6 Modèle de données

### 3.6.1 Entités

Le modèle comporte vingt-cinq entités persistées, réparties en six domaines fonctionnels. Le tableau 8 les recense.

**Tableau 8 - Entités du modèle de données**

| Domaine | Entités |
|---|---|
| Sécurité | `Utilisateur`, `HistoriqueConnexion` |
| Parc | `Hopital`, `Service`, `BlocOperatoire`, `ModeleRespirateur`, `Respirateur`, `DocumentTechnique` |
| Interventions | `Intervention`, `Symptome`, `Panne`, `CheckListCloture`, `LignePieceIntervention`, `HistoriqueEtatIntervention`, `Rapport`, `PhotoDocument` |
| Pièces | `Piece`, `CategoriePiece`, `Fournisseur`, `MouvementStock`, `PanneePiece` |
| Planning | `Ingenieur`, `Conge`, `Competence` |
| Notifications | `Notification` |

Les associations plusieurs-à-plusieurs — entre l'intervention et les symptômes, entre l'ingénieur et ses compétences — donnent lieu à des tables de liaison supplémentaires, portant le schéma à vingt-sept tables.

### 3.6.2 Énumérations

Les états et catégories du système sont modélisés par des énumérations plutôt que par des chaînes de caractères, ce qui rend les valeurs invalides impossibles à représenter. Le tableau 9 les présente.

**Tableau 9 - Énumérations du domaine**

| Énumération | Valeurs |
|---|---|
| `RoleType` | Administrateur, ResponsableSAV, Ingenieur, Technicien, Client, Invite |
| `Permission` | Quatorze permissions réparties en cinq groupes (tableau de bord, interventions, parc, pièces, administration) |
| `EtatRespirateur` | EnService, EnMaintenance, HorsService, EnAttente |
| `EtatIntervention` | Nouvelle, Affectee, EnDeplacement, Diagnostic, Reparation, EnAttentePiece, Test, Validation, Cloturee, Annulee |
| `Priorite` | Basse, Normale, Haute, Critique |
| `TypeNotification` | NouvelleDI, InterventionUrgente, PieceIndisponible, StockFaible, RespirateurCritique, FinIntervention, TempsDepasse |
| `TypeMouvement` | Entree, Sortie, Ajustement |

### 3.6.3 Schéma relationnel

La figure 8 présente le modèle entité-association.

`[Figure 8 - Diagramme entité-association]`
`[SOURCE : docs/04-modele-de-donnees.md §4, à exporter en image]`

Le parc s'organise en une hiérarchie stricte : un hôpital possède des services, un service contient des blocs opératoires, un bloc opératoire héberge des respirateurs. Cette hiérarchie n'est pas décorative : c'est par elle que le système remonte de l'appareil à l'hôpital lors de la déclaration d'une panne, et une intervention ne peut être créée sur un respirateur qui n'est rattaché à aucun bloc.

Le schéma applique quatre conventions. Chaque entité porte un identifiant entier auto-incrémenté ainsi que les champs d'audit hérités de la classe de base. Des index d'unicité protègent le numéro de série et le code QR du respirateur, le numéro de demande d'intervention et la référence de pièce détachée. Les pièces jointes sont stockées sur le système de fichiers, seules leurs métadonnées figurant en base. Enfin, la génération du schéma suit l'approche Code First : le modèle relationnel dérive des classes du domaine, et son évolution est versionnée par des migrations.

## 3.7 Comportements dynamiques

### 3.7.1 Cycle de vie d'une intervention

La figure 9 formalise le workflow décrit en 2.5.

`[Figure 9 - Diagramme d'états d'une intervention]`
`[SOURCE : docs/05-diagrammes-uml.md §4, à exporter en image]`

### 3.7.2 Déclaration d'une intervention critique

La figure 10 déroule le scénario complet d'une déclaration avec patient connecté, qui met en jeu la règle RG-01 et enchaîne quatre mécanismes distincts.

`[Figure 10 - Diagramme de séquence : déclaration d'une intervention critique]`
`[SOURCE : docs/05-diagrammes-uml.md §3, à exporter en image — REMPLACER « SignalR » par « WebSocket » conformément à 3.3.5]`

L'enchaînement est le suivant. La requête est d'abord validée. Le système vérifie ensuite que le respirateur existe et qu'il est rattaché à un hôpital. Il attribue un numéro de demande séquentiel par année, de la forme `DI-AAAA-NNNN`. La règle RG-01 s'applique alors : la priorité est forcée à *Critique*, le moteur d'affectation est sollicité, et l'intervention passe à l'état *Affectée* si un ingénieur a pu être retenu. L'ensemble est enregistré en une seule validation, puis la notification est émise.

Un détail de conception mérite attention : l'affectation précède l'enregistrement. L'intervention est donc créée d'emblée dans son état définitif, plutôt que créée puis modifiée. Le cas critique se résout ainsi en une seule transaction.

### 3.7.3 Clôture avec check-list

La figure 11 déroule la clôture d'une intervention et l'application de la règle RG-02.

`[Figure 11 - Diagramme de séquence : clôture avec check-list]`
`[SOURCE : docs/05-diagrammes-uml.md §5, à exporter en image]`

Le scénario comporte une alternative explicite. Si la check-list est incomplète, le domaine refuse la clôture et le service renvoie un échec porteur du motif, sans lever d'exception, conformément au choix exposé en 3.3.2. Si elle est complète, l'intervention change d'état, le rapport est généré puis archivé.

## 3.8 Conception du contrôle d'accès

### 3.8.1 Matrice des permissions

Le contrôle d'accès repose sur quatorze permissions élémentaires plutôt que sur les six rôles. Une matrice associe à chaque rôle l'ensemble de ses permissions ; elle réside dans le domaine, sous forme de règle pure et sans effet de bord. Le tableau 10 la restitue.

**Tableau 10 - Matrice des rôles et des permissions**

| Permission | Admin. | Resp. SAV | Ingénieur | Technicien | Client | Invité |
|---|:---:|:---:|:---:|:---:|:---:|:---:|
| Consulter le tableau de bord | X | X | X | X | X | X |
| Consulter le tableau de bord global | X | X | | | | |
| Consulter les interventions | X | X | X | X | X | X |
| Créer une intervention | X | X | X | X | X | |
| Affecter une intervention | X | X | | | | |
| Changer l'état d'une intervention | X | X | X | X | | |
| Clôturer une intervention | X | X | X | | | |
| Générer un rapport | X | X | X | X | | |
| Consulter le parc | X | X | X | X | X | |
| Gérer le parc | X | X | | | | |
| Consulter les pièces | X | X | X | X | | |
| Gérer le stock | X | X | X | X | | |
| Supprimer une pièce | X | X | | | | |
| Gérer les utilisateurs | X | | | | | |

Deux lignes traduisent des décisions métier énoncées au chapitre 2. La permission de clôture est distincte de la permission de changement d'état : le technicien peut faire avancer une intervention mais ne peut pas la clôturer, ce qui matérialise la distinction posée en 2.2. La permission de consultation globale du tableau de bord sépare la vue globale du parc, réservée au responsable et à l'administrateur, de la vue personnelle servie aux autres rôles.

### 3.8.2 Vérification à l'exécution

La vérification s'effectue à l'entrée de chaque méthode de service applicatif, conformément à la relation d'inclusion présentée en 2.6.2. Elle procède en deux temps : le service **revérifie l'identité en base** — le compte existe-t-il encore, est-il toujours actif ? — puis consulte la matrice.

Cette revérification est un choix de sécurité important. Un compte désactivé pendant la session d'un utilisateur perd immédiatement ses droits, sans attendre une reconnexion. L'identité portée par la session n'est jamais considérée comme suffisante à elle seule.

Un refus renvoie un échec « Accès refusé » sous forme de `Result`, et non une exception.

### 3.8.3 Protection des mots de passe

Les mots de passe sont hachés par l'algorithme BCrypt avec un facteur de travail de 12. Ce paramètre fixe le coût du calcul d'une empreinte : chaque incrément double le temps nécessaire, ce qui ralentit d'autant une attaque par force brute. La valeur 12 constitue un compromis entre la résistance à l'attaque et un délai de connexion qui reste imperceptible pour l'utilisateur. BCrypt intègre par ailleurs un sel aléatoire par empreinte, ce qui rend inopérantes les tables précalculées.

## 3.9 Conclusion

Ce chapitre a présenté une architecture en sept projets gouvernée par la règle de dépendance, et justifié les cinq décisions structurantes du projet : lecture par projection, patron Result, moteur d'affectation pur, base embarquée et serveur WebSocket natif. Il a décrit un modèle de domaine porteur de comportement, un modèle de données de vingt-cinq entités, les comportements dynamiques du système et un contrôle d'accès à granularité fine, vérifié à l'exécution. Le chapitre suivant expose la réalisation de cette conception, les tests qui la valident et les conditions de son déploiement.

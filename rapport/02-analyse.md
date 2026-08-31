# Chapitre 2
# Analyse et spécification des besoins

## 2.1 Introduction

Ce chapitre traduit les objectifs du chapitre précédent en besoins spécifiés. Nous identifions d'abord les acteurs du système et leurs objectifs, puis nous énonçons les besoins fonctionnels sous une forme traçable, et les besoins non fonctionnels qui contraignent la solution. Nous formalisons ensuite les cas d'utilisation, dont trois font l'objet d'une description détaillée, et nous énonçons enfin les règles de gestion qui encadrent le comportement du système.

## 2.2 Acteurs du système

Six acteurs interagissent avec l'application. Le tableau 3 en présente les objectifs.

**Tableau 3 - Acteurs du système et leurs objectifs**

| Acteur | Description | Objectifs principaux |
|---|---|---|
| Administrateur | Gère la configuration, les comptes et les droits | Paramétrer, administrer les comptes, superviser |
| Responsable SAV | Pilote le service après-vente | Affecter, suivre, valider, analyser les indicateurs |
| Ingénieur biomédical | Réalise les interventions techniques | Diagnostiquer, réparer, documenter, clôturer |
| Technicien | Assure l'appui terrain | Réaliser les interventions de premier niveau, saisir |
| Client (hôpital) | Personnel hospitalier utilisateur | Déclarer une panne, suivre l'avancement |
| Invité | Accès en lecture limitée | Consulter, assister à une démonstration |

La distinction entre l'ingénieur biomédical et le technicien porte sur la responsabilité de clôture : seul l'ingénieur biomédical peut clôturer définitivement une intervention. Cette distinction se retrouve au niveau des permissions, présentées au chapitre 3.

## 2.3 Besoins fonctionnels

### 2.3.1 Expression des besoins par domaine

Nous regroupons les besoins fonctionnels en treize familles.

**BF-01 — Gestion des utilisateurs et sécurité.** Authentification par identifiant et mot de passe haché, rôles à droits différenciés, gestion des sessions et journal des connexions consignant la date, l'adresse réseau et le résultat de la tentative.

**BF-02 — Gestion du parc de respirateurs.** Fiche appareil comportant le numéro de série, le QR Code, le code interne, le modèle, les versions logicielle et matérielle, le rattachement au bloc opératoire, au service et à l'hôpital, la date de mise en service, le contrat, la garantie et l'état. Chaque appareil possède une fiche de vie retraçant ses interventions, ses pannes, les pièces détachées consommées, les rapports produits et les documents associés.

**BF-03 — Déclaration d'intervention par QR Code.** Le personnel hospitalier accède à une interface simplifiée à partir du QR Code apposé sur l'appareil, et saisit les symptômes constatés parmi une liste prédéfinie, un commentaire, la priorité et l'information déterminante : le patient est-il connecté à l'appareil ?

**BF-04 — Gestion des interventions.** Une intervention porte un numéro de demande, une date, un client, un ingénieur affecté, un respirateur, une description, un diagnostic, une cause, les pièces détachées remplacées, le temps de déplacement, le temps de réparation, le coût de main-d'œuvre, un état et une signature. Son cycle de vie suit le workflow décrit en 2.5.

**BF-05 — Check-list de clôture obligatoire.** La clôture exige la validation de huit contrôles : autotest, test d'étanchéité, calibration du débit, calibration de l'oxygène, batterie, alimentation, alarmes et validation finale.

**BF-06 — Blocage des appareils hors service.** Un respirateur déclaré hors service ne peut plus recevoir de demande d'intervention de type programmé, et se signale visuellement dans le parc.

**BF-07 — Gestion des pièces détachées.** Fiche pièce comportant la référence, le nom, la catégorie, la compatibilité, le stock, le stock minimum, l'emplacement, le prix et le fournisseur. Le système associe les pannes aux pièces détachées probables et déclenche des alertes de stock.

**BF-08 — Documentation technique.** Rattachement à chaque modèle et à chaque appareil des manuels, schémas et procédures au format PDF.

**BF-09 — Rapports au format PDF.** Génération automatique d'un rapport d'intervention comportant les informations du client et de l'appareil, la description de la panne, le diagnostic, les pièces détachées consommées, les temps, le coût et la validation finale.

**BF-10 — Notifications temps réel.** Émission d'une notification lors d'une nouvelle demande, d'une intervention urgente, d'une pièce indisponible, d'un stock faible ou de la fin d'une intervention.

**BF-11 — Tableau de bord et indicateurs.** Restitution du nombre de respirateurs, des interventions ouvertes et critiques, des appareils hors service, de la disponibilité globale, du délai moyen de résolution, de la répartition des pannes et des coûts.

**BF-12 — Affectation automatique.** Choix de l'ingénieur à partir de ses compétences, de sa zone géographique, de sa disponibilité et de sa charge de travail.

**BF-13 — Recherche multicritère.** Recherche sur le numéro de série, le client, la ville, le modèle, la panne, l'ingénieur, l'état et la date.

### 2.3.2 Exigences fonctionnelles traçables

Pour permettre le suivi de la réalisation, nous reformulons ces besoins en vingt exigences identifiées, présentées au tableau 4. Le bilan de leur réalisation figure en 4.6.

**Tableau 4 - Exigences fonctionnelles**

| Identifiant | Exigence | Priorité | Module |
|---|---|---|---|
| EF-01 | Authentification avec hachage du mot de passe | Haute | Sécurité |
| EF-02 | Gestion des rôles et des droits | Haute | Sécurité |
| EF-03 | Historique des connexions | Moyenne | Sécurité |
| EF-04 | Gestion du parc et fiche de vie | Haute | Parc |
| EF-05 | Génération et lecture du QR Code | Haute | Parc |
| EF-06 | Déclaration d'intervention simplifiée | Haute | Interventions |
| EF-07 | Détection du patient connecté et passage en critique | Haute | Interventions |
| EF-08 | Workflow d'intervention complet | Haute | Interventions |
| EF-09 | Tableau Kanban temps réel | Haute | Interventions |
| EF-10 | Check-list de clôture obligatoire | Haute | Interventions |
| EF-11 | Blocage des appareils hors service | Haute | Parc |
| EF-12 | Gestion du stock et alertes | Haute | Pièces |
| EF-13 | Association entre panne et pièces détachées | Moyenne | Pièces |
| EF-14 | Documentation technique intégrée | Moyenne | Documentation |
| EF-15 | Rapports au format PDF | Haute | Rapports |
| EF-16 | Notifications temps réel | Haute | Notifications |
| EF-17 | Tableau de bord et indicateurs | Haute | Tableau de bord |
| EF-18 | Affectation automatique de l'ingénieur | Moyenne | Planning |
| EF-19 | Recherche multicritère | Moyenne | Transverse |
| EF-20 | Thèmes clair et sombre, navigation latérale | Moyenne | Interface |

## 2.4 Besoins non fonctionnels

Le tableau 5 énonce les contraintes de qualité que la solution doit respecter.

**Tableau 5 - Besoins non fonctionnels**

| Catégorie | Exigence |
|---|---|
| Performance | Affichage du tableau de bord en moins de deux secondes ; recherche en moins d'une seconde |
| Sécurité | Hachage des mots de passe, contrôle d'accès par rôle, journal d'audit des connexions |
| Fiabilité | Gestion centralisée des exceptions, transactions cohérentes |
| Disponibilité des données | Base locale et procédure de sauvegarde |
| Ergonomie | Interface de type Fluent, navigation latérale, thèmes clair et sombre |
| Maintenabilité | Architecture en couches, principes SOLID, tests unitaires, documentation du code |
| Traçabilité | Historisation complète et audit des créations et modifications |
| Internationalisation | Interface en français, ressources prévues pour la localisation |

La contrainte de maintenabilité pèse davantage que les autres sur les choix d'architecture. Nous la justifions au chapitre 3.

## 2.5 Workflow d'une intervention

Le cycle de vie d'une intervention comporte dix états. Une demande naît à l'état *Nouvelle*, passe à *Affectée* lorsqu'un ingénieur lui est attribué, puis suit la progression *En déplacement*, *Diagnostic*, *Réparation*, *Test*, *Validation* et *Clôturée*. Deux dérivations complètent ce chemin nominal : l'état *En attente de pièce*, atteint depuis *Réparation* en cas de rupture de stock et qui y ramène une fois la pièce reçue, et l'état *Annulée*, accessible depuis les états initiaux. Le chapitre 3 en donne la représentation formelle.

Chaque changement d'état est horodaté et attribué à son auteur, ce qui constitue la trace demandée par le premier objectif du projet.

## 2.6 Cas d'utilisation

### 2.6.1 Vue générale

La figure 4 présente les cas d'utilisation du système et leur rattachement aux acteurs.

`[Figure 4 - Diagramme de cas d'utilisation général]`
`[SOURCE : docs/05-diagrammes-uml.md §1, à exporter en image]`

Le diagramme fait apparaître une répartition cohérente avec les objectifs des acteurs : le client hôpital déclare et consulte, le responsable SAV affecte et analyse, l'ingénieur biomédical exécute et clôture, l'administrateur paramètre.

### 2.6.2 Relations d'inclusion et d'extension

La figure 5 précise deux relations structurantes.

`[Figure 5 - Cas d'utilisation détaillés : relations « include » et « extend »]`
`[SOURCE : docs/05-diagrammes-uml.md §1 bis, à exporter en image]`

La relation **« include »** matérialise la vérification de permission : tout cas d'utilisation métier inclut le cas « S'authentifier et vérifier la permission ». Ce n'est pas une commodité de notation mais la traduction d'un choix de conception — le contrôle d'accès s'effectue action par action, à l'entrée de chaque service applicatif, et non au seul niveau de l'interface.

La relation **« extend »** décrit trois cas optionnels : la réaffectation à un autre ingénieur étend l'affectation lorsque l'ingénieur initialement retenu devient indisponible ; la recherche étend la consultation de l'historique lorsque l'utilisateur saisit un critère ; l'affichage de la vue globale du parc étend la consultation du tableau de bord lorsque le rôle de l'utilisateur l'y autorise, la vue personnelle s'appliquant sinon.

### 2.6.3 Description détaillée de trois cas d'utilisation

Nous détaillons les trois cas d'utilisation qui portent les objectifs principaux du projet.

**Cas d'utilisation « Déclarer une intervention ».**

- *Acteur principal* : client hôpital. *Acteurs secondaires* : ingénieur biomédical, responsable SAV, destinataires de la notification.
- *Préconditions* : l'utilisateur est authentifié et possède la permission de création ; le respirateur existe et est rattaché à un bloc opératoire, donc à un hôpital.
- *Scénario nominal* : l'utilisateur identifie le respirateur ; il sélectionne les symptômes constatés dans la liste prédéfinie ; il saisit une description et indique si un patient est connecté à l'appareil ; le système attribue un numéro de demande séquentiel par année ; il enregistre l'intervention à l'état *Nouvelle* ; il confirme en affichant le numéro attribué.
- *Scénario alternatif A — patient connecté* : le système force la priorité à *Critique*, déclenche l'affectation automatique d'un ingénieur, place l'intervention à l'état *Affectée* et émet une notification. Si aucun ingénieur n'est disponible, l'intervention reste à l'état *Nouvelle* avec la priorité *Critique*.
- *Scénario alternatif B — respirateur non rattaché* : le système refuse la création et signale que l'appareil n'est rattaché à aucun hôpital.
- *Postcondition* : l'intervention existe, tracée avec son auteur et son état initial.

**Cas d'utilisation « Affecter une intervention ».**

- *Acteur principal* : responsable SAV. L'affectation peut également être déclenchée par le système lors d'une déclaration critique.
- *Préconditions* : l'utilisateur possède la permission d'affectation ; l'intervention n'est ni clôturée ni annulée.
- *Scénario nominal* : le système constitue la liste des ingénieurs candidats ; il écarte ceux qui sont indisponibles ou en congé ; il calcule pour chaque candidat restant un score fondé sur ses compétences sur le modèle concerné, sa zone géographique et sa charge d'interventions ouvertes ; il retient le meilleur score et enregistre l'affectation.
- *Scénario alternatif — aucun candidat disponible* : le système n'affecte personne et laisse l'intervention en attente d'affectation, ce que le tableau de bord signale.
- *Postcondition* : l'intervention porte un ingénieur affecté et son changement d'état est tracé.

**Cas d'utilisation « Clôturer une intervention ».**

- *Acteur principal* : ingénieur biomédical.
- *Préconditions* : l'utilisateur possède la permission de clôture, distincte de la permission de changement d'état ; l'intervention n'est ni clôturée ni annulée.
- *Scénario nominal* : l'ingénieur renseigne le diagnostic, les pièces détachées consommées et les temps ; il valide les huit contrôles de la check-list ; le système vérifie que la check-list est intégralement validée ; il fait passer l'intervention à l'état *Clôturée* ; il génère le rapport au format PDF et l'archive.
- *Scénario alternatif — check-list incomplète* : le système refuse la clôture et signale que la check-list est obligatoire. L'intervention conserve son état.
- *Postcondition* : l'intervention est clôturée, son rapport est archivé, l'appareil peut retourner en service.

## 2.7 Règles de gestion

Le tableau 6 énonce les règles que le système applique sans intervention de l'utilisateur. Elles constituent le cœur métier du projet et font l'objet d'une implémentation dans la couche de domaine ou dans les services applicatifs.

**Tableau 6 - Règles de gestion**

| Identifiant | Règle |
|---|---|
| RG-01 | Une demande déclarée avec un patient connecté prend automatiquement la priorité *Critique*, déclenche une affectation et une notification immédiates |
| RG-02 | Une intervention ne peut passer à l'état *Clôturée* que si toutes les lignes de la check-list sont validées |
| RG-03 | Un respirateur hors service ne peut recevoir de nouvelle demande d'intervention programmée |
| RG-04 | Toute pièce détachée dont le stock atteint le stock minimum génère une alerte ; un stock nul bloque la consommation |
| RG-05 | Le délai moyen de résolution se calcule sur les interventions clôturées |
| RG-06 | Le temps moyen entre pannes se calcule par appareil, comme le rapport du temps de bon fonctionnement cumulé au nombre de pannes |
| RG-07 | Chaque changement d'état d'intervention est horodaté et attribué à son auteur |

La règle RG-01 mérite d'être soulignée : elle est la traduction informatique de la criticité décrite en 1.3. Elle ne dépend d'aucune saisie de priorité par l'utilisateur, précisément parce que la personne qui déclare la panne, dans l'urgence d'un bloc opératoire, ne doit pas avoir à qualifier elle-même l'urgence.

## 2.8 Conclusion

Ce chapitre a identifié six acteurs, formulé treize familles de besoins fonctionnels déclinées en vingt exigences traçables, énoncé huit catégories de besoins non fonctionnels et sept règles de gestion. Il a détaillé les trois cas d'utilisation qui portent les objectifs du projet. Ces éléments constituent la spécification à partir de laquelle nous concevons la solution au chapitre suivant.

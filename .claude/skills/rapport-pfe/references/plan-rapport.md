# Plan du mémoire — squelette conforme au template UM6SS

Le squelette et l'ordre des parties viennent du template officiel et ne se
modifient pas. Seuls les titres et le contenu des quatre chapitres sont propres
au projet GMAO. Budget de pages indicatif : les chapitres doivent surtout rester
**équilibrés entre eux**, la charte l'exige explicitement.

## Pages préparatoires (pagination i, ii, iii…)

**Page de garde** (non numérotée) — logo UM6SS/ESGB et logo MEDICANA · « Cycle
Ingénieur : Génie Biomédical » · « Année universitaire `[À COMPLÉTER]` » ·
« Mémoire de projet de fin d'études » · intitulé du sujet · « Présenté par » +
`[À COMPLÉTER : nom et prénom]` · « Soutenue publiquement le `[À COMPLÉTER]`,
devant le jury composé de : » Président(e), Examinateur, Rapporteur,
Encadrant(e), Invité.

**Dédicaces** — à laisser à l'utilisateur, c'est un texte personnel.

**Remerciements** — 1 page. Encadrant académique, encadrant industriel, équipe
SAV de MEDICANA, jury, corps professoral de l'ESGB. Marquer les noms manquants.

**Résumé** (1 page, 2 maximum) — contexte de la maintenance biomédicale,
problématique, approche suivie, résultats obtenus, puis mots-clés : GMAO,
maintenance corrective, respirateur d'anesthésie, Datex-Ohmeda, génie biomédical.

**Abstract** — traduction fidèle du résumé, mêmes mots-clés.

**ملخص** — traduction arabe du résumé. Obligatoire.

**Table des matières** · **Liste des abréviations** · **Liste des tableaux** ·
**Liste des figures** — toutes paginées et générées automatiquement.

Abréviations à prévoir : UM6SS, ESGB, GMAO, CMMS, SAV, PFE, DI (Demande
d'Intervention), MTTR, SLA, RBAC, MVVM, UML, ORM, API, PDF, QR. **Attention :**
« DI » désigne aussi l'injection de dépendances — la charte interdit les
ambiguïtés de terme, donc réserver « DI » à la Demande d'Intervention et écrire
« injection de dépendances » en toutes lettres.

## Introduction générale (1 à 2 pages, sections non numérotées)

- **Contexte** — la maintenance des respirateurs d'anesthésie en bloc opératoire,
  MEDICANA distributeur Datex-Ohmeda, la gestion actuelle sur fichiers et papier.
- **Objectifs** — ce que le projet vise et ce qu'il exclut.
- **Méthodologie** — démarche itérative en dix phases incrémentales (0 à 9),
  d'après `docs/00-feuille-de-route.md`.
- **Résultats** — annoncés en deux ou trois phrases, sans chiffres ni détail
  (voir le point tranché au §7 de la charte).
- **Structure du mémoire** — annonce des quatre chapitres.

## Chapitre 1 — Contexte général du projet

- `1.1 Introduction`
- `1.2` Organisme d'accueil : MEDICANA, distributeur officiel Datex-Ohmeda
  `[À COMPLÉTER : historique, effectifs, organigramme, implantation]`
- `1.3` Le respirateur d'anesthésie : gammes Aespire, Avance, Aisys, rôle en bloc
  opératoire et criticité pour le patient
- `1.4` Étude de l'existant et critique : absence de traçabilité, d'analyse des
  défaillances et de pilotage de la disponibilité
- `1.5` Problématique et objectifs
- `1.6` Périmètre : maintenance **corrective** uniquement ; exclusions de la v1
  (préventif planifié, facturation, RH, mobile natif)
- `1.7` Conduite du projet : découpage en dix phases, planning (figure de Gantt)
- `1.x Conclusion`

## Chapitre 2 — Analyse et spécification des besoins

- `2.1 Introduction`
- `2.2` Acteurs : Administrateur, Responsable SAV, Ingénieur Biomédical,
  Technicien, Client Hôpital, Invité — tableau des objectifs par acteur
- `2.3` Besoins fonctionnels : les BF-01…, puis le tableau traçable EF-01 → EF-20
  (identifiant, exigence, priorité, module)
- `2.4` Besoins non fonctionnels : architecture, qualité, sécurité, performance
  (tableau de bord en moins de 2 s), portabilité, ergonomie
- `2.5` Diagramme de cas d'utilisation, puis deux ou trois descriptions
  textuelles détaillées (préconditions, scénario nominal, scénarios alternatifs)
  pour *Déclarer une intervention*, *Affecter une intervention*, *Clôturer via
  la check-list*
- `2.6` Règles de gestion : patient connecté ⇒ priorité critique, blocage HORS
  SERVICE, check-list de clôture obligatoire
- `2.x Conclusion`

## Chapitre 3 — Conception

- `3.1 Introduction`
- `3.2` Architecture logicielle : Clean Architecture, règle de dépendance
  `Presentation → Application → Domain`, rôle des sept projets de la solution,
  ports et adaptateurs
- `3.3` Justification des choix : pourquoi Clean Architecture, pourquoi le patron
  Result plutôt que des exceptions pour les échecs attendus, pourquoi Strategy
  pour le moteur d'affectation, pourquoi SQLite embarqué, pourquoi un serveur
  WebSocket natif sous Node.js à la place de SignalR
- `3.4` Patrons de conception appliqués : Repository et Unit of Work, MVVM,
  injection de dépendances, Strategy, suppression logique et audit centralisés
- `3.5` Diagramme de classes du domaine
- `3.6` Modèle de données : les entités par domaine (Sécurité, Parc,
  Interventions, Pièces, Planning, Notifications), MCD, MLD, contraintes
- `3.7` Diagramme de séquence : déclaration d'une intervention critique de bout
  en bout (QR → détection patient connecté → priorité critique → moteur
  d'affectation → notification)
- `3.8` Diagramme d'états de l'intervention
- `3.9` Conception de la sécurité : RBAC par `Permission` et
  `MatricePermissions`, revérification côté base, hachage BCrypt
- `3.x Conclusion`

## Chapitre 4 — Réalisation, tests et déploiement

- `4.1 Introduction`
- `4.2` Environnement et outils : .NET 10, WPF, EF Core et SQLite,
  CommunityToolkit.Mvvm, AutoMapper, FluentValidation, Serilog, iText7, QRCoder,
  LiveCharts2, BCrypt.Net, Node.js avec Express et ws, xUnit, FluentAssertions,
  Moq
- `4.3` Mise en œuvre par module — une capture d'écran et un extrait de code
  significatif chacun : Authentification et utilisateurs · Parc et QR Code ·
  Interventions et workflow · Pièces et alertes de stock · Rapports PDF ·
  Tableau de bord et indicateurs (MTTR, délai d'affectation, SLA, disponibilité,
  Pareto, charge par technicien, coûts) · Notifications temps réel · Moteur
  d'affectation automatique
- `4.4` Tests : stratégie, couverture des tests unitaires (affectation, PDF,
  autorisation, matrice RBAC, indicateurs, validation utilisateur), exemple
  commenté
- `4.5` Déploiement : `build.ps1`, publication win-x64, prérequis .NET 10 Desktop
  Runtime et Node.js 18+, lancement
- `4.6` Bilan par rapport aux exigences : tableau EF-01…EF-20 croisé avec l'état
  réel (réalisé / partiel / non réalisé), **aligné sur la feuille de route**.
  Ne pas déclarer réalisé un point marqué ⬜ : scan QR, carte de chaleur, thèmes
  clair et sombre, notifications par courriel, interface de planning,
  documentation technique.
- `4.x Conclusion`

## Conclusion générale (sections non numérotées)

- **Contributions** — ce que le mémoire apporte.
- **Critique du travail** — limites assumées : périmètre corrective seulement,
  modules partiels, absence de déploiement en conditions réelles, jeu de données
  de démonstration. La charte demande explicitement cette analyse critique.
- **Travaux futurs** — les ⬜ de la feuille de route, la maintenance préventive
  planifiée, l'application mobile de scan, les notifications par courriel.
- **Perspective** — inscription du travail dans un cadre plus large :
  généralisation à d'autres gammes d'équipements biomédicaux, déploiement
  multi-sites, apport au pilotage de la disponibilité du parc hospitalier.

## Bibliographie

Entrées numérotées, triées par auteur, groupées par type. Sources réelles
uniquement : documentation Microsoft .NET, WPF et EF Core ; *Clean Architecture*
de R. C. Martin ; documentation iText7, QRCoder, Serilog ; normes et référentiels
de maintenance biomédicale ; documentation constructeur Datex-Ohmeda et GE
Healthcare ; spécification UML de l'OMG. Privilégier livres et articles aux
documents web ; toute référence web porte sa date de visite.

## Annexes

**Annexe A — Diagrammes UML** (pleine page) · **Annexe B — Schéma complet de la
base de données** · **Annexe C — Extraits de code** (`MoteurAffectation`,
`MatricePermissions`, configuration EF) · **Annexe D — Exemple de rapport PDF
généré** · **Annexe E — Manuel d'installation et manuel utilisateur**.

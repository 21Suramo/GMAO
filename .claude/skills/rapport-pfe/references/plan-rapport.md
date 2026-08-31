# Plan détaillé du rapport

Budget indicatif pour un mémoire de 60-80 pages hors annexes. Ajuster
proportionnellement si l'utilisateur donne un autre volume.

## Liminaires (non paginés en chiffres arabes)

Page de garde · Dédicaces (1 p., laisser à l'utilisateur) · Remerciements (1 p.) ·
Résumé français ~200 mots + 5 mots-clés · Abstract anglais, traduction fidèle du
résumé · Liste des abréviations (GMAO, CMMS, DI, SAV, MTTR, SLA, RBAC, MVVM, DI —
attention, « DI » sert à la fois pour *Demande d'Intervention* et *Dependency
Injection* : désambiguïser) · Liste des figures · Liste des tableaux · Sommaire.

## Introduction générale (2-3 p.)

Contexte de la maintenance biomédicale → problématique (gestion papier :
traçabilité, disponibilité des respirateurs, criticité du patient connecté) →
objectif du PFE → annonce du plan. Aucune référence technique ici.

## Chapitre 1 — Contexte général du projet (10-12 p.)

1.1 Organisme d'accueil : MEDICANA, distributeur officiel Datex-Ohmeda
    `[À COMPLÉTER : historique, effectifs, organigramme, implantation]`
1.2 Domaine : les respirateurs d'anesthésie (Aespire, Avance, Aisys) et leur
    criticité en bloc opératoire
1.3 Étude de l'existant et critique : gestion sur fichiers/papier, absence de
    traçabilité, d'analyse des défaillances et de pilotage de la disponibilité
1.4 Problématique et objectifs
1.5 Périmètre : maintenance **corrective** uniquement ; exclusions v1 (préventif
    planifié, facturation, RH, mobile natif) — source `docs/02-cahier-des-charges.md` §2
1.6 Conduite du projet : découpage en 10 phases incrémentales (0 à 9), démarche
    itérative — source `docs/00-feuille-de-route.md`. Inclure un Gantt.

## Chapitre 2 — Analyse et spécification des besoins (12-15 p.)

2.1 Acteurs : Administrateur, Responsable SAV, Ingénieur Biomédical, Technicien,
    Client Hôpital, Invité (tableau des objectifs par acteur)
2.2 Besoins fonctionnels : reprendre les BF-01…, puis le tableau traçable
    EF-01 → EF-20 (ID, exigence, priorité, module)
2.3 Besoins non fonctionnels : architecture, qualité, sécurité, performance
    (< 2 s dashboard), portabilité, ergonomie
2.4 Diagramme de cas d'utilisation général + 2 ou 3 descriptions textuelles
    détaillées (scénario nominal, alternatifs, préconditions) pour les cas
    structurants : *Déclarer une intervention*, *Affecter une intervention*,
    *Clôturer via check-list*
2.5 Règles de gestion. Au minimum RG « patient connecté ⇒ priorité critique »,
    blocage HORS SERVICE, check-list de clôture obligatoire.

## Chapitre 3 — Conception (15-18 p.)

3.1 Architecture logicielle : Clean Architecture, règle de dépendance
    `Presentation → Application → Domain`, rôle de chaque projet des 7 de la
    solution, ports/adaptateurs (Persistence et Infrastructure implémentent les
    interfaces de Application)
3.2 Justification des choix : pourquoi Clean Architecture pour un PFE maintenable,
    pourquoi le pattern Result plutôt que des exceptions pour les échecs attendus,
    pourquoi Strategy pour le moteur d'affectation (testabilité, pureté),
    pourquoi SQLite embarqué, pourquoi un serveur WebSocket Node.js natif à la
    place de SignalR (auto-hébergement sous Node)
3.3 Patrons appliqués : Repository + Unit of Work, MVVM, Injection de dépendances,
    Strategy, soft-delete et audit centralisés dans `SaveChangesAsync`
3.4 Diagramme de classes du domaine
3.5 Modèle de données : les ~25 entités par domaine (Sécurité, Parc, Interventions,
    Pièces, Planning, Notifications), MCD/MLD, contraintes d'intégrité
3.6 Diagrammes de séquence : déclaration d'une DI critique de bout en bout
    (QR → détection patient connecté → priorité critique → moteur d'affectation →
    notification WebSocket)
3.7 Diagramme d'états de l'intervention (workflow)
3.8 Conception de la sécurité : RBAC par `Permission` + `MatricePermissions`,
    revérification côté base, BCrypt workFactor 12

## Chapitre 4 — Réalisation, tests et déploiement (15-18 p.)

4.1 Environnement et outils : .NET 10, WPF, EF Core + SQLite, CommunityToolkit.Mvvm,
    AutoMapper 13.0.1, FluentValidation, Serilog, iText7 (+ bouncy-castle-adapter),
    QRCoder, LiveCharts2, BCrypt.Net, Node.js/Express/ws, xUnit/FluentAssertions/Moq
4.2 Mise en œuvre par module, avec capture d'écran et extrait de code significatif
    chacun : Authentification & Utilisateurs · Parc & QR Code · Interventions &
    workflow · Pièces & alertes stock · Rapports PDF · Dashboard KPI (MTTR, délai
    d'affectation, SLA, disponibilité, Pareto, charge/technicien, coûts) ·
    Notifications temps réel · Moteur d'affectation automatique
4.3 Tests : stratégie, les 37 tests unitaires et ce qu'ils couvrent (affectation,
    PDF, autorisation, matrice RBAC, KPI, validation utilisateur), exemple de test
4.4 Déploiement : `build.ps1`, `dotnet publish` win-x64, prérequis .NET 10 Desktop
    Runtime + Node 18+, lancement via `Lancer-GMAO.cmd` — source `docs/06-guide-deploiement.md`
4.5 Bilan par rapport aux exigences : tableau EF-01…EF-20 × état réel (réalisé /
    partiel / non réalisé), **aligné sur la feuille de route**. Ne pas cocher
    « réalisé » un point marqué ⬜ (scan QR, heatmap, thèmes Light/Dark,
    notifications e-mail, UI de planning, documentation technique).

## Conclusion générale et perspectives (2-3 p.)

Rappel de la problématique, apports du travail, bilan technique et personnel
(compétences acquises), puis perspectives — les ⬜ de la feuille de route
constituent des perspectives honnêtes et crédibles, ainsi que : maintenance
préventive planifiée, application mobile de scan, notifications e-mail,
déploiement multi-sites.

## Bibliographie et webographie

Style IEEE ou APA, cohérent d'un bout à l'autre. Sources réelles uniquement :
documentation Microsoft .NET/WPF/EF Core, Clean Architecture (R. C. Martin),
documentation iText7/QRCoder/Serilog, normes de maintenance biomédicale,
documentation constructeur Datex-Ohmeda/GE Healthcare. **Ne jamais fabriquer de
référence** : citer une source signifie l'avoir consultée.

## Annexes

Diagrammes UML pleine page · schéma complet de la base (27 tables) · extraits de
code (`MoteurAffectation`, `MatricePermissions`, configuration EF) · exemple de
rapport PDF généré · guide d'installation condensé · manuel utilisateur.

# Cahier des charges — GMAO Datex-Ohmeda

## 1. Présentation du projet

Développement d'une application **desktop Windows** de GMAO pour la maintenance corrective des respirateurs d'anesthésie Datex-Ohmeda, au profit du SAV de **MEDICANA**.

## 2. Périmètre

**Inclus** : maintenance corrective, parc respirateurs, interventions, pièces détachées, KPI/BI, notifications, rapports PDF, documentation technique, gestion utilisateurs.

**Exclus (v1)** : maintenance préventive planifiée, facturation comptable, gestion RH complète, mobile natif (le scan QR se fait via interface desktop simplifiée).

## 3. Exigences fonctionnelles (synthèse traçable)

| ID | Exigence | Priorité | Module |
|---|---|---|---|
| EF-01 | Authentification + hachage BCrypt | Haute | Sécurité |
| EF-02 | Gestion des rôles & droits (RBAC) | Haute | Sécurité |
| EF-03 | Historique des connexions | Moyenne | Sécurité |
| EF-04 | CRUD parc respirateurs + fiche de vie | Haute | Parc |
| EF-05 | Génération + scan QR Code | Haute | Parc/DI |
| EF-06 | Déclaration d'intervention simplifiée | Haute | Interventions |
| EF-07 | Détection « patient connecté » → critique | Haute | Interventions |
| EF-08 | Workflow d'intervention complet | Haute | Interventions |
| EF-09 | Kanban temps réel drag & drop | Haute | Interventions |
| EF-10 | Check-list de clôture obligatoire | Haute | Interventions |
| EF-11 | Blocage HORS SERVICE | Haute | Parc |
| EF-12 | Gestion stock pièces + alertes intelligentes | Haute | Pièces |
| EF-13 | Association panne ↔ pièces | Moyenne | Pièces/Stats |
| EF-14 | Documentation technique (PDF intégrés) | Moyenne | Documentation |
| EF-15 | Rapports PDF iText7 | Haute | Rapports |
| EF-16 | Notifications temps réel (desktop + email) | Haute | Notifications |
| EF-17 | Dashboard + KPI/BI | Haute | Dashboard |
| EF-18 | Affectation automatique ingénieur | Moyenne | Planning |
| EF-19 | Recherche multicritère | Moyenne | Transverse |
| EF-20 | Thèmes Dark/Light, navigation latérale | Moyenne | UI |

## 4. Exigences non fonctionnelles

- **Architecture** : Clean Architecture, SOLID, MVVM, Repository + Unit of Work, DI.
- **Qualité** : tests unitaires, documentation XML, journalisation Serilog, gestion d'exceptions.
- **Sécurité** : BCrypt, RBAC, audit connexions, validation des entrées (FluentValidation).
- **Performance** : réponses UI fluides (< 2 s pour dashboard).
- **Portabilité données** : SQLite embarqué.
- **Ergonomie** : Fluent/Material, FontAwesome, animations, responsive desktop.

## 5. Contraintes techniques

- Plateforme : **Windows 10/11**, .NET 10, WPF.
- Base : **SQLite** via EF Core (Code First + migrations).
- Notifications : **Node.js + SignalR** (process séparé).
- PDF : **iText7**. QR : **QRCoder**. Graphiques : **LiveCharts2**.

## 6. Livrables

Conformes à la liste du PFE : analyse fonctionnelle, cahier des charges, UML (cas d'usage, classes, séquence), modèle de données, architecture, code des projets .NET, BD EF Core, entités, repositories, services, API Node.js, UI WPF, dashboard, QR, interventions, pièces, PDF, notifications, KPI, statistiques, tests, déploiement, documentation.

## 7. Planning indicatif

Voir [00-feuille-de-route.md](00-feuille-de-route.md). Découpage en 10 phases (0 à 9), chaque phase étant un incrément livrable et démontrable.

## 8. Critères d'acceptation (exemples)

- **CA-01** : Une DI avec patient connecté crée une intervention *Critique* affectée automatiquement, avec notification reçue par le Responsable SAV.
- **CA-02** : Impossible de clôturer une intervention si une case de check-list est non cochée (message bloquant).
- **CA-03** : Un respirateur HORS SERVICE n'apparaît pas comme programmable et affiche un badge rouge.
- **CA-04** : Le rapport PDF généré contient logo, QR Code, pièces, coûts et signature.
- **CA-05** : Le dashboard affiche MTTR, MTBF et disponibilité calculés à partir des données réelles.

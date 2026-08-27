# Feuille de route — GMAO Datex-Ohmeda

Découpage du projet en phases livrables. Chaque phase est validable indépendamment.

| Phase | Intitulé | Livrables | État |
|---|---|---|---|
| **0** | Fondations & Documentation | Analyse fonctionnelle, cahier des charges, UML, modèle de données, architecture | 🟢 Terminé |
| **1** | Socle technique | ✅ .NET 10 · ✅ solution 7 projets (Clean Archi) · ✅ paquets NuGet · ✅ 25 entités + logique métier · ✅ AppDbContext (audit + soft-delete) · ✅ configurations EF · ✅ Repository + UnitOfWork · ✅ migration `InitialCreate` appliquée (gmao.db, 27 tables) · ✅ seeder | 🟢 Terminé |
| **2** | Backend métier | ✅ Repository/UnitOfWork · ✅ DTO · ✅ AutoMapper (profil) · ✅ FluentValidation · ✅ Serilog (console + fichier) · ✅ services (Auth, TableauBord) | 🟢 Terminé |
| **3** | Sécurité & Utilisateurs | ✅ authentification · ✅ hachage BCrypt (workFactor 12) · ✅ `ICurrentUserService` · ✅ **autorisation action par action** (`Permission` + `MatricePermissions` + `IAutorisationService`, revérification base/compte actif) · ✅ **CRUD utilisateurs** (admin : création, modification, activation/désactivation, soft-delete, réinit. mot de passe, recherche/filtre) · ✅ audit auteur (`CreePar`/`ModifiePar`) · ✅ historique de connexion (succès/échec) | 🟢 Terminé |
| **4** | Interface WPF | ✅ connexion · ✅ shell + nav latérale **filtrée par rôle** · ✅ MVVM · ✅ **Dashboard refondu** (KPI SQL : MTTR, délai d'affectation, SLA, dispo/équipement, Pareto, charge/technicien, coûts · période sélectionnable · widgets par rôle vue globale/personnelle) · ✅ **Paramètres** (profil, changement mot de passe, historique connexions, à propos) · ✅ **Utilisateurs** (écran d'administration) · ⬜ thèmes Light/Dark | 🟡 En cours |
| **5** | Modules métier UI | ✅ **Parc** · ✅ **Interventions** · ✅ **Workflow** · ❌ **Kanban retiré** (workflow d'états conservé) · ✅ **Pièces** · ✅ **Rapports** (liste/génération/ouverture PDF) · ⬜ Documentation technique | 🟡 En cours |
| **6** | Fonctions avancées | ✅ **QR Code** · ✅ **Rapports PDF iText7** · ✅ check-list RG-02 · ✅ blocage HS · ✅ **KPI/BI LiveCharts2** · ⬜ scan QR · ⬜ heatmap | 🟡 En cours |
| **7** | Notifications temps réel | ✅ **serveur Node.js** (Express + WebSocket `ws`, REST `/notify`, historique) · ✅ **client WPF** (`ClientWebSocket`) · ✅ cloche + panneau notifications · ✅ déclenchement sur DI critique · ⬜ e-mail | 🟡 En cours |
| **8** | Affectation automatique | ✅ **moteur d'affectation** (Strategy : compétences + zone + charge + dispo/congés) · ✅ compétences & zones seedées · ✅ branché sur DI critique · ✅ **5 tests unitaires** · ⬜ UI de planning | 🟡 En cours |
| **9** | Qualité & Livraison | 🟡 tests unitaires (**37**, xUnit/FluentAssertions/Moq : affectation, PDF, autorisation, matrice RBAC, KPI, validation utilisateur) · ✅ documentation XML · ✅ gestion exceptions (filet global) · ✅ **packaging** (`dotnet publish` validé) + `build.ps1` + [guide déploiement](06-guide-deploiement.md) | 🟡 En cours |

## Principes transverses (appliqués dès la phase 1)

- **Clean Architecture** + **SOLID** + **Repository / Unit of Work**.
- **MVVM** strict côté WPF (aucune logique métier dans le code-behind).
- **Injection de dépendances** partout.
- **Validation** systématique (FluentValidation) aux frontières applicatives.
- **Journalisation** (Serilog) et **gestion centralisée des exceptions**.
- **Documentation XML** sur les API publiques.

## Légende

🟢 Terminé · 🟡 En cours · ⬜ À venir

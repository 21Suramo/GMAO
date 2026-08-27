# Context Save — GMAO Datex-Ohmeda (PFE MEDICANA)

> Reprise automatique : si ce fichier existe, le lire et continuer. Le projet est généré **par phases**.

## Tâche courante
Développement d'une appli desktop **GMAO .NET 10 / WPF (MVVM) / Clean Architecture** pour la maintenance corrective des respirateurs Datex-Ohmeda. Généré progressivement. **En attente du choix utilisateur** pour la suite (voir « Prochaine étape »).

## Avancement (feuille de route : `docs/00-feuille-de-route.md`)
- **Phase 0 — Docs** : 🟢 terminé (`docs/01..05`, README).
- **Phase 1 — Socle** : 🟢 terminé. 7 projets (`GMAO.slnx`), 25 entités Domain, `AppDbContext` (audit + soft-delete global), configs EF, Repository + UnitOfWork, migration `InitialCreate` appliquée, seeder.
- **Phase 2 — Backend** : 🟢 terminé. DTO, AutoMapper (figé **13.0.1** — v15+ payant), FluentValidation, Serilog (console+fichier `logs/`), services `AuthenticationService` & `TableauBordService`.
- **Phase 3 — Sécurité** : 🟢 terminé. Auth BCrypt (workFactor 12), `IPasswordHasher`/`BCryptPasswordHasher`, `ICurrentUserService` + RBAC `ADroit`, historique connexions.
- **Phase 4 — UI WPF** : 🟡 en cours. Fait : écran connexion, shell + nav latérale, Dashboard (KPI réels EF), navigation MVVM (DataTemplates VM→View), modules placeholder. **Reste** : thèmes Dark/Light, graphiques LiveCharts2.
- **Phase 5 — Modules métier** : 🟡. Fait : **Parc** (QR, blocage HS) ; **Interventions** (RG-01 patient→critique, workflow+historique, check-list RG-02) ; **Kanban** drag&drop ; **Pièces** (`IPieceService` : stock, mouvements Entrée/Sortie/Ajustement, association panne↔pièce `SeedAssociationsPannePieceAsync`). Reste : Documentation technique.
- **Phase 6** : ✅ **Rapports PDF iText7** (`IRapportService`+`RapportPdfGenerateur`). ⚠️ iText7 v9 exige le paquet **`itext7.bouncy-castle-adapter`** (sinon `PdfWriter` crash) — ajouté. ✅ **KPI/BI LiveCharts2** (Dashboard : camembert états + histogramme modèles). Reste : scan QR, heatmap.
- **Robustesse** : `App.xaml.cs` a un `DispatcherUnhandledException` (filet anti-crash global). Test unitaire PDF OK (`tests/.../RapportPdfGenerateurTests`).
- **Phase 9 — Livraison** : ✅ packaging validé (`dotnet publish -c Release -r win-x64 --self-contained false -o publish/GMAO-app`, exe démarre). Script `build.ps1` (restore+build+test+publish+npm). Guide `docs/06-guide-deploiement.md`. `publish/` dans .gitignore. 6 tests verts. Reste : module Documentation technique, e-mail, thème Dark/Light, UI planning.
- **Phase 8 — Affectation auto** : ✅ `MoteurAffectation` (pur, Strategy : +50 compétence, +30 zone, -5/intervention ouverte ; exclut indispo/congés) + `AffectationService` (charge candidats via projections). Branché dans `InterventionService.CreerAsync` (DI patient connecté). Compétences seedées (`SeedCompetencesAsync`). **6 tests unitaires** OK (`MoteurAffectationTests` + PDF). Reste : UI planning/congés.
- **Phase 7 — Notifications** : ✅ **serveur Node.js** `servers/notification-server` (Express + `ws` WebSocket, port **4000**, REST `POST /notify`, `GET /health`, `/notifications`). Démarrer : `cd servers/notification-server ; node server.js`. Client WPF `NotificationTempsReelClient` (`ClientWebSocket` ws://localhost:4000/ws + HTTP POST). Cloche + panneau dans le shell (`ShellViewModel.Notifications`). DI critique → `EnvoyerAsync`. ⚠️ SignalR remplacé par WebSocket natif (SignalR n'est pas hébergeable simplement sous Node.js) — choix documenté. Reste Phase 7 : e-mail. **Le serveur Node doit tourner** pour les notifs (sinon l'app fonctionne quand même, connexion ignorée).
- **Phase 6 — Avancé** : QR Code génération ✅ (QRCoder `IQrCodeService`/`QrCodeService`, PngByteQRCode). Reste : scan QR, PDF iText7, LiveCharts2, check-list.

## État exécutable
App **fonctionne** : `admin` / `Admin@123` → shell. Dashboard KPI réels (4 respirateurs : 2 en service, 1 maintenance, 1 hors service ; dispo 50%). Module **Parc** : sélectionner un respirateur → fiche + QR + bouton hors service.
Seeder ajoute 4 respirateurs (`SeedRespirateursAsync`). Pattern lecture : `IRepository.ListerAsync(filtre, projection)` (projection EF→DTO, garde Application sans EF). Services scoped résolus via `IServiceScopeFactory` dans les VMs.

## Modules Rapports & Paramètres (faits)
- **Rapports** (`RapportsViewModel`/`RapportsView`) : liste interventions + état PDF (`IRapportService.ListerAsync`), boutons Générer/Régénérer + Ouvrir. `RapportDto`.
- **Paramètres** (`ParametresViewModel`/`ParametresView`) : profil (depuis `ICurrentUserService`), **changement mot de passe** (`IUtilisateurService.ChangerMotDePasseAsync`, vérifie ancien via BCrypt), **historique connexions** (`HistoriqueConnexionsAsync`), à propos. PasswordBox synchronisés en code-behind.
- Plus aucun module placeholder (`PlaceholderViewModels.cs` supprimé ; `EnConstructionView` inutilisé).

## Lanceur & correctifs récents
- **`Lancer-GMAO.cmd`** (racine) : démarre serveur Node + app. Raccourcis `.lnk` (dossier + Bureau). Dans la batch, utiliser `ping -n 3 127.0.0.1 >nul` au lieu de `timeout` (qui échoue si stdin redirigé).
- **CRASH CORRIGÉ** : `<Run Text="{Binding ...}">` se lie en **TwoWay par défaut** → plante sur propriété lecture seule (`HistoriqueEtatDto.Libelle`). Fix : `Mode=OneWay`. Voir mémoire [[wpf-run-text-twoway-piege]].

## Environnement (IMPORTANT)
- `dotnet` **hors PATH** : préfixer chaque commande PS par `$env:PATH = "C:\Program Files\dotnet;$env:PATH"`. Exe : `C:\Program Files\dotnet\dotnet.exe` (SDK 10.0.301).
- Solution au format **`.slnx`** : `dotnet build GMAO.slnx`.
- **Smart App Control désactivé** (était bloquant `0x800711C7` ; irréversible). Node v24 présent.
- `dotnet-ef` 10 = outil local (`dotnet-tools.json`). Migrations : `--project src/GMAO.Persistence --startup-project src/GMAO.Presentation.Wpf` (le projet WPF a le package EF Design).
- **Fermer l'app avant rebuild** (verrou DLL) : `Stop-Process -Name "GMAO.Presentation.Wpf" -Force`.
- Harnais **mange les glyphes Unicode** dans Write/Edit → passer les icônes en **code hex int** (`0xE80F`) converti via `char.ConvertFromUtf32` (voir `NavItem`, `PlaceholderViewModels`).

## Structure projets
`src/GMAO.{Domain,Application,Persistence,Infrastructure,Shared,Presentation.Wpf}` + `tests/GMAO.Tests.Unit`. Entités par module : `Domain/Entities/{Securite,Parc,Interventions,Pieces,Planning,Notifications}`. UI : `Presentation.Wpf/{ViewModels,Views,Converters}`, démarrage `App.xaml.cs` (Generic Host + Serilog).

## Lancer
```
$env:PATH = "C:\Program Files\dotnet;$env:PATH"
cd C:\Users\pc\Desktop\GMAO
dotnet build GMAO.slnx -c Debug
Start-Process "src\GMAO.Presentation.Wpf\bin\Debug\net10.0-windows\GMAO.Presentation.Wpf.exe"
```

## Prochaine étape — choix proposé à l'utilisateur (non encore tranché)
- **A)** Thème Dark/Light commutable + finition visuelle du shell.
- **B)** Module **Parc** : liste respirateurs, fiche de vie, génération **QR Code** (QRCoder déjà référencé en Infrastructure).
- **C)** Module **Interventions** : workflow + **Kanban** drag & drop (règle « patient connecté → critique »).

## Prompt de reprise
« Reprends le PFE GMAO d'après `context-save.md`. J'avais le choix entre A (thème Dark/Light), B (module Parc + QR Code), C (interventions + Kanban). Mon choix : [A/B/C]. Continue. »

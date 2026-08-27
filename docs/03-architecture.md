# Architecture logicielle — GMAO Datex-Ohmeda

## 1. Vue d'ensemble (Clean Architecture)

```
┌─────────────────────────────────────────────────────────────┐
│                    GMAO.Presentation.Wpf                     │  ← WPF / MVVM
│   Views (XAML) · ViewModels · Thèmes · Converters · DI host  │
└───────────────────────────┬─────────────────────────────────┘
                            │ dépend de
┌───────────────────────────▼─────────────────────────────────┐
│                       GMAO.Application                       │  ← Use-cases
│  Services · DTO · Interfaces (I…Repository, IPdf, INotif…)   │
│  Validators (FluentValidation) · Profiles (AutoMapper)       │
└───────────┬─────────────────────────────────┬───────────────┘
            │ dépend de                        │ implémenté par
┌───────────▼───────────┐         ┌────────────▼───────────────┐
│      GMAO.Domain      │         │   GMAO.Persistence          │
│ Entités · Enums       │◄────────┤  DbContext · Configs        │
│ Règles métier         │         │  Repositories · UnitOfWork  │
│ Interfaces domaine    │         │  Migrations (SQLite)        │
└───────────────────────┘         └─────────────────────────────┘
            ▲                      ┌─────────────────────────────┐
            │                      │   GMAO.Infrastructure       │
            └──────────────────────┤  PDF (iText7) · QR (QRCoder)│
                                   │  Notifications (SignalR cli)│
            ┌──────────────────────┤  Email · Fichiers · Hashing │
            │                      └─────────────────────────────┘
┌───────────▼───────────┐
│      GMAO.Shared      │  ← Result<T>, constantes, exceptions, helpers (référencé partout)
└───────────────────────┘
```

### Règle de dépendance
- **Domain** ne dépend de **rien** (cœur métier pur).
- **Application** dépend de **Domain** (+ Shared). Définit les **interfaces** d'infrastructure (ports).
- **Persistence** et **Infrastructure** dépendent de **Application/Domain** et **implémentent** les ports (adapters).
- **Presentation** dépend de **Application** ; compose les implémentations via **DI** au démarrage.

## 2. Responsabilité des projets

| Projet | Type | Responsabilité |
|---|---|---|
| `GMAO.Domain` | classlib | Entités, value objects, enums, exceptions métier, interfaces de domaine. Aucune dépendance externe. |
| `GMAO.Application` | classlib | Orchestration des cas d'usage, services métier, DTO, validation, mapping, **interfaces** de repositories et de services d'infra. |
| `GMAO.Persistence` | classlib | EF Core `AppDbContext`, `IEntityTypeConfiguration`, repositories génériques + spécifiques, `UnitOfWork`, migrations SQLite, seed. |
| `GMAO.Infrastructure` | classlib | Adapters techniques : génération PDF (iText7), QR (QRCoder), client SignalR, email (SMTP), stockage fichiers, hachage (BCrypt). |
| `GMAO.Shared` | classlib | `Result`/`Result<T>`, `PagedResult<T>`, constantes, helpers, exceptions transverses. |
| `GMAO.Presentation.Wpf` | WPF (net10.0-windows) | Shell, navigation, Views/ViewModels MVVM, thèmes, converters, hôte DI. |
| `notification-server` | Node.js | Hub SignalR, push desktop, relais email. |
| `GMAO.Tests.Unit` | xUnit | Tests unitaires des services/règles métier. |

## 3. Patterns appliqués

- **MVVM** : `CommunityToolkit.Mvvm` (`ObservableObject`, `[ObservableProperty]`, `[RelayCommand]`). Aucune logique métier dans le code-behind.
- **Repository + Unit of Work** : abstraction de la persistance, transactions cohérentes.
- **Dependency Injection** : `Microsoft.Extensions.DependencyInjection`, composition racine dans `App.xaml.cs`.
- **Result Pattern** : retours `Result<T>` pour la gestion d'erreurs sans exceptions de contrôle de flux.
- **Specification / Query objects** pour la recherche multicritère.
- **Mediator-léger** optionnel via services applicatifs (pas de sur-ingénierie).
- **Strategy** pour le moteur d'affectation automatique.

## 4. Flux type — Déclaration d'intervention critique

```
[Vue DI WPF] --(scan QR)--> DeclarationViewModel
   → IInterventionService.CreerDepuisDeclarationAsync(dto)
      → Validation (FluentValidation)
      → Domain : si PatientConnecte ⇒ Priorité=Critique, État=Affectée
      → IAffectationService.ChoisirIngenieur(...)        (Strategy)
      → IUnitOfWork.Commit()
      → INotificationService.Push("Intervention critique")  → SignalR → Node.js → desktop+email
   ← Result<InterventionDto>
```

## 5. Journalisation & exceptions

- **Serilog** : sinks Console + Fichier (`logs/gmao-.log` rolling). Enrichers (machine, utilisateur).
- **Middleware d'exceptions** applicatif : conversion des exceptions techniques en `Result` + log.
- **Audit** : table `HistoriqueConnexion` + horodatage des changements d'état.

## 6. Composition racine (extrait conceptuel)

```csharp
services
  .AddPersistence(config)        // DbContext SQLite, repos, UnitOfWork
  .AddApplication()              // services, validators, automapper
  .AddInfrastructure(config)     // pdf, qr, email, notifications, hashing
  .AddWpfViewModels();           // VMs et navigation
```

## 7. Stratégie de tests

- **Domain** : tests unitaires des règles (workflow, criticité, check-list).
- **Application** : services avec repositories mockés (Moq).
- **Persistence** : tests d'intégration sur SQLite in-memory.

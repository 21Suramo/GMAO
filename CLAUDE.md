# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

GMAO Datex-Ohmeda — a Windows desktop application (WPF) for corrective-maintenance management (GMAO/CMMS) of Datex-Ohmeda anesthesia respirators (Aespire, Avance, Aisys…), built as a PFE (final-year project) for MEDICANA. Comments, identifiers, docs and UI strings are in **French** — keep new code consistent with that (e.g. `Respirateur`, `Intervention`, `Panne`, `Piece`, `Utilisateur`).

## Environment quirks (Windows)

- **`dotnet` is not on PATH** on this machine. Prefix every PowerShell command with:
  ```powershell
  $env:PATH = "C:\Program Files\dotnet;$env:PATH"
  ```
- The solution file is `GMAO.slnx` (new `.slnx` format, not `.sln`) — pass it directly to `dotnet build`/`dotnet restore`.
- Close the running app before rebuilding (locks `GMAO.Presentation.Wpf.exe`/DLLs):
  ```powershell
  Stop-Process -Name "GMAO.Presentation.Wpf" -Force
  ```
- Avoid Unicode glyph literals in source edited through this tool (they get mangled) — icons are stored as hex codepoints (e.g. `0xE80F`) and converted via `char.ConvertFromUtf32` (see `NavItem`).

## Commands

```powershell
$env:PATH = "C:\Program Files\dotnet;$env:PATH"

# Build
dotnet build GMAO.slnx -c Debug

# Run the WPF app
dotnet run --project src/GMAO.Presentation.Wpf

# Run all unit tests
dotnet test tests/GMAO.Tests.Unit

# Run a single test (by fully-qualified name or filter)
dotnet test tests/GMAO.Tests.Unit --filter "FullyQualifiedName~MoteurAffectationTests"

# EF Core migrations (dotnet-ef is a local tool, see dotnet-tools.json)
dotnet tool restore
dotnet ef migrations add <Nom> --project src/GMAO.Persistence --startup-project src/GMAO.Presentation.Wpf
dotnet ef database update --project src/GMAO.Persistence --startup-project src/GMAO.Presentation.Wpf

# Full pipeline: restore + build (Release) + test + publish (win-x64) + npm install
./build.ps1

# Real-time notification server (Node.js, port 4000)
cd servers/notification-server
npm install
node server.js

# One-click launch (starts notification server then the app)
./Lancer-GMAO.cmd
```

Default login: `admin` / `Admin@123` (seeded by `DbSeeder`).

## Architecture (Clean Architecture)

Dependency rule: `Presentation → Application → Domain`; `Persistence` and `Infrastructure` also depend on `Application/Domain` and implement its interfaces (ports/adapters). **Domain depends on nothing.**

- **`src/GMAO.Domain`** — entities (`Entities/{Securite,Parc,Interventions,Pieces,Planning,Notifications}`), enums, `EntiteBase` (audit fields + soft-delete flag), domain-only interfaces. No external dependencies.
- **`src/GMAO.Application`** — use-case services (`Services/*Service.cs`), DTOs, FluentValidation validators, AutoMapper profiles, and the **interfaces** that Persistence/Infrastructure implement (`Common/Interfaces`). This is where business rules like "connected patient ⇒ critical priority" live.
- **`src/GMAO.Persistence`** — `AppDbContext` (EF Core + SQLite), `IEntityTypeConfiguration`s, generic `Repository<T>` + `UnitOfWork`, migrations, `DbSeeder`.
- **`src/GMAO.Infrastructure`** — adapters: PDF generation (iText7, `RapportPdfGenerateur`), QR codes (QRCoder, `QrCodeService`), the WebSocket notification client (`NotificationTempsReelClient`), password hashing (BCrypt).
- **`src/GMAO.Shared`** — `Result`/`Result<T>`, cross-cutting helpers, referenced everywhere.
- **`src/GMAO.Presentation.Wpf`** — WPF/MVVM app (CommunityToolkit.Mvvm). `App.xaml.cs` is the composition root (Generic Host + Serilog); `Views/` + `ViewModels/` pairs per module, `Converters/`.
- **`servers/notification-server`** — standalone Node.js (Express + `ws`) server, not part of the .NET solution. REST `POST /notify`, `GET /health`, `GET /notifications`, WebSocket at `ws://localhost:4000/ws`. **SignalR was replaced by a native WebSocket server** because SignalR isn't easily self-hosted under Node.js — the app still works if this server isn't running (notifications are just silently skipped).
- **`tests/GMAO.Tests.Unit`** — xUnit + FluentAssertions + Moq.

### Key patterns to follow

- **Read pattern**: `IRepository<T>.ListerAsync(filtre, projection)` runs an EF `Where`+`Select` against an `Expression<Func<T, TResultat>>` projection straight to a DTO, so `GMAO.Application` never needs an EF Core reference. Prefer this over loading entities and mapping in memory.
- **Services in ViewModels**: scoped Application services are resolved per-operation via `IServiceScopeFactory` inside ViewModels (not injected directly as singletons), since `AppDbContext` is scoped.
- **Soft delete + audit**: everything inheriting `EntiteBase` gets a global EF query filter on `EstSupprime` and automatic `DateCreation`/`DateModification` stamping in `AppDbContext.SaveChanges(Async)` — don't hand-roll either.
- **Result pattern**: application services return `Result`/`Result<T>` rather than throwing for expected failure cases.
- **Affectation engine**: `MoteurAffectation` (`src/GMAO.Application/Services/Affectation`) is a pure Strategy-pattern scorer (competency/zone/workload weighting, excludes unavailable/on-leave staff), invoked by `AffectationService` and wired into `InterventionService.CreerAsync`. Keep it side-effect-free and unit-testable (see `MoteurAffectationTests`).
- **MVVM discipline**: no business logic in view code-behind; navigation is DataTemplate-based VM→View resolution driven by `ShellViewModel`.
- **WPF binding gotcha**: `<Run Text="{Binding ...}">` binds `TwoWay` by default and crashes against read-only properties — always set `Mode=OneWay` explicitly on `Run.Text` bindings.

### Notable fixed library choices

- AutoMapper pinned to **13.0.1** (v15+ requires a paid license).
- iText7 v9 requires the `itext7.bouncy-castle-adapter` package alongside `itext.kernel`/`itext.pdfa`, or `PdfWriter` crashes.
- BCrypt work factor 12 for password hashing (`BCryptPasswordHasher`).

## Documentation

Deeper design docs live under `docs/`: `00-feuille-de-route.md` (roadmap/progress), `01-analyse-fonctionnelle.md`, `02-cahier-des-charges.md`, `03-architecture.md`, `04-modele-de-donnees.md`, `05-diagrammes-uml.md`, `06-guide-deploiement.md`. Check `docs/00-feuille-de-route.md` before assuming a module is unfinished or missing.

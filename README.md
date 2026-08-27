# GMAO Datex-Ohmeda — MEDICANA

> Application desktop professionnelle de **Gestion de Maintenance Assistée par Ordinateur (GMAO)**, dédiée à la gestion des interventions de **maintenance corrective** des respirateurs d'anesthésie **Datex-Ohmeda** (Aespire, Avance, Aisys…).

Projet de Fin d'Études (PFE) réalisé au sein de **MEDICANA**, distributeur officiel Datex-Ohmeda.

---

## 🎯 Objectifs

- Améliorer la gestion du **Service Après-Vente (SAV)**.
- Assurer la **traçabilité** complète des interventions biomédicales.
- Optimiser la **disponibilité** des respirateurs d'anesthésie.
- **Analyser les défaillances** pour améliorer la fiabilité (MTBF, MTTR, indice de fiabilité).

## 🧱 Stack technique imposée

| Domaine | Technologie |
|---|---|
| Runtime / Langage | **.NET 10** / C# |
| UI Desktop | **WPF** + pattern **MVVM** (CommunityToolkit.Mvvm) |
| Accès données | **Entity Framework Core** + **SQLite** |
| Notifications temps réel | **Node.js** (serveur) + **SignalR** |
| Rapports PDF | **iText7** |
| Validation | **FluentValidation** |
| Mapping | **AutoMapper** |
| Journalisation | **Serilog** |
| Sécurité mots de passe | **BCrypt.Net** |
| QR Code | **QRCoder** |
| Graphiques / KPI | **LiveCharts2** |
| Injection de dépendances | Microsoft.Extensions.DependencyInjection |
| Tests | xUnit / FluentAssertions / Moq |

## 🏛️ Architecture (Clean Architecture)

```
GMAO.sln
├─ src/
│  ├─ GMAO.Domain            → Entités, enums, règles métier, interfaces du domaine
│  ├─ GMAO.Application       → Use-cases, services, DTO, validation, mapping, interfaces d'infra
│  ├─ GMAO.Infrastructure    → Implémentations transverses (PDF, QR, notifications, email, fichiers)
│  ├─ GMAO.Persistence       → DbContext EF Core, configurations, repositories, Unit of Work, migrations
│  ├─ GMAO.Shared            → Constantes, helpers, résultats (Result<T>), exceptions communes
│  └─ GMAO.Presentation.Wpf  → Application WPF (Views, ViewModels, thèmes Fluent/Material)
├─ servers/
│  └─ notification-server    → Serveur Node.js + SignalR (notifications desktop/email)
├─ reports/                  → Modèles de rapports PDF (templates, logos)
├─ assets/                   → Logos, icônes, ressources graphiques
├─ tests/                    → Tests unitaires et d'intégration
└─ docs/                     → Documentation (analyse, cahier des charges, UML, BD…)
```

> Règle de dépendance : `Presentation → Application → Domain`, `Persistence/Infrastructure → Application/Domain`.
> Le **Domain** ne dépend de rien. Tout pointe vers l'intérieur.

## 👥 Rôles utilisateurs

Administrateur · Responsable SAV · Ingénieur Biomédical · Client (Hôpital) · Technicien · Invité.

## 📦 État d'avancement

Voir [docs/00-feuille-de-route.md](docs/00-feuille-de-route.md).

## 🚀 Démarrage

**Le plus simple** : double-cliquer sur **`Lancer-GMAO.cmd`** (ou le raccourci **« GMAO Datex-Ohmeda »**) — démarre le serveur de notifications **et** l'application.

```powershell
# Build complète + tests + publication (script automatisé)
./build.ps1

# — ou manuellement —
dotnet build GMAO.slnx -c Debug
dotnet run --project src/GMAO.Presentation.Wpf

# Serveur de notifications temps réel (port 4000)
cd servers/notification-server
npm install
node server.js
```

Connexion par défaut : **admin / Admin@123**. Voir le [guide de déploiement](docs/06-guide-deploiement.md).

## 📄 Licence

Projet académique — MEDICANA / PFE. Usage interne.

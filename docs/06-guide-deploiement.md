# Guide de déploiement — GMAO Datex-Ohmeda

## 1. Prérequis

| Composant | Version | Rôle |
|---|---|---|
| **.NET 10 Desktop Runtime** (x64) | 10.0+ | Exécution de l'application WPF |
| **Node.js** | 18+ | Serveur de notifications temps réel |
| Windows | 10 / 11 (x64) | Plateforme cible |

> La build publiée est **framework-dependent** : le poste cible doit disposer du **.NET 10 Desktop Runtime**. Pour un poste sans runtime, produire une build **self-contained** (voir §4).

## 2. Compilation et publication (poste de développement)

Script automatisé à la racine du dépôt :

```powershell
./build.ps1
```

Il restaure, compile, exécute les tests, publie l'application dans `publish/GMAO-app/` et installe les dépendances du serveur Node.

### Étapes manuelles équivalentes

```powershell
dotnet build GMAO.slnx -c Release
dotnet test  tests/GMAO.Tests.Unit -c Release
dotnet publish src/GMAO.Presentation.Wpf -c Release -r win-x64 --self-contained false -o publish/GMAO-app
cd servers/notification-server ; npm install
```

## 3. Installation sur le poste client

1. Copier le dossier **`publish/GMAO-app/`** sur le poste.
2. Copier **`servers/notification-server/`** (avec `node_modules`) sur le poste serveur (ou le même poste).
3. Démarrer le serveur de notifications :
   ```powershell
   cd notification-server
   node server.js
   ```
4. Lancer **`GMAO.Presentation.Wpf.exe`**.

### Première connexion
- Identifiant : **admin** · Mot de passe : **Admin@123** (à changer en production).
- La base **`gmao.db`** (SQLite) est créée automatiquement au premier lancement, à côté de l'exécutable, et alimentée avec des données de démonstration.

## 4. Build autonome (sans runtime préinstallé)

```powershell
dotnet publish src/GMAO.Presentation.Wpf -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -o publish/GMAO-standalone
```

Produit un exécutable embarquant le runtime (taille plus importante).

## 5. Emplacements & données

| Élément | Emplacement |
|---|---|
| Base de données | `gmao.db` (à côté de l'exécutable) |
| Journaux Serilog | `logs/gmao-*.log` |
| Rapports PDF générés | `reports/generated/` |
| Serveur notifications | `http://localhost:4000` (port configurable via `PORT`) |

## 6. Configuration

- **Port du serveur de notifications** : variable d'environnement `PORT` (défaut 4000). Adapter `BaseHttp`/`UrlWebSocket` dans `NotificationTempsReelClient` si le serveur est distant.
- **Mot de passe administrateur par défaut** : `App.MotDePasseAdminParDefaut`.

## 7. Sauvegarde

Sauvegarder régulièrement le fichier **`gmao.db`** (et `gmao.db-wal` s'il est présent). Une copie à chaud est possible via l'API de sauvegarde SQLite ; à froid, fermer l'application au préalable.

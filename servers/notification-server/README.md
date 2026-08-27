# Serveur de notifications — GMAO Datex-Ohmeda

Serveur **Node.js** de notifications temps réel pour l'application GMAO (MEDICANA).

## Rôle

- Diffuse en **temps réel** (WebSocket) les notifications à tous les clients connectés (l'application WPF).
- Expose une **API REST** pour émettre des notifications depuis l'application.

> Note d'architecture : le cahier des charges mentionne SignalR. SignalR étant une technologie ASP.NET difficile à héberger sous Node.js, le transport temps réel est assuré par **WebSocket natif** (`ws`), consommé côté .NET par `ClientWebSocket`. L'objectif fonctionnel (notifications temps réel) est pleinement atteint.

## Démarrage

```bash
cd servers/notification-server
npm install
npm start
```

Le serveur écoute sur le port **4000** (configurable via `PORT`).

## API

| Méthode | Route | Description |
|---|---|---|
| `GET` | `/health` | État du service (nombre de clients, notifications) |
| `POST` | `/notify` | Émet une notification `{ type, titre, message, reference }` |
| `GET` | `/notifications` | Historique des dernières notifications |
| `WS` | `/ws` | Canal temps réel (réception des notifications) |

## Exemple

```bash
curl -X POST http://localhost:4000/notify \
  -H "Content-Type: application/json" \
  -d '{"type":"InterventionUrgente","titre":"DI critique","message":"Patient connecté"}'
```

## Test rapide

```bash
node test-client.js
```

Affiche le message de bienvenue puis la notification diffusée — preuve du fonctionnement temps réel.

'use strict';

/*
 * Serveur de notifications temps réel — GMAO Datex-Ohmeda (MEDICANA)
 * -----------------------------------------------------------------
 * - API REST (Express) pour émettre des notifications (POST /notify)
 * - Diffusion temps réel via WebSocket (lib ws) à tous les clients connectés
 *   (l'application WPF se connecte avec ClientWebSocket).
 */

const http = require('http');
const express = require('express');
const cors = require('cors');
const { WebSocketServer } = require('ws');

const PORT = process.env.PORT || 4000;

const app = express();
app.use(cors());
app.use(express.json());

const server = http.createServer(app);
const wss = new WebSocketServer({ server, path: '/ws' });

// Historique mémoire des dernières notifications.
const historique = [];
const MAX_HISTORIQUE = 100;

function diffuser(notification) {
  const charge = JSON.stringify(notification);
  let envoyes = 0;
  for (const client of wss.clients) {
    if (client.readyState === 1 /* OPEN */) {
      client.send(charge);
      envoyes++;
    }
  }
  return envoyes;
}

wss.on('connection', (socket) => {
  console.log(`[ws] client connecté (total : ${wss.clients.size})`);
  // Message de bienvenue.
  socket.send(JSON.stringify({
    type: 'Systeme',
    titre: 'Connexion établie',
    message: 'Notifications temps réel actives.',
    date: new Date().toISOString()
  }));
  socket.on('close', () => console.log(`[ws] client déconnecté (total : ${wss.clients.size})`));
});

// Santé du service.
app.get('/health', (req, res) => {
  res.json({ status: 'ok', clients: wss.clients.size, notifications: historique.length });
});

// Émission d'une notification.
app.post('/notify', (req, res) => {
  const { type, titre, message, reference } = req.body || {};
  if (!titre || !message) {
    return res.status(400).json({ erreur: 'Champs « titre » et « message » obligatoires.' });
  }
  const notification = {
    type: type || 'Info',
    titre,
    message,
    reference: reference || null,
    date: new Date().toISOString()
  };
  historique.unshift(notification);
  if (historique.length > MAX_HISTORIQUE) historique.pop();

  const envoyes = diffuser(notification);
  console.log(`[notify] « ${titre} » → ${envoyes} client(s)`);
  res.json({ diffuse: envoyes, notification });
});

// Récupération de l'historique.
app.get('/notifications', (req, res) => res.json(historique));

server.listen(PORT, () => {
  console.log(`Serveur de notifications GMAO démarré sur http://localhost:${PORT}`);
  console.log(`WebSocket : ws://localhost:${PORT}/ws`);
});

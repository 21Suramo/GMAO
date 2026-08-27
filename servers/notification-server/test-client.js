'use strict';
// Client de test : se connecte au WebSocket, déclenche une notification et affiche les messages reçus.
const WebSocket = require('ws');
const http = require('http');

const ws = new WebSocket('ws://localhost:4000/ws');

ws.on('open', () => {
  console.log('CONNECTE au serveur');
  const data = JSON.stringify({ type: 'Test', titre: 'Test temps reel', message: 'Diffusion WebSocket OK' });
  const req = http.request({
    host: 'localhost', port: 4000, path: '/notify', method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Content-Length': Buffer.byteLength(data) }
  });
  req.write(data);
  req.end();
});

ws.on('message', (d) => console.log('RECU:', d.toString()));
ws.on('error', (e) => console.log('ERREUR:', e.message));

setTimeout(() => process.exit(0), 1500);

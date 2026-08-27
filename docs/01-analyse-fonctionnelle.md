# Analyse fonctionnelle — GMAO Datex-Ohmeda

## 1. Contexte

MEDICANA, distributeur officiel **Datex-Ohmeda**, assure le SAV des respirateurs d'anesthésie (gammes **Aespire, Avance, Aisys**…) installés dans les blocs opératoires de ses hôpitaux clients. La gestion actuelle (fichiers, papier) ne permet ni traçabilité fiable, ni analyse des défaillances, ni pilotage de la disponibilité.

L'application GMAO cible exclusivement la **maintenance corrective** (interventions sur panne) de ces respirateurs.

## 2. Acteurs

| Acteur | Description | Objectifs principaux |
|---|---|---|
| **Administrateur** | Gère la configuration, les utilisateurs, les plannings | Paramétrage, droits, supervision globale |
| **Responsable SAV** | Pilote le service après-vente | Affecter, suivre, valider, analyser les KPI |
| **Ingénieur Biomédical** | Réalise les interventions techniques | Diagnostiquer, réparer, documenter, clôturer |
| **Technicien** | Appui terrain | Interventions de niveau 1, saisie |
| **Client (Hôpital)** | Personnel hospitalier | Déclarer une panne (DI), suivre l'état |
| **Invité** | Accès en lecture limitée | Consultation / démonstration |

## 3. Besoins fonctionnels

### BF-01 — Gestion des utilisateurs & sécurité
- Authentification sécurisée (mot de passe **haché BCrypt**).
- Rôles avec droits différenciés (RBAC).
- Gestion des sessions + **historique des connexions** (date, IP, succès/échec).

### BF-02 — Gestion du parc de respirateurs
- Fiche appareil : n° série, **QR Code**, code interne, modèle, versions logicielle/matérielle, bloc opératoire, service, hôpital, date de mise en service, contrat, garantie, état.
- **Fiche de vie** complète : interventions, pannes, pièces, rapports, tests, photos, documents, signatures.
- Documents associés (manuels PDF, schémas), photos.

### BF-03 — Déclaration d'intervention (DI) par QR Code
- Le personnel hospitalier **scanne le QR Code** → interface simplifiée.
- Saisie : respirateur, **symptômes prédéfinis**, commentaires, photo, vidéo, priorité, urgence, **patient connecté (O/N)**.
- Si **patient connecté = Oui** → intervention **critique**, **notification immédiate**, **priorité maximale**, **affectation automatique** d'un ingénieur.

### BF-04 — Gestion des interventions
- Champs : n° DI, date, client, ingénieur, respirateur, description, diagnostic, cause, pièces remplacées, temps de déplacement, temps de réparation, main d'œuvre, état, commentaires, photos, documents, **signature**.
- **Workflow** d'état (voir §4).
- **Tableau Kanban** temps réel avec **drag & drop**.

### BF-05 — Check-list de clôture obligatoire
Clôture impossible sans validation de : Autotest OK · Test étanchéité · Calibration débit · Calibration O₂ · Batterie · Alimentation · Alarmes · Validation finale.

### BF-06 — Blocage « HORS SERVICE »
- Un respirateur déclaré **HORS SERVICE** ne peut plus être programmé pour intervention médicale.
- Badge rouge, historique, motif, date, auteur.

### BF-07 — Gestion des pièces détachées
- Fiche pièce : référence, nom, catégorie, compatibilité, stock, stock minimum, emplacement, prix, fournisseur, historique, alertes.
- **Association panne ↔ pièce** (ex. *Erreur fuite → valve expiratoire → joint → capteur débit*).
- **Stock intelligent** : alertes stock minimum / nul / péremption / commande nécessaire.

### BF-08 — Documentation technique
Par respirateur : manuel utilisateur, manuel maintenance, schémas, procédures, calibration, guides SAV, photos, PDF intégrés.

### BF-09 — Rapports PDF (iText7)
Génération automatique : logo MEDICANA, infos client/appareil, description panne, diagnostic, pièces, temps, coût, signature, **QR Code**, photos, validation finale, historique.

### BF-10 — Notifications temps réel (Node.js + SignalR)
Nouvelle DI · intervention urgente · pièce indisponible · stock faible · respirateur critique · fin d'intervention · temps dépassé → **desktop + email**.

### BF-11 — Tableau de bord & KPI / BI
Indicateurs : nombre de respirateurs, interventions ouvertes/critiques, respirateurs HS, disponibilité globale, **MTTR**, **MTBF**, pannes par modèle/client, top 10 pièces, top 10 pannes, évolution mensuelle, camembert des états, histogrammes, **heatmap des pannes**, courbes.
BI : disponibilité, temps d'arrêt, coûts (appareil/client/panne), classements ingénieurs/clients, indice de fiabilité.

### BF-12 — Affectation automatique
À partir du **planning**, **congés**, **disponibilité**, **compétences**, **zone géographique** → choix automatique de l'ingénieur.

### BF-13 — Recherche multicritère
N° série · client · ville · modèle · panne · ingénieur · état · date · pièce · contrat.

## 4. Workflow d'une intervention

```
Nouvelle DI → Affectée → En déplacement → Diagnostic → Réparation → Test fonctionnel → Validation → Clôturée
                                                            │
                                                            └──(rupture)──► En attente pièce ──► (reprise) Réparation
```

## 5. Besoins non fonctionnels

| Catégorie | Exigence |
|---|---|
| **Performance** | Dashboard < 2 s ; recherche < 1 s sur parc de référence |
| **Sécurité** | Hachage BCrypt, RBAC, journal d'audit des connexions |
| **Fiabilité** | Gestion centralisée des exceptions, transactions (Unit of Work) |
| **Disponibilité données** | Base locale SQLite + sauvegardes |
| **Ergonomie** | Fluent/Material Design, Dark/Light, navigation latérale, animations |
| **Maintenabilité** | Clean Architecture, SOLID, tests, documentation XML |
| **Traçabilité** | Historisation complète (fiche de vie, audit) |
| **Internationalisation** | Interface FR (prévoir ressources localisables) |

## 6. Règles de gestion (extraits)

- **RG-01** : Une DI avec *patient connecté = Oui* est automatiquement de priorité **Critique** et déclenche une affectation + notification immédiates.
- **RG-02** : Une intervention ne peut passer à **Clôturée** que si **toutes** les lignes de check-list sont validées.
- **RG-03** : Un respirateur **HORS SERVICE** ne peut recevoir une nouvelle DI de type programmé/médical.
- **RG-04** : Toute pièce dont `stock ≤ stockMinimum` génère une alerte ; `stock = 0` bloque la consommation et notifie.
- **RG-05** : Le **MTTR** est calculé sur la base (temps de réparation + déplacement) des interventions clôturées.
- **RG-06** : Le **MTBF** est calculé par appareil = temps de bon fonctionnement cumulé / nombre de pannes.
- **RG-07** : Chaque changement d'état d'intervention est horodaté et tracé (auteur, date).

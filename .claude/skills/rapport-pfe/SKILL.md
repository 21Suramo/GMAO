---
name: rapport-pfe
description: Rédige le rapport de PFE du projet GMAO Datex-Ohmeda (MEDICANA) à partir du dépôt — docs/, code source, tests, captures. Produit un rapport académique français complet (page de garde, remerciements, résumé/abstract, 4 chapitres, conclusion, bibliographie, annexes) en Markdown maître, puis rendu .docx ou LaTeX/PDF. Utiliser dès que l'utilisateur demande le rapport de PFE, un chapitre du rapport, un plan de rapport, le résumé/abstract, ou la mise en forme finale du mémoire.
---

# Rapport de PFE — GMAO Datex-Ohmeda

Génère le mémoire de fin d'études à partir du dépôt lui-même : le code et `docs/`
sont la source de vérité, jamais la mémoire du modèle.

## Règle absolue : zéro invention

Tout chiffre, nom de classe, technologie, métrique ou capture doit être vérifiable
dans le dépôt. Avant d'écrire un chapitre, lire les sources qui l'alimentent
(tableau §3). Ce qui n'est pas vérifiable se signale explicitement au lieu d'être
comblé : `[À COMPLÉTER : nom du service d'accueil]`, `[À CONFIRMER : date de début
du stage]`. Un rapport de PFE est soutenu devant un jury qui pose des questions —
une donnée inventée coûte plus cher qu'un trou assumé.

Sont typiquement hors dépôt et donc à marquer : identité de l'encadrant, dates du
stage, organigramme de MEDICANA, chiffres d'affaires, effectifs, photos du parc.

## 1. Cadrage avant écriture

Demander à l'utilisateur, en une seule fois, ce qui n'est pas dans le dépôt :
établissement et filière, intitulé exact du PFE, encadrant académique et
industriel, période du stage, volume attendu (nb de pages), format de rendu
(.docx ou LaTeX/PDF), et s'il fournit des captures d'écran de l'application.

Si l'utilisateur veut un premier jet immédiat, ne pas bloquer : produire le
rapport avec des marqueurs `[À COMPLÉTER : …]` et lister ces marqueurs en fin de
réponse.

## 2. Procédé

1. Lire `docs/00-feuille-de-route.md` en premier — il donne l'état réel de chaque
   module. **Ne jamais présenter au passé accompli un module marqué ⬜ ou 🟡.**
2. Lire les sources du chapitre visé (§3), et le code quand le chapitre décrit une
   mécanique (moteur d'affectation, RBAC, génération PDF…).
3. Écrire dans `rapport/` : un fichier Markdown par chapitre
   (`rapport/01-contexte.md`, …) plus `rapport/00-liminaires.md`. Un chapitre à la
   fois, pas le rapport entier d'un bloc.
4. Rendre le document final (§5).

Écrire les chapitres dans l'ordre 1 → 4, puis l'introduction générale, la
conclusion et le résumé **en dernier** : ils synthétisent ce qui a réellement été
écrit.

## 3. Plan et sources

Plan détaillé, budget de pages et sections attendues :
`references/plan-rapport.md`. Correspondance chapitre → fichiers du dépôt :

| Chapitre | Sources dans le dépôt |
|---|---|
| 1. Contexte général | `docs/01-analyse-fonctionnelle.md` §1-2, `docs/02-cahier-des-charges.md` §1-2/§7, `docs/00-feuille-de-route.md` |
| 2. Analyse & spécification | `docs/01-analyse-fonctionnelle.md`, `docs/02-cahier-des-charges.md` §3-5, `docs/05-diagrammes-uml.md` (cas d'utilisation) |
| 3. Conception | `docs/03-architecture.md`, `docs/04-modele-de-donnees.md`, `docs/05-diagrammes-uml.md` (classes, séquence, états), `src/GMAO.Domain/Entities/`, `CLAUDE.md` (patterns) |
| 4. Réalisation & tests | `src/**`, `servers/notification-server/`, `tests/GMAO.Tests.Unit/`, `docs/06-guide-deploiement.md`, `build.ps1` |

Le chapitre 3 doit expliquer **pourquoi** ces choix (Clean Architecture, Result
pattern, Strategy pour l'affectation, SQLite embarqué, WebSocket natif en
remplacement de SignalR), pas seulement les décrire : c'est là que se joue la note.

## 4. Figures et tableaux

- Les diagrammes Mermaid de `docs/05-diagrammes-uml.md` se reprennent tels quels ;
  pour un rendu .docx/PDF, les exporter en PNG (`mmdc -i x.mmd -o x.png`) et les
  placer dans `rapport/figures/`.
- Chaque figure porte une légende numérotée sous l'image
  (`Figure 3.2 — Diagramme de classes du domaine`), chaque tableau une légende
  au-dessus (`Tableau 2.1 — Exigences fonctionnelles`).
- Toute figure est appelée dans le texte avant d'apparaître
  (« …comme le montre la figure 3.2 »). Une figure jamais citée est à supprimer.
- Les captures d'écran de l'application viennent de l'utilisateur ; si elles
  manquent, laisser `[CAPTURE : écran Interventions — liste + filtres]`.

## 5. Rendu final

- **.docx** (cas courant) : assembler les chapitres en un Markdown unique puis
  utiliser la skill `docx` — elle gère sommaire automatique, styles de titres,
  pagination et en-têtes. Numérotation des titres sur 3 niveaux, interligne 1,5,
  Times New Roman 12 ou Calibri 11, marges 2,5 cm, pagination en chiffres romains
  pour les liminaires puis arabes à partir de l'introduction.
- **LaTeX/PDF** : classe `report`, `\usepackage[french]{babel}`, moteur XeLaTeX.
- Vérifier avant remise : sommaire à jour, listes des figures/tableaux/
  abréviations, aucune occurrence résiduelle de `[À COMPLÉTER` ou `[CAPTURE`.

## 6. Style

Règles de rédaction, typographie française et pièges de la prose générée :
`references/style-redaction.md`. À lire avant d'écrire le premier chapitre.

---
name: rapport-pfe
description: Rédige le mémoire de projet de fin d'études du projet GMAO Datex-Ohmeda (MEDICANA) à partir du dépôt — docs/, code source, tests, captures — en respectant à la lettre la charte UM6SS/ESGB (Cycle Ingénieur Génie Biomédical). Produit page de garde, dédicaces, remerciements, résumé français/anglais/arabe, listes, introduction générale, 3 à 4 chapitres, conclusion générale, bibliographie et annexes, en Markdown maître puis rendu .docx. Utiliser dès que l'utilisateur demande le rapport de PFE, un chapitre, un plan, le résumé/abstract/ملخص, ou la mise en forme finale du mémoire.
---

# Mémoire de PFE — GMAO Datex-Ohmeda (UM6SS / ESGB)

Rédige le mémoire à partir du dépôt lui-même : le code et `docs/` sont la source
de vérité, jamais la mémoire du modèle.

## Règle n° 1 — la charte prime sur tout

`references/charte-um6ss.md` est **normatif** : structure imposée, mise en forme,
figures et tableaux, style, bibliographie. Le template officiel est joint
(`references/TEMPLATE_PFE_UM6SS.pdf`). **Lire la charte avant d'écrire la
première ligne**, et s'y conformer sans arbitrage personnel : ce document est
évalué par un jury qui l'applique.

Les cinq écarts les plus faciles à commettre, tous sanctionnés :

1. Le **résumé arabe (ملخص)** est obligatoire, au même titre que le français et
   l'anglais.
2. Les figures et tableaux sont numérotés **en continu** sur tout le mémoire
   (`Figure 1`, `Figure 2`…), légende **au-dessus** pour un tableau,
   **au-dessous** pour une figure.
3. Tout est au **présent**, l'auteur se dit **« nous »**, le lecteur **« on »**.
4. Chaque chapitre ouvre sur une section numérotée `N.1 Introduction` et ferme
   sur `N.x Conclusion`.
5. Le **code source** se place dans une figure constituée d'un tableau.

## Règle n° 2 — zéro invention

Tout chiffre, nom de classe, technologie ou métrique doit être vérifiable dans le
dépôt. Avant d'écrire un chapitre, lire les sources qui l'alimentent (§3). Ce qui
n'est pas vérifiable se signale au lieu d'être comblé :
`[À COMPLÉTER : date de soutenance]`, `[CAPTURE : écran Interventions]`. La
charte classe le **plagiat** parmi les erreurs éliminatoires ; une donnée ou une
référence fabriquée relève du même registre devant un jury.

Sont hors dépôt et donc systématiquement à marquer : nom et prénom de l'étudiant,
année universitaire, date de soutenance, composition du jury, encadrants,
présentation et logo de MEDICANA, photos du parc, captures d'écran.

## 1. Cadrage avant écriture

Demander en une seule fois : **nom et prénom**, **intitulé exact du sujet**,
**année universitaire**, **date de soutenance**, **jury** (président,
examinateur, rapporteur, encadrant, invité), **encadrant industriel**, **période
du stage**, **logos** disponibles, **captures d'écran** de l'application.

Si l'utilisateur veut un premier jet immédiat, ne pas bloquer : produire avec les
marqueurs `[À COMPLÉTER : …]`, puis les lister en fin de réponse.

## 2. Procédé

1. Lire `references/charte-um6ss.md`, puis `references/style-redaction.md`.
2. Lire `docs/00-feuille-de-route.md` — il donne l'état réel de chaque module.
   **Ne jamais présenter au présent accompli un module marqué ⬜ ou 🟡.**
3. Lire les sources du chapitre visé (§3), et le code quand le chapitre décrit
   une mécanique (moteur d'affectation, RBAC, génération PDF…).
4. Écrire dans `rapport/` : un fichier Markdown par élément
   (`rapport/00-liminaires.md`, `rapport/01-contexte.md`, …). **Un chapitre à la
   fois**, jamais le mémoire entier d'un bloc.
5. Relire le chapitre contre la charte (§4 ci-dessous) avant de passer au suivant.
6. Rendre le document final (§5).

Écrire les chapitres dans l'ordre 1 → 4, puis l'**introduction générale**, la
**conclusion générale** et les **trois résumés en dernier** : ils synthétisent ce
qui a réellement été écrit.

## 3. Plan et sources

Plan détaillé conforme au squelette du template : `references/plan-rapport.md`.
Correspondance chapitre → fichiers du dépôt :

| Chapitre | Sources dans le dépôt |
|---|---|
| 1. Contexte général | `docs/01-analyse-fonctionnelle.md` §1-2, `docs/02-cahier-des-charges.md` §1-2/§7, `docs/00-feuille-de-route.md` |
| 2. Analyse et spécification | `docs/01-analyse-fonctionnelle.md`, `docs/02-cahier-des-charges.md` §3-5, `docs/05-diagrammes-uml.md` (cas d'utilisation) |
| 3. Conception | `docs/03-architecture.md`, `docs/04-modele-de-donnees.md`, `docs/05-diagrammes-uml.md` (classes, séquence, états), `src/GMAO.Domain/Entities/`, `CLAUDE.md` (patrons) |
| 4. Réalisation et tests | `src/**`, `servers/notification-server/`, `tests/GMAO.Tests.Unit/`, `docs/06-guide-deploiement.md`, `build.ps1` |

Le chapitre 3 doit expliquer **pourquoi** ces choix (Clean Architecture, patron
Result, Strategy pour l'affectation, SQLite embarqué, serveur WebSocket natif en
remplacement de SignalR), pas seulement les décrire : c'est là que se joue la
note.

## 4. Contrôle de conformité par chapitre

Vérifier avant de passer au chapitre suivant :

- [ ] Sections `N.1 Introduction` et `N.x Conclusion` présentes et numérotées
- [ ] Aucune numérotation au 4e niveau (`1.1.1.1`)
- [ ] Verbes au présent · « nous » pour l'auteur · « on » pour le lecteur
- [ ] Aucun synonyme concurrent pour un même concept (§ style)
- [ ] Chevrons « » partout, italique + chevrons pour les termes anglais
- [ ] Chaque figure et chaque tableau annoncé **avant** d'apparaître, appelé par
      son numéro, jamais par « ci-dessus » / « ci-après »
- [ ] Légendes au bon endroit et au bon format (`Figure 4 - Titre`,
      `Tableau 2 - Titre`), numérotation continue avec les chapitres précédents
- [ ] Code source placé dans une figure-tableau
- [ ] Volume comparable aux autres chapitres (la charte exige l'équilibre)

## 5. Rendu final

Assembler les fichiers en un Markdown unique, puis utiliser la skill `docx` en
lui imposant la mise en forme de la charte : marges 2,5 cm, reliure 0,5 cm,
justification, interligne 1,5, Times New Roman 12, espacement 6 pts, titres
16/14/12 gras, pagination centrée en bas — romaine pour les liminaires, arabe à
partir de l'introduction générale, page de garde non numérotée.

Les diagrammes Mermaid de `docs/05-diagrammes-uml.md` s'exportent en PNG
(`mmdc -i x.mmd -o x.png`) vers `rapport/figures/`.

Vérification finale avant remise : table des matières, listes des abréviations,
des tableaux et des figures à jour et paginées ; numérotation des figures et
tableaux continue et sans trou ; aucune occurrence résiduelle de `[À COMPLÉTER`
ou `[CAPTURE` ; orthographe relue.

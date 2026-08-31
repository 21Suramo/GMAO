# Mémoire de PFE — fichiers sources

Premier jet rédigé à partir du dépôt (`docs/`, code, tests), conforme à la charte
UM6SS/ESGB : `.claude/skills/rapport-pfe/references/charte-um6ss.md`.

Livrable : **`Memoire-PFE-GMAO.docx`** — 60 pages, 24 figures, 14 tableaux.

## Régénérer le .docx

```bash
npm install docx          # une seule fois
node rapport/build-memoire.js
```

Le script applique la mise en forme imposée : marges 2,5 cm, reliure 0,5 cm à
gauche, texte justifié, interligne 1,5, Times New Roman 12, espacement 6 pts,
titres 16/14/12 gras en noir, pagination centrée en bas — page de garde non
numérotée, liminaires en chiffres romains, corps en chiffres arabes repartant
à 1. Il place les blocs de code dans une figure constituée d'un tableau
(règle 2.7 de la charte) et double les espaces entre les phrases (règle 2.9-9).

## Ordre d'assemblage

| Ordre | Fichier | Pagination |
|---|---|---|
| 1 | `00-liminaires.md` — page de garde | non numérotée |
| 2 | `00-liminaires.md` — dédicaces à listes | i, ii, iii… |
| 3 | `05-introduction-generale.md` | 1, 2, 3… |
| 4 | `01-contexte.md` | |
| 5 | `02-analyse.md` | |
| 6 | `03-conception.md` | |
| 7 | `04-realisation.md` | |
| 8 | `06-conclusion-generale.md` | |
| 9 | `07-bibliographie.md` | |
| 10 | `08-annexes.md` | |

La table des matières s'insère automatiquement après le résumé arabe.

## À faire avant la remise

1. **Compléter les 32 marqueurs** — `grep -n "À COMPLÉTER\|À FOURNIR\|À VALIDER\|CAPTURE\|À RÉDIGER\|À PERSONNALISER" rapport/*.md`
   Principaux manques : identité de l'auteur, année universitaire, date de
   soutenance, jury, encadrants, présentation et organigramme de MEDICANA,
   dates du stage pour le diagramme de Gantt, et les dix captures d'écran de
   l'application.
2. **Produire les figures.** Exporter les diagrammes Mermaid de
   `docs/05-diagrammes-uml.md` et `docs/04-modele-de-donnees.md` en PNG vers
   `figures/` (`mmdc -i x.mmd -o x.png`), puis insérer les captures.
   Sur la figure 10, remplacer « SignalR » par « WebSocket » : la source dans
   `docs/` est antérieure au changement de technologie décrit en 3.3.5.
3. **Vérifier la bibliographie.** Les entrées sont réelles mais doivent avoir
   été consultées ; retirer celles qui ne l'ont pas été et compléter les dates
   de consultation des documents web.
4. **Mettre à jour la table des matières** sous Word : clic droit sur le champ,
   « Mettre à jour les champs », puis supprimer la note d'instruction.
5. **Renseigner les numéros de page** dans les listes des tableaux et des
   figures, ou les régénérer par insertion de tables des illustrations.
6. **Rééquilibrer le chapitre 1**, plus court que les trois autres. La charte
   exige des chapitres équilibrés ; la présentation de MEDICANA (1.2) doit
   combler cet écart.

## Contrôles automatiques

```bash
cd rapport
grep -nE "^#{2,4} [0-9]+\.[0-9]+\.[0-9]+\.[0-9]+" *.md   # 4e niveau : interdit
grep -nE "ci-dessus|ci-après|ci-dessous" *.md            # renvois : interdits
grep -nE '"[A-Za-zÀ-ÿ]' *.md                             # guillemets droits : interdits
grep -c "À COMPLÉTER\|À FOURNIR\|CAPTURE" *.md           # marqueurs restants
```

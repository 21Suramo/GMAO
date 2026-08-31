# Style de rédaction — complément à la charte

`charte-um6ss.md` §4 fixe les règles de langue **imposées** (présent, « nous » /
« on », verbes d'action, terme unique par concept, chevrons, italique pour
l'étranger, deux espaces entre phrases). Elles priment. Ce fichier ne traite que
ce que la charte ne couvre pas.

## Terminologie du projet — un seul terme par concept

La charte interdit les synonymes concurrents. Le vocabulaire du domaine est
français et fixé par le code. Table de décision, à appliquer sur tout le
mémoire :

| Concept | Terme retenu | À ne plus employer |
|---|---|---|
| L'appareil maintenu | **respirateur** | équipement, appareil, machine, dispositif |
| La demande de maintenance | **intervention** (sigle **DI** pour la demande) | ticket, demande, requête, tâche |
| Celui qui répare | **ingénieur biomédical** | technicien (réservé au rôle Technicien), agent |
| L'établissement client | **hôpital** | client, établissement, structure |
| La pièce de rechange | **pièce détachée** | composant, article, référence |
| Le cycle de vie d'une intervention | **workflow** (« *workflow* » à la 1re occurrence) | processus, cycle, flux |
| L'enchaînement des états | **état** | statut, phase |

Les identifiants de code se citent tels quels, sans traduction ni accord :
`Respirateur`, `Intervention`, `MoteurAffectation`, `EntiteBase`.

**« DI »** est réservé à *Demande d'Intervention*. L'injection de dépendances
s'écrit en toutes lettres — jamais « DI » — pour éviter l'ambiguïté que la charte
proscrit.

## Traduire ou non les termes anglais

La charte demande de traduire « autant que possible, sauf pour les mots-clés d'un
langage technique ». Application :

- **Traduits** : *design pattern* → schéma ou patron de conception ·
  *soft delete* → suppression logique · *dependency injection* → injection de
  dépendances · *repository* → entrepôt (ou conservé, mais alors uniformément) ·
  *dashboard* → tableau de bord · *report* → rapport.
- **Conservés**, car noms propres de technologies ou mots-clés :
  Clean Architecture, MVVM, Repository, Unit of Work, Strategy, WebSocket,
  Result, xUnit, .NET, WPF, EF Core, SQLite, iText7, Serilog, LiveCharts2.
- À la première occurrence : forme française suivie de la forme anglaise en
  chevrons et italique — schéma de conception (« *design pattern* »).

Fixer le choix une fois et ne plus en dévier.

## Construction des paragraphes

Un paragraphe défend une idée. Chaque section s'ouvre sur une phrase qui annonce
son contenu ; chaque chapitre s'ouvre sur sa section `Introduction` (3 à 5 lignes
annonçant le contenu, comme le montre le template) et se ferme sur sa section
`Conclusion`, qui récapitule et enchaîne sur le chapitre suivant. Phrases de 15 à
25 mots.

## Typographie française, au-delà de la charte

- Espace **insécable** avant `: ; ! ?` et à l'intérieur des chevrons `« … »`
- Majuscules accentuées : `À`, `É`, `Ê`
- Nombres : espace insécable comme séparateur de milliers (`27 000`), virgule
  décimale (`1,5`), unité séparée du nombre (`12 ms`, `2,5 cm`)
- Sigles sans points (`GMAO`, `SAV`), développés à leur première occurrence :
  « Gestion de Maintenance Assistée par Ordinateur (GMAO) », et repris dans la
  liste des abréviations

## Défauts de prose générée

Un jury les repère, et la charte sanctionne la « structure qui manque de
rigueur » comme la forme négligée. À traquer en relecture :

- **Triades systématiques** : « robuste, évolutive et maintenable ». Une qualité
  annoncée se démontre, elle ne s'empile pas.
- **Emphase creuse** : « il convient de souligner que », « joue un rôle crucial »,
  « une solution moderne et innovante ». Supprimer sans remplacer.
- **Analyses en -ant** qui n'apportent rien : « permettant ainsi d'assurer une
  meilleure traçabilité, garantissant de ce fait… ». Couper en phrases courtes,
  et préférer un verbe d'action comme l'exige la charte.
- **Parallélismes négatifs** répétés : « non seulement X, mais aussi Y ».
- **Tirets cadratins** en excès : la virgule ou les parenthèses font le même
  travail ; les chevrons sont réservés à la citation et aux termes étrangers.
- **Conclusions gonflées** : une conclusion de chapitre énonce, elle ne célèbre
  pas.
- **Attributions vagues** : « les experts estiment », « il est communément
  admis ». Citer une entrée de la bibliographie ou retirer l'affirmation.

Relire chaque chapitre une fois contre cette liste. La skill `humanizer` sert de
seconde passe sur un chapitre trop lisse.

## Extraits de code

La charte impose de placer le code **dans une figure constituée d'un tableau**,
légendée au-dessous (`Figure N - Calcul du score d'affectation`). Un extrait ne
dépasse pas une vingtaine de lignes ; au-delà, il part en annexe avec un renvoi.
Chaque extrait est annoncé par une phrase disant ce qu'il faut y regarder, et
**copié du dépôt**, jamais reconstitué de mémoire. Les commentaires français du
code se conservent tels quels.

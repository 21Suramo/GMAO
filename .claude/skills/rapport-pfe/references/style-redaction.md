# Style de rédaction

## Registre

Français académique, **présent de narration** pour décrire le système
(« l'application enregistre… »), **passé composé** pour le travail réalisé
(« nous avons retenu… »). Le « nous » de modestie ou une tournure impersonnelle,
jamais le « je ». Phrases de 15 à 25 mots. Un paragraphe = une idée.

Chaque section commence par une phrase qui annonce son contenu et se termine par
une transition vers la suivante. Chaque chapitre s'ouvre sur une introduction
(3-5 lignes) et se ferme sur une conclusion partielle.

## Typographie française

- Espace **insécable** avant `: ; ! ?` et à l'intérieur des guillemets `« … »`
- Guillemets français `« »`, jamais `" "`
- Majuscules accentuées : `À`, `É`, `Ê`
- Nombres : espace insécable comme séparateur de milliers (`27 000`), virgule
  décimale (`1,5`), unités séparées du nombre (`12 ms`, `2,5 cm`)
- Sigles sans points (`GMAO`, `SAV`), développés à leur première occurrence :
  « Gestion de Maintenance Assistée par Ordinateur (GMAO) »
- Les termes techniques anglais en italique à la première occurrence
  (*pattern*, *soft delete*, *workflow*) ; préférer le terme français quand il
  existe et est courant

## Cohérence terminologique

Le vocabulaire du domaine est **français** et fixé par le code : `Respirateur`,
`Intervention`, `Panne`, `Piece`, `Utilisateur`, `Demande d'Intervention (DI)`,
`Ingénieur Biomédical`. Ne pas alterner « équipement »/« appareil »/« machine »
pour désigner un respirateur, ni « ticket »/« demande »/« intervention » pour la
même chose. Fixer un terme et s'y tenir dans tout le rapport.

## Défauts à éviter — prose générée

Un jury reconnaît un texte d'IA. Les marqueurs les plus visibles :

- **Triades systématiques** : « robuste, évolutive et maintenable », « rapide,
  fiable et sécurisée ». Une qualité annoncée doit être *démontrée*, pas empilée.
- **Emphase creuse** : « il convient de souligner que », « joue un rôle crucial »,
  « constitue une véritable révolution », « une solution moderne et innovante ».
  Supprimer sans remplacer : la phrase est presque toujours meilleure.
- **Analyses en -ant** qui n'apportent rien : « permettant ainsi d'assurer une
  meilleure traçabilité, garantissant de ce fait… ». Couper en phrases courtes.
- **Parallélismes négatifs** : « non seulement X, mais aussi Y » à répétition.
- **Tirets cadratins** en excès : au maximum un par paragraphe ; la virgule, les
  parenthèses ou le point font le même travail.
- **Conclusions gonflées** : une conclusion de chapitre énonce ce qui a été fait,
  elle ne célèbre pas.
- **Attributions vagues** : « les experts estiment », « il est communément admis ».
  Citer une source précise ou retirer l'affirmation.

Relire chaque chapitre une fois contre cette liste avant de le considérer terminé.
La skill `humanizer` peut servir de seconde passe sur un chapitre trop lisse.

## Code et extraits

Un extrait de code ne dépasse pas ~25 lignes ; au-delà, il va en annexe avec un
renvoi. Chaque extrait est précédé d'une phrase disant ce qu'il faut y regarder,
et porte une légende (`Listing 4.3 — Calcul du score d'affectation`). Conserver
les commentaires français du code. Le code cité doit être **copié du dépôt**, pas
reconstitué de mémoire.

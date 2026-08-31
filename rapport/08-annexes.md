# Annexe A
# Diagrammes UML

## A.1 Diagramme de cas d'utilisation général

`[Figure 4 en pleine page — SOURCE : docs/05-diagrammes-uml.md §1]`

## A.2 Diagramme de classes du domaine

`[Figure 7 en pleine page — SOURCE : docs/05-diagrammes-uml.md §2]`

## A.3 Diagrammes de séquence

`[Figures 10 et 11 en pleine page — SOURCE : docs/05-diagrammes-uml.md §3 et §5]`

## A.4 Diagramme d'états d'une intervention

`[Figure 9 en pleine page — SOURCE : docs/05-diagrammes-uml.md §4]`

---

# Annexe B
# Schéma de la base de données

## B.1 Diagramme entité-association complet

`[Figure 8 en pleine page — SOURCE : docs/04-modele-de-donnees.md §4]`

## B.2 Liste des tables

Les vingt-cinq entités du modèle, augmentées des tables de liaison des associations plusieurs-à-plusieurs, produisent vingt-sept tables. Le schéma est généré par migration à partir des classes du domaine.

`[À COMPLÉTER : export du schéma depuis la base gmao.db, ou capture du modèle généré]`

---

# Annexe C
# Extraits de code

## C.1 Matrice des permissions

`[Extrait à insérer dans une figure constituée d'un tableau — SOURCE : src/GMAO.Domain/Entities/Securite/MatricePermissions.cs]`

## C.2 Audit et suppression logique centralisés

`[Extrait du filtre global et de l'horodatage automatique — SOURCE : src/GMAO.Persistence/Context/AppDbContext.cs]`

## C.3 Calcul des indicateurs

`[Extrait du calculateur d'indicateurs — SOURCE : src/GMAO.Application/Services/TableauBord/CalculateurKpi.cs]`

## C.4 Exemple de test unitaire

`[Extrait d'un test du moteur d'affectation — SOURCE : tests/GMAO.Tests.Unit/MoteurAffectationTests.cs]`

---

# Annexe D
# Exemple de rapport d'intervention généré

`[À FOURNIR : rapport PDF produit par l'application, inséré en pleine page]`

---

# Annexe E
# Guide d'installation et manuel utilisateur

## E.1 Prérequis

Poste sous Windows 10 ou 11 en 64 bits, runtime .NET 10 Desktop, Node.js en version 18 ou supérieure pour le serveur de notifications.

## E.2 Installation

Copier le dossier publié de l'application et le dossier du serveur de notifications sur le poste, démarrer le serveur de notifications, puis lancer l'exécutable de l'application. Un fichier de commande fourni à la racine du dépôt enchaîne ces deux dernières opérations.

## E.3 Première connexion

Le compte administrateur initial est créé automatiquement au premier démarrage. Son mot de passe par défaut doit être changé dès la mise en service.

## E.4 Sauvegarde

Sauvegarder le fichier de base de données situé à côté de l'exécutable, ainsi que son fichier de journalisation anticipée lorsqu'il est présent. La copie à froid suppose de fermer préalablement l'application.

## E.5 Manuel utilisateur

`[À COMPLÉTER : parcours illustré des huit modules, à construire à partir des captures d'écran des figures 12 à 22]`

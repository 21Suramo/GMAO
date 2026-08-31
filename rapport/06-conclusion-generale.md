# Conclusion générale

## Contributions

Ce projet dote le service après-vente de MEDICANA d'une application de gestion de maintenance assistée par ordinateur dédiée aux respirateurs d'anesthésie Datex-Ohmeda. Il apporte quatre contributions au regard de la situation de départ.

**Une traçabilité intégrale.** Chaque intervention porte désormais son numéro, son auteur, son historique d'états horodaté et attribué, les pièces détachées consommées, les temps engagés et son rapport archivé. La fiche de vie d'un appareil, qui demandait auparavant un travail manuel de recoupement entre plusieurs sources, se consulte directement.

**Le traitement automatique de la criticité.** La déclaration d'une panne survenant alors qu'un patient est connecté à l'appareil déclenche, sans intervention humaine, le passage en priorité maximale, l'affectation d'un ingénieur et l'émission d'une notification. Cette règle ne dépend d'aucune appréciation de la personne qui déclare la panne, ce qui est déterminant dans le contexte d'urgence d'un bloc opératoire.

**La mesure de la performance.** Le service dispose d'indicateurs calculés sur ses propres données de production : délai moyen de résolution, délai moyen d'affectation, taux de disponibilité, dépassement des délais convenus selon la priorité, répartition des pannes, coûts et charge par technicien. Le pilotage devient possible là où il était auparavant impraticable.

**Une décision d'affectation objectivée.** Le moteur d'affectation formalise, sous la forme d'un score explicite et reproductible, ce qui relevait de la connaissance informelle : la compétence sur le modèle, la proximité géographique, la charge de travail et la disponibilité effective.

Sur le plan technique, le projet met en œuvre une architecture en couches dont la valeur s'est vérifiée en cours de réalisation : le remplacement de la technologie de notification n'a affecté qu'une seule couche, sans toucher au code métier.

## Critique du travail

L'honnêteté du bilan impose d'énoncer les limites du travail accompli.

**Le périmètre fonctionnel n'est pas intégralement couvert.** Sur vingt exigences, cinq ne sont que partiellement satisfaites et deux ne le sont pas. La plus significative est la lecture du QR Code par caméra : le code est généré et apposé sur les appareils, mais son exploitation passe encore par une sélection dans l'interface, ce qui appauvrit le scénario de déclaration initialement envisagé. Le module de documentation technique n'a pas dépassé le stade du modèle de données. Le relais des notifications par courriel, les thèmes clair et sombre et l'interface de planning restent à développer. Le temps moyen entre pannes, prévu par les règles de gestion, n'est pas calculé faute des données de temps de service nécessaires.

**La couverture de tests est partielle.** Les trente-sept tests unitaires portent sur la logique métier pure — contrôle d'accès, affectation, indicateurs, validation, génération de rapports. Ni la couche de persistance, ni l'interface graphique ne sont couvertes. Un défaut de configuration de la correspondance objet-relationnel ou de liaison d'une vue échapperait donc à la campagne actuelle.

**L'application n'a pas été éprouvée en conditions réelles.** Elle fonctionne sur un jeu de données de démonstration volontairement réduit. Son comportement sur un parc complet, avec plusieurs années d'historique et des utilisateurs simultanés, reste à vérifier. Le choix d'une base embarquée, pertinent pour un poste unique, atteindrait ses limites dans un déploiement multi-postes avec écritures concurrentes.

**Le tableau Kanban a été écarté.** Cette décision est assumée et argumentée au chapitre 4 — le déplacement libre de cartes aurait contourné le contrôle des transitions d'états — mais elle s'écarte d'une exigence de priorité haute et doit être présentée comme telle.

Enfin, la charge de structure induite par une architecture en sept projets est réelle. Elle se justifie sur un projet appelé à évoluer ; elle serait disproportionnée pour un outil figé.

## Travaux futurs

Les extensions découlent directement de la critique précédente. À court terme, il convient de compléter les exigences partiellement satisfaites : la lecture du QR Code par caméra, le module de documentation technique, le relais des notifications par courriel, les thèmes clair et sombre, l'interface de planning et la généralisation de la recherche multicritère. Le calcul du temps moyen entre pannes suppose au préalable d'enrichir le modèle des données de temps de service.

À moyen terme, l'effort porte sur la robustesse : étendre la campagne de tests à la couche de persistance et à l'interface graphique, puis éprouver l'application sur un parc réel et un historique volumineux. Le passage à un serveur de base de données relationnel, que l'approche Code First rend accessible sans réécriture de la couche applicative, conditionne le déploiement multi-postes.

## Perspective

Deux extensions dépassent le cadre initial et méritent d'être signalées.

La première est l'ouverture à la **maintenance préventive planifiée**, explicitement exclue de cette version. Le modèle de données actuel — parc, interventions, pièces détachées, compétences, congés — en constitue déjà l'essentiel du socle. L'ajout de gammes de maintenance et d'un échéancier permettrait de passer d'une maintenance réactive à une maintenance anticipée, ce qui prolonge naturellement le deuxième constat de l'analyse de l'existant.

La seconde tient à la **généralisation de l'outil**. Rien dans la conception ne restreint l'application aux respirateurs d'anesthésie : le modèle raisonne en termes d'appareil, de modèle, de panne et de pièce détachée. Son extension aux autres familles de dispositifs médicaux d'un établissement, puis son déploiement multi-sites, constituent une évolution cohérente. À cette échelle, les données accumulées permettraient d'aborder l'analyse prédictive des défaillances, en exploitant l'association entre pannes et pièces détachées déjà présente au modèle.

Au-delà de ces développements, ce projet nous a conduits à confronter des principes d'architecture logicielle aux contraintes d'un domaine où la défaillance d'un équipement engage la sécurité d'un patient. C'est cette confrontation, plus que la maîtrise d'une technologie particulière, qui constitue l'apport principal de ce travail de fin d'études.

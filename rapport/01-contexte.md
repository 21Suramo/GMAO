# Chapitre 1
# Contexte général du projet

## 1.1 Introduction

Ce premier chapitre situe le projet dans son environnement. Nous présentons l'organisme d'accueil, le domaine technique sur lequel porte le travail — les respirateurs d'anesthésie Datex-Ohmeda — puis la situation de départ du service après-vente (SAV). L'analyse critique de cette situation conduit à la problématique et aux objectifs du projet. Nous délimitons ensuite le périmètre retenu et exposons la démarche de conduite du projet.

## 1.2 Organisme d'accueil

MEDICANA est le distributeur officiel des équipements Datex-Ohmeda et assure le service après-vente des respirateurs d'anesthésie installés chez ses clients hospitaliers. Son activité de maintenance couvre l'ensemble du cycle de vie de ces équipements : installation, maintenance corrective, fourniture de pièces détachées et accompagnement technique des services biomédicaux hospitaliers.

`[À COMPLÉTER : historique de MEDICANA, date de création, effectifs, implantation géographique, chiffres d'activité, organigramme du service après-vente et positionnement du stage dans cet organigramme]`

`[Figure 1 - Organigramme de MEDICANA — À FOURNIR]`

## 1.3 Le respirateur d'anesthésie, un équipement critique

Le respirateur d'anesthésie assure la ventilation du patient pendant une intervention chirurgicale, ainsi que l'administration des gaz et des agents anesthésiques. Il appartient à la catégorie des dispositifs médicaux de maintien des fonctions vitales : une défaillance survenant pendant une opération met directement en jeu la sécurité du patient.

MEDICANA maintient les gammes **Aespire**, **Avance** et **Aisys** de Datex-Ohmeda. Ces appareils partagent une architecture commune — bloc de ventilation, circuit patient, capteurs de débit et d'oxygène, module d'alimentation et batterie de secours — mais diffèrent par leurs versions logicielle et matérielle, ce qui impose un suivi individualisé de chaque appareil du parc.

Cette criticité structure tout le projet. Elle justifie deux exigences que l'on retrouve dans l'ensemble du mémoire : la traçabilité intégrale des interventions, et le traitement prioritaire des pannes survenant alors qu'un patient est connecté à l'appareil.

`[Figure 2 - Respirateur d'anesthésie Datex-Ohmeda — PHOTO À FOURNIR]`

## 1.4 Étude de l'existant et analyse critique

La gestion actuelle du SAV repose sur des fichiers bureautiques et des documents papier. Les demandes d'intervention arrivent par téléphone ou par courriel, les comptes rendus sont saisis manuellement, et l'historique de chaque appareil se reconstitue en consultant plusieurs sources distinctes.

Ce mode de fonctionnement présente quatre limites que l'on peut caractériser précisément.

1) **Traçabilité incomplète.** L'historique d'un respirateur — pannes survenues, pièces détachées remplacées, ingénieurs intervenus, rapports produits — se disperse entre plusieurs classeurs et plusieurs fichiers. Reconstituer la fiche de vie d'un appareil demande un travail manuel de recoupement, et cette reconstitution reste partielle.

2) **Absence d'analyse des défaillances.** Faute de données structurées, le service ne dispose d'aucun moyen d'identifier les pannes récurrentes, les modèles les plus fragiles ou les pièces les plus consommées. La maintenance reste purement réactive.

3) **Pilotage impossible.** Les indicateurs usuels de la maintenance — délai moyen de résolution, délai d'affectation, taux de disponibilité du parc, respect des délais convenus — ne sont pas calculables. Le responsable du SAV pilote son activité sans mesure.

4) **Affectation empirique.** Le choix de l'ingénieur envoyé sur site repose sur la connaissance informelle qu'a le responsable des compétences et des disponibilités de son équipe. Ce choix n'est ni formalisé, ni reproductible, ni auditable.

Ces quatre limites se renforcent mutuellement : l'absence de traçabilité empêche l'analyse, qui empêche à son tour le pilotage.

## 1.5 Problématique et objectifs

La problématique du projet se formule ainsi : **comment doter le service après-vente de MEDICANA d'un outil qui trace intégralement la maintenance corrective des respirateurs d'anesthésie, sécurise le traitement des pannes critiques et rende mesurable la performance du service ?**

Nous en déduisons cinq objectifs.

- **Tracer** l'intégralité du cycle de vie de chaque intervention, depuis la déclaration de la panne jusqu'à l'archivage du rapport signé.
- **Sécuriser** le traitement des pannes critiques : lorsqu'un patient est connecté à l'appareil, la demande d'intervention doit être immédiatement portée au niveau de priorité maximal, affectée et notifiée.
- **Garantir** la qualité de la remise en service par une check-list de clôture dont la validation intégrale conditionne la fermeture de l'intervention.
- **Mesurer** la performance du service au moyen d'indicateurs calculés directement sur les données de production.
- **Objectiver** l'affectation des ingénieurs au moyen d'un moteur de décision fondé sur les compétences, la zone géographique et la charge de travail.

## 1.6 Périmètre du projet

Le projet cible exclusivement la **maintenance corrective**, c'est-à-dire les interventions déclenchées par une panne. Ce choix de périmètre découle directement de l'analyse de l'existant : c'est sur la maintenance corrective que porte l'essentiel de l'activité du SAV, et c'est là que l'absence de traçabilité pose le problème le plus aigu.

Le tableau 1 récapitule les fonctions incluses et exclues de la première version.

**Tableau 1 - Périmètre fonctionnel de la version 1**

| Inclus | Exclu de la version 1 |
|---|---|
| Maintenance corrective | Maintenance préventive planifiée |
| Gestion du parc de respirateurs | Facturation et comptabilité |
| Gestion des interventions et de leur workflow | Gestion des ressources humaines |
| Gestion des pièces détachées | Application mobile native |
| Indicateurs et informatique décisionnelle | |
| Notifications temps réel | |
| Rapports au format PDF | |
| Gestion des utilisateurs et des droits | |

L'exclusion de l'application mobile native mérite une précision : la déclaration d'une panne par le personnel hospitalier passe par une interface de bureau simplifiée, et non par une application installée sur téléphone.

## 1.7 Conduite du projet

Nous conduisons le projet de façon incrémentale, en dix phases numérotées de 0 à 9. Chaque phase produit un incrément démontrable et validable indépendamment des suivantes, ce qui permet de présenter régulièrement l'avancement et de réorienter le travail sans remettre en cause l'existant.

Le tableau 2 présente ce découpage.

**Tableau 2 - Découpage du projet en phases**

| Phase | Intitulé | Contenu principal |
|---|---|---|
| 0 | Fondations et documentation | Analyse fonctionnelle, cahier des charges, UML, modèle de données, architecture |
| 1 | Socle technique | Solution en sept projets, entités du domaine, contexte de persistance, migration initiale, jeu de données de démonstration |
| 2 | Backend métier | Entrepôts et unité de travail, objets de transfert, mappage, validation, journalisation, premiers services |
| 3 | Sécurité et utilisateurs | Authentification, hachage, autorisation action par action, gestion des comptes, historique des connexions |
| 4 | Interface graphique | Connexion, coquille de navigation filtrée par rôle, tableau de bord, paramètres, administration des comptes |
| 5 | Modules métier | Parc, interventions, workflow, pièces, rapports |
| 6 | Fonctions avancées | QR Code, rapports PDF, check-list, blocage des appareils hors service, indicateurs graphiques |
| 7 | Notifications temps réel | Serveur de notifications, client applicatif, panneau de notifications |
| 8 | Affectation automatique | Moteur d'affectation, compétences et zones, tests unitaires |
| 9 | Qualité et livraison | Tests, documentation, gestion des exceptions, empaquetage et déploiement |

La figure 3 traduit ce découpage en planning.

`[Figure 3 - Diagramme de Gantt du projet]`
`[À COMPLÉTER : dates de début et de fin du stage, puis répartition des dix phases sur cette période]`

## 1.8 Conclusion

Ce chapitre a établi que la gestion actuelle du SAV de MEDICANA ne permet ni de tracer la maintenance des respirateurs d'anesthésie, ni d'en analyser les défaillances, ni d'en piloter la performance. La criticité de ces équipements donne à ces manques une portée qui dépasse la simple efficacité administrative. Nous avons formulé la problématique, fixé cinq objectifs, délimité le périmètre à la maintenance corrective et arrêté une conduite de projet en dix phases incrémentales. Le chapitre suivant traduit ces objectifs en besoins spécifiés et traçables.

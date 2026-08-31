# Introduction générale

## Contexte

Le respirateur d'anesthésie assure la ventilation du patient et l'administration des gaz anesthésiques pendant une intervention chirurgicale. Il appartient à la catégorie des dispositifs médicaux de maintien des fonctions vitales : sa défaillance en cours d'opération engage directement la sécurité du patient. La maintenance de ces équipements ne relève donc pas seulement de la bonne gestion d'un parc technique, elle participe de la sécurité des soins.

MEDICANA, distributeur officiel des équipements Datex-Ohmeda, assure le service après-vente des respirateurs d'anesthésie installés dans les blocs opératoires de ses hôpitaux clients. Cette activité repose aujourd'hui sur des fichiers bureautiques et des documents papier. Ce mode de gestion ne permet ni de reconstituer de façon fiable l'historique d'un appareil, ni d'analyser les défaillances récurrentes, ni de mesurer la performance du service.

## Objectifs

Le projet vise à doter le service après-vente d'une application de gestion de maintenance assistée par ordinateur (GMAO) dédiée à la maintenance corrective de ces respirateurs. Nous poursuivons cinq objectifs : tracer intégralement le cycle de vie de chaque intervention, sécuriser le traitement des pannes survenant alors qu'un patient est connecté à l'appareil, garantir la qualité de la remise en service par une check-list de clôture obligatoire, rendre mesurable la performance du service au moyen d'indicateurs calculés sur les données de production, et objectiver l'affectation des ingénieurs.

Le périmètre est délibérément limité à la maintenance corrective. La maintenance préventive planifiée, la facturation, la gestion des ressources humaines et le développement d'une application mobile native sont exclus de cette première version.

## Méthodologie

Nous conduisons le projet de manière incrémentale, en dix phases numérotées de 0 à 9, chacune produisant un livrable démontrable et validable indépendamment des suivantes.

La solution est bâtie sur une architecture en couches de type Clean Architecture, organisée en sept projets et gouvernée par une règle de dépendance stricte : le cœur métier ne dépend d'aucune technologie. L'application de bureau est développée en C# sur la plateforme .NET 10 avec la technologie WPF, la persistance repose sur Entity Framework Core et une base SQLite embarquée, et les notifications temps réel sont assurées par un serveur Node.js autonome. Les règles métier les plus sensibles font l'objet de tests unitaires.

## Résultats

L'application réalisée couvre huit modules : authentification et gestion des comptes, parc de respirateurs, interventions et workflow, pièces détachées, rapports au format PDF, tableau de bord et statistiques, notifications temps réel et affectation automatique. Sur les vingt exigences fonctionnelles spécifiées, treize sont pleinement satisfaites et cinq le sont partiellement. Les fonctions portant le cœur du métier sont opérationnelles ; les manques concernent principalement des fonctions d'ergonomie et de confort. Le bilan détaillé de ces résultats et leur analyse critique figurent au chapitre 4 et dans la conclusion générale.

## Structure du mémoire

Ce mémoire comporte quatre chapitres.

Le **premier chapitre** présente le contexte général du projet : l'organisme d'accueil, la criticité des respirateurs d'anesthésie, l'analyse critique de la gestion actuelle du service après-vente, la problématique qui en découle, le périmètre retenu et la conduite du projet.

Le **deuxième chapitre** traduit ces objectifs en besoins spécifiés : acteurs, besoins fonctionnels déclinés en exigences traçables, besoins non fonctionnels, cas d'utilisation détaillés et règles de gestion.

Le **troisième chapitre** expose la conception de la solution : l'architecture en couches et sa justification, les décisions de conception structurantes, les patrons appliqués, le modèle du domaine, le modèle de données, les comportements dynamiques et la conception du contrôle d'accès.

Le **quatrième chapitre** présente la réalisation module par module, les mécanismes centraux avec leur code, la campagne de tests, les conditions de déploiement et le bilan des exigences.

La **conclusion générale** récapitule les contributions, porte une critique du travail accompli et ouvre sur les évolutions envisageables.

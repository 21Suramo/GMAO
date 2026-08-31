# Page de garde

`[LOGO UM6SS / ESGB]`  `[LOGO MEDICANA — À FOURNIR]`

**Conception et réalisation d'une application de gestion de maintenance assistée par ordinateur pour les respirateurs d'anesthésie Datex-Ohmeda**

`[À VALIDER : intitulé exact du sujet tel que déposé]`

Cycle Ingénieur : Génie Biomédical

Année universitaire `[À COMPLÉTER : 20XX-20XX]`

**Mémoire de projet de fin d'études**

Présenté par

`[À COMPLÉTER : nom et prénom de l'élève ingénieur]`

Soutenue publiquement le `[À COMPLÉTER : date]`, devant le jury composé de :

- Pr. `[À COMPLÉTER]` — Président(e)
- Pr. `[À COMPLÉTER]` — Examinateur
- Pr. `[À COMPLÉTER]` — Rapporteur
- Pr. `[À COMPLÉTER]` — Encadrant(e)
- M. `[À COMPLÉTER]` — Invité

---

# Dédicaces

`[À RÉDIGER PAR L'AUTEUR — texte personnel, une page]`

---

# Remerciements

`[À PERSONNALISER : compléter les noms et les fonctions]`

Je tiens à exprimer ma reconnaissance à toutes les personnes qui ont rendu ce travail possible.

Mes remerciements s'adressent d'abord à `[À COMPLÉTER : Pr. Nom Prénom]`, mon encadrant académique à l'École Supérieure de Génie Biomédical, pour la rigueur de son suivi, la pertinence de ses remarques et la disponibilité dont il a fait preuve tout au long de ce projet.

Je remercie également `[À COMPLÉTER : M. Nom Prénom, fonction]`, mon encadrant au sein de MEDICANA, qui m'a accueilli dans son service, m'a fait découvrir les réalités de la maintenance biomédicale sur le terrain et m'a donné accès aux informations nécessaires à la conduite de ce travail.

Ma gratitude va à l'ensemble de l'équipe du service après-vente de MEDICANA, dont la disponibilité et l'expérience ont directement nourri l'analyse fonctionnelle de ce projet.

J'adresse mes remerciements aux membres du jury, `[À COMPLÉTER : noms]`, qui me font l'honneur d'évaluer ce travail.

Je remercie enfin le corps professoral de l'École Supérieure de Génie Biomédical de l'Université Mohammed VI des Sciences de la Santé pour la formation reçue durant ce cycle d'ingénieur.

---

# Résumé

Le respirateur d'anesthésie est un dispositif médical de maintien des fonctions vitales dont la défaillance en cours d'intervention chirurgicale engage la sécurité du patient. MEDICANA, distributeur officiel des équipements Datex-Ohmeda, assure la maintenance de ces appareils dans les blocs opératoires de ses hôpitaux clients. Cette activité repose sur des fichiers bureautiques et des documents papier, un mode de gestion qui ne permet ni de reconstituer l'historique d'un appareil, ni d'analyser les défaillances récurrentes, ni de mesurer la performance du service.

Ce projet de fin d'études répond à cette situation par la conception et la réalisation d'une application de bureau de gestion de maintenance assistée par ordinateur, dédiée à la maintenance corrective de ces respirateurs. Nous avons conduit le travail en dix phases incrémentales, depuis l'analyse fonctionnelle jusqu'au déploiement. La solution repose sur une architecture en couches de type Clean Architecture organisée en sept projets, développée en C# sur la plateforme .NET 10 avec la technologie WPF, une persistance assurée par Entity Framework Core sur une base SQLite embarquée, et un serveur Node.js autonome pour les notifications temps réel.

L'application couvre huit modules : authentification et gestion des comptes avec contrôle d'accès à granularité fine, parc de respirateurs et QR Code, interventions et workflow à dix états, pièces détachées et alertes de stock, rapports au format PDF, tableau de bord et statistiques, notifications temps réel, et affectation automatique des ingénieurs. Trois mécanismes portent les objectifs du projet : le passage automatique en priorité maximale d'une panne survenant alors qu'un patient est connecté à l'appareil, la check-list de clôture dont la validation intégrale conditionne la fermeture d'une intervention, et un moteur d'affectation déterministe pondérant compétences, zone géographique et charge de travail. Sur vingt exigences fonctionnelles spécifiées, treize sont pleinement satisfaites et cinq le sont partiellement. Trente-sept tests unitaires couvrent la logique métier.

**Mots-clés :** GMAO, maintenance corrective, respirateur d'anesthésie, Datex-Ohmeda, génie biomédical, Clean Architecture, .NET, indicateurs de maintenance.

---

# Abstract

The anaesthesia ventilator is a life-support medical device whose failure during surgery directly threatens patient safety. MEDICANA, the official distributor of Datex-Ohmeda equipment, maintains these devices in the operating theatres of its client hospitals. This activity currently relies on office files and paper documents, a method that allows neither reliable reconstruction of a device's history, nor analysis of recurring failures, nor measurement of service performance.

This final-year project addresses this situation through the design and implementation of a desktop computerised maintenance management system dedicated to the corrective maintenance of these ventilators. The work was conducted in ten incremental phases, from functional analysis to deployment. The solution rests on a Clean Architecture layered design organised into seven projects, developed in C# on the .NET 10 platform using WPF, with persistence provided by Entity Framework Core over an embedded SQLite database, and a standalone Node.js server for real-time notifications.

The application covers eight modules: authentication and account management with fine-grained access control, ventilator fleet management with QR codes, work orders and a ten-state workflow, spare parts and stock alerts, PDF reports, dashboard and statistics, real-time notifications, and automatic engineer assignment. Three mechanisms carry the project's objectives: the automatic escalation to maximum priority of a failure occurring while a patient is connected to the device, the closing checklist whose full validation is required before a work order can be closed, and a deterministic assignment engine weighting skills, geographical zone and workload. Of twenty specified functional requirements, thirteen are fully met and five partially. Thirty-seven unit tests cover the business logic.

**Keywords:** CMMS, corrective maintenance, anaesthesia ventilator, Datex-Ohmeda, biomedical engineering, Clean Architecture, .NET, maintenance indicators.

---

# ملخص

يُعدّ جهاز التخدير والتنفس الاصطناعي من الأجهزة الطبية الداعمة للوظائف الحيوية، إذ إنّ عطبه أثناء عملية جراحية يمسّ مباشرة بسلامة المريض. تتولى شركة MEDICANA، الموزّع الرسمي لتجهيزات Datex-Ohmeda، صيانة هذه الأجهزة داخل قاعات العمليات بالمستشفيات الزبناء. وتعتمد هذه المصلحة حالياً على ملفات مكتبية ووثائق ورقية، وهي طريقة لا تتيح إعادة تكوين تاريخ الجهاز بشكل موثوق، ولا تحليل الأعطال المتكرّرة، ولا قياس أداء المصلحة.

يستجيب مشروع نهاية الدراسة هذا لهذه الوضعية من خلال تصميم وإنجاز تطبيق مكتبي لتدبير الصيانة بمساعدة الحاسوب، مخصّص للصيانة التصحيحية لهذه الأجهزة. أُنجز العمل عبر عشر مراحل تدريجية، من التحليل الوظيفي إلى النشر. ويرتكز الحل على بنية طبقية من نوع Clean Architecture موزّعة على سبعة مشاريع، مطوّرة بلغة C# على منصة ‎.NET 10 بتقنية WPF، مع تخزين للبيانات عبر Entity Framework Core على قاعدة SQLite مدمجة، وخادم مستقل بلغة Node.js للإشعارات الآنية.

يغطّي التطبيق ثمانية وحدات: المصادقة وتدبير الحسابات مع تحكّم دقيق في الصلاحيات، وتدبير أسطول الأجهزة مع رمز الاستجابة السريعة، وتدبير التدخّلات وفق مسار من عشر حالات، وقطع الغيار وتنبيهات المخزون، والتقارير بصيغة PDF، ولوحة القيادة والإحصائيات، والإشعارات الآنية، والإسناد التلقائي للمهندسين. وتحمل ثلاث آليات أهداف المشروع: الرفع التلقائي للأولوية إلى أقصاها عند وقوع عطب والمريض موصول بالجهاز، وقائمة التحقّق عند الإغلاق التي يشترط استيفاؤها كاملةً لإقفال التدخّل، ومحرّك إسناد حتمي يوازن بين الكفاءات والنطاق الجغرافي وعبء العمل. ومن أصل عشرين متطلّباً وظيفياً محدّداً، تحقّق ثلاثة عشر بالكامل وخمسة جزئياً. ويغطّي سبعة وثلاثون اختباراً وحدوياً المنطق المهني للتطبيق.

**الكلمات المفتاحية:** تدبير الصيانة بمساعدة الحاسوب، الصيانة التصحيحية، جهاز التخدير والتنفس الاصطناعي، Datex-Ohmeda، الهندسة الطبية الحيوية، Clean Architecture، ‎.NET، مؤشرات الصيانة.

---

# Liste des abréviations

| Sigle | Signification |
|---|---|
| API | Interface de programmation applicative (*Application Programming Interface*) |
| BF | Besoin fonctionnel |
| CMMS | *Computerised Maintenance Management System* |
| DI | Demande d'intervention |
| EF | Exigence fonctionnelle |
| ESGB | École Supérieure de Génie Biomédical |
| GMAO | Gestion de maintenance assistée par ordinateur |
| MTBF | Temps moyen entre pannes (*Mean Time Between Failures*) |
| MTTR | Temps moyen de résolution (*Mean Time To Repair*) |
| MVVM | Modèle-Vue-ModèleDeVue (*Model-View-ViewModel*) |
| ORM | Correspondance objet-relationnel (*Object-Relational Mapping*) |
| PDF | *Portable Document Format* |
| PFE | Projet de fin d'études |
| QR | *Quick Response* (code-barres bidimensionnel) |
| RBAC | Contrôle d'accès fondé sur les rôles (*Role-Based Access Control*) |
| REST | *Representational State Transfer* |
| RG | Règle de gestion |
| SAV | Service après-vente |
| SLA | Engagement de niveau de service (*Service Level Agreement*) |
| SQL | *Structured Query Language* |
| UM6SS | Université Mohammed VI des Sciences de la Santé |
| UML | Langage de modélisation unifié (*Unified Modeling Language*) |
| WPF | *Windows Presentation Foundation* |

Le sigle **DI** est employé dans ce mémoire au seul sens de *demande d'intervention*. L'injection de dépendances, souvent abrégée de la même manière, est systématiquement écrite en toutes lettres.

---

# Liste des tableaux

| | Intitulé | Page |
|---|---|---|
| Tableau 1 | Périmètre fonctionnel de la version 1 | |
| Tableau 2 | Découpage du projet en phases | |
| Tableau 3 | Acteurs du système et leurs objectifs | |
| Tableau 4 | Exigences fonctionnelles | |
| Tableau 5 | Besoins non fonctionnels | |
| Tableau 6 | Règles de gestion | |
| Tableau 7 | Responsabilité des projets de la solution | |
| Tableau 8 | Entités du modèle de données | |
| Tableau 9 | Énumérations du domaine | |
| Tableau 10 | Matrice des rôles et des permissions | |
| Tableau 11 | Bibliothèques utilisées | |
| Tableau 12 | Couverture des tests unitaires | |
| Tableau 13 | Emplacement des données | |
| Tableau 14 | Bilan de réalisation des exigences fonctionnelles | |

---

# Liste des figures

| | Intitulé | Page |
|---|---|---|
| Figure 1 | Organigramme de MEDICANA | |
| Figure 2 | Respirateur d'anesthésie Datex-Ohmeda | |
| Figure 3 | Diagramme de Gantt du projet | |
| Figure 4 | Diagramme de cas d'utilisation général | |
| Figure 5 | Cas d'utilisation détaillés : relations « include » et « extend » | |
| Figure 6 | Architecture en couches de la solution | |
| Figure 7 | Diagramme de classes du domaine | |
| Figure 8 | Diagramme entité-association | |
| Figure 9 | Diagramme d'états d'une intervention | |
| Figure 10 | Diagramme de séquence : déclaration d'une intervention critique | |
| Figure 11 | Diagramme de séquence : clôture avec check-list | |
| Figure 12 | Fenêtre de connexion | |
| Figure 13 | Écran d'administration des comptes | |
| Figure 14 | Écran de gestion du parc | |
| Figure 15 | QR Code généré pour un respirateur | |
| Figure 16 | Écran des interventions | |
| Figure 17 | Application de la règle RG-01 à la création d'une intervention | |
| Figure 18 | Écran des pièces détachées | |
| Figure 19 | Rapport d'intervention généré | |
| Figure 20 | Tableau de bord | |
| Figure 21 | Écran de statistiques | |
| Figure 22 | Panneau de notifications | |
| Figure 23 | Moteur d'affectation : calcul du score et sélection du candidat | |
| Figure 24 | Exécution de la campagne de tests | |

Les numéros de page sont générés automatiquement lors de la mise en forme finale sous traitement de texte.

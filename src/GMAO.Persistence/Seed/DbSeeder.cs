using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Pieces;
using GMAO.Domain.Entities.Planning;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Enums;
using GMAO.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GMAO.Persistence.Seed;

/// <summary>
/// Initialise la base avec des données de référence et de démonstration.
/// Idempotent : ne réinsère pas si les données existent déjà.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Applique les migrations en attente puis insère les données initiales.
    /// </summary>
    /// <param name="context">Contexte EF Core.</param>
    /// <param name="adminPasswordHash">Empreinte BCrypt du mot de passe administrateur par défaut.</param>
    public static async Task SeedAsync(AppDbContext context, string adminPasswordHash)
    {
        await context.Database.MigrateAsync();

        await SeedUtilisateursAsync(context, adminPasswordHash);
        await SeedModelesAsync(context);
        await SeedSymptomesEtPannesAsync(context);
        await SeedPiecesAsync(context);
        await SeedHopitalDemoAsync(context);
        await SeedIngenieursAsync(context);

        // Persiste les données de référence afin de disposer de leurs identifiants.
        await context.SaveChangesAsync();

        // Associations panne ↔ pièces (analyse des défaillances).
        await SeedAssociationsPannePieceAsync(context);
        // Compétences des ingénieurs (affectation automatique).
        await SeedCompetencesAsync(context);
        await context.SaveChangesAsync();

        // Liaison comptes ↔ ingénieurs (permet la vue personnelle du tableau de bord).
        await SeedLiaisonIngenieursAsync(context);
        await context.SaveChangesAsync();

        // Le parc de respirateurs dépend des modèles et des blocs déjà persistés.
        await SeedRespirateursAsync(context);
        await context.SaveChangesAsync();

        // Les interventions dépendent des respirateurs et ingénieurs persistés.
        await SeedInterventionsAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUtilisateursAsync(AppDbContext context, string motDePasseHash)
    {
        if (await context.Utilisateurs.AnyAsync()) return;

        // Comptes de démonstration : tous partagent le mot de passe par défaut (« Admin@123 »).
        context.Utilisateurs.AddRange(
            new Utilisateur
            {
                Login = "admin", MotDePasseHash = motDePasseHash,
                Nom = "Système", Prenom = "Administrateur", Email = "admin@medicana.local",
                Role = RoleType.Administrateur, Actif = true
            },
            new Utilisateur
            {
                Login = "responsable", MotDePasseHash = motDePasseHash,
                Nom = "Alaoui", Prenom = "Karim", Email = "k.alaoui@medicana.local",
                Telephone = "+212 6 61 00 00 01",
                Role = RoleType.ResponsableSAV, Actif = true
            },
            new Utilisateur
            {
                Login = "y.elamrani", MotDePasseHash = motDePasseHash,
                Nom = "El Amrani", Prenom = "Youssef", Email = "y.elamrani@medicana.local",
                Telephone = "+212 6 61 00 00 02",
                Role = RoleType.Ingenieur, Actif = true
            },
            new Utilisateur
            {
                Login = "s.bennani", MotDePasseHash = motDePasseHash,
                Nom = "Bennani", Prenom = "Sara", Email = "s.bennani@medicana.local",
                Telephone = "+212 6 61 00 00 03",
                Role = RoleType.Ingenieur, Actif = true
            },
            new Utilisateur
            {
                Login = "technicien", MotDePasseHash = motDePasseHash,
                Nom = "Idrissi", Prenom = "Omar", Email = "o.idrissi@medicana.local",
                Telephone = "+212 6 61 00 00 04",
                Role = RoleType.Technicien, Actif = true
            });
    }

    private static async Task SeedLiaisonIngenieursAsync(AppDbContext context)
    {
        var ingenieurs = await context.Ingenieurs.Where(i => i.UtilisateurId == null).ToListAsync();
        if (ingenieurs.Count == 0) return;

        var utilisateurs = await context.Utilisateurs.ToListAsync();
        foreach (var ingenieur in ingenieurs)
        {
            // Correspondance par e-mail entre le compte de connexion et la fiche ingénieur.
            var compte = utilisateurs.FirstOrDefault(u =>
                u.Email != null && ingenieur.Email != null &&
                string.Equals(u.Email, ingenieur.Email, StringComparison.OrdinalIgnoreCase));
            if (compte is not null)
                ingenieur.UtilisateurId = compte.Id;
        }
    }

    private static async Task SeedModelesAsync(AppDbContext context)
    {
        if (await context.ModelesRespirateur.AnyAsync()) return;

        context.ModelesRespirateur.AddRange(
            new ModeleRespirateur { Nom = "Aespire", Gamme = "Aespire 7100/7900", Description = "Station d'anesthésie Datex-Ohmeda Aespire" },
            new ModeleRespirateur { Nom = "Avance", Gamme = "Avance CS2", Description = "Station d'anesthésie Datex-Ohmeda Avance" },
            new ModeleRespirateur { Nom = "Aisys", Gamme = "Aisys CS2", Description = "Station d'anesthésie Datex-Ohmeda Aisys" }
        );
    }

    private static async Task SeedSymptomesEtPannesAsync(AppDbContext context)
    {
        if (!await context.Symptomes.AnyAsync())
        {
            context.Symptomes.AddRange(
                new Symptome { Libelle = "Fuite détectée", Description = "Perte d'étanchéité du circuit patient" },
                new Symptome { Libelle = "Erreur calibration O2", Description = "Échec de la calibration de la cellule O2" },
                new Symptome { Libelle = "Alarme débit", Description = "Alarme persistante sur le capteur de débit" },
                new Symptome { Libelle = "Écran figé", Description = "Interface utilisateur non réactive" },
                new Symptome { Libelle = "Batterie défaillante", Description = "Autonomie batterie insuffisante" }
            );
        }

        if (!await context.Pannes.AnyAsync())
        {
            context.Pannes.AddRange(
                new Panne { Code = "PN-FUITE", Libelle = "Fuite circuit", Description = "Défaut d'étanchéité (valve, joint, capteur)" },
                new Panne { Code = "PN-O2", Libelle = "Défaut calibration O2", Description = "Cellule O2 usée ou défaillante" },
                new Panne { Code = "PN-DEBIT", Libelle = "Capteur de débit", Description = "Capteur de débit défectueux" },
                new Panne { Code = "PN-BATT", Libelle = "Batterie", Description = "Batterie en fin de vie" }
            );
        }
    }

    private static async Task SeedPiecesAsync(AppDbContext context)
    {
        if (await context.Pieces.AnyAsync()) return;

        context.Pieces.AddRange(
            new Piece { Reference = "1503-3856-000", Nom = "Valve expiratoire", Compatible = "Aespire, Avance", Stock = 8, StockMinimum = 2, Emplacement = "A1-03", Prix = 320m },
            new Piece { Reference = "1407-3201-000", Nom = "Cellule O2", Compatible = "Aespire, Avance, Aisys", Stock = 5, StockMinimum = 3, Emplacement = "A2-01", Prix = 95m },
            new Piece { Reference = "1500-3357-000", Nom = "Capteur de débit", Compatible = "Avance, Aisys", Stock = 3, StockMinimum = 2, Emplacement = "A2-04", Prix = 410m },
            new Piece { Reference = "1006-8939-000", Nom = "Joint d'étanchéité circuit", Compatible = "Aespire, Avance, Aisys", Stock = 25, StockMinimum = 10, Emplacement = "B1-07", Prix = 12m },
            new Piece { Reference = "BATT-LI-7900", Nom = "Batterie Li-Ion", Compatible = "Aespire, Avance", Stock = 2, StockMinimum = 2, Emplacement = "C3-02", Prix = 180m }
        );
    }

    private static async Task SeedHopitalDemoAsync(AppDbContext context)
    {
        if (await context.Hopitaux.AnyAsync()) return;

        var hopital = new Hopital
        {
            Nom = "CHU Ibn Rochd",
            Ville = "Casablanca",
            Adresse = "1 rue des Hôpitaux",
            Contact = "Service biomédical",
            Telephone = "+212 5 22 00 00 00",
            Services =
            [
                new Service
                {
                    Nom = "Bloc central",
                    BlocsOperatoires =
                    [
                        new BlocOperatoire { Nom = "Bloc 1" },
                        new BlocOperatoire { Nom = "Bloc 2" }
                    ]
                }
            ]
        };

        context.Hopitaux.Add(hopital);
    }

    private static async Task SeedIngenieursAsync(AppDbContext context)
    {
        if (await context.Ingenieurs.AnyAsync()) return;

        context.Ingenieurs.AddRange(
            new Ingenieur { Nom = "El Amrani", Prenom = "Youssef", Zone = "Casablanca", Email = "y.elamrani@medicana.local", Disponible = true },
            new Ingenieur { Nom = "Bennani", Prenom = "Sara", Zone = "Rabat", Email = "s.bennani@medicana.local", Disponible = true }
        );
    }

    private static async Task SeedRespirateursAsync(AppDbContext context)
    {
        if (await context.Respirateurs.AnyAsync()) return;

        var modeles = await context.ModelesRespirateur.ToListAsync();
        var blocs = await context.BlocsOperatoires.OrderBy(b => b.Id).ToListAsync();
        if (modeles.Count == 0 || blocs.Count == 0) return;

        ModeleRespirateur Modele(string nom) => modeles.First(m => m.Nom == nom);
        BlocOperatoire Bloc(int index) => blocs[index % blocs.Count];

        context.Respirateurs.AddRange(
            new Respirateur
            {
                NumeroSerie = "AESP-2021-0142", CodeInterne = "MED-RESP-001",
                ModeleRespirateurId = Modele("Aespire").Id, BlocOperatoireId = Bloc(0).Id,
                VersionLogicielle = "7.4", VersionMaterielle = "Rev C",
                Etat = EtatRespirateur.EnService, DateMiseEnService = new DateTime(2021, 3, 15),
                SousContrat = true, NumeroContrat = "CT-2021-008", DateFinGarantie = new DateTime(2027, 3, 15)
            },
            new Respirateur
            {
                NumeroSerie = "AVAN-2022-0098", CodeInterne = "MED-RESP-002",
                ModeleRespirateurId = Modele("Avance").Id, BlocOperatoireId = Bloc(0).Id,
                VersionLogicielle = "11.1", VersionMaterielle = "Rev A",
                Etat = EtatRespirateur.EnService, DateMiseEnService = new DateTime(2022, 6, 1),
                SousContrat = true, NumeroContrat = "CT-2022-014", DateFinGarantie = new DateTime(2028, 6, 1)
            },
            new Respirateur
            {
                NumeroSerie = "AISY-2020-0033", CodeInterne = "MED-RESP-003",
                ModeleRespirateurId = Modele("Aisys").Id, BlocOperatoireId = Bloc(1).Id,
                VersionLogicielle = "9.2", VersionMaterielle = "Rev B",
                Etat = EtatRespirateur.EnMaintenance, DateMiseEnService = new DateTime(2020, 11, 20),
                SousContrat = true, NumeroContrat = "CT-2020-021", DateFinGarantie = new DateTime(2026, 11, 20)
            },
            new Respirateur
            {
                NumeroSerie = "AESP-2019-0210", CodeInterne = "MED-RESP-004",
                ModeleRespirateurId = Modele("Aespire").Id, BlocOperatoireId = Bloc(1).Id,
                VersionLogicielle = "7.2", VersionMaterielle = "Rev B",
                Etat = EtatRespirateur.HorsService, DateMiseEnService = new DateTime(2019, 2, 10),
                SousContrat = false, DateFinGarantie = new DateTime(2024, 2, 10),
                MotifHorsService = "Fuite persistante du circuit patient, pièce en attente",
                DateHorsService = DateTime.UtcNow.AddDays(-5), AuteurHorsService = "Administrateur Système"
            }
        );
    }

    private static async Task SeedCompetencesAsync(AppDbContext context)
    {
        if (await context.Competences.AnyAsync()) return;

        var modeles = await context.ModelesRespirateur.ToListAsync();
        var ingenieurs = await context.Ingenieurs.Include(i => i.Competences).OrderBy(i => i.Id).ToListAsync();
        if (modeles.Count == 0 || ingenieurs.Count < 2) return;

        ModeleRespirateur M(string nom) => modeles.First(m => m.Nom == nom);

        var compAespire = new Competence { Libelle = "Maîtrise Aespire", ModeleRespirateurId = M("Aespire").Id };
        var compAvance = new Competence { Libelle = "Maîtrise Avance", ModeleRespirateurId = M("Avance").Id };
        var compAisys = new Competence { Libelle = "Maîtrise Aisys", ModeleRespirateurId = M("Aisys").Id };
        context.Competences.AddRange(compAespire, compAvance, compAisys);

        // El Amrani (Casablanca) : Aespire + Avance — Bennani (Rabat) : Aisys + Avance.
        ingenieurs[0].Competences.Add(compAespire);
        ingenieurs[0].Competences.Add(compAvance);
        ingenieurs[1].Competences.Add(compAisys);
        ingenieurs[1].Competences.Add(compAvance);
    }

    private static async Task SeedAssociationsPannePieceAsync(AppDbContext context)
    {
        if (await context.PannesPieces.AnyAsync()) return;

        var pannes = await context.Pannes.ToListAsync();
        var pieces = await context.Pieces.ToListAsync();
        if (pannes.Count == 0 || pieces.Count == 0) return;

        Panne? P(string code) => pannes.FirstOrDefault(x => x.Code == code);
        Piece? R(string reference) => pieces.FirstOrDefault(x => x.Reference == reference);

        void Lier(string codePanne, string refPiece, int probabilite)
        {
            var panne = P(codePanne);
            var piece = R(refPiece);
            if (panne is not null && piece is not null)
                context.PannesPieces.Add(new PanneePiece { PanneId = panne.Id, PieceId = piece.Id, Probabilite = probabilite });
        }

        // Erreur fuite → valve expiratoire, joint, capteur débit
        Lier("PN-FUITE", "1503-3856-000", 60);
        Lier("PN-FUITE", "1006-8939-000", 30);
        Lier("PN-FUITE", "1500-3357-000", 10);
        // Défaut O2 → cellule O2
        Lier("PN-O2", "1407-3201-000", 90);
        // Défaut débit → capteur de débit
        Lier("PN-DEBIT", "1500-3357-000", 85);
        // Batterie → batterie Li-Ion
        Lier("PN-BATT", "BATT-LI-7900", 95);
    }

    private static async Task SeedInterventionsAsync(AppDbContext context)
    {
        if (await context.Interventions.AnyAsync()) return;

        var respirateurs = await context.Respirateurs
            .Include(r => r.BlocOperatoire!).ThenInclude(b => b.Service)
            .OrderBy(r => r.Id).ToListAsync();
        var ingenieurs = await context.Ingenieurs.OrderBy(i => i.Id).ToListAsync();
        if (respirateurs.Count < 4 || ingenieurs.Count < 2) return;

        var pieces = await context.Pieces.ToListAsync();
        int? HopitalDe(Respirateur r) => r.BlocOperatoire?.Service.HopitalId;
        var annee = DateTime.UtcNow.Year;
        var sequence = 0;

        Intervention Creer(Respirateur r, EtatIntervention etat, Priorite priorite, string description,
            int? ingenieurIndex, bool patientConnecte = false, bool checkListComplete = false, int joursAvant = 0,
            decimal mainOeuvre = 0, int tempsDeplacement = 0, int tempsReparation = 0,
            string? diagnostic = null, string? pieceReference = null, int pieceQuantite = 0)
        {
            sequence++;
            var inter = new Intervention
            {
                NumeroDI = $"DI-{annee}-{sequence:D4}",
                Date = DateTime.UtcNow.AddDays(-joursAvant),
                Description = description,
                Diagnostic = diagnostic,
                RespirateurId = r.Id,
                HopitalId = HopitalDe(r)!.Value,
                IngenieurId = ingenieurIndex.HasValue ? ingenieurs[ingenieurIndex.Value].Id : null,
                Etat = etat,
                Priorite = priorite,
                PatientConnecte = patientConnecte,
                Urgence = patientConnecte || priorite == Priorite.Haute,
                MainOeuvre = mainOeuvre,
                TempsDeplacement = tempsDeplacement,
                TempsReparation = tempsReparation,
                DateCloture = etat == EtatIntervention.Cloturee ? DateTime.UtcNow.AddDays(-joursAvant).AddHours(4) : null,
                CheckList = new CheckListCloture
                {
                    AutotestOk = checkListComplete, TestEtancheite = checkListComplete,
                    CalibrationDebit = checkListComplete, CalibrationO2 = checkListComplete,
                    Batterie = checkListComplete, Alimentation = checkListComplete,
                    Alarmes = checkListComplete, ValidationFinale = checkListComplete
                }
            };
            inter.HistoriqueEtats.Add(new HistoriqueEtatIntervention
            {
                AncienEtat = EtatIntervention.Nouvelle, NouvelEtat = etat,
                Auteur = "Seed", Commentaire = "Donnée de démonstration"
            });

            if (pieceReference is not null)
            {
                var piece = pieces.FirstOrDefault(p => p.Reference == pieceReference);
                if (piece is not null)
                    inter.PiecesUtilisees.Add(new LignePieceIntervention
                    {
                        PieceId = piece.Id, Quantite = pieceQuantite, PrixUnitaire = piece.Prix
                    });
            }
            return inter;
        }

        context.Interventions.AddRange(
            Creer(respirateurs[0], EtatIntervention.Nouvelle, Priorite.Normale, "Écran tactile peu réactif", null, joursAvant: 1),
            Creer(respirateurs[2], EtatIntervention.Diagnostic, Priorite.Haute, "Alarme de débit persistante", 0, joursAvant: 2),
            Creer(respirateurs[1], EtatIntervention.Reparation, Priorite.Haute, "Échec de la calibration O2", 1, joursAvant: 3),
            Creer(respirateurs[3], EtatIntervention.EnAttentePiece, Priorite.Haute, "Fuite du circuit patient — valve à remplacer", 0, joursAvant: 5),
            Creer(respirateurs[0], EtatIntervention.Affectee, Priorite.Critique, "Défaillance en cours d'utilisation — patient connecté", 0, patientConnecte: true),
            Creer(respirateurs[1], EtatIntervention.Cloturee, Priorite.Normale, "Remplacement batterie Li-Ion", 1,
                checkListComplete: true, joursAvant: 10, mainOeuvre: 450m, tempsDeplacement: 45, tempsReparation: 90,
                diagnostic: "Batterie en fin de vie, autonomie insuffisante. Remplacement effectué et testé.",
                pieceReference: "BATT-LI-7900", pieceQuantite: 1)
        );
    }
}

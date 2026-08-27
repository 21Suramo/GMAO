using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GMAO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriesPiece",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesPiece", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Contact = table.Column<string>(type: "TEXT", nullable: true),
                    Telephone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fournisseurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hopitaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Ville = table.Column<string>(type: "TEXT", nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", nullable: true),
                    Telephone = table.Column<string>(type: "TEXT", nullable: true),
                    Contact = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hopitaux", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelesRespirateur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Gamme = table.Column<string>(type: "TEXT", nullable: true),
                    Constructeur = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelesRespirateur", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pannes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Libelle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pannes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Symptomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Libelle = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Symptomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Actif = table.Column<bool>(type: "INTEGER", nullable: false),
                    DerniereConnexion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Compatible = table.Column<string>(type: "TEXT", nullable: true),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    StockMinimum = table.Column<int>(type: "INTEGER", nullable: false),
                    Emplacement = table.Column<string>(type: "TEXT", nullable: true),
                    Prix = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DatePeremption = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CategoriePieceId = table.Column<int>(type: "INTEGER", nullable: true),
                    FournisseurId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pieces_CategoriesPiece_CategoriePieceId",
                        column: x => x.CategoriePieceId,
                        principalTable: "CategoriesPiece",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pieces_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    HopitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Hopitaux_HopitalId",
                        column: x => x.HopitalId,
                        principalTable: "Hopitaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Competences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Libelle = table.Column<string>(type: "TEXT", nullable: false),
                    ModeleRespirateurId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Competences_ModelesRespirateur_ModeleRespirateurId",
                        column: x => x.ModeleRespirateurId,
                        principalTable: "ModelesRespirateur",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueConnexions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateConnexion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AdresseIp = table.Column<string>(type: "TEXT", nullable: true),
                    Succes = table.Column<bool>(type: "INTEGER", nullable: false),
                    Detail = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueConnexions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueConnexions_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingenieurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Zone = table.Column<string>(type: "TEXT", nullable: true),
                    Disponible = table.Column<bool>(type: "INTEGER", nullable: false),
                    UtilisateurId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingenieurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingenieurs_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Titre = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Lu = table.Column<bool>(type: "INTEGER", nullable: false),
                    DestinataireUtilisateurId = table.Column<int>(type: "INTEGER", nullable: true),
                    DestinataireId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReferenceType = table.Column<string>(type: "TEXT", nullable: true),
                    ReferenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Utilisateurs_DestinataireId",
                        column: x => x.DestinataireId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MouvementsStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PieceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    DateMouvement = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", nullable: true),
                    Auteur = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MouvementsStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MouvementsStock_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PannesPieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PanneId = table.Column<int>(type: "INTEGER", nullable: false),
                    PieceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Probabilite = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PannesPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PannesPieces_Pannes_PanneId",
                        column: x => x.PanneId,
                        principalTable: "Pannes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PannesPieces_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlocsOperatoires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlocsOperatoires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlocsOperatoires_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetenceIngenieur",
                columns: table => new
                {
                    CompetencesId = table.Column<int>(type: "INTEGER", nullable: false),
                    IngenieursId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetenceIngenieur", x => new { x.CompetencesId, x.IngenieursId });
                    table.ForeignKey(
                        name: "FK_CompetenceIngenieur_Competences_CompetencesId",
                        column: x => x.CompetencesId,
                        principalTable: "Competences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetenceIngenieur_Ingenieurs_IngenieursId",
                        column: x => x.IngenieursId,
                        principalTable: "Ingenieurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IngenieurId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conges_Ingenieurs_IngenieurId",
                        column: x => x.IngenieurId,
                        principalTable: "Ingenieurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Respirateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroSerie = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    CodeInterne = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CodeQr = table.Column<Guid>(type: "TEXT", nullable: false),
                    VersionLogicielle = table.Column<string>(type: "TEXT", nullable: true),
                    VersionMaterielle = table.Column<string>(type: "TEXT", nullable: true),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false),
                    DateMiseEnService = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NumeroContrat = table.Column<string>(type: "TEXT", nullable: true),
                    SousContrat = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateFinGarantie = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModeleRespirateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    BlocOperatoireId = table.Column<int>(type: "INTEGER", nullable: true),
                    MotifHorsService = table.Column<string>(type: "TEXT", nullable: true),
                    DateHorsService = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuteurHorsService = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respirateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Respirateurs_BlocsOperatoires_BlocOperatoireId",
                        column: x => x.BlocOperatoireId,
                        principalTable: "BlocsOperatoires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Respirateurs_ModelesRespirateur_ModeleRespirateurId",
                        column: x => x.ModeleRespirateurId,
                        principalTable: "ModelesRespirateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsTechniques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CheminFichier = table.Column<string>(type: "TEXT", nullable: false),
                    ModeleRespirateurId = table.Column<int>(type: "INTEGER", nullable: true),
                    RespirateurId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsTechniques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsTechniques_ModelesRespirateur_ModeleRespirateurId",
                        column: x => x.ModeleRespirateurId,
                        principalTable: "ModelesRespirateur",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentsTechniques_Respirateurs_RespirateurId",
                        column: x => x.RespirateurId,
                        principalTable: "Respirateurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroDI = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Diagnostic = table.Column<string>(type: "TEXT", nullable: true),
                    Cause = table.Column<string>(type: "TEXT", nullable: true),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false),
                    Priorite = table.Column<int>(type: "INTEGER", nullable: false),
                    PatientConnecte = table.Column<bool>(type: "INTEGER", nullable: false),
                    Urgence = table.Column<bool>(type: "INTEGER", nullable: false),
                    TempsDeplacement = table.Column<int>(type: "INTEGER", nullable: false),
                    TempsReparation = table.Column<int>(type: "INTEGER", nullable: false),
                    MainOeuvre = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Commentaires = table.Column<string>(type: "TEXT", nullable: true),
                    Signature = table.Column<string>(type: "TEXT", nullable: true),
                    DateCloture = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RespirateurId = table.Column<int>(type: "INTEGER", nullable: false),
                    HopitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    IngenieurId = table.Column<int>(type: "INTEGER", nullable: true),
                    PanneId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_Hopitaux_HopitalId",
                        column: x => x.HopitalId,
                        principalTable: "Hopitaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interventions_Ingenieurs_IngenieurId",
                        column: x => x.IngenieurId,
                        principalTable: "Ingenieurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Interventions_Pannes_PanneId",
                        column: x => x.PanneId,
                        principalTable: "Pannes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Interventions_Respirateurs_RespirateurId",
                        column: x => x.RespirateurId,
                        principalTable: "Respirateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CheckListsCloture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AutotestOk = table.Column<bool>(type: "INTEGER", nullable: false),
                    TestEtancheite = table.Column<bool>(type: "INTEGER", nullable: false),
                    CalibrationDebit = table.Column<bool>(type: "INTEGER", nullable: false),
                    CalibrationO2 = table.Column<bool>(type: "INTEGER", nullable: false),
                    Batterie = table.Column<bool>(type: "INTEGER", nullable: false),
                    Alimentation = table.Column<bool>(type: "INTEGER", nullable: false),
                    Alarmes = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidationFinale = table.Column<bool>(type: "INTEGER", nullable: false),
                    Commentaire = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckListsCloture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckListsCloture_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriquesEtatIntervention",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AncienEtat = table.Column<int>(type: "INTEGER", nullable: false),
                    NouvelEtat = table.Column<int>(type: "INTEGER", nullable: false),
                    DateChangement = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Auteur = table.Column<string>(type: "TEXT", nullable: true),
                    Commentaire = table.Column<string>(type: "TEXT", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriquesEtatIntervention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriquesEtatIntervention_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterventionSymptome",
                columns: table => new
                {
                    InterventionsId = table.Column<int>(type: "INTEGER", nullable: false),
                    SymptomesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionSymptome", x => new { x.InterventionsId, x.SymptomesId });
                    table.ForeignKey(
                        name: "FK_InterventionSymptome_Interventions_InterventionsId",
                        column: x => x.InterventionsId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionSymptome_Symptomes_SymptomesId",
                        column: x => x.SymptomesId,
                        principalTable: "Symptomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LignesPieceIntervention",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PieceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesPieceIntervention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LignesPieceIntervention_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LignesPieceIntervention_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PiecesJointes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheminFichier = table.Column<string>(type: "TEXT", nullable: false),
                    Legende = table.Column<string>(type: "TEXT", nullable: true),
                    TypeContenu = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PiecesJointes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PiecesJointes_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rapports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroRapport = table.Column<string>(type: "TEXT", nullable: false),
                    CheminPdf = table.Column<string>(type: "TEXT", nullable: false),
                    DateGeneration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModification = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreePar = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiePar = table.Column<string>(type: "TEXT", nullable: true),
                    EstSupprime = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rapports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rapports_Interventions_InterventionId",
                        column: x => x.InterventionId,
                        principalTable: "Interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlocsOperatoires_ServiceId",
                table: "BlocsOperatoires",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListsCloture_InterventionId",
                table: "CheckListsCloture",
                column: "InterventionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetenceIngenieur_IngenieursId",
                table: "CompetenceIngenieur",
                column: "IngenieursId");

            migrationBuilder.CreateIndex(
                name: "IX_Competences_ModeleRespirateurId",
                table: "Competences",
                column: "ModeleRespirateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Conges_IngenieurId",
                table: "Conges",
                column: "IngenieurId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsTechniques_ModeleRespirateurId",
                table: "DocumentsTechniques",
                column: "ModeleRespirateurId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsTechniques_RespirateurId",
                table: "DocumentsTechniques",
                column: "RespirateurId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueConnexions_UtilisateurId",
                table: "HistoriqueConnexions",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriquesEtatIntervention_InterventionId",
                table: "HistoriquesEtatIntervention",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingenieurs_UtilisateurId",
                table: "Ingenieurs",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_HopitalId",
                table: "Interventions",
                column: "HopitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_IngenieurId",
                table: "Interventions",
                column: "IngenieurId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_NumeroDI",
                table: "Interventions",
                column: "NumeroDI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_PanneId",
                table: "Interventions",
                column: "PanneId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_RespirateurId",
                table: "Interventions",
                column: "RespirateurId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionSymptome_SymptomesId",
                table: "InterventionSymptome",
                column: "SymptomesId");

            migrationBuilder.CreateIndex(
                name: "IX_LignesPieceIntervention_InterventionId",
                table: "LignesPieceIntervention",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_LignesPieceIntervention_PieceId",
                table: "LignesPieceIntervention",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_MouvementsStock_PieceId",
                table: "MouvementsStock",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DestinataireId",
                table: "Notifications",
                column: "DestinataireId");

            migrationBuilder.CreateIndex(
                name: "IX_PannesPieces_PanneId",
                table: "PannesPieces",
                column: "PanneId");

            migrationBuilder.CreateIndex(
                name: "IX_PannesPieces_PieceId",
                table: "PannesPieces",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_CategoriePieceId",
                table: "Pieces",
                column: "CategoriePieceId");

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_FournisseurId",
                table: "Pieces",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_Reference",
                table: "Pieces",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PiecesJointes_InterventionId",
                table: "PiecesJointes",
                column: "InterventionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rapports_InterventionId",
                table: "Rapports",
                column: "InterventionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Respirateurs_BlocOperatoireId",
                table: "Respirateurs",
                column: "BlocOperatoireId");

            migrationBuilder.CreateIndex(
                name: "IX_Respirateurs_CodeQr",
                table: "Respirateurs",
                column: "CodeQr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Respirateurs_ModeleRespirateurId",
                table: "Respirateurs",
                column: "ModeleRespirateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Respirateurs_NumeroSerie",
                table: "Respirateurs",
                column: "NumeroSerie",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_HopitalId",
                table: "Services",
                column: "HopitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Login",
                table: "Utilisateurs",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckListsCloture");

            migrationBuilder.DropTable(
                name: "CompetenceIngenieur");

            migrationBuilder.DropTable(
                name: "Conges");

            migrationBuilder.DropTable(
                name: "DocumentsTechniques");

            migrationBuilder.DropTable(
                name: "HistoriqueConnexions");

            migrationBuilder.DropTable(
                name: "HistoriquesEtatIntervention");

            migrationBuilder.DropTable(
                name: "InterventionSymptome");

            migrationBuilder.DropTable(
                name: "LignesPieceIntervention");

            migrationBuilder.DropTable(
                name: "MouvementsStock");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PannesPieces");

            migrationBuilder.DropTable(
                name: "PiecesJointes");

            migrationBuilder.DropTable(
                name: "Rapports");

            migrationBuilder.DropTable(
                name: "Competences");

            migrationBuilder.DropTable(
                name: "Symptomes");

            migrationBuilder.DropTable(
                name: "Pieces");

            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "CategoriesPiece");

            migrationBuilder.DropTable(
                name: "Fournisseurs");

            migrationBuilder.DropTable(
                name: "Ingenieurs");

            migrationBuilder.DropTable(
                name: "Pannes");

            migrationBuilder.DropTable(
                name: "Respirateurs");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "BlocsOperatoires");

            migrationBuilder.DropTable(
                name: "ModelesRespirateur");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Hopitaux");
        }
    }
}

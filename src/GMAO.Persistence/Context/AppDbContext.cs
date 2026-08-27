using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Domain.Common;
using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Entities.Notifications;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Pieces;
using GMAO.Domain.Entities.Planning;
using GMAO.Domain.Entities.Securite;
using Microsoft.EntityFrameworkCore;

namespace GMAO.Persistence.Context;

/// <summary>
/// Contexte EF Core de l'application (base SQLite).
/// Gère l'audit automatique des entités et le filtrage des éléments soft-deletés.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUser;

    /// <summary>
    /// Constructeur d'exécution : <paramref name="currentUser"/> (optionnel) permet de tracer
    /// l'auteur des créations/modifications. Il reste nul en conception (factory de migrations).
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService? currentUser = null)
        : base(options)
        => _currentUser = currentUser;

    // Sécurité
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<HistoriqueConnexion> HistoriqueConnexions => Set<HistoriqueConnexion>();

    // Parc
    public DbSet<Hopital> Hopitaux => Set<Hopital>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<BlocOperatoire> BlocsOperatoires => Set<BlocOperatoire>();
    public DbSet<ModeleRespirateur> ModelesRespirateur => Set<ModeleRespirateur>();
    public DbSet<Respirateur> Respirateurs => Set<Respirateur>();
    public DbSet<DocumentTechnique> DocumentsTechniques => Set<DocumentTechnique>();

    // Interventions
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<Symptome> Symptomes => Set<Symptome>();
    public DbSet<Panne> Pannes => Set<Panne>();
    public DbSet<CheckListCloture> CheckListsCloture => Set<CheckListCloture>();
    public DbSet<LignePieceIntervention> LignesPieceIntervention => Set<LignePieceIntervention>();
    public DbSet<Rapport> Rapports => Set<Rapport>();
    public DbSet<PhotoDocument> PiecesJointes => Set<PhotoDocument>();
    public DbSet<HistoriqueEtatIntervention> HistoriquesEtatIntervention => Set<HistoriqueEtatIntervention>();

    // Pièces
    public DbSet<Piece> Pieces => Set<Piece>();
    public DbSet<CategoriePiece> CategoriesPiece => Set<CategoriePiece>();
    public DbSet<Fournisseur> Fournisseurs => Set<Fournisseur>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();
    public DbSet<PanneePiece> PannesPieces => Set<PanneePiece>();

    // Planning
    public DbSet<Ingenieur> Ingenieurs => Set<Ingenieur>();
    public DbSet<Conge> Conges => Set<Conge>();
    public DbSet<Competence> Competences => Set<Competence>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applique toutes les IEntityTypeConfiguration de cet assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Filtre global de soft-delete sur toutes les entités héritant d'EntiteBase.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntiteBase).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(EntiteBase.EstSupprime));
                var filter = Expression.Lambda(Expression.Not(property), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AppliquerAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        AppliquerAudit();
        return base.SaveChanges();
    }

    /// <summary>Renseigne automatiquement les champs d'audit lors des ajouts/modifications.</summary>
    private void AppliquerAudit()
    {
        var auteur = _currentUser?.Utilisateur?.NomComplet;

        foreach (var entry in ChangeTracker.Entries<EntiteBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.DateCreation = DateTime.UtcNow;
                    entry.Entity.CreePar ??= auteur;
                    break;
                case EntityState.Modified:
                    entry.Entity.DateModification = DateTime.UtcNow;
                    entry.Entity.ModifiePar = auteur;
                    break;
            }
        }
    }
}

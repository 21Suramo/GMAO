using GMAO.Domain.Entities.Interventions;
using GMAO.Domain.Entities.Parc;
using GMAO.Domain.Entities.Pieces;
using GMAO.Domain.Entities.Securite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GMAO.Persistence.Configurations;

/// <summary>Configuration EF Core de l'entité Utilisateur.</summary>
public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> b)
    {
        b.Property(u => u.Login).IsRequired().HasMaxLength(60);
        b.HasIndex(u => u.Login).IsUnique();
        b.Property(u => u.MotDePasseHash).IsRequired();
        b.Property(u => u.Nom).HasMaxLength(80);
        b.Property(u => u.Prenom).HasMaxLength(80);
        b.Property(u => u.Email).HasMaxLength(160);

        b.HasMany(u => u.HistoriqueConnexions)
            .WithOne(h => h.Utilisateur)
            .HasForeignKey(h => h.UtilisateurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Configuration EF Core de l'entité Respirateur.</summary>
public class RespirateurConfiguration : IEntityTypeConfiguration<Respirateur>
{
    public void Configure(EntityTypeBuilder<Respirateur> b)
    {
        b.Property(r => r.NumeroSerie).IsRequired().HasMaxLength(60);
        b.HasIndex(r => r.NumeroSerie).IsUnique();
        b.HasIndex(r => r.CodeQr).IsUnique();
        b.Property(r => r.CodeInterne).HasMaxLength(40);

        b.HasOne(r => r.Modele)
            .WithMany(m => m.Respirateurs)
            .HasForeignKey(r => r.ModeleRespirateurId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(r => r.BlocOperatoire)
            .WithMany(bl => bl.Respirateurs)
            .HasForeignKey(r => r.BlocOperatoireId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>Configuration EF Core de l'entité Intervention (relations 1-1, prix, FK restreintes).</summary>
public class InterventionConfiguration : IEntityTypeConfiguration<Intervention>
{
    public void Configure(EntityTypeBuilder<Intervention> b)
    {
        b.Property(i => i.NumeroDI).IsRequired().HasMaxLength(30);
        b.HasIndex(i => i.NumeroDI).IsUnique();
        b.Property(i => i.Description).IsRequired();
        b.Property(i => i.MainOeuvre).HasPrecision(18, 2);

        b.HasOne(i => i.Respirateur)
            .WithMany(r => r.Interventions)
            .HasForeignKey(i => i.RespirateurId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(i => i.Hopital)
            .WithMany()
            .HasForeignKey(i => i.HopitalId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(i => i.Ingenieur)
            .WithMany(g => g.Interventions)
            .HasForeignKey(i => i.IngenieurId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(i => i.Panne)
            .WithMany(p => p.Interventions)
            .HasForeignKey(i => i.PanneId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relations 1-1
        b.HasOne(i => i.CheckList)
            .WithOne(c => c.Intervention)
            .HasForeignKey<CheckListCloture>(c => c.InterventionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(i => i.Rapport)
            .WithOne(r => r.Intervention)
            .HasForeignKey<Rapport>(r => r.InterventionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Configuration EF Core de l'entité Piece.</summary>
public class PieceConfiguration : IEntityTypeConfiguration<Piece>
{
    public void Configure(EntityTypeBuilder<Piece> b)
    {
        b.Property(p => p.Reference).IsRequired().HasMaxLength(60);
        b.HasIndex(p => p.Reference).IsUnique();
        b.Property(p => p.Nom).IsRequired().HasMaxLength(160);
        b.Property(p => p.Prix).HasPrecision(18, 2);

        b.HasOne(p => p.Categorie)
            .WithMany(c => c.Pieces)
            .HasForeignKey(p => p.CategoriePieceId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(p => p.Fournisseur)
            .WithMany(f => f.Pieces)
            .HasForeignKey(p => p.FournisseurId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>Configuration EF Core de la ligne de pièce d'intervention (prix décimal).</summary>
public class LignePieceInterventionConfiguration : IEntityTypeConfiguration<LignePieceIntervention>
{
    public void Configure(EntityTypeBuilder<LignePieceIntervention> b)
    {
        b.Property(l => l.PrixUnitaire).HasPrecision(18, 2);

        b.HasOne(l => l.Intervention)
            .WithMany(i => i.PiecesUtilisees)
            .HasForeignKey(l => l.InterventionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(l => l.Piece)
            .WithMany()
            .HasForeignKey(l => l.PieceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>Configuration de l'association Panne ↔ Pièce.</summary>
public class PanneePieceConfiguration : IEntityTypeConfiguration<PanneePiece>
{
    public void Configure(EntityTypeBuilder<PanneePiece> b)
    {
        b.HasOne(pp => pp.Panne)
            .WithMany(p => p.PiecesSuspectes)
            .HasForeignKey(pp => pp.PanneId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(pp => pp.Piece)
            .WithMany(p => p.PannesAssociees)
            .HasForeignKey(pp => pp.PieceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

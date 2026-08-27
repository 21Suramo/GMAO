using FluentAssertions;
using GMAO.Application.Services.Affectation;
using Xunit;

namespace GMAO.Tests.Unit;

/// <summary>Tests du moteur d'affectation automatique (logique de score et de sélection).</summary>
public class MoteurAffectationTests
{
    private static ContexteAffectation Contexte(string modele = "Aespire", string? ville = "Casablanca")
        => new() { ModeleNom = modele, VilleHopital = ville };

    [Fact]
    public void Choisir_PrefereCompetenceEtZone()
    {
        var contexte = Contexte("Aespire", "Casablanca");
        var candidats = new[]
        {
            new IngenieurCandidat { Id = 1, NomComplet = "A", Zone = "Rabat", EstDisponible = true, CompetencesModeles = new[] { "Aisys" } },
            new IngenieurCandidat { Id = 2, NomComplet = "B", Zone = "Casablanca", EstDisponible = true, CompetencesModeles = new[] { "Aespire" } }
        };

        var choisi = MoteurAffectation.Choisir(candidats, contexte);

        choisi.Should().NotBeNull();
        choisi!.Id.Should().Be(2);
    }

    [Fact]
    public void Choisir_IgnoreLesIndisponibles()
    {
        var contexte = Contexte();
        var candidats = new[]
        {
            new IngenieurCandidat { Id = 1, Zone = "Casablanca", EstDisponible = false, CompetencesModeles = new[] { "Aespire" } },
            new IngenieurCandidat { Id = 2, Zone = "Rabat", EstDisponible = true, CompetencesModeles = new[] { "Aisys" } }
        };

        var choisi = MoteurAffectation.Choisir(candidats, contexte);

        choisi!.Id.Should().Be(2);
    }

    [Fact]
    public void Choisir_EquilibreLaCharge_AScoreEgal()
    {
        var contexte = Contexte("Aespire", "Casablanca");
        var candidats = new[]
        {
            new IngenieurCandidat { Id = 1, Zone = "Casablanca", EstDisponible = true, CompetencesModeles = new[] { "Aespire" }, NbInterventionsOuvertes = 3 },
            new IngenieurCandidat { Id = 2, Zone = "Casablanca", EstDisponible = true, CompetencesModeles = new[] { "Aespire" }, NbInterventionsOuvertes = 0 }
        };

        var choisi = MoteurAffectation.Choisir(candidats, contexte);

        choisi!.Id.Should().Be(2, "l'ingénieur le moins chargé doit être préféré à score égal");
    }

    [Fact]
    public void Choisir_RetourneNull_SiAucunDisponible()
    {
        var candidats = new[]
        {
            new IngenieurCandidat { Id = 1, EstDisponible = false }
        };

        MoteurAffectation.Choisir(candidats, Contexte()).Should().BeNull();
    }

    [Fact]
    public void Score_CompteCompetenceZoneEtCharge()
    {
        var candidat = new IngenieurCandidat
        {
            Zone = "Casablanca",
            CompetencesModeles = new[] { "Aespire" },
            NbInterventionsOuvertes = 2
        };

        // 50 (compétence) + 30 (zone) - 2*5 (charge) = 70
        MoteurAffectation.Score(candidat, Contexte("Aespire", "Casablanca")).Should().Be(70);
    }
}

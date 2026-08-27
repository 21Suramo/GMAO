using System.Text;
using FluentAssertions;
using GMAO.Application.DTOs;
using GMAO.Infrastructure.Documents;
using Xunit;

namespace GMAO.Tests.Unit;

/// <summary>Vérifie la génération du rapport PDF (iText7) de bout en bout.</summary>
public class RapportPdfGenerateurTests
{
    [Fact]
    public void Generer_ProduitUnPdfNonVideEtValide()
    {
        // Arrange
        var generateur = new RapportPdfGenerateur(new QrCodeService());
        var data = new RapportInterventionData
        {
            NumeroDI = "DI-2026-0001",
            Date = DateTime.UtcNow,
            ClientNom = "CHU Ibn Rochd",
            ClientVille = "Casablanca",
            AppareilSerie = "AESP-2021-0142",
            AppareilModele = "Aespire",
            Description = "Remplacement batterie",
            Diagnostic = "Batterie en fin de vie",
            Etat = "Clôturée",
            Ingenieur = "Sara Bennani",
            TempsDeplacement = 45,
            TempsReparation = 90,
            MainOeuvre = 450m,
            Pieces =
            {
                new LignePieceData { Reference = "BATT-LI-7900", Nom = "Batterie Li-Ion", Quantite = 1, PrixUnitaire = 180m }
            },
            CheckListValidee = { "Autotest OK", "Batterie", "Validation finale" },
            QrContenu = "GMAO-DI:DI-2026-0001"
        };

        // Act
        var pdf = generateur.Generer(data);

        // Assert : en-tête de fichier PDF valide.
        pdf.Should().NotBeNullOrEmpty();
        pdf.Length.Should().BeGreaterThan(1000);
        Encoding.ASCII.GetString(pdf, 0, 5).Should().Be("%PDF-");
    }
}

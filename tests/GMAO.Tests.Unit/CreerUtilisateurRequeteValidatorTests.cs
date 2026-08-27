using FluentAssertions;
using GMAO.Application.Common.Validation;
using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using Xunit;

namespace GMAO.Tests.Unit;

/// <summary>Tests de validation du formulaire de création d'un utilisateur.</summary>
public class CreerUtilisateurRequeteValidatorTests
{
    private readonly CreerUtilisateurRequeteValidator _validateur = new();

    private static CreerUtilisateurRequete Valide() => new()
    {
        Login = "j.dupont",
        Nom = "Dupont",
        Prenom = "Jean",
        Email = "j.dupont@medicana.local",
        Telephone = "+212 6 00 00 00 00",
        Role = RoleType.Technicien,
        MotDePasse = "Medicana1"
    };

    [Fact]
    public void RequeteValide_EstAcceptee()
    {
        _validateur.Validate(Valide()).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Login_TropCourtOuVide_EstRejete(string login)
    {
        var requete = Valide();
        requete.Login = login;
        _validateur.Validate(requete).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Nom_Manquant_EstRejete()
    {
        var requete = Valide();
        requete.Nom = "";
        _validateur.Validate(requete).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Email_Invalide_EstRejete()
    {
        var requete = Valide();
        requete.Email = "pas-un-email";
        _validateur.Validate(requete).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Email_Absent_EstAccepte()
    {
        var requete = Valide();
        requete.Email = null;
        _validateur.Validate(requete).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("court1A")]      // < 8 caractères
    [InlineData("minuscule1")]   // pas de majuscule
    [InlineData("MAJUSCULE1")]   // pas de minuscule
    [InlineData("SansChiffre")]  // pas de chiffre
    public void MotDePasse_TropFaible_EstRejete(string motDePasse)
    {
        var requete = Valide();
        requete.MotDePasse = motDePasse;
        _validateur.Validate(requete).IsValid.Should().BeFalse();
    }
}

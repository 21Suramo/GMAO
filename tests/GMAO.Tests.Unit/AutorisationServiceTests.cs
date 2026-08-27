using System.Linq.Expressions;
using FluentAssertions;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Application.Services;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GMAO.Tests.Unit;

/// <summary>Tests du service d'autorisation par action (RBAC + revérification en base).</summary>
public class AutorisationServiceTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IRepository<Utilisateur>> _depot = new();

    public AutorisationServiceTests()
    {
        _unitOfWork.Setup(u => u.Repository<Utilisateur>()).Returns(_depot.Object);
    }

    private AutorisationService Creer() =>
        new(_currentUser.Object, _unitOfWork.Object, new Mock<ILogger<AutorisationService>>().Object);

    private void SessionAvecRole(RoleType role, int id = 1) =>
        _currentUser.Setup(c => c.Utilisateur).Returns(new UtilisateurDto { Id = id, Login = "u", Role = role });

    private void CompteEnBase(Utilisateur? utilisateur) =>
        _depot.Setup(d => d.FirstOrDefaultAsync(It.IsAny<Expression<Func<Utilisateur, bool>>>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(utilisateur);

    [Fact]
    public async Task AutoriserAsync_SansSession_Refuse()
    {
        _currentUser.Setup(c => c.Utilisateur).Returns((UtilisateurDto?)null);

        var resultat = await Creer().AutoriserAsync(Permission.CreerIntervention);

        resultat.EstEchec.Should().BeTrue();
        resultat.Erreur.Should().Contain("aucune session");
    }

    [Fact]
    public async Task AutoriserAsync_CompteDesactive_Refuse()
    {
        SessionAvecRole(RoleType.Administrateur);
        CompteEnBase(new Utilisateur { Id = 1, Role = RoleType.Administrateur, Actif = false });

        var resultat = await Creer().AutoriserAsync(Permission.GererUtilisateurs);

        resultat.EstEchec.Should().BeTrue();
        resultat.Erreur.Should().Contain("plus actif");
    }

    [Fact]
    public async Task AutoriserAsync_CompteSupprime_Refuse()
    {
        SessionAvecRole(RoleType.Administrateur);
        CompteEnBase(null); // filtre soft-delete ⇒ introuvable

        var resultat = await Creer().AutoriserAsync(Permission.GererUtilisateurs);

        resultat.EstEchec.Should().BeTrue();
    }

    [Fact]
    public async Task AutoriserAsync_RoleSansDroit_Refuse()
    {
        SessionAvecRole(RoleType.Technicien);
        CompteEnBase(new Utilisateur { Id = 1, Role = RoleType.Technicien, Actif = true });

        var resultat = await Creer().AutoriserAsync(Permission.GererUtilisateurs);

        resultat.EstEchec.Should().BeTrue();
        resultat.Erreur.Should().Contain("Accès refusé");
    }

    [Fact]
    public async Task AutoriserAsync_RoleAutorise_Accepte()
    {
        SessionAvecRole(RoleType.Administrateur);
        CompteEnBase(new Utilisateur { Id = 1, Role = RoleType.Administrateur, Actif = true });

        var resultat = await Creer().AutoriserAsync(Permission.GererUtilisateurs);

        resultat.EstSucces.Should().BeTrue();
    }

    [Fact]
    public void ADroit_ControleRapideSurLeRoleCourant()
    {
        SessionAvecRole(RoleType.Ingenieur);
        var service = Creer();

        service.ADroit(Permission.CreerIntervention).Should().BeTrue();
        service.ADroit(Permission.GererUtilisateurs).Should().BeFalse();
    }
}

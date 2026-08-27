using FluentAssertions;
using GMAO.Application.Services.TableauBord;
using GMAO.Domain.Enums;
using Xunit;

namespace GMAO.Tests.Unit;

/// <summary>Tests des calculs d'indicateurs (MTTR, délai d'affectation, disponibilité, SLA).</summary>
public class CalculateurKpiTests
{
    [Fact]
    public void Mttr_MoyenneLesDelaisCreationCloture_EnHeures()
    {
        var reference = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var interventions = new (DateTime, DateTime?)[]
        {
            (reference, reference.AddHours(2)),   // 2 h
            (reference, reference.AddHours(6)),   // 6 h
            (reference, null)                     // non clôturée → ignorée
        };

        CalculateurKpi.Mttr(interventions).Should().Be(4d);
    }

    [Fact]
    public void Mttr_SansCloture_Retourne0()
    {
        var reference = DateTime.UtcNow;
        CalculateurKpi.Mttr(new (DateTime, DateTime?)[] { (reference, null) }).Should().Be(0d);
    }

    [Fact]
    public void DelaiMoyenAffectation_MoyenneCreationPriseEnCharge()
    {
        var reference = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var interventions = new (DateTime, DateTime?)[]
        {
            (reference, reference.AddHours(1)),   // 1 h
            (reference, reference.AddHours(3))    // 3 h
        };

        CalculateurKpi.DelaiMoyenAffectation(interventions).Should().Be(2d);
    }

    [Fact]
    public void DisponibilitePourcent_ImmobilisationPartielle()
    {
        // 6 h d'immobilisation (360 min) sur 24 h (1440 min) ⇒ 75 % de disponibilité.
        CalculateurKpi.DisponibilitePourcent(360, 24 * 60).Should().Be(75d);
    }

    [Fact]
    public void DisponibilitePourcent_ImmobilisationSuperieureAPeriode_PlancherA0()
    {
        CalculateurKpi.DisponibilitePourcent(5000, 60).Should().Be(0d);
    }

    [Fact]
    public void DisponibilitePourcent_PeriodeNulle_Retourne100()
    {
        CalculateurKpi.DisponibilitePourcent(120, 0).Should().Be(100d);
    }

    [Theory]
    [InlineData(Priorite.Critique, 5, true)]   // seuil 4 h dépassé
    [InlineData(Priorite.Critique, 3, false)]
    [InlineData(Priorite.Haute, 25, true)]     // seuil 24 h dépassé
    [InlineData(Priorite.Normale, 48, false)]  // seuil 72 h non dépassé
    public void EstEnDepassementSla_SelonPrioriteEtAge(Priorite priorite, double ageHeures, bool attendu)
    {
        CalculateurKpi.EstEnDepassementSla(priorite, ageHeures).Should().Be(attendu);
    }
}

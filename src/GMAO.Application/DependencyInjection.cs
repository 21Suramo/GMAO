using System.Reflection;
using FluentValidation;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Application;

/// <summary>Enregistrement des services de la couche Application.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAutorisationService, AutorisationService>();
        services.AddScoped<ITableauBordService, TableauBordService>();
        services.AddScoped<IStatistiquesService, StatistiquesService>();
        services.AddScoped<IRespirateurService, RespirateurService>();
        services.AddScoped<IAffectationService, AffectationService>();
        services.AddScoped<IInterventionService, InterventionService>();
        services.AddScoped<IPieceService, PieceService>();
        services.AddScoped<IRapportService, RapportService>();
        services.AddScoped<IUtilisateurService, UtilisateurService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}

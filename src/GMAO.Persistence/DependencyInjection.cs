using GMAO.Application.Common.Interfaces;
using GMAO.Persistence.Context;
using GMAO.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Persistence;

/// <summary>
/// Enregistrement des services de la couche Persistence dans le conteneur d'injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Ajoute le DbContext SQLite, le dépôt générique et l'unité de travail.
    /// </summary>
    /// <param name="services">Conteneur de services.</param>
    /// <param name="connectionString">Chaîne de connexion SQLite (ex. « Data Source=gmao.db »).</param>
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

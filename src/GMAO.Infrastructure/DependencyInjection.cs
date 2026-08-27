using GMAO.Application.Common.Interfaces;
using GMAO.Infrastructure.Documents;
using GMAO.Infrastructure.Notifications;
using GMAO.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace GMAO.Infrastructure;

/// <summary>
/// Enregistrement des services techniques (adapters) de la couche Infrastructure.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Ajoute les implémentations d'infrastructure (sécurité, PDF, QR, notifications…).</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddSingleton<IRapportPdfGenerateur, RapportPdfGenerateur>();
        services.AddSingleton<INotificationTempsReel, NotificationTempsReelClient>();

        // Le service e-mail sera enregistré ici dans une phase ultérieure.

        return services;
    }
}

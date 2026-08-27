using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GMAO.Persistence.Context;

/// <summary>
/// Fabrique utilisée par les outils EF Core en conception (création de migrations).
/// Permet d'exécuter « dotnet ef » sans démarrer l'application WPF.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=gmao.db")
            .Options;

        return new AppDbContext(options);
    }
}

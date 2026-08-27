using GMAO.Application.Common.Interfaces;

namespace GMAO.Infrastructure.Security;

/// <summary>
/// Implémentation du hachage de mot de passe basée sur l'algorithme BCrypt.
/// </summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string Hacher(string motDePasse)
        => BCrypt.Net.BCrypt.HashPassword(motDePasse, workFactor: 12);

    /// <inheritdoc />
    public bool Verifier(string motDePasse, string empreinte)
        => BCrypt.Net.BCrypt.Verify(motDePasse, empreinte);
}

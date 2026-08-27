namespace GMAO.Application.Common.Interfaces;

/// <summary>
/// Service de hachage et de vérification des mots de passe.
/// L'implémentation (BCrypt) réside dans la couche Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Calcule l'empreinte sécurisée d'un mot de passe en clair.</summary>
    string Hacher(string motDePasse);

    /// <summary>Vérifie qu'un mot de passe en clair correspond à une empreinte.</summary>
    bool Verifier(string motDePasse, string empreinte);
}

using GMAO.Application.DTOs;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Service d'authentification des utilisateurs.</summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Vérifie les identifiants et journalise la tentative de connexion.
    /// </summary>
    /// <returns>L'utilisateur authentifié en cas de succès, sinon un échec explicite.</returns>
    Task<Result<UtilisateurDto>> ConnecterAsync(ConnexionRequete requete, CancellationToken cancellationToken = default);
}

using GMAO.Application.DTOs;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;

namespace GMAO.Application.Common.Interfaces;

/// <summary>Cas d'usage liés au compte utilisateur (profil, sécurité, administration).</summary>
public interface IUtilisateurService
{
    /// <summary>Historique des connexions d'un utilisateur (les plus récentes d'abord).</summary>
    Task<IReadOnlyList<HistoriqueConnexionDto>> HistoriqueConnexionsAsync(int utilisateurId, int max = 20, CancellationToken cancellationToken = default);

    /// <summary>Change le mot de passe après vérification de l'ancien (self-service, sans droit admin).</summary>
    Task<Result> ChangerMotDePasseAsync(int utilisateurId, string ancienMotDePasse, string nouveauMotDePasse, CancellationToken cancellationToken = default);

    // --- Administration des comptes (réservée à la permission GererUtilisateurs) ---

    /// <summary>Liste les comptes, filtrés par rôle et/ou par recherche texte (nom, prénom, login, e-mail).</summary>
    Task<Result<IReadOnlyList<UtilisateurListeDto>>> ListerAsync(string? recherche = null, RoleType? role = null, CancellationToken cancellationToken = default);

    /// <summary>Crée un compte (login/e-mail uniques, mot de passe haché en BCrypt).</summary>
    Task<Result<UtilisateurListeDto>> CreerAsync(CreerUtilisateurRequete requete, CancellationToken cancellationToken = default);

    /// <summary>Modifie les informations d'un compte existant.</summary>
    Task<Result> ModifierAsync(ModifierUtilisateurRequete requete, CancellationToken cancellationToken = default);

    /// <summary>Active ou désactive un compte (un compte désactivé ne peut plus se connecter).</summary>
    Task<Result> DefinirActifAsync(int utilisateurId, bool actif, CancellationToken cancellationToken = default);

    /// <summary>Désactive et soft-delete un compte (aucune suppression physique).</summary>
    Task<Result> SupprimerAsync(int utilisateurId, CancellationToken cancellationToken = default);

    /// <summary>Réinitialise le mot de passe d'un compte (action administrateur).</summary>
    Task<Result> ReinitialiserMotDePasseAsync(int utilisateurId, string nouveauMotDePasse, CancellationToken cancellationToken = default);
}

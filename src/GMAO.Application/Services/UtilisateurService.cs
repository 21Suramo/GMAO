using FluentValidation;
using GMAO.Application.Common;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.Common.Validation;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Securite;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>Implémentation des cas d'usage liés au compte utilisateur.</summary>
public class UtilisateurService : IUtilisateurService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAutorisationService _autorisation;
    private readonly IValidator<CreerUtilisateurRequete> _validateurCreation;
    private readonly ILogger<UtilisateurService> _logger;

    public UtilisateurService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IAutorisationService autorisation,
        IValidator<CreerUtilisateurRequete> validateurCreation,
        ILogger<UtilisateurService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _autorisation = autorisation;
        _validateurCreation = validateurCreation;
        _logger = logger;
    }

    public async Task<IReadOnlyList<HistoriqueConnexionDto>> HistoriqueConnexionsAsync(int utilisateurId, int max = 20, CancellationToken cancellationToken = default)
    {
        var liste = await _unitOfWork.Repository<HistoriqueConnexion>().ListerAsync(
            h => h.UtilisateurId == utilisateurId,
            h => new HistoriqueConnexionDto
            {
                DateConnexion = h.DateConnexion,
                AdresseIp = h.AdresseIp,
                Succes = h.Succes,
                Detail = h.Detail
            },
            cancellationToken);

        return liste.OrderByDescending(h => h.DateConnexion).Take(max).ToList();
    }

    public async Task<Result> ChangerMotDePasseAsync(int utilisateurId, string ancienMotDePasse, string nouveauMotDePasse, CancellationToken cancellationToken = default)
    {
        var erreurComplexite = RegleMotDePasse.Valider(nouveauMotDePasse);
        if (erreurComplexite is not null)
            return Result.Echec(erreurComplexite);

        var depot = _unitOfWork.Repository<Utilisateur>();
        var utilisateur = await depot.GetByIdAsync(utilisateurId, cancellationToken);
        if (utilisateur is null)
            return Result.Echec("Utilisateur introuvable.");

        if (!_passwordHasher.Verifier(ancienMotDePasse, utilisateur.MotDePasseHash))
            return Result.Echec("L'ancien mot de passe est incorrect.");

        utilisateur.MotDePasseHash = _passwordHasher.Hacher(nouveauMotDePasse);
        depot.Update(utilisateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Mot de passe modifié pour l'utilisateur {Id}", utilisateurId);
        return Result.Succes();
    }

    public async Task<Result<IReadOnlyList<UtilisateurListeDto>>> ListerAsync(string? recherche = null, RoleType? role = null, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererUtilisateurs, cancellationToken);
        if (acces.EstEchec)
            return Result.Echec<IReadOnlyList<UtilisateurListeDto>>(acces.Erreur);

        var liste = await _unitOfWork.Repository<Utilisateur>().ListerAsync(
            role.HasValue ? u => u.Role == role.Value : null,
            u => new UtilisateurListeDto
            {
                Id = u.Id,
                Login = u.Login,
                Nom = u.Nom,
                Prenom = u.Prenom,
                Email = u.Email,
                Telephone = u.Telephone,
                Role = u.Role,
                Actif = u.Actif,
                DerniereConnexion = u.DerniereConnexion
            },
            cancellationToken);

        IEnumerable<UtilisateurListeDto> resultat = liste;
        if (!string.IsNullOrWhiteSpace(recherche))
        {
            var terme = recherche.Trim();
            resultat = resultat.Where(u =>
                u.Nom.Contains(terme, StringComparison.OrdinalIgnoreCase)
                || u.Prenom.Contains(terme, StringComparison.OrdinalIgnoreCase)
                || u.Login.Contains(terme, StringComparison.OrdinalIgnoreCase)
                || (u.Email != null && u.Email.Contains(terme, StringComparison.OrdinalIgnoreCase)));
        }

        var final = resultat
            .Select(u => { u.NomComplet = $"{u.Prenom} {u.Nom}".Trim(); u.RoleLibelle = RoleLibelle.Pour(u.Role); return u; })
            .OrderBy(u => u.Nom).ThenBy(u => u.Prenom)
            .ToList();

        return Result.Succes<IReadOnlyList<UtilisateurListeDto>>(final);
    }

    public async Task<Result<UtilisateurListeDto>> CreerAsync(CreerUtilisateurRequete requete, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererUtilisateurs, cancellationToken);
        if (acces.EstEchec)
            return Result.Echec<UtilisateurListeDto>(acces.Erreur);

        var validation = await _validateurCreation.ValidateAsync(requete, cancellationToken);
        if (!validation.IsValid)
            return Result.Echec<UtilisateurListeDto>(validation.Errors.First().ErrorMessage);

        var depot = _unitOfWork.Repository<Utilisateur>();

        if (await depot.AnyAsync(u => u.Login == requete.Login, cancellationToken))
            return Result.Echec<UtilisateurListeDto>($"Le login « {requete.Login} » est déjà utilisé.");

        if (!string.IsNullOrWhiteSpace(requete.Email)
            && await depot.AnyAsync(u => u.Email == requete.Email, cancellationToken))
            return Result.Echec<UtilisateurListeDto>($"L'e-mail « {requete.Email} » est déjà utilisé.");

        var utilisateur = new Utilisateur
        {
            Login = requete.Login.Trim(),
            Nom = requete.Nom.Trim(),
            Prenom = requete.Prenom.Trim(),
            Email = string.IsNullOrWhiteSpace(requete.Email) ? null : requete.Email.Trim(),
            Telephone = string.IsNullOrWhiteSpace(requete.Telephone) ? null : requete.Telephone.Trim(),
            Role = requete.Role,
            Actif = true,
            MotDePasseHash = _passwordHasher.Hacher(requete.MotDePasse)
        };

        await depot.AddAsync(utilisateur, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Compte utilisateur créé : {Login} (rôle {Role})", utilisateur.Login, utilisateur.Role);
        return Result.Succes(new UtilisateurListeDto
        {
            Id = utilisateur.Id,
            Login = utilisateur.Login,
            Nom = utilisateur.Nom,
            Prenom = utilisateur.Prenom,
            NomComplet = utilisateur.NomComplet,
            Email = utilisateur.Email,
            Telephone = utilisateur.Telephone,
            Role = utilisateur.Role,
            RoleLibelle = RoleLibelle.Pour(utilisateur.Role),
            Actif = utilisateur.Actif
        });
    }

    public async Task<Result> ModifierAsync(ModifierUtilisateurRequete requete, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererUtilisateurs, cancellationToken);
        if (acces.EstEchec)
            return acces;

        if (string.IsNullOrWhiteSpace(requete.Nom) || string.IsNullOrWhiteSpace(requete.Prenom))
            return Result.Echec("Le nom et le prénom sont obligatoires.");

        var depot = _unitOfWork.Repository<Utilisateur>();
        var utilisateur = await depot.GetByIdAsync(requete.Id, cancellationToken);
        if (utilisateur is null)
            return Result.Echec("Utilisateur introuvable.");

        if (!string.IsNullOrWhiteSpace(requete.Email)
            && await depot.AnyAsync(u => u.Email == requete.Email && u.Id != requete.Id, cancellationToken))
            return Result.Echec($"L'e-mail « {requete.Email} » est déjà utilisé par un autre compte.");

        utilisateur.Nom = requete.Nom.Trim();
        utilisateur.Prenom = requete.Prenom.Trim();
        utilisateur.Email = string.IsNullOrWhiteSpace(requete.Email) ? null : requete.Email.Trim();
        utilisateur.Telephone = string.IsNullOrWhiteSpace(requete.Telephone) ? null : requete.Telephone.Trim();
        utilisateur.Role = requete.Role;
        utilisateur.Actif = requete.Actif;

        depot.Update(utilisateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Compte utilisateur {Id} modifié ({Login})", utilisateur.Id, utilisateur.Login);
        return Result.Succes();
    }

    public async Task<Result> DefinirActifAsync(int utilisateurId, bool actif, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererUtilisateurs, cancellationToken);
        if (acces.EstEchec)
            return acces;

        var depot = _unitOfWork.Repository<Utilisateur>();
        var utilisateur = await depot.GetByIdAsync(utilisateurId, cancellationToken);
        if (utilisateur is null)
            return Result.Echec("Utilisateur introuvable.");

        utilisateur.Actif = actif;
        depot.Update(utilisateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Compte {Id} {Etat}", utilisateurId, actif ? "activé" : "désactivé");
        return Result.Succes();
    }

    public async Task<Result> SupprimerAsync(int utilisateurId, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererUtilisateurs, cancellationToken);
        if (acces.EstEchec)
            return acces;

        var depot = _unitOfWork.Repository<Utilisateur>();
        var utilisateur = await depot.GetByIdAsync(utilisateurId, cancellationToken);
        if (utilisateur is null)
            return Result.Echec("Utilisateur introuvable.");

        // Soft-delete (aucune suppression physique) : le filtre global l'exclut désormais des requêtes.
        utilisateur.Actif = false;
        utilisateur.EstSupprime = true;
        depot.Update(utilisateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Compte {Id} supprimé (soft-delete)", utilisateurId);
        return Result.Succes();
    }

    public async Task<Result> ReinitialiserMotDePasseAsync(int utilisateurId, string nouveauMotDePasse, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererUtilisateurs, cancellationToken);
        if (acces.EstEchec)
            return acces;

        var erreurComplexite = RegleMotDePasse.Valider(nouveauMotDePasse);
        if (erreurComplexite is not null)
            return Result.Echec(erreurComplexite);

        var depot = _unitOfWork.Repository<Utilisateur>();
        var utilisateur = await depot.GetByIdAsync(utilisateurId, cancellationToken);
        if (utilisateur is null)
            return Result.Echec("Utilisateur introuvable.");

        utilisateur.MotDePasseHash = _passwordHasher.Hacher(nouveauMotDePasse);
        depot.Update(utilisateur);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Mot de passe réinitialisé pour l'utilisateur {Id} (par admin)", utilisateurId);
        return Result.Succes();
    }
}

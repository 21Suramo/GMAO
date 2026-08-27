using AutoMapper;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Securite;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>
/// Service d'authentification : vérifie les identifiants, met à jour la dernière
/// connexion et journalise chaque tentative (succès comme échec).
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<AuthenticationService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UtilisateurDto>> ConnecterAsync(ConnexionRequete requete, CancellationToken cancellationToken = default)
    {
        var utilisateurs = _unitOfWork.Repository<Utilisateur>();
        var utilisateur = await utilisateurs.FirstOrDefaultAsync(u => u.Login == requete.Login, cancellationToken);

        if (utilisateur is null)
        {
            _logger.LogWarning("Échec de connexion : login inconnu « {Login} »", requete.Login);
            return Result.Echec<UtilisateurDto>("Identifiants invalides.");
        }

        if (!utilisateur.Actif)
        {
            await JournaliserAsync(utilisateur.Id, requete.AdresseIp, succes: false, "Compte désactivé", cancellationToken);
            return Result.Echec<UtilisateurDto>("Ce compte est désactivé.");
        }

        if (!_passwordHasher.Verifier(requete.MotDePasse, utilisateur.MotDePasseHash))
        {
            await JournaliserAsync(utilisateur.Id, requete.AdresseIp, succes: false, "Mot de passe invalide", cancellationToken);
            _logger.LogWarning("Échec de connexion : mot de passe invalide pour « {Login} »", requete.Login);
            return Result.Echec<UtilisateurDto>("Identifiants invalides.");
        }

        utilisateur.DerniereConnexion = DateTime.UtcNow;
        utilisateurs.Update(utilisateur);
        await JournaliserAsync(utilisateur.Id, requete.AdresseIp, succes: true, null, cancellationToken);

        _logger.LogInformation("Connexion réussie de « {Login} » (rôle {Role})", utilisateur.Login, utilisateur.Role);
        return Result.Succes(_mapper.Map<UtilisateurDto>(utilisateur));
    }

    private async Task JournaliserAsync(int utilisateurId, string? ip, bool succes, string? detail, CancellationToken ct)
    {
        await _unitOfWork.Repository<HistoriqueConnexion>().AddAsync(new HistoriqueConnexion
        {
            UtilisateurId = utilisateurId,
            DateConnexion = DateTime.UtcNow,
            AdresseIp = ip,
            Succes = succes,
            Detail = detail
        }, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}

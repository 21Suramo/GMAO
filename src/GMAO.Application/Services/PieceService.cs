using System.Linq.Expressions;
using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using GMAO.Domain.Entities.Pieces;
using GMAO.Domain.Enums;
using GMAO.Shared.Results;
using Microsoft.Extensions.Logging;

namespace GMAO.Application.Services;

/// <summary>Implémentation des cas d'usage des pièces détachées.</summary>
public class PieceService : IPieceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAutorisationService _autorisation;
    private readonly ILogger<PieceService> _logger;

    public PieceService(IUnitOfWork unitOfWork, IAutorisationService autorisation, ILogger<PieceService> logger)
    {
        _unitOfWork = unitOfWork;
        _autorisation = autorisation;
        _logger = logger;
    }

    private static readonly Expression<Func<Piece, PieceDto>> Projection = p => new PieceDto
    {
        Id = p.Id,
        Reference = p.Reference,
        Nom = p.Nom,
        Compatible = p.Compatible,
        Stock = p.Stock,
        StockMinimum = p.StockMinimum,
        Emplacement = p.Emplacement,
        Prix = p.Prix,
        DatePeremption = p.DatePeremption,
        CategorieNom = p.Categorie != null ? p.Categorie.Nom : null,
        FournisseurNom = p.Fournisseur != null ? p.Fournisseur.Nom : null,
        PannesAssociees = p.PannesAssociees.Select(pp => pp.Panne!.Libelle).ToList()
    };

    public async Task<IReadOnlyList<PieceDto>> ListerAsync(CancellationToken cancellationToken = default)
    {
        var liste = await _unitOfWork.Repository<Piece>().ListerAsync(null, Projection, cancellationToken);
        return liste.OrderBy(p => p.Nom).ToList();
    }

    public async Task<Result> EnregistrerMouvementAsync(int pieceId, TypeMouvement type, int quantite, string? motif, string auteur, CancellationToken cancellationToken = default)
    {
        var acces = await _autorisation.AutoriserAsync(Permission.GererStock, cancellationToken);
        if (acces.EstEchec)
            return acces;

        if (quantite <= 0)
            return Result.Echec("La quantité doit être positive.");

        var depot = _unitOfWork.Repository<Piece>();
        var piece = await depot.GetByIdAsync(pieceId, cancellationToken);
        if (piece is null)
            return Result.Echec("Pièce introuvable.");

        switch (type)
        {
            case TypeMouvement.Entree:
                piece.Stock += quantite;
                break;
            case TypeMouvement.Sortie:
                if (quantite > piece.Stock)
                    return Result.Echec($"Stock insuffisant (disponible : {piece.Stock}).");
                piece.Stock -= quantite;
                break;
            case TypeMouvement.Ajustement:
                piece.Stock = quantite;
                break;
        }

        depot.Update(piece);
        await _unitOfWork.Repository<MouvementStock>().AddAsync(new MouvementStock
        {
            PieceId = pieceId,
            Type = type,
            Quantite = quantite,
            Motif = motif,
            Auteur = auteur
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (piece.EnAlerte())
            _logger.LogWarning("Pièce {Ref} en alerte de stock ({Stock} ≤ {Min})", piece.Reference, piece.Stock, piece.StockMinimum);

        return Result.Succes();
    }
}

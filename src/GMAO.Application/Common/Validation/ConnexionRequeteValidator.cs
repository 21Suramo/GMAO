using FluentValidation;
using GMAO.Application.DTOs;

namespace GMAO.Application.Common.Validation;

/// <summary>Règles de validation d'une demande de connexion.</summary>
public class ConnexionRequeteValidator : AbstractValidator<ConnexionRequete>
{
    public ConnexionRequeteValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Le login est obligatoire.")
            .MaximumLength(60);

        RuleFor(x => x.MotDePasse)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.");
    }
}

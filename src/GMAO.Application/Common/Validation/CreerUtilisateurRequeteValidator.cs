using FluentValidation;
using GMAO.Application.DTOs;

namespace GMAO.Application.Common.Validation;

/// <summary>
/// Règles de validation de la création d'un compte utilisateur.
/// L'unicité du login et de l'e-mail est vérifiée en base par le service
/// (une contrainte nécessitant un accès aux données).
/// </summary>
public class CreerUtilisateurRequeteValidator : AbstractValidator<CreerUtilisateurRequete>
{
    public CreerUtilisateurRequeteValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Le login est obligatoire.")
            .MinimumLength(3).WithMessage("Le login doit comporter au moins 3 caractères.")
            .MaximumLength(60);

        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(80);

        RuleFor(x => x.Prenom)
            .NotEmpty().WithMessage("Le prénom est obligatoire.")
            .MaximumLength(80);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.")
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Telephone)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Telephone));

        RuleFor(x => x.MotDePasse)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(RegleMotDePasse.LongueurMinimale).WithMessage(RegleMotDePasse.MessageLongueur)
            .Matches("[A-Z]").WithMessage(RegleMotDePasse.MessageMajuscule)
            .Matches("[a-z]").WithMessage(RegleMotDePasse.MessageMinuscule)
            .Matches("[0-9]").WithMessage(RegleMotDePasse.MessageChiffre);
    }
}

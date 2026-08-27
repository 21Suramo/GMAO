using System.Text.RegularExpressions;

namespace GMAO.Application.Common.Validation;

/// <summary>
/// Règle unique de complexité des mots de passe, partagée entre la validation
/// FluentValidation (création d'utilisateur) et les services (changement /
/// réinitialisation), afin d'éviter toute divergence entre les couches :
/// min. 8 caractères, 1 majuscule, 1 minuscule, 1 chiffre.
/// </summary>
public static class RegleMotDePasse
{
    public const int LongueurMinimale = 8;

    public const string MessageLongueur = "Le mot de passe doit comporter au moins 8 caractères.";
    public const string MessageMajuscule = "Le mot de passe doit contenir au moins une majuscule.";
    public const string MessageMinuscule = "Le mot de passe doit contenir au moins une minuscule.";
    public const string MessageChiffre = "Le mot de passe doit contenir au moins un chiffre.";

    /// <summary>
    /// Valide la complexité d'un mot de passe. Retourne le premier message d'erreur
    /// rencontré, ou <c>null</c> si le mot de passe respecte toutes les règles.
    /// </summary>
    public static string? Valider(string? motDePasse)
    {
        if (string.IsNullOrWhiteSpace(motDePasse) || motDePasse.Length < LongueurMinimale)
            return MessageLongueur;
        if (!Regex.IsMatch(motDePasse, "[A-Z]"))
            return MessageMajuscule;
        if (!Regex.IsMatch(motDePasse, "[a-z]"))
            return MessageMinuscule;
        if (!Regex.IsMatch(motDePasse, "[0-9]"))
            return MessageChiffre;
        return null;
    }
}

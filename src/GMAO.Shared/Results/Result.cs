namespace GMAO.Shared.Results;

/// <summary>
/// Résultat d'une opération applicative, sans valeur de retour.
/// Permet de gérer les erreurs métier sans recourir aux exceptions de flux.
/// </summary>
public class Result
{
    /// <summary>Indique si l'opération a réussi.</summary>
    public bool EstSucces { get; }

    /// <summary>Indique si l'opération a échoué.</summary>
    public bool EstEchec => !EstSucces;

    /// <summary>Message d'erreur en cas d'échec (vide si succès).</summary>
    public string Erreur { get; }

    protected Result(bool estSucces, string erreur)
    {
        EstSucces = estSucces;
        Erreur = erreur;
    }

    /// <summary>Crée un résultat de succès.</summary>
    public static Result Succes() => new(true, string.Empty);

    /// <summary>Crée un résultat d'échec avec un message.</summary>
    public static Result Echec(string erreur) => new(false, erreur);

    /// <summary>Crée un résultat typé de succès.</summary>
    public static Result<T> Succes<T>(T valeur) => Result<T>.Reussi(valeur);

    /// <summary>Crée un résultat typé d'échec.</summary>
    public static Result<T> Echec<T>(string erreur) => Result<T>.Rate(erreur);
}

/// <summary>
/// Résultat d'une opération applicative renvoyant une valeur de type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type de la valeur retournée en cas de succès.</typeparam>
public sealed class Result<T> : Result
{
    /// <summary>Valeur retournée (disponible uniquement en cas de succès).</summary>
    public T? Valeur { get; }

    private Result(bool estSucces, T? valeur, string erreur) : base(estSucces, erreur)
    {
        Valeur = valeur;
    }

    internal static Result<T> Reussi(T valeur) => new(true, valeur, string.Empty);

    internal static Result<T> Rate(string erreur) => new(false, default, erreur);
}

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GMAO.Domain.Enums;

namespace GMAO.Presentation.Wpf.Converters;

/// <summary>Convertit un état de respirateur en couleur (badge).</summary>
public class EtatRespirateurVersCouleurConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var couleur = value is EtatRespirateur etat
            ? etat switch
            {
                EtatRespirateur.EnService => Color.FromRgb(0x2E, 0x7D, 0x32),
                EtatRespirateur.EnMaintenance => Color.FromRgb(0xF9, 0xA8, 0x25),
                EtatRespirateur.HorsService => Color.FromRgb(0xC6, 0x28, 0x28),
                EtatRespirateur.EnAttente => Color.FromRgb(0x60, 0x7D, 0x8B),
                _ => Color.FromRgb(0x90, 0xA4, 0xAE)
            }
            : Color.FromRgb(0x90, 0xA4, 0xAE);

        return new SolidColorBrush(couleur);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit une chaîne non vide en Visible, vide/null en Collapsed.</summary>
public class StringVersVisibiliteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit une priorité d'intervention en couleur.</summary>
public class PrioriteVersCouleurConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var couleur = value is Priorite p
            ? p switch
            {
                Priorite.Basse => Color.FromRgb(0x90, 0xA4, 0xAE),
                Priorite.Normale => Color.FromRgb(0x42, 0x85, 0xF4),
                Priorite.Haute => Color.FromRgb(0xF9, 0xA8, 0x25),
                Priorite.Critique => Color.FromRgb(0xC6, 0x28, 0x28),
                _ => Color.FromRgb(0x90, 0xA4, 0xAE)
            }
            : Color.FromRgb(0x90, 0xA4, 0xAE);
        return new SolidColorBrush(couleur);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit un état d'intervention en couleur.</summary>
public class EtatInterventionVersCouleurConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var couleur = value is EtatIntervention e
            ? e switch
            {
                EtatIntervention.Nouvelle => Color.FromRgb(0x60, 0x7D, 0x8B),
                EtatIntervention.Affectee => Color.FromRgb(0x42, 0x85, 0xF4),
                EtatIntervention.EnDeplacement => Color.FromRgb(0x7E, 0x57, 0xC2),
                EtatIntervention.Diagnostic => Color.FromRgb(0x00, 0x97, 0xA7),
                EtatIntervention.Reparation => Color.FromRgb(0xF9, 0xA8, 0x25),
                EtatIntervention.EnAttentePiece => Color.FromRgb(0xEF, 0x6C, 0x00),
                EtatIntervention.Test => Color.FromRgb(0x5C, 0x6B, 0xC0),
                EtatIntervention.Validation => Color.FromRgb(0x26, 0xA6, 0x9A),
                EtatIntervention.Cloturee => Color.FromRgb(0x2E, 0x7D, 0x32),
                EtatIntervention.Annulee => Color.FromRgb(0x9E, 0x9E, 0x9E),
                _ => Color.FromRgb(0x90, 0xA4, 0xAE)
            }
            : Color.FromRgb(0x90, 0xA4, 0xAE);
        return new SolidColorBrush(couleur);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit un niveau d'alerte de stock (texte) en couleur.</summary>
public class NiveauAlerteVersCouleurConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var couleur = (value as string) switch
        {
            "Rupture" => Color.FromRgb(0xC6, 0x28, 0x28),
            "Stock bas" => Color.FromRgb(0xEF, 0x6C, 0x00),
            "Périmé" => Color.FromRgb(0x7E, 0x57, 0xC2),
            _ => Color.FromRgb(0x2E, 0x7D, 0x32)
        };
        return new SolidColorBrush(couleur);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit une référence en Visibilité (non null = Visible).</summary>
public class NullVersVisibiliteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit un booléen en son inverse (utile pour IsEnabled).</summary>
public class BoolInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : true;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : false;
}

/// <summary>Convertit un booléen en Visibilité (true = Visible).</summary>
public class BoolVersVisibiliteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Visible;
}

/// <summary>Convertit un booléen en Visibilité inversée (true = Collapsed).</summary>
public class BoolInverseVersVisibiliteConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v != Visibility.Visible;
}

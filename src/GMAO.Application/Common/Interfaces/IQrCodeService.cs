namespace GMAO.Application.Common.Interfaces;

/// <summary>Génération de QR Codes (encodage de l'identité des respirateurs).</summary>
public interface IQrCodeService
{
    /// <summary>Génère un QR Code au format PNG.</summary>
    /// <param name="contenu">Texte à encoder.</param>
    /// <param name="pixelsParModule">Taille d'un module en pixels (qualité/résolution).</param>
    byte[] GenererPng(string contenu, int pixelsParModule = 20);
}

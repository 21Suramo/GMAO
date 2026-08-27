using GMAO.Application.Common.Interfaces;
using QRCoder;

namespace GMAO.Infrastructure.Documents;

/// <summary>
/// Génération de QR Codes via QRCoder (rendu PNG indépendant de System.Drawing,
/// donc compatible toutes plateformes).
/// </summary>
public class QrCodeService : IQrCodeService
{
    public byte[] GenererPng(string contenu, int pixelsParModule = 20)
    {
        using var generateur = new QRCodeGenerator();
        using var donnees = generateur.CreateQrCode(contenu, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(donnees);
        return pngQr.GetGraphic(pixelsParModule);
    }
}

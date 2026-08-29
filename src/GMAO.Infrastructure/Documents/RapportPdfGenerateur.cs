using GMAO.Application.Common.Interfaces;
using GMAO.Application.DTOs;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace GMAO.Infrastructure.Documents;

/// <summary>Génération du rapport PDF d'intervention avec iText7.</summary>
public class RapportPdfGenerateur : IRapportPdfGenerateur
{
    private readonly IQrCodeService _qrCode;

    private static readonly DeviceRgb Marine = new(0x20, 0x3A, 0x43);
    private static readonly DeviceRgb Accent = new(0x2C, 0x53, 0x64);
    private static readonly DeviceRgb GrisTexte = new(0x54, 0x6E, 0x7A);
    private static readonly DeviceRgb GrisClair = new(0xEC, 0xEF, 0xF1);

    private PdfFont _bold = null!;

    public RapportPdfGenerateur(IQrCodeService qrCode) => _qrCode = qrCode;

    public byte[] Generer(RapportInterventionData d)
    {
        _bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        using var flux = new MemoryStream();
        var writer = new PdfWriter(flux);
        writer.SetCloseStream(false);

        var pdf = new PdfDocument(writer);
        var doc = new Document(pdf, PageSize.A4);
        doc.SetMargins(36, 40, 36, 40);

        EnTete(doc, d);
        BandeauReference(doc, d);

        SectionTitre(doc, "CLIENT");
        Ligne(doc, "Établissement", d.ClientNom);
        Ligne(doc, "Ville", d.ClientVille);

        SectionTitre(doc, "APPAREIL");
        Ligne(doc, "N° de série", d.AppareilSerie);
        Ligne(doc, "Modèle", $"Datex-Ohmeda {d.AppareilModele}");

        SectionTitre(doc, "INTERVENTION");
        Ligne(doc, "Description", d.Description);
        if (!string.IsNullOrWhiteSpace(d.Diagnostic)) Ligne(doc, "Diagnostic", d.Diagnostic!);
        if (!string.IsNullOrWhiteSpace(d.Cause)) Ligne(doc, "Cause", d.Cause!);
        Ligne(doc, "Ingénieur", d.Ingenieur ?? "Non affecté");

        SectionTitre(doc, "PIÈCES REMPLACÉES");
        TableauPieces(doc, d);

        SectionTitre(doc, "TEMPS & COÛTS");
        Ligne(doc, "Temps de déplacement", $"{d.TempsDeplacement} min");
        Ligne(doc, "Temps de réparation", $"{d.TempsReparation} min");
        Ligne(doc, "Main d'œuvre", $"{d.MainOeuvre:N2} MAD");
        Ligne(doc, "Coût des pièces", $"{d.CoutPieces:N2} MAD");
        TotalCout(doc, d.CoutTotal);

        SectionTitre(doc, "CHECK-LIST DE CONTRÔLE VALIDÉE");
        if (d.CheckListValidee.Count == 0)
            doc.Add(new Paragraph("Aucun contrôle validé.").SetFontColor(GrisTexte).SetFontSize(10));
        else
            foreach (var item in d.CheckListValidee)
                doc.Add(new Paragraph($"[X]  {item}").SetFontSize(10).SetMarginBottom(1));

        Signature(doc);
        PiedDePage(doc);

        doc.Close();
        return flux.ToArray();
    }

    private void EnTete(Document doc, RapportInterventionData d)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).UseAllAvailableWidth();
        table.SetBorder(Border.NO_BORDER);

        var gauche = new Cell().SetBorder(Border.NO_BORDER);
        gauche.Add(new Paragraph("MEDICANA").SetFontSize(24).SetFont(_bold).SetFontColor(Marine).SetMarginBottom(0));
        gauche.Add(new Paragraph("Rapport d'intervention SAV").SetFontSize(13).SetFontColor(Accent).SetMarginTop(2));
        gauche.Add(new Paragraph("Distributeur officiel Datex-Ohmeda").SetFontSize(9).SetFontColor(GrisTexte));
        table.AddCell(gauche);

        var droite = new Cell().SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT);
        try
        {
            var png = _qrCode.GenererPng(d.QrContenu, 6);
            var image = new Image(ImageDataFactory.Create(png)).SetWidth(88).SetHeight(88);
            droite.Add(image);
        }
        catch { /* QR optionnel */ }
        table.AddCell(droite);

        doc.Add(table);
        doc.Add(new Paragraph().SetBorderBottom(new SolidBorder(Marine, 1.5f)).SetMarginTop(4).SetMarginBottom(8));
    }

    private void BandeauReference(Document doc, RapportInterventionData d)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 34, 33, 33 })).UseAllAvailableWidth();
        table.SetBackgroundColor(GrisClair).SetMarginBottom(6);

        table.AddCell(CelluleRef("N° DI", d.NumeroDI));
        table.AddCell(CelluleRef("Date", d.Date.ToString("dd/MM/yyyy")));
        table.AddCell(CelluleRef("État", d.Etat));
        doc.Add(table);
    }

    private Cell CelluleRef(string libelle, string valeur)
    {
        var cell = new Cell().SetBorder(Border.NO_BORDER).SetPadding(8);
        cell.Add(new Paragraph(libelle).SetFontSize(8).SetFontColor(GrisTexte).SetMarginBottom(1));
        cell.Add(new Paragraph(valeur).SetFontSize(12).SetFont(_bold).SetFontColor(Marine));
        return cell;
    }

    private void SectionTitre(Document doc, string titre)
        => doc.Add(new Paragraph(titre).SetFontSize(11).SetFont(_bold).SetFontColor(Accent).SetMarginTop(12).SetMarginBottom(4));

    private void Ligne(Document doc, string libelle, string valeur)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 28, 72 })).UseAllAvailableWidth();
        table.AddCell(new Cell().SetBorder(Border.NO_BORDER)
            .Add(new Paragraph(libelle).SetFontSize(9).SetFontColor(GrisTexte)));
        table.AddCell(new Cell().SetBorder(Border.NO_BORDER)
            .Add(new Paragraph(valeur).SetFontSize(10).SetFontColor(Marine)));
        doc.Add(table);
    }

    private void TableauPieces(Document doc, RapportInterventionData d)
    {
        if (d.Pieces.Count == 0)
        {
            doc.Add(new Paragraph("Aucune pièce remplacée.").SetFontColor(GrisTexte).SetFontSize(10));
            return;
        }

        var table = new Table(UnitValue.CreatePercentArray(new float[] { 22, 40, 10, 14, 14 })).UseAllAvailableWidth();
        foreach (var entete in new[] { "Référence", "Désignation", "Qté", "P.U.", "Total" })
            table.AddHeaderCell(new Cell().SetBackgroundColor(Marine).SetPadding(5)
                .Add(new Paragraph(entete).SetFontColor(ColorConstants.WHITE).SetFontSize(9).SetFont(_bold)));

        foreach (var p in d.Pieces)
        {
            table.AddCell(CelluleData(p.Reference));
            table.AddCell(CelluleData(p.Nom));
            table.AddCell(CelluleData(p.Quantite.ToString()));
            table.AddCell(CelluleData($"{p.PrixUnitaire:N2}"));
            table.AddCell(CelluleData($"{p.Total:N2}"));
        }
        doc.Add(table);
    }

    private static Cell CelluleData(string texte)
        => new Cell().SetPadding(5).SetBorder(new SolidBorder(GrisClair, 0.5f))
            .Add(new Paragraph(texte).SetFontSize(9).SetFontColor(Marine));

    private void TotalCout(Document doc, decimal total)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).UseAllAvailableWidth().SetMarginTop(4);
        table.AddCell(new Cell().SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().SetBackgroundColor(Accent).SetPadding(8)
            .Add(new Paragraph($"TOTAL : {total:N2} MAD").SetFontColor(ColorConstants.WHITE).SetFont(_bold).SetFontSize(12)
                .SetTextAlignment(TextAlignment.RIGHT)));
        doc.Add(table);
    }

    private void Signature(Document doc)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth().SetMarginTop(24);
        table.AddCell(BlocSignature("Signature ingénieur MEDICANA"));
        table.AddCell(BlocSignature("Signature & cachet client"));
        doc.Add(table);
    }

    private static Cell BlocSignature(string libelle)
    {
        var cell = new Cell().SetBorder(Border.NO_BORDER).SetPaddingRight(20);
        cell.Add(new Paragraph(" ").SetMarginBottom(28));
        cell.Add(new Paragraph().SetBorderBottom(new SolidBorder(GrisTexte, 0.7f)));
        cell.Add(new Paragraph(libelle).SetFontSize(8).SetFontColor(GrisTexte).SetMarginTop(2));
        return cell;
    }

    private void PiedDePage(Document doc)
        => doc.Add(new Paragraph("Document généré automatiquement par l'application MEDICANA / Datex-Ohmeda")
            .SetFontSize(8).SetFontColor(GrisTexte).SetTextAlignment(TextAlignment.CENTER).SetMarginTop(20));
}

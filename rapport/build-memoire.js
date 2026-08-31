const fs = require('fs');
const path = require('path');
const D = require('docx');
const {
  Document, Packer, Paragraph, TextRun, HeadingLevel, AlignmentType,
  Table, TableRow, TableCell, WidthType, ShadingType, BorderStyle,
  Footer, PageNumber, NumberFormat, PageBreak, TableOfContents, LevelFormat
} = D;

const SRC = '/home/user/GMAO/rapport';
const OUT = '/home/user/GMAO/rapport/Memoire-PFE-GMAO.docx';

const CM = 567;                 // 1 cm en DXA (twips)
const MARGE = Math.round(2.5 * CM);   // 1417
const RELIURE = Math.round(0.5 * CM); // 283
const LARGEUR_A4 = 11906;
const UTILE = LARGEUR_A4 - 2 * MARGE - RELIURE;   // largeur de table disponible

const POLICE = 'Times New Roman';
const CORPS = 24;               // 12 pt (demi-points)
const INTERLIGNE = { line: 360, lineRule: 'auto' };  // 1,5
const ESP = { before: 120, after: 120 };             // 6 pt / 6 pt

// --- Règle 2.9-9 de la charte : deux espaces entre deux phrases -------------
const RE_PHRASE = /(?<!\bPr)(?<!\bM)(?<!\bMme)(?<!\bcf)(?<!\bfig)(?<!\bp)(?<!\b[A-ZÀ-Ý])([.!?])[ ]+(?=[A-ZÀ-Ý«])/g;
const deuxEspaces = (s) => s.replace(RE_PHRASE, '$1  ');

const estArabe = (s) => /[؀-ۿ]/.test(s);

// --- Analyse des enrichissements en ligne ----------------------------------
function runs(texte, opts = {}) {
  const base = { font: POLICE, size: opts.size || CORPS, ...(opts.bold ? { bold: true } : {}) };
  const out = [];
  // **gras**, *italique*, `code`
  const re = /(\*\*[^*]+\*\*|\*[^*\n]+\*|`[^`]+`)/g;
  let dernier = 0, m;
  const pousser = (t, extra) => { if (t) out.push(new TextRun({ ...base, ...extra, text: t })); };
  while ((m = re.exec(texte)) !== null) {
    pousser(texte.slice(dernier, m.index));
    const jeton = m[0];
    if (jeton.startsWith('**')) pousser(jeton.slice(2, -2), { bold: true });
    else if (jeton.startsWith('`')) pousser(jeton.slice(1, -1), { font: 'Consolas', size: 20 });
    else pousser(jeton.slice(1, -1), { italics: true });
    dernier = m.index + jeton.length;
  }
  pousser(texte.slice(dernier));
  return out.length ? out : [new TextRun({ ...base, text: '' })];
}

function para(texte, opts = {}) {
  const rtl = estArabe(texte);
  return new Paragraph({
    alignment: opts.alignment || (rtl ? AlignmentType.RIGHT : AlignmentType.JUSTIFIED),
    spacing: { ...ESP, ...INTERLIGNE },
    bidirectional: rtl || undefined,
    children: runs(rtl ? texte : deuxEspaces(texte), opts),
  });
}

function titre(texte, niveau, sousTitre) {
  const tailles = { 1: 32, 2: 32, 3: 28, 4: 24 };  // charte : 16 / 16 / 14 / 12 pt
  const enfants = [new TextRun({ font: POLICE, size: tailles[niveau], bold: true, color: '000000', text: texte })];
  if (sousTitre) enfants.push(new TextRun({ font: POLICE, size: tailles[niveau], bold: true, color: '000000', break: 1, text: sousTitre }));
  return new Paragraph({
    heading: [HeadingLevel.HEADING_1, HeadingLevel.HEADING_1, HeadingLevel.HEADING_2, HeadingLevel.HEADING_3][niveau - 1],
    alignment: AlignmentType.LEFT,
    spacing: { before: 240, after: 120, ...INTERLIGNE },
    keepNext: true,                 // charte : jamais un titre en bas de page
    children: enfants,
  });
}

function cellule(texte, entete, largeur) {
  return new TableCell({
    width: { size: largeur, type: WidthType.DXA },
    shading: entete ? { type: ShadingType.CLEAR, fill: 'E7E7E7' } : undefined,
    margins: { top: 60, bottom: 60, left: 90, right: 90 },
    children: [new Paragraph({
      alignment: AlignmentType.LEFT,
      spacing: { before: 40, after: 40, line: 240, lineRule: 'auto' },
      children: runs(texte, { bold: entete, size: 22 }),
    })],
  });
}

function tableau(lignes) {
  const cols = lignes[0].length;
  const base = Math.floor(UTILE / cols);
  const largeurs = Array(cols).fill(base);
  largeurs[cols - 1] = UTILE - base * (cols - 1);
  return new Table({
    columnWidths: largeurs,
    width: { size: UTILE, type: WidthType.DXA },
    rows: lignes.map((l, i) => new TableRow({
      tableHeader: i === 0,
      children: l.map((c, j) => cellule(c, i === 0, largeurs[j])),
    })),
  });
}

// Charte §2.7 : le code source va dans une figure constituée d'un tableau.
function tableauCode(lignes) {
  return new Table({
    columnWidths: [UTILE],
    width: { size: UTILE, type: WidthType.DXA },
    rows: [new TableRow({
      children: [new TableCell({
        width: { size: UTILE, type: WidthType.DXA },
        margins: { top: 120, bottom: 120, left: 150, right: 120 },
        children: lignes.map((l) => new Paragraph({
          alignment: AlignmentType.LEFT,
          spacing: { before: 0, after: 0, line: 240, lineRule: 'auto' },
          children: [new TextRun({ font: 'Consolas', size: 18, text: l || ' ' })],
        })),
      })],
    })],
  });
}

// --- Conversion d'un fichier Markdown en éléments docx ----------------------
function convertir(md) {
  const lignes = md.split('\n');
  const el = [];
  let i = 0;
  while (i < lignes.length) {
    const l = lignes[i];

    if (/^\s*$/.test(l)) { i++; continue; }

    if (l.trim() === '---') { el.push(new Paragraph({ children: [new PageBreak()] })); i++; continue; }

    if (l.startsWith('```')) {          // bloc de code -> figure-tableau
      const code = []; i++;
      while (i < lignes.length && !lignes[i].startsWith('```')) code.push(lignes[i++]);
      i++;
      el.push(tableauCode(code));
      continue;
    }

    if (l.startsWith('|')) {           // tableau Markdown
      const brut = [];
      while (i < lignes.length && lignes[i].startsWith('|')) brut.push(lignes[i++]);
      const cellules = brut
        .filter((r) => !/^\|[\s:|-]+\|$/.test(r.replace(/\s/g, '')))
        .map((r) => r.replace(/^\||\|$/g, '').split('|').map((c) => c.trim()));
      if (cellules.length) el.push(tableau(cellules));
      el.push(new Paragraph({ spacing: { after: 120 }, children: [] }));
      continue;
    }

    if (l.startsWith('#')) {
      const m = l.match(/^(#{1,4})\s+(.*)$/);
      const niveau = m[1].length;
      let texte = m[2].trim();
      // « Chapitre N » suivi du titre : un seul titre sur deux lignes
      if (niveau === 1 && /^Chapitre\s+\d+$/i.test(texte)) {
        let j = i + 1;
        while (j < lignes.length && /^\s*$/.test(lignes[j])) j++;
        if (j < lignes.length && /^#\s+/.test(lignes[j])) {
          el.push(titre(texte, 1, lignes[j].replace(/^#\s+/, '').trim()));
          i = j + 1; continue;
        }
      }
      el.push(titre(texte, niveau));
      i++; continue;
    }

    if (/^[-*]\s+/.test(l)) {          // liste à puces
      while (i < lignes.length && /^[-*]\s+/.test(lignes[i])) {
        el.push(new Paragraph({
          numbering: { reference: 'puces', level: 0 },
          alignment: AlignmentType.JUSTIFIED,
          spacing: { before: 40, after: 40, ...INTERLIGNE },
          children: runs(deuxEspaces(lignes[i].replace(/^[-*]\s+/, ''))),
        }));
        i++;
      }
      continue;
    }

    // paragraphe : agréger les lignes consécutives
    const bloc = [];
    while (i < lignes.length && !/^\s*$/.test(lignes[i]) && !lignes[i].startsWith('#')
           && !lignes[i].startsWith('|') && !lignes[i].startsWith('```')
           && lignes[i].trim() !== '---' && !/^[-*]\s+/.test(lignes[i])) {
      bloc.push(lignes[i++]);
    }
    const texte = bloc.join(' ').trim();
    if (!texte) continue;

    // Légendes de figure / tableau : centrées, non justifiées
    const legende = /^\*\*(Figure|Tableau)\s+\d+\s+-\s+/.test(texte);
    el.push(para(texte, legende ? { alignment: AlignmentType.CENTER } : {}));
  }
  return el;
}

const lire = (f) => fs.readFileSync(path.join(SRC, f), 'utf8');

// --- Découpage des liminaires : page de garde / reste ------------------------
const liminaires = lire('00-liminaires.md');
const morceaux = liminaires.split(/\n---\n/);
const pageDeGarde = morceaux[0];

// La page de garde suit la mise en page du modèle officiel : tout est centré,
// le titre du sujet en haut, le jury en bas. Le marqueur « # Page de garde »
// est structurel et ne doit pas apparaître dans le document.
function elementsPageDeGarde(md) {
  const espace = (n) => new Paragraph({ spacing: { before: 0, after: n }, children: [] });
  const ligne = (texte, opts = {}) => new Paragraph({
    alignment: AlignmentType.CENTER,
    spacing: { before: opts.avant || 0, after: opts.apres || 60, ...INTERLIGNE },
    children: runs(texte, { size: opts.size || CORPS }),
  });
  const brut = md.split('\n').map((l) => l.trim())
    .filter((l) => l && !/^#\s/.test(l));
  const out = [espace(240)];
  brut.forEach((l) => {
    const puce = /^-\s+/.test(l);
    const titreSujet = /^\*\*[A-ZÉÀ].{30,}\*\*$/.test(l);
    out.push(ligne(l.replace(/^-\s+/, ''), {
      size: titreSujet ? 32 : CORPS,
      apres: puce ? 40 : (titreSujet ? 200 : 120),
      avant: titreSujet ? 200 : 0,
    }));
  });
  return out;
}
const resteLiminaires = morceaux.slice(1).join('\n---\n');

const coupe = resteLiminaires.indexOf('# Liste des abréviations');
if (coupe < 0) throw new Error('Section « Liste des abréviations » introuvable dans les liminaires.');
const avantSommaire = resteLiminaires.slice(0, coupe).replace(/\n---\n\s*$/, '');
const apresSommaire = resteLiminaires.slice(coupe);

const corps = ['05-introduction-generale.md', '01-contexte.md', '02-analyse.md',
  '03-conception.md', '04-realisation.md', '06-conclusion-generale.md',
  '07-bibliographie.md', '08-annexes.md'];

const elementsCorps = [];
corps.forEach((f, idx) => {
  if (idx > 0) elementsCorps.push(new Paragraph({ children: [new PageBreak()] }));
  elementsCorps.push(...convertir(lire(f)));
});

const piedCentre = new Footer({
  children: [new Paragraph({
    alignment: AlignmentType.CENTER,
    children: [new TextRun({ font: POLICE, size: 20, children: [PageNumber.CURRENT] })],
  })],
});

const pageProps = (formatType, start) => ({
  page: {
    margin: { top: MARGE, right: MARGE, bottom: MARGE, left: MARGE, gutter: RELIURE },
    pageNumbers: { start, formatType },
  },
});

const doc = new Document({
  creator: 'PFE GMAO Datex-Ohmeda',
  title: 'Mémoire de projet de fin d’études — GMAO Datex-Ohmeda',
  numbering: {
    config: [{
      reference: 'puces',
      levels: [{ level: 0, format: LevelFormat.BULLET, text: '•', alignment: AlignmentType.LEFT,
        style: { paragraph: { indent: { left: 720, hanging: 360 } } } }],
    }],
  },
  styles: {
    default: {
      document: { run: { font: POLICE, size: CORPS }, paragraph: { spacing: { ...ESP, ...INTERLIGNE } } },
      heading1: { run: { font: POLICE, size: 32, bold: true, color: '000000' } },
      heading2: { run: { font: POLICE, size: 32, bold: true, color: '000000' } },
      heading3: { run: { font: POLICE, size: 28, bold: true, color: '000000' } },
      heading4: { run: { font: POLICE, size: 24, bold: true, color: '000000' } },
    },
  },
  sections: [
    // Section 1 — page de garde, non paginée
    { properties: pageProps(NumberFormat.LOWER_ROMAN, 1), children: elementsPageDeGarde(pageDeGarde) },
    // Section 2 — liminaires, pagination romaine
    // Ordre imposé par la charte : dédicaces, remerciements, résumés FR/EN/AR,
    // table des matières, puis les trois listes.
    { properties: pageProps(NumberFormat.LOWER_ROMAN, 1), footers: { default: piedCentre },
      children: [
        ...convertir(avantSommaire),
        new Paragraph({ children: [new PageBreak()] }),
        titre('Table des matières', 1),
        new TableOfContents('Sommaire', { hyperlink: true, headingStyleRange: '1-3' }),
        new Paragraph({
          alignment: AlignmentType.LEFT,
          spacing: { before: 240, after: 120, ...INTERLIGNE },
          children: [new TextRun({ font: POLICE, size: 20, italics: true, color: '808080',
            text: '[Champ de table des matières — sous Word : clic droit sur ce champ puis « Mettre à jour les champs » pour générer les entrées et leurs numéros de page. Supprimer cette note.]' })],
        }),
        new Paragraph({ children: [new PageBreak()] }),
        ...convertir(apresSommaire),
      ] },
    // Section 3 — corps, pagination arabe repartant à 1
    { properties: pageProps(NumberFormat.DECIMAL, 1), footers: { default: piedCentre },
      children: elementsCorps },
  ],
});

Packer.toBuffer(doc).then((buf) => {
  fs.writeFileSync(OUT, buf);
  console.log('Écrit :', OUT, (buf.length / 1024).toFixed(0) + ' Ko');
});

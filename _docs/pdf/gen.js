const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const docsDir = path.resolve(__dirname, '..');
const outDir = __dirname;
const cssFile = path.join(outDir, 'pdf-style.css');

const docs = [
  { file: 'gdd.md', title: 'Game Design Document' },
  { file: 'ui-ux.md', title: 'UI/UX Design' },
  { file: 'jalons.md', title: 'Jalons de Production' },
  { file: '2d-art.md', title: '2D Art Assets' },
  { file: '2d-animation.md', title: '2D Animation' },
  { file: 'sound-design.md', title: 'Sound Design' },
  { file: 'sound-design-minimal.md', title: 'Sound Design - Kit Minimal' },
];

for (const doc of docs) {
  const input = path.join(docsDir, doc.file);
  const output = path.join(outDir, doc.file.replace('.md', '.pdf'));
  const tmpFile = path.join(outDir, '_tmp_' + doc.file);

  if (!fs.existsSync(input)) { console.log('SKIP:', doc.file); continue; }
  console.log('Generating', doc.file, '...');

  const frontmatter = `---
pdf_options:
  format: A4
  margin:
    top: 20mm
    bottom: 25mm
    left: 15mm
    right: 15mm
  displayHeaderFooter: true
  headerTemplate: '<div style="width:100%;font-size:8px;font-family:Inter,sans-serif;padding:0 15mm;display:flex;justify-content:space-between;color:#E17055;"><span style="font-weight:600;">CAT HOTEL TYCOON</span><span>${doc.title}</span></div>'
  footerTemplate: '<div style="width:100%;font-size:8px;font-family:Inter,sans-serif;padding:0 15mm;display:flex;justify-content:space-between;color:#636E72;"><span>Royal Pourceau Studios - Mars 2026</span><span>Page <span class="pageNumber"></span> / <span class="totalPages"></span></span></div>'
dest: "${output.replace(/\\/g, '/')}"
stylesheet: "${cssFile.replace(/\\/g, '/')}"
---
`;

  fs.writeFileSync(tmpFile, frontmatter + fs.readFileSync(input, 'utf8'), 'utf8');
  try {
    execSync(`npx md-to-pdf "${tmpFile.replace(/\\/g, '/')}"`, { stdio: 'inherit', timeout: 60000 });
    console.log('  OK:', path.basename(output));
  } catch (err) { console.error('  FAIL:', doc.file, err.message); }
  finally { fs.unlinkSync(tmpFile); }
}

// Pitch deck (Marp)
const pitchInput = path.join(docsDir, 'pitch-deck.md');
const pitchOutput = path.join(outDir, 'pitch-deck.pdf');
if (fs.existsSync(pitchInput)) {
  console.log('Generating pitch-deck.pdf (Marp)...');
  try {
    execSync(`npx @marp-team/marp-cli "${pitchInput.replace(/\\/g, '/')}" --pdf -o "${pitchOutput.replace(/\\/g, '/')}"`, { stdio: 'inherit', timeout: 60000 });
    console.log('  OK: pitch-deck.pdf');
  } catch (err) { console.error('  FAIL: pitch-deck.pdf', err.message); }
}

console.log('\nDone!');

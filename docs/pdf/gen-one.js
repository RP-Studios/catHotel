const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const docFile = process.argv[2];
const title = process.argv[3] || docFile;

const docsDir = path.resolve(__dirname, '..');
const outDir = __dirname;
const cssFile = path.join(outDir, 'pdf-style.css');
const input = path.join(docsDir, docFile);
const output = path.join(outDir, docFile.replace('.md', '.pdf'));
const tmpFile = path.join(outDir, '_tmp_' + docFile);

console.log('Generating', docFile, '...');

const frontmatter = `---
pdf_options:
  format: A4
  margin:
    top: 20mm
    bottom: 25mm
    left: 15mm
    right: 15mm
  displayHeaderFooter: true
  headerTemplate: '<div style="width:100%;font-size:8px;font-family:Inter,sans-serif;padding:0 15mm;display:flex;justify-content:space-between;color:#E17055;"><span style="font-weight:600;">CAT HOTEL TYCOON</span><span>${title}</span></div>'
  footerTemplate: '<div style="width:100%;font-size:8px;font-family:Inter,sans-serif;padding:0 15mm;display:flex;justify-content:space-between;color:#636E72;"><span>Royal Pourceau Studios - Mars 2026</span><span>Page <span class="pageNumber"></span> / <span class="totalPages"></span></span></div>'
dest: "${output.replace(/\\/g, '/')}"
stylesheet: "${cssFile.replace(/\\/g, '/')}"
---
`;

fs.writeFileSync(tmpFile, frontmatter + fs.readFileSync(input, 'utf8'), 'utf8');
try {
  execSync(`npx md-to-pdf "${tmpFile.replace(/\\/g, '/')}"`, { stdio: 'inherit', timeout: 60000 });
  console.log('  OK:', path.basename(output));
} catch (err) {
  console.error('  FAIL:', err.message);
} finally {
  fs.unlinkSync(tmpFile);
}

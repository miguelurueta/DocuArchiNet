const test = require('node:test');
const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const expected = {
  'generic_control/FileUploadHandler_.ashx.vb': '8a42b7439f754e23d257e67a3591e46f99063c648d01847ba6aa891ddbdff6c1',
  'js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js': '3c780836a0f379bc1dd0cfd901d122d1066ada4f219dd79aa868ab359324e645'
};

for (const [relativePath, fingerprint] of Object.entries(expected)) {
  test(`${relativePath} conserva la huella aprobada tras DOC-90`, () => {
    const content = fs.readFileSync(path.join(root, ...relativePath.split('/')), 'utf8').replace(/\r\n/g, '\n');
    const actual = crypto.createHash('sha256').update(content).digest('hex');
    assert.equal(actual, fingerprint);
  });
}

test('la proyección compartida limita la inserción nueva a ADJUNTARADICACION', () => {
  const source = fs.readFileSync(path.join(root, 'generic_control', 'FileUploadHandler.js'), 'utf8');
  const start = source.indexOf('if (this.settings.funcion_name == "adjunta_documeto_version_document")');
  const end = source.indexOf('if (this.settings.funcion_name == "adjunta_nueva_version_document")', start);
  assert.ok(start >= 0 && end > start);
  const branch = source.slice(start, end);
  assert.match(branch, /evento_adjunta == "ADJUNTARADICACION"[\s\S]*insert_row_table\(this\.settings\.element_html_table, row\)/);
  assert.match(branch, /else\s*\{[\s\S]*updateCelByUniqueIdReinit/);
  assert.doesNotMatch(branch, /location\.reload|__doPostBack/);
});

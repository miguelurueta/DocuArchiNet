const test = require('node:test');
const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const expected = {
  'generic_control/FileUploadHandler_.ashx.vb': '8459dd56d2abed043203c21a0eea2f2ae31fc8035f9c9e386fb774bd867a3b73',
  'js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js': 'f29fbd798ccb492fa924a522ae5dcb19d7f280b55f0f9c4e71b016cd5f026b69'
};

for (const [relativePath, fingerprint] of Object.entries(expected)) {
  test(`${relativePath} conserva la huella aprobada por DOC-85`, () => {
    const content = fs.readFileSync(path.join(root, ...relativePath.split('/')));
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

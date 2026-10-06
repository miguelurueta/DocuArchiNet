const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const source = fs.readFileSync(path.join(root, 'webservice', 'WebServiceProducion.asmx.vb'), 'utf8');

test('PRODUCCION rechaza YES sin configuración materializada', () => {
  const start = source.indexOf('Case "PRODUCCION"');
  const end = source.indexOf('Case "WORKFLOW"', start);
  const branch = source.slice(start, end);

  assert.match(branch, /ra_config_upload_gestion\.ID_CONFIG_UPLOAD_GESTION <= 0 OrElse/);
  assert.match(branch, /String\.IsNullOrWhiteSpace\(ra_config_upload_gestion\.EXTENSION_UPLOAD\)/);
  assert.match(branch, /ra_config_upload_gestion\.LENG_UPLOAD <= 0/);
  assert.match(branch, /PRODUCCION_CARGA_CONFIGURACION_INCOMPLETA/);
  assert.ok(branch.indexOf('ID_CONFIG_UPLOAD_GESTION <= 0') < branch.indexOf('parameter_upload.ExtensionPermitida'));
});

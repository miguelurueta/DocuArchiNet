const test = require('node:test');
const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const handlerPath = path.join(root, 'generic_control', 'FileUploadHandler_.ashx.vb');
const source = fs.readFileSync(handlerPath, 'utf8');

test('el handler servidor compartido conserva la huella de DOC-85', () => {
  const fingerprint = crypto.createHash('sha256').update(fs.readFileSync(handlerPath)).digest('hex');
  assert.equal(fingerprint, '8459dd56d2abed043203c21a0eea2f2ae31fc8035f9c9e386fb774bd867a3b73');
});

test('el handler sanea por etapa únicamente la excepción de PRODUCCION', () => {
  assert.match(source, /If eventoAdjuntaActual = "PRODUCCION" Then[\s\S]*uploadFiles\.error_sistema = etapaCargaProduccion[\s\S]*Else[\s\S]*uploadFiles\.error_sistema = ex\.Message/);
  assert.match(source, /PRODUCCION_CARGA_HANDLER_ALMACENAMIENTO_INVALIDO/);
  assert.match(source, /PRODUCCION_CARGA_HANDLER_RESPUESTA_INVALIDA/);
});

test('el handler acepta radicado_radicacion ausente en la carga de PRODUCCION', () => {
  assert.match(source, /If\(context\.Request\("radicado_radicacion"\), String\.Empty\)\.Trim\(\)/);
  assert.doesNotMatch(source, /Convert\.ToString\(context\.Request\("radicado_radicacion"\)\)\.Trim\(\)/);
});

test('la rama PRODUCCION conserva el contrato UploadFilesResult y el error terminal', () => {
  const start = source.indexOf('If evento_adjunta = "PRODUCCION" Then');
  const end = source.indexOf('If evento_adjunta = "SUBE_RESPUESTA" Then', start);
  const branch = source.slice(start, end);

  assert.match(branch, /If Result <> "YES" Then\s+uploadFiles\.error_sistema = Result\s+Else/);
  for (const field of ['name_gabinete', 'id_image', 'tipodocumental', 'id_registro', 'fecha', 'aleas', 'nombre_archivo']) {
    assert.match(branch, new RegExp('uploadFiles\\.' + field + ' = stru_datos_image_lista\\.'));
  }
  assert.doesNotMatch(branch, /Solicita_id_inventario_documental|ExisteDocumento|SELECT/);
});

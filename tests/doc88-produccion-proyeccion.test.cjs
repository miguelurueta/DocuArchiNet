const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const source = fs.readFileSync(path.join(root, 'generic_control', 'FileUploadHandler.js'), 'utf8');

test('la nueva proyección queda aislada al evento PRODUCCION y usa el id de imagen real', () => {
  const start = source.indexOf('async _RegistraArchivoInterfaz');
  const end = source.indexOf('if (this.settings.funcion_name == "insert_row_producion_documental" && this.settings.evento_adjunta != "PRODUCCION")', start);
  const method = source.slice(start, end);

  assert.match(method, /funcion_name == "insert_row_producion_documental" && this\.settings\.evento_adjunta == "PRODUCCION"/);
  assert.match(method, /PRODUCCION_CARGA_PROYECCION_FALLIDA/);
  assert.match(method, /estado_firma_digital[\s\S]*FileIconSome[\s\S]*id_image/);
  assert.doesNotMatch(method, /id_imageinsert_row_documento_relacionado/);
  assert.doesNotMatch(method, /insert_row_producion_documental\(DateCampo\);\s*insert_row_documento_relacionado/);
});

test('el bloqueo de doble activación existe solo para PRODUCCION y siempre se libera', () => {
  const bulkStart = source.indexOf('async _EventEnviarArchivosServer');
  const bulkEnd = source.indexOf('async _EventEnviarArchivoServer', bulkStart);
  const singleEnd = source.indexOf('async _EventDragLeave', bulkEnd);
  const methods = source.slice(bulkStart, singleEnd);

  assert.match(methods, /evento_adjunta == "PRODUCCION"/);
  assert.match(methods, /_ProduccionCargaActiva === true/);
  assert.match(methods, /finally[\s\S]*_ProduccionCargaActiva = false/);
});

test('un rechazo o una falla visual no vuelve a enviar el archivo', () => {
  const start = source.indexOf('async _EnviaArchivoServidor');
  const end = source.indexOf('async _RegistraArchivoInterfaz', start);
  const method = source.slice(start, end);

  assert.equal((method.match(/fetch\(/g) || []).length, 1);
  assert.doesNotMatch(method, /Solicita_id_inventario_documental|ExisteDocumento|retry|reintento/i);
  assert.match(method, /PRODUCCION_CARGA_CONFIRMACION_INCIERTA/);
});

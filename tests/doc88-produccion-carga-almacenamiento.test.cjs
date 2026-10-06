const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const source = fs.readFileSync(path.join(root, 'workflow', 'ClassAlmacenamiento.vb'), 'utf8');

test('el almacenamiento de producción valida archivo, expediente y lista antes de desreferenciar', () => {
  const start = source.indexOf('Function AlmacenamientoDocumentoProduccionDocumental');
  const end = source.indexOf('End Function', start);
  const method = source.slice(start, end);

  assert.match(method, /PRODUCCION_CARGA_ARCHIVO_NO_DISPONIBLE/);
  assert.match(method, /If EsctructuraExpediente Is Nothing OrElse EsctructuraExpediente\.Length = 0 Then/);
  assert.ok(method.indexOf('Length = 0') < method.indexOf('EsctructuraExpediente(0)'));
  assert.match(method, /If CDcamposAsignaAlmacenamiento IsNot Nothing AndAlso CDcamposAsignaAlmacenamiento\.Count > 0 Then/);
});

test('el prealmacenamiento rechaza contexto, tipología y gabinete no materializados', () => {
  const start = source.indexOf('Function PreAlmacenaDocumentoProduccion');
  const end = source.indexOf('Function AlmacenaDocumentoTareaWorkflow', start);
  const method = source.slice(start, end);

  assert.match(method, /IdExpediente <= 0 OrElse IdTipoLogia <= 0/);
  assert.match(method, /String\.IsNullOrWhiteSpace\(DescripcionTipo\)/);
  assert.match(method, /String\.IsNullOrWhiteSpace\(NombreGabinete\)/);
  assert.match(method, /If IdGabineteDocuarchi <= 0 Then/);
});

test('un error de Almacenamiento es terminal y no inicia una consulta de existencia', () => {
  const start = source.indexOf('Function AlmacenamientoDocumentoProduccionDocumental');
  const end = source.indexOf('End Function', start);
  const method = source.slice(start, end);

  assert.match(method, /Result = Me\.Almacenamiento\(/);
  assert.match(method, /If Result <> "YES" Then[\s\S]*AlmacenamientoDocumentoProduccionDocumental = "PRODUCCION_CARGA_PERSISTENCIA_RECHAZADA"[\s\S]*Exit Function/);
  assert.doesNotMatch(method, /Solicita_id_inventario_documental|SELECT|ExisteRecibo|ExisteDocumento/);
});

test('un tipo inválido de campo fecha retorna un código saneado de producción', () => {
  const start = source.indexOf('Function PreAlmacenaDocumentoProduccion');
  const end = source.indexOf('Function AlmacenaDocumentoTareaWorkflow', start);
  const method = source.slice(start, end);

  assert.match(method, /If CDCamposFechaGabinetePro\.Item\(i\)\.Tipo <> "DATE" Then[\s\S]*PreAlmacenaDocumentoProduccion = "PRODUCCION_CARGA_CAMPOS_FECHA_INVALIDOS"/);
  assert.doesNotMatch(method, /No es posible actualizar el campo/);
});

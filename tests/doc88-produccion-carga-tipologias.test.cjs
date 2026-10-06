const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const source = fs.readFileSync(path.join(root, 'Gestion', 'ClassGaProducionDocumental.vb'), 'utf8');

test('las tipologías validan la estructura del expediente antes de usar el índice cero', () => {
  const start = source.indexOf('Function SolicitaListaTipologiasExpediente');
  const end = source.indexOf('Function Solicitar_agregar_documento_a_carpeta_expediente', start);
  const method = source.slice(start, end);

  assert.match(method, /If EstruUnidadConservacion Is Nothing OrElse EstruUnidadConservacion\.Length = 0 Then/);
  assert.match(method, /PRODUCCION_CARGA_CONTEXTO_INVALIDO/);
  assert.ok(method.indexOf('Length = 0') < method.indexOf('EstruUnidadConservacion(0)'));
});

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const source = fs.readFileSync(path.join(root, 'Gestion', 'ClassGaProducionDocumental.vb'), 'utf8');

test('la selección de producción se valida antes de dividir y autorizar', () => {
  const start = source.indexOf('Function SolicitaCargarDocumentoExpediente');
  const end = source.indexOf('Function SolicitaListaTipologiasExpediente', start);
  const method = source.slice(start, end);

  assert.match(method, /String\.IsNullOrWhiteSpace\(SeleccionProduccion\)/);
  assert.match(method, /SeleccionPartes\.Length < 3/);
  assert.match(method, /IdExpediente <= 0 OrElse[\s\S]*IdNivelExpediente <= 0/);
  assert.match(method, /IdUsuarioGestion <= 0/);
  assert.ok(method.indexOf('SeleccionPartes.Length < 3') < method.indexOf('SeleccionPartes(2)'));
});

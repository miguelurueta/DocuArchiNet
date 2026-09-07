const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const factory = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultFactory.vb', 'utf8');
const machine = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb', 'utf8');

test('incertidumbre no es reintentable y los fallos conocidos se clasifican', () => {
  assert.match(factory, /Reintentable = resultado\.Reintentable AndAlso item\.PersistenciaConocida/);
  assert.match(factory, /ResultadoIncierto/);
  assert.match(factory, /FallidaAntesDePersistir/);
  assert.match(factory, /Parcial/);
});

test('cada fase mutadora puede terminar parcial o incierta', () => {
  for (const phase of ['RecursoObtenido', 'ExpedientePreparado', 'IndicesActualizados', 'DocumentoAlmacenado']) {
    const line = machine.split(/\r?\n/).find(x => new RegExp(`Agregar\\(t, FaseImportacionServicio\\.${phase},`).test(x));
    assert.match(line, /Parcial/);
    assert.match(line, /ResultadoIncierto/);
  }
});

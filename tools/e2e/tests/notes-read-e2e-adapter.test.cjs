'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const { NOTES_READ_E2E_ADAPTER } = require('../scripts/adapters/notes-read-e2e-adapter.cjs');

test('el adaptador notes-read limita sus tres operaciones y conserva solo evidencia saneada', async () => {
  const calls = [];
  const evidence = await NOTES_READ_E2E_ADAPTER.executeRead({
    taskId: 708,
    budgetMs: 10000,
    invoke: async (operation, payload) => {
      calls.push({ operation, payload });
      if (operation === 'ConsultarNota') return { dto: { Exito: true, Nota: { Version: 1 } }, elapsedMs: 5 };
      if (payload.cursor === 'cursor-invalido-e2e') return { dto: { Exito: false, Error: { Codigo: 'CURSOR_INVALIDO' } }, elapsedMs: 4 };
      return { dto: { Exito: true, Notas: [{ IdNota: 13, Contenido: 'no-persistir' }] }, elapsedMs: 3 };
    }
  });
  assert.deepEqual(calls.map((call) => call.operation), ['ListarNotas', 'ConsultarNota', 'ListarNotas']);
  assert.deepEqual(calls[0].payload, { idTarea: 708, cursor: '', tamanoPagina: 1 });
  assert.deepEqual(calls[2].payload, { idTarea: 708, cursor: 'cursor-invalido-e2e', tamanoPagina: 1 });
  assert.deepEqual(evidence, {
    codes: { list: null, get: null, invalidCursor: 'CURSOR_INVALIDO' },
    count: 1,
    latenciesMs: [3, 5, 4]
  });
  assert.doesNotMatch(JSON.stringify(evidence), /Contenido|no-persistir|IdNota|13/);
});

test('el adaptador bloquea una respuesta de cursor que no sea funcionalmente rechazada', async () => {
  await assert.rejects(() => NOTES_READ_E2E_ADAPTER.executeRead({
    taskId: 708,
    budgetMs: 1000,
    invoke: async (operation, payload) => {
      if (operation === 'ConsultarNota') return { dto: { Exito: true }, elapsedMs: 1 };
      if (payload.cursor) return { dto: { Exito: true }, elapsedMs: 1 };
      return { dto: { Exito: true, Notas: [{ IdNota: 9 }] }, elapsedMs: 1 };
    }
  }), { code: 'NOTES_READ_CURSOR_NOT_BLOCKED' });
});

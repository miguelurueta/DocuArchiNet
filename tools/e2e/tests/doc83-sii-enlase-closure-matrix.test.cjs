'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const { loadDoc83ClosureMatrix, validateDoc83ClosureMatrix } = require('../scripts/support/doc83-sii-enlase-closure-matrix.cjs');

const clone = (value) => JSON.parse(JSON.stringify(value));

test('la matriz DOC-83 enlaza riesgos altos con pruebas, escenarios registrados y evidencia existente', () => {
  assert.deepEqual(validateDoc83ClosureMatrix(loadDoc83ClosureMatrix()), { scenarios: 6, risks: 10 });
});

test('la matriz DOC-83 falla si una referencia determinística no existe', () => {
  const manifest = clone(loadDoc83ClosureMatrix());
  manifest.risks[0].tests[0] = 'tests/referencia-ausente.test.cjs';
  assert.throws(() => validateDoc83ClosureMatrix(manifest), { code: 'DOC83_MATRIX_REFERENCE_MISSING' });
});

test('la matriz DOC-83 falla si un escenario no coincide con el registro', () => {
  const manifest = clone(loadDoc83ClosureMatrix());
  manifest.scenarios[0].stage = 'execution';
  assert.throws(() => validateDoc83ClosureMatrix(manifest), { code: 'DOC83_MATRIX_SCENARIO_MISMATCH' });
});

test('la matriz DOC-83 rechaza una brecha E2E declarada sobre un riesgo cerrado', () => {
  const manifest = clone(loadDoc83ClosureMatrix());
  manifest.risks.find((risk) => risk.id === 'R-DOC83-05').realGap = {
    operation: 'successful-explicit-assignment',
    requiredAuthorizations: ['environment', 'gate', 'execution', 'discardable-resource'],
    resourcePolicy: 'distinct-lifecycle-reservation'
  };
  assert.throws(() => validateDoc83ClosureMatrix(manifest), { code: 'DOC83_MATRIX_REAL_GAP_INVALID' });
});

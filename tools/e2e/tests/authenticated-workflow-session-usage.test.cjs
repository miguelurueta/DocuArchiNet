'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const e2eRoot = path.resolve(__dirname, '..');
const consumers = [
  'tests/doc10-preview.spec.cjs',
  'tests/doc11-execution.spec.cjs',
  'scripts/run-doc10-concurrency.cjs',
  'scripts/run-doc11-concurrency.cjs',
  'tests/doc32-return-activity.spec.cjs',
  'scripts/run-doc32-return-activity-concurrency.cjs',
  'tests/notes-workflow.spec.cjs',
  'scripts/run-notes-workflow-concurrency.cjs'
];
const loginSelectors = /ContentPlacenter_(?:DropDownListmodulos|TextBoxuser|TextBoxpasw)|a\.da-login-submit/;
const notesSupport = fs.readFileSync(path.join(e2eRoot, 'tests', 'support', 'notes-workflow-e2e.cjs'), 'utf8');

test('las suites autenticadas reutilizan el helper y no copian selectores de login', () => {
  for (const relativePath of consumers) {
    const source = fs.readFileSync(path.join(e2eRoot, relativePath), 'utf8');
    assert.match(source, /createAuthenticatedWorkflowSession|notes-workflow-e2e\.cjs/, `${relativePath} debe usar el helper de sesión.`);
    assert.doesNotMatch(source, loginSelectors, `${relativePath} no debe duplicar selectores de login.`);
  }
  assert.match(notesSupport, /createAuthenticatedWorkflowSession/, 'El soporte de Notas debe reutilizar el helper de sesión.');
});

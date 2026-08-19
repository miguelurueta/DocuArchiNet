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
  'scripts/run-doc11-concurrency.cjs'
];
const loginSelectors = /ContentPlacenter_(?:DropDownListmodulos|TextBoxuser|TextBoxpasw)|a\.da-login-submit/;

test('las suites autenticadas reutilizan el helper y no copian selectores de login', () => {
  for (const relativePath of consumers) {
    const source = fs.readFileSync(path.join(e2eRoot, relativePath), 'utf8');
    assert.match(source, /createAuthenticatedWorkflowSession/, `${relativePath} debe usar el helper de sesión.`);
    assert.doesNotMatch(source, loginSelectors, `${relativePath} no debe duplicar selectores de login.`);
  }
});

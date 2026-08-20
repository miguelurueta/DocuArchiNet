'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const e2eRoot = path.resolve(__dirname, '..');
const repositoryRoot = path.resolve(e2eRoot, '..', '..');

function readE2e(relativePath) {
  return fs.readFileSync(path.join(e2eRoot, relativePath), 'utf8');
}

test('el arnés DOC-10 usa contextos oficiales y no un escenario de piloto', () => {
  const packageJson = JSON.parse(readE2e('package.json'));
  const configurationSource = readE2e('scripts/assert-e2e-config.cjs');
  const previewSource = readE2e('tests/doc10-preview.spec.cjs');

  assert.match(packageJson.scripts['test:contexts'], /assert-e2e-config\.cjs contexts/);
  assert.equal(packageJson.scripts['test:authorization'], undefined);
  assert.match(configurationSource, /contexts:\s*\[/);
  assert.doesNotMatch(configurationSource, /authorization:\s*\[/);
  assert.match(configurationSource, /DOC10_E2E_SECONDARY_USER/);
  assert.doesNotMatch(configurationSource, /DOC10_E2E_UNAUTHORIZED_/);
  assert.match(previewSource, /test\('@contexts Dos cuentas Gestión válidas/);
  assert.doesNotMatch(previewSource, /test\('@authorization/);
  assert.match(previewSource, /expectOfficialWorkflowContext/);
  assert.match(previewSource, /not\.toBe\('WORKFLOW_MODERN_INACTIVE'\)/);
  assert.doesNotMatch(previewSource, /DOC10_E2E_UNAUTHORIZED_/);
});

test('la E2E completa conserva una única cuenta principal y huellas de lectura', () => {
  const previewSource = readE2e('tests/doc10-preview.spec.cjs');
  const start = previewSource.indexOf("test('@full");
  const fullScenario = previewSource.slice(start);

  assert.ok(start >= 0);
  assert.match(fullScenario, /primaryContext = await login/);
  assert.doesNotMatch(fullScenario, /secondaryContext|UNAUTHORIZED|noAutorizado/);
  assert.match(fullScenario, /assertReadOnlySql\(settings\.taskStateSql/);
  assert.match(fullScenario, /expect\(afterTask/);
  assert.match(fullScenario, /expect\(afterAudit/);
  assert.match(fullScenario, /principal:/);
});

test('documentación y alternativa PowerShell no reactivan el gate', () => {
  const readme = readE2e('README.md');
  const runbook = readE2e('AGENT-RUNBOOK.md');
  const powershell = readE2e('Invoke-Doc10PreviewE2E.ps1');
  const concurrency = readE2e('scripts/run-doc10-concurrency.cjs');
  const verifier = fs.readFileSync(path.join(repositoryRoot, 'tools', 'validation', 'Verify-Doc10Preview.ps1'), 'utf8');

  assert.match(readme, /test:contexts/);
  assert.match(runbook, /No activar, editar ni limitar/);
  assert.doesNotMatch(readme, /test:authorization|DOC10_E2E_UNAUTHORIZED|no-piloto/);
  assert.doesNotMatch(runbook, /test:authorization|DOC10_E2E_UNAUTHORIZED|habilitarlo temporalmente/);
  assert.match(powershell, /\$PrimaryCredential/);
  assert.doesNotMatch(powershell, /UnauthorizedCredential|noAutorizado/);
  assert.match(concurrency, /rolAutenticado: 'contexto-workflow-valido'/);
  assert.match(verifier, /WORKFLOW_MODERN_OFFICIAL/);
  assert.doesNotMatch(verifier, /WORKFLOW_MODERN_INACTIVE|New-EnabledGate/);
});

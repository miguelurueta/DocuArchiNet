const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const e2eRoot = path.join(root, 'tools', 'e2e');

function read(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), 'utf8');
}

test('DOC-85 integra una única E2E real sobre la sesión autenticada común', () => {
  const suite = read('tools/e2e/tests/radicacion-simple-attachment.spec.cjs');
  const runner = read('tools/e2e/scripts/run-radicacion-simple-attachment-interactive.cjs');
  const validator = read('tools/e2e/scripts/assert-radicacion-simple-attachment-config.cjs');
  const packageJson = JSON.parse(read('tools/e2e/package.json'));

  assert.match(suite, /createAuthenticatedWorkflowSession/);
  assert.match(suite, /fileuploadhandler_\\\.ashx/);
  assert.match(suite, /setInputFiles/);
  assert.match(suite, /radicado_radicacion/);
  assert.match(suite, /DOC85_E2E_DOCUMENT_COUNT_SQL/);
  assert.match(suite, /navigationCount/);
  assert.match(suite, /request\.resourceType\(\) === 'document'/);
  assert.doesNotMatch(suite, /page\.on\('framenavigated'/);
  assert.match(suite, /activeText\.includes\(expectedRadicado\)/);
  assert.match(suite, /La sesión ya tiene asignado un radicado distinto/);
  assert.ok(
    suite.indexOf("page.locator('#Label_estado_transac')") < suite.indexOf("page.locator('#boton_rad_list_task').click()"),
    'la E2E debe reutilizar primero el radicado activo antes de buscarlo en pendientes'
  );
  assert.doesNotMatch(suite, /storageState|dotenv|document\.cookie/);
  assert.match(runner, /requireInteractiveConsole/);
  assert.match(runner, /collectConfirmation/);
  assert.match(validator, /isSingleReadOnlyQuery/);
  assert.equal(
    packageJson.scripts['test:radicacion-simple:attachment'],
    'node scripts/run-radicacion-simple-attachment-interactive.cjs'
  );
});

test('el perfil de ejemplo es no sensible y sus fixtures quedan dentro de tools/e2e', () => {
  const profile = JSON.parse(read('tools/e2e/profiles/radicacion-simple-attachment.profile.example.json'));
  const serialized = JSON.stringify(profile);
  assert.doesNotMatch(serialized, /password|cookie|token|connectionString|mysqlUrl/i);
  for (const key of ['contextSql', 'datAdicSql', 'documentCountSql']) {
    assert.match(profile[key], /^SELECT\b/i);
    assert.equal((profile[key].match(/\?/g) || []).length, 1);
  }
  for (const key of ['positiveFixturePath', 'negativeFixturePath']) {
    const fixture = path.resolve(e2eRoot, profile[key]);
    assert.ok(fixture.startsWith(`${e2eRoot}${path.sep}`));
    assert.ok(fs.existsSync(fixture));
  }
});

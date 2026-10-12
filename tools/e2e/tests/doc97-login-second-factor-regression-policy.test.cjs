const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const e2eRoot = path.resolve(__dirname, '..');
const repoRoot = path.resolve(e2eRoot, '..', '..');
const read = file => fs.readFileSync(path.join(repoRoot, ...file.split('/')), 'utf8');

test('DOC-97 registra una política única y reutiliza la infraestructura 2FA', () => {
  const packageJson = JSON.parse(read('tools/e2e/package.json'));
  assert.equal(packageJson.scripts['test:doc97:policy'], 'node --test tests/doc97-login-second-factor-regression-policy.test.cjs');
  assert.ok(packageJson.scripts['test:doc95:second-factor']);
  assert.ok(packageJson.scripts['test:doc96:login-ui']);
  assert.equal(Object.keys(packageJson.scripts).filter(name => name.startsWith('test:doc97:')).length, 1);
});

test('DOC-97 no usa el control temporal de flags como evidencia', () => {
  const tasks = read('openspec/changes/doc-97-doble-factor-seguridad-regresion/tasks.md');
  const spec = read('openspec/changes/doc-97-doble-factor-seguridad-regresion/specs/doble-factor-seguridad-regresion/spec.md');
  assert.doesNotMatch(tasks, /run-doc96-gestor-off-temporary-interactive\.cjs/);
  assert.match(spec, /mantener flags intactos/i);
  assert.match(spec, /controles `SELECT`/);
});

test('DOC-97 mantiene secretos y E2E real fuera de la política CI', () => {
  const workflow = read('.github/workflows/opsxj-validation.yml');
  assert.match(workflow, /npm\.cmd --prefix tools\/e2e run test:doc97:policy/);
  assert.doesNotMatch(workflow, /test:doc95:second-factor|test:doc96:login-ui/);
  assert.doesNotMatch(workflow, /run-doc96-gestor-off-temporary/);
});

'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..', '..', '..');
const read = (...segments) => fs.readFileSync(path.join(root, ...segments), 'utf8');

test('DOC-90 registra runner interactivo y perfil sin secretos', () => {
  const packageJson = JSON.parse(read('tools', 'e2e', 'package.json'));
  const runner = read('tools', 'e2e', 'scripts', 'run-doc90-radicacion-classic-attachment-interactive.cjs');
  const profile = JSON.parse(read('tools', 'e2e', 'profiles', 'doc90-radicacion-classic-attachment.profile.example.json'));

  assert.equal(packageJson.scripts['test:doc90:radicacion-classic-attachment'], 'node scripts/run-doc90-radicacion-classic-attachment-interactive.cjs');
  assert.match(runner, /requireInteractiveConsole\(\)/);
  assert.match(runner, /collectConfirmation/);
  assert.match(runner, /collectValue\(values, 'DOC90_E2E_AUTHORIZED_PASSWORD'.*secret: true/);
  assert.equal(profile.pagePath, 'radicador/WebFormRadicacionEntrante.aspx');
  assert.equal(profile.fixturePath, 'fixtures/doc85-constancia-inscripcion.pdf');
  assert.equal(Object.hasOwn(profile, 'stateRecordId'), false);
  assert.match(profile.contextSql, /id_estado_radicado AS estado/i);
  assert.match(profile.contextSql, /estado AS estado_pendiente/i);
  assert.match(profile.contextSql, /Nombre_Plantilla_Radicado AS plantilla_nombre/i);
  assert.match(profile.contextSql, /consecutivo_radicado\s*=\s*\?/i);
  assert.equal(Object.keys(profile).some(key => /password|user/i.test(key)), false);
});

test('DOC-90 limita los controles a SELECT parametrizados y exige cuatro autorizaciones', () => {
  const validator = read('tools', 'e2e', 'scripts', 'assert-doc90-radicacion-classic-attachment-config.cjs');
  const checker = read('tools', 'e2e', 'scripts', 'check-doc90-radicacion-classic-attachment-readonly.cjs');
  const profile = JSON.parse(read('tools', 'e2e', 'profiles', 'doc90-radicacion-classic-attachment.profile.example.json'));

  for (const query of [profile.contextSql, profile.documentCountSql]) {
    assert.match(query, /^SELECT\b/i);
    assert.equal((query.match(/\?/g) || []).length, 1);
    assert.doesNotMatch(query, /\b(?:INSERT|UPDATE|DELETE|CALL|EXEC)\b/i);
  }
  for (const authorization of [
    'DOC90_E2E_ENVIRONMENT_AUTHORIZED', 'DOC90_E2E_ACCOUNT_AUTHORIZED',
    'DOC90_E2E_EXECUTION_AUTHORIZED', 'DOC90_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
  ]) assert.match(validator, new RegExp(authorization));
  assert.match(validator, /isSingleReadOnlyQuery/);
  assert.match(validator, /debe permanecer dentro del paquete E2E/);
  assert.match(checker, /isSingleReadOnlyQuery/);
  assert.match(checker, /connection\.execute\(profile\.contextSql/);
  assert.match(checker, /connection\.execute\(profile\.documentCountSql/);
  assert.match(checker, /estadoPendiente: pending/);
  assert.match(checker, /documentosActuales: total/);
  assert.doesNotMatch(checker, /expectedRadicado:\s*profile\.expectedRadicado/);
});

test('DOC-90 reproduce el flujo clásico y separa el multipart de Simplificada', () => {
  const suite = read('tools', 'e2e', 'tests', 'doc90-radicacion-classic-attachment.spec.cjs');

  assert.match(suite, /createAuthenticatedWorkflowSession/);
  assert.match(suite, /#A1/);
  assert.ok(suite.includes('a[tip_event="a_s_r_p_333"][idd="${stateId}"]'));
  assert.match(suite, /findClassicAssignment/);
  assert.match(suite, /#GridView_list_registro_rad \.pagination-ys/);
  assert.match(suite, /currentPage \+ 1/);
  assert.match(suite, /queryRows\(connection, 'DOC90_E2E_CONTEXT_SQL', expectedRadicado\)/);
  assert.match(suite, /stateId = Number\(contextRows\[0\]\.estado\)/);
  assert.match(suite, /selectIncomingTemplate\(page, templateId, templateName\)/);
  assert.match(suite, /ITEMS_DATOS_TOKENIZE_2/);
  assert.match(suite, /nodo_plantilla_radicado === 'yes'/);
  assert.match(suite, /#CR-PR-00 > a/);
  assert.match(suite, /#CR-PR-11 > a/);
  assert.match(suite, /#a_load_file:visible, #a_load_file_nav:visible/);
  assert.match(suite, /installClassicMultipartProbe\(page\)/);
  assert.match(suite, /window\.__doc90ClassicMultipart/);
  assert.doesNotMatch(suite, /postDataBuffer\(\)/);
  assert.ok(suite.indexOf('const response = await responsePromise') < suite.indexOf('inspectClassicMultipart(response.request()'));
  assert.match(suite, /La carga DOC-90 falló después de iniciar el POST y el conteo cambió/);
  assert.match(suite, /no fue posible reconciliar la persistencia con SELECT/);
  assert.match(suite, /ADJUNTARADICACION_CLASICA/);
  assert.match(suite, /field\('id_registro_estado_radicacion'\)\)\.toHaveLength\(0\)/);
  assert.match(suite, /field\('radicado_radicacion'\)\)\.toHaveLength\(0\)/);
  assert.match(suite, /afterTotal\)\.toBe\(beforeTotal \+ 1\)/);
  assert.match(suite, /rowsBefore \+ 1/);
  assert.match(suite, /navigationCount\)\.toBe\(0\)/);
  assert.match(suite, /postbackCount\)\.toBe\(0\)/);
});

test('DOC-90 mantiene DOC-85 como comando E2E separado de no regresión', () => {
  const packageJson = JSON.parse(read('tools', 'e2e', 'package.json'));
  assert.ok(packageJson.scripts['test:radicacion-simple:attachment']);
  assert.ok(packageJson.scripts['test:doc90:radicacion-classic-attachment']);
});

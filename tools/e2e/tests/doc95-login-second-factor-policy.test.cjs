'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { spawnSync } = require('node:child_process');

const root = path.resolve(__dirname, '..', '..', '..');
const read = (...segments) => fs.readFileSync(path.join(root, ...segments), 'utf8');

test('DOC-95 registra un único runner común y un perfil sin secretos', () => {
  const packageJson = JSON.parse(read('tools', 'e2e', 'package.json'));
  const runner = read('tools', 'e2e', 'scripts', 'run-doc95-login-second-factor-interactive.cjs');
  const profile = JSON.parse(read('tools', 'e2e', 'profiles', 'doc95-login-second-factor.profile.example.json'));

  assert.equal(packageJson.scripts['test:doc95:second-factor'], 'node scripts/run-doc95-login-second-factor-interactive.cjs');
  assert.match(runner, /createAuthenticatedWorkflowSession/);
  assert.match(runner, /support\/interactive-e2e-console\.cjs/);
  assert.match(runner, /WebServiceLoginSegundoFactor\.asmx/);
  assert.match(runner, /onPostbackCompleted/);
  assert.equal(Object.keys(profile).some(key => /password|user|otp|mailbox|challenge/i.test(key)), false);
  assert.deepEqual(Object.keys(profile).sort(), ['baseUrl', 'ignoreHttpsErrors', 'module', 'timeoutMilliseconds'].sort());
});

test('DOC-95 clasifica solo códigos públicos permitidos sin imprimir el postback', () => {
  const { publicLoginCode, requestedScenario } = require('../scripts/run-doc95-login-second-factor-interactive.cjs');
  assert.equal(publicLoginCode('<script>SECOND_FACTOR_START_FAILED</script>'), 'SECOND_FACTOR_START_FAILED');
  assert.equal(publicLoginCode('<script>SECOND_FACTOR_PRINCIPAL_NOT_FOUND</script>'), 'SECOND_FACTOR_PRINCIPAL_NOT_FOUND');
  assert.equal(publicLoginCode('<script>detalle SQL contraseña=secreto</script>'), '');
  assert.equal(requestedScenario(['node', 'runner', '--scenario', 'expiry']), 'expiry');
  assert.equal(requestedScenario(['node', 'runner', '--scenario', 'verify']), 'verify');
  assert.equal(requestedScenario(['node', 'runner', '--scenario', 'stability']), 'stability');
  assert.equal(requestedScenario(['node', 'runner']), 'concurrency');
  assert.throws(() => requestedScenario(['node', 'runner', '--scenario', 'otro']), /DOC95_SCENARIO_INVALID/);
});

test('DOC-95 exige autorizaciones independientes y no persiste secretos', () => {
  const runner = read('tools', 'e2e', 'scripts', 'run-doc95-login-second-factor-interactive.cjs');
  for (const authorization of ['environment', 'account', 'mailbox', 'challenge', 'concurrency', 'discardableData']) {
    assert.match(runner, new RegExp(`collectConfirmation\\(authorization, '${authorization}'`));
  }
  assert.match(runner, /collectValue\(secrets, 'password'.*secret: true/);
  assert.match(runner, /promptSecret\('OTP recibido en el buzón autorizado'\)/);
  assert.match(runner, /Promise\.all\(\[/);
  assert.doesNotMatch(runner, /mysql2|dotenv|writeFile|appendFile|process\.env\s*=/i);
  for (const logCall of runner.matchAll(/console\.log\(([^\n]+)\)/g)) {
    assert.match(logCall[1], /^'DOC95_[A-Z0-9_]+=.*'$/);
  }
});

test('DOC-95 comprueba el perfil sin abrir navegador ni solicitar TTY', () => {
  const result = spawnSync(process.execPath, [
    path.join(root, 'tools', 'e2e', 'scripts', 'run-doc95-login-second-factor-interactive.cjs'),
    '--check',
    '--profile',
    path.join(root, 'tools', 'e2e', 'profiles', 'doc95-login-second-factor.profile.example.json')
  ], { cwd: path.join(root, 'tools', 'e2e'), encoding: 'utf8' });

  assert.equal(result.status, 0, result.stderr);
  assert.match(result.stdout, /DOC95_E2E_CHECK_OK=true/);
  assert.match(result.stdout, /DOC95_E2E_REAL_EXECUTION=false/);
});

test('DOC-95 limita la evidencia a banderas y exige un solo ganador', () => {
  const runner = read('tools', 'e2e', 'scripts', 'run-doc95-login-second-factor-interactive.cjs');
  assert.match(runner, /completed\.length !== 1 \|\| rejected\.length !== 1/);
  assert.match(runner, /DOC95_CONCURRENCY_WINNERS=1/);
  assert.match(runner, /RutaLocal\.startsWith\('~\/'\)/);
  assert.doesNotMatch(runner, /console\.log\([^\n]*(?:DestinoEnmascarado|RutaLocal|results|state)/);
});

test('DOC-95 separa expiración, validación OTP y concurrencia', () => {
  const runner = read('tools', 'e2e', 'scripts', 'run-doc95-login-second-factor-interactive.cjs');
  assert.match(runner, /state\.ExpiraEnSegundos \+ 3/);
  assert.match(runner, /expired\.EstadoPublico !== 'NO_DISPONIBLE'/);
  assert.match(runner, /DOC95_AUTH_COOKIE_AFTER_EXPIRY=false/);
  assert.match(runner, /DOC95_EXPIRY_RESULT=PASSED/);
  assert.match(runner, /DOC95_OTP_VALIDATION_OK=true/);
  assert.match(runner, /DOC95_VERIFY_RESULT=PASSED/);
  assert.match(runner, /attempt < 3/);
  assert.match(runner, /DOC95_STABILITY_SESSIONS=3/);
  assert.match(runner, /collectConfirmation\(authorization, 'stability'/);
});

test('DOC-95 aprovisiona HMAC local fuera de Git y sin imprimir la llave', () => {
  const webConfig = read('Web.config');
  const gitignore = read('.gitignore');
  const provisioning = read('tools', 'e2e', 'scripts', 'configure-doc95-hmac-local.ps1');
  assert.match(webConfig, /<appSettings\s+file="AppSettings\.local\.config">/);
  assert.match(gitignore, /^\/AppSettings\.local\.config$/m);
  assert.match(provisioning, /RandomNumberGenerator/);
  assert.match(provisioning, /byte\[\]\s+32/i);
  assert.doesNotMatch(provisioning, /Write-(?:Output|Host).*Base64|Write-(?:Output|Host).*LoginSecondFactorHmacKey/i);
});

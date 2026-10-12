'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { execFileSync } = require('node:child_process');
const {
  allowedProfileFields,
  deriveDifferentCode,
  loadProfile,
  publicLoginCode,
  requestedScenario,
  scenarios
} = require('../scripts/run-doc96-login-second-factor-ui-interactive.cjs');
const {
  DISABLE_SQL,
  RESTORE_SQL,
  SELECT_MODULE_SQL,
  assertSqlScope,
  safeFailureCode,
  validSnapshot
} = require('../scripts/run-doc96-gestor-off-temporary-interactive.cjs');
const {
  safeFailureCode: safeDiagnosticFailureCode
} = require('../scripts/inspect-doc96-module-second-factor-interactive.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const repositoryRoot = path.resolve(e2eRoot, '..', '..');
const runnerPath = path.join(e2eRoot, 'scripts', 'run-doc96-login-second-factor-ui-interactive.cjs');
const exampleProfile = path.join(e2eRoot, 'profiles', 'doc96-login-second-factor-ui.profile.example.json');

test('DOC-96 define matriz OFF/ON sin secretos y perfil cerrado', () => {
  const profileText = fs.readFileSync(exampleProfile, 'utf8');
  const profile = loadProfile(exampleProfile);
  assert.notEqual(profile.moduleOff, profile.moduleOn);
  assert.deepEqual([...allowedProfileFields].sort(), [
    'baseUrl', 'ignoreHttpsErrors', 'maxExpiryWaitMilliseconds',
    'moduleOff', 'moduleOn', 'timeoutMilliseconds'
  ]);
  assert.doesNotMatch(profileText, /password|contrase(?:ñ|n)a|account|usuario|otp|cookie|connection/i);
});

test('DOC-96 rechaza campos extra y módulos iguales', () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc96-profile-'));
  try {
    const base = JSON.parse(fs.readFileSync(exampleProfile, 'utf8'));
    const extra = path.join(directory, 'extra.json');
    fs.writeFileSync(extra, JSON.stringify({ ...base, password: 'prohibido' }));
    assert.throws(() => loadProfile(extra), /DOC96_PROFILE_FIELD_NOT_ALLOWED/);
    const same = path.join(directory, 'same.json');
    fs.writeFileSync(same, JSON.stringify({ ...base, moduleOn: base.moduleOff }));
    assert.throws(() => loadProfile(same), /DOC96_MODULE_MATRIX_INVALID/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('DOC-96 incluye escenarios visuales obligatorios y rechaza otros', () => {
  for (const scenario of ['off', 'verify', 'refresh', 'invalid', 'blocked', 'resend', 'cancel', 'expiry', 'double-submit', 'matrix']) {
    assert.equal(scenarios.has(scenario), true);
    assert.equal(requestedScenario(['node', 'runner', '--scenario', scenario]), scenario);
  }
  assert.throws(() => requestedScenario(['node', 'runner', '--scenario', 'desconocido']), /DOC96_SCENARIO_INVALID/);
});

test('clasifica códigos públicos de login sin conservar mensajes internos', () => {
  assert.equal(publicLoginCode('alert: LOGIN_FINALIZATION_FAILED'), 'LOGIN_FINALIZATION_FAILED');
  assert.equal(publicLoginCode('SECOND_FACTOR_REQUIRED'), 'SECOND_FACTOR_REQUIRED');
  assert.equal(publicLoginCode('mensaje de credenciales inválidas'), '');
});

test('deriva un código inválido distinto sin exponer el OTP recibido', () => {
  assert.equal(deriveDifferentCode('123456'), '123457');
  assert.equal(deriveDifferentCode('999999'), '000000');
  assert.throws(() => deriveDifferentCode('12345'), /DOC96_OTP_FORMAT_INVALID/);
});

test('modo check valida configuración sin TTY, navegador ni secretos', () => {
  const output = execFileSync(process.execPath, [runnerPath, '--scenario', 'matrix', '--profile', exampleProfile, '--check'], {
    cwd: repositoryRoot,
    encoding: 'utf8',
    stdio: ['ignore', 'pipe', 'pipe']
  });
  assert.match(output, /DOC96_E2E_CHECK_OK=true/);
  assert.match(output, /DOC96_E2E_REAL_EXECUTION=false/);
});

test('runner reutiliza helper, autorización interactiva y no contiene sleeps, SQL o secretos', () => {
  const runner = fs.readFileSync(runnerPath, 'utf8');
  assert.match(runner, /createWorkflowLoginPageSession/);
  assert.match(runner, /collectConfirmation/);
  assert.match(runner, /promptSecret/);
  assert.doesNotMatch(runner, /setTimeout|["'`]\.env(?:["'`/\\])|dotenv|SELECT\s|UPDATE\s|INSERT\s|DELETE\s/i);
  assert.doesNotMatch(runner, /console\.log\(\s*(?:otp|invalid|secrets|environment|.*Password|.*Account)\b/);
  assert.match(runner, /console\.error\(`La E2E DOC-96 se detuvo de forma segura \(\$\{code\}\)/);
  assert.match(runner, /DOC96_OFF_LOGIN_REJECTED/);
  assert.match(runner, /DOC96_OFF_FINALIZATION_NOT_COMPLETED/);
  assert.match(runner, /DOC96_OFF_REDIRECT_NOT_OBSERVED/);
  assert.match(runner, /DOC96_PENDING_UI_NOT_OBSERVED/);
  assert.match(runner, /2FA apagado \(\$\{profile\.moduleOff\}\)/);
  assert.match(runner, /2FA activo \(\$\{profile\.moduleOn\}\)/);
  assert.match(runner, /chromium\.launch\(\{ headless: false \}\)/);
  assert.match(runner, /DOC96_BROWSER_OBSERVATION_ONLY=true/);
  assert.match(runner, /response\.text\(\)\.catch\(\(\) => ''\)/);
  assert.match(runner, /DOC96_BROWSER_CHANGED_BEFORE_AUTOMATION/);
  assert.match(runner, /ingréselo solo aquí, no en Chromium/);
  assert.match(runner, /DOC96_VERIFY_RESPONSE_NOT_OBSERVED/);
  assert.doesNotMatch(runner, /response\.json\(\)/);
  assert.match(runner, /DOC96_BLOCKED_UI_NOT_CLOSED/);
  assert.match(runner, /ATTEMPT_\$\{attemptNumber\}_INPUT_FAILED/);
  assert.match(runner, /ATTEMPT_\$\{attemptNumber\}_SUBMIT_FAILED/);
  assert.match(runner, /ATTEMPT_\$\{attemptNumber\}_BUTTON_NOT_READY/);
  assert.match(runner, /ATTEMPT_\$\{attemptNumber\}_VALID_OTP_ACCEPTED/);
  assert.match(runner, /ATTEMPT_\$\{attemptNumber\}_TERMINAL_EARLY/);
  assert.match(runner, /const invalid = deriveDifferentCode\(receivedOtp\)/);
  assert.doesNotMatch(runner, /console\.log\([^\n]*receivedOtp/);
  assert.match(runner, /DOC96_RESEND_PREVIOUS_CODE_NOT_REJECTED/);
  assert.match(runner, /DOC96_EXPIRY_VERIFICATION_REQUEST_OBSERVED/);
});

test('package registra un runner y una política DOC-96', () => {
  const packageJson = JSON.parse(fs.readFileSync(path.join(e2eRoot, 'package.json'), 'utf8'));
  assert.equal(packageJson.scripts['test:doc96:login-ui'], 'node scripts/run-doc96-login-second-factor-ui-interactive.cjs');
  assert.match(packageJson.scripts['test:doc96:policy'], /doc96-login-second-factor-ui-policy/);
});

test('control temporal limita escritura al flag de GESTOR y exige snapshot ON válido', () => {
  assert.doesNotThrow(() => assertSqlScope());
  assert.match(SELECT_MODULE_SQL, /FROM gestor_modulos[\s\S]*NOMBRE_MODULO=\?/);
  assert.match(DISABLE_SQL, /SET RequiereSegundoFactor=0[\s\S]*RequiereSegundoFactor=1/);
  assert.match(RESTORE_SQL, /SET RequiereSegundoFactor=\?[\s\S]*RequiereSegundoFactor=0/);
  assert.doesNotMatch(`${DISABLE_SQL}\n${RESTORE_SQL}`, /SecondFactorProviderType\s*=|SegundoFactorTiempoExpira\s*=/);
  assert.equal(validSnapshot([{ ID_MODULO: 1, RequiereSegundoFactor: 1, SecondFactorProviderType: 1, SegundoFactorTiempoExpira: 5 }]), true);
  assert.equal(validSnapshot([{ ID_MODULO: 1, RequiereSegundoFactor: 0, SecondFactorProviderType: 1, SegundoFactorTiempoExpira: 5 }]), false);
});

test('diagnósticos SQL publican etapa segura sin filtrar mensajes internos', () => {
  assert.equal(safeFailureCode({ code: 'ER_ACCESS_DENIED_ERROR' }, 'connection'), 'DOC96_GESTOR_DB_ACCESS_DENIED');
  assert.equal(safeFailureCode({ code: 'ER_TABLEACCESS_DENIED_ERROR' }, 'disable'), 'DOC96_GESTOR_UPDATE_PERMISSION_DENIED');
  assert.equal(safeFailureCode({ code: 'ER_TABLEACCESS_DENIED_ERROR' }, 'restore'), 'DOC96_GESTOR_RESTORE_PERMISSION_DENIED');
  assert.equal(safeFailureCode({ message: 'detalle interno sensible' }, 'precondition'), 'DOC96_GESTOR_PRECONDITION_FAILED');
  assert.equal(safeDiagnosticFailureCode({ code: 'ER_ACCESS_DENIED_ERROR' }, 'connection'), 'DOC96_DB_ACCESS_DENIED');
  assert.equal(safeDiagnosticFailureCode({ message: 'detalle interno sensible' }, 'query'), 'DOC96_DIAGNOSTIC_QUERY_FAILED');
});

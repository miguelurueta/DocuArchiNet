'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
  ProfileValidationError,
  loadProfile,
  validateProfile
} = require('../scripts/support/workflow-e2e-platform-profile.cjs');

function validProfile(overrides = {}) {
  return {
    scenarioId: 'notes-read',
    baseUrl: 'https://workflow.example.invalid/app/',
    module: 'GESTOR',
    environment: 'CERTIFICACION',
    odbcDsn: 'workflowconta',
    taskId: 708,
    budgetMs: 10000,
    ignoreHttpsErrors: false,
    ...overrides
  };
}

function invalid(code) {
  return (error) => error instanceof ProfileValidationError && error.code === code;
}

test('el perfil no sensible acepta únicamente los campos declarados para notes-read', () => {
  const profile = validateProfile(validProfile());
  assert.equal(profile.scenarioId, 'notes-read');
  assert.equal(profile.taskId, 708);
  assert.equal(profile.ignoreHttpsErrors, false);
  assert.ok(Object.isFrozen(profile));
});

test('el perfil falla cerrado para secretos, SQL, URL de base de datos y campos desconocidos', () => {
  assert.throws(() => validateProfile(validProfile({ password: 'valor-prohibido' })), invalid('E2E_PLATFORM_PROFILE_FORBIDDEN_FIELD'));
  assert.throws(() => validateProfile(validProfile({ consulta: 'SELECT secreto FROM tabla' })), invalid('E2E_PLATFORM_PROFILE_UNKNOWN_FIELD'));
  assert.throws(() => validateProfile(validProfile({ odbcDsn: 'mysql://valor-prohibido' })), invalid('E2E_PLATFORM_PROFILE_DSN_INVALID'));
  assert.throws(() => validateProfile(validProfile({ script: 'node externo.cjs' })), invalid('E2E_PLATFORM_PROFILE_FORBIDDEN_FIELD'));
  try {
    validateProfile(validProfile({ odbcDsn: 'mysql://valor-prohibido' }));
  } catch (error) {
    assert.doesNotMatch(error.message, /valor-prohibido/);
  }
});

test('el perfil de etapa anónima no acepta una tarea ni rutas fuera del directorio de perfiles', async () => {
  const anonymous = validateProfile({ scenarioId: 'notes-anonymous', baseUrl: 'https://workflow.example.invalid/app/', ignoreHttpsErrors: false });
  assert.equal(anonymous.budgetMs, 10000);
  assert.throws(() => validateProfile(validProfile({ scenarioId: 'notes-anonymous' })), invalid('E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID'));
  const root = await fs.mkdtemp(path.join(os.tmpdir(), 'workflow-e2e-platform-profile-'));
  try {
    await fs.writeFile(path.join(root, 'valid.json'), JSON.stringify(validProfile()), 'utf8');
    const loaded = await loadProfile('valid.json', { profilesRoot: root });
    assert.equal(loaded.scenarioId, 'notes-read');
    await assert.rejects(() => loadProfile('../outside.json', { profilesRoot: root }), invalid('E2E_PLATFORM_PROFILE_PATH_INVALID'));
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});

'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { resolveScenario } = require('./workflow-e2e-platform-registry.cjs');

const PROFILE_KEYS = new Set(['scenarioId', 'baseUrl', 'module', 'environment', 'odbcDsn', 'taskId', 'noteId', 'budgetMs', 'browser', 'ignoreHttpsErrors']);
const FORBIDDEN_KEY = /(passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|sql|query|command|comando|script|mysql|database|user)/i;
const FORBIDDEN_VALUE = /(?:mysql|odbc):\/\/|(?:^|[;\s])(?:password|pwd|uid)\s*=|\b(?:SELECT|INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i;
const SAFE_LABEL = /^[A-Za-z0-9_-]{2,80}$/;
const SAFE_DSN = /^[A-Za-z0-9 _.-]{1,128}$/;

class ProfileValidationError extends Error {
  constructor(code) {
    super(`El perfil E2E no es válido (${code}). No se mostró ningún valor.`);
    this.name = 'ProfileValidationError';
    this.code = code;
  }
}

function fail(code) {
  throw new ProfileValidationError(code);
}

function assertPlainObject(value) {
  if (!value || typeof value !== 'object' || Array.isArray(value)) fail('E2E_PLATFORM_PROFILE_OBJECT_INVALID');
}

function assertSafeText(value, expression, code) {
  if (typeof value !== 'string' || !expression.test(value) || FORBIDDEN_VALUE.test(value)) fail(code);
  return value;
}

function assertBaseUrl(value) {
  if (typeof value !== 'string' || FORBIDDEN_VALUE.test(value)) fail('E2E_PLATFORM_PROFILE_BASE_URL_INVALID');
  try {
    const url = new URL(value);
    if (!/^https?:$/.test(url.protocol) || url.username || url.password) throw new Error('invalid');
    return url.toString();
  } catch {
    fail('E2E_PLATFORM_PROFILE_BASE_URL_INVALID');
  }
}

function assertPositiveInteger(value, code) {
  if (!Number.isSafeInteger(value) || value <= 0 || value > 600000) fail(code);
  return value;
}

function validateBrowser(value) {
  if (value === undefined) return undefined;
  assertPlainObject(value);
  const keys = Object.keys(value);
  if (keys.length !== 1 || keys[0] !== 'channel' || !['chrome', 'msedge'].includes(value.channel)) {
    fail('E2E_PLATFORM_PROFILE_BROWSER_INVALID');
  }
  return Object.freeze({ channel: value.channel });
}

function validateProfile(input) {
  assertPlainObject(input);
  for (const key of Object.keys(input)) {
    if (FORBIDDEN_KEY.test(key)) fail('E2E_PLATFORM_PROFILE_FORBIDDEN_FIELD');
    if (!PROFILE_KEYS.has(key)) fail('E2E_PLATFORM_PROFILE_UNKNOWN_FIELD');
  }
  if (typeof input.scenarioId !== 'string') fail('E2E_PLATFORM_PROFILE_SCENARIO_INVALID');
  const scenario = resolveScenario(input.scenarioId);
  const profile = {
    scenarioId: scenario.id,
    baseUrl: assertBaseUrl(input.baseUrl),
    budgetMs: input.budgetMs === undefined ? 10000 : assertPositiveInteger(input.budgetMs, 'E2E_PLATFORM_PROFILE_BUDGET_INVALID'),
    ignoreHttpsErrors: input.ignoreHttpsErrors === true,
    browser: validateBrowser(input.browser)
  };
  if (scenario.transport.session === 'workflow') {
    profile.module = assertSafeText(input.module, SAFE_LABEL, 'E2E_PLATFORM_PROFILE_MODULE_INVALID');
    profile.environment = assertSafeText(input.environment, SAFE_LABEL, 'E2E_PLATFORM_PROFILE_ENVIRONMENT_INVALID');
  } else if (input.module !== undefined || input.environment !== undefined || input.odbcDsn !== undefined) {
    fail('E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID');
  }
  if (scenario.controls.length > 0) {
    profile.odbcDsn = assertSafeText(input.odbcDsn, SAFE_DSN, 'E2E_PLATFORM_PROFILE_DSN_INVALID');
  }
  if (scenario.resource?.profileField === 'taskId') {
    profile.taskId = assertPositiveInteger(input.taskId, 'E2E_PLATFORM_PROFILE_TASK_INVALID');
  } else if (input.taskId !== undefined) {
    fail('E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID');
  }
  if (scenario.stage === 'concurrency') {
    profile.noteId = assertPositiveInteger(input.noteId, 'E2E_PLATFORM_PROFILE_NOTE_INVALID');
  } else if (input.noteId !== undefined) {
    fail('E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID');
  }
  return Object.freeze(profile);
}

async function loadProfile(profilePath, { profilesRoot = path.resolve(__dirname, '..', '..', 'profiles') } = {}) {
  if (typeof profilePath !== 'string' || !profilePath.trim()) fail('E2E_PLATFORM_PROFILE_PATH_INVALID');
  const root = path.resolve(profilesRoot);
  const resolved = path.resolve(root, profilePath);
  if (path.relative(root, resolved).startsWith('..') || path.extname(resolved).toLowerCase() !== '.json') {
    fail('E2E_PLATFORM_PROFILE_PATH_INVALID');
  }
  let text;
  try {
    text = await fs.readFile(resolved, 'utf8');
  } catch {
    fail('E2E_PLATFORM_PROFILE_READ_FAILED');
  }
  try {
    return validateProfile(JSON.parse(text.replace(/^\uFEFF/, '')));
  } catch (error) {
    if (error instanceof ProfileValidationError) throw error;
    fail('E2E_PLATFORM_PROFILE_JSON_INVALID');
  }
}

module.exports = {
  PROFILE_KEYS,
  ProfileValidationError,
  loadProfile,
  validateProfile
};

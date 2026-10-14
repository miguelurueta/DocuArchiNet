'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const {
  collectConfirmation, collectValue, requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { loadProfile, runReal } = require('./run-doc103-graph-save-interactive.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const allowedScenarios = new Set(['normal', 'rotated', 'file', 'base64', 'transparent', 'opaque', 'linearized', 'signed']);
const forbiddenEvidence = /https?:|[A-Za-z]:\\|(?:cookie|token|password|contrase(?:ñ|n)a|base64-data|usuario|taskId|documentTitle|documentOrdinal|baseUrl|url|path|query)/i;

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

function argument(name) {
  const index = process.argv.indexOf(name);
  return index >= 0 ? process.argv[index + 1] : undefined;
}

function sanitizeEvidence(input) {
  if (forbiddenEvidence.test(JSON.stringify(input))) fail('DOC104_EVIDENCE_INVALID');
  const scenario = allowedScenarios.has(input.scenario) ? input.scenario : 'policy';
  const evidence = Object.freeze({
    mode: input.mode === 'real' ? 'real' : 'double',
    scenario,
    result: input.result === 'PASSED' ? 'PASSED' : 'FAILED',
    postCount: Number(input.postCount) || 0,
    singleEffect: input.singleEffect === true,
    selectControlsCompleted: input.selectControlsCompleted === true,
    policyPassed: input.policyPassed === true
  });
  if (forbiddenEvidence.test(JSON.stringify(evidence))) fail('DOC104_EVIDENCE_INVALID');
  return evidence;
}

async function writeEvidence(evidence) {
  const target = path.join(e2eRoot, 'artifacts', 'doc104-server-processing.json');
  await fs.mkdir(path.dirname(target), { recursive: true });
  await fs.writeFile(target, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function writeFailure(code) {
  const target = path.join(e2eRoot, 'artifacts', 'doc104-server-processing-failure.json');
  await fs.mkdir(path.dirname(target), { recursive: true });
  await fs.writeFile(target, `${JSON.stringify({ result: 'BLOCKED', code }, null, 2)}\n`, 'utf8');
}

async function runDouble() {
  const tests = [
    'tests/workflow-pdf-server-processing-policy.test.cjs',
    'tests/workflow-pdf-viewer-graph-save-policy.test.cjs',
    'tests/workflow-pdf-viewer-graph-save.test.cjs'
  ];
  const run = spawnSync(process.execPath, ['--test', ...tests], {
    cwd: e2eRoot, stdio: 'inherit', shell: false
  });
  const passed = run.status === 0;
  const evidence = sanitizeEvidence({
    mode: 'double', scenario: 'policy', result: passed ? 'PASSED' : 'FAILED',
    postCount: 0, singleEffect: passed, selectControlsCompleted: false, policyPassed: passed
  });
  await writeEvidence(evidence);
  if (!passed) fail('DOC104_DOUBLE_FAILED');
  return evidence;
}

async function main() {
  const mode = argument('--mode') || 'double';
  if (mode === 'double') {
    const evidence = await runDouble();
    console.log(`DOC104_SERVER_PROCESSING_RESULT=${evidence.result}`);
    return;
  }
  if (mode !== 'real') fail('DOC104_MODE_INVALID');

  requireInteractiveConsole();
  const profileTarget = argument('--profile');
  if (!profileTarget) fail('DOC104_PROFILE_REQUIRED');
  const scenario = argument('--scenario');
  if (!allowedScenarios.has(scenario)) fail('DOC104_SCENARIO_INVALID');
  const profile = loadProfile(path.resolve(profileTarget));
  const values = { DOC104_E2E_MODULE: profile.module, DOC104_E2E_ODBC_DSN: profile.odbcDsn };

  await collectConfirmation(values, 'DOC104_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza el ambiente para la E2E mutadora DOC-104?');
  await collectConfirmation(values, 'DOC104_E2E_ACCOUNT_AUTHORIZED', '¿Autoriza la cuenta para esta prueba DOC-104?');
  await collectConfirmation(values, 'DOC104_E2E_DISPOSABLE_PDF_AUTHORIZED', '¿Confirma que el PDF y el grafo son descartables?');
  await collectConfirmation(values, 'DOC104_E2E_SCENARIO_CONFIRMED', `¿Confirma que el fixture corresponde al escenario ${scenario}?`);
  await collectValue(values, 'DOC104_E2E_AUTHORIZED_USER', 'Cuenta autorizada de Workflow');
  await collectValue(values, 'DOC104_E2E_AUTHORIZED_PASSWORD', 'Contraseña de Workflow', { secret: true });
  await collectValue(values, 'DOC104_E2E_MYSQL_USER', 'Usuario ODBC de solo lectura');
  await collectValue(values, 'DOC104_E2E_MYSQL_PASSWORD', 'Contraseña ODBC de solo lectura', { secret: true });

  const environment = {
    ...process.env, ...values,
    DOC103_E2E_MODULE: values.DOC104_E2E_MODULE,
    DOC103_E2E_ODBC_DSN: values.DOC104_E2E_ODBC_DSN,
    DOC103_E2E_AUTHORIZED_USER: values.DOC104_E2E_AUTHORIZED_USER,
    DOC103_E2E_AUTHORIZED_PASSWORD: values.DOC104_E2E_AUTHORIZED_PASSWORD,
    DOC103_E2E_MYSQL_USER: values.DOC104_E2E_MYSQL_USER,
    DOC103_E2E_MYSQL_PASSWORD: values.DOC104_E2E_MYSQL_PASSWORD
  };
  const runOptions = scenario === 'signed'
    ? { failurePrefix: 'DOC104', captureDocumentFingerprint: true, expectedRejectionCode: 'DOC104_DOCUMENT_SIGNED' }
    : { failurePrefix: 'DOC104' };
  const inherited = await runReal(profile, environment, runOptions);
  const evidence = sanitizeEvidence({
    mode: 'real', scenario, result: inherited.result, postCount: inherited.postCount,
    singleEffect: inherited.singleEffect, selectControlsCompleted: inherited.selectControlsCompleted,
    policyPassed: true
  });
  await writeEvidence(evidence);
  console.log(`DOC104_SERVER_PROCESSING_POST_COUNT=${evidence.postCount}`);
  console.log(`DOC104_SERVER_PROCESSING_RESULT=${evidence.result}`);
}

if (require.main === module) {
  main().catch(async error => {
    const value = error?.code || error?.message || '';
    const code = /^[A-Z0-9_]{3,120}$/.test(value) ? value : 'DOC104_SERVER_PROCESSING_FAILED';
    console.error(`La E2E DOC-104 se detuvo de forma segura (${code}). No se mostraron rutas, credenciales ni documentos.`);
    await writeFailure(code).catch(() => {});
    process.exitCode = 2;
  });
}

module.exports = { allowedScenarios, runDouble, sanitizeEvidence };

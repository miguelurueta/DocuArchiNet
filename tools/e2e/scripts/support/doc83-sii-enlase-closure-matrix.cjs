'use strict';

const fs = require('node:fs');
const path = require('node:path');
const { CONTROL_REGISTRY, resolveScenario } = require('./workflow-e2e-platform-registry.cjs');

const REPOSITORY_ROOT = path.resolve(__dirname, '..', '..', '..', '..');
const DEFAULT_MANIFEST = path.join(REPOSITORY_ROOT, 'tools', 'e2e', 'validation', 'doc83-sii-enlase-closure-matrix.json');
const ALLOWED_SEVERITIES = new Set(['high', 'very-high']);
const SENSITIVE_KEY = /^(?:passw(?:ord)?|pwd|cookie|token|secret|credential|connectionString|sql|query|script)$/i;

function fail(code, detail) {
  const error = new Error(`La matriz DOC-83 es inválida (${code}): ${detail}`);
  error.code = code;
  throw error;
}

function assertArray(value, code, detail, allowEmpty = false) {
  if (!Array.isArray(value) || (!allowEmpty && value.length === 0) || new Set(value).size !== value.length) fail(code, detail);
}

function assertRepositoryFile(relativePath, owner) {
  if (typeof relativePath !== 'string' || path.isAbsolute(relativePath) || relativePath.includes('..')) fail('DOC83_MATRIX_REFERENCE_INVALID', `${owner}: ${String(relativePath)}`);
  const resolved = path.resolve(REPOSITORY_ROOT, relativePath);
  if (!resolved.startsWith(`${REPOSITORY_ROOT}${path.sep}`) || !fs.existsSync(resolved) || !fs.statSync(resolved).isFile()) fail('DOC83_MATRIX_REFERENCE_MISSING', `${owner}: ${relativePath}`);
}

function assertNoSensitiveKeys(value, pointer = '$') {
  if (Array.isArray(value)) return value.forEach((entry, index) => assertNoSensitiveKeys(entry, `${pointer}[${index}]`));
  if (!value || typeof value !== 'object') return;
  for (const [key, entry] of Object.entries(value)) {
    if (SENSITIVE_KEY.test(key)) fail('DOC83_MATRIX_SENSITIVE_FIELD', `${pointer}.${key}`);
    assertNoSensitiveKeys(entry, `${pointer}.${key}`);
  }
}

function validateScenarioReference(reference) {
  if (!reference || typeof reference !== 'object') fail('DOC83_MATRIX_SCENARIO_INVALID', 'entrada ausente');
  const scenario = resolveScenario(reference.id);
  if (scenario.doc.toUpperCase() !== reference.sourceDoc.replace('-', '') || scenario.stage !== reference.stage || Boolean(scenario.resource?.mutating) !== reference.mutating) fail('DOC83_MATRIX_SCENARIO_MISMATCH', reference.id);
  assertArray(reference.requiredBaseAuthorizations, 'DOC83_MATRIX_SCENARIO_INVALID', `${reference.id}: requiredBaseAuthorizations`);
  if (JSON.stringify(scenario.requiredAuthorizations) !== JSON.stringify(reference.requiredBaseAuthorizations)) fail('DOC83_MATRIX_SCENARIO_MISMATCH', `${reference.id}: authorizations`);
  assertArray(reference.requiredControls, 'DOC83_MATRIX_SCENARIO_INVALID', `${reference.id}: requiredControls`, reference.stage === 'anonymous');
  if (JSON.stringify(scenario.controls) !== JSON.stringify(reference.requiredControls)) fail('DOC83_MATRIX_SCENARIO_MISMATCH', `${reference.id}: controls`);
  for (const controlId of reference.requiredControls) {
    const control = CONTROL_REGISTRY[controlId];
    if (!control || !/^SELECT\b/i.test(control.query) || /;/.test(control.query)) fail('DOC83_MATRIX_CONTROL_INVALID', `${reference.id}: ${controlId}`);
  }
  assertArray(reference.requiredExpectations, 'DOC83_MATRIX_SCENARIO_INVALID', `${reference.id}: requiredExpectations`);
  for (const expectation of reference.requiredExpectations) if (!scenario.expectations.includes(expectation)) fail('DOC83_MATRIX_SCENARIO_MISMATCH', `${reference.id}: ${expectation}`);
  if (reference.mutating) {
    assertArray(reference.requiredStageAuthorizations, 'DOC83_MATRIX_SCENARIO_INVALID', `${reference.id}: requiredStageAuthorizations`);
    if (!reference.requiredStageAuthorizations.includes('execution') || !reference.requiredStageAuthorizations.includes('discardable-resource')) fail('DOC83_MATRIX_SCENARIO_MISMATCH', `${reference.id}: mutating authorizations`);
  }
  assertRepositoryFile(reference.evidence, reference.id);
}

function validateRisk(risk, scenarioIds) {
  if (!risk || typeof risk !== 'object' || !/^R-DOC83-\d{2}$/.test(risk.id) || !ALLOWED_SEVERITIES.has(risk.severity)) fail('DOC83_MATRIX_RISK_INVALID', risk?.id || 'identifier');
  if (typeof risk.description !== 'string' || risk.description.length < 20 || typeof risk.closure !== 'string' || risk.closure.length < 10) fail('DOC83_MATRIX_RISK_INVALID', risk.id);
  assertArray(risk.tests, 'DOC83_MATRIX_RISK_INVALID', `${risk.id}: tests`);
  assertArray(risk.scenarios, 'DOC83_MATRIX_RISK_INVALID', `${risk.id}: scenarios`, true);
  risk.tests.forEach((file) => assertRepositoryFile(file, risk.id));
  for (const scenarioId of risk.scenarios) if (!scenarioIds.has(scenarioId)) fail('DOC83_MATRIX_SCENARIO_UNDECLARED', `${risk.id}: ${scenarioId}`);
  const requiresRealGap = risk.closure.includes('authorization-required');
  if (requiresRealGap) {
    if (!risk.realGap || typeof risk.realGap.operation !== 'string' || typeof risk.realGap.resourcePolicy !== 'string') fail('DOC83_MATRIX_REAL_GAP_INVALID', risk.id);
    assertArray(risk.realGap.requiredAuthorizations, 'DOC83_MATRIX_REAL_GAP_INVALID', `${risk.id}: requiredAuthorizations`);
    if (!risk.realGap.requiredAuthorizations.includes('environment')) fail('DOC83_MATRIX_REAL_GAP_INVALID', `${risk.id}: environment`);
  } else if (risk.realGap) fail('DOC83_MATRIX_REAL_GAP_INVALID', `${risk.id}: unexpected gap`);
}

function validateDoc83ClosureMatrix(manifest) {
  if (!manifest || manifest.schemaVersion !== 1 || manifest.ticket !== 'DOC-83' || manifest.capability !== 'ANEXOS_RADICADO_ENLASE') fail('DOC83_MATRIX_HEADER_INVALID', 'header');
  assertNoSensitiveKeys(manifest);
  const policy = manifest.policy;
  const gate = policy?.safeFinalGate;
  if (!policy?.realExecutionRequiresCurrentExplicitAuthorization || !policy?.assignmentUsesIndependentReservation || !policy?.controlsMustBeRegisteredSelects || gate?.enabled !== false || gate.users?.length !== 0 || gate.groups?.length !== 0) fail('DOC83_MATRIX_POLICY_INVALID', 'security policy');
  assertArray(manifest.scenarios, 'DOC83_MATRIX_SCENARIO_INVALID', 'scenarios');
  assertArray(manifest.risks, 'DOC83_MATRIX_RISK_INVALID', 'risks');
  manifest.scenarios.forEach(validateScenarioReference);
  const scenarioIds = new Set(manifest.scenarios.map((entry) => entry.id));
  if (scenarioIds.size !== manifest.scenarios.length) fail('DOC83_MATRIX_SCENARIO_INVALID', 'duplicates');
  manifest.risks.forEach((risk) => validateRisk(risk, scenarioIds));
  const riskIds = new Set(manifest.risks.map((entry) => entry.id));
  if (riskIds.size !== manifest.risks.length) fail('DOC83_MATRIX_RISK_INVALID', 'duplicates');
  for (const scenarioId of scenarioIds) if (!manifest.risks.some((risk) => risk.scenarios.includes(scenarioId))) fail('DOC83_MATRIX_SCENARIO_ORPHAN', scenarioId);
  return Object.freeze({ scenarios: scenarioIds.size, risks: riskIds.size });
}

function loadDoc83ClosureMatrix(file = DEFAULT_MANIFEST) {
  return JSON.parse(fs.readFileSync(file, 'utf8'));
}

module.exports = { DEFAULT_MANIFEST, loadDoc83ClosureMatrix, validateDoc83ClosureMatrix };

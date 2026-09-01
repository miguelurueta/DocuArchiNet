'use strict';

const { NOTES_READ_E2E_ADAPTER } = require('../adapters/notes-read-e2e-adapter.cjs');
const { NOTES_WRITE_E2E_ADAPTER } = require('../adapters/notes-write-e2e-adapter.cjs');

const SAFE_ID = /^[a-z][a-z0-9-]{1,79}$/;
const STAGES = Object.freeze(['anonymous', 'read', 'preview', 'execution', 'concurrency', 'ui-lock']);
const STAGE_HANDLER = Object.freeze({
  anonymous: 'executeAnonymous',
  read: 'executeRead',
  preview: 'executePreview',
  execution: 'executeExecution',
  concurrency: 'executeConcurrency',
  'ui-lock': 'executeUiLock'
});
const SENSITIVE_KEY = /(passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|sql|query|command|comando|script|url.*(?:mysql|database)|database.*url)/i;
const READ_ONLY_SQL = /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i;

function fail(code) {
  const error = new Error(`El registro de plataforma E2E rechazó la configuración (${code}).`);
  error.code = code;
  throw error;
}

function assertId(value, code) {
  if (typeof value !== 'string' || !SAFE_ID.test(value)) fail(code);
}

function assertStringList(value, code) {
  if (!Array.isArray(value) || value.length > 20 || new Set(value).size !== value.length) fail(code);
  for (const item of value) assertId(item, code);
}

function assertReadOnlySql(sql, code) {
  if (typeof sql !== 'string' || !/^\s*SELECT\b/i.test(sql) || READ_ONLY_SQL.test(sql) || (sql.match(/\?/g) || []).length !== 1) {
    fail(code);
  }
}

const CONTROL_REGISTRY = Object.freeze({
  'notes-task-state': Object.freeze({
    id: 'notes-task-state',
    query: 'SELECT ID_ANOTACION, INICIO_TAREAS_WORKFLOW_ID_TAREA, ID_ACTIVIDAD, ID_USUARIO, FECHA_ANOTACION, ESTADO_TAREA FROM ANOTACION_TAREA WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = ? ORDER BY ID_ANOTACION'
  }),
  'notes-audit': Object.freeze({
    id: 'notes-audit',
    query: "SELECT usuario_workflow_idU_suario, fecha_hora, operacion, ID_TAREA_WORKFLOW, opcion, descripcion_opcion, ip_transacion, id_operacion FROM wf_log_workflow WHERE ID_TAREA_WORKFLOW = ? AND descripcion_opcion = 'NOTA WORKFLOW' ORDER BY fecha_hora, id_operacion"
  })
});

const ADAPTER_REGISTRY = Object.freeze({
  [NOTES_READ_E2E_ADAPTER.id]: NOTES_READ_E2E_ADAPTER,
  [NOTES_WRITE_E2E_ADAPTER.id]: NOTES_WRITE_E2E_ADAPTER
});

const SCENARIO_REGISTRY = Object.freeze({
  'notes-anonymous': Object.freeze({
    id: 'notes-anonymous',
    doc: 'doc41',
    stage: 'anonymous',
    adapterId: 'notes-read',
    requiredAuthorizations: Object.freeze([]),
    requiredSecrets: Object.freeze([]),
    resource: null,
    controls: Object.freeze([]),
    controlExpectations: Object.freeze({}),
    transport: Object.freeze({ session: 'none', service: 'notes-modern' }),
    expectations: Object.freeze(['blocked-without-session', 'sanitized-evidence'])
  }),
  'notes-read': Object.freeze({
    id: 'notes-read',
    doc: 'doc41',
    stage: 'read',
    adapterId: 'notes-read',
    requiredAuthorizations: Object.freeze(['environment']),
    requiredSecrets: Object.freeze(['workflow-account', 'workflow-password', 'readonly-db-user', 'readonly-db-password']),
    resource: Object.freeze({ kind: 'workflow-task', role: 'read', profileField: 'taskId', mutating: false }),
    controls: Object.freeze(['notes-task-state', 'notes-audit']),
    controlExpectations: Object.freeze({ 'notes-task-state': 'unchanged', 'notes-audit': 'unchanged' }),
    transport: Object.freeze({ session: 'workflow', service: 'notes-modern' }),
    expectations: Object.freeze(['no-state-change', 'no-audit-change', 'sanitized-evidence'])
  }),
  'notes-write': Object.freeze({
    id: 'notes-write',
    doc: 'doc42',
    stage: 'execution',
    adapterId: 'notes-write',
    requiredAuthorizations: Object.freeze(['environment']),
    requiredSecrets: Object.freeze(['workflow-account', 'workflow-password', 'readonly-db-user', 'readonly-db-password']),
    resource: Object.freeze({ kind: 'workflow-task', role: 'execution', profileField: 'taskId', mutating: true, contractId: 'workflow-task-controls' }),
    controls: Object.freeze(['notes-task-state', 'notes-audit']),
    transport: Object.freeze({ session: 'workflow', service: 'notes-modern' }),
    controlExpectations: Object.freeze({ 'notes-task-state': 'unchanged', 'notes-audit': 'changed' }),
    expectations: Object.freeze(['no-state-change', 'audit-change', 'idempotent-create', 'version-conflict', 'sanitized-evidence'])
  }),
  'notes-concurrency': Object.freeze({
    id: 'notes-concurrency',
    doc: 'doc42',
    stage: 'concurrency',
    adapterId: 'notes-write',
    requiredAuthorizations: Object.freeze(['environment']),
    requiredSecrets: Object.freeze(['workflow-account', 'workflow-password', 'readonly-db-user', 'readonly-db-password']),
    resource: Object.freeze({ kind: 'workflow-task', role: 'concurrency', profileField: 'taskId', mutating: true, contractId: 'workflow-task-controls' }),
    controls: Object.freeze(['notes-task-state', 'notes-audit']),
    controlExpectations: Object.freeze({ 'notes-task-state': 'changed', 'notes-audit': 'changed' }),
    transport: Object.freeze({ session: 'workflow', service: 'notes-modern' }),
    expectations: Object.freeze(['state-change', 'audit-change', 'single-success', 'version-conflict', 'sanitized-evidence'])
  })
});

function validateAdapter(adapter) {
  if (!adapter || typeof adapter !== 'object') fail('E2E_PLATFORM_ADAPTER_INVALID');
  assertId(adapter.id, 'E2E_PLATFORM_ADAPTER_INVALID');
  if (typeof adapter.servicePath !== 'string' || !/^[A-Za-z0-9_./-]+\.asmx$/i.test(adapter.servicePath) || adapter.servicePath.includes('..')) {
    fail('E2E_PLATFORM_ADAPTER_INVALID');
  }
  if (!adapter.operations || typeof adapter.operations !== 'object' || Object.keys(adapter.operations).length === 0) fail('E2E_PLATFORM_ADAPTER_INVALID');
  for (const operation of Object.values(adapter.operations)) {
    if (!operation || typeof operation !== 'object' || typeof operation.id !== 'string' || !/^[A-Za-z][A-Za-z0-9_]{2,80}$/.test(operation.id) || !Array.isArray(operation.payload)) {
      fail('E2E_PLATFORM_ADAPTER_INVALID');
    }
  }
}

function validateScenario(scenario) {
  if (!scenario || typeof scenario !== 'object') fail('E2E_PLATFORM_SCENARIO_INVALID');
  assertId(scenario.id, 'E2E_PLATFORM_SCENARIO_INVALID');
  assertId(scenario.doc, 'E2E_PLATFORM_SCENARIO_INVALID');
  if (!STAGES.includes(scenario.stage)) fail('E2E_PLATFORM_SCENARIO_INVALID');
  assertId(scenario.adapterId, 'E2E_PLATFORM_SCENARIO_INVALID');
  assertStringList(scenario.requiredAuthorizations, 'E2E_PLATFORM_SCENARIO_INVALID');
  assertStringList(scenario.requiredSecrets, 'E2E_PLATFORM_SCENARIO_INVALID');
  assertStringList(scenario.controls, 'E2E_PLATFORM_SCENARIO_INVALID');
  assertStringList(scenario.expectations, 'E2E_PLATFORM_SCENARIO_INVALID');
  if (!scenario.controlExpectations || typeof scenario.controlExpectations !== 'object' || Array.isArray(scenario.controlExpectations)) {
    fail('E2E_PLATFORM_SCENARIO_INVALID');
  }
  const expectationControls = Object.keys(scenario.controlExpectations).sort();
  if (expectationControls.length !== scenario.controls.length || expectationControls.some((id, index) => id !== [...scenario.controls].sort()[index])) {
    fail('E2E_PLATFORM_SCENARIO_INVALID');
  }
  for (const expectation of Object.values(scenario.controlExpectations)) {
    if (expectation !== 'changed' && expectation !== 'unchanged') fail('E2E_PLATFORM_SCENARIO_INVALID');
  }
  if (!scenario.transport || typeof scenario.transport !== 'object' || !['none', 'workflow'].includes(scenario.transport.session) || typeof scenario.transport.service !== 'string') {
    fail('E2E_PLATFORM_SCENARIO_INVALID');
  }
  if (scenario.resource !== null) {
    if (!scenario.resource || typeof scenario.resource !== 'object' || typeof scenario.resource.mutating !== 'boolean') fail('E2E_PLATFORM_SCENARIO_INVALID');
    assertId(scenario.resource.kind, 'E2E_PLATFORM_SCENARIO_INVALID');
    assertId(scenario.resource.role, 'E2E_PLATFORM_SCENARIO_INVALID');
    if (typeof scenario.resource.profileField !== 'string' || !/^[a-z][A-Za-z0-9]{1,79}$/.test(scenario.resource.profileField)) fail('E2E_PLATFORM_SCENARIO_INVALID');
    if (scenario.resource.mutating && (typeof scenario.resource.contractId !== 'string' || !SAFE_ID.test(scenario.resource.contractId))) {
      fail('E2E_PLATFORM_SCENARIO_INVALID');
    }
  }
}

function validateRegistry() {
  for (const [id, control] of Object.entries(CONTROL_REGISTRY)) {
    assertId(id, 'E2E_PLATFORM_CONTROL_INVALID');
    if (!control || control.id !== id || SENSITIVE_KEY.test(id)) fail('E2E_PLATFORM_CONTROL_INVALID');
    assertReadOnlySql(control.query, 'E2E_PLATFORM_CONTROL_INVALID');
  }
  for (const [id, adapter] of Object.entries(ADAPTER_REGISTRY)) {
    assertId(id, 'E2E_PLATFORM_ADAPTER_INVALID');
    validateAdapter(adapter);
    if (adapter.id !== id) fail('E2E_PLATFORM_ADAPTER_INVALID');
  }
  for (const [id, scenario] of Object.entries(SCENARIO_REGISTRY)) {
    assertId(id, 'E2E_PLATFORM_SCENARIO_INVALID');
    validateScenario(scenario);
    if (scenario.id !== id || !ADAPTER_REGISTRY[scenario.adapterId]) fail('E2E_PLATFORM_SCENARIO_INVALID');
    if (typeof ADAPTER_REGISTRY[scenario.adapterId][STAGE_HANDLER[scenario.stage]] !== 'function') fail('E2E_PLATFORM_SCENARIO_INVALID');
    for (const control of scenario.controls) if (!CONTROL_REGISTRY[control]) fail('E2E_PLATFORM_SCENARIO_INVALID');
  }
}

function resolveScenario(id) {
  if (typeof id !== 'string' || !SCENARIO_REGISTRY[id]) fail('E2E_PLATFORM_SCENARIO_UNREGISTERED');
  return SCENARIO_REGISTRY[id];
}

function resolveAdapter(id) {
  if (typeof id !== 'string' || !ADAPTER_REGISTRY[id]) fail('E2E_PLATFORM_ADAPTER_UNREGISTERED');
  return ADAPTER_REGISTRY[id];
}

function resolveControls(ids) {
  if (!Array.isArray(ids)) fail('E2E_PLATFORM_CONTROL_UNREGISTERED');
  return Object.freeze(ids.map((id) => {
    if (typeof id !== 'string' || !CONTROL_REGISTRY[id]) fail('E2E_PLATFORM_CONTROL_UNREGISTERED');
    return CONTROL_REGISTRY[id];
  }));
}

validateRegistry();

module.exports = {
  ADAPTER_REGISTRY,
  CONTROL_REGISTRY,
  SCENARIO_REGISTRY,
  STAGE_HANDLER,
  STAGES,
  resolveAdapter,
  resolveControls,
  resolveScenario,
  validateRegistry
};

'use strict';

const mode = process.argv[2];

const authenticated = [
  'DOC28_E2E_BASE_URL',
  'DOC28_E2E_MODULE',
  'DOC28_E2E_AUTHORIZED_USER',
  'DOC28_E2E_AUTHORIZED_PASSWORD'
];

const requiredByMode = {
  anonymous: ['DOC28_E2E_BASE_URL'],
  validation: authenticated,
  preview: [
    ...authenticated,
    'DOC28_E2E_TASK_ID',
    'DOC28_E2E_MYSQL_URL',
    'DOC28_E2E_TASK_STATE_SQL',
    'DOC28_E2E_AUDIT_SQL'
  ],
  execute: [
    ...authenticated,
    'DOC28_E2E_EXECUTION_AUTHORIZED',
    'DOC28_E2E_TASK_ID',
    'DOC28_E2E_EXPECTED_OUTCOME',
    'DOC28_E2E_MYSQL_URL',
    'DOC28_E2E_TASK_STATE_SQL',
    'DOC28_E2E_AUDIT_SQL'
  ],
  concurrency: [
    ...authenticated,
    'DOC28_E2E_EXECUTION_AUTHORIZED',
    'DOC28_E2E_CONCURRENCY_AUTHORIZED',
    'DOC28_E2E_TASK_ID',
    'DOC28_E2E_MYSQL_URL',
    'DOC28_E2E_TASK_STATE_SQL',
    'DOC28_E2E_AUDIT_SQL'
  ]
};

function exit(message) {
  console.error(message);
  process.exit(2);
}

function isPositiveInteger(value) {
  return /^\d+$/.test(value || '') && Number(value) > 0;
}

function validateOptionalBoolean(name) {
  const value = process.env[name];
  if (value !== undefined && !['true', 'false'].includes(value.toLowerCase())) {
    exit(`${name} debe ser true o false cuando se configure.`);
  }
}

function validateOptionalPageSize() {
  const value = process.env.DOC28_E2E_PAGE_SIZE;
  if (value !== undefined && (!isPositiveInteger(value) || Number(value) > 50)) {
    exit('DOC28_E2E_PAGE_SIZE debe ser un entero entre 1 y 50 cuando se configure.');
  }
}

if (!Object.hasOwn(requiredByMode, mode)) {
  exit('Modo inválido. Use anonymous, validation, preview, execute o concurrency.');
}

const missing = requiredByMode[mode].filter((name) => !process.env[name] || !process.env[name].trim());
if (missing.length > 0) {
  exit(`Faltan variables DOC-28 requeridas: ${missing.join(', ')}. No se mostraron valores.`);
}

try {
  new URL(process.env.DOC28_E2E_BASE_URL);
} catch {
  exit('DOC28_E2E_BASE_URL debe ser una URL absoluta válida.');
}

if (mode === 'preview' || mode === 'execute' || mode === 'concurrency') {
  if (!isPositiveInteger(process.env.DOC28_E2E_TASK_ID)) {
    exit('DOC28_E2E_TASK_ID debe ser un entero positivo.');
  }
}

if (mode === 'preview' || mode === 'execute' || mode === 'concurrency') {
  validateOptionalPageSize();
}

if (mode === 'preview') {
  validateOptionalBoolean('DOC28_E2E_EXPECT_PAGINATION');
}

if (mode === 'execute' || mode === 'concurrency') {
  if (process.env.DOC28_E2E_EXECUTION_AUTHORIZED.toLowerCase() !== 'true') {
    exit('DOC28_E2E_EXECUTION_AUTHORIZED debe ser exactamente true para permitir una prueba que cambia estado.');
  }
}

if (mode === 'concurrency' && process.env.DOC28_E2E_CONCURRENCY_AUTHORIZED.toLowerCase() !== 'true') {
  exit('DOC28_E2E_CONCURRENCY_AUTHORIZED debe ser exactamente true para permitir una carrera mutante.');
}

if (mode === 'execute') {
  const outcome = process.env.DOC28_E2E_EXPECTED_OUTCOME.toLowerCase();
  if (!['success', 'blocked'].includes(outcome)) {
    exit('DOC28_E2E_EXPECTED_OUTCOME debe ser success o blocked.');
  }
  if (outcome === 'blocked' && (!process.env.DOC28_E2E_EXPECTED_CODE || !process.env.DOC28_E2E_EXPECTED_CODE.trim())) {
    exit('DOC28_E2E_EXPECTED_CODE es obligatorio cuando DOC28_E2E_EXPECTED_OUTCOME es blocked.');
  }
}

const mode = process.argv[2];

const commonAuthenticated = [
  'DOC11_E2E_BASE_URL',
  'DOC11_E2E_MODULE',
  'DOC11_E2E_AUTHORIZED_USER',
  'DOC11_E2E_AUTHORIZED_PASSWORD'
];

const requiredByMode = {
  anonymous: ['DOC11_E2E_BASE_URL'],
  validation: commonAuthenticated,
  execute: [
    ...commonAuthenticated,
    'DOC11_E2E_EXECUTION_AUTHORIZED',
    'DOC11_E2E_TASK_ID',
    'DOC11_E2E_CONNECTOR_ID',
    'DOC11_E2E_TOKEN_VERSION',
    'DOC11_E2E_EXPECTED_OUTCOME',
    'DOC11_E2E_MYSQL_URL',
    'DOC11_E2E_TASK_STATE_SQL',
    'DOC11_E2E_AUDIT_SQL'
  ],
  concurrency: [
    ...commonAuthenticated,
    'DOC11_E2E_EXECUTION_AUTHORIZED',
    'DOC11_E2E_TASK_ID',
    'DOC11_E2E_CONNECTOR_ID',
    'DOC11_E2E_TOKEN_VERSION',
    'DOC11_E2E_MYSQL_URL',
    'DOC11_E2E_TASK_STATE_SQL',
    'DOC11_E2E_AUDIT_SQL'
  ]
};

if (!Object.hasOwn(requiredByMode, mode)) {
  console.error('Modo inválido. Use anonymous, validation, execute o concurrency.');
  process.exit(2);
}

const missing = requiredByMode[mode].filter((name) => !process.env[name] || !process.env[name].trim());
if (missing.length > 0) {
  console.error(`Faltan variables DOC-11 requeridas: ${missing.join(', ')}. No se mostraron valores.`);
  process.exit(2);
}

try {
  new URL(process.env.DOC11_E2E_BASE_URL);
} catch {
  console.error('DOC11_E2E_BASE_URL debe ser una URL absoluta válida.');
  process.exit(2);
}

if (mode === 'execute' || mode === 'concurrency') {
  for (const name of ['DOC11_E2E_TASK_ID', 'DOC11_E2E_CONNECTOR_ID']) {
    if (!/^\d+$/.test(process.env[name]) || Number(process.env[name]) <= 0) {
      console.error(`${name} debe ser un entero positivo.`);
      process.exit(2);
    }
  }
  if (process.env.DOC11_E2E_EXECUTION_AUTHORIZED.toLowerCase() !== 'true') {
    console.error('DOC11_E2E_EXECUTION_AUTHORIZED debe ser exactamente true para permitir una prueba que cambia estado.');
    process.exit(2);
  }
}

if (mode === 'execute' && !['success', 'blocked'].includes(process.env.DOC11_E2E_EXPECTED_OUTCOME.toLowerCase())) {
  console.error('DOC11_E2E_EXPECTED_OUTCOME debe ser success o blocked.');
  process.exit(2);
}

if (mode === 'execute' && process.env.DOC11_E2E_EXPECTED_OUTCOME.toLowerCase() === 'blocked' &&
  (!process.env.DOC11_E2E_EXPECTED_CODE || !process.env.DOC11_E2E_EXPECTED_CODE.trim())) {
  console.error('DOC11_E2E_EXPECTED_CODE es obligatorio cuando DOC11_E2E_EXPECTED_OUTCOME es blocked.');
  process.exit(2);
}

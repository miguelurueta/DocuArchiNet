const mode = process.argv[2];

const requiredByMode = {
  anonymous: ['DOC10_E2E_BASE_URL'],
  session: [
    'DOC10_E2E_BASE_URL',
    'DOC10_E2E_MODULE',
    'DOC10_E2E_AUTHORIZED_USER',
    'DOC10_E2E_AUTHORIZED_PASSWORD'
  ],
  contexts: [
    'DOC10_E2E_BASE_URL',
    'DOC10_E2E_MODULE',
    'DOC10_E2E_AUTHORIZED_USER',
    'DOC10_E2E_AUTHORIZED_PASSWORD',
    'DOC10_E2E_SECONDARY_USER',
    'DOC10_E2E_SECONDARY_PASSWORD',
    'DOC10_E2E_TASK_ID'
  ],
  full: [
    'DOC10_E2E_BASE_URL',
    'DOC10_E2E_MODULE',
    'DOC10_E2E_AUTHORIZED_USER',
    'DOC10_E2E_AUTHORIZED_PASSWORD',
    'DOC10_E2E_TASK_ID',
    'DOC10_E2E_MYSQL_URL',
    'DOC10_E2E_AUDIT_SQL'
  ],
  load: [
    'DOC10_E2E_BASE_URL',
    'DOC10_E2E_MODULE',
    'DOC10_E2E_AUTHORIZED_USER',
    'DOC10_E2E_AUTHORIZED_PASSWORD',
    'DOC10_E2E_TASK_ID',
    'DOC10_E2E_MYSQL_URL',
    'DOC10_E2E_AUDIT_SQL'
  ]
};

if (!Object.hasOwn(requiredByMode, mode)) {
  console.error('Modo inválido. Use anonymous, session, contexts, full o load.');
  process.exit(2);
}

const missing = requiredByMode[mode].filter((name) => !process.env[name] || !process.env[name].trim());
if (missing.length > 0) {
  console.error(`Faltan variables E2E requeridas: ${missing.join(', ')}. No se mostraron valores.`);
  process.exit(2);
}

try {
  new URL(process.env.DOC10_E2E_BASE_URL);
} catch {
  console.error('DOC10_E2E_BASE_URL debe ser una URL absoluta válida.');
  process.exit(2);
}

if ((mode === 'contexts' || mode === 'full' || mode === 'load') && (!/^\d+$/.test(process.env.DOC10_E2E_TASK_ID) || Number(process.env.DOC10_E2E_TASK_ID) <= 0)) {
  console.error('DOC10_E2E_TASK_ID debe ser un entero positivo.');
  process.exit(2);
}

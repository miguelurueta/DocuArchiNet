const { chromium } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');
const mysql = require('mysql2/promise');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const DEFAULT_TASK_STATE_SQL = [
  'SELECT ID_ESTADO, INICIO_TAREAS_WORKFLOW_ID_TAREA, ID_ACTIVIDAD,',
  'FECHA_INICIO, FECHA_SELECCION, FECHA_FIN, ESTADO_TAREA, ID_USUARIO,',
  'ID_FLUJO_TRABAJO, ID_ACTIVIDAD_FLUJO_TRABAJO',
  'FROM estados_tarea_workflow',
  'WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = ?',
  'ORDER BY ID_ESTADO'
].join(' ');

function required(name) {
  const value = process.env[name];
  return value && value.trim() ? value.trim() : null;
}

function normalizeBaseUrl(value) {
  return new URL(value.endsWith('/') ? value : `${value}/`).toString();
}

function getPositiveInteger(value, name, fallback) {
  const resolved = value === undefined || value === null || value === '' ? fallback : Number(value);
  if (!Number.isSafeInteger(resolved) || resolved <= 0) {
    throw new Error(`${name} debe ser un entero positivo.`);
  }
  return resolved;
}

function getNonNegativeNumber(value, name, fallback) {
  const resolved = value === undefined || value === null || value === '' ? fallback : Number(value);
  if (!Number.isFinite(resolved) || resolved < 0) {
    throw new Error(`${name} debe ser un número mayor o igual a cero.`);
  }
  return resolved;
}

function getBoundedPositiveInteger(value, name, fallback, maximum) {
  const resolved = getPositiveInteger(value, name, fallback);
  if (resolved > maximum) throw new Error(`${name} no puede superar ${maximum}.`);
  return resolved;
}

function getConcurrencyLevels() {
  const raw = required('DOC10_LOAD_CONCURRENCIES') || '20,30';
  const levels = raw.split(',').map((value) => getPositiveInteger(value.trim(), 'DOC10_LOAD_CONCURRENCIES')).filter((value, index, all) => all.indexOf(value) === index);
  if (levels.length === 0 || levels.some((value) => value > 50)) {
    throw new Error('DOC10_LOAD_CONCURRENCIES debe contener entre uno y 50 usuarios por nivel.');
  }
  return levels;
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql)) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura.`);
  }
  if ((sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe usar exactamente un parámetro posicional ? para idTarea.`);
  }
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

function percentile(samples, ratio) {
  if (samples.length === 0) return null;
  const index = Math.min(samples.length - 1, Math.max(0, Math.ceil(samples.length * ratio) - 1));
  return Math.round(samples[index] * 100) / 100;
}

function createErrorCounts(results) {
  return results.filter((result) => !result.ok).reduce((counts, result) => {
    const key = result.code || 'REQUEST_FAILED';
    counts[key] = (counts[key] || 0) + 1;
    return counts;
  }, {});
}

function createLoginErrorCounts(sessions) {
  return sessions.filter((session) => session.status === 'rejected').reduce((counts, session) => {
    const key = session.reason && session.reason.name === 'TimeoutError' ? 'LOGIN_TIMEOUT' : 'LOGIN_FAILED';
    counts[key] = (counts[key] || 0) + 1;
    return counts;
  }, {});
}

function getEvidencePath() {
  const configured = required('DOC10_LOAD_EVIDENCE_PATH');
  if (!configured) return path.join(__dirname, '..', 'artifacts', 'doc10-preview-load.json');
  return path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured);
}

const settings = {
  baseUrl: normalizeBaseUrl(required('DOC10_E2E_BASE_URL')),
  idTarea: getPositiveInteger(required('DOC10_E2E_TASK_ID'), 'DOC10_E2E_TASK_ID'),
  mysqlUrl: process.env.DOC10_E2E_MYSQL_URL,
  auditSql: process.env.DOC10_E2E_AUDIT_SQL,
  taskStateSql: process.env.DOC10_E2E_TASK_STATE_SQL || DEFAULT_TASK_STATE_SQL,
  concurrencyLevels: getConcurrencyLevels(),
  requestsPerSession: getPositiveInteger(process.env.DOC10_LOAD_REQUESTS_PER_SESSION, 'DOC10_LOAD_REQUESTS_PER_SESSION', 1),
  loginConcurrency: getBoundedPositiveInteger(process.env.DOC10_LOAD_LOGIN_CONCURRENCY, 'DOC10_LOAD_LOGIN_CONCURRENCY', 5, 20),
  loginTimeoutMilliseconds: getBoundedPositiveInteger(process.env.DOC10_LOAD_LOGIN_TIMEOUT_MS, 'DOC10_LOAD_LOGIN_TIMEOUT_MS', 30000, 120000),
  maximumFailurePercent: getNonNegativeNumber(process.env.DOC10_LOAD_MAX_FAILURE_PERCENT, 'DOC10_LOAD_MAX_FAILURE_PERCENT', 0),
  maximumP95Milliseconds: process.env.DOC10_LOAD_MAX_P95_MS ? getNonNegativeNumber(process.env.DOC10_LOAD_MAX_P95_MS, 'DOC10_LOAD_MAX_P95_MS', 0) : null,
  evidencePath: getEvidencePath(),
  browserChannel: required('DOC10_LOAD_BROWSER_CHANNEL') || required('DOC10_E2E_BROWSER_CHANNEL'),
  browserPath: required('DOC10_LOAD_BROWSER_PATH') || required('DOC10_E2E_BROWSER_PATH'),
  ignoreHttpsErrors: process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS === 'true'
};

function previewUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea', settings.baseUrl).toString();
}

async function queryFingerprint(pool, sql) {
  const [rows] = await pool.execute(sql, [settings.idTarea]);
  return fingerprint(rows);
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: settings.baseUrl,
    moduleEnvironmentVariable: 'DOC10_E2E_MODULE',
    userEnvironmentVariable: 'DOC10_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC10_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: settings.ignoreHttpsErrors,
    timeoutMilliseconds: settings.loginTimeoutMilliseconds
  });
}

async function createSessions(browser, total) {
  const sessions = new Array(total);
  let nextIndex = 0;
  const workers = Array.from({ length: Math.min(total, settings.loginConcurrency) }, async () => {
    while (nextIndex < total) {
      const currentIndex = nextIndex;
      nextIndex += 1;
      try {
        sessions[currentIndex] = { status: 'fulfilled', value: await login(browser) };
      } catch (reason) {
        sessions[currentIndex] = { status: 'rejected', reason };
      }
    }
  });
  await Promise.all(workers);
  return sessions;
}

async function invokePreview(context) {
  const started = performance.now();
  try {
    const response = await context.request.post(previewUrl(), {
      headers: { 'X-Requested-With': 'XMLHttpRequest' },
      data: { idTarea: settings.idTarea },
      timeout: 60000
    });
    const elapsedMilliseconds = performance.now() - started;
    if (!response.ok()) return { ok: false, elapsedMilliseconds, code: `HTTP_${response.status()}` };

    const envelope = await response.json();
    const preview = envelope && envelope.d;
    if (!preview) return { ok: false, elapsedMilliseconds, code: 'INVALID_RESPONSE' };
    if (preview.Error) return { ok: false, elapsedMilliseconds, code: preview.Error.Codigo || 'FUNCTIONAL_BLOCK' };
    if (!Array.isArray(preview.Destinos) || preview.Destinos.length === 0) {
      return { ok: false, elapsedMilliseconds, code: 'NO_DESTINATIONS' };
    }
    return { ok: true, elapsedMilliseconds, code: null };
  } catch (error) {
    return { ok: false, elapsedMilliseconds: performance.now() - started, code: 'REQUEST_FAILED' };
  }
}

function launchOptions() {
  const options = { headless: true };
  if (settings.browserPath) options.executablePath = settings.browserPath;
  else if (settings.browserChannel) options.channel = settings.browserChannel;
  return options;
}

async function runLevel(browser, pool, concurrency) {
  const report = {
    concurrencia: concurrency,
    solicitudesPorSesion: settings.requestsPerSession,
    paralelismoLogin: settings.loginConcurrency,
    sesionesAutenticadas: 0,
    sesionesFallidas: 0,
    solicitudes: 0,
    exitosas: 0,
    fallidas: 0,
    porcentajeFallos: null,
    latenciaMs: { p50: null, p95: null, p99: null, minimo: null, maximo: null },
    estadoSinMutacion: false,
    auditoriaSinMutacion: false,
    erroresSesion: {},
    errores: {}
  };
  let contexts = [];
  let beforeTask;
  let beforeAudit;
  let afterTask;
  let afterAudit;
  let results = [];

  try {
    beforeTask = await queryFingerprint(pool, settings.taskStateSql);
    beforeAudit = await queryFingerprint(pool, settings.auditSql);

    const sessions = await createSessions(browser, concurrency);
    contexts = sessions.filter((session) => session.status === 'fulfilled').map((session) => session.value);
    report.sesionesAutenticadas = contexts.length;
    report.sesionesFallidas = sessions.length - contexts.length;
    report.erroresSesion = createLoginErrorCounts(sessions);

    const requests = contexts.flatMap((context) => Array.from({ length: settings.requestsPerSession }, () => invokePreview(context)));
    results = await Promise.all(requests);
  } finally {
    try {
      afterTask = await queryFingerprint(pool, settings.taskStateSql);
      afterAudit = await queryFingerprint(pool, settings.auditSql);
    } finally {
      await Promise.all(contexts.map((context) => context.close()));
    }
  }

  report.solicitudes = results.length;
  report.exitosas = results.filter((result) => result.ok).length;
  report.fallidas = results.length - report.exitosas;
  report.porcentajeFallos = report.solicitudes === 0 ? 100 : Math.round((report.fallidas * 10000) / report.solicitudes) / 100;
  report.errores = createErrorCounts(results);
  const samples = results.map((result) => result.elapsedMilliseconds).sort((left, right) => left - right);
  report.latenciaMs = {
    p50: percentile(samples, 0.5),
    p95: percentile(samples, 0.95),
    p99: percentile(samples, 0.99),
    minimo: samples.length ? Math.round(samples[0] * 100) / 100 : null,
    maximo: samples.length ? Math.round(samples[samples.length - 1] * 100) / 100 : null
  };
  report.estadoSinMutacion = beforeTask === afterTask;
  report.auditoriaSinMutacion = beforeAudit === afterAudit;
  report.aprobada = report.sesionesFallidas === 0 &&
    report.porcentajeFallos <= settings.maximumFailurePercent &&
    report.estadoSinMutacion && report.auditoriaSinMutacion &&
    (settings.maximumP95Milliseconds === null || (report.latenciaMs.p95 !== null && report.latenciaMs.p95 <= settings.maximumP95Milliseconds));
  return report;
}

async function writeEvidence(evidence) {
  await fs.mkdir(path.dirname(settings.evidencePath), { recursive: true });
  await fs.writeFile(settings.evidencePath, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function main() {
  assertReadOnlySql(settings.taskStateSql, 'DOC10_E2E_TASK_STATE_SQL');
  assertReadOnlySql(settings.auditSql, 'DOC10_E2E_AUDIT_SQL');

  const browser = await chromium.launch(launchOptions());
  const pool = mysql.createPool(settings.mysqlUrl);
  let levels = [];
  try {
    for (const concurrency of settings.concurrencyLevels) {
      levels.push(await runLevel(browser, pool, concurrency));
    }
  } finally {
    await pool.end();
    await browser.close();
  }

  const evidence = {
    fechaUtc: new Date().toISOString(),
    endpoint: previewUrl(),
    idTarea: settings.idTarea,
    rolAutenticado: 'contexto-workflow-valido',
    parametros: {
      concurrencias: settings.concurrencyLevels,
      solicitudesPorSesion: settings.requestsPerSession,
      paralelismoLogin: settings.loginConcurrency,
      maximoPorcentajeFallos: settings.maximumFailurePercent,
      maximoP95Ms: settings.maximumP95Milliseconds
    },
    resultados: levels,
    aprobada: levels.every((level) => level.aprobada)
  };
  await writeEvidence(evidence);

  for (const level of levels) {
    console.log(`Concurrencia ${level.concurrencia}: ${level.sesionesAutenticadas}/${level.concurrencia} sesiones, ${level.exitosas}/${level.solicitudes} solicitudes exitosas, p95 ${level.latenciaMs.p95} ms, fallos ${level.porcentajeFallos}%.`);
  }
  if (!evidence.aprobada) process.exitCode = 1;
}

main().catch(async () => {
  console.error('La prueba de concurrencia no pudo completarse. No se mostraron secretos ni detalles internos.');
  process.exitCode = 1;
});

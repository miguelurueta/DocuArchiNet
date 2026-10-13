'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
  allowedFailureFields,
  classifyBrowserFailure,
  classifyPageError,
  deriveResult,
  loadProfile,
  summarize
} = require('../scripts/run-workflow-pdf-viewer-performance-interactive.cjs');

const runnerPath = path.resolve(__dirname, '../scripts/run-workflow-pdf-viewer-performance-interactive.cjs');
const runner = fs.readFileSync(runnerPath, 'utf8');

function withProfile(value, action) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'workflow-pdf-perf-policy-'));
  const target = path.join(directory, 'profile.json');
  fs.writeFileSync(target, JSON.stringify(value), 'utf8');
  try { return action(target); } finally { fs.rmSync(directory, { recursive: true, force: true }); }
}

function validProfile() {
  return {
    baseUrl: 'https://workflow.example.invalid/app/', module: 'GESTOR', taskId: 1,
    documentOrdinal: 1, documentTitle: 'Estados Financieros', iterations: 3, scrollSteps: 12, timeoutMilliseconds: 120000,
    headless: true, browser: { channel: 'chrome' }, ignoreHttpsErrors: false
  };
}

test('el perfil del visor admite solo datos no sensibles y límites acotados', () => {
  const profile = withProfile(validProfile(), loadProfile);
  assert.equal(profile.taskId, 1);
  assert.equal(profile.iterations, 3);
  assert.equal(profile.documentOrdinal, 1);
  assert.equal(profile.documentTitle, 'Estados Financieros');
  assert.throws(() => withProfile({ ...validProfile(), password: 'secreto' }, loadProfile), /WORKFLOW_PDF_PERF_PROFILE_FIELD_NOT_ALLOWED/);
  assert.throws(() => withProfile({ ...validProfile(), iterations: 11 }, loadProfile), /WORKFLOW_PDF_PERF_ITERATIONS_INVALID/);
});

test('la evidencia resume tiempos sin incluir identidad de tarea, documento ni URL', () => {
  assert.deepEqual(summarize([{ firstCanvasMs: 900 }, { firstCanvasMs: 300 }, { firstCanvasMs: 600 }]), {
    minimumMs: 300, medianMs: 600, maximumMs: 900
  });
  assert.doesNotMatch(runner, /evidence[\s\S]{0,160}(?:taskId|documentOrdinal|documentTitle|baseUrl)/);
  assert.doesNotMatch(runner, /console\.(?:log|error)\([^\n]*(?:url|src|rut_image|cookie)/i);
});

test('la medición abre el visor oficial sin invocar persistencia de grafos', () => {
  assert.match(runner, /createAuthenticatedWorkflowSession/);
  assert.match(runner, /tip_event=\"seleccion_tarea_wf\"/);
  assert.match(runner, /tip_event=\"vis_doc_selecion_wf\"/);
  assert.match(runner, /#ifrm_visor_/);
  assert.match(runner, /pdf_anotate_view/);
  assert.match(runner, /WORKFLOW_PDF_PERF_TASK_LIST_PERMISSION_DENIED/);
  assert.match(runner, /WORKFLOW_PDF_PERF_DIAGNOSTIC/);
  assert.match(runner, /handlerStatuses/);
  assert.match(runner, /WORKFLOW_PDF_PERF_DOCUMENT_TITLE_NOT_FOUND/);
  assert.match(runner, /contentWindow/);
  assert.match(runner, /typeof pdfViewer === 'undefined'/);
  assert.match(runner, /WORKFLOW_PDF_PERF_SCROLL_COMPLETED/);
  assert.doesNotMatch(runner, /service_anotate_pdf_image/);
  assert.doesNotMatch(runner, /Pdf_btn_update_insert/);
});

test('clasifica errores con un contrato cerrado y saneado', () => {
  const cases = [
    [{ type: 'error', source: 'main_frame', fileName: 'https://host/app.js?task=99', line: 8 }, 'WORKFLOW_PDF_SCRIPT_ERROR'],
    [{ type: 'pageerror', source: 'viewer', fileName: 'C:\\private\\viewer.js', line: 19, message: 'documento secreto' }, 'WORKFLOW_PDF_PAGE_ERROR'],
    [{ type: 'error', source: 'worker', fileName: '/pdf.worker.js#token', line: 4 }, 'WORKFLOW_PDF_WORKER_ERROR'],
    [{ type: 'resource', source: 'optional_resource', fileName: '/fonts/ui.woff2?user=ana' }, 'WORKFLOW_PDF_OPTIONAL_RESOURCE_FAILED'],
    [{ type: 'not-known', source: 'not-known', fileName: '/folder/file.js' }, 'WORKFLOW_PDF_UNKNOWN_FAILURE']
  ];
  for (const [input, code] of cases) {
    const classified = classifyBrowserFailure(input);
    assert.deepEqual(Object.keys(classified), allowedFailureFields);
    assert.equal(classified.code, code);
    assert.doesNotMatch(JSON.stringify(classified), /https?:|private|task=|token|documento secreto|user=ana/i);
  }
});

test('pageerror extrae solo archivo y línea sin conservar stack, URL ni mensaje', () => {
  const classified = classifyPageError({
    name: 'ReferenceError',
    message: 'documento privado',
    stack: 'ReferenceError: documento privado\n    at https://host.invalid/pdfjs/pdf_anotate_view/pdf.worker.entry.min.js?token=secret:1:45'
  });
  assert.deepEqual(classified, {
    code: 'WORKFLOW_PDF_PAGE_ERROR', source: 'viewer', type: 'pageerror', fileBase: 'pdf.worker.entry.min.js', line: 1
  });
  assert.doesNotMatch(JSON.stringify(classified), /https?:|token|secret|documento|stack/i);
});

test('deriva PASSED, PASSED_WITH_OBSERVATIONS y FAILED sin falsos limpios', () => {
  const milestones = { documentSelected: true, firstCanvas: true, scroll: true, zoom: true, reloads: true };
  const optional = classifyBrowserFailure({ type: 'resource', source: 'optional_resource', fileName: 'ui.woff2' });
  const optionalScript = classifyBrowserFailure({ type: 'error', source: 'optional_resource', fileName: 'ui.js' });
  const viewer = classifyBrowserFailure({ type: 'pageerror', source: 'viewer', fileName: 'viewer.js' });
  assert.equal(deriveResult({ milestones, failures: [] }), 'PASSED');
  assert.equal(deriveResult({ milestones, failures: [optional] }), 'PASSED_WITH_OBSERVATIONS');
  assert.equal(deriveResult({ milestones, failures: [optionalScript] }), 'FAILED');
  assert.equal(deriveResult({ milestones, failures: [viewer] }), 'FAILED');
  assert.equal(deriveResult({ milestones: { ...milestones, zoom: false }, failures: [] }), 'FAILED');
  assert.doesNotMatch(runner, /WORKFLOW_PDF_PERF_RESULT=PASSED['"`]/);
});

'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
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

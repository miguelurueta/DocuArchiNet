'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

const root = path.resolve(__dirname, '..', '..', '..');
const read = (...segments) => fs.readFileSync(path.join(root, ...segments), 'utf8');

const page = read('workflow', 'Webworkflow.aspx');
const codeBehind = read('workflow', 'Webworkflow.aspx.vb');
const scanner = read('js', 'workflow', 'WebFormEscan.js');
const transport = read('Resources', 'online_demo_operation.js');
const legacyStyles = read('Styles', 'Aplicaction.css');
const modernStyles = read('Styles', 'workflow-centro-trabajo-moderno.css');
const platformRunner = read('tools', 'e2e', 'scripts', 'run-workflow-e2e-platform.cjs');
const platformCore = read('tools', 'e2e', 'scripts', 'support', 'workflow-e2e-platform.cjs');
const profile = validateProfile(JSON.parse(read('tools', 'e2e', 'profiles', 'doc89-scanner-link-overlay.profile.example.json')));

const between = (source, start, end) => {
  const startIndex = source.indexOf(start);
  assert.notEqual(startIndex, -1, `No se encontro el inicio: ${start}`);
  const endIndex = source.indexOf(end, startIndex + start.length);
  assert.notEqual(endIndex, -1, `No se encontro el final: ${end}`);
  return source.slice(startIndex, endIndex);
};

const initializeRequest = between(page, 'function InitializeRequest(sender, args)', 'function CheckStatus(sender, args)');
const checkStatus = between(page, 'function CheckStatus(sender, args)', '</script>');
const buttonPanel = between(page, '<asp:UpdatePanel ID="UpdatePanel_boton_tool"', '</asp:UpdatePanel>');
const storeHandler = between(
  codeBehind,
  'Private Sub ButtonAlmacenar_Click',
  "'----------Seleccionar tarea",
);
const scannerContinuation = between(scanner, 'function activa_document_save()', 'function progres_hiden(progres)');
const uploadSuccess = between(transport, 'function OnHttpUploadSuccess()', '//fuccion que activa la carga');
const uploadFailure = between(transport, 'function OnHttpUploadFailure', '//guardar documento local');

test('DOC-89 conserva ButtonAlmacenar como async postback del UpdatePanel existente', () => {
  assert.match(buttonPanel, /<asp:Button ID="ButtonAlmacenar"[^>]*runat="server"/);
  assert.doesNotMatch(page, /<asp:PostBackTrigger\s+ControlID="ButtonAlmacenar"/i);
  assert.doesNotMatch(codeBehind, /RegisterPostBackControl\s*\(\s*(?:Me\.)?ButtonAlmacenar\s*\)/i);
  assert.doesNotMatch(initializeRequest, /location\.reload|window\.location|\.src\s*=/i);
});

test('DOC-89 aisla un indicador compacto para ButtonAlmacenar sin overlay de viewport', () => {
  assert.match(initializeRequest, /if\s*\(elment_postbak\.id\s*==\s*["']ButtonAlmacenar["']\)\s*\{[\s\S]*?mostrar_progreso_almacenamiento_digitalizado\(\)/);
  assert.doesNotMatch(initializeRequest, /ButtonAlmacenar["']\s*\|\|[\s\S]{0,120}Button_guardar_desicion/);

  const compactProgress = between(
    page,
    'function mostrar_progreso_almacenamiento_digitalizado()',
    'function limpiar_progreso_almacenamiento_digitalizado()',
  );
  assert.match(compactProgress, /classList\.remove\(["']overlay_["']\)/);
  assert.match(compactProgress, /style\.display\s*=\s*["']block["']/);
  assert.match(compactProgress, /style\.width\s*=\s*["']200px["']/);
  assert.doesNotMatch(compactProgress, /addClass\(["']overlay_["']\)|style\.(?:width|height)\s*=\s*["']100%["']/);
});

test('DOC-89 limpia el progreso compacto en finally y preserva Button_guardar_desicion', () => {
  assert.match(checkStatus, /finally\s*\{[\s\S]*?limpiar_progreso_almacenamiento_digitalizado\(\)[\s\S]*?progres_hiden\(["']progres_bar["']\)/);
  assert.match(initializeRequest, /if\s*\(elment_postbak\.id\s*==\s*["']Button_guardar_desicion["']\)\s*\{[\s\S]*?posicion_update_pogres_modal\(["']progres_bar["']\)/);
  assert.match(checkStatus, /elment_postbak\.id\s*==\s*["']Button_guardar_desicion["'][\s\S]{0,160}removeClass\(["']overlay_["']\)/);

  const cleanup = between(page, 'function limpiar_progreso_almacenamiento_digitalizado()', 'function InitializeRequest(sender, args)');
  assert.match(cleanup, /style\.display\s*=\s*["']none["']/);
  assert.match(cleanup, /style\.position\s*=\s*estado_progreso_almacenamiento_digitalizado\.position/);
  assert.match(cleanup, /style\.transform\s*=\s*estado_progreso_almacenamiento_digitalizado\.transform/);
  assert.match(cleanup, /estado_progreso_almacenamiento_digitalizado\s*=\s*null/);
});

test('DOC-89 mantiene una persistencia y una proyeccion incremental para ButtonAlmacenar', () => {
  assert.equal((storeHandler.match(/UploadSaveFileScan\s*\(/g) || []).length, 1);
  assert.match(storeHandler, /If Result <> "YES" Then[\s\S]*?Exit Sub/);
  assert.equal((storeHandler.match(/Hidden_result_load_\.Value\s*=\s*"YES"/g) || []).length, 1);

  const projection = between(
    checkStatus,
    'if (elment_postbak.id == "ButtonAlmacenar")',
    '//Boton recupera la tarea',
  );
  assert.equal((projection.match(/insert_row_documento_relacionado\s*\(/g) || []).length, 1);
  assert.match(projection, /Hidden_result_load_["']\)\.value\s*=\s*["']["']/);
  assert.doesNotMatch(projection, /DataBind|location\.reload|setTimeout|ButtonAlmacenar["']\)\.click/i);
});

test('DOC-89 preserva las cinco continuaciones del escaner compartido', () => {
  const expectations = [
    [/Hidden21["']\)\.value == ["']1["'][\s\S]*?ButtonAlmacenar["']\)\.click\(\)/, 'TRAMITE'],
    [/Hidden21["']\)\.value == ["']2["'][\s\S]*?Button_añade_documento["']\)\.click\(\)/, 'Añadir a documento'],
    [/Hidden21["']\)\.value == ["']3["'][\s\S]*?save_document_scan["']\)\.click\(\)/, 'MIGRACION'],
    [/Hidden21["']\)\.value == ["']4["'][\s\S]*?save_document_scan["']\)\.click\(\)/, 'TRAMITE SIMPLE'],
    [/Hidden21["']\)\.value == ["']5["'][\s\S]*?Button_save_replace_dig["']\)\.click\(\)/, 'REMPLAZAVERSION'],
  ];
  for (const [pattern, context] of expectations) {
    assert.match(scannerContinuation, pattern, `Se altero la continuacion ${context}`);
  }
});

test('DOC-89 caracteriza el transporte Dynamsoft y evita cambiar estilos globales', () => {
  assert.match(transport, /function Gurdar_documento_htpp_server\s*\(/);
  assert.match(uploadSuccess, /\/\/activa_document_save\(\)/);
  assert.match(uploadFailure, /errorCode != ["']-2003["'][\s\S]*?activa_document_save\(\)/);
  assert.match(legacyStyles, /\.overlay_\s*\{[\s\S]*?width:\s*100%[\s\S]*?height:\s*100%/);
  assert.match(modernStyles, /#progres_bar\.ctw-loading-indicator\s*\{[\s\S]*?background:\s*#fff/);
  assert.match(page, /id="progres_bar"[\s\S]{0,180}ctw-loading-indicator[\s\S]{0,180}role="status"[\s\S]{0,120}aria-live="polite"/);
});

test('DOC-89 registra E2E mutante sobre la plataforma Workflow existente', () => {
  const scenario = resolveScenario('scanner-link-overlay-execution');
  assert.equal(scenario.doc, 'doc89');
  assert.equal(scenario.stage, 'execution');
  assert.equal(scenario.resource.mutating, true);
  assert.equal(scenario.resource.profileField, 'taskId');
  assert.deepEqual(scenario.requiredAuthorizations, ['environment', 'account']);
  assert.deepEqual(scenario.controls, ['workflow-assignment-state']);
  assert.equal(scenario.controlExpectations['workflow-assignment-state'], 'unchanged');
  assert.ok(scenario.expectations.includes('scanner-link-overlay-ui'));
  assert.equal(profile.scenarioId, scenario.id);
  assert.equal(profile.odbcDsn, 'workflowconta');
});

test('DOC-89 E2E usa sesión común, navegador visible y evidencia saneada', () => {
  const scannerInspection = between(platformRunner, 'async function inspectScannerLinkOverlayUi', 'async function inspectProductionDocumentUploadPreviewUi');
  assert.match(platformRunner, /createAuthenticatedWorkflowSession/);
  assert.match(platformRunner, /headless:\s*!plan\.scenario\.expectations[\s\S]*?scanner-link-overlay-ui/);
  assert.match(platformRunner, /inspectScannerLinkOverlayUi/);
  assert.match(platformRunner, /async function selectScannerLinkTask[\s\S]*?DOC89_TASK_SELECTION_READY[\s\S]*?Hidden_id_tarea_selecionada[\s\S]*?HiddenIdFlujo[\s\S]*?ENLASE/);
  assert.match(platformRunner, /attempt <= 3[\s\S]*?DOC89_TASK_SELECTION_RETRY[\s\S]*?SCANNER_LINK_E2E_TASK_CONTEXT_REJECTED/);
  assert.match(platformRunner, /scanner-link-overlay-ui["']\)\) return context/);
  assert.match(scannerInspection, /await selectScannerLinkTask\(page, plan\)/);
  assert.match(platformRunner, /ButtonAlmacenar/);
  assert.match(platformRunner, /initializeCount !== 1[\s\S]*?buttonClicks !== 1[\s\S]*?postbackCount !== 1/);
  assert.match(platformRunner, /after\.rows !== before\.rows \+ 1/);
  assert.match(platformRunner, /SCANNER_LINK_E2E_PROGRESS_NOT_COMPACT/);
  assert.match(scannerInspection, /locator\('#IframeDitaliza_'\)\.elementHandle\(\)[\s\S]*?contentFrame\(\)/);
  assert.doesNotMatch(scannerInspection, /page\.frames\(\)\.filter/);
  assert.match(platformRunner, /GetWebTwain\?\.\('dwtcontrolContainer'\)/);
  assert.match(platformRunner, /scannerState\.bufferCount < 1 && scannerState\.pagerCount < 1/);
  assert.match(platformRunner, /SCANNER_LINK_E2E_SCANNER_FRAME_UNAVAILABLE/);
  assert.match(platformRunner, /SCANNER_LINK_E2E_ACCEPT_NOT_OBSERVED/);
  assert.match(scannerInspection, /no seleccione el nodo nuevo ni cambie el visor antes de escribir SI/);
  assert.match(scannerInspection, /after\.task !== before\.task[\s\S]*?SCANNER_LINK_E2E_TASK_NOT_PRESERVED/);
  assert.match(scannerInspection, /after\.selection !== before\.selection[\s\S]*?SCANNER_LINK_E2E_SELECTION_NOT_PRESERVED/);
  assert.match(scannerInspection, /after\.viewer\.visible !== before\.viewer\.visible[\s\S]*?SCANNER_LINK_E2E_VIEWER_VISIBILITY_NOT_PRESERVED/);
  assert.match(scannerInspection, /before\.viewer\.visible && after\.viewer\.src !== before\.viewer\.src[\s\S]*?SCANNER_LINK_E2E_VIEWER_NOT_PRESERVED/);
  assert.doesNotMatch(scannerInspection, /SCANNER_LINK_E2E_CONTEXT_NOT_PRESERVED/);
  assert.match(platformCore, /SCANNER_LINK_E2E/);
  assert.doesNotMatch(scannerInspection, /WorkflowCentroTrabajoModern(?:Active|Users|Groups)/);
});

'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const runner = require('../scripts/run-doc105-pdf-cross-module-interactive.cjs');
const workflowPerformanceRunner = require('../scripts/run-workflow-pdf-viewer-performance-interactive.cjs');
const runnerSource = fs.readFileSync(path.join(__dirname, '../scripts/run-doc105-pdf-cross-module-interactive.cjs'), 'utf8');

test('diagnóstico horizontal exige Producción exclusiva sin mutaciones', () => {
  const profile = validProfile();
  assert.throws(() => runner.validateLayoutDiagnostic(profile), /DOC105_LAYOUT_REQUIRES_READONLY_PRODUCTION/);
  for (const consumer of profile.consumers) consumer.enabled = consumer.kind === 'production';
  assert.doesNotThrow(() => runner.validateLayoutDiagnostic(profile));
  profile.mutation.enabled = true;
  assert.throws(() => runner.validateLayoutDiagnostic(profile), /DOC105_LAYOUT_REQUIRES_READONLY_PRODUCTION/);
  const source = runnerSource.slice(runnerSource.indexOf('async function runLayoutDiagnostic'), runnerSource.indexOf('async function main'));
  assert.match(source, /createAuthenticatedWorkflowSession/);
  assert.match(source, /openProductionConsumer/);
  assert.match(source, /service_anotate_pdf_image\/i, route => route.abort\(\)/);
  assert.doesNotMatch(source, /runAuthorizedGraphSave|storageState|screenshot/);
});

test('medición horizontal conserva scrollLeft y detecta recorte de ancestro', async () => {
  const { chromium } = require('playwright');
  const browser = await chromium.launch({ channel: 'chrome', headless: true });
  try {
    const page = await browser.newPage();
    await page.route('**/*', route => route.abort());
    await page.setContent('<form><div id="modal" style="height:400px;overflow:hidden"><iframe style="height:600px;width:600px"></iframe></div></form>');
    const frame = page.frames()[1];
    await frame.setContent('<div id="pdf-container_" style="width:500px;height:550px;overflow:auto"><div style="width:1400px;height:800px"></div></div>');
    await frame.evaluate(() => {
      window.pdfViewer = { getZoom: () => 2 };
      document.querySelector('#pdf-container_').scrollLeft = 20;
    });
    const clipped = await runner.measureHorizontalLayout(frame);
    assert.equal(clipped.rightReachable, true);
    assert.equal(clipped.bottomVisible, false);
    assert.equal(clipped.ancestorBottomVisible, false);
    assert.equal(await frame.locator('#pdf-container_').evaluate(el => el.scrollLeft), 20);
    await page.locator('#modal').evaluate(el => el.style.height = '650px');
    assert.equal((await runner.measureHorizontalLayout(frame)).bottomVisible, true);
    await page.locator('#modal').evaluate(el => {
      const form = el.parentElement;
      form.style.cssText = 'height:0;overflow:hidden';
      el.style.position = 'fixed';
      el.style.top = '0';
    });
    // Un ancestro estático de altura cero no recorta este modal fixed.
    assert.equal((await runner.measureHorizontalLayout(frame)).bottomVisible, true);
    assert.equal(Object.values(clipped).every(value => typeof value === 'number' || typeof value === 'boolean'), true);
  } finally { await browser.close(); }
});

function validProfile() {
  return {
    baseUrl: 'https://localhost/GestionDocumental-Docuarchi.net/',
    module: 'GESTOR',
    timeoutMilliseconds: 120000,
    headless: true,
    browser: { channel: 'chrome' },
    ignoreHttpsErrors: false,
    iterations: 3,
    scrollSteps: 12,
    networkProfiles: [
      { name: 'remote-10', downloadMbps: 10, latencyMilliseconds: 100, firstPageBudget: { maxMilliseconds: 60000, maxBytesBeforeCanvas: 33554432, maxDocumentFractionBeforeCanvas: 0.5, maxRequestsBeforeCanvas: 64 } },
      { name: 'remote-20', downloadMbps: 20, latencyMilliseconds: 80, firstPageBudget: { maxMilliseconds: 35000, maxBytesBeforeCanvas: 33554432, maxDocumentFractionBeforeCanvas: 0.5, maxRequestsBeforeCanvas: 64 } },
      { name: 'remote-50', downloadMbps: 50, latencyMilliseconds: 60, firstPageBudget: { maxMilliseconds: 20000, maxBytesBeforeCanvas: 33554432, maxDocumentFractionBeforeCanvas: 0.5, maxRequestsBeforeCanvas: 64 } },
      { name: 'remote-100', downloadMbps: 100, latencyMilliseconds: 40, firstPageBudget: { maxMilliseconds: 15000, maxBytesBeforeCanvas: 33554432, maxDocumentFractionBeforeCanvas: 0.5, maxRequestsBeforeCanvas: 64 } }
    ],
    cacheModes: ['cold', 'warm'],
    consumers: [
      { kind: 'workflow', enabled: true, taskId: 1, documentOrdinal: 1 },
      { kind: 'correspondence', enabled: false, entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', menuSelector: '#CR-GT-01_card_boton', containerFrameSelector: '#ContentPlacenter_ifrm_ds_', contextOrdinal: 1, rowSelector: '[tip_event="documento_solic_tramite"]', openSelector: '[tip_event="vis_doc_selecion_wf"]:has(.fa-file-pdf)', viewerFrameSelector: '#ifrm_visor_' },
      { kind: 'production', enabled: false, entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', menuSelector: '#GD-GD-14_card_boton', containerFrameSelector: '#ContentPlacenter_ifrm_ds_', contextSelector: '#TreeViewArchivo a', contextOrdinal: 1, rowSelector: '#data_grid tr[id]', openSelector: 'a[title="Visualiza archivo"]', viewerFrameSelector: '#Iframe_visor_externo_da_' },
      { kind: 'approvalExternal', enabled: false, entryPath: 'Gestion/WebFormGaconsultaclasificacion.aspx', contextSelector: '#TreeViewEstructura a[href*="__doPostBack"]', contextOrdinal: 1, rowSelector: '[tip_event="ver_doc_col"][title="Con documentos"]', openSelector: '[tip_event="ver_doc"]', containerFrameSelector: '#Iframe_visor_externo__', viewerFrameSelector: '#ifrm_visor_' },
      { kind: 'simpleHandler', enabled: false, simpleClass: 'workflowOtherFormats', taskId: 1, documentOrdinal: 1, entryPath: 'workflow/WebFormVisor_Otros_Formatos.aspx', viewerFrameSelector: '#ifrm_visor_' }
    ],
    mutation: { enabled: false, consumer: 'workflow' }
  };
}

test('Producción reserva cabecera del modal y mantiene barra horizontal dentro del viewport', async () => {
  const { chromium } = require('playwright');
  const browser = await chromium.launch({ channel: 'chrome', headless: true });
  try {
    const page = await browser.newPage({ viewport: { width: 1500, height: 880 } });
    await page.route('**/*', route => route.abort());
    const root = path.resolve(__dirname, '../../..');
    const markup = fs.readFileSync(path.join(root, 'Gestion/WebFormProducionDocumental.aspx'), 'utf8');
    const fragment = markup.slice(markup.indexOf('<!--Popup visor externo-->'), markup.indexOf('<!--Popup radica interno-->'))
      .replace(/<asp:ModalPopupExtender[\s\S]*?<\/asp:ModalPopupExtender>/g, '')
      .replace(/<asp:Button\b[\s\S]*?\/>/g, '')
      .replace(/<\/?ContentTemplate>/g, '')
      .replace(/asp:(Panel|UpdatePanel)/g, 'div');
    const source = fs.readFileSync(path.join(root, 'js/gestion/WebFormProducionDocumental.js'), 'utf8');
    const sizing = source.slice(source.indexOf('    function auto_zise_popup_visor_externo()'), source.indexOf('    function auto_zise_popup_radicador()'));
    const pageStyles = markup.match(/<style type="text\/css">([\s\S]*?)<\/style>/)[1];
    await page.setContent(`<style>${pageStyles} body{margin:0} #Panel_visor_externo{position:absolute;top:2px;width:98%}
      #Cabecerapendiente_visor_externo{height:35px;flex-shrink:0}
      .modal_content_back{border-top:1px solid #ccc}</style>${fragment}`);
    await page.evaluate(() => {
      window.$ = selector => ({ css: (key, value) => { document.querySelector(selector).style[key] = value; } });
      window.alert = () => { throw new Error('SIZING_ERROR'); };
    });
    await page.addScriptTag({ content: sizing });
    // Inicialización con modal oculto, como ready() en el módulo real.
    await page.evaluate(() => {
      auto_zise_popup_visor_externo();
      document.getElementById('Panel_visor_externo').style.display = 'block';
    });
    const frame = await (await page.locator('#Iframe_visor_externo_da_').elementHandle()).contentFrame();
    await frame.setContent(`<style>body{margin:0}</style><div style="height:50px">Toolbar</div>
      <div id="pdf-container_" style="height:calc(100vh - 52px);overflow:auto"><div style="width:2400px;height:1800px"></div></div>`);
    await frame.evaluate(() => { window.pdfViewer = { getZoom: () => 3 }; });
    for (const height of [880, 600, 1000]) {
      await page.setViewportSize({ width: 1500, height });
      await page.evaluate(() => auto_zise_popup_visor_externo());
      const measured = await runner.measureHorizontalLayout(frame);
      assert.equal(measured.rightReachable, true);
      assert.equal(measured.bottomVisible, true);
      const header = await page.locator('#Cabecerapendiente_visor_externo').boundingBox();
      assert.equal(header.y, 2);
    }
  } finally { await browser.close(); }
});

function withProfile(input, callback) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc105-policy-'));
  const target = path.join(directory, 'profile.json');
  try {
    fs.writeFileSync(target, JSON.stringify(input), 'utf8');
    return callback(target);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
}

test('referencias legacy de preview Gestión se reemplazan antes de renderizar el iframe', () => {
  for (const relative of ['radicador/WebFormCorespondenciaporenviar.aspx.vb', 'radicador/WebFormArchivaenviocorrespo.aspx.vb']) {
    const source = fs.readFileSync(path.resolve(__dirname, '../../..', relative), 'utf8');
    const assignments = source.match(/Me\.IframeVisor_\.Attributes\("src"\) = "\.\.\/gestion\/WebFormGaVisorDocumentoExterno\.aspx"/g) || [];
    assert.equal(assignments.length, 1);
    assert.match(source, /Me\.IframeVisor_\.Attributes\("src"\) = "\.\.\/gestion\/WebFormGaVisorDocumentoExterno\.aspx"\s+Me\.IframeVisor_\.Attributes\("src"\) = "\.\.\/gestion\/WebFormGaVisorClasificacion\.aspx"/);
  }
});

test('Migración espera navegación del iframe y admite un único gabinete disponible', () => {
  const source = runnerSource.slice(runnerSource.indexOf('async function openMigrationConsumer'), runnerSource.indexOf('function loadIncomingVersionsSelection'));
  assert.ok(source.indexOf('DOC105_MIGRATION_FRAME_NAVIGATION_FAILED') < source.indexOf('resolveContainerFrame'));
  assert.match(source, /DOC105_MIGRATION_MENU_ACTION_UNAVAILABLE/);
  assert.match(source, /DOC105_MIGRATION_CABINET_LIST_UNAVAILABLE/);
  assert.match(source, /\.some\(option => option\.value/);
  assert.doesNotMatch(source, /option'\)\.length > 1/);
});

test('Migración sigue menú lateral y búsqueda de campo del video sin confundir original y convertido', () => {
  const source = runnerSource.slice(runnerSource.indexOf('async function openMigrationConsumer'), runnerSource.indexOf('function loadIncomingVersionsSelection'));
  assert.match(source, /#GD-PR-00 > a/);
  assert.match(source, /#GD-MR-15 > a/);
  assert.match(source, /#GD-MR-99 > a/);
  assert.doesNotMatch(source, /GD-MR-99_card_boton|Button_search_gabinete_general/);
  assert.match(source, /atrib_campo_beetwen="0"/);
  assert.match(source, /DOC105_MIGRATION_SOURCE_PREVIEW/);
  const input = { cabinetName: 'TEST', searchField: 'MATRICULA', searchValue: '123', documentPrefix: 'RECIBO', view: 'source' };
  const profile = withProfile(input, target => runner.loadMigrationSelection(validProfile(), target));
  assert.equal(profile.consumers.find(c => c.enabled).migrationView, 'source');
  assert.throws(() => withProfile({ ...input, view: 'write' }, target => runner.loadMigrationSelection(validProfile(), target)), /SELECTION_INVALID/);
});

test('Migración exige perfil cerrado y habilita solo lectura de su consumidor', () => {
  const input = { cabinetName: 'TEST', searchField: 'MATRICULA', searchValue: '123', documentPrefix: 'RECIBO 1' };
  const profile = withProfile(input, target => runner.loadMigrationSelection(validProfile(), target));
  assert.equal(profile.mutation.enabled, false);
  assert.deepEqual(profile.consumers.filter(c => c.enabled).map(c => c.simpleClass), ['migration']);
  assert.throws(() => withProfile({ ...input, password: 'forbidden' }, target => runner.loadMigrationSelection(validProfile(), target)), /SELECTION_INVALID/);
});

test('Migración exige matrícula exacta y prefijo documental único', () => {
  const previous = global.document;
  let values = [['123', 'RECIBO 1 detalle'], ['1234', 'RECIBO 1 detalle']];
  global.document = { querySelector: () => ({ querySelectorAll: selector => selector.startsWith('thead')
    ? ['MATRICULA', 'DOCUMENTO'].map(textContent => ({ textContent, getAttribute: () => textContent }))
    : values.map(row => ({ children: row.map(textContent => ({ textContent })), querySelector: () => ({}) })) }) };
  const selection = { searchField: 'MATRICULA', searchValue: '123', documentPrefix: 'RECIBO 1' };
  try {
    assert.deepEqual(runner.findMigrationRow(selection), { count: 1, index: 0 });
    assert.equal(runner.findMigrationRow({ ...selection, searchValue: '12' }), null);
    values.push(['123', 'RECIBO 1 otro']);
    assert.deepEqual(runner.findMigrationRow(selection), { count: 2, index: -1 });
  } finally { if (previous === undefined) delete global.document; else global.document = previous; }
});

test('Migración abre destino existente sin convertir y no atribuye respuesta del original al destino', () => {
  const source = runnerSource.slice(runnerSource.indexOf('async function openMigrationConsumer'), runnerSource.indexOf('function loadIncomingVersionsSelection'));
  assert.match(source, /MIGRATION_EXISTING_TARGET_UNAVAILABLE/);
  assert.doesNotMatch(source, /Button_migra|Button_remplaza|Service_REST_migra/);
  assert.match(runnerSource, /exchanges\.filter\(e => e\.requestUrl === opened\.handlerUrl\)/);
  assert.doesNotMatch(JSON.stringify(runner.sanitizeEvidence({ rows: [{ requestUrl: 'https://private.invalid/private' }] })), /private/);
});

test('Versiones selecciona únicamente el consumidor autorizado sin mutación', () => {
  const selection = { templateName: 'TEST', radicado: '123', documentTitle: 'Documento' };
  const profile = withProfile(selection, target => runner.loadIncomingVersionsSelection(validProfile(), target));
  assert.equal(profile.mutation.enabled, false);
  assert.deepEqual(profile.consumers.filter(c => c.enabled).map(c => c.simpleClass), ['versions']);
  for (const input of [{ ...selection, password: 'forbidden' }, { ...selection, radicado: '12*' }]) {
    assert.throws(() => withProfile(input, target => runner.loadIncomingVersionsSelection(validProfile(), target)), /VERSIONS_SELECTION_INVALID/);
  }
});

test('plantilla entrante exige nombre exacto y único, no el primer resultado', () => {
  const item = { nodo_plantilla_radicado: 'yes', tipo_plantilla: 'RADICACION ENTRANTE', Text_node: 'TEST', value_node: 'menu-1' };
  assert.equal(runner.resolveIncomingTemplate([item], ' test '), 'menu-1');
  for (const items of [[], [item, item], [{ ...item, tipo_plantilla: 'RADICACION SALIENTE' }]]) {
    assert.throws(() => runner.resolveIncomingTemplate(items, 'TEST'), /TEMPLATE_NOT_UNIQUE/);
  }
});

test('documento de versiones tolera tildes pero rechaza prefijos y duplicados', () => {
  const previous = global.document;
  let titles = ['Documento de Identificación', 'Otro'];
  global.document = { querySelectorAll: () => titles.map(title => ({ getAttribute: () => `a|b|c|d|${title}` })) };
  try {
    assert.deepEqual(runner.findIncomingVersionDocument('Documento de Identificacion'), { count: 1, index: 0 });
    assert.deepEqual(runner.findIncomingVersionDocument('Documento'), { count: 0, index: -1 });
    titles.push('Documento de Identificación');
    assert.deepEqual(runner.findIncomingVersionDocument('Documento de Identificacion'), { count: 2, index: -1 });
  } finally { if (previous === undefined) delete global.document; else global.document = previous; }
});

test('Versiones no asigna pendientes ni restaura, adjunta o elimina versiones', () => {
  const source = runnerSource.slice(runnerSource.indexOf('async function openIncomingVersionsConsumer'), runnerSource.indexOf('function loadDocuarchiSelection'));
  assert.match(source, /DOC105_VERSIONS_ASSIGNED_RECORD_MISMATCH/);
  assert.match(source, /active_version_document_view_document\.nav-link/);
  assert.doesNotMatch(source, /a_s_r_p_333|Button_tool_asigna|restore_version|delete_version|Button_activa_adjunta/);
});

test('selección Docuarchi habilita solo preview de lectura y rechaza campos desconocidos', () => {
  const selection = { cabinetName: 'TEST', searchField: 'CONSECUTIVO', searchValue: '123' };
  const profile = withProfile(selection, target => runner.loadDocuarchiSelection(validProfile(), target));
  assert.equal(profile.mutation.enabled, false);
  assert.deepEqual(profile.consumers.filter(c => c.enabled).map(c => c.simpleClass), ['docuarchiExternal']);
  assert.throws(() => withProfile({ ...selection, password: 'forbidden' }, target =>
    runner.loadDocuarchiSelection(validProfile(), target)), /DOC105_DOCUARCHI_SELECTION_INVALID/);
});

test('gabinete se resuelve por valor, tolera espacios y mayúsculas pero no otro nombre', () => {
  const options = [{ label: '  Cabinet  A ', value: '7' }, { label: 'Cabinet B', value: '8' }];
  assert.equal(runner.resolveDocuarchiCabinetValue(options, 'cabinet a'), '7');
  assert.throws(() => runner.resolveDocuarchiCabinetValue(options, 'Cabinett A'), /CABINET_NOT_FOUND/);
  assert.throws(() => runner.resolveDocuarchiCabinetValue([...options, { label: 'CABINET A', value: '9' }], 'Cabinet A'), /CABINET_NOT_UNIQUE/);
});

test('selección de descarga es explícita y no habilita otras clases ni mutaciones', () => {
  const selection = { cabinetName: 'TEST', searchField: 'CONSECUTIVO', searchValue: '123', simpleClass: 'docuarchiDownload' };
  const profile = withProfile(selection, target => runner.loadDocuarchiSelection(validProfile(), target));
  assert.deepEqual(profile.consumers.filter(c => c.enabled).map(c => c.simpleClass), ['docuarchiDownload']);
  assert.equal(profile.mutation.enabled, false);
  for (const simpleClass of ['versions', '', null]) {
    assert.throws(() => withProfile({ ...selection, simpleClass }, target =>
      runner.loadDocuarchiSelection(validProfile(), target)), /DOC105_DOCUARCHI_SELECTION_INVALID/);
  }
});

test('descarga exige HTTP 200 único y longitud igual a bytes completados', () => {
  const exchanges = [{ status: 200, contentLength: 12 }];
  assert.equal(runner.downloadTransportPassed(12, exchanges), true);
  for (const [bytes, responses] of [[0, exchanges], [11, exchanges], [12, []],
    [12, [...exchanges, ...exchanges]], [12, [{ status: 403, contentLength: 12 }]],
    [12, [{ status: 200, contentLength: 0 }]]]) {
    assert.equal(runner.downloadTransportPassed(bytes, responses), false);
  }
});

test('descarga completa cuenta bytes sin conservar contenido y elimina el temporal propio', async () => {
  const { Readable } = require('node:stream');
  let deleted = 0;
  const bytes = await runner.verifyCompletedDownload({
    failure: async () => null,
    createReadStream: async () => Readable.from([Buffer.alloc(5), Buffer.alloc(7)]),
    delete: async () => { deleted++; },
    cancel: async () => assert.fail('not cancelled')
  }, 1000);
  assert.equal(bytes, 12);
  assert.equal(deleted, 1);
});

test('descarga fallida, vacía o vencida no aprueba y limpia su temporal', async () => {
  const { Readable } = require('node:stream');
  for (const [failure, code] of [[async () => 'sensitive detail', 'FAILED'],
    [async () => null, 'EMPTY'], [() => new Promise(() => {}), 'TIMEOUT']]) {
    let deleted = 0;
    let cancelled = 0;
    await assert.rejects(runner.verifyCompletedDownload({
      failure, createReadStream: async () => Readable.from([]),
      cancel: async () => { cancelled++; }, delete: async () => { deleted++; }
    }, 20), new RegExp(`DOC105_DOWNLOAD_${code}`));
    assert.equal(cancelled, 1);
    assert.equal(deleted, 1);
  }
});

test('búsqueda Docuarchi exige columna exacta y detecta registros ambiguos', () => {
  const oldDocument = global.document;
  let values = [['123', '999'], ['1234', '123']];
  global.document = { querySelector: () => ({ querySelectorAll: selector => selector.startsWith('thead')
    ? ['CONSECUTIVO', 'OTRO'].map(field => ({ getAttribute: () => field }))
    : values.map(row => ({ children: row.map(textContent => ({ textContent })), querySelector: () => ({}) })) }) };
  try {
    assert.deepEqual(runner.findExactDocuarchiRow({ field: 'CONSECUTIVO', value: '123' }), { count: 1, index: 0 });
    assert.equal(runner.findExactDocuarchiRow({ field: 'CONSECUTIVO', value: '12' }), null);
    assert.equal(runner.findExactDocuarchiRow({ field: 'MISSING', value: '123' }), null);
    values.push(['123', '888']);
    assert.deepEqual(runner.findExactDocuarchiRow({ field: 'CONSECUTIVO', value: '123' }), { count: 2, index: -1 });
  } finally { if (oldDocument === undefined) delete global.document; else global.document = oldDocument; }
});

test('perfil DOC-105 exige matriz cerrada, cuatro redes y caché fría/caliente', () => {
  const profile = withProfile(validProfile(), runner.loadProfile);
  assert.deepEqual(profile.networkProfiles.map(item => item.downloadMbps), [10, 20, 50, 100]);
  assert.deepEqual(profile.cacheModes, ['cold', 'warm']);
  assert.deepEqual(profile.consumers.map(item => item.kind), runner.CONSUMER_KINDS);
  assert.equal(profile.consumers[0].taskId, 1);
});

test('bootstrap reutiliza el perfil Workflow validado sin copiarlo a otro artefacto', () => {
  const target = path.resolve(__dirname, '../profiles/workflow-pdf-viewer-performance.profile.example.json');
  const profile = runner.loadWorkflowBootstrapProfile(target);
  assert.equal(profile.consumers.length, 5);
  assert.deepEqual(profile.consumers.filter(item => item.enabled).map(item => item.kind), ['workflow']);
  assert.deepEqual(profile.consumers.filter(item => !item.enabled).map(item => item.kind), [
    'correspondence', 'production', 'approvalExternal', 'simpleHandler'
  ]);
  assert.equal(profile.consumers[0].kind, 'workflow');
  const simple = profile.consumers.find(item => item.kind === 'simpleHandler');
  assert.equal(simple.simpleClass, 'workflowOtherFormats');
  assert.equal(simple.taskId, profile.consumers[0].taskId);
  assert.equal(profile.mutation.enabled, false);
  assert.deepEqual(profile.networkProfiles.map(item => item.downloadMbps), [10, 20, 50, 100]);
});

test('modo diagnóstico habilita en memoria los cinco consumidores inventariados sin mutación', () => {
  const profile = runner.enableInventoriedConsumers(validProfile());
  assert.deepEqual(profile.consumers.map(item => item.enabled), [true, true, true, true, true]);
  assert.equal(profile.timeoutMilliseconds, 30000);
  assert.equal(profile.mutation.enabled, false);
});

test('selección operativa habilita un solo consumidor por cuenta', () => {
  const profile = runner.selectConsumer(validProfile(), 'correspondence');
  assert.deepEqual(profile.consumers.filter(item => item.enabled).map(item => item.kind), ['correspondence']);
  assert.throws(() => runner.selectConsumer(validProfile(), 'unknown'), /DOC105_CONSUMER_ARGUMENT_INVALID/);
  const mutating = validProfile();
  mutating.mutation.enabled = true;
  mutating.mutation.profile = 'profiles/doc103.runtime.json';
  assert.throws(() => runner.selectConsumer(mutating, 'production'), /DOC105_MUTATION_CONSUMER_FORBIDDEN/);
});

test('módulo de autenticación puede separarse del perfil Workflow sin aceptar valores inseguros', () => {
  const profile = runner.selectAuthenticationModule(validProfile(), 'GESTOR');
  assert.equal(profile.module, 'GESTOR');
  assert.throws(() => runner.selectAuthenticationModule(validProfile(), ''), /DOC105_MODULE_ARGUMENT_INVALID/);
  assert.throws(() => runner.selectAuthenticationModule(validProfile(), 'GESTOR\nOTRO'), /DOC105_MODULE_ARGUMENT_INVALID/);
});

test('perfil rechaza campos desconocidos antes de abrir navegador', () => {
  const input = validProfile();
  input.password = 'never-allowed';
  assert.throws(() => withProfile(input, runner.loadProfile), /DOC105_PROFILE_FIELD_NOT_ALLOWED/);
});

test('perfil rechaza matrices de red incompletas o repetidas', () => {
  const input = validProfile();
  input.networkProfiles[3].downloadMbps = 50;
  assert.throws(() => withProfile(input, runner.loadProfile), /DOC105_NETWORK_MATRIX_INVALID/);
});

test('perfil exige presupuestos cerrados de primera página', () => {
  const missing = validProfile();
  delete missing.networkProfiles[0].firstPageBudget;
  assert.throws(() => withProfile(missing, runner.loadProfile), /DOC105_FIRST_PAGE_BUDGET_REQUIRED/);
  const invalid = validProfile();
  invalid.networkProfiles[0].firstPageBudget.maxDocumentFractionBeforeCanvas = 1;
  assert.throws(() => withProfile(invalid, runner.loadProfile), /DOC105_FIRST_PAGE_FRACTION_BUDGET_INVALID/);
});

test('perfil exige las cinco clases de consumidor una sola vez', () => {
  const input = validProfile();
  input.consumers[4].kind = 'workflow';
  assert.throws(() => withProfile(input, runner.loadProfile), /DOC105_CONSUMER_MATRIX_INVALID/);
});

test('perfil no permite mutación fuera de Workflow ni con Workflow deshabilitado', () => {
  const wrongConsumer = validProfile();
  wrongConsumer.mutation.consumer = 'production';
  assert.throws(() => withProfile(wrongConsumer, runner.loadProfile), /DOC105_MUTATION_INVALID/);
  const disabled = validProfile();
  disabled.mutation.enabled = true;
  disabled.consumers[0].enabled = false;
  disabled.consumers[1].enabled = true;
  assert.throws(() => withProfile(disabled, runner.loadProfile), /DOC105_MUTATION_WORKFLOW_DISABLED/);
  const missingProfile = validProfile();
  missingProfile.mutation.enabled = true;
  assert.throws(() => withProfile(missingProfile, runner.loadProfile), /DOC105_MUTATION_PROFILE_REQUIRED/);
});

test('registro de adaptadores declara capacidades sin habilitar mutaciones laterales', () => {
  assert.deepEqual(Object.keys(runner.adapterRegistry), runner.CONSUMER_KINDS);
  assert.equal(runner.adapterRegistry.workflow.mutation, true);
  for (const kind of runner.CONSUMER_KINDS.filter(value => value !== 'workflow')) {
    assert.equal(runner.adapterRegistry[kind].mutation, false);
  }
  assert.equal(runner.adapterRegistry.correspondence.viewer, false);
  assert.equal(runner.adapterRegistry.correspondence.scrollZoom, false);
  assert.equal(runner.adapterRegistry.correspondence.entry, 'correspondence-readonly-handler');
  assert.equal(runner.adapterRegistry.simpleHandler.scrollZoom, false);
});

test('cada visor no Workflow exige su contrato de navegación real', () => {
  for (const kind of ['correspondence', 'production', 'approvalExternal']) {
    const input = validProfile();
    const consumer = input.consumers.find(item => item.kind === kind);
    consumer.enabled = true;
    delete consumer.rowSelector;
    assert.throws(() => withProfile(input, runner.loadProfile), /DOC105_NAVIGATION_CONTRACT_REQUIRED/);
  }
  const approval = validProfile();
  const approvalConsumer = approval.consumers.find(item => item.kind === 'approvalExternal');
  approvalConsumer.enabled = true;
  delete approvalConsumer.containerFrameSelector;
  assert.throws(() => withProfile(approval, runner.loadProfile), /DOC105_APPROVAL_ENTRY_REQUIRED/);
  const approvalWithoutTree = validProfile();
  const approvalWithoutTreeConsumer = approvalWithoutTree.consumers.find(item => item.kind === 'approvalExternal');
  approvalWithoutTreeConsumer.enabled = true;
  delete approvalWithoutTreeConsumer.contextSelector;
  assert.throws(() => withProfile(approvalWithoutTree, runner.loadProfile), /DOC105_APPROVAL_ENTRY_REQUIRED/);
  const correspondence = validProfile();
  const correspondenceConsumer = correspondence.consumers.find(item => item.kind === 'correspondence');
  correspondenceConsumer.enabled = true;
  delete correspondenceConsumer.viewerFrameSelector;
  assert.throws(() => withProfile(correspondence, runner.loadProfile), /DOC105_CORRESPONDENCE_ENTRY_REQUIRED/);
  const production = validProfile();
  const productionConsumer = production.consumers.find(item => item.kind === 'production');
  productionConsumer.enabled = true;
  delete productionConsumer.contextSelector;
  assert.throws(() => withProfile(production, runner.loadProfile), /DOC105_PRODUCTION_ENTRY_REQUIRED/);
  const simpleHandler = validProfile();
  const simpleConsumer = simpleHandler.consumers.find(item => item.kind === 'simpleHandler');
  simpleConsumer.enabled = true;
  delete simpleConsumer.viewerFrameSelector;
  assert.throws(() => withProfile(simpleHandler, runner.loadProfile), /DOC105_SIMPLE_HANDLER_FRAME_REQUIRED/);
  const simpleWithoutClass = validProfile();
  const simpleWithoutClassConsumer = simpleWithoutClass.consumers.find(item => item.kind === 'simpleHandler');
  simpleWithoutClassConsumer.enabled = true;
  delete simpleWithoutClassConsumer.simpleClass;
  assert.throws(() => withProfile(simpleWithoutClass, runner.loadProfile), /DOC105_SIMPLE_HANDLER_CLASS_REQUIRED/);
});

function navigationPage(frame) {
  const calls = [];
  return {
    calls,
    goto: async value => calls.push(['goto', value]),
    waitForTimeout: async () => {},
    locator: selector => ({
      first() { return this; },
      nth(index) { return { click: async () => calls.push(['click', selector, index]) }; },
      waitFor: async () => calls.push(['wait', selector]),
      count: async () => 2,
      inputValue: async () => '1',
      evaluateAll: async (_callback, value) => { calls.push(['click-text', selector, value]); return true; },
      evaluate: async () => false,
      getAttribute: async () => '../pdfjs/pdf_anotate_view/index.html',
      elementHandle: async () => ({ contentFrame: async () => frame })
    }),
    evaluate: async () => 'pdfjs',
    waitForFunction: async (_callback, value) => calls.push(['waitForFunction', value])
  };
}

test('adaptadores recorren Correspondencia y Producción por sus entradas reales', async () => {
  const profile = { baseUrl: 'https://localhost/app/', timeoutMilliseconds: 1000 };
  const correspondenceFrame = navigationPage();
  const correspondence = navigationPage(correspondenceFrame);
  const openedCorrespondence = await runner.openCorrespondenceConsumer(correspondence, profile, {
    entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', menuSelector: '#CR-GT-01_card_boton',
    containerFrameSelector: '#ContentPlacenter_ifrm_ds_', contextOrdinal: 2,
    documentOrdinal: 1, rowSelector: '[tip_event="documento_solic_tramite"]',
    openSelector: '[tip_event="vis_doc_selecion_wf"]:has(.fa-file-pdf)', viewerFrameSelector: '#ifrm_visor_'
  });
  assert.equal(openedCorrespondence.root, correspondenceFrame);
  assert.deepEqual(correspondence.calls.filter(item => item[0] === 'click'), [
    ['click', '#CR-GT-01_card_boton:visible', 0]
  ]);
  assert.deepEqual(correspondenceFrame.calls.filter(item => item[0] === 'click'), [
    ['click', '[tip_event="documento_solic_tramite"]:visible', 1],
    ['click', '[tip_event="vis_doc_selecion_wf"]:has(.fa-file-pdf):visible', 0]
  ]);
  assert.equal(runner.adapterRegistry.correspondence.viewer, false);
  assert.equal(runner.adapterRegistry.correspondence.scrollZoom, false);

  const productionFrame = navigationPage();
  productionFrame.locator = selector => {
    const state = { selector };
    const locator = {
      first() { return this; },
      nth(index) { state.index = index; return this; },
      filter(options) { state.text = options.hasText; return this; },
      locator(child) { state.child = child; return this; },
      waitFor: async () => productionFrame.calls.push(['wait', state.selector, state.text, state.child]),
      count: async () => state.text ? 1 : 2,
      click: async () => productionFrame.calls.push(state.selector === '#TreeViewArchivo a'
        ? ['tree-select', state.selector, state.text]
        : ['click-row-action', state.selector, state.text, state.child]),
      evaluateAll: async (callback, label) => {
        const callbackSource = callback.toString();
        if (callbackSource.includes('collapsedAncestors')) return 'already-visible';
        state.text = label;
        return 0;
      },
      getAttribute: async () => '../pdfjs/pdf_anotate_view/index.html'
    };
    return locator;
  };
  const production = navigationPage(productionFrame);
  const openedProduction = await runner.openProductionConsumer(production, profile, {
    entryPath: 'Defaul/WebFormInicioDocuarchiGestion.aspx', menuSelector: '#GD-GD-14_card_boton',
    containerFrameSelector: '#ContentPlacenter_ifrm_ds_', contextSelector: '#TreeViewArchivo a',
    contextPath: ['Nivel raíz', 'Nivel hijo', 'Expediente de prueba'], documentTitle: 'Documento pesado.pdf',
    rowSelector: '#data_grid tr[id]', openSelector: 'a[title="Visualiza archivo"]',
    viewerFrameSelector: '#Iframe_visor_externo_da_'
  });
  assert.equal(openedProduction.root, productionFrame);
  assert.deepEqual(production.calls.filter(item => item[0] === 'click'), [
    ['click', '#GD-GD-14_card_boton:visible', 0]
  ]);
  assert.deepEqual(productionFrame.calls.filter(item => item[0].startsWith('tree-') || item[0] === 'click-row-action'), [
    ['tree-select', '#TreeViewArchivo a', 'Nivel raíz'],
    ['tree-select', '#TreeViewArchivo a', 'Nivel hijo'],
    ['tree-select', '#TreeViewArchivo a', 'Expediente de prueba'],
    ['click-row-action', '#data_grid tr[id]:visible', 'Documento pesado.pdf', 'a[title="Visualiza archivo"]:visible']
  ]);
});

test('Producción rechaza un prefijo documental ambiguo antes de abrir el visor', async () => {
  const matchingRows = {
    filter() { return this; },
    first() { return this; },
    waitFor: async () => {},
    count: async () => 2
  };
  await assert.rejects(
    runner.clickProductionDocumentAction({ locator: () => matchingRows }, {
      rowSelector: '#data_grid tr[id]', documentTitle: 'Handler_image_wf (5',
      openSelector: 'a[title="Visualiza archivo"]'
    }, 1000),
    /DOC105_PRODUCTION_DOCUMENT_AMBIGUOUS/
  );
});

test('Producción selecciona cada nivel además de expandirlo antes del expediente', async () => {
  const actions = [];
  let selectedLabel = '';
  const root = {
    waitForFunction: async () => {},
    waitForTimeout: async () => {},
    locator: () => ({
      nth: () => ({ click: async () => actions.push(`select:${selectedLabel}`) }),
      evaluateAll: async (callback, label) => {
        const callbackSource = callback.toString();
        if (callbackSource.includes('collapsedAncestors')) return 'already-visible';
        selectedLabel = label;
        return 0;
      }
    })
  };

  await runner.openProductionTreePath(root, '#TreeViewArchivo a',
    ['Nivel raíz', 'Nivel hijo', 'Expediente de prueba'], 1000);

  assert.deepEqual(actions, [
    'select:Nivel raíz',
    'select:Nivel hijo',
    'select:Expediente de prueba'
  ]);
  assert.match(runnerSource, /get_isInAsyncPostBack\(\) === false/);
  assert.doesNotMatch(runnerSource, /if \(!isFinal\) await root\.waitForTimeout\(100\)/);
});

test('Producción revela ancestros WebForms colapsados antes de seleccionar la ruta', async () => {
  const actions = [];
  const previousGetComputedStyle = global.getComputedStyle;
  global.getComputedStyle = element => element.style;
  try {
    let selectedLabel = '';
    const hiddenContainer = {
      style: { display: 'none' },
      matches: selector => selector === 'div[id$="Nodes"]',
      parentElement: null,
      previousElementSibling: null
    };
    const root = {
      waitForFunction: async () => {},
      waitForTimeout: async () => {},
      locator: () => ({
        nth: () => ({ click: async () => actions.push(`select-${selectedLabel}`) }),
        evaluateAll: async (callback, options) => {
          const target = {
            offsetWidth: 1,
            innerText: options,
            getClientRects: () => [],
            parentElement: callback.toString().includes('collapsedAncestors') ? hiddenContainer : null,
            closest: () => ({ querySelector: () => null })
          };
          if (callback.toString().includes('collapsedAncestors')) return callback([target], options);
          selectedLabel = options;
          return 0;
        }
      })
    };

    await runner.openProductionTreePath(root, '#TreeViewArchivo a', ['Expediente de prueba'], 1000);
    assert.equal(hiddenContainer.style.display, 'block');
    assert.deepEqual(actions, ['select-Expediente de prueba']);
    assert.match(runnerSource, /treeNodes\.nth\(targetIndex\)\.click\(\{ timeout \}\)/);
  } finally {
    global.getComputedStyle = previousGetComputedStyle;
  }
});

test('selección operativa de Producción se aplica solo en memoria y exige ese consumidor', () => {
  const input = validProfile();
  const base = withProfile(input, runner.loadProfile);
  assert.throws(() => runner.overrideProductionSelection(base, 'Expediente', 'Documento.pdf'),
    /DOC105_PRODUCTION_SELECTION_FORBIDDEN/);
  const selected = runner.selectConsumer(base, 'production');
  const overridden = runner.overrideProductionSelection(selected, 'Expediente', 'Documento.pdf', ['Raíz', 'Expediente']);
  const production = overridden.consumers.find(item => item.kind === 'production');
  assert.equal(production.contextTitle, 'Expediente');
  assert.equal(production.documentTitle, 'Documento.pdf');
  assert.deepEqual(production.contextPath, ['Raíz', 'Expediente']);
  assert.equal(base.consumers.find(item => item.kind === 'production').contextTitle, undefined);
});

test('adaptador de aprobación entra por la consulta autenticada y mide el iframe anidado', async () => {
  const nestedFrame = navigationPage();
  const page = navigationPage(nestedFrame);
  const opened = await runner.openApprovalExternalConsumer(page, {
    baseUrl: 'https://localhost/app/', timeoutMilliseconds: 1000
  }, {
    entryPath: 'Gestion/WebFormGaconsultaclasificacion.aspx',
    contextSelector: '#TreeViewEstructura a[href*="__doPostBack"]', treeOrdinal: 2,
    contextOrdinal: 1, documentOrdinal: 2,
    rowSelector: '[tip_event="ver_doc_col"]', openSelector: '[tip_event="ver_doc"]',
    containerFrameSelector: '#Iframe_visor_externo__', viewerFrameSelector: '#ifrm_visor_'
  });
  assert.equal(opened.root, nestedFrame);
  assert.equal(opened.frameSelector, '#ifrm_visor_');
  assert.equal(opened.viewer, true);
  assert.deepEqual(page.calls.filter(item => item[0] === 'click'), [
    ['click', '#TreeViewEstructura a[href*="__doPostBack"]:visible', 1],
    ['click', '[tip_event="ver_doc_col"]:visible', 0], ['click', '[tip_event="ver_doc"]:visible', 1]
  ]);
  assert.equal(page.calls.filter(item => item[0] === 'waitForFunction').length, 3);
  assert.match(runnerSource,
    /clickConsumerTarget\(page, consumer\.openSelector[\s\S]*waitForApprovalPostback\(page, profile\.timeoutMilliseconds\)[\s\S]*resolveContainerFrame/);
});

test('adaptador de aprobación conserva el fallback directo al handler sin exigir PDF.js', async () => {
  const nestedFrame = navigationPage();
  nestedFrame.evaluate = async () => 'handler';
  const page = navigationPage(nestedFrame);
  const opened = await runner.openApprovalExternalConsumer(page, {
    baseUrl: 'https://localhost/app/', timeoutMilliseconds: 1000
  }, {
    entryPath: 'Gestion/WebFormGaconsultaclasificacion.aspx',
    contextSelector: '#TreeViewEstructura a[href*="__doPostBack"]', treeOrdinal: 1,
    contextOrdinal: 1, documentOrdinal: 1,
    rowSelector: '[tip_event="ver_doc_col"][title="Con documentos"]', openSelector: '[tip_event="ver_doc"]',
    containerFrameSelector: '#Iframe_visor_externo__', viewerFrameSelector: '#ifrm_visor_'
  });
  assert.equal(opened.viewer, false);
  assert.equal(opened.viewerMode, 'handler');
});

test('adaptador de aprobación reconoce el visor clásico sin inventar un canvas PDF.js', async () => {
  const nestedFrame = navigationPage();
  nestedFrame.evaluate = async () => 'classic';
  const page = navigationPage(nestedFrame);
  const opened = await runner.openApprovalExternalConsumer(page, {
    baseUrl: 'https://localhost/app/', timeoutMilliseconds: 1000
  }, {
    entryPath: 'Gestion/WebFormGaconsultaclasificacion.aspx',
    contextSelector: '#TreeViewEstructura a[href*="__doPostBack"]', treeOrdinal: 1,
    contextOrdinal: 1, documentOrdinal: 1,
    rowSelector: '[tip_event="ver_doc_col"][title="Con documentos"]', openSelector: '[tip_event="ver_doc"]',
    containerFrameSelector: '#Iframe_visor_externo__', viewerFrameSelector: '#ifrm_visor_'
  });
  assert.equal(opened.viewer, false);
  assert.equal(opened.viewerMode, 'classic');
});

test('adaptador simple prepara Workflow y conserva el iframe directo del handler', async () => {
  const page = navigationPage();
  const opened = await runner.openSimpleHandlerConsumer(page, {
    baseUrl: 'https://localhost/app/', timeoutMilliseconds: 1000
  }, {
    simpleClass: 'workflowOtherFormats', taskId: 1, documentOrdinal: 1,
    entryPath: 'workflow/WebFormVisor_Otros_Formatos.aspx', viewerFrameSelector: '#ifrm_visor_'
  });
  assert.equal(opened.root, page);
  assert.equal(opened.frameSelector, '#ifrm_visor_');
  assert.deepEqual(page.calls.filter(item => item[0] === 'goto').map(item => new URL(item[1]).pathname), [
    '/app/workflow/Webworkflow.aspx', '/app/workflow/WebFormVisor_Otros_Formatos.aspx'
  ]);
  assert.equal(page.calls.filter(item => item[0] === 'click').length, 1);
  assert.equal(runner.adapterRegistry.simpleHandler.viewer, false);
  assert.equal(runner.adapterRegistry.simpleHandler.scrollZoom, false);
});

test('plan ordena frío y caliente para las cuatro redes y bloquea consumidores deshabilitados', () => {
  const profile = withProfile(validProfile(), runner.loadProfile);
  const plan = runner.buildSamplePlan(profile);
  const workflow = plan.filter(item => item.consumer === 'workflow');
  assert.equal(workflow.length, 8);
  assert.deepEqual(workflow.slice(0, 2).map(item => item.cacheMode), ['cold', 'warm']);
  assert.equal(plan.filter(item => item.state === 'BLOCKED').length, 4);
});

test('cada par frío/caliente conserva el contexto autenticado completo sin clonarlo por storageState', () => {
  assert.doesNotMatch(runnerSource, /authenticationContext\.storageState\(\)/);
  assert.match(runnerSource, /createAuthenticatedWorkflowSession\(browser, authenticationOptions\)/);
  assert.match(runnerSource, /'cold', authenticationContext\)/);
  assert.match(runnerSource, /'warm', cold\.context\)/);
});

test('la espera opcional del derivado ocurre entre frío y caliente en la misma sesión', () => {
  assert.match(runnerSource, /options\.derivativeWaitMilliseconds > 0 && cold\.row\.loadingMode === 'STREAM'/);
  assert.match(runnerSource, /setTimeout\(resolve, options\.derivativeWaitMilliseconds\)/);
  assert.match(runnerSource, /--derivative-wait-ms/);
  assert.match(runnerSource, /DOC105_DERIVATIVE_WAIT_INVALID/);
});

test('Workflow espera el iframe oficial antes de medir el primer canvas', () => {
  assert.equal(typeof workflowPerformanceRunner.waitForViewerFrame, 'function');
  assert.equal(typeof workflowPerformanceRunner.waitForFirstCanvas, 'function');
  assert.match(runnerSource, /await waitForViewerFrame\(measurementRoot, profile\.timeoutMilliseconds, frameSelector\)/);
  assert.match(runnerSource, /await waitForFirstCanvas\(measurementRoot, profile\.timeoutMilliseconds, network, frameSelector\)/);
});

test('condiciones CDP convierten Mbps a bytes por segundo y conservan latencia', () => {
  assert.deepEqual(runner.networkConditions({ downloadMbps: 20, latencyMilliseconds: 80 }), {
    offline: false,
    latency: 80,
    downloadThroughput: 2500000,
    uploadThroughput: 2500000
  });
});

test('aplicación CDP habilita Network antes de imponer las condiciones', async () => {
  const calls = [];
  const session = { send: async (name, payload) => calls.push({ name, payload }) };
  await runner.applyNetworkProfile(session, { downloadMbps: 10, latencyMilliseconds: 100 });
  assert.deepEqual(calls.map(item => item.name), ['Network.enable', 'Network.emulateNetworkConditions']);
  assert.equal(calls[1].payload.downloadThroughput, 1250000);
  await assert.rejects(() => runner.applyNetworkProfile({ send: async () => { throw new Error('closed'); } }, {
    downloadMbps: 10, latencyMilliseconds: 100
  }), /closed/);
});

test('fallo desconocido se clasifica por etapa sin conservar el mensaje', async () => {
  const outcome = await runner.executeCell(
    { newContext: async () => { throw new Error('sensitive runtime detail'); } },
    {},
    {},
    { kind: 'workflow' },
    { name: 'remote-100' },
    'cold'
  );
  assert.equal(outcome.row.state, 'FAILED');
  assert.equal(outcome.row.errorCategory, 'DOC105_CONTEXT_FAILED');
  assert.doesNotMatch(JSON.stringify(outcome), /sensitive runtime detail/);
});

test('bloqueo del runner Workflow conserva una categoría DOC-105 saneada', async () => {
  const outcome = await runner.executeCell(
    { newContext: async () => { throw new Error('WORKFLOW_PDF_PERF_LIVE_FRAME_UNAVAILABLE'); } },
    {},
    {},
    { kind: 'workflow' },
    { name: 'remote-100' },
    'cold'
  );
  assert.equal(outcome.row.state, 'BLOCKED');
  assert.equal(outcome.row.errorCategory, 'DOC105_WORKFLOW_LIVE_FRAME_UNAVAILABLE');
  const evidence = runner.sanitizeEvidence({ rows: [outcome.row] });
  assert.equal(evidence.rows[0].errorCategory, 'DOC105_WORKFLOW_LIVE_FRAME_UNAVAILABLE');
});

test('normalización no acepta mensajes libres como categorías de evidencia', () => {
  assert.equal(
    runner.normalizeExecutionErrorCode(new Error('private URL and document title'), 'MEASUREMENT'),
    'DOC105_MEASUREMENT_FAILED'
  );
});

test('una detención previa a la matriz persiste únicamente su categoría saneada', async () => {
  const artifacts = path.resolve(__dirname, '../artifacts');
  const target = path.join(artifacts, 'doc105-pdf-cross-module-stop.json');
  await runner.writeStoppedEvidence(Object.assign(new Error('private detail'), { code: 'DOC105_LOGIN_FAILED' }));
  const evidence = JSON.parse(fs.readFileSync(target, 'utf8'));
  assert.deepEqual(evidence, { result: 'BLOCKED', errorCategory: 'DOC105_LOGIN_FAILED' });
  await runner.writeStoppedEvidence(new Error('private URL and document title'));
  const sanitized = JSON.parse(fs.readFileSync(target, 'utf8'));
  assert.deepEqual(sanitized, { result: 'BLOCKED', errorCategory: 'DOC105_FAILED' });
  fs.rmSync(target, { force: true });
});

test('diagnóstico de primer canvas distingue sesión, iframe y ausencia de handler', async () => {
  const page = {
    isClosed: () => false,
    evaluate: async () => ({
      loginVisible: false, framePresent: true, viewerSourceSet: true,
      frameDocumentAvailable: true, pageCount: 0, canvasCount: 0, nonZeroCanvasCount: 0
    })
  };
  assert.equal(await runner.diagnoseFirstCanvasFailure(page, '#ifrm_visor_', { exchanges: [] }),
    'DOC105_HANDLER_RESPONSE_NOT_OBSERVED');
  page.evaluate = async () => ({ loginVisible: true });
  assert.equal(await runner.diagnoseFirstCanvasFailure(page, '#ifrm_visor_', { exchanges: [] }),
    'DOC105_SESSION_NOT_AUTHENTICATED');
});

test('Content-Range acepta únicamente intervalos cerrados coherentes', () => {
  assert.deepEqual(runner.parseContentRange('bytes 0-262143/1048576'), {
    start: 0, end: 262143, total: 1048576, length: 262144
  });
  assert.equal(runner.parseContentRange('bytes */1048576'), null);
  assert.equal(runner.parseContentRange('bytes 20-10/100'), null);
  assert.equal(runner.parseContentRange('bytes 0-100/100'), null);
});

test('unión no duplica intervalos solapados o adyacentes', () => {
  const result = runner.unionIntervals([
    { start: 0, end: 99 }, { start: 50, end: 149 }, { start: 150, end: 199 }, { start: 400, end: 449 }
  ]);
  assert.equal(result.bytes, 250);
  assert.deepEqual(result.intervals, [{ start: 0, end: 199 }, { start: 400, end: 449 }]);
});

test('transferencia separa bytes únicos, declarados y físicos', () => {
  const result = runner.summarizeTransfers([
    { status: 206, contentRange: 'bytes 0-262143/1048576', contentLength: 262144, transferSize: 262500, beforeFirstCanvas: true },
    { status: 206, contentRange: 'bytes 131072-393215/1048576', contentLength: 262144, transferSize: 262500, beforeFirstCanvas: true }
  ]);
  assert.equal(result.uniqueBytes, 393216);
  assert.equal(result.uniqueBytesBeforeFirstCanvas, 393216);
  assert.equal(result.bytesBeforeFirstCanvas, 393216);
  assert.equal(result.requestCountBeforeFirstCanvas, 2);
  assert.equal(result.physicalDeclaredBytes, 524288);
  assert.equal(result.physicalTransferredBytes, 525000);
  assert.equal(result.partialBeforeFirstCanvas, true);
});

test('la captura conserva solo Range para evaluar el contrato de transporte', () => {
  assert.match(runnerSource, /const requestHeaders = response\.request\(\)\.headers\(\)/);
  assert.match(runnerSource, /requestRange: requestHeaders\.range \|\| ''/);
  assert.match(runnerSource, /requestHeaders: \{ range: item\.requestRange \}/);
  assert.doesNotMatch(runnerSource, /requestHeaders:\s*response\.request\(\)\.headers\(\)/);
});

test('respuesta 200 STREAM nunca se declara parcial', () => {
  const result = runner.summarizeTransfers([
    { status: 200, contentLength: 1048576, transferSize: 500000, beforeFirstCanvas: true }
  ]);
  assert.equal(result.rangeResponseCount, 0);
  assert.equal(result.partialBeforeFirstCanvas, false);
  assert.equal(result.bytesBeforeFirstCanvas, 1048576);
  assert.equal(result.documentLengthBytes, 1048576);
});

test('vista previa evalúa parcialidad contra el PDF original sin relajar el visor completo', () => {
  const metrics = { pdfLoading: { mode: 'RANGE' }, transportPolicySatisfied: false };
  const transfers = { rangeResponseCount: 6 };
  assert.equal(runner.evaluateConsumerTransportPolicy(metrics, transfers, 'preview', 1629367, 289802461), true);
  assert.equal(runner.evaluateConsumerTransportPolicy(metrics, transfers, 'preview', 289802461, 289802461), false);
  assert.equal(runner.evaluateConsumerTransportPolicy(metrics, transfers, 'full', 1629367, 289802461), false);
});

test('primera página útil rechaza el falso positivo observado en Producción', () => {
  const budget = validProfile().networkProfiles[3].firstPageBudget;
  assert.deepEqual(runner.evaluateFirstPageBudget({
    firstCanvasMilliseconds: 48774,
    bytesBeforeFirstCanvas: 231079058,
    documentLengthBytes: 289799314,
    requestCountBeforeFirstCanvas: 223
  }, budget), {
    state: 'FAILED', passed: false, errorCategory: 'DOC105_FIRST_PAGE_TIME_BUDGET_EXCEEDED'
  });
  assert.equal(runner.evaluateFirstPageBudget({
    firstCanvasMilliseconds: 5000,
    bytesBeforeFirstCanvas: 231079058,
    documentLengthBytes: 289799314,
    requestCountBeforeFirstCanvas: 223
  }, budget).errorCategory, 'DOC105_FIRST_PAGE_BYTES_BUDGET_EXCEEDED');
  assert.equal(runner.evaluateFirstPageBudget({
    firstCanvasMilliseconds: 5000,
    bytesBeforeFirstCanvas: 8388608,
    documentLengthBytes: 289799314,
    requestCountBeforeFirstCanvas: 223
  }, budget).errorCategory, 'DOC105_FIRST_PAGE_REQUEST_BUDGET_EXCEEDED');
});

test('cronometra desde la acción documental y acepta solo una vista previa terminada', () => {
  assert.match(runnerSource, /const measurementStarted = await clickProductionDocumentAction/);
  assert.match(runnerSource, /started = opened\.measurementStarted \|\| started/);
  assert.match(runnerSource, /DOC105_TRANSPORT_POLICY_FAILED/);
  const performanceSource = fs.readFileSync(path.join(__dirname, '../scripts/run-workflow-pdf-viewer-performance-interactive.cjs'), 'utf8');
  assert.match(performanceSource, /#pdf-first-page-preview\[data-ready="true"\] canvas/);
});

test('primera página útil bloquea presupuesto ausente y acepta una muestra acotada', () => {
  const measurement = {
    firstCanvasMilliseconds: 5000,
    bytesBeforeFirstCanvas: 8388608,
    documentLengthBytes: 33554432,
    requestCountBeforeFirstCanvas: 12
  };
  assert.equal(runner.evaluateFirstPageBudget(measurement).state, 'BLOCKED');
  assert.deepEqual(runner.evaluateFirstPageBudget(measurement, validProfile().networkProfiles[3].firstPageBudget), {
    state: 'PASSED', passed: true, errorCategory: ''
  });
});

test('difiere el fallo del handler solo cuando canvas y respuestas Range fueron útiles', () => {
  assert.equal(runner.hasFatalHandlerFailure(1, { rangeResponseCount: 38 }, true), false);
  assert.equal(runner.hasFatalHandlerFailure(1, { rangeResponseCount: 0 }, true), true);
  assert.equal(runner.hasFatalHandlerFailure(1, { rangeResponseCount: 38 }, false), true);
  assert.equal(runner.hasFatalHandlerFailure(0, { rangeResponseCount: 0 }, false), false);
});

test('agregación conserva FAILED y BLOCKED como estados globales', () => {
  assert.equal(runner.aggregateResults([{ state: 'PASSED' }, { state: 'FAILED' }]), 'FAILED');
  assert.equal(runner.aggregateResults([{ state: 'PASSED' }, { state: 'BLOCKED' }]), 'BLOCKED');
  assert.equal(runner.aggregateResults([{ state: 'PASSED' }, { state: 'NOT_APPLICABLE' }]), 'PASSED');
});

test('la salida separa el resultado Workflow del bloqueo de cobertura transversal', () => {
  assert.match(runnerSource, /DOC105_WORKFLOW_RESULT=/);
  assert.match(runnerSource, /enabledConsumers/);
  assert.match(runnerSource, /consumer\.kind\.replace/);
  assert.match(runnerSource, /DOC105_BLOCKED_CONSUMER_COUNT=/);
});

test('límites históricos fallan exactamente en 3548 ms de scroll y 435 ms de zoom', () => {
  assert.deepEqual(runner.evaluateInteractionBudget(3547, 434), {
    scrollPassed: true, zoomPassed: true, scrollLimit: 3548, zoomLimit: 435
  });
  assert.equal(runner.evaluateInteractionBudget(3548, 100).scrollPassed, false);
  assert.equal(runner.evaluateInteractionBudget(100, 435).zoomPassed, false);
  assert.deepEqual(runner.evaluateInteractionBudget(499, 99, { scrollMilliseconds: 500, zoomMilliseconds: 100 }), {
    scrollPassed: true, zoomPassed: true, scrollLimit: 500, zoomLimit: 100
  });
});

test('errores se reducen a categorías cerradas', () => {
  assert.deepEqual(runner.classifyRuntimeError({ type: 'page', message: 'sensitive' }), {
    code: 'DOC105_PAGE_ERROR', type: 'page'
  });
  assert.deepEqual(runner.classifyRuntimeError({ type: 'other' }), {
    code: 'DOC105_UNKNOWN_ERROR', type: 'unknown'
  });
});

test('errores de consola se clasifican sin conservar mensajes ni URL', () => {
  assert.equal(runner.classifyConsoleFailure('Failed to load resource: net::ERR_ABORTED'),
    'DOC105_CONSOLE_RESOURCE_ERROR');
  assert.equal(runner.classifyConsoleFailure('Uncaught TypeError: detalle sensible'),
    'DOC105_CONSOLE_TYPE_ERROR');
  assert.equal(runner.classifyConsoleFailure('Uncaught ReferenceError: detalle sensible'),
    'DOC105_CONSOLE_REFERENCE_ERROR');
  assert.equal(runner.classifyConsoleFailure('Uncaught SyntaxError: detalle sensible'),
    'DOC105_CONSOLE_SYNTAX_ERROR');
  assert.equal(runner.classifyConsoleFailure('detalle sensible'), 'DOC105_CONSOLE_ERROR');
  assert.match(runnerSource, /DOC105_CONSOLE_RESOURCE_ERROR'\) observation\.warnings\.push/);
  assert.match(runnerSource, /else observation\.failures\.push\(category\)/);
});

test('evidencia elimina campos no permitidos y no promueve un bloqueo', () => {
  const evidence = runner.sanitizeEvidence({
    rows: [{
      consumer: 'workflow', network: 'remote-10', cacheMode: 'cold', state: 'BLOCKED',
      httpStatus: 0, loadingMode: 'UNKNOWN', secret: 'must-not-be-copied'
    }]
  });
  assert.equal(evidence.result, 'BLOCKED');
  assert.equal(evidence.rows[0].firstCanvasSource, 'full');
  assert.equal(Object.hasOwn(evidence.rows[0], 'secret'), false);
  assert.doesNotMatch(JSON.stringify(evidence), /must-not-be-copied/);
});

test('un guardado fallido impide aprobar una matriz de lectura exitosa', () => {
  const evidence = runner.sanitizeEvidence({
    rows: [{ consumer: 'workflow', network: 'remote-10', cacheMode: 'cold', state: 'PASSED' }],
    mutation: { state: 'FAILED', postCount: 2 }
  });
  assert.equal(evidence.result, 'FAILED');
  assert.deepEqual(evidence.mutation, { state: 'FAILED', postCount: 2 });
});

test('guardado DOC-104 heredado solo aprueba exactamente un POST y efecto único', () => {
  assert.deepEqual(runner.classifyGraphSaveEvidence({ result: 'PASSED', postCount: 1, singleEffect: true }), {
    state: 'PASSED', postCount: 1
  });
  for (const graphEvidence of [
    { result: 'PASSED', postCount: 0, singleEffect: true },
    { result: 'PASSED', postCount: 2, singleEffect: true },
    { result: 'PASSED', postCount: 1, singleEffect: false },
    { result: 'FAILED', postCount: 1, singleEffect: true }
  ]) assert.equal(runner.classifyGraphSaveEvidence(graphEvidence).state, 'FAILED');
});

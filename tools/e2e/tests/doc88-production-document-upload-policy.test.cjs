'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const fs = require('node:fs');
const path = require('node:path');
const { resolveControls, resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');
const {
  createRuntimeEnvironment,
  eraseSecrets,
  executePlatformRun,
  preflightPlatform
} = require('../scripts/support/workflow-e2e-platform.cjs');

const root = path.resolve(__dirname, '..', '..', '..');
const read = (...segments) => fs.readFileSync(path.join(root, ...segments), 'utf8');
const profileExample = () => JSON.parse(read('tools', 'e2e', 'profiles', 'doc88-production-document-upload.profile.example.json'));

test('DOC-88 conserva el handler aprobado tras DOC-90 y delimita el cliente a PRODUCCION', () => {
  const handlerPath = path.join(root, 'generic_control', 'FileUploadHandler_.ashx.vb');
  const normalizedHandler = fs.readFileSync(handlerPath, 'utf8').replace(/\r\n/g, '\n');
  const handlerHash = crypto.createHash('sha256').update(normalizedHandler).digest('hex');
  const client = read('generic_control', 'FileUploadHandler.js');

  assert.equal(handlerHash, '8a42b7439f754e23d257e67a3591e46f99063c648d01847ba6aa891ddbdff6c1');
  assert.match(fs.readFileSync(handlerPath, 'utf8'), /If eventoAdjuntaActual = "PRODUCCION" Then[\s\S]*uploadFiles\.error_sistema = etapaCargaProduccion[\s\S]*Else[\s\S]*uploadFiles\.error_sistema = ex\.Message/);
  assert.match(client, /funcion_name == "insert_row_producion_documental" && this\.settings\.evento_adjunta == "PRODUCCION"/);
  assert.match(client, /funcion_name == "insert_row_producion_documental" && this\.settings\.evento_adjunta != "PRODUCCION"/);
});

test('DOC-88 rechaza contexto incompleto antes de desreferenciarlo', () => {
  const preparation = read('Gestion', 'ClassGaProducionDocumental.vb');
  const service = read('webservice', 'WebServiceProducion.asmx.vb');
  const storage = read('workflow', 'ClassAlmacenamiento.vb');

  assert.match(preparation, /SeleccionPartes\.Length < 3/);
  assert.match(preparation, /EstruUnidadConservacion Is Nothing OrElse EstruUnidadConservacion\.Length = 0/);
  assert.match(service, /PRODUCCION_CARGA_CONFIGURACION_INCOMPLETA/);
  assert.match(storage, /EsctructuraExpediente Is Nothing OrElse EsctructuraExpediente\.Length = 0/);
});

test('DOC-88 realiza un solo envío automático y no consulta existencia', () => {
  const client = read('generic_control', 'FileUploadHandler.js');
  const storage = read('workflow', 'ClassAlmacenamiento.vb');
  const sendStart = client.indexOf('async _EnviaArchivoServidor');
  const sendEnd = client.indexOf('async _RegistraArchivoInterfaz', sendStart);
  const send = client.slice(sendStart, sendEnd);
  const storageStart = storage.indexOf('Function AlmacenamientoDocumentoProduccionDocumental');
  const storageEnd = storage.indexOf('End Function', storageStart);
  const productionStorage = storage.slice(storageStart, storageEnd);

  assert.equal((send.match(/fetch\(/g) || []).length, 1);
  assert.match(send, /PRODUCCION_CARGA_CONFIRMACION_INCIERTA/);
  assert.doesNotMatch(send + productionStorage, /Solicita_id_inventario_documental|ExisteDocumento|ExisteRecibo|retry/i);
  assert.match(productionStorage, /If Result <> "YES" Then[\s\S]*Exit Function/);
});

test('DOC-88 proyecta el identificador confirmado sin iniciar otra carga', () => {
  const client = read('generic_control', 'FileUploadHandler.js');
  const start = client.indexOf('async _RegistraArchivoInterfaz');
  const end = client.indexOf('this.settings.evento_adjunta != "PRODUCCION"', start);
  const productionProjection = client.slice(start, end);

  assert.match(productionProjection, /PRODUCCION_CARGA_PROYECCION_FALLIDA/);
  assert.match(productionProjection, /UploadFilesResult\[0\]\.id_image;/);
  assert.doesNotMatch(productionProjection, /id_imageinsert_row_documento_relacionado|fetch\(/);
});

test('DOC-88 registra un preview UI autenticado sin almacenamiento', () => {
  const scenario = resolveScenario('production-document-upload-preview');
  assert.equal(scenario.doc, 'doc88');
  assert.equal(scenario.stage, 'preview');
  assert.equal(scenario.resource, null);
  assert.equal(scenario.controlProfileField, 'expedientId');
  assert.deepEqual(scenario.requiredSecrets, ['workflow-account', 'workflow-password', 'readonly-db-user', 'readonly-db-password']);
  assert.deepEqual(scenario.requiredAuthorizations, ['environment', 'account', 'discardable-file']);
  assert.deepEqual(scenario.controls, ['production-document-records']);
  assert.equal(scenario.controlExpectations['production-document-records'], 'unchanged');
  assert.ok(scenario.expectations.includes('production-document-upload-preview-ui'));
  assert.ok(scenario.expectations.includes('no-upload-request'));
  const [control] = resolveControls(scenario.controls);
  assert.match(control.query, /^SELECT\b/i);
  assert.equal((control.query.match(/\?/g) || []).length, 1);
  assert.doesNotMatch(control.query, /\b(?:INSERT|UPDATE|DELETE|CALL|EXEC)\b/i);
});

test('DOC-88 valida solamente datos no sensibles y un PDF del paquete E2E', () => {
  const profile = validateProfile(profileExample());
  assert.equal(profile.module, 'GESTOR');
  assert.equal(profile.expedientId, 123);
  assert.equal(profile.documentTypeId, 50);
  assert.equal(profile.documentTypeName, 'Comprobante de Egreso');
  assert.equal(profile.fixturePath, 'doc85-constancia-inscripcion.pdf');
  assert.equal(validateProfile({ ...profileExample(), expedientId: 700001 }).expedientId, 700001);
  assert.throws(() => validateProfile({ ...profileExample(), fixturePath: '../externo.pdf' }), /FIXTURE_INVALID/);
  assert.throws(() => validateProfile({ ...profileExample(), password: 'prohibido' }), /FORBIDDEN_FIELD/);
});

test('DOC-88 reutiliza sesión y prepara la UI sin activar Guardar', () => {
  const runner = read('tools', 'e2e', 'scripts', 'run-workflow-e2e-platform.cjs');
  assert.match(runner, /createAuthenticatedWorkflowSession/);
  assert.match(runner, /inspectProductionDocumentUploadPreviewUi/);
  assert.match(runner, /Gestion\/WebFormProducionDocumental\.aspx/);
  assert.match(runner, /#a_load_file:visible, #ma_load_file:visible/);
  assert.match(runner, /#file_element_adjunta_documeto_load_documento_006/);
  assert.match(runner, /a\[title="Guardar archivo"\]/);
  assert.match(runner, /referencia a objeto\|object reference\|nullreference/);
  assert.match(runner, /invalidResultText !== 'YES'/);
  assert.match(runner, /context\.request\.post\(preparationServiceUrl/);
  assert.match(runner, /route\.abort\(\)/);
  assert.match(runner, /uploadRequests !== 0 \|\| observedUploadRequests !== 0/);
  const previewStart = runner.indexOf('async function inspectProductionDocumentUploadPreviewUi');
  const previewEnd = runner.indexOf('async function inspectRegistroRutaSiiUi', previewStart);
  const preview = runner.slice(previewStart, previewEnd);
  assert.match(preview, /if \(!executesStorage\)[\s\S]*route\.abort\(\)/);
  assert.match(preview, /if \(executesStorage\)[\s\S]*save\.click\(\)/);
  assert.doesNotMatch(preview, /route\.fetch/);
});

test('DOC-88 registra una ejecución separada que exige almacenamiento real', () => {
  const scenario = resolveScenario('production-document-upload-execution');
  const profile = validateProfile({ ...profileExample(), scenarioId: scenario.id });
  assert.equal(scenario.stage, 'execution');
  assert.equal(profile.expedientId, 123);
  assert.equal(scenario.resource.mutating, true);
  assert.equal(scenario.resource.profileField, 'expedientId');
  assert.equal(scenario.controlProfileField, 'expedientId');
  assert.deepEqual(scenario.controlExpectations, { 'production-document-records': 'changed' });
  assert.ok(scenario.expectations.includes('production-document-upload-execution-ui'));
  assert.ok(scenario.expectations.includes('single-upload-request'));
  assert.ok(scenario.expectations.includes('visual-projection'));
});

test('DOC-88 diferencia rechazo del servidor y respuesta YES incompleta', () => {
  const runner = read('tools', 'e2e', 'scripts', 'run-workflow-e2e-platform.cjs');
  const start = runner.indexOf('async function inspectProductionDocumentUploadPreviewUi');
  const end = runner.indexOf('async function inspectRegistroRutaSiiUi', start);
  const execution = runner.slice(start, end);

  assert.match(execution, /stored\.error_sistema !== 'YES'/);
  assert.match(execution, /PRODUCCION_DOCUMENTAL_E2E_STORAGE_RECORD_ID_INVALID/);
  assert.match(execution, /PRODUCCION_DOCUMENTAL_E2E_STORAGE_IMAGE_ID_INVALID/);
  assert.match(execution, /PRODUCCION_DOCUMENTAL_E2E_STORAGE_FILE_NAME_INVALID/);
});

test('DOC-88 no crea reserva mutante y elimina secretos efímeros', () => {
  const profile = validateProfile(profileExample());
  const plan = preflightPlatform({ profile, authorizations: ['environment', 'account', 'discardable-file'] });
  const secrets = {
    'workflow-account': 'cuenta-prueba', 'workflow-password': 'secreto-efimero',
    'readonly-db-user': 'lector', 'readonly-db-password': 'secreto-lector'
  };
  const environment = createRuntimeEnvironment(plan, secrets);
  assert.equal(plan.scenario.resource, null);
  assert.equal(plan.scenario.stage, 'preview');
  eraseSecrets(secrets, environment);
  assert.equal(environment.DOC88_E2E_MYSQL_USER, undefined);
  assert.equal(environment.DOC88_E2E_MYSQL_PASSWORD, undefined);
});

test('DOC-88 usa el expediente como parámetro del control aunque no exista recurso mutante', async () => {
  const observedParameters = [];
  const outcome = await executePlatformRun({
    profile: profileExample(),
    authorizations: ['environment', 'account', 'discardable-file'],
    collectSecrets: async () => ({
      'workflow-account': 'cuenta-prueba',
      'workflow-password': 'secreto-efimero',
      'readonly-db-user': 'lector',
      'readonly-db-password': 'secreto-lector'
    }),
    createBrowser: async () => ({ close: async () => {} }),
    createSession: async () => ({ close: async () => {} }),
    createClient: async () => ({ dispose: async () => {} }),
    invoke: async () => { throw new Error('El preview no debe invocar operaciones mutantes.'); },
    inspectSession: async () => ({
      codes: { preparation: 'CONFIRMED', storage: 'NOT_INVOKED' },
      count: 1,
      latenciesMs: []
    }),
    readControl: async ({ taskId }) => {
      observedParameters.push(taskId);
      return 'a'.repeat(64);
    },
    assertIntegrity: async () => {},
    writeEvidence: async () => {}
  });

  assert.equal(outcome.success, true);
  assert.deepEqual(observedParameters, [123, 123]);
  assert.equal(outcome.controls.checked, 1);
  assert.equal(outcome.controls.unchanged, true);
});

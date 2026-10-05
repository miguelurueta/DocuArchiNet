const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const cp = require('node:child_process');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');

test('DOC-87 adapta únicamente el endpoint de ruta y no invoca el escritor legacy compartido', () => {
  const source = read('webservice', 'WebServiceWorkflow.asmx.vb');
  const start = source.indexOf('Public Function Service_registro_tarea_ruta_sii');
  const end = source.indexOf('Public Function Service_registro_tarea_flujo_sii', start);
  const method = source.slice(start, end);
  assert.match(method, /RegistroTareaRutaSiiCommandAdapter\.TryParse/);
  assert.match(method, /ServicioRegistroTareaRutaSii/);
  assert.doesNotMatch(method, /Registra_tarea_ruta_SII\(/);
  assert.doesNotMatch(method, /ex\.Message/);
});

test('DOC-87 expone colecciones concretas serializables en el contrato ASMX de Workflow', () => {
  const source = read('webservice', 'WebServiceWorkflow.asmx.vb');
  assert.doesNotMatch(source, /Public Function[^\r\n]*As IEnumerable\(Of /);
  assert.match(source, /Service_registro_tarea_ruta_sii\(ByVal parameter As Object\) As List\(Of class_service_workflow\)/);
  assert.match(source, /Service_crea_interface_registro_gestion\(ByVal id As Object\) As List\(Of control_general_drow_lista\)/);
  assert.match(source, /Service_solicita_lista_gabinetes_permitidos\(ByVal id As Object\) As List\(Of control_general_drow_lista\)/);
});

test('DOC-87 admite matrícula y subtipo SII opcionales sin omitir la validación del trámite seleccionado', () => {
  const service = read('Services', 'Workflow', 'RegistroTareaRutaSii', 'ServicioRegistroTareaRutaSii.vb');
  const repository = read('Infrastructure', 'Repositories', 'Workflow', 'RegistroTareaRutaSii', 'MySqlRegistroTareaRutaSiiRepository.vb');
  assert.match(service, /ConsultaAutoritativaRegistroRutaSii/);
  assert.match(repository, /Dim tipoEfectivo = If\(Not String\.IsNullOrWhiteSpace\(subtipo\), subtipo, tipoRecibo\)/);
  assert.match(repository, /StringComparison\.OrdinalIgnoreCase/);
  assert.match(repository, /NullIfEmpty\(datos\.Matricula\)/);
});

test('DOC-87 parametriza valores, restringe tabla dinámica y mantiene tarea más outbox en una transacción', () => {
  const source = read('Infrastructure', 'Repositories', 'Workflow', 'RegistroTareaRutaSii', 'MySqlRegistroTareaRutaSiiRepository.vb');
  assert.match(source, /Regex\.IsMatch\(datos\.NombreRuta, "\^\[A-Za-z0-9_\]\+\$"\)/);
  assert.match(source, /Using transaction = connection\.BeginTransaction\(\)[\s\S]*INSERT INTO INICIO_TAREAS_WORKFLOW[\s\S]*INSERT INTO workflow_registro_ruta_sii_outbox[\s\S]*transaction\.Commit\(\)/);
  assert.match(source, /SELECT GET_LOCK\(@lockName,5\)/);
  assert.match(source, /SELECT RELEASE_LOCK\(@lockName\)/);
  assert.match(source, /UPPER\(Nombre_Ruta\)='REGISTROPUBLICO'/);
  assert.match(source, /VALUES \(@receipt,@barcode/);
  assert.match(source, /SELECT 1 FROM F_W_E_REGISTROPUBLICO WHERE DATOS_RECIBO=@receipt LIMIT 1/);
  assert.doesNotMatch(source, /VALUES \('"?\s*&\s*datos\./i);
});

test('DOC-87 implementa despacho Docuarchi idempotente y observable', () => {
  const repository = read('Infrastructure', 'Repositories', 'Workflow', 'RegistroTareaRutaSii', 'MySqlRegistroTareaRutaSiiRepository.vb');
  const service = read('Services', 'Workflow', 'RegistroTareaRutaSii', 'ServicioRegistroTareaRutaSii.vb');
  assert.match(repository, /doc87-relation-/);
  assert.match(repository, /SELECT expediente_archivo_ID_EXPEDIENTE[\s\S]*WHERE RadicadoExterno=@receipt/);
  assert.match(repository, /RELATION_CONFLICT/);
  assert.match(service, /REGISTERED_RELATION_PENDING/);
  assert.match(service, /MarcarEventoPendiente/);
});

test('DOC-87 entrega migración idempotente, preflight SELECT y rollback conservador', () => {
  const base = ['Doc', 'Actualizacion', 'workflow', 'RegistroTareaRuta', 'DOC-87-correccion-registro-recibo-ruta-sii', 'SQL'];
  const apply = read(...base, '01-apply-workflow-outbox.sql');
  const preflight = read(...base, '02-preflight-workflow-outbox.sql');
  const rollback = read(...base, '03-rollback-workflow-outbox.sql');
  assert.match(apply, /CREATE TABLE IF NOT EXISTS workflow_registro_ruta_sii_outbox/i);
  assert.match(apply, /UNIQUE KEY uq_workflow_route_sii_receipt \(route_id, receipt\)/i);
  const executable = preflight.replace(/^\s*--.*$/gm, '').trim();
  executable.split(';').filter(Boolean).forEach(statement => assert.match(statement.trim(), /^SELECT\b/i));
  assert.match(rollback, /DOC87_ROLLBACK_BLOCKED_OUTBOX_HAS_EVENTS/);
  assert.match(rollback, /IF EXISTS \(SELECT 1 FROM workflow_registro_ruta_sii_outbox LIMIT 1\)/i);
  assert.doesNotMatch(rollback, /SIGNAL\s+SQLSTATE/i, 'el rollback debe ser compatible con MySQL 5.1');
});

test('DOC-87 conserva el resultado funcional de recibo ya registrado', () => {
  const service = read('Services', 'Workflow', 'RegistroTareaRutaSii', 'ServicioRegistroTareaRutaSii.vb');
  const client = read('js', 'workflow', 'WebFormGestionFlujoTrabajoCamaras.js');
  assert.match(service, /alreadyRegistered[\s\S]*ALREADY_REGISTERED/);
  assert.match(service, /ALREADY_REGISTERED_RELATION_PENDING/);
  assert.match(client, /El recibo ya se encuentra registrado/);
  assert.match(client, /ALREADY_REGISTERED_RELATION_PENDING/);
});

test('DOC-87 no modifica superficies compartidas protegidas', () => {
  const changed = cp.execFileSync('git', ['diff', '--name-only'], { cwd: root, encoding: 'utf8' }).replace(/\\/g, '/').split(/\r?\n/).filter(Boolean);
  const prohibited = [
    'generic_control/FileUploadHandler_.ashx',
    'generic_control/FileUploadHandler.js',
    'js/java_general/general_control_java.js',
    'workflow/ClassGestionTareasFlujoTrabajo.vb',
    'Gestion/ClassRaRelacionRadicadoExternoExpediente.vb'
  ];
  prohibited.forEach(file => assert.equal(changed.includes(file), false, `${file} debe permanecer intacto`));
});

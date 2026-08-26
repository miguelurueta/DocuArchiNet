const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');

const models = read('Modelo', 'Workflow', 'DevolverUsuarioAnterior', 'DevolverUsuarioAnteriorModels.vb');
const interfaces = read('Modelo', 'Workflow', 'DevolverUsuarioAnterior', 'DevolverUsuarioAnteriorInterfaces.vb');
const dtos = read('DTOs', 'Workflow', 'DevolverUsuarioAnterior', 'DevolverUsuarioAnteriorDtos.vb');
const repository = read('Infrastructure', 'Repositories', 'Workflow', 'MySqlDevolverUsuarioAnteriorRepository.vb');
const token = read('Infrastructure', 'Workflow', 'DevolverUsuarioAnterior', 'DevolverUsuarioAnteriorTokenCodec.vb');
const guard = read('Infrastructure', 'Workflow', 'DevolverUsuarioAnterior', 'MySqlDevolverUsuarioAnteriorConcurrencyGuard.vb');
const executor = read('Infrastructure', 'Workflow', 'DevolverUsuarioAnterior', 'WorkflowLegacyDevolverUsuarioAnteriorExecutorAdapter.vb');
const audit = read('Infrastructure', 'Workflow', 'DevolverUsuarioAnterior', 'WorkflowLegacyDevolverUsuarioAnteriorAuditoriaAdapter.vb');
const service = read('Services', 'Workflow', 'DevolverUsuarioAnterior', 'ServicioDevolverUsuarioAnterior.vb');
const gate = read('webservice', 'WorkflowPreviewSessionContextGate.vb');
const sharedModels = read('Modelo', 'Workflow', 'Terminar', 'WorkflowModernModels.vb');
const asmx = read('webservice', 'WebServiceWorkflowModern.asmx.vb');
const project = read('GestionDocumental-Docuarchi.net.vbproj');

test('DOC-36: contratos y puertos son exclusivos de usuario anterior', () => {
  for (const typeName of [
    'TareaDevolverUsuarioAnterior',
    'UsuarioHistoricoDevolverUsuarioAnterior',
    'ResultadoHistorialDevolverUsuarioAnterior',
    'SolicitudPreviewDevolverUsuarioAnterior',
    'SolicitudEjecutarDevolverUsuarioAnterior',
    'ResultadoEjecucionDevolverUsuarioAnterior',
    'AuditoriaDevolverUsuarioAnterior',
  ]) assert.match(models, new RegExp(`Class\\s+${typeName}\\b`));
  for (const typeName of [
    'ErrorDevolverUsuarioAnteriorDto',
    'PrevisualizacionDevolverUsuarioAnteriorDto',
    'ResultadoDevolverUsuarioAnteriorDto',
    'CodigosBloqueoDevolverUsuarioAnterior',
  ]) assert.match(dtos, new RegExp(`Class\\s+${typeName}\\b`));
  for (const interfaceName of [
    'IDevolverUsuarioAnteriorTareaRepository',
    'IDevolverUsuarioAnteriorAutorizacionRepository',
    'IDevolverUsuarioAnteriorHistorialRepository',
    'IDevolverUsuarioAnteriorTokenCodec',
    'IDevolverUsuarioAnteriorConcurrencyGuard',
    'IDevolverUsuarioAnteriorLegacyExecutor',
    'IDevolverUsuarioAnteriorAuditoriaRepository',
  ]) assert.match(interfaces, new RegExp(`Interface\\s+${interfaceName}\\b`));
  for (const source of [models, interfaces, dtos]) assert.doesNotMatch(source, /\b(?:Page|Session|IdConector)\b/);
});

test('DOC-36: permiso específico se resuelve en servidor y falla cerrado', () => {
  assert.match(sharedModels, /Property\s+PuedeDevolverUsuarioAnterior\s+As\s+Boolean/);
  const method = gate.match(/Public Function AsegurarContextoDevolverUsuarioAnterior[\s\S]*?End Function/);
  assert.ok(method, 'Falta el gate específico');
  assert.match(method[0], /SolicitaPermisosUsuarioWorkflow/);
  assert.match(method[0], /permisos\.Length\s*>\s*43/);
  assert.match(method[0], /PuedeDevolverUsuarioAnterior\s*=\s*False/);
  assert.doesNotMatch(method[0], /PuedeCambioRuta/);
});

test('DOC-36: historial es inmediato, parametrizado y no contiene escrituras', () => {
  assert.match(repository, /estado\.id_Estado < @idEstadoActual/);
  assert.match(repository, /estado\.Id_Usuario > 0/);
  assert.match(repository, /estado\.Id_Usuario <> @idUsuarioActual/);
  assert.match(repository, /ORDER BY estado\.id_Estado DESC LIMIT 1/);
  assert.match(repository, /@idEstadoActual/);
  assert.match(repository, /Dim anterior As EstadoHistorico = estados\(0\)/);
  assert.match(repository, /estados Is Nothing OrElse estados\.Count = 0/);
  assert.match(repository, /Parametro\("@idTarea", tarea\.IdTarea\)/);
  assert.match(repository, /anterior\.IdUsuario <= 0/);
  assert.match(repository, /usuario\.ESTADO_USUARIO = 1 AND usuario\.UTIL_ASIGNA_TAREA = 1/);
  assert.doesNotMatch(repository, /\b(?:INSERT|UPDATE|DELETE|CALL)\b/i);
});

test('DOC-36: token compromete tarea, snapshots, contexto y vencimiento', () => {
  assert.match(token, /Private Const DuracionMinutos As Integer = 5/);
  assert.match(token, /MachineKey\.Protect/);
  assert.match(token, /MachineKey\.Unprotect/);
  for (const member of ['IdTarea', 'IdEstadoActual', 'IdEstadoHistorico', 'IdUsuarioWorkflow', 'IdGrupoWorkflow', 'IdRutaWorkflow']) {
    assert.match(token, new RegExp(member));
  }
  assert.match(token, /vence <= DateTime\.UtcNow\.Ticks/);
});

test('DOC-36: lock exclusivo depende solo de tarea y se libera en la misma conexión', () => {
  assert.match(guard, /workflow-return-user-/);
  assert.match(guard, /SELECT GET_LOCK\(@lockName, 0\)/);
  assert.match(guard, /SELECT RELEASE_LOCK\(@lockName\)/);
  assert.doesNotMatch(guard, /tokenVersion/i);
});

test('DOC-36: preview no usa dependencias mutantes y ejecución relee dentro del lock', () => {
  const preview = service.match(/Public Function Previsualizar[\s\S]*?End Function/);
  const execute = service.match(/Public Function Ejecutar[\s\S]*?End Function/);
  assert.ok(preview && execute);
  assert.doesNotMatch(preview[0], /(?:Concurrency|Auditoria|Terminar_Tarea_Workflow|_ejecutor)/);
  assert.match(execute[0], /_concurrencyGuard\.Adquirir\(contexto, solicitud\.IdTarea\)/);
  const lockBlock = execute[0].match(/Using guard\.Lease[\s\S]*?End Using/);
  assert.ok(lockBlock, 'La revalidación debe estar dentro del lock');
  for (const pattern of [/_tareaRepository\.ObtenerTarea/, /_historialRepository\.ObtenerAntecedente/, /_tokenCodec\.Validar/, /_ejecutor\.Ejecutar/]) {
    assert.match(lockBlock[0], pattern);
  }
  assert.match(service, /historial\.UsuarioHistorico\.IdUsuarioWorkflow = contexto\.IdUsuarioWorkflow/);
});

test('DOC-36: adaptador mutante no activa correo, interfaz ni eventos', () => {
  assert.equal((executor.match(/\.Terminar_Tarea_Workflow\(/g) || []).length, 1);
  assert.match(executor, /Dim pagina As System\.Web\.UI\.Page = Nothing/);
  assert.match(executor, /resultadoEvento,\s*\r?\n\s*0,\s*\r?\n\s*resultadoCorreo/);
  assert.match(executor, /tarea\.IdActividadActual,\s*\r?\n\s*0,\s*\r?\n\s*0,\s*\r?\n\s*0,\s*\r?\n\s*0\)/);
  assert.doesNotMatch(executor, /Devolver_tarea_workflow_usuario_anterior|GridView|UpdatePanel|ModalPopupExtender/);
});

test('DOC-36: auditoría exclusiva es saneada y no depende de contratos de otra operación', () => {
  assert.match(audit, /Implements IDevolverUsuarioAnteriorAuditoriaRepository/);
  assert.match(audit, /Mecanismo=ASMX_DEVOLVER_USUARIO_ANTERIOR/);
  assert.doesNotMatch(audit, /WorkflowLegacyAuditoriaAdapter|TokenVersion/);
  assert.match(service, /If Not _auditoriaRepository\.Registrar\(auditoria\) AndAlso respuesta\.Exito Then/);
});

test('DOC-36: ASMX acepta solo tarea y token, y los archivos están incluidos en el proyecto', () => {
  const preview = asmx.match(/Public Function PreviewDevolverUsuarioAnterior[\s\S]*?End Function/);
  const execute = asmx.match(/Public Function EjecutarDevolverUsuarioAnterior[\s\S]*?End Function/);
  assert.ok(preview && execute);
  assert.match(preview[0], /AsegurarContextoDevolverUsuarioAnterior\(\)/);
  assert.match(execute[0], /AsegurarContextoDevolverUsuarioAnterior\(True\)/);
  assert.match(execute[0], /ByVal idTarea As Long,\s*[\s\S]*?ByVal tokenVersion As String/);
  assert.doesNotMatch(execute[0], /ByVal\s+(?:idUsuario|idActividad|idGrupo|idRuta|idFlujo|idConector)/i);
  for (const include of [
    'DTOs\\Workflow\\DevolverUsuarioAnterior\\DevolverUsuarioAnteriorDtos.vb',
    'Modelo\\Workflow\\DevolverUsuarioAnterior\\DevolverUsuarioAnteriorModels.vb',
    'Modelo\\Workflow\\DevolverUsuarioAnterior\\DevolverUsuarioAnteriorInterfaces.vb',
    'Infrastructure\\Repositories\\Workflow\\MySqlDevolverUsuarioAnteriorRepository.vb',
    'Infrastructure\\Workflow\\DevolverUsuarioAnterior\\DevolverUsuarioAnteriorTokenCodec.vb',
    'Infrastructure\\Workflow\\DevolverUsuarioAnterior\\MySqlDevolverUsuarioAnteriorConcurrencyGuard.vb',
    'Infrastructure\\Workflow\\DevolverUsuarioAnterior\\WorkflowLegacyDevolverUsuarioAnteriorExecutorAdapter.vb',
    'Infrastructure\\Workflow\\DevolverUsuarioAnterior\\WorkflowLegacyDevolverUsuarioAnteriorAuditoriaAdapter.vb',
    'Services\\Workflow\\DevolverUsuarioAnterior\\ServicioDevolverUsuarioAnterior.vb',
  ]) assert.match(project, new RegExp(`Compile Include="${include.replace(/\\/g, '\\\\')}"`));
});

test('DOC-36: la capacidad no refiere respuestas ni la devolución legacy', () => {
  for (const source of [models, interfaces, dtos, repository, token, guard, executor, audit, service]) {
    assert.doesNotMatch(source, /Classgestionrespuesta|Verifica_respuesta_|Reasigna_respuesta_envia_tarea_usuario|Devolver_tarea_workflow_usuario_anterior/);
  }
});

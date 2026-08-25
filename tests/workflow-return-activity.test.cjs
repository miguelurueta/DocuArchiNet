const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');

const models = read('Modelo', 'Workflow', 'Devolver', 'DevolverActividadModels.vb');
const interfaces = read('Modelo', 'Workflow', 'Devolver', 'DevolverActividadInterfaces.vb');
const dtos = read('DTOs', 'Workflow', 'Devolver', 'DevolverActividadDtos.vb');
const gate = read('webservice', 'WorkflowPreviewSessionContextGate.vb');
const sharedModels = read('Modelo', 'Workflow', 'Terminar', 'WorkflowModernModels.vb');
const repository = read('Infrastructure', 'Repositories', 'Workflow', 'MySqlDevolverActividadRepository.vb');
const cursorCodec = read('Infrastructure', 'Workflow', 'Devolver', 'DevolverActividadCursorCodec.vb');
const service = read('Services', 'Workflow', 'Devolver', 'ServicioDevolverActividad.vb');
const asmx = read('webservice', 'WebServiceWorkflowModern.asmx.vb');
const returnGuard = read('Infrastructure', 'Workflow', 'Devolver', 'MySqlDevolverActividadConcurrencyGuard.vb');
const legacyExecutor = read('Infrastructure', 'Workflow', 'Devolver', 'WorkflowLegacyDevolverActividadExecutorAdapter.vb');
const legacyAudit = read('Infrastructure', 'Workflow', 'Terminar', 'WorkflowLegacyAuditoriaAdapter.vb');
const project = read('GestionDocumental-Docuarchi.net.vbproj');
const returnPage = read('workflow', 'Webworkflow.aspx');
const returnCodeBehind = read('workflow', 'Webworkflow.aspx.vb');
const returnDesigner = read('workflow', 'Webworkflow.aspx.designer.vb');
const returnLegacyScript = read('js', 'workflow', 'Webworkflow.js');

test('DOC-32: contratos de devolución son exclusivos y no dependen de UI', () => {
  for (const typeName of [
    'TareaDevolverActividad',
    'SolicitudPreviewDevolverActividad',
    'SolicitudEjecutarDevolverActividad',
    'DestinoDevolverActividad',
    'ResultadoBusquedaDevolverActividad',
    'ResultadoResolucionDevolverActividad',
    'ResultadoEjecucionDevolverActividad',
    'AuditoriaDevolverActividad',
  ]) {
    assert.match(models, new RegExp(`Class\\s+${typeName}\\b`));
  }

  for (const typeName of [
    'ErrorDevolverActividadDto',
    'PrevisualizacionDevolverActividadDto',
    'ResultadoDevolverActividadDto',
    'CodigosBloqueoDevolverActividad',
  ]) {
    assert.match(dtos, new RegExp(`Class\\s+${typeName}\\b`));
  }

  for (const interfaceName of [
    'IDevolverActividadTareaRepository',
    'IDevolverActividadAutorizacionRepository',
    'IDevolverActividadPreviewRepository',
    'IDevolverActividadEjecucionRepository',
    'IDevolverActividadConcurrencyGuard',
    'IDevolverActividadAuditoriaRepository',
    'IDevolverActividadLegacyExecutor',
  ]) {
    assert.match(interfaces, new RegExp(`Interface\\s+${interfaceName}\\b`));
  }

  for (const source of [models, interfaces, dtos]) {
    assert.doesNotMatch(source, /\b(?:Page|Session)\b/);
  }
});

test('DOC-32: el permiso de devolución se resuelve en el servidor y falla cerrado', () => {
  assert.match(sharedModels, /Property\s+PuedeDevolverActividad\s+As\s+Boolean/);
  const match = gate.match(/Public Function AsegurarContextoDevolverActividad[\s\S]*?End Function/);
  assert.ok(match, 'Falta el gate de sesión para devolución');
  const method = match[0];
  assert.match(method, /SolicitaPermisosUsuarioWorkflow/);
  assert.match(method, /permisos\.Length\s*>\s*43/);
  assert.match(method, /permisos\(43\)/);
  assert.match(method, /PuedeDevolverActividad\s*=\s*False/);
});

test('DOC-32: los contratos están incluidos en el proyecto legado', () => {
  for (const include of [
    'Modelo\\Workflow\\Devolver\\DevolverActividadModels.vb',
    'Modelo\\Workflow\\Devolver\\DevolverActividadInterfaces.vb',
    'DTOs\\Workflow\\Devolver\\DevolverActividadDtos.vb',
    'Infrastructure\\Repositories\\Workflow\\MySqlDevolverActividadRepository.vb',
    'Infrastructure\\Workflow\\Devolver\\DevolverActividadCursorCodec.vb',
    'Services\\Workflow\\Devolver\\ServicioDevolverActividad.vb',
    'Infrastructure\\Workflow\\Devolver\\MySqlDevolverActividadConcurrencyGuard.vb',
    'Infrastructure\\Workflow\\Devolver\\WorkflowLegacyDevolverActividadExecutorAdapter.vb',
  ]) {
    assert.match(project, new RegExp(`Compile Include="${include.replace(/\\/g, '\\\\')}"`));
  }
});

test('DOC-32: las lecturas de Ruta y Flujo son entrantes, parametrizadas y aisladas', () => {
  assert.match(repository, /Implements[\s\S]*IDevolverActividadTareaRepository/);
  assert.match(repository, /disponible\.ID_ACTIVIDAD_SIGUIENTE\s*=\s*@idActividadActual/);
  assert.match(repository, /disponible\.ID_ACTIVIDADES_DISPONIBLES_ENVIO\s+AS\s+ID_CONECTOR/);
  assert.match(repository, /conector\.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO\s*=\s*@idActividadFlujoActual/);
  assert.match(repository, /conector\.ID_REGISTRO_ACTIVIDAD_ENVIO\s+AS\s+ID_CONECTOR/);
  assert.match(repository, /usuarioValido\.ESTADO_USUARIO\s*=\s*1/);
  assert.match(repository, /LIMIT\s+@limite\s+OFFSET\s+@desplazamiento/);
  assert.match(repository, /Parametro\("@idTarea"/);
  assert.doesNotMatch(repository, /\b(?:INSERT|UPDATE|DELETE|CALL)\b/i);
});

test('DOC-32: la devolución de Flujo agrupa la actividad fuente que devuelve', () => {
  const flujo = repository.match(/Private Function LeerDestinosFlujo[\s\S]*?End Function/);
  assert.ok(flujo, 'Falta la lectura de actividades anteriores de Flujo');
  assert.match(flujo[0], /conector\.ID_ACTIVIDAD_FUENTE\s+AS\s+ID_ACTIVIDAD_DESTINO/);
  assert.match(flujo[0], /conector\.ID_ACTIVIDAD_FUENTE, conector\.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE/);
  assert.match(flujo[0], /INNER JOIN grupos_workflow AS grupoValido ON grupoValido\.ID_GRUPO = usuarioValido\.GRUPOS_WORKFLOW_ID_GRUPO\s*"\s*&\s*\r?\n\s*"WHERE grupoValido\.RUTAS_WORKFLOW_ID_RUTA = @idRuta/);
  assert.match(flujo[0], /"AND grupoValido\.ID_ACTIVIDAD = origen\.ID_ACTIVIDAD\s*"\s*&\s*\r?\n\s*"AND usuarioValido\.idU_suario = conector\.ID_USUARIO_WORKFLOW_FUENTE/);
});

test('DOC-32: el cursor está ligado a tarea, contexto, tipo, filtro, orden y conector', () => {
  assert.match(cursorCodec, /MachineKey\.Protect/);
  assert.match(cursorCodec, /MachineKey\.Unprotect/);
  assert.match(cursorCodec, /tarea\.IdTarea/);
  assert.match(cursorCodec, /contexto\.IdUsuarioWorkflow/);
  assert.match(cursorCodec, /contexto\.IdGrupoWorkflow/);
  assert.match(cursorCodec, /contexto\.IdRutaWorkflow/);
  assert.match(cursorCodec, /tarea\.TipoContexto/);
  assert.match(cursorCodec, /terminoNormalizado/);
  assert.match(cursorCodec, /destino\.Orden/);
  assert.match(cursorCodec, /destino\.IdConector/);
});

test('DOC-32: el preview normaliza y no contiene dependencias mutantes', () => {
  const preview = service.match(/Public Function Previsualizar[\s\S]*?End Function/);
  assert.ok(preview, 'Falta el caso de uso de preview');
  assert.match(service, /LongitudMinimaTermino\s+As\s+Integer\s*=\s*2/);
  assert.match(service, /TamanoPaginaMaximo\s+As\s+Integer\s*=\s*50/);
  assert.match(preview[0], /_cursorCodec\.Validar/);
  assert.match(preview[0], /_previewRepository\.BuscarDestinos/);
  assert.doesNotMatch(preview[0], /(?:Concurrency|Auditoria|Terminar_Tarea_Workflow)/);
});

test('DOC-32: el ASMX de preview usa solo contexto y contratos de devolución', () => {
  const endpoint = asmx.match(/Public Function PreviewDevolverActividad[\s\S]*?End Function/);
  assert.ok(endpoint, 'Falta el endpoint PreviewDevolverActividad');
  assert.match(endpoint[0], /AsegurarContextoDevolverActividad/);
  assert.match(endpoint[0], /ServicioDevolverActividad/);
  assert.doesNotMatch(endpoint[0], /(?:SELECT|Terminar_Tarea_Workflow|MySqlTransicion)/);
});

test('DOC-32: el lock es exclusivo por tarea y no depende del token', () => {
  assert.match(returnGuard, /SELECT GET_LOCK\(@lockName, 0\)/);
  assert.match(returnGuard, /workflow-return-/);
  assert.match(returnGuard, /CrearNombreLock\(ByVal idTarea As Long\)/);
  assert.doesNotMatch(returnGuard, /tokenVersion/i);
});

test('DOC-32: el adaptador conserva eventos y notificación sin interfaz ni reasignación', () => {
  const calls = legacyExecutor.match(/\.Terminar_Tarea_Workflow\(/g) || [];
  assert.equal(calls.length, 1);
  assert.match(legacyExecutor, /Dim pagina As System\.Web\.UI\.Page = Nothing/);
  assert.match(legacyExecutor, /If\(destino\.RequiereNotificacion, 1, 0\)/);
  assert.match(legacyExecutor, /\r?\n\s*0,\r?\n\s*1,\r?\n\s*0,\r?\n\s*0\)/);
  assert.doesNotMatch(legacyExecutor, /(?:Activa_devolver_actividades_anteriores|Enviar_actividad_por_conector_flujo_de_trabajo_anterior|GridView|UpdatePanel|ModalPopupExtender)/);
});

test('DOC-32: la ejecución revalida dentro del lock y la auditoría no revierte éxitos', () => {
  const execute = service.match(/Public Function Ejecutar[\s\S]*?End Function/);
  assert.ok(execute, 'Falta el caso de uso de ejecución');
  assert.match(execute[0], /_concurrencyGuard\.Adquirir\(contexto, solicitud\.IdTarea\)/);
  const lockBlock = execute[0].match(/Using guard\.Lease[\s\S]*?End Using/);
  assert.ok(lockBlock, 'La revalidación debe ocurrir dentro del lock');
  assert.match(lockBlock[0], /_tareaRepository\.ObtenerTarea/);
  assert.match(lockBlock[0], /_autorizacionRepository\.Evaluar/);
  assert.match(lockBlock[0], /tarea\.TokenVersion, solicitud\.TokenVersion/);
  assert.match(lockBlock[0], /_ejecucionRepository\.ResolverDestino/);
  assert.match(lockBlock[0], /_ejecutor\.Ejecutar/);
  assert.match(service, /If Not _auditoriaRepository\.Registrar\(auditoria\) AndAlso resultado\.Exito Then/);
  assert.match(legacyAudit, /Implements IAuditoriaTransicionRepository, IDevolverActividadAuditoriaRepository/);
  assert.match(legacyAudit, /ASMX_DEVOLVER_ACTIVIDAD/);
});

test('DOC-32: el ASMX de ejecución solo acepta la identidad mínima de la operación', () => {
  const endpoint = asmx.match(/Public Function EjecutarDevolverActividad[\s\S]*?End Function/);
  assert.ok(endpoint, 'Falta el endpoint EjecutarDevolverActividad');
  assert.match(endpoint[0], /AsegurarContextoDevolverActividad\(True\)/);
  assert.match(endpoint[0], /MySqlDevolverActividadConcurrencyGuard/);
  assert.match(endpoint[0], /WorkflowLegacyDevolverActividadExecutorAdapter/);
  assert.match(endpoint[0], /\.IdTarea = idTarea/);
  assert.match(endpoint[0], /\.IdConector = idConector/);
  assert.match(endpoint[0], /\.TokenVersion = tokenVersion/);
  assert.doesNotMatch(endpoint[0], /(?:idActividadDestino|idUsuario|idGrupo|idRuta|idFlujo|destino)/i);
});

test('DOC-32: la capacidad nueva no incorpora recorridos de respuestas ni helpers Web Forms excluidos', () => {
  const capabilitySources = [models, interfaces, dtos, repository, cursorCodec, returnGuard, legacyExecutor, service];
  for (const source of capabilitySources) {
    assert.doesNotMatch(
      source,
      /Classgestionrespuesta|Verifica_respuesta_|Reasigna_respuesta_envia_tarea_usuario|Activa_devolver_actividades_anteriores|Enviar_actividad_por_conector_flujo_de_trabajo_anterior/,
    );
  }
});

test('DOC-32: contratos existentes de envío y su guard tokenizado mantienen sus firmas', () => {
  assert.match(asmx, /Public Function PreviewEnviarTarea\(ByVal idTarea As Long\) As PrevisualizacionTransicionDto/);
  assert.match(asmx, /Public Function EjecutarEnvioTarea\(ByVal idTarea As Long,[\s\S]*?ByVal idConector As Integer,[\s\S]*?ByVal tokenVersion As String\) As ResultadoTransicionDto/);
  assert.match(asmx, /Public Function EjecutarEnvioGrupo\(ByVal idTarea As Long,[\s\S]*?ByVal idActividadDestino As Integer,[\s\S]*?ByVal tokenVersion As String\) As ResultadoEnvioGrupoDto/);
  const oldGuard = read('Infrastructure', 'Workflow', 'Terminar', 'MySqlTransicionConcurrencyGuard.vb');
  assert.match(oldGuard, /Public Function Adquirir\(ByVal contexto As ContextoModuloWorkflow,[\s\S]*?ByVal idTarea As Long,[\s\S]*?ByVal tokenVersion As String\) As ResultadoGuardTransicion/);
  assert.match(oldGuard, /CrearNombreLock\(idTarea, tokenVersion\)/);
});

test('DOC-33: la interfaz moderna sustituye solo el postback legacy de actividad anterior', () => {
  assert.match(returnPage, /workflow-return-activity-trigger/);
  assert.match(returnPage, /workflow-return-activity-modern-modal/);
  assert.match(returnCodeBehind, /RegisterWorkflowReturnActivityModernPresentation\(\)/);
  assert.match(returnCodeBehind, /workflow-return-activity-ui\.js/);
  assert.doesNotMatch(returnPage, /D-TASK-ANT|Button_tool_devolver_a_actividades_anterior/);
  assert.doesNotMatch(returnCodeBehind, /Button_tool_devolver_a_actividades_anterior|Activa_devolver_actividades_anteriores/);
  assert.doesNotMatch(returnDesigner, /Button_tool_devolver_a_actividades_anterior/);
  assert.doesNotMatch(returnLegacyScript, /D-TASK-ANT|Button_tool_devolver_a_actividades_anterior/);
  assert.match(returnPage, /Button_tool_devolver_a_usuario/);
  assert.match(returnCodeBehind, /Button_tool_devolver_a_usuario_Click/);
});

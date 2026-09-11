const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const src = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb', 'utf8');
const legacy = fs.readFileSync('workflow/ClassAlmacenamiento.vb', 'utf8');
const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
const port = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportDocumentStoragePort.vb', 'utf8');
const sessionBootstrap = fs.readFileSync('Defaul/ClassGestorSesion.vb', 'utf8');

test('adaptador es el unico componente moderno que invoca storage legacy', () => {
  assert.match(src, /New ClassAlmacenamiento\(\)\.AlmacenaDocumentoTareaWorkflow/);
  assert.match(src, /1, comando\.NombreGabinete, comando\.Radicado, comando\.RutaArchivo/);
  assert.match(src, /comando\.NombreCaso, comando\.NombreClaseFormatoDocumento/);
});

test('mapea YES, rechazo y excepcion sin filtrar el mensaje legacy', () => {
  assert.match(src, /String\.Equals\(respuesta, "YES"/);
  assert.match(src, /DOCUMENT_STORAGE_REJECTED/);
  assert.match(src, /ClasificarRechazo\(respuesta\)/);
  assert.match(src, /DOCUMENT_STORAGE_GESTION_USER_MISSING/);
  assert.match(src, /DOCUMENT_STORAGE_CABINET_FIELDS_INVALID/);
  assert.match(src, /MensajeDiagnosticoLegacy\(respuesta & " \| " & DiagnosticoFormato\(comando\)\)/);
  assert.match(src, /\[REDACTED\]/);
  assert.match(src, /DOCUMENT_STORAGE_UNCERTAIN/);
  assert.doesNotMatch(src, /MensajeVisible = respuesta/);
  assert.doesNotMatch(src, /HttpContext\.Current|Session\.Item/);
  assert.match(src, /DiagnosticoFormato\(comando\)/);
  for (const label of ['nombreArchivo=', 'formatoSii=', 'tipoContenido=', 'extensionPreparada=', 'extensionLegacy=']) {
    assert.match(src, new RegExp(label));
  }
});

test('la caja negra permanece declarada con su firma conocida', () => {
  assert.match(legacy, /Function AlmacenaDocumentoTareaWorkflow\([\s\S]*ByRef EstructuraDatosImagen/);
});

test('login Workflow propaga el usuario Gestión requerido por almacenamiento', () => {
  const workflowBranch = sessionBootstrap.slice(
    sessionBootstrap.indexOf('If Modulestr = "WORKFLOW DOCUMENTAL" Then'),
    sessionBootstrap.indexOf('If Modulestr = "RADICACION DOCUMENTAL" Then')
  );
  assert.match(workflowBranch, /SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow[\s\S]*Session\.Item\("GA_IDUSUARIOGESTION"\) = id_usuario_gestion_wf/);
});

test('orquestador persiste exclusivamente por pasos modernos y una invocacion del adaptador', () => {
  assert.match(orchestrator, /IList\(Of IImportExecutionStep\)/);
  assert.match(orchestrator, /For Each stepItem In _steps/);
  assert.doesNotMatch(orchestrator, /ClassAlmacenamiento|AlmacenaDocumentoTareaWorkflow/);
  assert.match(port, /Interface IImportDocumentStoragePort/);
  assert.equal((src.match(/\.AlmacenaDocumentoTareaWorkflow\(/g) || []).length, 1);
});

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const source = fs.readFileSync(
  path.resolve(__dirname, '..', 'workflow', 'ClassAlmacenamiento.vb'),
  'utf8'
);

function sliceBetween(startMarker, endMarker) {
  const start = source.indexOf(startMarker);
  const end = source.indexOf(endMarker, start + startMarker.length);
  assert.ok(start >= 0, `No se encontró ${startMarker}`);
  assert.ok(end > start, `No se encontró ${endMarker}`);
  return source.slice(start, end);
}

test('ADJUNTARADICACION entrega el radicado del estado al prealmacenamiento', () => {
  const branch = sliceBetween(
    'If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ADJUNTARADICACION" Then',
    'If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "PRODUCCION" Then'
  );

  assert.match(
    branch,
    /PreAlmacenaDocumentosRadicacion\([\s\S]*StruDatosImageLista,\s*RadicadoRegistroSeleccionado\)/
  );
});

test('el radicado del estado es únicamente fallback de una resolución vacía', () => {
  const method = sliceBetween(
    'Function PreAlmacenaDocumentosRadicacion(',
    'Function PreAlmacenaDocumentoProduccion('
  );
  const resolver = method.indexOf('Select Case StructureDatosTareaWorkflow.FLUJO_INTERNO_WF');
  const fallback = method.indexOf('If String.IsNullOrWhiteSpace(Radicado) AndAlso Not String.IsNullOrWhiteSpace(ConsecutivoRadicadoEstado) Then');
  const storage = method.indexOf('Result = AlmacenaDocumentosRadicacion(');

  assert.match(method, /Optional ByVal ConsecutivoRadicadoEstado As String = ""/);
  assert.ok(resolver >= 0 && resolver < fallback, 'el mecanismo histórico debe ejecutarse primero');
  assert.ok(fallback < storage, 'el fallback debe aplicarse antes del almacenamiento');
  assert.match(method, /Radicado = ConsecutivoRadicadoEstado\.Trim\(\)/);
});

test('los demás tipos de carga no suministran el fallback específico', () => {
  const upload = sliceBetween(
    'Function UploadSaveFile(',
    'Function Solicita_datos_estructura_tipo_documento_lista_chequeo('
  );
  const branchStart = upload.indexOf('If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ADJUNTARADICACION" Then');
  const branchEnd = upload.indexOf('If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "PRODUCCION" Then', branchStart);
  const withoutRadicacionBranch = upload.slice(0, branchStart) + upload.slice(branchEnd);

  assert.ok(branchStart >= 0 && branchEnd > branchStart);
  assert.doesNotMatch(withoutRadicacionBranch, /RadicadoRegistroSeleccionado/);
});

test('la carga de Radicación Simplificada viaja ligada a su selección y valida inconsistencias', () => {
  const page = fs.readFileSync(
    path.resolve(__dirname, '..', 'js', 'RadicadorSimplificado', 'Web_form_radicacion_simpilificada.js'),
    'utf8'
  );
  const uploader = fs.readFileSync(
    path.resolve(__dirname, '..', 'generic_control', 'FileUploadHandler.js'),
    'utf8'
  );
  const handler = fs.readFileSync(
    path.resolve(__dirname, '..', 'generic_control', 'FileUploadHandler_.ashx.vb'),
    'utf8'
  );
  const attachmentStart = handler.indexOf('If evento_adjunta = "ADJUNTARADICACION" Then');
  const attachmentEnd = handler.indexOf('If evento_adjunta = "PRODUCCION" Then', attachmentStart);
  const attachmentBranch = handler.slice(attachmentStart, attachmentEnd);
  const responseStart = handler.indexOf('If evento_adjunta = "GESTION_RESPUESTA" Then');
  const responseEnd = handler.indexOf('If evento_adjunta = "WORKFLOWSELECCION" Then', responseStart);
  const responseBranch = handler.slice(responseStart, responseEnd);

  assert.match(page, /const RegistroEstadoSeleccionado = CONST_STRU_RAD_ASIN\[0\]\.stru_registro_estado/);
  assert.match(page, /IdRegistroEstadoRadicacion:\s*CONST_ID_REGISTRO_ESTADO/);
  assert.match(page, /RadicadoRadicacion:\s*String\(RadicadoSeleccionado\)\.trim\(\)/);
  assert.match(page, /No existe un contexto válido del radicado seleccionado/);
  assert.match(uploader, /evento_adjunta === "ADJUNTARADICACION"[\s\S]*id_registro_estado_radicacion[\s\S]*radicado_radicacion/);
  assert.ok(attachmentStart >= 0 && attachmentEnd > attachmentStart);
  assert.ok(responseStart >= 0 && responseEnd > responseStart);
  assert.match(attachmentBranch, /UploadSaveFile\([\s\S]*IdRegistroEstadoRadicacion,\s*RadicadoRadicacion\)/);
  assert.doesNotMatch(responseBranch, /IdRegistroEstadoRadicacion|RadicadoRadicacion/);
  assert.doesNotMatch(
    handler.slice(0, attachmentStart) + handler.slice(attachmentEnd),
    /UploadSaveFile\([\s\S]{0,1200}IdRegistroEstadoRadicacion,\s*RadicadoRadicacion\)/
  );
  assert.match(source, /String\.Equals\(RadicadoRadicacion\.Trim\(\), StruRegistroEstado\.consecutivo_radicado\.Trim\(\), StringComparison\.Ordinal\)/);
  assert.match(source, /El registro seleccionado no contiene un radicado válido para adjuntar el documento/);
});

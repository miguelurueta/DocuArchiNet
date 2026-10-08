const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const storageSource = fs.readFileSync(path.join(root, 'workflow', 'ClassAlmacenamiento.vb'), 'utf8');
const cabinetSource = fs.readFileSync(path.join(root, 'Docuarchi', 'ClassDaGabinete.vb'), 'utf8');

function sliceBetween(source, startMarker, endMarker) {
  const start = source.indexOf(startMarker);
  const end = source.indexOf(endMarker, start + startMarker.length);
  assert.ok(start >= 0, `No se encontró ${startMarker}`);
  assert.ok(end > start, `No se encontró ${endMarker}`);
  return source.slice(start, end);
}

test('caracteriza el defecto: el resolver histórico acepta DAT_ADIC_TAR vacío y la plantilla consume el valor', () => {
  const datAdic = fs.readFileSync(path.join(root, 'workflow', 'Class_DAT_ADIC_TAR.vb'), 'utf8');
  const legacyResolver = sliceBetween(
    datAdic,
    'Function SolicitaRadicadoTareaWorkflow(',
    'Function Solicita_radicado_id_tarea_seleccionada('
  );
  const legacyBuilder = sliceBetween(
    cabinetSource,
    'Function SolicitaDatosCamposIndiceGabinete(',
    'Function ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow('
  );

  assert.match(legacyResolver, /Rows\.Count = 0[\s\S]*RadicadoTarea = ""[\s\S]*SolicitaRadicadoTareaWorkflow = "YES"/);
  assert.match(legacyResolver, /IsNull\(0\)[\s\S]*RadicadoTarea = ""/);
  assert.match(legacyBuilder, /SolicitaRadicadoTareaWorkflow\([\s\S]*Radicado\)/);
  assert.match(legacyBuilder, /SolicitaNombrePlantillaRadicado\(Radicado,/);
});

test('ADJUNTARADICACION usa servicio y preparación con contexto autoritativo', () => {
  const branch = sliceBetween(
    storageSource,
    'Function UploadSaveFile(ByVal IdExpediente As Integer,',
    'Function UploadSaveFile(ByVal IdExpediente As Integer,'
  );

  assert.match(branch, /IdRegistroEstadoRadicacion As Long/);
  assert.match(branch, /"ADJUNTARADICACION"/);
  assert.match(branch, /ServicioAdjuntoRadicacion/);
  assert.match(branch, /PreAlmacenaDocumentosRadicacionConContexto/);
  assert.doesNotMatch(branch, /RadicadoRegistroSeleccionado\s*=\s*RadicadoRadicacion/);
  assert.doesNotMatch(branch, /SolicitaDatosEstructuraEstadoRadicado/);
});

test('la preparación específica no redescubre el radicado desde DAT_ADIC_TAR', () => {
  const method = sliceBetween(
    storageSource,
    'Function PreAlmacenaDocumentosRadicacionConContexto(',
    'Function PreAlmacenaDocumentoProduccion('
  );
  const constructor = sliceBetween(
    cabinetSource,
    'Function ConstruirDatosCamposIndiceGabineteConRadicado(',
    'Function ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow('
  );

  assert.match(method, /ContextoAdjuntoRadicacion/);
  assert.match(method, /ConstruirDatosCamposIndiceGabineteConRadicado\(/);
  assert.equal((method.match(/AlmacenaDocumentosRadicacion\(/g) || []).length, 1);
  assert.doesNotMatch(method, /SolicitaRadicadoTareaWorkflow/);
  assert.doesNotMatch(constructor, /SolicitaRadicadoTareaWorkflow/);
  assert.match(constructor, /String\.IsNullOrWhiteSpace\(Radicado\)/);
});

test('el recorrido legacy admite el radicado autoritativo clásico sin alterar la preparación específica DOC-85', () => {
  const legacy = sliceBetween(
    storageSource,
    'Function PreAlmacenaDocumentosRadicacion(',
    'Function PreAlmacenaDocumentosRadicacionConContexto('
  );

  assert.match(legacy, /SolicitaDatosCamposIndiceGabinete\(/);
  assert.match(legacy, /Optional ByVal ConsecutivoRadicadoEstado As String = ""/);
  assert.match(legacy, /Not String\.IsNullOrWhiteSpace\(ConsecutivoRadicadoEstado\)[\s\S]*Radicado = ConsecutivoRadicadoEstado\.Trim\(\)/);
});

test('la carga conserva el transporte y la proyección JavaScript existentes', () => {
  const page = fs.readFileSync(path.join(root, 'js', 'RadicadorSimplificado', 'Web_form_radicacion_simpilificada.js'), 'utf8');
  const uploader = fs.readFileSync(path.join(root, 'generic_control', 'FileUploadHandler.js'), 'utf8');
  const handler = fs.readFileSync(path.join(root, 'generic_control', 'FileUploadHandler_.ashx.vb'), 'utf8');
  const attachmentBranch = sliceBetween(
    handler,
    'If evento_adjunta = "ADJUNTARADICACION" Then',
    'If evento_adjunta = "PRODUCCION" Then'
  );
  const responseBranch = sliceBetween(
    handler,
    'If evento_adjunta = "GESTION_RESPUESTA" Then',
    'If evento_adjunta = "WORKFLOWSELECCION" Then'
  );

  assert.match(page, /const RegistroEstadoSeleccionado = CONST_STRU_RAD_ASIN\[0\]\.stru_registro_estado/);
  assert.match(page, /IdRegistroEstadoRadicacion:\s*CONST_ID_REGISTRO_ESTADO/);
  assert.match(page, /RadicadoRadicacion:\s*String\(RadicadoSeleccionado\)\.trim\(\)/);
  assert.match(uploader, /evento_adjunta === "ADJUNTARADICACION"[\s\S]*id_registro_estado_radicacion[\s\S]*radicado_radicacion/);
  assert.match(attachmentBranch, /UploadSaveFile\([\s\S]*IdRegistroEstadoRadicacion,\s*RadicadoRadicacion\)/);
  assert.doesNotMatch(responseBranch, /IdRegistroEstadoRadicacion|RadicadoRadicacion/);
  assert.doesNotMatch(page, /location\.reload|__doPostBack|\.DataBind\(/);
});

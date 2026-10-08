const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');

function sliceBetween(source, startMarker, endMarker) {
  const start = source.indexOf(startMarker);
  const end = source.indexOf(endMarker, start + startMarker.length);
  assert.ok(start >= 0, `No se encontró ${startMarker}`);
  assert.ok(end > start, `No se encontró ${endMarker}`);
  return source.slice(start, end);
}

function countTopLevelArguments(call) {
  const start = call.indexOf('(');
  let depth = 1;
  let inString = false;
  let commas = 0;
  for (let index = start + 1; index < call.length; index += 1) {
    const char = call[index];
    if (char === '"') inString = !inString;
    else if (!inString && char === '(') depth += 1;
    else if (!inString && char === ')') {
      depth -= 1;
      if (depth === 0) return commas + 1;
    } else if (!inString && char === ',' && depth === 1) commas += 1;
  }
  assert.fail('La llamada no está balanceada');
}

test('Radicación Entrante declara un origen clásico explícito sin identidad de navegador', () => {
  const source = read('js', 'radicacion', 'WebFormRadicacionEntrante.js');
  const method = sliceBetween(source, 'const ActivaAdjuntarDocumentoRadicacion', 'function rezize_event');
  const page = read('radicador', 'WebFormRadicacionEntrante.aspx');

  assert.match(method, /NameLoadProceso:\s*"ADJUNTARADICACION"/);
  assert.match(method, /evento_adjunta:\s*"ADJUNTARADICACION_CLASICA"/);
  assert.match(method, /funcion_name:\s*"insert_row_documento_relacionado"/);
  assert.doesNotMatch(method, /IdRegistroEstadoRadicacion|RadicadoRadicacion/);
  assert.match(page, /WebFormRadicacionEntrante\.js\?v=20261007-doc90-3/);
});

test('sin radicado asignado muestra Recepción y bloquea Soporte aunque existan pendientes', () => {
  const source = read('js', 'radicacion', 'WebFormRadicacionEntrante.js');
  const initialization = sliceBetween(source, 'function inicio_tab_radicador()', 'function tab_sow');
  const noAssignment = initialization.slice(initialization.indexOf('} else {'));

  assert.match(noAssignment, /tab_sow\('home_radic', 'home-radicador'\)/);
  assert.match(noAssignment, /tab_enabled\('home-radicador'\)/);
  assert.match(noAssignment, /tab_disable\('soporte-envio_nav'\)/);
  assert.match(noAssignment, /show_tab_boton_content_radicado\(\)/);
  assert.doesNotMatch(noAssignment, /Hidden_numero_rad_pend|show_tab_boton_content_gestion_radicado/);
  assert.match(initialization, /Hidden_radicado_seleccion/);
  assert.match(initialization, /if \(radicadoAsignado !== ""\)/);
  assert.doesNotMatch(initialization, /HiddenIdFlujo|tareaAsignada|Hidden_numero_rad_pend/);
});

test('las transiciones de asignación controlan Soporte sin depender del contador pendiente', () => {
  const source = read('js', 'radicacion', 'WebFormRadicacionEntrante.js');
  const newRecord = sliceBetween(source, 'function nuevo_radicado_tab()', 'function show_tab_boton_content_radicado');
  const assigned = sliceBetween(source, 'function asig_radicado_tab()', 'function terminar_radicado_tab');
  const finished = sliceBetween(source, 'function terminar_radicado_tab()', 'function event_click');

  for (const transition of [newRecord, finished]) {
    assert.match(transition, /tab_sow\('home_radic', 'home-radicador'\)/);
    assert.match(transition, /tab_disable\('soporte-envio_nav'\)/);
    assert.doesNotMatch(transition, /Hidden_numero_rad_pend/);
  }
  assert.match(assigned, /tab_sow\('soporte_envio', 'soporte-envio_nav'\)/);
  assert.match(assigned, /tab_enabled\('soporte-envio_nav'\)/);
  assert.match(assigned, /tab_disable\('home-radicador'\)/);
  assert.doesNotMatch(assigned, /Hidden_radicado_seleccion|HiddenIdFlujo|Hidden_numero_rad_pend|nuevo_radicado_tab\(\)/);
});

test('la selección de pestaña es idempotente y conserva atributos accesibles', () => {
  const source = read('js', 'radicacion', 'WebFormRadicacionEntrante.js');
  const tabs = sliceBetween(source, 'function tab_sow', 'function nuevo_radicado_tab');

  assert.match(tabs, /removeClass\('active show'\)/);
  assert.match(tabs, /addClass\('active show'\)/);
  assert.match(tabs, /attr\('aria-selected', 'false'\)/);
  assert.match(tabs, /attr\('aria-selected', 'true'\)/);
  assert.match(tabs, /addClass\('disabled'\)\.attr\('aria-disabled', 'true'\)/);
  assert.match(tabs, /removeClass\('disabled'\)\.attr\('aria-disabled', 'false'\)/);
  assert.doesNotMatch(tabs, /toggleClass/);
});

test('el acceso a radicados pendientes permanece fuera del soporte documental', () => {
  const page = read('radicador', 'WebFormRadicacionEntrante.aspx');
  const pendingPanel = page.indexOf('ID="Panel_pendiente_radicado"');
  const tabContent = page.indexOf('class="tab-content" id="tab_content"');
  const supportPane = page.indexOf('id="soporte_envio"');

  assert.ok(pendingPanel > 0, 'falta Panel_pendiente_radicado');
  assert.ok(pendingPanel < tabContent, 'Pendientes debe estar en el encabezado común');
  assert.ok(pendingPanel < supportPane, 'Pendientes no puede depender de la pestaña Soporte');
  assert.equal((page.match(/id="A1"/g) || []).length, 1);
  assert.match(page.slice(pendingPanel, tabContent), /Button_tool_lista_pendientes_radicados/);
  assert.match(page.slice(pendingPanel, tabContent), /ID="Label_numero_item"/);
  assert.match(page.slice(0, tabContent), /ID="UpdatePanel_pendientes_radicacion"[^>]*UpdateMode="Always"[^>]*RenderMode="Inline"/);
  assert.match(page.slice(pendingPanel, tabContent), /id="Hidden_numero_rad_pend"/);
});

test('los estados mutados por asignación pertenecen a paneles del postback parcial', () => {
  const page = read('radicador', 'WebFormRadicacionEntrante.aspx');
  const toolPanel = sliceBetween(page, 'ID="UpdatePanel_boton_tool"', '</asp:UpdatePanel>');
  const pendingPanel = sliceBetween(page, 'ID="UpdatePanel_pendientes_radicacion"', '</asp:UpdatePanel>');
  const server = read('radicador', 'Class_ra_rad_estados_modulo_radicacion.vb');

  assert.match(toolPanel, /id="Hidden_result_boton_tool"/);
  assert.match(toolPanel, /id="HiddenIdFlujo"/);
  assert.match(toolPanel, /id="Hidden_radicado_seleccion"/);
  assert.match(pendingPanel, /ID="Panel_pendiente_radicado"/);
  assert.match(pendingPanel, /ID="Label_numero_item"/);
  assert.match(pendingPanel, /id="Hidden_numero_rad_pend"/);
  assert.equal((page.match(/id="Hidden_radicado_seleccion"/g) || []).length, 1);
  assert.equal((page.match(/id="Hidden_numero_rad_pend"/g) || []).length, 1);
  assert.match(server, /HiddenIdFlujo\.Value = stru_registro_estado\.id_tarea_workflow[\s\S]*ref_UpdatePanel_boton_tool\.Update\(\)[\s\S]*Hidden_radicado_seleccion\.Value = stru_registro_estado\.consecutivo_radicado/);
});

test('el handler separa las firmas clásica y Simplificada y conserva la respuesta clásica', () => {
  const source = read('generic_control', 'FileUploadHandler_.ashx.vb');
  const classic = sliceBetween(
    source,
    'If evento_adjunta = "ADJUNTARADICACION_CLASICA" Then',
    'If evento_adjunta = "ADJUNTARADICACION" Then'
  );
  const simplified = sliceBetween(
    source,
    'If evento_adjunta = "ADJUNTARADICACION" Then',
    'If evento_adjunta = "PRODUCCION" Then'
  );
  const classicCall = classic.slice(classic.indexOf('.UploadSaveFile('));
  const simplifiedCall = simplified.slice(simplified.indexOf('.UploadSaveFile('));

  assert.match(classic, /WF_TIPO_ADJUNTA"\) = "ADJUNTARADICACION_CLASICA"/);
  assert.equal((classic.match(/\.UploadSaveFile\(/g) || []).length, 1);
  assert.equal(countTopLevelArguments(classicCall), 10);
  assert.doesNotMatch(classicCall, /IdRegistroEstadoRadicacion|RadicadoRadicacion/);
  assert.equal(countTopLevelArguments(simplifiedCall), 12);
  assert.match(simplifiedCall, /IdRegistroEstadoRadicacion,\s*RadicadoRadicacion\)/);

  for (const field of [
    'name_gabinete', 'id_image', 'radicado', 'tipodocumental',
    'notitipodocumental', 'id_tarea_workflow', 'estado_firma_digital',
    'contador_paginas', 'icono_icono_awe_some', 'id_registro', 'fecha',
    'aleas', 'nombre_archivo'
  ]) {
    assert.match(classic, new RegExp(`uploadFiles\\.${field}\\s*=`), `falta ${field}`);
  }
  assert.match(classic, /uploadFiles\.error_sistema = "YES"/);
});

test('la ruta clásica valida la sesión y la estructura antes de un único prealmacenamiento', () => {
  const source = read('workflow', 'ClassAlmacenamiento.vb');
  const classic = sliceBetween(
    source,
    'If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ADJUNTARADICACION_CLASICA" Then',
    'If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "PRODUCCION" Then'
  );
  const preStore = classic.indexOf('PreAlmacenaDocumentosRadicacion(');

  assert.match(classic, /RA_MODULO_SELECCIONADO/);
  assert.match(classic, /"RADICACION"[\s\S]*"RADICACION ENTRANTE"/);
  assert.match(classic, /RA_ID_PLANTILLA_RADICADO_SELECCIONADO/);
  assert.match(classic, /Long\.TryParse\(Convert\.ToString\(HttpContext\.Current\.Session\.Item\("RA_ID_REGISTRO_RADICADO"\)\)/);
  assert.ok(classic.indexOf('El módulo seleccionado no corresponde') < preStore);
  assert.ok(classic.indexOf('IdPlantillaSeleccionada <= 0') < preStore);
  assert.ok(classic.indexOf('IdRegistroEstadoSeleccionado <= 0') < preStore);
  assert.ok(classic.indexOf('SolicitaDatosEstructuraEstadoRadicado') < preStore);
  assert.ok(classic.indexOf('system_plantilla_radicado_id_Plantilla <> IdPlantillaSeleccionada') < preStore);
  assert.ok(classic.indexOf('String.IsNullOrWhiteSpace(StruRegistroEstado.consecutivo_radicado)') < preStore);
  assert.ok(classic.indexOf('StruRegistroEstado.id_tarea_workflow <= 0') < preStore);
  assert.ok(classic.indexOf('StruRegistroEstado.tipo_doc_entrante_id_Tipo_Doc_Entrante <= 0') < preStore);
  assert.equal((classic.match(/PreAlmacenaDocumentosRadicacion\(/g) || []).length, 1);
  assert.match(classic, /StruRegistroEstado\.consecutivo_radicado,\s*IdPlantillaSeleccionada\)/);
  assert.match(classic, /IdTareaWorkflow = StruRegistroEstado\.id_tarea_workflow[\s\S]*Return "YES"/);
  assert.doesNotMatch(classic, /IdRegistroEstadoRadicacion|RadicadoRadicacion/);
});

test('el radicado clásico resuelto prevalece y Simplificada conserva su preparación autoritativa', () => {
  const source = read('workflow', 'ClassAlmacenamiento.vb');
  const legacy = sliceBetween(
    source,
    'Function PreAlmacenaDocumentosRadicacion(',
    'Function PreAlmacenaDocumentosRadicacionConContexto('
  );
  const simplified = sliceBetween(
    source,
    'Function PreAlmacenaDocumentosRadicacionConContexto(',
    'Function PreAlmacenaDocumentoProduccion('
  );

  assert.match(legacy, /Optional ByVal ConsecutivoRadicadoEstado As String = ""/);
  assert.match(legacy, /Optional ByVal IdPlantillaRadicadoEstado As Long = 0/);
  assert.match(legacy, /TieneRadicadoEstado As Boolean = Not String\.IsNullOrWhiteSpace\(ConsecutivoRadicadoEstado\)[\s\S]*Radicado = ConsecutivoRadicadoEstado\.Trim\(\)/);
  assert.match(legacy, /If TieneRadicadoEstado Then[\s\S]*ConstruirDatosCamposIndiceGabineteConRadicado\([\s\S]*Radicado,[\s\S]*CInt\(IdPlantillaRadicadoEstado\)[\s\S]*Else[\s\S]*SolicitaDatosCamposIndiceGabinete\(/);
  assert.match(simplified, /ContextoAdjuntoRadicacion/);
  assert.doesNotMatch(simplified, /ConsecutivoRadicadoEstado|RA_ID_REGISTRO_RADICADO/);
});

test('la proyección clásica depende de funcion_name y no fuerza postback ni recarga', () => {
  const uploader = read('generic_control', 'FileUploadHandler.js');
  const projection = sliceBetween(
    uploader,
    'if (this.settings.funcion_name == "insert_row_documento_relacionado")',
    '//--------Inserta registro interfaz workflow tarea asiganda y enlace------////'
  );

  assert.match(projection, /insert_row_documento_relacionado\(date_campo, UploadFilesResult, 1\)/);
  assert.doesNotMatch(projection, /evento_adjunta|location\.reload|__doPostBack|\.DataBind\(/);
});

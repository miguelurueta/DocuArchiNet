const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const read = relativePath => fs.readFileSync(path.join(root, ...relativePath.split('/')), 'utf8');

function extractCalls(source) {
  const calls = [];
  const marker = '.UploadSaveFile(';
  let cursor = 0;
  while ((cursor = source.indexOf(marker, cursor)) >= 0) {
    let depth = 1;
    let inString = false;
    let commas = 0;
    let index = cursor + marker.length;
    for (; index < source.length && depth > 0; index += 1) {
      const char = source[index];
      if (char === '"') inString = !inString;
      else if (!inString && char === '(') depth += 1;
      else if (!inString && char === ')') depth -= 1;
      else if (!inString && char === ',' && depth === 1) commas += 1;
    }
    assert.equal(depth, 0, 'La llamada UploadSaveFile debe estar balanceada');
    calls.push({ offset: cursor, arity: commas + 1 });
    cursor = index;
  }
  return calls;
}

test('el handler conserva los catorce eventos y agrega únicamente el origen clásico', () => {
  const source = read('generic_control/FileUploadHandler_.ashx.vb');
  const events = Array.from(
    source.matchAll(/If evento_adjunta = "([A-Z_]+)" Then/g),
    match => match[1]
  );

  assert.deepEqual(events, [
    'GESTION_PQRS', 'ADJUNTAVERSION', 'REMPLAZAVERSION', 'INTRUESII',
    'INTVIRTUALSII', 'MIGRACION', 'GESTION_RESPUESTA', 'WORKFLOWSELECCION',
    'WORKFLOWENLACE', 'ADJUNTARADICACION_CLASICA', 'ADJUNTARADICACION',
    'PRODUCCION', 'SUBE_RESPUESTA', 'SUBE_ANEXO', 'RADICA_WORKFLOW'
  ]);
});

test('las llamadas existentes conservan propietario y aridad; solo se agrega la clásica legacy', () => {
  const relativePath = 'generic_control/FileUploadHandler_.ashx.vb';
  const source = read(relativePath);
  const eventMarkers = Array.from(
    source.matchAll(/If evento_adjunta = "([A-Z_]+)" Then/g),
    match => ({ event: match[1], offset: match.index })
  );
  const handlerCalls = extractCalls(source).map(call => {
    const owner = eventMarkers.filter(marker => marker.offset < call.offset).at(-1);
    return { event: owner.event, arity: call.arity };
  });

  assert.deepEqual(handlerCalls, [
    { event: 'GESTION_RESPUESTA', arity: 10 },
    { event: 'WORKFLOWSELECCION', arity: 10 },
    { event: 'WORKFLOWENLACE', arity: 10 },
    { event: 'ADJUNTARADICACION_CLASICA', arity: 10 },
    { event: 'ADJUNTARADICACION', arity: 12 },
    { event: 'PRODUCCION', arity: 10 },
    { event: 'RADICA_WORKFLOW', arity: 10 }
  ]);
  assert.deepEqual(extractCalls(read('workflow/Webworkflow.aspx.vb')).map(call => call.arity), [10]);
  assert.deepEqual(extractCalls(read('webservice/WebServiceRadicacion.asmx.vb')).map(call => call.arity), [10]);
});

test('los consumidores JavaScript mantienen identidades explícitas por módulo', () => {
  const expected = [
    ['js/radicacion/WebFormRadicacionEntrante.js', 'ADJUNTARADICACION_CLASICA'],
    ['js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js', 'ADJUNTARADICACION'],
    ['js/versiondocumento/gestion_version_documento.js', 'ADJUNTAVERSION'],
    ['js/versiondocumento/gestion_version_documento.js', 'REMPLAZAVERSION'],
    ['js/gestion/WebFormProducionDocumental.js', 'PRODUCCION'],
    ['js/workflow/WebFormGestionFlujoTrabajoCamaras.js', 'INTRUESII'],
    ['js/workflow/WebFormGestionFlujoTrabajoCamaras.js', 'INTVIRTUALSII'],
    ['js/workflow/Webworkflow.js', 'WORKFLOWENLACE'],
    ['js/workflow/Webworkflow.js', 'WORKFLOWSELECCION']
  ];

  for (const [relativePath, event] of expected) {
    assert.match(read(relativePath), new RegExp(`evento_adjunta:\\s*"${event}"`), `${relativePath}: ${event}`);
  }
});

test('las ramas clásica y Simplificada conservan fronteras de sesión independientes', () => {
  const source = read('generic_control/FileUploadHandler_.ashx.vb');
  const classicStart = source.indexOf('If evento_adjunta = "ADJUNTARADICACION_CLASICA" Then');
  const simplifiedStart = source.indexOf('If evento_adjunta = "ADJUNTARADICACION" Then', classicStart);
  const productionStart = source.indexOf('If evento_adjunta = "PRODUCCION" Then', simplifiedStart);
  const classic = source.slice(classicStart, simplifiedStart);
  const simplified = source.slice(simplifiedStart, productionStart);

  assert.match(classic, /WF_TIPO_ADJUNTA"\) = "ADJUNTARADICACION_CLASICA"/);
  assert.doesNotMatch(classic, /IdRegistroEstadoRadicacion|RadicadoRadicacion/);
  assert.match(simplified, /WF_TIPO_ADJUNTA"\) = "ADJUNTARADICACION"/);
  assert.match(simplified, /IdRegistroEstadoRadicacion,\s*RadicadoRadicacion\)/);
});

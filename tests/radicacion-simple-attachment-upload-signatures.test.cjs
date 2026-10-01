const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const consumers = [
  'generic_control/FileUploadHandler_.ashx.vb',
  'workflow/Webworkflow.aspx.vb',
  'webservice/WebServiceRadicacion.asmx.vb'
];

function extractCalls(source) {
  const calls = [];
  const marker = '.UploadSaveFile(';
  let cursor = 0;
  while ((cursor = source.indexOf(marker, cursor)) >= 0) {
    const start = cursor + marker.length;
    let depth = 1;
    let inString = false;
    let commas = 0;
    let index = start;
    for (; index < source.length && depth > 0; index += 1) {
      const char = source[index];
      if (char === '"') {
        if (inString && source[index + 1] === '"') index += 1;
        else inString = !inString;
      } else if (!inString) {
        if (char === '(') depth += 1;
        else if (char === ')') depth -= 1;
        else if (char === ',' && depth === 1) commas += 1;
      }
    }
    assert.equal(depth, 0, 'La llamada UploadSaveFile debe estar balanceada');
    calls.push({ arity: commas + 1, text: source.slice(cursor, index), offset: cursor });
    cursor = index;
  }
  return calls;
}

test('el inventario conserva siete consumidores legacy y uno exclusivo de Radicación Simplificada', () => {
  const calls = consumers.flatMap((relativePath) => {
    const source = fs.readFileSync(path.join(root, ...relativePath.split('/')), 'utf8');
    return extractCalls(source).map((call) => ({ ...call, relativePath, source }));
  });
  const legacy = calls.filter((call) => call.arity === 10);
  const radicacion = calls.filter((call) => call.arity === 12);

  assert.equal(calls.length, 8);
  assert.equal(legacy.length, 7);
  assert.equal(radicacion.length, 1);
  assert.equal(radicacion[0].relativePath, 'generic_control/FileUploadHandler_.ashx.vb');
  const branchStart = radicacion[0].source.lastIndexOf('If evento_adjunta = "ADJUNTARADICACION" Then', radicacion[0].offset);
  const branchEnd = radicacion[0].source.indexOf('If evento_adjunta = "PRODUCCION" Then', branchStart);
  assert.ok(branchStart >= 0 && radicacion[0].offset > branchStart && radicacion[0].offset < branchEnd);
});

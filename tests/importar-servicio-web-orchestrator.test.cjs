const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const src = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');

test('autoriza, carga y verifica version antes de los pasos', () => {
  const validate = src.indexOf('_validator.Validar');
  const load = src.indexOf('_repository.Obtener');
  const version = src.indexOf('VERSION_CONFLICT');
  const loop = src.indexOf('For Each stepItem In _steps');
  assert.ok(validate >= 0 && validate < load && load < version && version < loop);
});

test('procesa items y pasos con bucles secuenciales', () => {
  assert.match(src, /For Each item In intent\.Resultados[\s\S]*For Each stepItem In _steps/);
  assert.doesNotMatch(src, /Task\.WhenAll|Parallel\.|AsParallel/);
});

test('detencion conserva confirmados y no inicia pasos pendientes', () => {
  assert.match(src, /If intent\.DetencionSolicitada Then[\s\S]*Continue For/);
  assert.match(src, /StopRequested/);
});

test('Get mapea solo la instantanea obtenida del repositorio', () => {
  const getBlock = src.slice(src.indexOf('Public Function [Get]'));
  assert.match(getBlock, /_repository\.Obtener/);
  assert.doesNotMatch(getBlock, /HttpContext|Session|IExternalImportProvider/);
});

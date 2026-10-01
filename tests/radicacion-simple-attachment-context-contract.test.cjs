const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');

const root = path.resolve(__dirname, '..');

function source(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), 'utf8');
}

test('el repository limita la resolución al estado y usuario autorizados con parámetros', () => {
  const repository = source('Infrastructure/Repositories/RadicacionSimplificada/Adjuntos/MySqlContextoAdjuntoRadicacionRepository.vb');
  assert.match(repository, /WHERE id_estado_radicado = @idRegistroEstado/);
  assert.match(repository, /AND id_usuario_radicado = @idUsuarioRadicacion LIMIT 1/);
  assert.match(repository, /Parametro\("@idRegistroEstado", idRegistroEstado\)/);
  assert.match(repository, /Parametro\("@idUsuarioRadicacion", contextoModulo\.IdUsuario\)/);
  assert.equal((repository.match(/ExecuteReader\(Of ContextoAdjuntoRadicacion\)/g) || []).length, 1);
  assert.doesNotMatch(repository, /SELECT[\s\S]*(?:&|\+)\s*idRegistroEstado/);
  assert.doesNotMatch(repository, /LegacyRadicacionConnectionFactory|Public Sub New\(\s*\)/);
});

test('la composición reutiliza la factoría y el ejecutor compartidos de Radicación', () => {
  const storage = source('workflow/ClassAlmacenamiento.vb');
  const sessionGate = source('webservice/WorkflowPreviewSessionContextGate.vb');
  const resolver = source('Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb');

  assert.match(storage, /ModuleSessionConnectionStringResolver\.Resolve\(HttpContext\.Current, "RA_"\)/);
  assert.match(storage, /New RadicacionModuleConnectionFactory\(CadenaConexionRadicacion\)/);
  assert.match(storage, /New AdoNetDataExecutor\(\)/);
  assert.match(sessionGate, /ModuleSessionConnectionStringResolver\.Resolve\(requestContext, "RA_"\)/);
  assert.match(resolver, /Public Shared Function Resolve/);
  assert.doesNotMatch(resolver, /connection\.Open|Returna_Conexion_Mysql/);
});

test('repository y servicio reales ejecutan casos negativos, resolución única y aislamiento', () => {
  const compilerLookup = spawnSync('where.exe', ['vbc.exe'], { encoding: 'utf8' });
  assert.equal(compilerLookup.status, 0, 'vbc.exe debe estar disponible para ejecutar el contrato VB real.');
  const compiler = compilerLookup.stdout.split(/\r?\n/).find(Boolean);
  const temp = fs.mkdtempSync(path.join(os.tmpdir(), 'doc85-vb-'));
  const executable = path.join(temp, 'ServicioAdjuntoRadicacionHarness.exe');
  const mysqlAssembly = path.join(root, 'bin', 'MySql.Data.dll');
  const inputs = [
    'Domain/Shared/ContextoModulo.vb',
    'Infrastructure/Shared/Data/ModuleDataContracts.vb',
    'Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb',
    'Modelo/RadicacionSimplificada/Adjuntos/ContextoAdjuntoRadicacion.vb',
    'Modelo/RadicacionSimplificada/Adjuntos/ResultadoAdjuntoRadicacion.vb',
    'Modelo/RadicacionSimplificada/Adjuntos/IContextoAdjuntoRadicacionRepository.vb',
    'Infrastructure/Repositories/RadicacionSimplificada/Adjuntos/MySqlContextoAdjuntoRadicacionRepository.vb',
    'Services/RadicacionSimplificada/Adjuntos/ServicioAdjuntoRadicacion.vb',
    'tests/Fixtures/RadicacionSimplificada/ServicioAdjuntoRadicacionHarness.vb'
  ].map(relativePath => path.join(root, relativePath));

  try {
    fs.copyFileSync(mysqlAssembly, path.join(temp, 'MySql.Data.dll'));
    const compilation = spawnSync(compiler, ['/nologo', '/target:exe', `/out:${executable}`, `/reference:${mysqlAssembly}`, ...inputs], {
      encoding: 'utf8',
      cwd: root
    });
    assert.equal(compilation.status, 0, compilation.stdout || compilation.stderr);

    const execution = spawnSync(executable, [], { encoding: 'utf8', cwd: root });
    assert.equal(execution.status, 0, execution.stdout || execution.stderr);
    assert.match(execution.stdout, /PASS ServicioAdjuntoRadicacion/);
  } finally {
    fs.rmSync(temp, { recursive: true, force: true });
  }
});

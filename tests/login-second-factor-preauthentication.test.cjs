const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const source = path.join(__dirname, 'LoginSecondFactorPreAuthenticationBehaviorTests.cs');
const repoRoot = path.join(root, 'Infrastructure', 'Repositories', 'Login', 'SegundoFactor');
const requiredSources = [
  'Domain\\Shared\\ContextoPreautenticacionModulo.vb',
  'Services\\Login\\SegundoFactor\\SecondFactorPreAuthenticationService.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\OdbcSecondFactorLoginModuleRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorPrincipalRepositoryBase.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlDocuarchiSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlGestorSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlRadicacionSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlWorkflowSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\SecondFactorPrincipalRepositoryResolver.vb'
];

function run(command, args, options = {}) { return execFileSync(command, args, { cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], ...options }); }
function compiler() { return run('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source']).trim(); }
function read(relative) { return fs.readFileSync(path.join(root, ...relative.split('/')), 'utf8'); }

test('DOC-94: contratos, repositorios y decisión cumplen comportamiento net461', () => {
  run('msbuild.exe', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const build = path.join(root, 'bin');
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc94-preauth-'));
  try {
    for (const name of ['GestionDocumental-Docuarchi.net.dll', 'MySql.Data.dll']) fs.copyFileSync(path.join(build, name), path.join(directory, name));
    const executable = path.join(directory, 'LoginSecondFactorPreAuthenticationBehaviorTests.exe');
    run(compiler(), ['/nologo', '/target:exe', `/out:${executable}`, `/reference:${path.join(directory, 'GestionDocumental-Docuarchi.net.dll')}`, '/reference:System.Data.dll', source]);
    assert.match(run(executable, [], { cwd: directory }), /preauthentication behavior tests: passed/);
  } finally { fs.rmSync(directory, { recursive: true, force: true }); }
});

test('DOC-94: proyecto registra fuentes y repositorios respetan límites', () => {
  const projectText = fs.readFileSync(project, 'utf8');
  for (const relative of requiredSources) assert.equal(projectText.split(`Include="${relative}"`).length - 1, 1, `${relative} debe registrarse una vez`);
  const repositories = fs.readdirSync(repoRoot).filter(name => /SecondFactor(?:LoginModule|Principal)/.test(name));
  for (const name of repositories) {
    const text = fs.readFileSync(path.join(repoRoot, name), 'utf8');
    assert.doesNotMatch(text, /HttpContext|System\.Web|\bSession\b|\bconect\b/i, `${name} no debe conocer HTTP/Session`);
  }
  const interfaces = read('Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb');
  assert.match(interfaces, /FinalizeLogin\(ByVal context As LegacyLoginFinalizationContext\) As LegacyLoginFinalizationResult/);
  assert.doesNotMatch(interfaces, /FinalizeLogin\(ByVal context As PendingSecondFactorContext\)/);
});

test('DOC-94: cuatro adaptadores conservan tabla, columnas y parámetro', () => {
  const contracts = [
    ['MySqlDocuarchiSecondFactorPrincipalRepository.vb', 'usuarios_da', 'Clave_Usuario', 'idusuario', 'correo'],
    ['MySqlGestorSecondFactorPrincipalRepository.vb', 'remit_dest_interno', 'Id_Remit_Dest_Int', 'Login_Usuario', 'Correo_Electronico'],
    ['MySqlRadicacionSecondFactorPrincipalRepository.vb', 'usuario_radicador', 'id_usuario', 'Login_usuario', 'Correo_Usuario'],
    ['MySqlWorkflowSecondFactorPrincipalRepository.vb', 'usuario_workflow', 'idU_suario', 'login_Usuario', 'Correo_Usuario']
  ];
  for (const [file, ...symbols] of contracts) {
    const text = fs.readFileSync(path.join(repoRoot, file), 'utf8');
    for (const symbol of symbols) assert.ok(text.includes(symbol), `${file}: falta ${symbol}`);
    assert.match(text, /WHERE\s+\w+=@login\s+LIMIT 2/i);
  }
  const central = fs.readFileSync(path.join(repoRoot, 'OdbcSecondFactorLoginModuleRepository.vb'), 'utf8');
  for (const symbol of ['RequiereSegundoFactor', 'SecondFactorProviderType', 'SegundoFactorTiempoExpira']) assert.ok(central.includes(symbol));
  assert.match(central, /RAZON_SOCIAL_EMPRESA=\?.*NOMBRE_MODULO=\?/i);
});

test('DOC-94: alcance excluye correo, challenge, ASMX y UI', () => {
  const service = read('Services/Login/SegundoFactor/SecondFactorPreAuthenticationService.vb');
  assert.doesNotMatch(service, /ISecondFactorEmailSender|ISecondFactorChallengeRepository|HttpContext|FormsAuthentication|\.aspx|\.asmx/i);
  const projectText = fs.readFileSync(project, 'utf8');
  assert.equal(projectText.includes('WebServiceLoginSegundoFactor'), false);
});

test('DOC-94: finalizador conserva orden de efectos caracterizado por módulo', () => {
  const source = read('Defaul/ClassGestorSesion.vb');
  const start = source.indexOf('Public Function FinalizeLogin(');
  const end = source.indexOf('Private Function CreateSecondFactorPreAuthenticationService', start);
  assert.ok(start >= 0 && end > start, 'Debe existir un único finalizador extraído.');
  const finalizer = source.slice(start, end);
  const effects = JSON.parse(read('tests/Fixtures/Login/SegundoFactor/doc94-legacy-finalization-effects.json'));
  const boundaries = {
    'DOCUARCHI CONTENEDOR': 'If Modulestr = "DOCUARCHI CONTENEDOR" Then',
    'WORKFLOW DOCUMENTAL': 'If Modulestr = "WORKFLOW DOCUMENTAL" Then',
    'RADICACION DOCUMENTAL': 'If Modulestr = "RADICACION DOCUMENTAL" Then',
    'GESTOR DOCUMENTAL': 'If Modulestr = "GESTOR DOCUMENTAL" Then'
  };
  const orderedTypes = Object.keys(boundaries);
  for (let i = 0; i < orderedTypes.length; i++) {
    const type = orderedTypes[i];
    const branchStart = finalizer.indexOf(boundaries[type]);
    const nextStarts = orderedTypes.slice(i + 1).map(next => finalizer.indexOf(boundaries[next])).filter(value => value > branchStart);
    const branchEnd = nextStarts.length ? Math.min(...nextStarts) : finalizer.length;
    const branch = finalizer.slice(branchStart, branchEnd);
    assert.ok(branchStart >= 0, `Falta rama ${type}`);
    let cursor = -1;
    for (const effect of effects[type]) {
      const found = branch.indexOf(effect, cursor + 1);
      assert.ok(found > cursor, `${type}: falta o cambió orden de ${effect}`);
      cursor = found;
    }
  }
  assert.doesNotMatch(finalizer, /FormsAuthentication|RedirectFromLoginPage/);
  const wrapper = source.slice(source.indexOf('Function InicioAplicacionWebGestorDocumental'), start);
  assert.equal((wrapper.match(/FormsAuthentication\.RedirectFromLoginPage/g) || []).length, 1);
  assert.ok(wrapper.indexOf('ValidaUserAplicacion') < wrapper.indexOf('passs = String.Empty'));
  assert.ok(wrapper.indexOf('If Result <> "YES" Then', wrapper.indexOf('ValidaUserAplicacion')) < wrapper.indexOf('passs = String.Empty'), 'Credenciales inválidas salen antes de preautenticación.');
  assert.ok(wrapper.indexOf('passs = String.Empty') < wrapper.indexOf('preAuthentication.Execute'));
});

test('DOC-94: validación legacy mantiene reglas de estado asimétricas', () => {
  const source = read('Defaul/ClassGestorSesion.vb');
  const start = source.indexOf('Function ValidaUserAplicacion(');
  const end = source.indexOf('Function Gestor_Retorna_Detalle_webserice', start);
  const validation = source.slice(start, end);
  for (const type of ['DOCUARCHI CONTENEDOR', 'GESTOR DOCUMENTAL', 'RADICACION DOCUMENTAL', 'WORKFLOW DOCUMENTAL']) assert.ok(validation.includes(type));
  const gestor = validation.slice(validation.indexOf('If Nombre_Aplication = "GESTOR DOCUMENTAL"'), validation.indexOf('If Nombre_Aplication = "RADICACION DOCUMENTAL"'));
  const radicacion = validation.slice(validation.indexOf('If Nombre_Aplication = "RADICACION DOCUMENTAL"'), validation.indexOf('If Nombre_Aplication = "WORKFLOW DOCUMENTAL"'));
  const workflow = validation.slice(validation.indexOf('If Nombre_Aplication = "WORKFLOW DOCUMENTAL"'));
  const docuarchi = validation.slice(validation.indexOf('If Nombre_Aplication = "DOCUARCHI CONTENEDOR"'), validation.indexOf('If Nombre_Aplication = "GESTOR DOCUMENTAL"'));
  assert.match(gestor, /Estado_Usuario/i);
  assert.match(workflow, /ESTADO_USUARIO/);
  assert.doesNotMatch(radicacion, /ESTADO_USUARIO/i);
  assert.doesNotMatch(docuarchi, /ESTADO_USUARIO/i);
});

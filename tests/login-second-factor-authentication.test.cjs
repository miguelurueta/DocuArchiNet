const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const source = path.join(__dirname, 'LoginSecondFactorAuthenticationBehaviorTests.cs');

function run(command, args, options = {}) { return execFileSync(command, args, { cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], ...options }); }
function csharpCompiler() { return run('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source']).trim(); }

test('DOC-95: orquestador cumple inicio, estado, verificación, reenvío y cancelación', () => {
  run('msbuild', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc95-auth-'));
  try {
    const assembly = path.join(root, 'bin', 'GestionDocumental-Docuarchi.net.dll');
    const copied = path.join(directory, path.basename(assembly));
    const executable = path.join(directory, 'LoginSecondFactorAuthenticationBehaviorTests.exe');
    fs.copyFileSync(assembly, copied);
    run(csharpCompiler(), ['/nologo', '/target:exe', `/out:${executable}`, `/reference:${copied}`, source]);
    assert.match(run(executable, [], { cwd: directory }), /authentication behavior tests: passed/);
  } finally { fs.rmSync(directory, { recursive: true, force: true }); }
});

test('DOC-95: servicio no conoce fronteras concretas', () => {
  const service = fs.readFileSync(path.join(root, 'Services', 'Login', 'SegundoFactor', 'SecondFactorAuthenticationService.vb'), 'utf8');
  assert.doesNotMatch(service, /HttpContext|System\.Web|FormsAuthentication|MySql|Odbc|SmtpClient|System\.Net\.Mail|IDbConnection/i);
  assert.equal((service.match(/^\s*Implements ISecondFactorAuthenticationService\s*$/gm) || []).length, 1);
});

test('DOC-95: composition root reutiliza infraestructura y no contiene SQL ni secretos', () => {
  const factory = fs.readFileSync(path.join(root, 'webservice', 'Login', 'SegundoFactor', 'LoginSecondFactorCompositionFactory.vb'), 'utf8');
  for (const symbol of [
    'MySqlSecondFactorChallengeRepository', 'MySqlSecondFactorSmtpConfigurationRepository',
    'SecondFactorSmtpEmailSender', 'HmacSecondFactorCodeProtector',
    'SessionPendingSecondFactorContextStore', 'FormsAuthenticationTicketIssuer',
    'SecondFactorRecipientResolver', 'EventLogSecondFactorDiagnosticSink'
  ]) assert.ok(factory.includes(symbol), `Falta reutilizar ${symbol}`);
  assert.doesNotMatch(factory, /\bSELECT\b|\bINSERT\b|\bUPDATE\b|\bDELETE\b|ClassCorreo|Recuperar_pasword_usuario/i);
  const legacyMail = fs.readFileSync(path.join(root, 'Defaul', 'ClassGestorSesion.vb'), 'utf8');
  assert.ok(legacyMail.includes('Dim refclas_correo As New ClassCorreo'), 'La recuperación legacy permanece presente e independiente.');
});

test('DOC-95: observabilidad acepta solo correlación y catálogos cerrados', () => {
  const interfaces = fs.readFileSync(path.join(root, 'Modelo', 'Login', 'SegundoFactor', 'SegundoFactorInterfaces.vb'), 'utf8');
  const adapter = fs.readFileSync(path.join(root, 'Infrastructure', 'Login', 'SegundoFactor', 'Diagnostics', 'TraceSecondFactorDiagnosticSink.vb'), 'utf8');
  const eventLogAdapter = fs.readFileSync(path.join(root, 'Infrastructure', 'Login', 'SegundoFactor', 'Diagnostics', 'EventLogSecondFactorDiagnosticSink.vb'), 'utf8');
  const signature = /Sub Record\(ByVal correlationId As Guid,\s*ByVal stage As SecondFactorDiagnosticStage,\s*ByVal outcome As SecondFactorDiagnosticOutcome\)/i;
  assert.match(interfaces, signature);
  assert.match(adapter, signature);
  assert.doesNotMatch(adapter, /Exception\.Message|EmailAddress|NormalizedLogin|ProtectedCode|ChallengeId|SessionNonce|connectionString|password/i);
  assert.match(eventLogAdapter, /Private Const EventSource As String = "Application"/);
  assert.match(eventLogAdapter, /outcome = SecondFactorDiagnosticOutcome\.FAILED OrElse\s*outcome = SecondFactorDiagnosticOutcome\.REJECTED/i);
  assert.doesNotMatch(eventLogAdapter, /Exception\.Message|EmailAddress|NormalizedLogin|ProtectedCode|ChallengeId|SessionNonce|connectionString|password/i);
  const projectText = fs.readFileSync(project, 'utf8');
  assert.equal(projectText.split('Include="Infrastructure\\Login\\SegundoFactor\\Diagnostics\\EventLogSecondFactorDiagnosticSink.vb"').length - 1, 1);
});

test('DOC-95: el límite legacy no devuelve mensajes de excepción al cliente', () => {
  const legacy = fs.readFileSync(path.join(root, 'Defaul', 'ClassGestorSesion.vb'), 'utf8');
  assert.doesNotMatch(legacy, /InicioAplicacionWebGestorDocumental\s*=\s*[^\r\n]*ex\.Message/i);
  assert.match(legacy, /InicioAplicacionWebGestorDocumental\s*=\s*"LOGIN_UNAVAILABLE"/i);
});

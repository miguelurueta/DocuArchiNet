const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const sources = [
  'Modelo\\Login\\SegundoFactor\\SegundoFactorSmtpModels.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorSmtpConfigurationRepository.vb',
  'Infrastructure\\Login\\SegundoFactor\\Smtp\\FrameworkSmtpClientAdapter.vb',
  'Infrastructure\\Login\\SegundoFactor\\Smtp\\SecondFactorSmtpTransport.vb',
  'Infrastructure\\Login\\SegundoFactor\\Smtp\\SecondFactorSmtpEmailSender.vb'
];

function read(relative) { return fs.readFileSync(path.join(root, ...relative.split('\\')), 'utf8'); }
function run(command, args, options = {}) {
  return execFileSync(command, args, { cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], ...options });
}
function compiler() {
  return run('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source']).trim();
}

test('DOC-93: rutas, contrato SQL y aislamiento respetan la arquitectura aprobada', () => {
  const projectText = fs.readFileSync(project, 'utf8');
  for (const source of sources) assert.equal(projectText.split(`Include="${source}"`).length - 1, 1, `${source} debe registrarse una vez.`);

  const repository = read(sources[1]);
  assert.match(repository, /SELECT SERV_SMTP, PUERTO_SERV_SMTP, USUARIO_SMTP, PASW_SMTP, DOMINIO_SMTP, SMTP_TIEMPO, ESTADO_SSL, ESTADO_ENVIO, ESTADO_BODY, ESTADO_CREDENCIAL FROM Config_Smpt_Side WHERE ESTADO_ENVIO=@enabled/);
  assert.match(repository, /IModuleConnectionFactory/);
  assert.match(repository, /IDataExecutor/);
  assert.doesNotMatch(repository, /SELECT\s+\*|LIMIT\s+1|HttpContext|System\.Web|\bSession\b|\bconect\b/i);

  const newText = sources.map(read).join('\n');
  assert.doesNotMatch(newText, /ClassCorreo|ClassRaEnvioCorrespondencia|Task\.Run|SendAsync|fire-and-forget/i);
  assert.doesNotMatch(newText, /NetworkCredential\([^\r\n]*Domain|NetworkCredential\([^\r\n]*configuration\.Domain/i);
  assert.doesNotMatch(read('Modelo\\Login\\SegundoFactor\\SegundoFactorSmtpModels.vb'), /System\.Web|MySql|\bSmtpClient\b|\bMailMessage\b/i);
  assert.doesNotMatch(read('Modelo\\Login\\SegundoFactor\\SegundoFactorInterfaces.vb'), /System\.Web|MySql|\bSmtpClient\b|\bMailMessage\b/i);

  const changed = new Set(run('git', ['diff', '--name-only', 'origin/main...HEAD']).trim().split(/\r?\n/).filter(Boolean));
  for (const local of run('git', ['diff', '--name-only']).trim().split(/\r?\n/).filter(Boolean)) changed.add(local);
  assert.equal(changed.has('radicador/ClassCorreo.vb'), false, 'ClassCorreo.vb debe permanecer fuera del diff DOC-93.');
  assert.equal(changed.has('radicador/ClassRaEnvioCorrespondencia.vb'), false, 'El consumidor legacy debe permanecer fuera del diff DOC-93.');
});

test('DOC-93: comportamiento SMTP se valida con dobles y sin red', () => {
  run('msbuild.exe', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const build = path.join(root, 'bin');
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc93-smtp-'));
  try {
    for (const name of ['GestionDocumental-Docuarchi.net.dll', 'MySql.Data.dll']) fs.copyFileSync(path.join(build, name), path.join(directory, name));
    const executable = path.join(directory, 'LoginSecondFactorSmtpBehaviorTests.exe');
    run(compiler(), [
      '/nologo', '/target:exe', `/out:${executable}`,
      `/reference:${path.join(directory, 'GestionDocumental-Docuarchi.net.dll')}`,
      '/reference:System.Data.dll', '/reference:System.dll',
      path.join(__dirname, 'LoginSecondFactorSmtpBehaviorTests.cs')
    ]);
    assert.match(run(executable, [], { cwd: directory }), /login-second-factor SMTP behavior tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

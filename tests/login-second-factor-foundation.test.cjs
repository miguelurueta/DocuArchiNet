const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const source = path.join(__dirname, 'LoginSecondFactorFoundationBehaviorTests.cs');
const requiredSources = [
  'Modelo\\Login\\SegundoFactor\\SegundoFactorModels.vb',
  'Modelo\\Login\\SegundoFactor\\SegundoFactorInterfaces.vb',
  'DTOs\\Login\\SegundoFactor\\SegundoFactorDtos.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\SystemSecondFactorClock.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\CryptographicSecondFactorOtpGenerator.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\AppSettingsSecondFactorKeyProvider.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\HmacSecondFactorCodeProtector.vb',
  'webservice\\Login\\SegundoFactor\\SessionPendingSecondFactorContextStore.vb'
];

function run(command, args, options = {}) {
  return execFileSync(command, args, {
    cwd: root,
    encoding: 'utf8',
    stdio: ['ignore', 'pipe', 'pipe'],
    ...options
  });
}

function csharpCompiler() {
  return run('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source']).trim();
}

test('DOC-91: contratos y seguridad 2FA cumplen comportamiento net461', () => {
  run('msbuild', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const assembly = path.join(root, 'bin', 'GestionDocumental-Docuarchi.net.dll');
  assert.equal(fs.existsSync(assembly), true, 'El build debe generar el ensamblado probado.');

  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc91-foundation-'));
  try {
    const copiedAssembly = path.join(directory, path.basename(assembly));
    const executable = path.join(directory, 'LoginSecondFactorFoundationBehaviorTests.exe');
    fs.copyFileSync(assembly, copiedAssembly);
    run(csharpCompiler(), [
      '/nologo',
      '/target:exe',
      `/out:${executable}`,
      `/reference:${copiedAssembly}`,
      '/reference:System.Web.dll',
      source
    ]);
    assert.match(run(executable, [], { cwd: directory }), /foundation behavior tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('DOC-91: proyecto registra una vez cada fuente y puertos no acoplan infraestructura', () => {
  const projectText = fs.readFileSync(project, 'utf8');
  for (const relative of requiredSources) {
    assert.equal(projectText.split(`Include="${relative}"`).length - 1, 1, `${relative} debe registrarse exactamente una vez.`);
  }

  const pureFiles = [
    path.join(root, 'Modelo', 'Login', 'SegundoFactor', 'SegundoFactorModels.vb'),
    path.join(root, 'Modelo', 'Login', 'SegundoFactor', 'SegundoFactorInterfaces.vb')
  ];
  for (const file of pureFiles) {
    const text = fs.readFileSync(file, 'utf8');
    assert.doesNotMatch(text, /Imports\s+System\.Web|MySql|SmtpClient|SqlConnection|HttpContext/i);
  }
});

test('DOC-91: fundación permanece sin referencias desde archivos productivos legacy', () => {
  const normalize = value => value.replace(/[\\/]/g, path.sep).toLowerCase();
  const allowed = new Set(requiredSources.map(normalize));
  allowed.add(normalize('Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorChallengeRepository.vb'));
  const symbols = /SegundoFactorIdentity|SegundoFactorConfiguration|PendingSecondFactorContext|CryptographicSecondFactorOtpGenerator|HmacSecondFactorCodeProtector|SessionPendingSecondFactorContextStore/;
  const trackedSources = run('git', ['-c', 'core.quotepath=false', 'ls-files', '-z', '*.vb', '*.aspx', '*.ascx', '*.ashx', '*.js'])
    .split('\0')
    .filter(Boolean);
  for (const tracked of trackedSources) {
    const relative = normalize(tracked);
    if (!allowed.has(relative)) {
      assert.doesNotMatch(fs.readFileSync(path.join(root, tracked), 'utf8'), symbols, `Referencia productiva inesperada: ${relative}`);
    }
  }
});

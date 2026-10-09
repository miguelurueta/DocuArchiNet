const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const repositoryRelative = 'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorChallengeRepository.vb';
const repository = path.join(root, ...repositoryRelative.split('\\'));
const sqlRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-92', 'Sql');

function read(file) { return fs.readFileSync(file, 'utf8'); }
function run(command, args, options = {}) { return execFileSync(command, args, { cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], ...options }); }
function compiler() { return run('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source']).trim(); }

test('DOC-92: contrato, rutas y SQL respetan el límite de persistencia', () => {
  const projectText = read(project);
  assert.equal(projectText.split(`Include="${repositoryRelative}"`).length - 1, 1);
  const repositoryText = read(repository);
  assert.match(repositoryText, /Implements ISecondFactorChallengeRepository/);
  assert.match(repositoryText, /SELECT .* FOR UPDATE/i);
  assert.match(repositoryText, /IDataExecutor/);
  assert.match(repositoryText, /ITransactionFactory/);
  assert.doesNotMatch(repositoryText, /HttpContext|System\.Web|\bSession\b|\bconect\b/i);
  assert.doesNotMatch(repositoryText, /CommandText|CreateCommand\(/i);

  assert.deepEqual(fs.readdirSync(sqlRoot).sort(), ['README.md']);
  assert.match(read(path.join(sqlRoot, 'README.md')), /no requiere migración/i);
  assert.match(repositoryText, /ChallengeId,AuthUserId,Provider,CodeHash,ExpiresAtUtc,Consumed,Attempts,CreatedAtUtc,AuthPayloadJson/);
  assert.doesNotMatch(repositoryText, /\bSchemaVersion\b|\bSessionBindingHash\b.*(?:SELECT|INSERT|UPDATE)|\bTerminalAtUtc\b|\bLastSentAtUtc\b/);
  const integration = read(path.join(root, 'tools', 'e2e', 'tests', 'login-second-factor-persistence.integration.test.cjs'));
  assert.match(integration, /DOC92_MYSQL_AUTHORIZED\s*===\s*'SI'/);
  assert.match(integration, /\^doc92_/);
  assert.match(integration, /FOR UPDATE/);
});

test('DOC-92: comportamiento transaccional con dobles locales', () => {
  run('msbuild.exe', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const build = path.join(root, 'bin');
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc92-persistence-'));
  try {
    for (const name of ['GestionDocumental-Docuarchi.net.dll', 'MySql.Data.dll']) fs.copyFileSync(path.join(build, name), path.join(directory, name));
    const executable = path.join(directory, 'LoginSecondFactorPersistenceBehaviorTests.exe');
    run(compiler(), ['/nologo','/target:exe',`/out:${executable}`,`/reference:${path.join(directory, 'GestionDocumental-Docuarchi.net.dll')}`,'/reference:System.Data.dll',path.join(__dirname, 'LoginSecondFactorPersistenceBehaviorTests.cs')]);
    assert.match(run(executable, [], { cwd: directory }), /persistence behavior tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('DOC-92: la fundación DOC-91 conserva firmas previas', () => {
  const interfaces = read(path.join(root, 'Modelo', 'Login', 'SegundoFactor', 'SegundoFactorInterfaces.vb'));
  for (const signature of ['Function Create(', 'Function GetForVerification(', 'Function RegisterFailedAttempt(', 'Function TryBeginFinalization(', 'Function Complete(', 'Function FailFinalization(', 'Function Revoke(']) assert.match(interfaces, new RegExp(signature.replace('(', '\\(')));
});

const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const source = path.join(__dirname, 'WorkflowReturnUserPreviousBehaviorTests.cs');

function csharpCompiler() {
  const command = '(Get-Command csc.exe -ErrorAction Stop).Source';
  return execFileSync('powershell.exe', ['-NoProfile', '-Command', command], {
    cwd: root,
    encoding: 'utf8',
    stdio: ['ignore', 'pipe', 'pipe']
  }).trim();
}

function run(command, args, options = {}) {
  return execFileSync(command, args, {
    cwd: root,
    encoding: 'utf8',
    stdio: ['ignore', 'pipe', 'pipe'],
    ...options
  });
}

test('DOC-36: el servicio ejecuta preview, token y exclusión por tarea con puertos controlados', () => {
  run('msbuild', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const assembly = path.join(root, 'bin', 'GestionDocumental-Docuarchi.net.dll');
  assert.equal(fs.existsSync(assembly), true, 'La compilación debe generar el ensamblado probado.');

  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc36-behavior-'));
  try {
    const copiedAssembly = path.join(directory, path.basename(assembly));
    const executable = path.join(directory, 'WorkflowReturnUserPreviousBehaviorTests.exe');
    fs.copyFileSync(assembly, copiedAssembly);
    run(csharpCompiler(), [
      '/nologo',
      '/target:exe',
      `/out:${executable}`,
      `/reference:${copiedAssembly}`,
      '/reference:System.Web.dll',
      source
    ]);
    const output = run(executable, [], { cwd: directory });
    assert.match(output, /behavior tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

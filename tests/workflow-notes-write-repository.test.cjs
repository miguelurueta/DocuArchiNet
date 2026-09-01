const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const project = path.join(root, 'GestionDocumental-Docuarchi.net.vbproj');
const source = path.join(__dirname, 'WorkflowNotesWriteRepositoryTests.cs');

function csharpCompiler() {
  return execFileSync('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source'], {
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

test('DOC-42: el repositorio falla cerrado y revierte escritura/auditoría con dobles locales', () => {
  run('msbuild', [project, '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const build = path.join(root, 'bin');
  const assembly = path.join(build, 'GestionDocumental-Docuarchi.net.dll');
  assert.equal(fs.existsSync(assembly), true, 'La compilación debe generar el ensamblado probado.');

  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc42-notes-repository-'));
  try {
    for (const name of ['GestionDocumental-Docuarchi.net.dll', 'MySql.Data.dll']) {
      const origin = path.join(build, name);
      if (fs.existsSync(origin)) fs.copyFileSync(origin, path.join(directory, name));
    }
    const executable = path.join(directory, 'WorkflowNotesWriteRepositoryTests.exe');
    run(csharpCompiler(), [
      '/nologo',
      '/target:exe',
      `/out:${executable}`,
      `/reference:${path.join(directory, 'GestionDocumental-Docuarchi.net.dll')}`,
      '/reference:System.Data.dll',
      source
    ]);
    const output = run(executable, [], { cwd: directory });
    assert.match(output, /workflow-notes-write repository tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

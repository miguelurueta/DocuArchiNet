const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { execFileSync } = require('node:child_process');

const root = path.resolve(__dirname, '..');
const authorized = process.env.DOC87_MYSQL_INTEGRATION_AUTHORIZED === 'true';
const run = (command, args, options = {}) => execFileSync(command, args, {
  cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'], ...options
});

test('DOC-87 valida rollback, reintento, conflicto y concurrencia sobre MySQL aislado', { skip: !authorized }, () => {
  run('msbuild.exe', ['GestionDocumental-Docuarchi.net.vbproj', '/t:Build', '/p:Configuration=Debug', '/m:1', '/v:minimal']);
  const build = path.join(root, 'bin');
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc87-mysql-integration-'));
  try {
    for (const name of ['GestionDocumental-Docuarchi.net.dll', 'MySql.Data.dll', 'Newtonsoft.Json.dll']) {
      if (fs.existsSync(path.join(build, name))) fs.copyFileSync(path.join(build, name), path.join(directory, name));
    }
    const compiler = run('powershell.exe', ['-NoProfile', '-Command', '(Get-Command csc.exe -ErrorAction Stop).Source']).trim();
    const executable = path.join(directory, 'RegistroTareaRutaSiiMySqlIntegrationTests.exe');
    run(compiler, [
      '/nologo', '/target:exe', `/out:${executable}`,
      `/reference:${path.join(directory, 'GestionDocumental-Docuarchi.net.dll')}`,
      `/reference:${path.join(directory, 'MySql.Data.dll')}`,
      path.join(__dirname, 'RegistroTareaRutaSiiMySqlIntegrationTests.cs')
    ]);
    assert.match(run(executable, [], { cwd: directory }), /doc87 mysql integration tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

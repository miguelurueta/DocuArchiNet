const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const vbSources = [
  'Infrastructure/Documentos/Streaming/DocumentoStreamingReferencia.vb',
  'Infrastructure/Documentos/Streaming/RangoHttpDocumento.vb',
  'Infrastructure/Documentos/Streaming/CondicionesHttpDocumento.vb',
  'Infrastructure/Documentos/Streaming/EscritorStreamingDocumento.vb',
  'Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb'
].map(value => path.join(root, value));
const csSources = [
  'DocumentStreamingReferenceBehaviorTests.cs',
  'DocumentStreamingRangeBehaviorTests.cs',
  'DocumentStreamingConditionalBehaviorTests.cs',
  'DocumentStreamingWriterBehaviorTests.cs',
  'DocumentStreamingStrategyBehaviorTests.cs',
  'DocumentStreamingBehaviorTestRunner.cs'
].map(value => path.join(__dirname, value));

function run(command, args, options = {}) {
  return execFileSync(command, args, {
    cwd: root,
    encoding: 'utf8',
    stdio: ['ignore', 'pipe', 'pipe'],
    ...options
  });
}

function compiler(name) {
  return run('powershell.exe', ['-NoProfile', '-Command', `(Get-Command ${name} -ErrorAction Stop).Source`]).trim();
}

test('DOC-102: referencia, rangos, condiciones y escritor cumplen contrato net461', () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc102-streaming-'));
  try {
    const assembly = path.join(directory, 'DocumentStreaming.dll');
    const executable = path.join(directory, 'DocumentStreamingBehaviorTests.exe');
    run(compiler('vbc.exe'), [
      '/nologo', '/target:library', `/out:${assembly}`,
      '/reference:System.dll', '/reference:System.Core.dll', '/reference:System.Web.dll',
      ...vbSources
    ]);
    run(compiler('csc.exe'), [
      '/nologo', '/target:exe', `/out:${executable}`,
      `/reference:${assembly}`, '/reference:System.dll', '/reference:System.Core.dll', '/reference:System.Web.dll',
      ...csSources
    ]);
    assert.match(run(executable, [], { cwd: directory }), /document streaming behavior tests: passed/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('DOC-102: proyecto registra cada componente streaming exactamente una vez', () => {
  const project = fs.readFileSync(path.join(root, 'GestionDocumental-Docuarchi.net.vbproj'), 'utf8');
  for (const relative of [
    'Infrastructure\\Documentos\\Streaming\\DocumentoStreamingReferencia.vb',
    'Infrastructure\\Documentos\\Streaming\\RangoHttpDocumento.vb',
    'Infrastructure\\Documentos\\Streaming\\CondicionesHttpDocumento.vb',
    'Infrastructure\\Documentos\\Streaming\\EscritorStreamingDocumento.vb',
    'Infrastructure\\Documentos\\Streaming\\EstrategiaCargaPdf.vb'
  ]) {
    assert.equal(project.split(`Include="${relative}"`).length - 1, 1, relative);
  }
});

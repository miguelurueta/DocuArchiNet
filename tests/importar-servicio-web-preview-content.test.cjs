const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const policy = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewContentService.vb', 'utf8');
const handler = fs.readFileSync('workflow/ImportarServicioWebPreview.ashx.vb', 'utf8');
const handlerDirective = fs.readFileSync('workflow/ImportarServicioWebPreview.ashx', 'utf8');
const sql = fs.readFileSync('Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-69-lista-preview-incripciones/Sql/001-create-workflow-import-preview-descriptor-mysql51.sql', 'utf8');

test('política aplica allowlist servidor, tamaño y nombre seguro', () => {
  for (const type of ['application/pdf', 'image/png', 'image/jpeg', 'image/tiff']) assert.ok(policy.includes(type));
  assert.match(policy, /SafeFileName/);
  assert.match(policy, /\[\^A-Za-z0-9\._-\]/);
  assert.match(sql, /content MEDIUMBLOB NOT NULL/);
});

test('handler publica headers defensivos y transmite por bloques', () => {
  assert.match(handlerDirective, /Class="GestionDocumental_Docuarchi\.net\.ImportarServicioWebPreview"/);
  for (const value of ['Content-Length', 'Content-Disposition', 'SetNoStore', 'HttpCacheability.Private', 'no-cache', 'nosniff', 'SAMEORIGIN']) {
    assert.ok(handler.includes(value), value);
  }
  assert.match(handler, /Const blockSize As Integer = 65536/);
  assert.match(handler, /Response\.IsClientConnected/);
  assert.match(handler, /Finally[\s\S]*service\.Complete/);
  const reject = handler.match(/Private Shared Sub Reject[\s\S]*?End Sub/)[0];
  assert.doesNotMatch(reject, /ApplyDefensiveHeaders/);
});

test('HEAD no reclama contenido y GET usa transición atómica', () => {
  const repository = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewDescriptorRepository.vb', 'utf8');
  assert.match(handler, /If method = "HEAD" Then[\s\S]*service\.Head/);
  assert.match(handler, /service\.Claim/);
  assert.match(repository, /SET status='Reclamado'.*status='Disponible'/);
  assert.match(repository, /BeginTransaction/);
  assert.match(repository, /ReadBytes\(reader, "content"\)/);
  assert.match(repository, /record\.GetBytes/);
  assert.doesNotMatch(repository, /DirectCast\(reader\("content"\), Byte\(\)\)/);
  assert.match(repository, /content=X''/);
  assert.doesNotMatch(repository, /content=0x/);
});

test('migración MySQL 5.1 no contiene FK ni referencias cross-database', () => {
  assert.match(sql, /ENGINE=InnoDB/);
  assert.doesNotMatch(sql, /FOREIGN\s+KEY|REFERENCES/i);
  assert.doesNotMatch(sql, /\b(?:docuarchi|radicacion)\s*\./i);
  assert.match(sql, /UNIQUE KEY ux_import_preview_descriptor_hash/);
});

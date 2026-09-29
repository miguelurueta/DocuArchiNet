'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const apply = fs.readFileSync('Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-83-pruebas-gate-compatibilidad/Sql/001-allow-multiple-cash-receipts-mutacionregmer.sql', 'utf8');
const rollback = fs.readFileSync('Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-83-pruebas-gate-compatibilidad/Sql/002-rollback-multiple-cash-receipts-mutacionregmer.sql', 'utf8');
const preflight = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb', 'utf8');

test('mutacionregmer conserva Recibo De Caja obligatorio y permite cardinalidad múltiple con rollback', () => {
  for (const sql of [apply, rollback]) {
    assert.match(sql, /ID_TIPO_DOCUMENTAL_CHEQUEO\s*=\s*265/);
    assert.match(sql, /tipo_doc_entrante_id_Tipo_Doc_Entrante\s*=\s*290/);
    assert.match(sql, /tipo_doc_series_Id_Tipo_Doc_Series\s*=\s*186/);
    assert.match(sql, /OBLIGATORIO\s*=\s*1/);
  }
  assert.match(apply, /SET\s+UNICO\s*=\s*0/i);
  assert.match(rollback, /SET\s+UNICO\s*=\s*1/i);
});

test('preflight deduplica por identidad externa y no por tipo documental', () => {
  assert.match(preflight, /externalKeys\.Add\(item\.ExternalKey\.Trim\(\)\)/);
  assert.doesNotMatch(preflight, /documentTypeIds\.Add|DocumentTypeId[^\r\n]*DUPLICATE_SELECTION/i);
});

'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const e2eRoot = path.resolve(__dirname, '..');
const previewSource = fs.readFileSync(path.join(e2eRoot, 'tests', 'doc36-return-user-previous.spec.cjs'), 'utf8');
const concurrencySource = fs.readFileSync(path.join(e2eRoot, 'scripts', 'run-doc36-return-user-previous-concurrency.cjs'), 'utf8');
const diagnosticSource = fs.readFileSync(path.join(e2eRoot, 'scripts', 'run-doc36-active-activity.cjs'), 'utf8');
const profile = JSON.parse(fs.readFileSync(path.join(e2eRoot, 'profiles', 'doc36-workflow-user-previous.profile.example.json'), 'utf8'));

test('DOC-36 E2E usa solo preview vigente, payload mínimo, ODBC de lectura y gate local apagado', () => {
  assert.match(previewSource, /PreviewDevolverUsuarioAnterior/);
  assert.match(previewSource, /EjecutarDevolverUsuarioAnterior/);
  assert.match(previewSource, /\{ idTarea: taskId, tokenVersion: preview\.TokenVersion \}/);
  assert.match(previewSource, /queryFingerprint\(stateSql, taskId\)/);
  assert.match(previewSource, /queryFingerprint\(auditSql, taskId\)/);
  assert.match(previewSource, /queryFinalActivity\(taskId, preview\.Contexto\?\.ActividadAnterior, process\.env, prefix\)/);
  assert.match(previewSource, /assertLocalGateOff/);
  assert.doesNotMatch(previewSource, /idConector|idUsuarioDestino|idActividadDestino/i);
});

test('DOC-36 E2E reserva una segunda tarea para concurrencia y exige una sola mutación', () => {
  assert.match(concurrencySource, /PreviewDevolverUsuarioAnterior/);
  assert.match(concurrencySource, /EjecutarDevolverUsuarioAnterior/);
  assert.match(concurrencySource, /Promise\.all\(contexts\.map/);
  assert.match(concurrencySource, /successes\.length === 1/);
  assert.match(concurrencySource, /WORKFLOW_RETURN_USER_IN_PROGRESS/);
  assert.match(concurrencySource, /WORKFLOW_RETURN_USER_VERSION_CONFLICT/);
  assert.match(concurrencySource, /assertLocalGateOff/);
  assert.doesNotMatch(concurrencySource, /idConector|idUsuarioDestino|idActividadDestino/i);
});

test('la plantilla persistente DOC-36 contiene solo datos operativos no sensibles y recursos distintos', () => {
  assert.equal(profile.doc, 'doc36');
  assert.notEqual(profile.executionTaskId, profile.concurrencyTaskId);
  assert.equal(typeof profile.baseUrl, 'string');
  assert.equal(typeof profile.odbcDsn, 'string');
  for (const key of Object.keys(profile)) {
    assert.doesNotMatch(key, /passw(?:ord)?|cookie|token|secret|credential|connection|authorization|authorized/i);
  }
});

test('el diagnóstico temporal de actividad DOC-36 usa TTY y ODBC de solo lectura', () => {
  assert.match(diagnosticSource, /collectValue\(secrets, 'DOC36_E2E_MYSQL_PASSWORD'/);
  assert.match(diagnosticSource, /queryActiveActivity\(taskId, environment, 'DOC36_E2E'\)/);
  assert.match(diagnosticSource, /DOC36_E2E_ODBC_DSN: 'workflowconta'/);
  assert.doesNotMatch(diagnosticSource, /(?:\bINSERT\b|\bUPDATE\b|\bDELETE\b|\bCREATE\b|\bDROP\b)\s+(?:INTO|SET|TABLE|DATABASE)/i);
});

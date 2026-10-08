'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const {
  COLUMNS_SQL,
  INDEXES_SQL,
  TABLES_SQL,
  TARGET_TABLES,
  assertReadOnlyQueries,
  buildReport,
  isMetadataSelect
} = require('../scripts/inspect-login-second-factor-schema-interactive.cjs');

test('el inspector usa exclusivamente SELECT sobre information_schema', () => {
  assert.doesNotThrow(() => assertReadOnlyQueries());
  for (const sql of [TABLES_SQL, COLUMNS_SQL, INDEXES_SQL]) {
    assert.equal(isMetadataSelect(sql), true);
    assert.doesNotMatch(sql, /\b(?:INSERT|UPDATE|DELETE|ALTER|CREATE|DROP)\b/i);
  }
});

test('el contrato incluye configuración, challenge y usuarios de los cuatro módulos', () => {
  assert.deepEqual(Object.keys(TARGET_TABLES).sort(), [
    'gestor_modulos',
    'ra_auth_second_factor_challenge',
    'remit_dest_interno',
    'usuario_radicador',
    'usuario_workflow',
    'usuarios_da'
  ]);
  assert.ok(TARGET_TABLES.gestor_modulos.includes('RequiereSegundoFactor'));
  assert.ok(TARGET_TABLES.usuario_radicador.includes('Correo_Usuario'));
});

test('el reporte distingue columnas ausentes e índice único del challenge', () => {
  const tables = [{
    TABLE_SCHEMA: 'radicacion',
    TABLE_NAME: 'ra_auth_second_factor_challenge',
    ENGINE: 'InnoDB',
    TABLE_COLLATION: 'utf8mb4_general_ci'
  }];
  const columns = TARGET_TABLES.ra_auth_second_factor_challenge
    .filter(name => name !== 'AuthPayloadJson')
    .map((name, index) => ({
      TABLE_SCHEMA: 'radicacion',
      TABLE_NAME: 'ra_auth_second_factor_challenge',
      COLUMN_NAME: name,
      COLUMN_TYPE: 'varchar(36)',
      IS_NULLABLE: 'NO',
      COLUMN_KEY: name === 'ChallengeId' ? 'UNI' : '',
      ORDINAL_POSITION: index + 1
    }));
  const indexes = [{
    TABLE_SCHEMA: 'radicacion',
    TABLE_NAME: 'ra_auth_second_factor_challenge',
    INDEX_NAME: 'UX_challenge',
    NON_UNIQUE: 0,
    SEQ_IN_INDEX: 1,
    COLUMN_NAME: 'ChallengeId'
  }];

  const report = buildReport(tables, columns, indexes, 'radicacion');
  assert.deepEqual(report.occurrences[0].missingColumns, ['AuthPayloadJson']);
  assert.equal(report.occurrences[0].challengeIdUnique, true);
  assert.equal(report.occurrences[0].configuredDatabase, true);
});

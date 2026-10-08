'use strict';

const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

const TARGET_TABLES = Object.freeze({
  gestor_modulos: [
    'ID_MODULO',
    'RequiereSegundoFactor',
    'SecondFactorProviderType',
    'SegundoFactorTiempoExpira'
  ],
  ra_auth_second_factor_challenge: [
    'Id',
    'ChallengeId',
    'AuthUserId',
    'Provider',
    'CodeHash',
    'ExpiresAtUtc',
    'Consumed',
    'Attempts',
    'CreatedAtUtc',
    'AuthPayloadJson'
  ],
  usuarios_da: ['Clave_Usuario', 'idusuario', 'correo'],
  remit_dest_interno: ['Id_Remit_Dest_Int', 'Login_Usuario', 'Correo_Electronico'],
  usuario_radicador: ['id_usuario', 'Login_usuario', 'Correo_Usuario'],
  usuario_workflow: ['idU_suario', 'login_Usuario', 'Correo_Usuario']
});

const TABLE_NAMES_SQL = Object.keys(TARGET_TABLES)
  .map(name => `'${name.toUpperCase()}'`)
  .join(', ');

const TABLES_SQL = `
  SELECT TABLE_SCHEMA,
         TABLE_NAME,
         ENGINE,
         TABLE_COLLATION
    FROM information_schema.TABLES
   WHERE UPPER(TABLE_NAME) IN (${TABLE_NAMES_SQL})
     AND TABLE_SCHEMA NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
   ORDER BY TABLE_SCHEMA, TABLE_NAME`;

const COLUMNS_SQL = `
  SELECT TABLE_SCHEMA,
         TABLE_NAME,
         COLUMN_NAME,
         COLUMN_TYPE,
         IS_NULLABLE,
         COLUMN_DEFAULT,
         COLUMN_KEY,
         EXTRA,
         ORDINAL_POSITION
    FROM information_schema.COLUMNS
   WHERE UPPER(TABLE_NAME) IN (${TABLE_NAMES_SQL})
     AND TABLE_SCHEMA NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
   ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION`;

const INDEXES_SQL = `
  SELECT TABLE_SCHEMA,
         TABLE_NAME,
         INDEX_NAME,
         NON_UNIQUE,
         SEQ_IN_INDEX,
         COLUMN_NAME
    FROM information_schema.STATISTICS
   WHERE UPPER(TABLE_NAME) IN (${TABLE_NAMES_SQL})
     AND TABLE_SCHEMA NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')
   ORDER BY TABLE_SCHEMA, TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX`;

function profilePath(argv = process.argv) {
  const index = argv.indexOf('--profile');
  if (index < 0 || !argv[index + 1]) {
    throw new Error('Debe indicar --profile <archivo-json-no-sensible>.');
  }
  return path.resolve(argv[index + 1]);
}

function loadConnectionMetadata(target) {
  const profile = JSON.parse(fs.readFileSync(target, 'utf8'));
  const forbidden = Object.keys(profile).filter(key =>
    /(?:password|passwd|secret|credential|mysqluser|mysqlurl|connectionstring)/i.test(key));
  if (forbidden.length > 0) {
    throw new Error(`El perfil contiene campos sensibles no permitidos: ${forbidden.join(', ')}.`);
  }

  const host = String(profile.mysqlHost || '').trim();
  const port = Number(profile.mysqlPort);
  const database = String(profile.mysqlDatabase || '').trim();
  if (!host || !Number.isInteger(port) || port < 1 || port > 65535 || !database) {
    throw new Error('El perfil no contiene host, puerto y base MySQL válidos.');
  }
  return { host, port, database };
}

function isMetadataSelect(sql) {
  return /^\s*SELECT\b/i.test(sql) &&
    /\binformation_schema\./i.test(sql) &&
    !/;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql);
}

function assertReadOnlyQueries() {
  for (const [name, sql] of Object.entries({ TABLES_SQL, COLUMNS_SQL, INDEXES_SQL })) {
    if (!isMetadataSelect(sql)) throw new Error(`${name} no cumple el contrato SELECT de metadatos.`);
  }
}

function normalize(value) {
  return String(value || '').toLowerCase();
}

function buildReport(tables, columns, indexes, configuredDatabase) {
  const occurrences = tables.map(table => {
    const schema = String(table.TABLE_SCHEMA);
    const tableName = String(table.TABLE_NAME);
    const expectedEntry = Object.entries(TARGET_TABLES)
      .find(([name]) => normalize(name) === normalize(tableName));
    const expectedColumns = expectedEntry?.[1] || [];
    const actualColumns = columns.filter(column =>
      normalize(column.TABLE_SCHEMA) === normalize(schema) &&
      normalize(column.TABLE_NAME) === normalize(tableName));
    const actualByName = new Map(actualColumns.map(column => [normalize(column.COLUMN_NAME), column]));
    const requiredColumns = expectedColumns.map(expectedName => {
      const actual = actualByName.get(normalize(expectedName));
      return {
        schema,
        table: tableName,
        requiredColumn: expectedName,
        exists: Boolean(actual),
        actualColumn: actual?.COLUMN_NAME || 'AUSENTE',
        type: actual?.COLUMN_TYPE || 'No aplica',
        nullable: actual?.IS_NULLABLE || 'No aplica',
        key: actual?.COLUMN_KEY || 'No aplica'
      };
    });
    const tableIndexes = indexes.filter(index =>
      normalize(index.TABLE_SCHEMA) === normalize(schema) &&
      normalize(index.TABLE_NAME) === normalize(tableName));
    const challengeIdIndexes = tableIndexes.filter(index => normalize(index.COLUMN_NAME) === 'challengeid');

    return {
      schema,
      table: tableName,
      configuredDatabase: normalize(schema) === normalize(configuredDatabase),
      engine: table.ENGINE,
      collation: table.TABLE_COLLATION,
      requiredColumns,
      missingColumns: requiredColumns.filter(column => !column.exists).map(column => column.requiredColumn),
      challengeIdUnique: normalize(tableName) === 'ra_auth_second_factor_challenge'
        ? challengeIdIndexes.some(index => Number(index.NON_UNIQUE) === 0)
        : null,
      indexes: tableIndexes.map(index => ({
        name: index.INDEX_NAME,
        unique: Number(index.NON_UNIQUE) === 0,
        sequence: Number(index.SEQ_IN_INDEX),
        column: index.COLUMN_NAME
      }))
    };
  });

  const foundNames = new Set(occurrences.map(item => normalize(item.table)));
  return {
    occurrences,
    tablesNotFound: Object.keys(TARGET_TABLES).filter(name => !foundNames.has(normalize(name)))
  };
}

function printReport(report) {
  console.log('Tablas localizadas:');
  console.table(report.occurrences.map(item => ({
    schema: item.schema,
    table: item.table,
    configuredDatabase: item.configuredDatabase,
    engine: item.engine,
    collation: item.collation,
    missingColumns: item.missingColumns.length ? item.missingColumns.join(', ') : 'NINGUNA',
    challengeIdUnique: item.challengeIdUnique === null ? 'No aplica' : item.challengeIdUnique
  })));

  console.log('Contrato de columnas requeridas:');
  console.table(report.occurrences.flatMap(item => item.requiredColumns));

  const challengeIndexes = report.occurrences
    .filter(item => normalize(item.table) === 'ra_auth_second_factor_challenge')
    .flatMap(item => item.indexes.map(index => ({ schema: item.schema, table: item.table, ...index })));
  console.log('Índices de la tabla de challenges:');
  if (challengeIndexes.length > 0) console.table(challengeIndexes);
  else console.log('No se encontraron índices porque la tabla de challenges no existe o no es visible.');

  console.log(`Tablas no localizadas en los esquemas visibles: ${report.tablesNotFound.length ? report.tablesNotFound.join(', ') : 'NINGUNA'}.`);
}

function safeDatabaseFailure(error) {
  const code = typeof error?.code === 'string' && /^[A-Z0-9_]+$/.test(error.code)
    ? ` (${error.code})`
    : '';
  return `No fue posible inspeccionar el esquema de segundo factor${code}. No se mostraron credenciales ni filas de negocio.`;
}

async function main() {
  requireInteractiveConsole();
  assertReadOnlyQueries();
  const connectionMetadata = loadConnectionMetadata(profilePath());
  const secrets = {};
  await collectValue(secrets, 'LOGIN_2FA_SCHEMA_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(secrets, 'LOGIN_2FA_SCHEMA_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectConfirmation(secrets, 'LOGIN_2FA_SCHEMA_AUTHORIZED', '¿Autoriza la inspección SELECT de metadatos para Login 2FA?');

  let connection;
  try {
    connection = await mysql.createConnection({
      ...connectionMetadata,
      user: secrets.LOGIN_2FA_SCHEMA_MYSQL_USER,
      password: secrets.LOGIN_2FA_SCHEMA_MYSQL_PASSWORD,
      multipleStatements: false
    });
    const [tables] = await connection.execute(TABLES_SQL);
    const [columns] = await connection.execute(COLUMNS_SQL);
    const [indexes] = await connection.execute(INDEXES_SQL);
    printReport(buildReport(tables, columns, indexes, connectionMetadata.database));
    console.log('Inspección Login 2FA completada exclusivamente con SELECT sobre information_schema.');
  } catch (error) {
    console.error(safeDatabaseFailure(error));
    process.exitCode = 1;
  } finally {
    secrets.LOGIN_2FA_SCHEMA_MYSQL_PASSWORD = '';
    await connection?.end().catch(() => {});
  }
}

if (require.main === module) main().catch(error => {
  console.error(error?.message || 'La inspección Login 2FA se detuvo sin mostrar valores sensibles.');
  process.exitCode = 1;
});

module.exports = {
  COLUMNS_SQL,
  INDEXES_SQL,
  TABLES_SQL,
  TARGET_TABLES,
  assertReadOnlyQueries,
  buildReport,
  isMetadataSelect,
  loadConnectionMetadata
};

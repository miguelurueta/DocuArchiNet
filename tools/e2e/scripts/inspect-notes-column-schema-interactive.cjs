'use strict';

const mysql = require('mysql2/promise');
const {
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

const STORAGE_COLUMNS_SQL = `
  SELECT TABLE_SCHEMA,
         TABLE_NAME,
         COLUMN_NAME,
         COLUMN_TYPE,
         CHARACTER_MAXIMUM_LENGTH,
         CHARACTER_OCTET_LENGTH,
         CHARACTER_SET_NAME,
         COLLATION_NAME,
         IS_NULLABLE,
         COLUMN_KEY,
         EXTRA
    FROM information_schema.COLUMNS
   WHERE UPPER(TABLE_NAME) IN ('ANOTACION_TAREA', 'WF_LOG_WORKFLOW', 'WORKFLOW_NOTAS_IDEMPOTENCIA')
     AND (
       UPPER(TABLE_NAME) = 'ANOTACION_TAREA'
       OR UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_IDEMPOTENCIA'
       OR UPPER(COLUMN_NAME) IN ('DATOS_OPERACION', 'ID_OPERACION', 'ID_TAREA_WORKFLOW', 'OPERACION', 'FECHA_HORA', 'USUARIO_WORKFLOW_IDU_SUARIO')
     )
   ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION`;

const STORAGE_INDEXES_SQL = `
  SELECT TABLE_SCHEMA,
         TABLE_NAME,
         INDEX_NAME,
         NON_UNIQUE,
         SEQ_IN_INDEX,
         COLUMN_NAME
    FROM information_schema.STATISTICS
   WHERE UPPER(TABLE_NAME) IN ('ANOTACION_TAREA', 'WF_LOG_WORKFLOW', 'WORKFLOW_NOTAS_IDEMPOTENCIA')
   ORDER BY TABLE_SCHEMA, TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX`;

const STORAGE_VOLUME_SQL = `
  SELECT TABLE_SCHEMA,
         TABLE_NAME,
         ENGINE,
         TABLE_ROWS,
         DATA_LENGTH,
         INDEX_LENGTH
    FROM information_schema.TABLES
   WHERE UPPER(TABLE_NAME) IN ('ANOTACION_TAREA', 'WF_LOG_WORKFLOW', 'WORKFLOW_NOTAS_IDEMPOTENCIA')
   ORDER BY TABLE_SCHEMA, TABLE_NAME`;

function safeDatabaseFailure(error) {
  const code = typeof error?.code === 'string' && /^[A-Z0-9_]+$/.test(error.code)
    ? ` (${error.code})`
    : '';
  return `No fue posible consultar el metadato de almacenamiento de notas${code}. No se mostraron credenciales ni datos de notas.`;
}

async function main() {
  requireInteractiveConsole();

  const credentials = {};
  await collectValue(credentials, 'NOTES_SCHEMA_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(credentials, 'NOTES_SCHEMA_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });

  let connection;
  try {
    connection = await mysql.createConnection({
      host: '127.0.0.1',
      user: credentials.NOTES_SCHEMA_MYSQL_USER,
      password: credentials.NOTES_SCHEMA_MYSQL_PASSWORD
    });
    const [columns] = await connection.execute(STORAGE_COLUMNS_SQL);
    const [indexes] = await connection.execute(STORAGE_INDEXES_SQL);
    const [volume] = await connection.execute(STORAGE_VOLUME_SQL);
    console.log('Columnas de notas y auditoría:');
    console.table(columns);
    console.log('Índices:');
    console.table(indexes);
    console.log('Volumen agregado aproximado:');
    console.table(volume);
  } catch (error) {
    console.error(safeDatabaseFailure(error));
    process.exitCode = 1;
  } finally {
    credentials.NOTES_SCHEMA_MYSQL_PASSWORD = '';
    if (connection) await connection.end();
  }
}

main();

'use strict';

const mysql = require('mysql2/promise');
const { collectValue, requireInteractiveConsole } = require('./support/interactive-e2e-console.cjs');

const quote = (value) => `\`${String(value).replace(/`/g, '``')}\``;
const safe = (value) => String(value ?? '').replace(/[\r\n\t]+/g, ' ').trim().slice(0, 120);

function registrationType(name) {
  const value = safe(name).toUpperCase();
  if (/\bRUP\b|PROPONENTE/.test(value)) return 'RUP';
  if (/\bESAL\b|SIN ANIMO|SIN ÁNIMO/.test(value)) return 'ESAL';
  if (/MERCANTIL|REGISTRO MERCANTIL/.test(value)) return 'MERCANTIL';
  return 'NO_CLASIFICADO';
}

async function main() {
  requireInteractiveConsole();
  const values = {};
  await collectValue(values, 'DOC67_DATA_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(values, 'DOC67_DATA_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectValue(values, 'DOC67_DATA_SCHEMA', 'Esquema Workflow', { defaultValue: 'workflowdocument' });
  await collectValue(values, 'DOC67_DATA_LOGIN', 'Login Workflow autorizado', { defaultValue: 'jhorman.rebellon-abo' });
  await collectValue(values, 'DOC67_DATA_GROUP', 'Grupo Workflow', { defaultValue: 'ABOGADOS' });

  let connection;
  try {
    connection = await mysql.createConnection({
      host: '127.0.0.1',
      user: values.DOC67_DATA_MYSQL_USER,
      password: values.DOC67_DATA_MYSQL_PASSWORD,
      database: values.DOC67_DATA_SCHEMA
    });
    const schema = quote(values.DOC67_DATA_SCHEMA);
    const [users] = await connection.execute(
      `SELECT idU_suario AS idUsuario FROM ${schema}.usuario_workflow WHERE login_Usuario=? LIMIT 2`,
      [values.DOC67_DATA_LOGIN]
    );
    if (users.length !== 1) throw new Error('DOC67_USER_NOT_UNIQUE');

    const [cacheLocations] = await connection.execute(
      "SELECT TABLE_SCHEMA AS tableSchema FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ra_sii_cache_exepediente' ORDER BY TABLE_SCHEMA LIMIT 2"
    );
    const cacheSchema = cacheLocations.length === 1 ? quote(cacheLocations[0].tableSchema) : null;
    const cacheSelect = cacheSchema ? 'MAX(cache.NombreGabinete)' : 'NULL';
    const cacheJoin = cacheSchema
      ? `LEFT JOIN ${cacheSchema}.ra_sii_cache_exepediente cache ON cache.CodigoBarras=log.CODIGO_BARRAS OR cache.RadicadoSII=log.DATOS_RECIBO`
      : '';

    const [rows] = await connection.execute(
      `SELECT et.Inicio_Tareas_Workflow_id_Tarea AS taskId,
              log.CODIGO_RUE AS radicado,
              log.DATOS_RECIBO AS datosRecibo,
              log.CODIGO_BARRAS AS codigoBarras,
              log.NOMBRE_TRAMITE AS tramite,
              CASE WHEN et.FECHA_SELECCION IS NULL THEN 0 ELSE 1 END AS seleccionada,
              owner.login_Usuario AS usuarioAsignado,
              ownerGroup.Nombre_Grupo AS grupoAsignado,
              COUNT(DISTINCT wi.intent_id) AS intentCount,
              ${cacheSelect} AS gabineteHistorico
         FROM ${schema}.estados_tarea_workflow et
         INNER JOIN ${schema}.wf_int_sii_registro_tarea_rue_virtual log
                 ON log.inicio_tareas_workflow_id_Tarea=et.Inicio_Tareas_Workflow_id_Tarea
         LEFT JOIN ${schema}.usuario_workflow owner ON owner.idU_suario=et.ID_USUARIO
         LEFT JOIN ${schema}.grupos_workflow ownerGroup
                ON ownerGroup.ID_GRUPO=owner.GRUPOS_WORKFLOW_ID_GRUPO
               AND ownerGroup.RUTAS_WORKFLOW_ID_RUTA=owner.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA
         LEFT JOIN ${schema}.workflow_import_intent wi
                ON wi.task_id=et.Inicio_Tareas_Workflow_id_Tarea
         ${cacheJoin}
        WHERE et.FECHA_FIN IS NULL
          AND et.ESTADO_TAREA=0
          AND log.CODIGO_RUE IS NOT NULL
          AND log.CODIGO_BARRAS IS NOT NULL
          AND ownerGroup.Nombre_Grupo LIKE ?
        GROUP BY et.Inicio_Tareas_Workflow_id_Tarea,log.CODIGO_RUE,log.DATOS_RECIBO,log.CODIGO_BARRAS,
                 log.NOMBRE_TRAMITE,et.FECHA_SELECCION,owner.login_Usuario,ownerGroup.Nombre_Grupo
        ORDER BY et.Inicio_Tareas_Workflow_id_Tarea DESC
        LIMIT 200`,
      [`%${safe(values.DOC67_DATA_GROUP)}%`]
    );

    const candidates = rows.map((row) => ({
      tipo: registrationType(row.gabineteHistorico || row.tramite),
      taskId: Number(row.taskId),
      radicado: safe(row.datosRecibo || row.radicado),
      codigoBarras: safe(row.codigoBarras),
      tramite: safe(row.tramite),
      gabineteHistorico: safe(row.gabineteHistorico),
      seleccionada: Number(row.seleccionada) === 1,
      usuarioAsignado: safe(row.usuarioAsignado),
      grupoAsignado: safe(row.grupoAsignado),
      intencionesPrevias: Number(row.intentCount)
    }));

    console.log('Candidatos DOC-67 sin intención moderna previa (solo SELECT):');
    console.table(candidates);
    for (const type of ['MERCANTIL', 'ESAL', 'RUP']) {
      if (!candidates.some((candidate) => candidate.tipo === type)) console.log(`${type}: SIN_CANDIDATO`);
    }
  } catch (error) {
    const code = typeof error?.code === 'string' && /^[A-Z0-9_]+$/.test(error.code) ? error.code : 'DOC67_CANDIDATE_QUERY_FAILED';
    console.error(`No fue posible resolver candidatos DOC-67 (${code}). No se mostraron secretos.`);
    process.exitCode = 1;
  } finally {
    values.DOC67_DATA_MYSQL_PASSWORD = '';
    if (connection) await connection.end();
  }
}

main();

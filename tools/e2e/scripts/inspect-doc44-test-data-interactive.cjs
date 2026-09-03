'use strict';

const mysql = require('mysql2/promise');
const { collectValue, requireInteractiveConsole } = require('./support/interactive-e2e-console.cjs');

const quote = value => `\`${String(value).replace(/`/g, '``')}\``;

async function main() {
  requireInteractiveConsole();
  const values = {};
  await collectValue(values, 'DOC44_DATA_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(values, 'DOC44_DATA_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectValue(values, 'DOC44_DATA_SCHEMA', 'Esquema Workflow', { defaultValue: 'workflowtconta' });
  await collectValue(values, 'DOC44_DATA_LOGIN', 'Login Workflow autorizado', { defaultValue: 'yalile.mojica' });
  await collectValue(values, 'DOC44_DATA_TARGET_TASK_ID', 'ID de tarea descartable para validar propiedad', { defaultValue: '627' });

  let connection;
  try {
    connection = await mysql.createConnection({
      host: '127.0.0.1',
      user: values.DOC44_DATA_MYSQL_USER,
      password: values.DOC44_DATA_MYSQL_PASSWORD,
      database: values.DOC44_DATA_SCHEMA
    });
    const schema = quote(values.DOC44_DATA_SCHEMA);
    const [users] = await connection.execute(
      `SELECT idU_suario AS idUsuario FROM ${schema}.usuario_workflow WHERE login_Usuario = ? LIMIT 2`,
      [values.DOC44_DATA_LOGIN]
    );
    if (users.length !== 1) throw new Error('USER_NOT_UNIQUE');
    const userId = Number(users[0].idUsuario);
    const targetTaskId = Number(values.DOC44_DATA_TARGET_TASK_ID);
    if (!Number.isSafeInteger(targetTaskId) || targetTaskId <= 0) throw new Error('INVALID_TASK_ID');

    const [own] = await connection.execute(
      `SELECT Inicio_Tareas_Workflow_id_Tarea AS idTarea FROM ${schema}.estados_tarea_workflow WHERE id_usuario = ? AND fecha_fin IS NULL ORDER BY Inicio_Tareas_Workflow_id_Tarea DESC LIMIT 5`,
      [userId]
    );
    const [foreign] = await connection.execute(
      `SELECT Inicio_Tareas_Workflow_id_Tarea AS idTarea FROM ${schema}.estados_tarea_workflow WHERE id_usuario IS NOT NULL AND id_usuario <> ? AND fecha_fin IS NULL ORDER BY Inicio_Tareas_Workflow_id_Tarea DESC LIMIT 5`,
      [userId]
    );
    const [inactive] = await connection.execute(
      `SELECT Inicio_Tareas_Workflow_id_Tarea AS idTarea FROM ${schema}.estados_tarea_workflow WHERE fecha_fin IS NOT NULL ORDER BY Inicio_Tareas_Workflow_id_Tarea DESC LIMIT 5`
    );
    const ownIds = own.map(row => Number(row.idTarea));
    const [notes] = await connection.execute(
      `SELECT Id_Anotacion AS idNota, Inicio_Tareas_Workflow_id_Tarea AS idTarea FROM ${schema}.anotacion_tarea WHERE Inicio_Tareas_Workflow_id_Tarea NOT IN (${ownIds.length ? ownIds.map(() => '?').join(',') : '0'}) ORDER BY Id_Anotacion DESC LIMIT 5`,
      ownIds
    );
    const [nonOwnerLongNotes] = await connection.execute(
      `SELECT Id_Anotacion AS idNota, Inicio_Tareas_Workflow_id_Tarea AS idTarea, CHAR_LENGTH(COALESCE(Dato_Anotacion, '')) AS longitud FROM ${schema}.anotacion_tarea WHERE Inicio_Tareas_Workflow_id_Tarea = ? AND Estado_Tarea = 1 AND COALESCE(id_usuario, 0) <> ? AND CHAR_LENGTH(COALESCE(Dato_Anotacion, '')) > 280 ORDER BY Id_Anotacion DESC LIMIT 5`,
      [targetTaskId, userId]
    );
    const [nonOwnerNotes] = await connection.execute(
      `SELECT Id_Anotacion AS idNota, Inicio_Tareas_Workflow_id_Tarea AS idTarea, CHAR_LENGTH(COALESCE(Dato_Anotacion, '')) AS longitud FROM ${schema}.anotacion_tarea WHERE Inicio_Tareas_Workflow_id_Tarea = ? AND Estado_Tarea = 1 AND COALESCE(id_usuario, 0) <> ? ORDER BY Id_Anotacion DESC LIMIT 5`,
      [targetTaskId, userId]
    );
    const [ownedTasksWithForeignNotes] = ownIds.length
      ? await connection.execute(
        `SELECT Id_Anotacion AS idNota, Inicio_Tareas_Workflow_id_Tarea AS idTarea, CHAR_LENGTH(COALESCE(Dato_Anotacion, '')) AS longitud FROM ${schema}.anotacion_tarea WHERE Inicio_Tareas_Workflow_id_Tarea IN (${ownIds.map(() => '?').join(',')}) AND Estado_Tarea = 1 AND COALESCE(id_usuario, 0) <> ? ORDER BY Id_Anotacion DESC LIMIT 10`,
        [...ownIds, userId]
      )
      : [[]];
    const [ownedTasksWithoutNotes] = ownIds.length
      ? await connection.execute(
        `SELECT et.Inicio_Tareas_Workflow_id_Tarea AS idTarea FROM ${schema}.estados_tarea_workflow AS et LEFT JOIN ${schema}.anotacion_tarea AS at ON at.Inicio_Tareas_Workflow_id_Tarea=et.Inicio_Tareas_Workflow_id_Tarea AND at.Estado_Tarea=1 WHERE et.id_usuario=? AND et.fecha_fin IS NULL AND et.Inicio_Tareas_Workflow_id_Tarea IN (${ownIds.map(() => '?').join(',')}) GROUP BY et.Inicio_Tareas_Workflow_id_Tarea HAVING COUNT(at.Id_Anotacion)=0 ORDER BY et.Inicio_Tareas_Workflow_id_Tarea DESC LIMIT 5`,
        [userId, ...ownIds]
      )
      : [[]];

    console.log('Candidatos DOC-44 (solo identificadores; valide que la tarea propia sea descartable):');
    console.table({
      tareasPropiasActivas: ownIds.join(', ') || 'sin candidato',
      tareasAjenasActivas: foreign.map(row => row.idTarea).join(', ') || 'sin candidato',
      tareasInactivas: inactive.map(row => row.idTarea).join(', ') || 'sin candidato',
      notasCruzadas: notes.map(row => `${row.idNota} (tarea ${row.idTarea})`).join(', ') || 'sin candidato'
      , notasAjenasExtensasMismaTarea: nonOwnerLongNotes.map(row => `${row.idNota} (tarea ${row.idTarea}, ${row.longitud} caracteres)`).join(', ') || 'sin candidato'
      , notasAjenasMismaTarea: nonOwnerNotes.map(row => `${row.idNota} (tarea ${row.idTarea}, ${row.longitud} caracteres)`).join(', ') || 'sin candidato'
      , tareasPropiasConNotaAjena: ownedTasksWithForeignNotes.map(row => `${row.idNota} (tarea ${row.idTarea}, ${row.longitud} caracteres)`).join(', ') || 'sin candidato'
      , tareasPropiasSinNotas: ownedTasksWithoutNotes.map(row => row.idTarea).join(', ') || 'sin candidato'
    });
  } catch (error) {
    const code = typeof error?.code === 'string' && /^[A-Z0-9_]+$/.test(error.code) ? ` (${error.code})` : '';
    console.error(`No fue posible resolver candidatos DOC-44${code}. No se mostraron secretos ni contenido de notas.`);
    process.exitCode = 1;
  } finally {
    values.DOC44_DATA_MYSQL_PASSWORD = '';
    if (connection) await connection.end();
  }
}

main();

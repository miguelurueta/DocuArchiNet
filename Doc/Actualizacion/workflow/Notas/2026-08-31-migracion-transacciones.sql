-- DOC-42 / D-06. Ejecutar únicamente con autorización explícita del ambiente.
-- No contiene credenciales ni se ejecuta como parte de las pruebas locales.
-- El responsable debe ejecutar y conservar primero las consultas SELECT de
-- inventario de motor, columnas e índices. Este script es por esquema y no
-- es idempotente para ALTER/CREATE INDEX: revisar cada línea contra ese
-- inventario antes de aprobarla o ejecutarla.

-- Preflight de revisión (solamente lectura):
SELECT TABLE_NAME, ENGINE
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND UPPER(TABLE_NAME) IN ('ANOTACION_TAREA', 'WF_LOG_WORKFLOW', 'WORKFLOW_NOTAS_IDEMPOTENCIA', 'WORKFLOW_NOTAS_VERSION');

SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_SET_NAME, COLUMN_TYPE
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND ((UPPER(TABLE_NAME) = 'ANOTACION_TAREA' AND UPPER(COLUMN_NAME) = 'DATO_ANOTACION')
       OR UPPER(TABLE_NAME) IN ('WORKFLOW_NOTAS_IDEMPOTENCIA', 'WORKFLOW_NOTAS_VERSION'));

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND UPPER(TABLE_NAME) IN ('ANOTACION_TAREA', 'WF_LOG_WORKFLOW', 'WORKFLOW_NOTAS_IDEMPOTENCIA', 'WORKFLOW_NOTAS_VERSION')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

-- Aplicar solo cuando el inventario aprobado confirme que no existen aún los
-- índices indicados y que la columna conserva semántica TEXT compatible.
ALTER TABLE ANOTACION_TAREA ENGINE=InnoDB;
ALTER TABLE ANOTACION_TAREA MODIFY DATO_ANOTACION TEXT CHARACTER SET utf8 NULL;
CREATE INDEX IX_ANOTACION_OPERATIVA_ORDEN
  ON ANOTACION_TAREA (INICIO_TAREAS_WORKFLOW_ID_TAREA, ESTADO_TAREA, FECHA_ANOTACION, ID_ANOTACION);
CREATE INDEX IX_ANOTACION_HISTORICO_ORDEN
  ON ANOTACION_TAREA (INICIO_TAREAS_WORKFLOW_ID_TAREA, FECHA_ANOTACION, ID_ANOTACION);

ALTER TABLE wf_log_workflow ENGINE=InnoDB;
CREATE INDEX IX_WF_LOG_TAREA_FECHA
  ON wf_log_workflow (ID_TAREA_WORKFLOW, fecha_hora, id_operacion);

-- El esquema objetivo puede ya contener esta tabla. Este DDL queda como
-- referencia para instalaciones que aún no la tengan, respetando sus nombres legacy.
CREATE TABLE IF NOT EXISTS workflow_notas_idempotencia (
  Id_Solicitud BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  Inicio_Tareas_Workflow_id_Tarea BIGINT NOT NULL,
  Id_Usuario_Workflow INT UNSIGNED NOT NULL,
  Client_Request_Id CHAR(36) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  Id_Anotacion BIGINT DEFAULT NULL,
  Version_Resultado CHAR(64) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  Codigo_Resultado VARCHAR(32) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  Fecha_Creacion DATETIME NOT NULL,
  Fecha_Expiracion DATETIME NOT NULL,
  PRIMARY KEY (Id_Solicitud),
  UNIQUE KEY UX_notas_idempotencia_intencion (Inicio_Tareas_Workflow_id_Tarea, Id_Usuario_Workflow, Client_Request_Id),
  KEY IX_notas_idempotencia_expiracion (Fecha_Expiracion)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- No se usa SHA2 de MySQL durante las lecturas ni mutaciones: algunos MySQL 5.1
-- locales no lo habilitan. La aplicación calcula SHA-256 y conserva la versión
-- vigente separada de la respuesta original de idempotencia.
CREATE TABLE IF NOT EXISTS workflow_notas_version (
  Id_Anotacion BIGINT NOT NULL,
  Inicio_Tareas_Workflow_id_Tarea BIGINT NOT NULL,
  Id_Usuario_Workflow INT UNSIGNED NOT NULL,
  Version_Nota CHAR(64) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  Fecha_Actualizacion DATETIME NOT NULL,
  PRIMARY KEY (Id_Anotacion, Inicio_Tareas_Workflow_id_Tarea),
  KEY IX_notas_version_tarea_usuario (Inicio_Tareas_Workflow_id_Tarea, Id_Usuario_Workflow, Id_Anotacion)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Las notas anteriores a DOC-42 no reciben una versión sintética por este DDL.
-- Su eventual backfill debe ser una operación separada, revisada y autorizada;
-- nunca se deriva con SHA2 de MySQL ni se ejecuta desde una lectura.

-- Reversión, solo con autorización y únicamente para objetos introducidos por
-- esta corrida. Revertir ENGINE a MyISAM solo si el inventario previo de esa
-- tabla registró MyISAM; no degradar una tabla que ya era InnoDB.
-- DROP TABLE WORKFLOW_NOTAS_VERSION;
-- DROP TABLE WORKFLOW_NOTAS_IDEMPOTENCIA;
-- DROP INDEX IX_WF_LOG_TAREA_FECHA ON wf_log_workflow;
-- DROP INDEX IX_ANOTACION_HISTORICO_ORDEN ON ANOTACION_TAREA;
-- DROP INDEX IX_ANOTACION_OPERATIVA_ORDEN ON ANOTACION_TAREA;
-- ALTER TABLE wf_log_workflow ENGINE=MyISAM;
-- ALTER TABLE ANOTACION_TAREA ENGINE=MyISAM;

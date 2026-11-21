-- DOC-121 | PREFLIGHT | SOLO SELECT | MySQL 5.1
-- Ejecutar manualmente una vez en RA y otra en DA, seleccionadas por el operador.
-- No contiene USE, credenciales ni consultas de valores de imp01_xmlconexion.
-- Conservar el resultado saneado y comprobarlo antes de ejecutar cualquier DDL.
SELECT VERSION() AS version_servidor, DATABASE() AS base_seleccionada;

SELECT ENGINE, SUPPORT, TRANSACTIONS
FROM information_schema.ENGINES
WHERE ENGINE = 'InnoDB';

SELECT TABLE_NAME, ENGINE, TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND (TABLE_NAME IN ('remit_dest_interno_perfil_produccion',
       'imp01_xmlconexion', 'imp01_plantillaimp', 'imp01_campos_plantilla',
       'imp01_carga_ejecucion', 'ra_integracion_fuente',
       'ra_integracion_fuente_columna', 'ra_integracion_proveedor',
       'ra_integracion_proveedor_version') OR LEFT(TABLE_NAME, 7) = 'imp_02_')
ORDER BY TABLE_NAME;

SELECT TABLE_NAME, ORDINAL_POSITION, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE,
       COLUMN_DEFAULT, CHARACTER_MAXIMUM_LENGTH, CHARACTER_SET_NAME,
       COLLATION_NAME, EXTRA, COLUMN_COMMENT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND (TABLE_NAME IN ('imp01_xmlconexion', 'imp01_plantillaimp',
       'imp01_campos_plantilla', 'imp01_carga_ejecucion')
       OR LEFT(TABLE_NAME, 7) = 'imp_02_'
       OR (TABLE_NAME = 'remit_dest_interno_perfil_produccion'
           AND COLUMN_NAME IN ('remit_dest_interno_idremit_dest_interno',
                               'ADMINISTRAR_CARGAS_IMPORTACION')))
ORDER BY TABLE_NAME, ORDINAL_POSITION;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND (TABLE_NAME IN ('imp01_xmlconexion', 'imp01_plantillaimp',
       'imp01_campos_plantilla', 'imp01_carga_ejecucion')
       OR LEFT(TABLE_NAME, 7) = 'imp_02_')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

SELECT TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME,
       REFERENCED_TABLE_SCHEMA, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('imp01_xmlconexion', 'imp01_plantillaimp', 'imp01_campos_plantilla')
ORDER BY TABLE_NAME, CONSTRAINT_NAME, ORDINAL_POSITION;

SELECT EVENT_OBJECT_TABLE, TRIGGER_NAME, EVENT_MANIPULATION, ACTION_TIMING
FROM information_schema.TRIGGERS
WHERE TRIGGER_SCHEMA = DATABASE()
  AND (EVENT_OBJECT_TABLE IN ('imp01_xmlconexion', 'imp01_plantillaimp',
       'imp01_campos_plantilla', 'imp01_carga_ejecucion')
       OR LEFT(EVENT_OBJECT_TABLE, 7) = 'imp_02_')
ORDER BY EVENT_OBJECT_TABLE, TRIGGER_NAME;

-- Metadatos de privilegios visibles a la cuenta de esta consulta.
-- Ausencia de filas no prueba ausencia de privilegios de una cuenta diferente.
SELECT GRANTEE, TABLE_SCHEMA, PRIVILEGE_TYPE, IS_GRANTABLE
FROM information_schema.SCHEMA_PRIVILEGES
WHERE TABLE_SCHEMA = DATABASE()
ORDER BY GRANTEE, PRIVILEGE_TYPE;

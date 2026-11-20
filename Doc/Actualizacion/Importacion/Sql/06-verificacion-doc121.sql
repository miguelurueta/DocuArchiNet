-- DOC-121 | POSTFLIGHT | SOLO SELECT | Ejecutar en RA y DA correspondientes.
-- El agente revisa este resultado despues de confirmar el usuario su aplicacion.
-- Esperado RA: una columna tinyint unsigned, NOT NULL, DEFAULT 0.
SELECT TABLE_NAME, ORDINAL_POSITION, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'remit_dest_interno_perfil_produccion'
  AND COLUMN_NAME = 'ADMINISTRAR_CARGAS_IMPORTACION';

-- Esperado DA: dos columnas nuevas al final; conservar ordinales historicos.
SELECT TABLE_NAME, ORDINAL_POSITION, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE,
       COLUMN_DEFAULT, CHARACTER_SET_NAME, COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('imp01_plantillaimp', 'imp01_carga_ejecucion')
ORDER BY TABLE_NAME, ORDINAL_POSITION;

-- Esperado tabla operativa: InnoDB/ASCII; 22 columnas, PK y tres indices.
SELECT TABLE_NAME, ENGINE, TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'imp01_carga_ejecucion';

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND (TABLE_NAME = 'imp01_carga_ejecucion' OR LEFT(TABLE_NAME, 7) = 'imp_02_')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

-- Ninguna FK nueva debe depender de la tabla operativa; conservar FKs legacy.
SELECT TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME,
       REFERENCED_TABLE_SCHEMA, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('imp01_plantillaimp', 'imp01_campos_plantilla', 'imp01_carga_ejecucion')
ORDER BY TABLE_NAME, CONSTRAINT_NAME, ORDINAL_POSITION;

SELECT EVENT_OBJECT_TABLE, TRIGGER_NAME
FROM information_schema.TRIGGERS
WHERE TRIGGER_SCHEMA = DATABASE() AND EVENT_OBJECT_TABLE = 'imp01_carga_ejecucion';

-- Privilegios visibles, no contrasenas ni valores de imp01_xmlconexion.
-- La cuenta que consulta puede no ver las concesiones de la cuenta dedicada.
SELECT GRANTEE, TABLE_SCHEMA, PRIVILEGE_TYPE, IS_GRANTABLE
FROM information_schema.SCHEMA_PRIVILEGES
WHERE TABLE_SCHEMA = DATABASE()
ORDER BY GRANTEE, PRIVILEGE_TYPE;

-- DOC-125 | DIAGNOSTICO MANUAL | SOLO SELECT | MySQL 5.1.37
-- Estado de entrega: PENDIENTE_EJECUCION_MANUAL. Este archivo no instala nada.
-- Ejecutar los bloques por separado en RA y DA del modulo autorizado.
-- Usar la cuenta de aplicacion correspondiente para comprobar visibilidad real;
-- un resultado obtenido como administrador no acredita los permisos del escritor.
-- No exportar credenciales, cadenas de conexion ni filas de negocio.
-- Un resultado vacio de TRIGGERS sin privilegio TRIGGER no prueba ausencia.

-- BLOQUE DA: base de datos de las plantillas/cargas/destinos del modulo.
-- Identificar el destino seleccionado y capacidades; sin modificar sql_mode.
SELECT 'DOC125_DA_CAPACIDADES' AS control,
       DATABASE() AS base_seleccionada, VERSION() AS version_mysql,
       @@lower_case_table_names AS nombres_tabla,
       @@max_allowed_packet AS max_allowed_packet,
       @@session.sql_mode AS modo_sesion;

SELECT ENGINE, SUPPORT, TRANSACTIONS
FROM information_schema.ENGINES
WHERE ENGINE = 'InnoDB';

-- Las tablas de ejecucion/configuracion deben ser InnoDB.
-- Se listan los destinos imp_02_ visibles sin leer su contenido.
SELECT TABLE_NAME, TABLE_TYPE, ENGINE, TABLE_COLLATION
FROM information_schema.TABLES
WHERE BINARY TABLE_SCHEMA = BINARY DATABASE()
  AND (TABLE_NAME IN ('imp01_carga_ejecucion', 'imp01_plantillaimp',
                      'imp01_campos_plantilla')
       OR LEFT(TABLE_NAME, 7) = 'imp_02_')
ORDER BY TABLE_NAME;

-- Tabla operativa: comparar las 22 columnas con SQL03 de DOC121.
-- Incluye filas_confirmadas/confirmado_utc ya instaladas, sin nueva migracion.
SELECT COLUMN_NAME, ORDINAL_POSITION, COLUMN_TYPE, IS_NULLABLE,
       COLUMN_DEFAULT, CHARACTER_SET_NAME, COLLATION_NAME, EXTRA
FROM information_schema.COLUMNS
WHERE BINARY TABLE_SCHEMA = BINARY DATABASE()
  AND BINARY TABLE_NAME = BINARY 'imp01_carga_ejecucion'
ORDER BY ORDINAL_POSITION;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, SUB_PART
FROM information_schema.STATISTICS
WHERE BINARY TABLE_SCHEMA = BINARY DATABASE()
  AND (TABLE_NAME IN ('imp01_carga_ejecucion', 'imp01_plantillaimp',
                      'imp01_campos_plantilla')
       OR LEFT(TABLE_NAME, 7) = 'imp_02_')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

-- Privilegios globales/de DA visibles para la propia cuenta, sin imprimirla.
-- Completar con privilegios por tabla si no existe una concesion de base.
-- La aplicacion debe acreditar visibilidad completa, no inferirla de cero filas.
SELECT 'GLOBAL' AS nivel, PRIVILEGE_TYPE
FROM information_schema.USER_PRIVILEGES
WHERE GRANTEE = CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(), '@', 1)),
                       '@', QUOTE(SUBSTRING_INDEX(CURRENT_USER(), '@', -1)))
UNION ALL
SELECT 'BASE_DA' AS nivel, PRIVILEGE_TYPE
FROM information_schema.SCHEMA_PRIVILEGES
WHERE BINARY TABLE_SCHEMA = BINARY DATABASE()
  AND GRANTEE = CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(), '@', 1)),
                       '@', QUOTE(SUBSTRING_INDEX(CURRENT_USER(), '@', -1)))
ORDER BY nivel, PRIVILEGE_TYPE;

SELECT TABLE_NAME, PRIVILEGE_TYPE
FROM information_schema.TABLE_PRIVILEGES
WHERE BINARY TABLE_SCHEMA = BINARY DATABASE()
  AND GRANTEE = CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(), '@', 1)),
                       '@', QUOTE(SUBSTRING_INDEX(CURRENT_USER(), '@', -1)))
  AND (TABLE_NAME = 'imp01_carga_ejecucion'
       OR LEFT(TABLE_NAME, 7) = 'imp_02_')
ORDER BY TABLE_NAME, PRIVILEGE_TYPE;

-- No seleccionar ACTION_STATEMENT/DEFINER: basta identificar efectos asociados.
SELECT EVENT_OBJECT_TABLE AS tabla, TRIGGER_NAME, EVENT_MANIPULATION,
       ACTION_TIMING
FROM information_schema.TRIGGERS
WHERE BINARY EVENT_OBJECT_SCHEMA = BINARY DATABASE()
  AND (EVENT_OBJECT_TABLE = 'imp01_carga_ejecucion'
       OR LEFT(EVENT_OBJECT_TABLE, 7) = 'imp_02_')
ORDER BY EVENT_OBJECT_TABLE, TRIGGER_NAME;

-- Referencias salientes y entrantes, incluidas las de otras bases visibles.
-- Visibilidad parcial impide acreditar la ausencia de relaciones.
SELECT TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME,
       REFERENCED_TABLE_SCHEMA, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE REFERENCED_TABLE_NAME IS NOT NULL
  AND ((BINARY TABLE_SCHEMA = BINARY DATABASE()
        AND (TABLE_NAME = 'imp01_carga_ejecucion'
             OR LEFT(TABLE_NAME, 7) = 'imp_02_'))
       OR (BINARY REFERENCED_TABLE_SCHEMA = BINARY DATABASE()
           AND (REFERENCED_TABLE_NAME = 'imp01_carga_ejecucion'
                OR LEFT(REFERENCED_TABLE_NAME, 7) = 'imp_02_')))
ORDER BY TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_NAME, ORDINAL_POSITION;

-- FIN DA. No confirmar cargas, crear fixtures ni modificar tablas desde aqui.

-- BLOQUE RA: cambiar MANUALMENTE a la base RA autorizada antes de este bloque.
-- Comprobar presencia de autoridad y politica, sin consultar datos de usuarios.
SELECT 'DOC125_RA_ESTRUCTURA' AS control, TABLE_NAME, COLUMN_NAME,
       COLUMN_TYPE, IS_NULLABLE
FROM information_schema.COLUMNS
WHERE BINARY TABLE_SCHEMA = BINARY DATABASE()
  AND ((TABLE_NAME = 'remit_dest_interno_perfil_produccion'
        AND COLUMN_NAME = 'ADMINISTRAR_CARGAS_IMPORTACION')
       OR (TABLE_NAME = 'ra_integracion_fuente'
           AND COLUMN_NAME IN ('fuente_id', 'revision', 'activo',
                               'conexion_ref', 'tabla_nombre', 'ambito_ref',
                               'modo_aislamiento', 'columna_empresa')))
ORDER BY TABLE_NAME, ORDINAL_POSITION;

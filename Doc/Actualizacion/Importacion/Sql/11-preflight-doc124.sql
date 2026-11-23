-- DOC-124. ENTREGA PARA EJECUCION MANUAL; SOLO SELECT. MySQL 5.1.37.
-- Ejecutar RA/DA por separado, en recursos vigentes del modulo. Sin USE/SET/DDL.
-- No reemplaza confirmacion de instalacion ni autoriza la E2E.

-- DA: capacidad observable; no retorna usuario, host, base ni secretos.
SELECT VERSION() AS version_motor,@@max_allowed_packet AS max_allowed_packet,
       @@session.sql_mode AS modo_sesion,
       @@character_set_database AS charset_proyectado,
       @@collation_database AS collation_proyectada;

SELECT ENGINE,SUPPORT,TRANSACTIONS FROM information_schema.ENGINES
WHERE ENGINE='InnoDB';

-- DA: imp01_carga_ejecucion debe conservar las 22 columnas del SQL03, motor
-- InnoDB, PK completa id_carga y hashes BINARY(32). No se reinstala.
SELECT TABLE_NAME,ENGINE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp01_carga_ejecucion','imp01_plantillaimp','imp01_campos_plantilla');

SELECT TABLE_NAME,ORDINAL_POSITION,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,
       COLUMN_DEFAULT,CHARACTER_MAXIMUM_LENGTH,CHARACTER_SET_NAME,COLLATION_NAME,EXTRA
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp01_carga_ejecucion','imp01_plantillaimp','imp01_campos_plantilla')
ORDER BY TABLE_NAME,ORDINAL_POSITION;

SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp01_carga_ejecucion'
ORDER BY INDEX_NAME,SEQ_IN_INDEX;

-- DA: inventario de destinos permitidos, sin elegir EGRESOS/CONTRATOS ni leer
-- datos de negocio. Revisar la tabla de prueba concreta en la copia local.
SELECT TABLE_NAME,ENGINE,TABLE_COLLATION,TABLE_TYPE
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND LEFT(BINARY TABLE_NAME,7)='imp_02_'
ORDER BY TABLE_NAME;

SELECT TABLE_NAME,ORDINAL_POSITION,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,
       COLUMN_DEFAULT,CHARACTER_MAXIMUM_LENGTH,CHARACTER_SET_NAME,COLLATION_NAME,EXTRA
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND LEFT(BINARY TABLE_NAME,7)='imp_02_'
ORDER BY TABLE_NAME,ORDINAL_POSITION;

-- UNIQUE completo: clave requerida; en COLUMNA, empresa+clave requeridas.
-- Cada componente tiene SUB_PART NULL; rechazar componentes adicionales.
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND LEFT(BINARY TABLE_NAME,7)='imp_02_'
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;

SELECT CHARACTER_SET_NAME,MAXLEN FROM information_schema.CHARACTER_SETS
WHERE CHARACTER_SET_NAME=@@character_set_database;

-- DA: privilegios visibles de la cuenta efectiva, sin imprimir su identidad.
-- Preparar necesita CREATE TEMPORARY TABLES y SELECT, ademas de INSERT/UPDATE
-- del registro de ejecucion. Este SELECT NO concede nada ni prueba por si solo
-- privilegios efectivos de todos los niveles.
SELECT PRIVILEGE_TYPE,IS_GRANTABLE FROM information_schema.SCHEMA_PRIVILEGES
WHERE TABLE_SCHEMA=DATABASE()
  AND GRANTEE=CONCAT(CHAR(39),SUBSTRING_INDEX(CURRENT_USER(),'@',1),CHAR(39),'@',
                     CHAR(39),SUBSTRING_INDEX(CURRENT_USER(),'@',-1),CHAR(39));

-- RA: permiso propio y catalogo de aislamiento existente. No cambia permisos.
SELECT TABLE_NAME,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND ((TABLE_NAME='remit_dest_interno_perfil_produccion' AND COLUMN_NAME='ADMINISTRAR_CARGAS_IMPORTACION')
       OR (TABLE_NAME='ra_integracion_fuente' AND COLUMN_NAME IN
           ('fuente_id','conexion_ref','tabla_nombre','ambito_ref','modo_aislamiento','columna_empresa','activo','revision')))
ORDER BY TABLE_NAME,ORDINAL_POSITION;

-- RA: politica explicita. Los resultados son configuracion interna, no reporte
-- publico. CONEXION no se deduce por prefijo ni ausencia de columna empresarial.
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,
       columna_empresa,activo,revision
FROM ra_integracion_fuente
WHERE conexion_ref='DA' AND LEFT(BINARY tabla_nombre,7)='imp_02_'
ORDER BY tabla_nombre,ambito_ref,fuente_id;

-- Cualquier diferencia permanente se entrega en OTRO archivo SQL especifico.
-- No sembrar/limpiar/arreglar destinos desde tests o al iniciar la aplicacion.

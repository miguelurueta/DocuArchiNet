-- DOC127 | SOLO SELECT | MySQL 5.1.37 | sin instalar, corregir ni limpiar.
-- Ejecutar bloques POR SEPARADO en RA/DA confirmadas; no ejecutar todo en una base.
-- No reaplicar 00-27. Una diferencia requiere otro archivo revisable.
-- La visibilidad parcial de metadatos no acredita ausencia de relaciones/permisos.

-- A. RA AUTORIZADA: estructura del permiso funcional y politicas existentes.
SELECT 'DOC127_RA' AS control, DATABASE() AS base_actual, VERSION() AS version;
SELECT COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME='remit_dest_interno_perfil_produccion'
  AND COLUMN_NAME IN ('ADMINISTRAR_CARGAS_IMPORTACION','IMPORTAR_CARPETAS',
    'ADMINISTRAR_PERFILES_IMPORTACION','ADMINISTRAR_FUENTES_IMPORTACION',
    'ADMINISTRAR_PROVEEDORES_IMPORTACION')
ORDER BY ORDINAL_POSITION;

-- Completar los tres NULL en una copia local despues de identificar las cuentas.
-- No devuelve nombres de login, claves, tokens ni columnas ajenas a los permisos.
-- Debe devolver una fila por cuenta; no habilitar permisos para fabricar un PASS.
SELECT u.id_Remit_Dest_Int AS usuario_id, u.Estado_Usuario AS activo,
  p.ADMINISTRAR_CARGAS_IMPORTACION AS cargas, p.IMPORTAR_CARPETAS AS carpetas,
  p.ADMINISTRAR_PERFILES_IMPORTACION AS perfiles,
  p.ADMINISTRAR_FUENTES_IMPORTACION AS fuentes,
  p.ADMINISTRAR_PROVEEDORES_IMPORTACION AS proveedores
FROM remit_dest_interno u JOIN remit_dest_interno_perfil_produccion p
  ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int
CROSS JOIN (SELECT CAST(NULL AS UNSIGNED) AS empresa_id,
  CAST(NULL AS UNSIGNED) AS propietario_id,
  CAST(NULL AS UNSIGNED) AS restringido_id) alcance
WHERE u.Empresa_Gestion_Documental_id_empresa=alcance.empresa_id
  AND u.id_Remit_Dest_Int IN (alcance.propietario_id,alcance.restringido_id);

SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,
  columna_empresa,activo,revision
FROM ra_integracion_fuente
WHERE ambito_ref='2' AND LEFT(tabla_nombre,7)='imp_02_'
ORDER BY fuente_id;

-- Contrato de proveedores TABLA; propiedades_doc contiene mapeos, no datos origen.
-- Un proveedor PUBLICADA/inactivo no es una publicacion operativa autorizada.
SELECT p.proveedor_id,v.version_id,p.activo,v.estado,v.conexion_ref,
  v.tabla_nombre,v.clave_columna,v.clave_tipo,v.empresa_columna,
  v.propiedades_doc,v.revision
FROM ra_integracion_proveedor p JOIN ra_integracion_proveedor_version v
  ON v.proveedor_id=p.proveedor_id
WHERE p.ambito_ref='2' AND v.tipo='TABLA' AND v.esquema_config=1
  AND LEFT(v.tabla_nombre,7)='imp_02_'
ORDER BY p.proveedor_id,v.numero;

-- FIN RA. Seleccionar MANUALMENTE la DA del modulo autorizado antes de continuar.

-- B. DA: contrato base y destinos existentes; no consultar valores de conexion.
SELECT 'DOC127_DA' AS control,DATABASE() AS base_actual,VERSION() AS version,
  @@max_allowed_packet AS max_allowed_packet,
  @@character_set_database AS charset_base,@@collation_database AS collation_base;
SELECT TABLE_NAME,ENGINE,TABLE_TYPE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE()
  AND (TABLE_NAME IN ('imp01_xmlconexion','imp01_plantillaimp',
    'imp01_campos_plantilla','imp01_carga_ejecucion')
    OR LEFT(TABLE_NAME,7)='imp_02_')
ORDER BY TABLE_NAME;

-- Comparar las 22 columnas con 03-ejecucion-da-doc121.sql, no solo su cantidad.
-- CAMPO_UNICO y REVISION_CARGA conservan ordinales finales 11/12 de la cabecera.
SELECT TABLE_NAME,COLUMN_NAME,ORDINAL_POSITION,COLUMN_TYPE,IS_NULLABLE,
  COLUMN_DEFAULT,CHARACTER_SET_NAME,COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND (TABLE_NAME='imp01_carga_ejecucion'
    OR (TABLE_NAME='imp01_plantillaimp'
      AND COLUMN_NAME IN ('CAMPO_UNICO','REVISION_CARGA'))
    OR (LEFT(TABLE_NAME,7)='imp_02_'))
ORDER BY TABLE_NAME,ORDINAL_POSITION;
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE()
  AND (TABLE_NAME='imp01_carga_ejecucion' OR LEFT(TABLE_NAME,7)='imp_02_')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;

-- Sin ACTION_STATEMENT, DEFINER ni datos de credenciales.
SELECT EVENT_OBJECT_TABLE,TRIGGER_NAME,EVENT_MANIPULATION,ACTION_TIMING
FROM information_schema.TRIGGERS
WHERE EVENT_OBJECT_SCHEMA=DATABASE()
  AND (EVENT_OBJECT_TABLE='imp01_carga_ejecucion'
    OR LEFT(EVENT_OBJECT_TABLE,7)='imp_02_')
ORDER BY EVENT_OBJECT_TABLE,TRIGGER_NAME;
SELECT TABLE_SCHEMA,TABLE_NAME,CONSTRAINT_NAME,COLUMN_NAME,
  REFERENCED_TABLE_SCHEMA,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE REFERENCED_TABLE_NAME IS NOT NULL
  AND ((TABLE_SCHEMA=DATABASE()
    AND (TABLE_NAME='imp01_carga_ejecucion' OR LEFT(TABLE_NAME,7)='imp_02_'))
    OR (REFERENCED_TABLE_SCHEMA=DATABASE()
    AND (REFERENCED_TABLE_NAME='imp01_carga_ejecucion'
      OR LEFT(REFERENCED_TABLE_NAME,7)='imp_02_')))
ORDER BY TABLE_SCHEMA,TABLE_NAME,CONSTRAINT_NAME,ORDINAL_POSITION;

-- Capacidades observables de la cuenta efectiva, sin mostrar su identidad.
SELECT 'GLOBAL' AS nivel,PRIVILEGE_TYPE
FROM information_schema.USER_PRIVILEGES
WHERE GRANTEE=CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',1)),
  '@',QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',-1)))
UNION ALL
SELECT 'DA' AS nivel,PRIVILEGE_TYPE
FROM information_schema.SCHEMA_PRIVILEGES
WHERE TABLE_SCHEMA=DATABASE()
  AND GRANTEE=CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',1)),
    '@',QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',-1)));
SELECT TABLE_NAME,PRIVILEGE_TYPE
FROM information_schema.TABLE_PRIVILEGES
WHERE TABLE_SCHEMA=DATABASE()
  AND (TABLE_NAME='imp01_carga_ejecucion' OR LEFT(TABLE_NAME,7)='imp_02_')
  AND GRANTEE=CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',1)),
    '@',QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',-1)))
ORDER BY TABLE_NAME,PRIVILEGE_TYPE;

SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,
  IMP01_XMLCONEXION_ID_XMLCONEXION AS conexion_id,CONEXION_FUENTE,LIBRO_EXCEL,
  CAMPO_UNICO,REVISION_CARGA,ESTADO_PLANTILLA,ELIMINA_TABLA,
  ELIMINA_INSERTA_RECORD,INSERTA_RECORD
FROM imp01_plantillaimp
WHERE LEFT(NOMBRE_PLANTILLA,7)='imp_02_'
ORDER BY ID_PLANTILLAIMP;

-- Estado de los cinco resultados DOC126: conservar, no repetir las operaciones.
SELECT id_carga,id_plantilla,tabla_destino,modo,estado,filas_previstas,
  filas_confirmadas,codigo_incidencia,LOWER(HEX(archivo_hash)) AS archivo_hash
FROM imp01_carga_ejecucion
WHERE ambito_ref='2' AND id_plantilla IN (57,58,59,60)
ORDER BY creado_utc,id_carga;

-- FIN DA. Las huellas completas por ventana se obtienen con los controles E2E
-- existentes, antes/despues de una prueba expresamente autorizada.

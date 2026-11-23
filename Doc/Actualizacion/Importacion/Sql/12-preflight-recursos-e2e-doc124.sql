-- DOC-124. REUTILIZACION DE RECURSOS EXISTENTES. SOLO SELECT; EJECUCION MANUAL.
-- No modifica datos, indices, politicas, plantillas, credenciales o permisos.
-- No exige crear tablas nuevas para probar Preparar; usa fuentes ya consumidas.

-- RA: mismas fuentes/proveedores que consumen las E2E DOC110.
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,
       columna_empresa,activo,revision
FROM ra_integracion_fuente
WHERE conexion_ref='DA' AND tabla_nombre IN ('imp_02_egresos','imp_02_creditos')
ORDER BY tabla_nombre,fuente_id;

SELECT DISTINCT conexion_ref,tabla_nombre
FROM ra_integracion_proveedor_version
WHERE conexion_ref='DA' AND tabla_nombre IN ('imp_02_egresos','imp_02_creditos')
ORDER BY tabla_nombre;

-- DA: distinguir la plantilla historica del archivo de su tabla fisica.
-- No cambiar NOMBRE_PLANTILLA o conexion: otros consumidores los reutilizan.
SELECT ID_PLANTILLAIMP,IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,
       CAMPO_UNICO,REVISION_CARGA,LIBRO_EXCEL,CONEXION_FUENTE,
       ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA
FROM imp01_plantillaimp
WHERE NOMBRE_PLANTILLA IN ('EGRESOS','CREDITOS','imp_02_egresos','imp_02_creditos')
ORDER BY ID_PLANTILLAIMP;

SELECT c.ID_CAMPOS_PLANTILLA,c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,
       c.CAMPO_FUENTE,c.CAMPO_DESTINO,c.TIPO_CAMPO
FROM imp01_campos_plantilla c
INNER JOIN imp01_plantillaimp p
        ON p.ID_PLANTILLAIMP=c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP
WHERE p.NOMBRE_PLANTILLA IN ('EGRESOS','CREDITOS','imp_02_egresos','imp_02_creditos')
ORDER BY c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,c.ID_CAMPOS_PLANTILLA;

SELECT TABLE_NAME,ENGINE,TABLE_TYPE FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp_02_egresos','imp_02_creditos','imp01_carga_ejecucion');

SELECT TABLE_NAME,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,
       CHARACTER_MAXIMUM_LENGTH,CHARACTER_SET_NAME,COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp_02_egresos','imp_02_creditos')
ORDER BY TABLE_NAME,ORDINAL_POSITION;

SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp_02_egresos','imp_02_creditos')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;

-- Conteos de incidencias sin publicar claves/beneficiarios. No reparar datos.
SELECT 'imp_02_egresos' AS fuente,COUNT(*) AS filas,
       SUM(CONSECUTIVO IS NULL) AS claves_nulas,
       SUM(CONSECUTIVO IS NOT NULL AND TRIM(CONSECUTIVO)='') AS claves_vacias
FROM imp_02_egresos;

SELECT 'imp_02_egresos' AS fuente,COUNT(*) AS grupos_duplicados
FROM (SELECT CONSECUTIVO FROM imp_02_egresos
      WHERE CONSECUTIVO IS NOT NULL GROUP BY CONSECUTIVO HAVING COUNT(*)>1) d;

SELECT 'imp_02_creditos' AS fuente,COUNT(*) AS filas,
       SUM(CONSECUTIVO IS NULL) AS claves_nulas,
       SUM(CONSECUTIVO IS NOT NULL AND TRIM(CONSECUTIVO)='') AS claves_vacias
FROM imp_02_creditos;

SELECT 'imp_02_creditos' AS fuente,COUNT(*) AS grupos_duplicados
FROM (SELECT CONSECUTIVO FROM imp_02_creditos
      WHERE CONSECUTIVO IS NOT NULL GROUP BY CONSECUTIVO HAVING COUNT(*)>1) d;

-- Una PK tecnica IMP_02_ID_CLAVE no reemplaza UNIQUE completo de CONSECUTIVO.
-- Si faltan condiciones, acreditar rechazo; no marcar PREPARADA ni modificar
-- tablas/politicas/plantillas para convertir el caso en un exito artificial.

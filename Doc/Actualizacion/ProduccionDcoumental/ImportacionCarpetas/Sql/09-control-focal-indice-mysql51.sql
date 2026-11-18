-- APLICACION MANUAL en RA. Respaldar esquema antes de ejecutar.
-- Solo agrega un indice a ra_integracion_evento. No crea tablas/columnas,
-- no cambia documentos, perfiles, intentos ni datos de negocio.
-- No ejecutar por un despliegue de DLL ni desde la aplicacion.
SET @doc120_indice_columnas = (
  SELECT GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX SEPARATOR ',')
  FROM information_schema.STATISTICS
  WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_integracion_evento'
    AND INDEX_NAME='ix_integracion_evento_recurso'
);
SET @doc120_indice_partes_validas = (
  SELECT COUNT(*) FROM information_schema.STATISTICS
  WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_integracion_evento'
    AND INDEX_NAME='ix_integracion_evento_recurso' AND NON_UNIQUE=1 AND SUB_PART IS NULL
);
SET @doc120_ddl_indice = IF(@doc120_indice_columnas IS NULL,
  'ALTER TABLE ra_integracion_evento ADD INDEX ix_integracion_evento_recurso (lote_id,recurso_tipo,recurso_id,accion)',
  IF(@doc120_indice_columnas='lote_id,recurso_tipo,recurso_id,accion' AND @doc120_indice_partes_validas=4,
    'SELECT ''DOC120_INDICE_YA_COMPATIBLE'' AS resultado',
    'SELECT ''DOC120_ERROR_INDICE_INCOMPATIBLE_NO_MODIFICADO'' AS resultado'));
PREPARE doc120_indice_stmt FROM @doc120_ddl_indice;
EXECUTE doc120_indice_stmt;
DEALLOCATE PREPARE doc120_indice_stmt;
SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_integracion_evento'
  AND INDEX_NAME='ix_integracion_evento_recurso'
ORDER BY SEQ_IN_INDEX;
SET @doc120_indice_columnas=NULL;
SET @doc120_indice_partes_validas=NULL;
SET @doc120_ddl_indice=NULL;

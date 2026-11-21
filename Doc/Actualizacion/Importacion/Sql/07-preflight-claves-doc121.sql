-- DOC-121 | DA seleccionada por el operador | SOLO SELECT
-- Control previo a cualquier adecuacion. No corrige ni elimina filas.
-- Revisar ambiente, atributos e indices antes de ejecutar el archivo 08.
SELECT VERSION() AS version_servidor, DATABASE() AS base_seleccionada;

SELECT c.TABLE_NAME, t.ENGINE, c.COLUMN_NAME, c.COLUMN_TYPE, c.IS_NULLABLE,
       c.COLUMN_DEFAULT, c.CHARACTER_SET_NAME, c.COLLATION_NAME,
       c.COLUMN_COMMENT, c.EXTRA
FROM information_schema.COLUMNS c
INNER JOIN information_schema.TABLES t
  ON t.TABLE_SCHEMA=c.TABLE_SCHEMA AND t.TABLE_NAME=c.TABLE_NAME
WHERE c.TABLE_SCHEMA=DATABASE()
  AND ((c.TABLE_NAME='imp_02_egresos' AND c.COLUMN_NAME='CONSECUTIVO')
    OR (c.TABLE_NAME='imp_02_contratos' AND c.COLUMN_NAME='NCONTRATO'))
ORDER BY c.TABLE_NAME;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp_02_egresos','imp_02_contratos')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

SELECT COUNT(*) AS filas,
       COALESCE(SUM(CONSECUTIVO IS NULL),0) AS claves_nulas,
       COALESCE(SUM(CONSECUTIVO IS NOT NULL AND TRIM(CONSECUTIVO)=''),0) AS claves_vacias
FROM imp_02_egresos;

SELECT COUNT(*) AS grupos_duplicados,
       COALESCE(SUM(d.cantidad-1),0) AS filas_excedentes_en_grupos
FROM (SELECT COUNT(*) AS cantidad
      FROM imp_02_egresos
      WHERE CONSECUTIVO IS NOT NULL AND TRIM(CONSECUTIVO)<>''
      GROUP BY CONSECUTIVO HAVING COUNT(*)>1) d;

SELECT COUNT(*) AS filas,
       COALESCE(SUM(NCONTRATO IS NULL),0) AS claves_nulas,
       COALESCE(SUM(NCONTRATO IS NOT NULL AND TRIM(NCONTRATO)=''),0) AS claves_vacias
FROM imp_02_contratos;

SELECT COUNT(*) AS grupos_duplicados,
       COALESCE(SUM(d.cantidad-1),0) AS filas_excedentes_en_grupos
FROM (SELECT COUNT(*) AS cantidad
      FROM imp_02_contratos
      WHERE NCONTRATO IS NOT NULL AND TRIM(NCONTRATO)<>''
      GROUP BY NCONTRATO HAVING COUNT(*)>1) d;

-- Autoridad de fuente registrada en esta misma base, cuando corresponda a RA.
-- Si RA es otra base, ejecutar esta consulta alli. No inferir aislamiento
-- compartido/exclusivo por la mera ausencia de una fila en el catalogo.
SELECT conexion_ref, tabla_nombre, modo_aislamiento, columna_empresa, activo
FROM ra_integracion_fuente
WHERE activo=1 AND tabla_nombre IN ('imp_02_egresos','imp_02_contratos');

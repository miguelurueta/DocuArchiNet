-- DOC-121 | POSTFLIGHT DE CLAVES | SOLO SELECT
-- Ejecutar despues de confirmar manualmente el ALTER pertinente.
-- CONTRATOS: NCONTRATO NOT NULL, mismo tipo/charset/collation,
-- UNIQUE completo de una columna con NON_UNIQUE=0 y SUB_PART NULL.
-- EGRESOS permanece sin adecuacion hasta resolver ambiente y datos.
SELECT VERSION() AS version_servidor, DATABASE() AS base_seleccionada;

SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE,
       COLUMN_DEFAULT, CHARACTER_SET_NAME, COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND ((TABLE_NAME='imp_02_contratos' AND COLUMN_NAME='NCONTRATO')
    OR (TABLE_NAME='imp_02_egresos' AND COLUMN_NAME='CONSECUTIVO'))
ORDER BY TABLE_NAME;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp_02_contratos','imp_02_egresos')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

SELECT COUNT(*) AS filas_contratos,
       COALESCE(SUM(NCONTRATO IS NULL),0) AS claves_nulas,
       COALESCE(SUM(NCONTRATO IS NOT NULL AND TRIM(NCONTRATO)=''),0) AS claves_vacias
FROM imp_02_contratos;

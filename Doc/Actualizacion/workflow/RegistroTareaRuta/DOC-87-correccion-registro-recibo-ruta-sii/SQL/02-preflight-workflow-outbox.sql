-- DOC-87 / control de solo lectura.
SELECT TABLE_NAME, ENGINE
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'workflow_registro_ruta_sii_outbox';

SELECT INDEX_NAME, NON_UNIQUE, GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) AS columns_in_index
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'workflow_registro_ruta_sii_outbox'
GROUP BY INDEX_NAME, NON_UNIQUE;

SELECT status, COUNT(*) AS event_count
FROM workflow_registro_ruta_sii_outbox
GROUP BY status;

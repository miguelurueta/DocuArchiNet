DROP PROCEDURE IF EXISTS doc67_assert_document_link_cache_empty;
DELIMITER $$
CREATE PROCEDURE doc67_assert_document_link_cache_empty()
BEGIN
  IF EXISTS (
    SELECT 1 FROM information_schema.TABLES
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_document_link_cache'
  ) THEN
    SET @doc67_cache_rows = 0;
    SET @doc67_sql = 'SELECT COUNT(*) INTO @doc67_cache_rows FROM workflow_import_document_link_cache';
    PREPARE doc67_statement FROM @doc67_sql;
    EXECUTE doc67_statement;
    DEALLOCATE PREPARE doc67_statement;
    IF @doc67_cache_rows > 0 THEN
      SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'DOC-67 rollback detenido: la cache documental contiene verificaciones; exporte o conserve los datos';
    END IF;
  END IF;
END$$
DELIMITER ;

CALL doc67_assert_document_link_cache_empty();
DROP PROCEDURE IF EXISTS doc67_assert_document_link_cache_empty;
DROP TABLE IF EXISTS workflow_import_document_link_cache;


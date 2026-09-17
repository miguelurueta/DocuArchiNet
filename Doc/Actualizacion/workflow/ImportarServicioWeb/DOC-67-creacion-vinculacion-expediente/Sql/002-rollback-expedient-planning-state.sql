DROP PROCEDURE IF EXISTS doc67_assert_empty;
DELIMITER $$
CREATE PROCEDURE doc67_assert_empty()
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_related_document') THEN
    SET @doc67_rows = 0;
    SET @doc67_sql = 'SELECT COUNT(*) INTO @doc67_rows FROM workflow_import_related_document';
    PREPARE doc67_statement FROM @doc67_sql; EXECUTE doc67_statement; DEALLOCATE PREPARE doc67_statement;
    IF @doc67_rows > 0 THEN
      SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC-67 rollback detenido: existen documentos relacionados; exporte o conserve los datos';
    END IF;
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_inscription') THEN
    SET @doc67_rows = 0;
    SET @doc67_sql = 'SELECT COUNT(*) INTO @doc67_rows FROM workflow_import_inscription';
    PREPARE doc67_statement FROM @doc67_sql; EXECUTE doc67_statement; DEALLOCATE PREPARE doc67_statement;
    IF @doc67_rows > 0 THEN
      SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC-67 rollback detenido: existen inscripciones; exporte o conserve los datos';
    END IF;
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'inscription_key') THEN
    SET @doc67_rows = 0;
    SET @doc67_sql = 'SELECT COUNT(*) INTO @doc67_rows FROM workflow_import_intent_item WHERE inscription_key IS NOT NULL OR expedient_id IS NOT NULL OR storage_status <> ''Pendiente'' OR relation_status <> ''Pendiente'' OR index_status <> ''Pendiente'' OR cache_status <> ''Pendiente''';
    PREPARE doc67_statement FROM @doc67_sql; EXECUTE doc67_statement; DEALLOCATE PREPARE doc67_statement;
    IF @doc67_rows > 0 THEN
      SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC-67 rollback detenido: los items contienen estado DOC-67';
    END IF;
  END IF;
END$$
DELIMITER ;
CALL doc67_assert_empty();
DROP PROCEDURE IF EXISTS doc67_assert_empty;

DROP TABLE IF EXISTS workflow_import_related_document;

DROP PROCEDURE IF EXISTS doc67_drop_item_extensions;
DELIMITER $$
CREATE PROCEDURE doc67_drop_item_extensions()
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND CONSTRAINT_NAME = 'fk_import_item_inscription') THEN
    ALTER TABLE workflow_import_intent_item DROP FOREIGN KEY fk_import_item_inscription;
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND INDEX_NAME = 'ix_import_item_inscription') THEN
    ALTER TABLE workflow_import_intent_item DROP INDEX ix_import_item_inscription;
  END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'cache_status') THEN ALTER TABLE workflow_import_intent_item DROP COLUMN cache_status; END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'index_status') THEN ALTER TABLE workflow_import_intent_item DROP COLUMN index_status; END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'relation_status') THEN ALTER TABLE workflow_import_intent_item DROP COLUMN relation_status; END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'storage_status') THEN ALTER TABLE workflow_import_intent_item DROP COLUMN storage_status; END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'expedient_id') THEN ALTER TABLE workflow_import_intent_item DROP COLUMN expedient_id; END IF;
  IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'inscription_key') THEN ALTER TABLE workflow_import_intent_item DROP COLUMN inscription_key; END IF;
END$$
DELIMITER ;
CALL doc67_drop_item_extensions();
DROP PROCEDURE IF EXISTS doc67_drop_item_extensions;

DROP TABLE IF EXISTS workflow_import_inscription;


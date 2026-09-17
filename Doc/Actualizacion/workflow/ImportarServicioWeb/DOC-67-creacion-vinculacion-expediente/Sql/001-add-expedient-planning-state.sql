DROP PROCEDURE IF EXISTS doc67_add_column;
DELIMITER $$
CREATE PROCEDURE doc67_add_column(IN p_table VARCHAR(64), IN p_column VARCHAR(64), IN p_definition TEXT)
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = p_table AND COLUMN_NAME = p_column
  ) THEN
    SET @doc67_sql = CONCAT('ALTER TABLE `', p_table, '` ADD COLUMN ', p_definition);
    PREPARE doc67_statement FROM @doc67_sql;
    EXECUTE doc67_statement;
    DEALLOCATE PREPARE doc67_statement;
  END IF;
END$$
DELIMITER ;

CREATE TABLE IF NOT EXISTS workflow_import_inscription (
  intent_id VARCHAR(32) NOT NULL,
  inscription_key VARCHAR(128) NOT NULL,
  inscription_ordinal INT NOT NULL,
  book_code VARCHAR(80) NULL,
  registry_number VARCHAR(128) NULL,
  matricula VARCHAR(128) NULL,
  normalized_matricula VARCHAR(128) NULL,
  proponente VARCHAR(128) NULL,
  subject_identification VARCHAR(128) NULL,
  subject_name VARCHAR(500) NULL,
  owner_matricula VARCHAR(128) NULL,
  owner_identification VARCHAR(128) NULL,
  owner_name VARCHAR(500) NULL,
  cabinet_name VARCHAR(128) NOT NULL,
  expedient_id BIGINT NULL,
  expedient_role VARCHAR(24) NOT NULL DEFAULT 'Pendiente',
  expedient_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  cache_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  created_utc DATETIME NOT NULL,
  updated_utc DATETIME NOT NULL,
  PRIMARY KEY (intent_id, inscription_key),
  UNIQUE KEY uq_import_inscription_ordinal (intent_id, inscription_ordinal),
  KEY ix_import_inscription_expedient (expedient_id),
  CONSTRAINT fk_import_inscription_intent FOREIGN KEY (intent_id)
    REFERENCES workflow_import_intent(intent_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CALL doc67_add_column('workflow_import_intent_item', 'inscription_key',
  '`inscription_key` VARCHAR(128) NULL AFTER `client_item_id`');
CALL doc67_add_column('workflow_import_intent_item', 'expedient_id',
  '`expedient_id` BIGINT NULL AFTER `document_id`');
CALL doc67_add_column('workflow_import_intent_item', 'storage_status',
  '`storage_status` VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER `expedient_id`');
CALL doc67_add_column('workflow_import_intent_item', 'relation_status',
  '`relation_status` VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER `storage_status`');
CALL doc67_add_column('workflow_import_intent_item', 'index_status',
  '`index_status` VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER `relation_status`');
CALL doc67_add_column('workflow_import_intent_item', 'cache_status',
  '`cache_status` VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER `index_status`');

DROP PROCEDURE IF EXISTS doc67_add_column;

DROP PROCEDURE IF EXISTS doc67_add_item_relation;
DELIMITER $$
CREATE PROCEDURE doc67_add_item_relation()
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item'
      AND INDEX_NAME = 'ix_import_item_inscription'
  ) THEN
    ALTER TABLE workflow_import_intent_item
      ADD KEY ix_import_item_inscription (intent_id, inscription_key);
  END IF;
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item'
      AND CONSTRAINT_NAME = 'fk_import_item_inscription'
  ) THEN
    ALTER TABLE workflow_import_intent_item
      ADD CONSTRAINT fk_import_item_inscription
      FOREIGN KEY (intent_id, inscription_key)
      REFERENCES workflow_import_inscription(intent_id, inscription_key);
  END IF;
END$$
DELIMITER ;
CALL doc67_add_item_relation();
DROP PROCEDURE IF EXISTS doc67_add_item_relation;

CREATE TABLE IF NOT EXISTS workflow_import_related_document (
  related_document_id BIGINT NOT NULL AUTO_INCREMENT,
  intent_id VARCHAR(32) NOT NULL,
  task_id BIGINT NOT NULL,
  image_id BIGINT NOT NULL,
  cabinet_name VARCHAR(128) NOT NULL,
  sii_radicado VARCHAR(255) NOT NULL,
  document_type_id INT NULL,
  inscription_key VARCHAR(128) NULL,
  expected_expedient_id BIGINT NULL,
  discovery_status VARCHAR(40) NOT NULL DEFAULT 'Descubierto',
  destination_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  relation_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  cache_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  cabinet_index_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  electronic_index_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  xml_index_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  reconciliation_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  retryable BIT NOT NULL DEFAULT 0,
  error_code VARCHAR(80) NULL,
  created_utc DATETIME NOT NULL,
  updated_utc DATETIME NOT NULL,
  PRIMARY KEY (related_document_id),
  UNIQUE KEY uq_import_related_document (intent_id, task_id, image_id, cabinet_name),
  KEY ix_import_related_document_expedient (intent_id, expected_expedient_id),
  KEY ix_import_related_document_inscription (intent_id, inscription_key),
  CONSTRAINT fk_import_related_document_intent FOREIGN KEY (intent_id)
    REFERENCES workflow_import_intent(intent_id) ON DELETE CASCADE,
  CONSTRAINT fk_import_related_document_inscription FOREIGN KEY (intent_id, inscription_key)
    REFERENCES workflow_import_inscription(intent_id, inscription_key)
) ENGINE=InnoDB;


DROP PROCEDURE IF EXISTS doc81_add_intent_context;
DELIMITER $$
CREATE PROCEDURE doc81_add_intent_context()
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'workflow_import_intent'
      AND COLUMN_NAME = 'capability'
  ) THEN
    ALTER TABLE workflow_import_intent
      ADD COLUMN capability VARCHAR(80) NOT NULL DEFAULT '' AFTER provider_id;
  END IF;

  IF NOT EXISTS (
    SELECT 1 FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'workflow_import_intent'
      AND COLUMN_NAME = 'provider_reference'
  ) THEN
    ALTER TABLE workflow_import_intent
      ADD COLUMN provider_reference VARCHAR(80) NOT NULL DEFAULT '' AFTER radicado;
  END IF;
END$$
DELIMITER ;
CALL doc81_add_intent_context();
DROP PROCEDURE IF EXISTS doc81_add_intent_context;
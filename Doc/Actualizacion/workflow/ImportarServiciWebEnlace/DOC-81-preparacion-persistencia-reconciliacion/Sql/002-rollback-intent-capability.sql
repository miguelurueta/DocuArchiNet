DROP PROCEDURE IF EXISTS doc81_drop_intent_context;
DELIMITER $$
CREATE PROCEDURE doc81_drop_intent_context()
BEGIN
  IF EXISTS (
    SELECT 1 FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'workflow_import_intent'
      AND COLUMN_NAME = 'provider_reference'
  ) THEN
    ALTER TABLE workflow_import_intent DROP COLUMN provider_reference;
  END IF;

  IF EXISTS (
    SELECT 1 FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'workflow_import_intent'
      AND COLUMN_NAME = 'capability'
  ) THEN
    ALTER TABLE workflow_import_intent DROP COLUMN capability;
  END IF;
END$$
DELIMITER ;
CALL doc81_drop_intent_context();
DROP PROCEDURE IF EXISTS doc81_drop_intent_context;
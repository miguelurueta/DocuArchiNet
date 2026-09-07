DROP TABLE IF EXISTS workflow_import_intent_transition;

ALTER TABLE workflow_import_intent_item
  DROP COLUMN correlation_id,
  DROP COLUMN visible_message,
  DROP COLUMN error_code,
  DROP COLUMN retryable,
  DROP COLUMN persistence_known,
  DROP COLUMN document_id;

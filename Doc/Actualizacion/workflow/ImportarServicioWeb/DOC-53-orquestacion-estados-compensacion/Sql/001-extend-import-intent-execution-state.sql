ALTER TABLE workflow_import_intent_item
  ADD COLUMN document_id BIGINT NULL AFTER status,
  ADD COLUMN persistence_known BIT NOT NULL DEFAULT 1 AFTER document_id,
  ADD COLUMN retryable BIT NOT NULL DEFAULT 0 AFTER persistence_known,
  ADD COLUMN error_code VARCHAR(80) NULL AFTER retryable,
  ADD COLUMN visible_message VARCHAR(500) NULL AFTER error_code,
  ADD COLUMN correlation_id VARCHAR(128) NULL AFTER visible_message;

CREATE TABLE workflow_import_intent_transition (
  transition_id BIGINT NOT NULL AUTO_INCREMENT,
  intent_id VARCHAR(32) NOT NULL,
  client_item_id VARCHAR(128) NOT NULL,
  previous_status VARCHAR(40) NOT NULL,
  new_status VARCHAR(40) NOT NULL,
  previous_version CHAR(64) NOT NULL,
  new_version CHAR(64) NOT NULL,
  occurred_utc DATETIME NOT NULL,
  correlation_id VARCHAR(128) NULL,
  result_code VARCHAR(80) NULL,
  PRIMARY KEY (transition_id),
  KEY ix_import_transition_intent (intent_id, transition_id),
  CONSTRAINT fk_import_transition_intent FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id) ON DELETE CASCADE
) ENGINE=InnoDB;

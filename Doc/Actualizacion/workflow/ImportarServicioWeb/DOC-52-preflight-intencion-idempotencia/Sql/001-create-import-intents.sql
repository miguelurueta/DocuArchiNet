CREATE TABLE workflow_import_intent (
  intent_id VARCHAR(32) NOT NULL,
  idempotency_key VARCHAR(128) NOT NULL,
  payload_hash CHAR(64) NOT NULL,
  operation_id VARCHAR(128) NOT NULL,
  correlation_id VARCHAR(128) NOT NULL,
  user_id INT NOT NULL, group_id INT NOT NULL, user_login VARCHAR(128) NOT NULL,
  task_id BIGINT NOT NULL, route_id INT NOT NULL, procedure_id INT NOT NULL,
  provider_id VARCHAR(128) NOT NULL, status VARCHAR(40) NOT NULL,
  version_token CHAR(64) NOT NULL, created_utc DATETIME NOT NULL, updated_utc DATETIME NOT NULL,
  PRIMARY KEY (intent_id),
  UNIQUE KEY uq_workflow_import_intent_idempotency (user_id, task_id, idempotency_key),
  KEY ix_workflow_import_intent_operation (operation_id),
  KEY ix_workflow_import_intent_correlation (correlation_id)
) ENGINE=InnoDB;

CREATE TABLE workflow_import_intent_requirement (
  intent_id VARCHAR(32) NOT NULL, requirement_code VARCHAR(80) NOT NULL,
  is_satisfied BIT NOT NULL, visible_message VARCHAR(500) NULL,
  PRIMARY KEY (intent_id, requirement_code),
  CONSTRAINT fk_import_requirement_intent FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE workflow_import_intent_item (
  intent_id VARCHAR(32) NOT NULL, client_item_id VARCHAR(128) NOT NULL,
  provider_id VARCHAR(128) NOT NULL, external_key VARCHAR(500) NOT NULL,
  target_task_id BIGINT NOT NULL, document_type_id INT NULL,
  file_name VARCHAR(500) NULL, content_type VARCHAR(255) NULL, status VARCHAR(40) NOT NULL,
  PRIMARY KEY (intent_id, client_item_id),
  UNIQUE KEY uq_import_intent_external (intent_id, provider_id, external_key, target_task_id),
  CONSTRAINT fk_import_item_intent FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id) ON DELETE CASCADE
) ENGINE=InnoDB;

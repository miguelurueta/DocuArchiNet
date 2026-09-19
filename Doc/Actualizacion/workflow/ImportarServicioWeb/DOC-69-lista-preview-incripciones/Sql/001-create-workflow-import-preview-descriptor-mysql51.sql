-- DOC-69. Ejecutar exclusivamente en la base Workflow (por defecto workflowdocument).
-- Compatible con MySQL 5.1. No crea procedimientos ni llaves foráneas entre bases.
CREATE TABLE workflow_import_preview_descriptor (
  id BIGINT NOT NULL AUTO_INCREMENT,
  descriptor_hash BINARY(32) NOT NULL,
  resource_hash BINARY(32) NOT NULL,
  user_id INT NOT NULL,
  task_id BIGINT NOT NULL,
  provider_id VARCHAR(40) NOT NULL,
  content_type VARCHAR(100) NOT NULL,
  content_length BIGINT NOT NULL,
  content_disposition VARCHAR(20) NOT NULL,
  safe_file_name VARCHAR(255) NOT NULL,
  content MEDIUMBLOB NOT NULL,
  status VARCHAR(20) NOT NULL,
  expires_utc DATETIME NOT NULL,
  claimed_utc DATETIME NULL,
  consumed_utc DATETIME NULL,
  created_utc DATETIME NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY ux_import_preview_descriptor_hash (descriptor_hash),
  KEY ix_import_preview_authority (user_id, task_id, provider_id, status, expires_utc),
  KEY ix_import_preview_expiry (status, expires_utc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

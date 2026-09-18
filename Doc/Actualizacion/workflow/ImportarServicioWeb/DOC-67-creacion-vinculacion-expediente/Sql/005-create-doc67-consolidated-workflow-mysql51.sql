-- DOC-67 - Estructura consolidada de persistencia moderna.
-- Motor objetivo: MySQL 5.1.
-- Base de datos obligatoria: workflowdocument (módulo WORKFLOW REGISTRO).
-- No crea procedures ni objetos en DocuArchi/Radicación.
-- Ejecutar únicamente para una instalación limpia de estas tablas.

USE `workflowdocument`;

CREATE TABLE IF NOT EXISTS workflow_import_intent (
  intent_id VARCHAR(32) NOT NULL,
  idempotency_key VARCHAR(128) NOT NULL,
  payload_hash CHAR(64) NOT NULL,
  operation_id VARCHAR(128) NOT NULL,
  correlation_id VARCHAR(128) NOT NULL,
  user_id INT NOT NULL,
  group_id INT NOT NULL,
  user_login VARCHAR(128) NOT NULL,
  task_id BIGINT NOT NULL,
  route_id INT NOT NULL,
  procedure_id INT NOT NULL,
  provider_id VARCHAR(128) NOT NULL,
  radicado VARCHAR(255) NOT NULL,
  status VARCHAR(40) NOT NULL,
  version_token CHAR(64) NOT NULL,
  created_utc DATETIME NOT NULL,
  updated_utc DATETIME NOT NULL,
  PRIMARY KEY (intent_id),
  UNIQUE KEY uq_workflow_import_intent_idempotency (user_id, task_id, idempotency_key),
  KEY ix_workflow_import_intent_task (task_id, created_utc),
  KEY ix_workflow_import_intent_operation (operation_id),
  KEY ix_workflow_import_intent_correlation (correlation_id)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS workflow_import_intent_requirement (
  intent_id VARCHAR(32) NOT NULL,
  requirement_code VARCHAR(80) NOT NULL,
  is_satisfied BIT NOT NULL,
  visible_message VARCHAR(500) NULL,
  PRIMARY KEY (intent_id, requirement_code),
  CONSTRAINT fk_import_requirement_intent
    FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

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
  CONSTRAINT fk_import_inscription_intent
    FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS workflow_import_intent_item (
  intent_id VARCHAR(32) NOT NULL,
  client_item_id VARCHAR(128) NOT NULL,
  inscription_key VARCHAR(128) NULL,
  provider_id VARCHAR(128) NOT NULL,
  external_key VARCHAR(500) NOT NULL,
  target_task_id BIGINT NOT NULL,
  document_type_id INT NULL,
  document_type_name VARCHAR(255) NULL,
  file_name VARCHAR(500) NULL,
  content_type VARCHAR(255) NULL,
  status VARCHAR(40) NOT NULL,
  document_id BIGINT NULL,
  expedient_id BIGINT NULL,
  storage_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  relation_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  index_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  cache_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente',
  persistence_known BIT NOT NULL DEFAULT 1,
  retryable BIT NOT NULL DEFAULT 0,
  error_code VARCHAR(80) NULL,
  visible_message VARCHAR(500) NULL,
  correlation_id VARCHAR(128) NULL,
  PRIMARY KEY (intent_id, client_item_id),
  UNIQUE KEY uq_import_intent_external (intent_id, provider_id, external_key, target_task_id),
  KEY ix_import_item_inscription (intent_id, inscription_key),
  KEY ix_import_item_document (document_id),
  CONSTRAINT fk_import_item_intent
    FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id)
    ON DELETE CASCADE,
  CONSTRAINT fk_import_item_inscription
    FOREIGN KEY (intent_id, inscription_key)
    REFERENCES workflow_import_inscription(intent_id, inscription_key)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS workflow_import_intent_transition (
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
  CONSTRAINT fk_import_transition_intent
    FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

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
  KEY ix_import_related_document_task (task_id, image_id, cabinet_name),
  KEY ix_import_related_document_expedient (intent_id, expected_expedient_id),
  KEY ix_import_related_document_inscription (intent_id, inscription_key),
  CONSTRAINT fk_import_related_document_intent
    FOREIGN KEY (intent_id) REFERENCES workflow_import_intent(intent_id)
    ON DELETE CASCADE,
  CONSTRAINT fk_import_related_document_inscription
    FOREIGN KEY (intent_id, inscription_key)
    REFERENCES workflow_import_inscription(intent_id, inscription_key)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS workflow_import_document_link_cache (
  document_link_cache_id BIGINT NOT NULL AUTO_INCREMENT,
  task_id BIGINT NOT NULL,
  image_id BIGINT NOT NULL,
  cabinet_name VARCHAR(128) NOT NULL,
  expected_expedient_id BIGINT NOT NULL,
  sii_radicado VARCHAR(255) NOT NULL,
  relation_status VARCHAR(40) NOT NULL,
  created_utc DATETIME NOT NULL,
  verified_utc DATETIME NULL,
  PRIMARY KEY (document_link_cache_id),
  UNIQUE KEY uq_import_document_link_cache_identity (task_id, image_id, cabinet_name),
  KEY ix_import_document_link_cache_expedient (expected_expedient_id),
  KEY ix_import_document_link_cache_radicado (sii_radicado)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Compatibilidad con instalaciones antiguas.
-- MySQL 5.1 no soporta ADD COLUMN IF NOT EXISTS. Cada sentencia consulta
-- information_schema y ejecuta el ALTER solamente cuando el objeto falta.
-- PREPARE/EXECUTE no crea procedures ni deja objetos persistentes.

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent' AND COLUMN_NAME = 'radicado'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent ADD COLUMN radicado VARCHAR(255) NOT NULL AFTER provider_id'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'document_type_name'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN document_type_name VARCHAR(255) NULL AFTER document_type_id'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'document_id'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN document_id BIGINT NULL AFTER status'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'persistence_known'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN persistence_known BIT NOT NULL DEFAULT 1 AFTER document_id'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'retryable'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN retryable BIT NOT NULL DEFAULT 0 AFTER persistence_known'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'error_code'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN error_code VARCHAR(80) NULL AFTER retryable'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'visible_message'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN visible_message VARCHAR(500) NULL AFTER error_code'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'correlation_id'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN correlation_id VARCHAR(128) NULL AFTER visible_message'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'inscription_key'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN inscription_key VARCHAR(128) NULL AFTER client_item_id'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'expedient_id'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN expedient_id BIGINT NULL AFTER document_id'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'storage_status'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN storage_status VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER expedient_id'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'relation_status'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN relation_status VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER storage_status'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'index_status'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN index_status VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER relation_status'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND COLUMN_NAME = 'cache_status'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD COLUMN cache_status VARCHAR(40) NOT NULL DEFAULT ''Pendiente'' AFTER index_status'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent' AND INDEX_NAME = 'ix_workflow_import_intent_task'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent ADD KEY ix_workflow_import_intent_task (task_id, created_utc)'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND INDEX_NAME = 'ix_import_item_inscription'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD KEY ix_import_item_inscription (intent_id, inscription_key)'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND INDEX_NAME = 'ix_import_item_document'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD KEY ix_import_item_document (document_id)'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = IF(
  EXISTS (SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'workflow_import_intent_item' AND CONSTRAINT_NAME = 'fk_import_item_inscription' AND CONSTRAINT_TYPE = 'FOREIGN KEY'),
  'SELECT 1',
  'ALTER TABLE workflow_import_intent_item ADD CONSTRAINT fk_import_item_inscription FOREIGN KEY (intent_id, inscription_key) REFERENCES workflow_import_inscription(intent_id, inscription_key)'
);
PREPARE doc67_stmt FROM @doc67_sql; EXECUTE doc67_stmt; DEALLOCATE PREPARE doc67_stmt;

SET @doc67_sql = NULL;

-- Tablas físicas reutilizadas, NO creadas por este script:
-- DocuArchi: expediente_archivo, ra_sii_cache_exepediente,
-- ra_relacion_radicado_externo_expediente, registro_producion_documental,
-- logdocuarchi, DETALLE_GABIENETE y tablas dinámicas de gabinete.
-- Radicación/configuración: tipo_doc_entrante, ra_auto_campo_unico_expediente,
-- ra_dig_tipos_docum_lista_chequeo, tipo_doc_series y configuracion_gabinete.

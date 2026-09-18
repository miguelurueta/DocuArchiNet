-- DOC-67 - Creacion manual de la estructura faltante para MySQL 5.1.
-- Alternativa a 001-add-expedient-planning-state.sql y
-- 003-create-document-link-cache.sql. No ejecutar ambos caminos.
-- Precondicion comprobada: los objetos declarados en este archivo no existen.

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

ALTER TABLE workflow_import_intent_item
  ADD COLUMN inscription_key VARCHAR(128) NULL AFTER client_item_id,
  ADD COLUMN expedient_id BIGINT NULL AFTER document_id,
  ADD COLUMN storage_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente' AFTER expedient_id,
  ADD COLUMN relation_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente' AFTER storage_status,
  ADD COLUMN index_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente' AFTER relation_status,
  ADD COLUMN cache_status VARCHAR(40) NOT NULL DEFAULT 'Pendiente' AFTER index_status,
  ADD KEY ix_import_item_inscription (intent_id, inscription_key),
  ADD CONSTRAINT fk_import_item_inscription
    FOREIGN KEY (intent_id, inscription_key)
    REFERENCES workflow_import_inscription(intent_id, inscription_key);

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
) ENGINE=InnoDB;

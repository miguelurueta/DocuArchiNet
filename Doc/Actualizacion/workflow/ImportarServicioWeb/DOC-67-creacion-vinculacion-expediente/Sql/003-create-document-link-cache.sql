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


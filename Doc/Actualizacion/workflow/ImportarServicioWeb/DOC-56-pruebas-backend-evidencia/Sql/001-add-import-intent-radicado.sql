ALTER TABLE workflow_import_intent
  ADD COLUMN radicado VARCHAR(255) NOT NULL AFTER provider_id;

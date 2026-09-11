ALTER TABLE workflow_import_intent_item
  ADD COLUMN document_type_name VARCHAR(255) NULL AFTER document_type_id;

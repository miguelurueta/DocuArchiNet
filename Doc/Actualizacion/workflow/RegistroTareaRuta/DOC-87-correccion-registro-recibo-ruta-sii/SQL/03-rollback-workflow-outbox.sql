-- DOC-87 / ejecutar después de retirar la versión aplicativa.
-- Falla de forma deliberada si existen eventos para impedir pérdida de trabajo.
DELIMITER $$
CREATE PROCEDURE doc87_rollback_outbox()
BEGIN
    IF EXISTS (SELECT 1 FROM workflow_registro_ruta_sii_outbox LIMIT 1) THEN
        SELECT 'DOC87_ROLLBACK_BLOCKED_OUTBOX_HAS_EVENTS' AS rollback_status;
    ELSE
        DROP TABLE workflow_registro_ruta_sii_outbox;
        SELECT 'DOC87_ROLLBACK_COMPLETED' AS rollback_status;
    END IF;
END$$
DELIMITER ;
CALL doc87_rollback_outbox();
DROP PROCEDURE doc87_rollback_outbox;

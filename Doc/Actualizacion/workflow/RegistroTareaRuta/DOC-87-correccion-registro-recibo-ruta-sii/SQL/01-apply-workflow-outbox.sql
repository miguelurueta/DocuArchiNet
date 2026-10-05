-- DOC-87 / base Workflow / migración idempotente.
CREATE TABLE IF NOT EXISTS workflow_registro_ruta_sii_outbox (
    id BIGINT NOT NULL AUTO_INCREMENT,
    operation_id CHAR(36) NOT NULL,
    route_id INT NOT NULL,
    route_name VARCHAR(64) NOT NULL,
    task_id BIGINT NOT NULL,
    receipt VARCHAR(10) NOT NULL,
    enrollment VARCHAR(40) NOT NULL,
    cabinet_name VARCHAR(120) NOT NULL,
    status VARCHAR(24) NOT NULL DEFAULT 'PENDING',
    attempts INT NOT NULL DEFAULT 0,
    last_error_code VARCHAR(64) NULL,
    created_at DATETIME NOT NULL,
    updated_at DATETIME NOT NULL,
    next_attempt_at DATETIME NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_workflow_route_sii_operation (operation_id),
    UNIQUE KEY uq_workflow_route_sii_receipt (route_id, receipt),
    KEY ix_workflow_route_sii_pending (status, next_attempt_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

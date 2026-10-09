-- DOC-92 ROLLBACK DESTRUCTIVO PARA ATRIBUTOS V1.
-- Requiere respaldo y confirmación de que ninguna fila SchemaVersion=1 deba conservarse.
DELIMITER $$
DROP PROCEDURE IF EXISTS doc92_rollback$$
CREATE PROCEDURE doc92_rollback()
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND INDEX_NAME='IX_ra_auth_sfc_identity_purpose_state') THEN
        DROP INDEX IX_ra_auth_sfc_identity_purpose_state ON ra_auth_second_factor_challenge;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND INDEX_NAME='IX_ra_auth_sfc_session_state') THEN
        DROP INDEX IX_ra_auth_sfc_session_state ON ra_auth_second_factor_challenge;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND INDEX_NAME='IX_ra_auth_sfc_state_expiry') THEN
        DROP INDEX IX_ra_auth_sfc_state_expiry ON ra_auth_second_factor_challenge;
    END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='SchemaVersion') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN SchemaVersion; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='UpdatedAtUtc') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN UpdatedAtUtc; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='TerminalAtUtc') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN TerminalAtUtc; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='LastSentAtUtc') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN LastSentAtUtc; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='ResendCount') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN ResendCount; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='KeyId') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN KeyId; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='State') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN State; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='SessionBindingHash') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN SessionBindingHash; END IF;
    IF EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='Purpose') THEN ALTER TABLE ra_auth_second_factor_challenge DROP COLUMN Purpose; END IF;
END$$
CALL doc92_rollback()$$
DROP PROCEDURE doc92_rollback$$
DELIMITER ;

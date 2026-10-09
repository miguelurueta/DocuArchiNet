-- DOC-92. Extensión aditiva e idempotente. No hace backfill de filas legacy.
DELIMITER $$
DROP PROCEDURE IF EXISTS doc92_apply$$
CREATE PROCEDURE doc92_apply()
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='Purpose') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN Purpose VARCHAR(20) NULL AFTER Provider;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='SessionBindingHash') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN SessionBindingHash VARCHAR(200) NULL AFTER Purpose;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='State') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN State VARCHAR(30) NULL AFTER SessionBindingHash;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='KeyId') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN KeyId VARCHAR(100) NULL AFTER State;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='ResendCount') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN ResendCount INT UNSIGNED NULL AFTER Attempts;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='LastSentAtUtc') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN LastSentAtUtc DATETIME NULL AFTER ResendCount;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='TerminalAtUtc') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN TerminalAtUtc DATETIME NULL AFTER LastSentAtUtc;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='UpdatedAtUtc') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN UpdatedAtUtc DATETIME NULL AFTER TerminalAtUtc;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND COLUMN_NAME='SchemaVersion') THEN
        ALTER TABLE ra_auth_second_factor_challenge ADD COLUMN SchemaVersion SMALLINT UNSIGNED NULL AFTER UpdatedAtUtc;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND INDEX_NAME='IX_ra_auth_sfc_identity_purpose_state') THEN
        CREATE INDEX IX_ra_auth_sfc_identity_purpose_state ON ra_auth_second_factor_challenge (AuthUserId, Purpose, State);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND INDEX_NAME='IX_ra_auth_sfc_session_state') THEN
        CREATE INDEX IX_ra_auth_sfc_session_state ON ra_auth_second_factor_challenge (SessionBindingHash, State);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge' AND INDEX_NAME='IX_ra_auth_sfc_state_expiry') THEN
        CREATE INDEX IX_ra_auth_sfc_state_expiry ON ra_auth_second_factor_challenge (State, ExpiresAtUtc);
    END IF;
END$$
CALL doc92_apply()$$
DROP PROCEDURE doc92_apply$$
DELIMITER ;

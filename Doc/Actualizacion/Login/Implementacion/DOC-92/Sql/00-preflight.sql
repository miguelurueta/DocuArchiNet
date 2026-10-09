-- DOC-92. Solo inspección del contrato previo; no modifica datos de negocio.
DELIMITER $$
DROP PROCEDURE IF EXISTS doc92_preflight$$
CREATE PROCEDURE doc92_preflight()
BEGIN
    DECLARE table_count INT DEFAULT 0;
    DECLARE base_columns INT DEFAULT 0;
    DECLARE engine_name VARCHAR(64);
    DECLARE challenge_unique INT DEFAULT 0;
    DECLARE auth_user_index INT DEFAULT 0;

    SELECT COUNT(*), MAX(ENGINE)
      INTO table_count, engine_name
      FROM information_schema.TABLES
     WHERE TABLE_SCHEMA = DATABASE()
       AND TABLE_NAME = 'ra_auth_second_factor_challenge';

    IF table_count <> 1 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC92_PREFLIGHT_TABLE_MISSING';
    END IF;
    IF UPPER(COALESCE(engine_name, '')) <> 'INNODB' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC92_PREFLIGHT_INNODB_REQUIRED';
    END IF;

    SELECT COUNT(DISTINCT COLUMN_NAME)
      INTO base_columns
      FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA = DATABASE()
       AND TABLE_NAME = 'ra_auth_second_factor_challenge'
       AND COLUMN_NAME IN ('Id','ChallengeId','AuthUserId','Provider','CodeHash','ExpiresAtUtc','Consumed','Attempts','CreatedAtUtc','AuthPayloadJson');
    IF base_columns <> 10 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC92_PREFLIGHT_BASE_COLUMNS_MISMATCH';
    END IF;

    SELECT COUNT(DISTINCT INDEX_NAME) INTO challenge_unique
      FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ra_auth_second_factor_challenge'
       AND INDEX_NAME = 'uq_challengeid' AND NON_UNIQUE = 0 AND COLUMN_NAME = 'ChallengeId';
    SELECT COUNT(DISTINCT INDEX_NAME) INTO auth_user_index
      FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ra_auth_second_factor_challenge'
       AND INDEX_NAME = 'IX_ra_auth_sfc_authuserid' AND COLUMN_NAME = 'AuthUserId';
    IF challenge_unique <> 1 OR auth_user_index <> 1 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'DOC92_PREFLIGHT_LEGACY_INDEX_MISMATCH';
    END IF;

    SELECT 'DOC92_PREFLIGHT_OK' AS Result, DATABASE() AS SchemaName, engine_name AS EngineName;
END$$
CALL doc92_preflight()$$
DROP PROCEDURE doc92_preflight$$
DELIMITER ;

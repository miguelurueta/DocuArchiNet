-- DOC-92. Verifica el contrato resultante y el orden de índices.
DELIMITER $$
DROP PROCEDURE IF EXISTS doc92_postflight$$
CREATE PROCEDURE doc92_postflight()
BEGIN
    DECLARE new_columns INT DEFAULT 0;
    DECLARE required_indexes INT DEFAULT 0;
    DECLARE preserved_indexes INT DEFAULT 0;

    SELECT COUNT(DISTINCT COLUMN_NAME) INTO new_columns
      FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
       AND COLUMN_NAME IN ('Purpose','SessionBindingHash','State','KeyId','ResendCount','LastSentAtUtc','TerminalAtUtc','UpdatedAtUtc','SchemaVersion');
    IF new_columns <> 9 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='DOC92_POSTFLIGHT_COLUMNS_MISMATCH';
    END IF;

    SELECT COUNT(*) INTO required_indexes FROM (
        SELECT INDEX_NAME, GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX SEPARATOR ',') AS ColumnsInOrder
          FROM information_schema.STATISTICS
         WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
           AND INDEX_NAME IN ('IX_ra_auth_sfc_identity_purpose_state','IX_ra_auth_sfc_session_state','IX_ra_auth_sfc_state_expiry')
         GROUP BY INDEX_NAME
        HAVING (INDEX_NAME='IX_ra_auth_sfc_identity_purpose_state' AND ColumnsInOrder='AuthUserId,Purpose,State')
            OR (INDEX_NAME='IX_ra_auth_sfc_session_state' AND ColumnsInOrder='SessionBindingHash,State')
            OR (INDEX_NAME='IX_ra_auth_sfc_state_expiry' AND ColumnsInOrder='State,ExpiresAtUtc')
    ) expected;
    IF required_indexes <> 3 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='DOC92_POSTFLIGHT_INDEX_MISMATCH';
    END IF;

    SELECT COUNT(DISTINCT INDEX_NAME) INTO preserved_indexes
      FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
       AND INDEX_NAME IN ('uq_challengeid','IX_ra_auth_sfc_authuserid');
    IF preserved_indexes <> 2 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='DOC92_POSTFLIGHT_LEGACY_INDEX_REMOVED';
    END IF;
    SELECT 'DOC92_POSTFLIGHT_OK' AS Result;
END$$
CALL doc92_postflight()$$
DROP PROCEDURE doc92_postflight$$
DELIMITER ;

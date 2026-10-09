-- EJECUCIÓN MANUAL. Valor opcional: SET @doc92_retention_days = 30;
SET @doc92_retention_days = COALESCE(@doc92_retention_days, 30);
DELETE FROM ra_auth_second_factor_challenge
 WHERE SchemaVersion = 1
   AND State IN ('COMPLETED','DELIVERY_FAILED','BLOCKED','EXPIRED','REVOKED','FINALIZATION_FAILED')
   AND TerminalAtUtc IS NOT NULL
   AND TerminalAtUtc < UTC_TIMESTAMP() - INTERVAL @doc92_retention_days DAY;

-- DOC-98. Vista previa únicamente; no elimina registros.

SELECT
    'terminal_older_than_30_days' AS check_name,
    COUNT(*) AS candidate_count,
    MIN(CreatedAtUtc) AS oldest_created_at_utc,
    MAX(CreatedAtUtc) AS newest_created_at_utc
FROM ra_auth_second_factor_challenge
WHERE CreatedAtUtc < UTC_TIMESTAMP() - INTERVAL 30 DAY
  AND (Consumed = 1 OR Attempts >= 5 OR ExpiresAtUtc <= UTC_TIMESTAMP())
  AND NOT (Consumed = 0 AND Attempts < 5 AND ExpiresAtUtc > UTC_TIMESTAMP());

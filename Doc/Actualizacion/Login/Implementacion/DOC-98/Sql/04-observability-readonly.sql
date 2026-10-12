-- DOC-98. Métricas agregadas; no proyecta identidad, correo, login, hash ni payload.

SELECT
    'challenge_state' AS metric,
    SUM(CASE WHEN Consumed = 0 AND Attempts < 5 AND ExpiresAtUtc > UTC_TIMESTAMP() THEN 1 ELSE 0 END) AS eligible_count,
    SUM(CASE WHEN Consumed = 1 THEN 1 ELSE 0 END) AS consumed_count,
    SUM(CASE WHEN Consumed = 0 AND Attempts >= 5 THEN 1 ELSE 0 END) AS blocked_count,
    SUM(CASE WHEN Consumed = 0 AND ExpiresAtUtc <= UTC_TIMESTAMP() THEN 1 ELSE 0 END) AS expired_count,
    COUNT(*) AS total_count
FROM ra_auth_second_factor_challenge
WHERE CreatedAtUtc >= UTC_TIMESTAMP() - INTERVAL 60 MINUTE;

SELECT
    'challenge_attempts' AS metric,
    Attempts AS attempt_count,
    COUNT(*) AS challenge_count
FROM ra_auth_second_factor_challenge
WHERE CreatedAtUtc >= UTC_TIMESTAMP() - INTERVAL 60 MINUTE
GROUP BY Attempts
ORDER BY Attempts;

SELECT
    'module_configuration' AS metric,
    TIPO_MODULO AS module_type,
    RequiereSegundoFactor AS required_flag,
    SecondFactorProviderType AS provider_type,
    SegundoFactorTiempoExpira AS expiry_minutes,
    COUNT(*) AS module_count
FROM gestor_modulos
WHERE ESTADO_MODULO = 1
  AND TIPO_MODULO IN ('DOCUARCHI CONTENEDOR','GESTOR DOCUMENTAL','RADICACION DOCUMENTAL','WORKFLOW DOCUMENTAL')
GROUP BY TIPO_MODULO, RequiereSegundoFactor, SecondFactorProviderType, SegundoFactorTiempoExpira
ORDER BY TIPO_MODULO, RequiereSegundoFactor;

SELECT
    'active_smtp' AS metric,
    COUNT(*) AS active_configuration_count
FROM Config_Smpt_Side
WHERE ESTADO_ENVIO = 1;

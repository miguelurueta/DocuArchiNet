-- DOC-98. Ejecutar con una cuenta de solo lectura en cada esquema correspondiente.
-- Este archivo contiene exclusivamente SELECT; no corrige estructura ni configuración.

SELECT
    'challenge_required_columns' AS check_name,
    COUNT(*) AS actual_count,
    10 AS expected_count,
    CASE WHEN COUNT(*) = 10 THEN 'PASS' ELSE 'BLOCKED' END AS result
FROM information_schema.columns
WHERE table_schema = DATABASE()
  AND table_name = 'ra_auth_second_factor_challenge'
  AND column_name IN ('Id','ChallengeId','AuthUserId','Provider','CodeHash','ExpiresAtUtc','Consumed','Attempts','CreatedAtUtc','AuthPayloadJson');

SELECT
    'challenge_column_contract' AS check_name,
    required.required_column,
    CASE WHEN actual.column_name IS NULL THEN 'MISSING' ELSE 'PASS' END AS result
FROM (
    SELECT 'Id' AS required_column UNION ALL
    SELECT 'ChallengeId' UNION ALL SELECT 'AuthUserId' UNION ALL SELECT 'Provider' UNION ALL
    SELECT 'CodeHash' UNION ALL SELECT 'ExpiresAtUtc' UNION ALL SELECT 'Consumed' UNION ALL
    SELECT 'Attempts' UNION ALL SELECT 'CreatedAtUtc' UNION ALL SELECT 'AuthPayloadJson'
) required
LEFT JOIN information_schema.columns actual
  ON actual.table_schema = DATABASE()
 AND actual.table_name = 'ra_auth_second_factor_challenge'
 AND actual.column_name = required.required_column
ORDER BY required.required_column;

SELECT
    'challenge_primary_id' AS check_name,
    CASE WHEN COUNT(*) = 1 THEN 'PASS' ELSE 'BLOCKED' END AS result
FROM information_schema.statistics
WHERE table_schema = DATABASE()
  AND table_name = 'ra_auth_second_factor_challenge'
  AND index_name = 'PRIMARY'
  AND column_name = 'Id'
  AND seq_in_index = 1;

SELECT
    'challenge_unique_challenge_id' AS check_name,
    CASE WHEN COUNT(*) = 1 THEN 'PASS' ELSE 'BLOCKED' END AS result
FROM information_schema.statistics
WHERE table_schema = DATABASE()
  AND table_name = 'ra_auth_second_factor_challenge'
  AND column_name = 'ChallengeId'
  AND non_unique = 0
  AND seq_in_index = 1;

SELECT
    'challenge_auth_user_index' AS check_name,
    CASE WHEN COUNT(*) >= 1 THEN 'PASS' ELSE 'BLOCKED' END AS result
FROM information_schema.statistics
WHERE table_schema = DATABASE()
  AND table_name = 'ra_auth_second_factor_challenge'
  AND column_name = 'AuthUserId'
  AND seq_in_index = 1;

SELECT
    'central_module_columns' AS check_name,
    COUNT(*) AS actual_count,
    7 AS expected_count,
    CASE WHEN COUNT(*) = 7 THEN 'PASS' ELSE 'BLOCKED' END AS result
FROM information_schema.columns
WHERE table_schema = DATABASE()
  AND table_name = 'gestor_modulos'
  AND column_name IN ('ID_MODULO','EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA','NOMBRE_MODULO','TIPO_MODULO','RequiereSegundoFactor','SecondFactorProviderType','SegundoFactorTiempoExpira');

SELECT
    'active_smtp_rows' AS check_name,
    COUNT(*) AS actual_count,
    1 AS expected_count,
    CASE WHEN COUNT(*) = 1 THEN 'PASS' ELSE 'BLOCKED' END AS result
FROM Config_Smpt_Side
WHERE ESTADO_ENVIO = 1;

SELECT
    'active_target_module_rows' AS check_name,
    TIPO_MODULO AS module_type,
    COUNT(*) AS actual_count,
    SUM(CASE
        WHEN (RequiereSegundoFactor IS NULL OR RequiereSegundoFactor IN (0,1))
         AND (SecondFactorProviderType IS NULL OR SecondFactorProviderType = 1)
         AND (SegundoFactorTiempoExpira IS NULL OR SegundoFactorTiempoExpira BETWEEN 1 AND 10)
        THEN 0 ELSE 1 END) AS invalid_configuration_count
FROM gestor_modulos
WHERE ESTADO_MODULO = 1
  AND TIPO_MODULO IN ('DOCUARCHI CONTENEDOR','GESTOR DOCUMENTAL','RADICACION DOCUMENTAL','WORKFLOW DOCUMENTAL')
GROUP BY TIPO_MODULO
ORDER BY TIPO_MODULO;

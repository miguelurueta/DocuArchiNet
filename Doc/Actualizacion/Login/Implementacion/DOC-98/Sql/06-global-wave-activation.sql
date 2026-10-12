-- DOC-98. Plantilla para una oleada. Cada UNION ALL representa una fila autorizada.
-- No sustituir el manifiesto por una selección dinámica de todos los módulos.
SET @doc98_authorized = 0;
SET @expected_manifest_rows = -1;

START TRANSACTION;

SELECT COUNT(*) INTO @actual_candidate_rows
FROM gestor_modulos gm
INNER JOIN (
    SELECT -1 AS id_empresa, -1 AS id_modulo,
           'REPLACE_MODULE_NAME_1' AS module_name,
           'REPLACE_MODULE_TYPE_1' AS module_type,
           -1 AS old_required, -1 AS old_provider, -1 AS old_expiry, -1 AS new_expiry
    UNION ALL
    SELECT -2, -2, 'REPLACE_MODULE_NAME_2', 'REPLACE_MODULE_TYPE_2', -1, -1, -1, -1
) manifest
  ON manifest.id_empresa = gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA
 AND manifest.id_modulo = gm.ID_MODULO
 AND manifest.module_name = gm.NOMBRE_MODULO
 AND manifest.module_type = gm.TIPO_MODULO
WHERE @doc98_authorized = 1
  AND @expected_manifest_rows > 0
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor <=> manifest.old_required
  AND gm.SecondFactorProviderType <=> manifest.old_provider
  AND gm.SegundoFactorTiempoExpira <=> manifest.old_expiry
  AND manifest.new_expiry BETWEEN 1 AND 10
FOR UPDATE;

SELECT
    'DOC98_WAVE_CANDIDATES' AS check_name,
    @actual_candidate_rows AS actual_count,
    @expected_manifest_rows AS expected_count;

UPDATE gestor_modulos gm
INNER JOIN (
    SELECT -1 AS id_empresa, -1 AS id_modulo,
           'REPLACE_MODULE_NAME_1' AS module_name,
           'REPLACE_MODULE_TYPE_1' AS module_type,
           -1 AS old_required, -1 AS old_provider, -1 AS old_expiry, -1 AS new_expiry
    UNION ALL
    SELECT -2, -2, 'REPLACE_MODULE_NAME_2', 'REPLACE_MODULE_TYPE_2', -1, -1, -1, -1
) manifest
  ON manifest.id_empresa = gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA
 AND manifest.id_modulo = gm.ID_MODULO
 AND manifest.module_name = gm.NOMBRE_MODULO
 AND manifest.module_type = gm.TIPO_MODULO
SET gm.RequiereSegundoFactor = 1,
    gm.SecondFactorProviderType = 1,
    gm.SegundoFactorTiempoExpira = manifest.new_expiry
WHERE @doc98_authorized = 1
  AND @expected_manifest_rows > 0
  AND @actual_candidate_rows = @expected_manifest_rows
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor <=> manifest.old_required
  AND gm.SecondFactorProviderType <=> manifest.old_provider
  AND gm.SegundoFactorTiempoExpira <=> manifest.old_expiry
  AND manifest.new_expiry BETWEEN 1 AND 10
  AND (SELECT COUNT(*) FROM (
        SELECT -1 AS id_empresa, -1 AS id_modulo UNION ALL SELECT -2, -2
      ) manifest_count) = @expected_manifest_rows;

SELECT ROW_COUNT() AS DOC98_WAVE_AFFECTED_ROWS;

SELECT
    'DOC98_WAVE_POSTCONDITION' AS check_name,
    COUNT(*) AS actual_count,
    @expected_manifest_rows AS expected_count
FROM gestor_modulos gm
INNER JOIN (
    SELECT -1 AS id_empresa, -1 AS id_modulo, -1 AS new_expiry
    UNION ALL SELECT -2, -2, -1
) manifest
  ON manifest.id_empresa = gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA
 AND manifest.id_modulo = gm.ID_MODULO
WHERE gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor = 1
  AND gm.SecondFactorProviderType = 1
  AND gm.SegundoFactorTiempoExpira = manifest.new_expiry;

-- COMMIT solo si candidatos, filas afectadas y postcondición igualan el manifiesto.
ROLLBACK;

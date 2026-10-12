-- DOC-98. Restauración exacta del snapshot de una fila piloto.
SET @doc98_authorized = 0;
SET @id_empresa = -1;
SET @id_modulo = -1;
SET @expected_module_name = 'REPLACE_WITH_APPROVED_MODULE_NAME';
SET @expected_module_type = 'REPLACE_WITH_APPROVED_MODULE_TYPE';
SET @current_expiry_minutes = -1;
SET @restore_required = -1;
SET @restore_provider = -1;
SET @restore_expiry = -1;

START TRANSACTION;

UPDATE gestor_modulos gm
SET gm.RequiereSegundoFactor = @restore_required,
    gm.SecondFactorProviderType = @restore_provider,
    gm.SegundoFactorTiempoExpira = @restore_expiry
WHERE @doc98_authorized = 1
  AND gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = @id_empresa
  AND gm.ID_MODULO = @id_modulo
  AND gm.NOMBRE_MODULO = @expected_module_name
  AND gm.TIPO_MODULO = @expected_module_type
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor = 1
  AND gm.SecondFactorProviderType = 1
  AND gm.SegundoFactorTiempoExpira = @current_expiry_minutes
  AND @current_expiry_minutes BETWEEN 1 AND 10
  AND (@restore_required IS NULL OR @restore_required IN (0,1));

SELECT ROW_COUNT() AS DOC98_ROLLBACK_AFFECTED_ROWS;

SELECT
    'DOC98_ROLLBACK_POSTCONDITION' AS check_name,
    COUNT(*) AS actual_count,
    1 AS expected_count
FROM gestor_modulos gm
WHERE gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = @id_empresa
  AND gm.ID_MODULO = @id_modulo
  AND gm.NOMBRE_MODULO = @expected_module_name
  AND gm.TIPO_MODULO = @expected_module_type
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor <=> @restore_required
  AND gm.SecondFactorProviderType <=> @restore_provider
  AND gm.SegundoFactorTiempoExpira <=> @restore_expiry;

-- COMMIT solo después de comprobar manualmente 1/1 en la misma sesión.
ROLLBACK;

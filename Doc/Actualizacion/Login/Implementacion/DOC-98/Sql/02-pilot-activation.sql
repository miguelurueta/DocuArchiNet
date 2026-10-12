-- DOC-98. Plantilla mutable: una fila piloto, autorización específica y snapshot obligatorio.
-- Los valores inválidos iniciales hacen que una ejecución accidental afecte cero filas.
SET @doc98_authorized = 0;
SET @id_empresa = -1;
SET @id_modulo = -1;
SET @expected_module_name = 'REPLACE_WITH_APPROVED_MODULE_NAME';
SET @expected_module_type = 'REPLACE_WITH_APPROVED_MODULE_TYPE';
SET @snapshot_required = -1;
SET @snapshot_provider = -1;
SET @snapshot_expiry = -1;
SET @target_expiry_minutes = -1;

START TRANSACTION;

SELECT
    'DOC98_PILOT_CANDIDATES' AS check_name,
    COUNT(*) AS actual_count,
    1 AS expected_count
FROM gestor_modulos gm
WHERE @doc98_authorized = 1
  AND gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = @id_empresa
  AND gm.ID_MODULO = @id_modulo
  AND gm.NOMBRE_MODULO = @expected_module_name
  AND gm.TIPO_MODULO = @expected_module_type
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor <=> @snapshot_required
  AND gm.SecondFactorProviderType <=> @snapshot_provider
  AND gm.SegundoFactorTiempoExpira <=> @snapshot_expiry
  AND @target_expiry_minutes BETWEEN 1 AND 10
FOR UPDATE;

UPDATE gestor_modulos gm
SET gm.RequiereSegundoFactor = 1,
    gm.SecondFactorProviderType = 1,
    gm.SegundoFactorTiempoExpira = @target_expiry_minutes
WHERE @doc98_authorized = 1
  AND gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = @id_empresa
  AND gm.ID_MODULO = @id_modulo
  AND gm.NOMBRE_MODULO = @expected_module_name
  AND gm.TIPO_MODULO = @expected_module_type
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor <=> @snapshot_required
  AND gm.SecondFactorProviderType <=> @snapshot_provider
  AND gm.SegundoFactorTiempoExpira <=> @snapshot_expiry
  AND @target_expiry_minutes BETWEEN 1 AND 10;

SELECT ROW_COUNT() AS DOC98_AFFECTED_ROWS;

SELECT
    'DOC98_PILOT_POSTCONDITION' AS check_name,
    COUNT(*) AS actual_count,
    1 AS expected_count
FROM gestor_modulos gm
WHERE gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = @id_empresa
  AND gm.ID_MODULO = @id_modulo
  AND gm.NOMBRE_MODULO = @expected_module_name
  AND gm.TIPO_MODULO = @expected_module_type
  AND gm.ESTADO_MODULO = 1
  AND gm.RequiereSegundoFactor = 1
  AND gm.SecondFactorProviderType = 1
  AND gm.SegundoFactorTiempoExpira = @target_expiry_minutes;

-- COMMIT solo después de comprobar manualmente 1/1/1 en la misma sesión.
ROLLBACK;

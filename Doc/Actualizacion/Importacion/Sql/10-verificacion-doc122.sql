-- DOC-122: comprobación manual, exclusivamente SELECT, MySQL 5.1.37.
-- Ejecutar cada bloque en la conexión indicada. No cambiar bases mediante USE.
-- No publicar resultados con nombres de cuentas o datos de conexión.
-- RA: esquema del permiso propio. No concede ni habilita permisos.
SELECT COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME='remit_dest_interno_perfil_produccion'
  AND COLUMN_NAME='ADMINISTRAR_CARGAS_IMPORTACION';

-- DA: ambos catálogos deben ser InnoDB para guardar atómicamente.
SELECT TABLE_NAME,ENGINE
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp01_xmlconexion','imp01_plantillaimp','imp01_campos_plantilla');

-- DA: metadatos consumidos; no lee datos ni credenciales históricas.
SELECT TABLE_NAME,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT,
       CHARACTER_MAXIMUM_LENGTH,CHARACTER_SET_NAME,COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('imp01_plantillaimp','imp01_campos_plantilla')
ORDER BY TABLE_NAME,ORDINAL_POSITION;

-- DA: la cuenta dedicada debe ejecutar este bloque; no devuelve su identidad.
-- Revisión del DBA: permisos efectivos pueden incluir globales o por tabla.
SELECT 'ESQUEMA' AS NIVEL,PRIVILEGE_TYPE,IS_GRANTABLE
FROM information_schema.SCHEMA_PRIVILEGES
WHERE TABLE_SCHEMA=DATABASE()
  AND GRANTEE=CONCAT(CHAR(39),SUBSTRING_INDEX(CURRENT_USER(),'@',1),CHAR(39),'@',
                     CHAR(39),SUBSTRING_INDEX(CURRENT_USER(),'@',-1),CHAR(39));

SELECT 'GLOBAL' AS NIVEL,PRIVILEGE_TYPE,IS_GRANTABLE
FROM information_schema.USER_PRIVILEGES
WHERE GRANTEE=CONCAT(CHAR(39),SUBSTRING_INDEX(CURRENT_USER(),'@',1),CHAR(39),'@',
                     CHAR(39),SUBSTRING_INDEX(CURRENT_USER(),'@',-1),CHAR(39));

-- La ausencia de filas no acredita privilegios suficientes ni mínimos.
-- Revisar SELECT/INSERT/UPDATE/DELETE para catálogos en esta fase y el contrato
-- completo DOC121 para fases posteriores. DROP y GRANT OPTION no son admisibles.
-- No invocar guardados hasta verificar esquema, cuenta y ámbito del ambiente.

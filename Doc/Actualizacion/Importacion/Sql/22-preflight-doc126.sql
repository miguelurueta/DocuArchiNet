-- DOC126 | SOLO SELECT | SIN APLICACION NI ADECUACIONES AUTOMATICAS
-- Ejecutar cada bloque en su destino indicado. No ejecutar los dos en una misma base.
-- Las referencias NULL se completan en una copia local, sin credenciales.
-- A. RA: verificar permiso y cuenta exclusiva antes de autenticar la E2E.
SELECT DATABASE() AS base_ra,VERSION() AS version_servidor;
SELECT COLUMN_NAME,DATA_TYPE,IS_NULLABLE,COLUMN_DEFAULT
FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE()
AND TABLE_NAME='remit_dest_interno_perfil_produccion'
AND COLUMN_NAME IN ('ADMINISTRAR_CARGAS_IMPORTACION','IMPORTAR_CARPETAS',
 'ADMINISTRAR_PERFILES_IMPORTACION','ADMINISTRAR_FUENTES_IMPORTACION','ADMINISTRAR_PROVEEDORES_IMPORTACION')
ORDER BY COLUMN_NAME;
-- Debe devolver exactamente los dos usuarios, activos y de la empresa autorizada.
-- Propietario: cargas=1 y otros cuatro permisos=0. Restringido: cargas=0.
-- No modificar usuarios/permisos para obtener un PASS; entregar otra adecuacion revisada si hace falta.
SELECT u.id_Remit_Dest_Int AS usuario_id,u.Estado_Usuario AS activo,
 p.ADMINISTRAR_CARGAS_IMPORTACION AS cargas,p.IMPORTAR_CARPETAS AS carpetas,
 p.ADMINISTRAR_PERFILES_IMPORTACION AS perfiles,p.ADMINISTRAR_FUENTES_IMPORTACION AS fuentes,
 p.ADMINISTRAR_PROVEEDORES_IMPORTACION AS proveedores
FROM remit_dest_interno u INNER JOIN remit_dest_interno_perfil_produccion p
 ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int
CROSS JOIN (SELECT CAST(NULL AS UNSIGNED) AS empresa_id,
 CAST(NULL AS UNSIGNED) AS propietario_id,CAST(NULL AS UNSIGNED) AS restringido_id) alcance
WHERE u.Empresa_Gestion_Documental_id_empresa=alcance.empresa_id
AND u.id_Remit_Dest_Int IN (alcance.propietario_id,alcance.restringido_id);
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,activo,revision
FROM ra_integracion_fuente WHERE tabla_nombre IN
('imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar');

-- B. DA: instalacion anterior compatible y nuevos fixtures separados de DOC125.
SELECT DATABASE() AS base_da,VERSION() AS version_servidor;
SELECT TABLE_NAME,ENGINE,TABLE_TYPE FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
('imp01_xmlconexion','imp01_plantillaimp','imp01_campos_plantilla','imp01_carga_ejecucion',
 'imp_02_doc126_e2e_crear','imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar');
SELECT COUNT(*) AS columnas_ejecucion FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp01_carga_ejecucion';
SELECT COLUMN_NAME,DATA_TYPE FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp01_carga_ejecucion'
AND COLUMN_NAME IN ('id_carga','estado','filas_previstas','filas_confirmadas','confirmado_utc');
-- No consultar las columnas de credenciales de imp01_xmlconexion.
SELECT ID_XMLCONEXION,NOMBRE_CONEXION,DBMS_BD FROM imp01_xmlconexion
WHERE BINARY NOMBRE_DB_DEFAULT=BINARY DATABASE() AND LOWER(DBMS_BD)='mysql';
SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,ESTADO_PLANTILLA,REVISION_CARGA,CAMPO_UNICO
FROM imp01_plantillaimp WHERE NOMBRE_PLANTILLA IN
('imp_02_doc126_e2e_crear','imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar');
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE()
AND TABLE_NAME IN ('imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;
SELECT COUNT(*) AS triggers_incompatibles FROM information_schema.TRIGGERS
WHERE EVENT_OBJECT_SCHEMA=DATABASE() AND EVENT_OBJECT_TABLE IN
('imp01_carga_ejecucion','imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar');
SELECT PRIVILEGE_TYPE FROM information_schema.USER_PRIVILEGES
WHERE GRANTEE=CONCAT(QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',1)),'@',QUOTE(SUBSTRING_INDEX(CURRENT_USER(),'@',-1)))
AND PRIVILEGE_TYPE IN ('SELECT','TRIGGER');
-- La ausencia de filas de metadatos no acredita privilegios suficientes.
-- La confirmacion mantiene su inspeccion completa de relaciones y autoridad DOC125.
-- Antes de una escritura, verificar tambien configuracion/huellas/archivos y limites del scope.

-- DOC-98. Ejecutar cada bloque en la base resuelta para el tipo de módulo indicado.
-- Solo entrega conteos agregados. No es una lista ni una puerta de activación por usuario.

SELECT 'DOCUARCHI CONTENEDOR' AS module_type,
       COUNT(*) AS principal_count,
       SUM(CASE WHEN Clave_Usuario IS NULL OR Clave_Usuario <= 0 THEN 1 ELSE 0 END) AS invalid_identity_count,
       SUM(CASE WHEN idusuario IS NULL OR TRIM(idusuario) = '' THEN 1 ELSE 0 END) AS invalid_login_count,
       SUM(CASE WHEN correo IS NULL OR TRIM(correo) NOT LIKE '%_@_%._%' THEN 1 ELSE 0 END) AS invalid_email_count
FROM usuarios_da;
SELECT 'DOCUARCHI CONTENEDOR' AS module_type, COUNT(*) AS duplicated_login_group_count
FROM (SELECT idusuario FROM usuarios_da GROUP BY idusuario HAVING COUNT(*) > 1) duplicate_groups;

SELECT 'GESTOR DOCUMENTAL' AS module_type,
       COUNT(*) AS principal_count,
       SUM(CASE WHEN Id_Remit_Dest_Int IS NULL OR Id_Remit_Dest_Int <= 0 THEN 1 ELSE 0 END) AS invalid_identity_count,
       SUM(CASE WHEN Login_Usuario IS NULL OR TRIM(Login_Usuario) = '' THEN 1 ELSE 0 END) AS invalid_login_count,
       SUM(CASE WHEN Correo_Electronico IS NULL OR TRIM(Correo_Electronico) NOT LIKE '%_@_%._%' THEN 1 ELSE 0 END) AS invalid_email_count
FROM remit_dest_interno;
SELECT 'GESTOR DOCUMENTAL' AS module_type, COUNT(*) AS duplicated_login_group_count
FROM (SELECT Login_Usuario FROM remit_dest_interno GROUP BY Login_Usuario HAVING COUNT(*) > 1) duplicate_groups;

SELECT 'RADICACION DOCUMENTAL' AS module_type,
       COUNT(*) AS principal_count,
       SUM(CASE WHEN id_usuario IS NULL OR id_usuario <= 0 THEN 1 ELSE 0 END) AS invalid_identity_count,
       SUM(CASE WHEN Login_usuario IS NULL OR TRIM(Login_usuario) = '' THEN 1 ELSE 0 END) AS invalid_login_count,
       SUM(CASE WHEN Correo_Usuario IS NULL OR TRIM(Correo_Usuario) NOT LIKE '%_@_%._%' THEN 1 ELSE 0 END) AS invalid_email_count
FROM usuario_radicador;
SELECT 'RADICACION DOCUMENTAL' AS module_type, COUNT(*) AS duplicated_login_group_count
FROM (SELECT Login_usuario FROM usuario_radicador GROUP BY Login_usuario HAVING COUNT(*) > 1) duplicate_groups;

SELECT 'WORKFLOW DOCUMENTAL' AS module_type,
       COUNT(*) AS principal_count,
       SUM(CASE WHEN idU_suario IS NULL OR idU_suario <= 0 THEN 1 ELSE 0 END) AS invalid_identity_count,
       SUM(CASE WHEN login_Usuario IS NULL OR TRIM(login_Usuario) = '' THEN 1 ELSE 0 END) AS invalid_login_count,
       SUM(CASE WHEN Correo_Usuario IS NULL OR TRIM(Correo_Usuario) NOT LIKE '%_@_%._%' THEN 1 ELSE 0 END) AS invalid_email_count
FROM usuario_workflow;
SELECT 'WORKFLOW DOCUMENTAL' AS module_type, COUNT(*) AS duplicated_login_group_count
FROM (SELECT login_Usuario FROM usuario_workflow GROUP BY login_Usuario HAVING COUNT(*) > 1) duplicate_groups;

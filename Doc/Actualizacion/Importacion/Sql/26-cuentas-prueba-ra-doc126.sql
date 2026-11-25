-- DOC126 | RA | SOLO SELECT | NO MODIFICA CUENTAS NI PERMISOS
-- Elegir dos cuentas dedicadas de prueba, activas y de empresa 2.
-- Sustituir NULL en UNA COPIA LOCAL por sus IDs; nunca usar 141 (LUZ.AGUILERA).
-- No incluir contrasenas. La seleccion de cuentas debe confirmarla el usuario.
SELECT DATABASE() AS base_ra, VERSION() AS version_servidor;

SELECT TABLE_NAME, ENGINE FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE()
AND TABLE_NAME IN ('remit_dest_interno','remit_dest_interno_perfil_produccion');

SELECT u.id_Remit_Dest_Int AS usuario_id,u.Login_Usuario AS usuario,
 u.Empresa_Gestion_Documental_id_empresa AS empresa_id,u.Estado_Usuario AS activo,
 p.id_perfil_produccion AS perfil_id,
 p.ADMINISTRAR_CARGAS_IMPORTACION AS cargas,p.IMPORTAR_CARPETAS AS carpetas,
 p.ADMINISTRAR_PERFILES_IMPORTACION AS perfiles,
 p.ADMINISTRAR_FUENTES_IMPORTACION AS fuentes,
 p.ADMINISTRAR_PROVEEDORES_IMPORTACION AS proveedores
FROM remit_dest_interno u
INNER JOIN remit_dest_interno_perfil_produccion p
 ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int
CROSS JOIN (SELECT CAST(NULL AS UNSIGNED) AS exclusiva_id,
 CAST(NULL AS UNSIGNED) AS restringida_id) alcance
WHERE u.Empresa_Gestion_Documental_id_empresa=2
AND u.id_Remit_Dest_Int IN (alcance.exclusiva_id,alcance.restringida_id)
AND alcance.exclusiva_id<>alcance.restringida_id
AND alcance.exclusiva_id<>141 AND alcance.restringida_id<>141
ORDER BY u.id_Remit_Dest_Int,p.id_perfil_produccion;

-- Deben aparecer exactamente dos filas: una por usuario, activos y empresa 2.
-- Exclusiva: cargas 0 o 1; carpetas/perfiles/fuentes/proveedores EXACTAMENTE 0.
-- Restringida: cargas EXACTAMENTE 0; no cambiar sus otros permisos.
-- Si falta un perfil, esta duplicado o sobran permisos, DETENER:
-- no crear usuarios, quitar permisos ni reparar perfiles desde esta prueba.
-- SQL27 solo habilita cargas en la exclusiva elegida; no adecua otros permisos.

-- DOC-110. Compatible con MySQL 5.1.
-- Ejecutar manualmente en la base RA de Gestion Documental.
-- La columna ADMINISTRAR_PERFILES_IMPORTACION ya fue creada por SQL 01.
-- Actualiza la fila existente; no crea usuarios ni modifica otros permisos.
-- La inspeccion previa encontro una cuenta activa y una fila de permisos.

SELECT u.id_Remit_Dest_Int, u.Login_Usuario,
       p.ADMINISTRAR_PERFILES_IMPORTACION
FROM remit_dest_interno AS u
INNER JOIN remit_dest_interno_perfil_produccion AS p
  ON p.remit_dest_interno_idremit_dest_interno = u.id_Remit_Dest_Int
WHERE u.Login_Usuario = 'LUZ.AGUILERA'
  AND u.Estado_Usuario = 1
  AND u.Empresa_Gestion_Documental_id_empresa > 0;

UPDATE remit_dest_interno_perfil_produccion AS p
INNER JOIN remit_dest_interno AS u
  ON u.id_Remit_Dest_Int = p.remit_dest_interno_idremit_dest_interno
SET p.ADMINISTRAR_PERFILES_IMPORTACION = 1
WHERE u.Login_Usuario = 'LUZ.AGUILERA'
  AND u.Estado_Usuario = 1
  AND u.Empresa_Gestion_Documental_id_empresa > 0;

SELECT u.id_Remit_Dest_Int, u.Login_Usuario,
       p.ADMINISTRAR_PERFILES_IMPORTACION
FROM remit_dest_interno AS u
INNER JOIN remit_dest_interno_perfil_produccion AS p
  ON p.remit_dest_interno_idremit_dest_interno = u.id_Remit_Dest_Int
WHERE u.Login_Usuario = 'LUZ.AGUILERA'
  AND u.Estado_Usuario = 1
  AND u.Empresa_Gestion_Documental_id_empresa > 0;

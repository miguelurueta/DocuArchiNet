-- DOC126 | RA | APLICACION MANUAL | PENDIENTE_IDENTIFICAR_CUENTAS
-- Requiere SQL26 revisado y dos cuentas dedicadas confirmadas por el usuario.
-- Efecto maximo: habilitar SOLO cargas (0->1) en UN perfil existente.
-- No crea usuarios/perfiles, no toca otras capacidades ni la cuenta 141.
-- No forma parte del arranque ni de la E2E. No aplicar a una cuenta de negocio.
SET @doc126_exclusiva=NULL;
SET @doc126_restringida=NULL;
SET @doc126_cuentas_prueba_confirmadas=0;

-- Conservar la transaccion y los bloqueos hasta COMMIT. MySQL 5.1/InnoDB.
-- El indice por usuario del perfil no es UNIQUE: bloquear los rangos antes de
-- comprobar que cada usuario tiene un solo perfil evita actualizar duplicados.
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
START TRANSACTION;
SELECT u.id_Remit_Dest_Int AS usuario_id,u.Login_Usuario AS usuario,
 p.id_perfil_produccion AS perfil_id,
 p.ADMINISTRAR_CARGAS_IMPORTACION AS cargas,p.IMPORTAR_CARPETAS AS carpetas,
 p.ADMINISTRAR_PERFILES_IMPORTACION AS perfiles,
 p.ADMINISTRAR_FUENTES_IMPORTACION AS fuentes,
 p.ADMINISTRAR_PROVEEDORES_IMPORTACION AS proveedores
FROM remit_dest_interno u
INNER JOIN remit_dest_interno_perfil_produccion p
 ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int
WHERE u.Empresa_Gestion_Documental_id_empresa=2
AND u.id_Remit_Dest_Int IN (@doc126_exclusiva,@doc126_restringida)
AND @doc126_cuentas_prueba_confirmadas=1
AND @doc126_exclusiva<>141 AND @doc126_restringida<>141
FOR UPDATE;

SET @doc126_cuentas_aptas=IFNULL((
 @doc126_cuentas_prueba_confirmadas=1
 AND @doc126_exclusiva REGEXP '^[1-9][0-9]*$'
 AND @doc126_restringida REGEXP '^[1-9][0-9]*$'
 AND @doc126_exclusiva BETWEEN 1 AND 4294967295
 AND @doc126_restringida BETWEEN 1 AND 4294967295
 AND @doc126_exclusiva<>@doc126_restringida
 AND @doc126_exclusiva<>141 AND @doc126_restringida<>141
 AND (SELECT COUNT(*) FROM information_schema.TABLES
      WHERE TABLE_SCHEMA=DATABASE() AND ENGINE='InnoDB'
      AND TABLE_NAME IN ('remit_dest_interno','remit_dest_interno_perfil_produccion'))=2
 AND (SELECT COUNT(*) FROM remit_dest_interno
      WHERE Empresa_Gestion_Documental_id_empresa=2 AND Estado_Usuario=1
      AND id_Remit_Dest_Int IN (@doc126_exclusiva,@doc126_restringida))=2
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno IN
       (@doc126_exclusiva,@doc126_restringida))=2
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno=@doc126_exclusiva
      AND ADMINISTRAR_CARGAS_IMPORTACION IN (0,1)
      AND IMPORTAR_CARPETAS=0 AND ADMINISTRAR_PERFILES_IMPORTACION=0
      AND ADMINISTRAR_FUENTES_IMPORTACION=0 AND ADMINISTRAR_PROVEEDORES_IMPORTACION=0)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno=@doc126_restringida
      AND ADMINISTRAR_CARGAS_IMPORTACION=0)=1
),0);

UPDATE remit_dest_interno_perfil_produccion p
INNER JOIN remit_dest_interno u
 ON u.id_Remit_Dest_Int=p.remit_dest_interno_idremit_dest_interno
SET p.ADMINISTRAR_CARGAS_IMPORTACION=1
WHERE @doc126_cuentas_aptas=1
AND u.id_Remit_Dest_Int=@doc126_exclusiva
AND u.id_Remit_Dest_Int<>141
AND u.Empresa_Gestion_Documental_id_empresa=2 AND u.Estado_Usuario=1
AND p.ADMINISTRAR_CARGAS_IMPORTACION=0
AND p.IMPORTAR_CARPETAS=0 AND p.ADMINISTRAR_PERFILES_IMPORTACION=0
AND p.ADMINISTRAR_FUENTES_IMPORTACION=0 AND p.ADMINISTRAR_PROVEEDORES_IMPORTACION=0;
SET @doc126_perfiles_modificados=ROW_COUNT();
COMMIT;

SELECT @doc126_cuentas_aptas AS precondiciones_cumplidas,
 @doc126_perfiles_modificados AS perfiles_modificados;
-- Esperado: aptas=1, modificados=1 (o 0 si ya estaba habilitada).
-- aptas=0 significa DETENER; no acredita aplicacion ni autoriza la E2E.
-- Verificar desde conexion NUEVA mediante SQL26: dos perfiles, cargas 1/0,
-- cuatro capacidades de la exclusiva en 0. Contraseñas solo en consola privada.

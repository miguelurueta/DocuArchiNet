-- DOC127 SQL31 | RA | APLICACION MANUAL | ENTREGADO, NO APLICADO
-- Requiere28 revisado y SQL30 confirmado en DA. No concede permisos.
-- Dos fuentes nuevas CONEXION, cuatro columnas y cuatro historiales ALTA/ACTIVAR.
-- Preserva fuentes8/9 DOC126 y no crea/publica proveedores.
-- COMPLETAR empresa y actor reales; NULL impide las escrituras.
-- Ejecutar sin continuar ante errores; mantener la misma sesion abierta.
SET @doc127_empresa=NULL;
SET @doc127_actor=NULL;
SET @doc127_utc=UTC_TIMESTAMP();
SET @doc127_fuente_agregar=NULL;
SET @doc127_fuente_reemplazar=NULL;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
START TRANSACTION;
SELECT fuente_id FROM ra_integracion_fuente
WHERE conexion_ref='DA' AND tabla_nombre IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY fuente_id FOR UPDATE;
-- Fuente de fixture agregar: alta/activacion con historial, sin proveedor.
SET @doc127_apta=(
 @doc127_empresa=2 AND @doc127_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=@doc127_empresa)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno WHERE id_Remit_Dest_Int=@doc127_actor
      AND Empresa_Gestion_Documental_id_empresa=@doc127_empresa AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno=@doc127_actor)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno=@doc127_actor
      AND ADMINISTRAR_FUENTES_IMPORTACION=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre='imp_02_doc127_e2e_agregar_xls')=0
);
INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc)
SELECT CAST(@doc127_empresa AS CHAR),'DA','imp_02_doc127_e2e_agregar_xls','CONEXION','',0,1,@doc127_actor,@doc127_utc
FROM DUAL WHERE @doc127_apta=1;
SET @doc127_alta=ROW_COUNT();
SET @doc127_id=IF(@doc127_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc127_id,c.nombre FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc127_alta=1;
SET @doc127_columnas=ROW_COUNT();
SET @doc127_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc127_id,'","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_doc127_e2e_agregar_xls"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc127_id,1,'ALTA',@doc127_actor,@doc127_utc,'DOC127: fixture descartable manual; conservar efectos.',@doc127_doc
FROM DUAL WHERE @doc127_alta=1 AND @doc127_columnas=2;
SET @doc127_hist=ROW_COUNT();
UPDATE ra_integracion_fuente SET activo=1,revision=2,actualizado_por=@doc127_actor,actualizado_utc=@doc127_utc
WHERE fuente_id=@doc127_id AND activo=0 AND revision=1 AND @doc127_hist=1;
SET @doc127_activa=ROW_COUNT();
SET @doc127_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc127_id,'","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_doc127_e2e_agregar_xls"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc127_id,2,'ACTIVAR',@doc127_actor,@doc127_utc,'DOC127: activacion manual de fixture autorizado.',@doc127_doc
FROM DUAL WHERE @doc127_activa=1;
SET @doc127_hist_activa=ROW_COUNT();
SET @doc127_fuente_agregar=IF(@doc127_alta=1 AND @doc127_columnas=2 AND @doc127_hist=1 AND @doc127_activa=1 AND @doc127_hist_activa=1,@doc127_id,NULL);

-- Fuente de fixture reemplazar: alta/activacion con historial, sin proveedor.
SET @doc127_apta=(
 @doc127_empresa=2 AND @doc127_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=@doc127_empresa)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno WHERE id_Remit_Dest_Int=@doc127_actor
      AND Empresa_Gestion_Documental_id_empresa=@doc127_empresa AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno=@doc127_actor)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno_perfil_produccion
      WHERE remit_dest_interno_idremit_dest_interno=@doc127_actor
      AND ADMINISTRAR_FUENTES_IMPORTACION=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre='imp_02_doc127_e2e_reemplazar_xls')=0
);
INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc)
SELECT CAST(@doc127_empresa AS CHAR),'DA','imp_02_doc127_e2e_reemplazar_xls','CONEXION','',0,1,@doc127_actor,@doc127_utc
FROM DUAL WHERE @doc127_apta=1;
SET @doc127_alta=ROW_COUNT();
SET @doc127_id=IF(@doc127_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc127_id,c.nombre FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc127_alta=1;
SET @doc127_columnas=ROW_COUNT();
SET @doc127_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc127_id,'","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_doc127_e2e_reemplazar_xls"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc127_id,1,'ALTA',@doc127_actor,@doc127_utc,'DOC127: fixture descartable manual; conservar efectos.',@doc127_doc
FROM DUAL WHERE @doc127_alta=1 AND @doc127_columnas=2;
SET @doc127_hist=ROW_COUNT();
UPDATE ra_integracion_fuente SET activo=1,revision=2,actualizado_por=@doc127_actor,actualizado_utc=@doc127_utc
WHERE fuente_id=@doc127_id AND activo=0 AND revision=1 AND @doc127_hist=1;
SET @doc127_activa=ROW_COUNT();
SET @doc127_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc127_id,'","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_doc127_e2e_reemplazar_xls"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc127_id,2,'ACTIVAR',@doc127_actor,@doc127_utc,'DOC127: activacion manual de fixture autorizado.',@doc127_doc
FROM DUAL WHERE @doc127_activa=1;
SET @doc127_hist_activa=ROW_COUNT();
SET @doc127_fuente_reemplazar=IF(@doc127_alta=1 AND @doc127_columnas=2 AND @doc127_hist=1 AND @doc127_activa=1 AND @doc127_hist_activa=1,@doc127_id,NULL);

SELECT IF(@doc127_fuente_agregar IS NOT NULL AND @doc127_fuente_reemplazar IS NOT NULL,
 'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
 @doc127_fuente_agregar AS fuente_agregar,@doc127_fuente_reemplazar AS fuente_reemplazar;
-- SOLO ante LISTO_PARA_COMMIT y cero errores: COMMIT manual en esta misma sesion.
-- Otro resultado o error: ROLLBACK manual. No hay COMMIT automatico.
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,activo,revision FROM ra_integracion_fuente
WHERE fuente_id IN (@doc127_fuente_agregar,@doc127_fuente_reemplazar) ORDER BY fuente_id;

-- Despues de COMMIT confirmado, revisar33 mediante SELECT en otra conexion.
-- No reaplicar31 si ya existe cualquiera de estas fuentes: revisar su estado.

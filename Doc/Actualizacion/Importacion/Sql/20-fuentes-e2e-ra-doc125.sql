-- DOC125 | RA | SOLO APLICACION MANUAL | NO EJECUTADO
-- Requiere SQL18 revisado y SQL19 confirmado en DA. No concede permisos.
-- COMPLETAR empresa y actor reales; NULL impide las escrituras.
-- Ejecutar sin continuar ante errores; mantener la misma sesion abierta.
SET @doc125_empresa=NULL;
SET @doc125_actor=NULL;
SET @doc125_utc=UTC_TIMESTAMP();
SET @doc125_fuente_agregar=NULL;
SET @doc125_fuente_reemplazar=NULL;
START TRANSACTION;
-- Fuente de fixture agregar: alta/activacion con historial, sin proveedor.
SET @doc125_apta=(
 @doc125_empresa REGEXP '^[1-9][0-9]*$' AND @doc125_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=@doc125_empresa)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno WHERE id_Remit_Dest_Int=@doc125_actor
      AND Empresa_Gestion_Documental_id_empresa=@doc125_empresa AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre='imp_02_doc125_e2e_agregar')=0
);
INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc)
SELECT CAST(@doc125_empresa AS CHAR),'DA','imp_02_doc125_e2e_agregar','CONEXION','',0,1,@doc125_actor,@doc125_utc
FROM DUAL WHERE @doc125_apta=1;
SET @doc125_alta=ROW_COUNT();
SET @doc125_id=IF(@doc125_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc125_id,c.nombre FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc125_alta=1;
SET @doc125_columnas=ROW_COUNT();
SET @doc125_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc125_id,'","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_doc125_e2e_agregar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc125_id,1,'ALTA',@doc125_actor,@doc125_utc,'DOC125: fixture descartable manual; conservar efectos.',@doc125_doc
FROM DUAL WHERE @doc125_alta=1 AND @doc125_columnas=2;
SET @doc125_hist=ROW_COUNT();
UPDATE ra_integracion_fuente SET activo=1,revision=2,actualizado_por=@doc125_actor,actualizado_utc=@doc125_utc
WHERE fuente_id=@doc125_id AND activo=0 AND revision=1 AND @doc125_hist=1;
SET @doc125_activa=ROW_COUNT();
SET @doc125_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc125_id,'","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_doc125_e2e_agregar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc125_id,2,'ACTIVAR',@doc125_actor,@doc125_utc,'DOC125: activacion manual de fixture autorizado.',@doc125_doc
FROM DUAL WHERE @doc125_activa=1;
SET @doc125_hist_activa=ROW_COUNT();
SET @doc125_fuente_agregar=IF(@doc125_alta=1 AND @doc125_columnas=2 AND @doc125_hist=1 AND @doc125_activa=1 AND @doc125_hist_activa=1,@doc125_id,NULL);

-- Fuente de fixture reemplazar: alta/activacion con historial, sin proveedor.
SET @doc125_apta=(
 @doc125_empresa REGEXP '^[1-9][0-9]*$' AND @doc125_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=@doc125_empresa)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno WHERE id_Remit_Dest_Int=@doc125_actor
      AND Empresa_Gestion_Documental_id_empresa=@doc125_empresa AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre='imp_02_doc125_e2e_reemplazar')=0
);
INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc)
SELECT CAST(@doc125_empresa AS CHAR),'DA','imp_02_doc125_e2e_reemplazar','CONEXION','',0,1,@doc125_actor,@doc125_utc
FROM DUAL WHERE @doc125_apta=1;
SET @doc125_alta=ROW_COUNT();
SET @doc125_id=IF(@doc125_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc125_id,c.nombre FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc125_alta=1;
SET @doc125_columnas=ROW_COUNT();
SET @doc125_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc125_id,'","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_doc125_e2e_reemplazar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc125_id,1,'ALTA',@doc125_actor,@doc125_utc,'DOC125: fixture descartable manual; conservar efectos.',@doc125_doc
FROM DUAL WHERE @doc125_alta=1 AND @doc125_columnas=2;
SET @doc125_hist=ROW_COUNT();
UPDATE ra_integracion_fuente SET activo=1,revision=2,actualizado_por=@doc125_actor,actualizado_utc=@doc125_utc
WHERE fuente_id=@doc125_id AND activo=0 AND revision=1 AND @doc125_hist=1;
SET @doc125_activa=ROW_COUNT();
SET @doc125_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc125_id,'","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_doc125_e2e_reemplazar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc125_id,2,'ACTIVAR',@doc125_actor,@doc125_utc,'DOC125: activacion manual de fixture autorizado.',@doc125_doc
FROM DUAL WHERE @doc125_activa=1;
SET @doc125_hist_activa=ROW_COUNT();
SET @doc125_fuente_reemplazar=IF(@doc125_alta=1 AND @doc125_columnas=2 AND @doc125_hist=1 AND @doc125_activa=1 AND @doc125_hist_activa=1,@doc125_id,NULL);

SELECT IF(@doc125_fuente_agregar IS NOT NULL AND @doc125_fuente_reemplazar IS NOT NULL,
 'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
 @doc125_fuente_agregar AS fuente_agregar,@doc125_fuente_reemplazar AS fuente_reemplazar;
-- SOLO ante LISTO_PARA_COMMIT y cero errores: COMMIT manual en esta misma sesion.
-- Otro resultado o error: ROLLBACK manual. No hay COMMIT automatico.
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,activo,revision FROM ra_integracion_fuente
WHERE fuente_id IN (@doc125_fuente_agregar,@doc125_fuente_reemplazar) ORDER BY fuente_id;

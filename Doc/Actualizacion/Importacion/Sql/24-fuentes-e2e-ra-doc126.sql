-- DOC126 | RA | SOLO APLICACION MANUAL | NO EJECUTADO
-- Requiere SQL22 revisado y SQL23 confirmado en DA. No concede permisos.
-- COMPLETAR empresa y actor reales; NULL impide las escrituras.
-- Ejecutar sin continuar ante errores; mantener la misma sesion abierta.
SET @doc126_empresa=NULL;
SET @doc126_actor=NULL;
SET @doc126_utc=UTC_TIMESTAMP();
SET @doc126_fuente_agregar=NULL;
SET @doc126_fuente_reemplazar=NULL;
START TRANSACTION;
-- Fuente de fixture agregar: alta/activacion con historial, sin proveedor.
SET @doc126_apta=(
 @doc126_empresa REGEXP '^[1-9][0-9]*$' AND @doc126_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=@doc126_empresa)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno WHERE id_Remit_Dest_Int=@doc126_actor
      AND Empresa_Gestion_Documental_id_empresa=@doc126_empresa AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre='imp_02_doc126_e2e_agregar')=0
);
INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc)
SELECT CAST(@doc126_empresa AS CHAR),'DA','imp_02_doc126_e2e_agregar','CONEXION','',0,1,@doc126_actor,@doc126_utc
FROM DUAL WHERE @doc126_apta=1;
SET @doc126_alta=ROW_COUNT();
SET @doc126_id=IF(@doc126_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc126_id,c.nombre FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc126_alta=1;
SET @doc126_columnas=ROW_COUNT();
SET @doc126_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc126_id,'","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_doc126_e2e_agregar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc126_id,1,'ALTA',@doc126_actor,@doc126_utc,'DOC126: fixture descartable manual; conservar efectos.',@doc126_doc
FROM DUAL WHERE @doc126_alta=1 AND @doc126_columnas=2;
SET @doc126_hist=ROW_COUNT();
UPDATE ra_integracion_fuente SET activo=1,revision=2,actualizado_por=@doc126_actor,actualizado_utc=@doc126_utc
WHERE fuente_id=@doc126_id AND activo=0 AND revision=1 AND @doc126_hist=1;
SET @doc126_activa=ROW_COUNT();
SET @doc126_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc126_id,'","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_doc126_e2e_agregar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc126_id,2,'ACTIVAR',@doc126_actor,@doc126_utc,'DOC126: activacion manual de fixture autorizado.',@doc126_doc
FROM DUAL WHERE @doc126_activa=1;
SET @doc126_hist_activa=ROW_COUNT();
SET @doc126_fuente_agregar=IF(@doc126_alta=1 AND @doc126_columnas=2 AND @doc126_hist=1 AND @doc126_activa=1 AND @doc126_hist_activa=1,@doc126_id,NULL);

-- Fuente de fixture reemplazar: alta/activacion con historial, sin proveedor.
SET @doc126_apta=(
 @doc126_empresa REGEXP '^[1-9][0-9]*$' AND @doc126_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=@doc126_empresa)=1
 AND (SELECT COUNT(*) FROM remit_dest_interno WHERE id_Remit_Dest_Int=@doc126_actor
      AND Empresa_Gestion_Documental_id_empresa=@doc126_empresa AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente WHERE conexion_ref='DA' AND tabla_nombre='imp_02_doc126_e2e_reemplazar')=0
);
INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc)
SELECT CAST(@doc126_empresa AS CHAR),'DA','imp_02_doc126_e2e_reemplazar','CONEXION','',0,1,@doc126_actor,@doc126_utc
FROM DUAL WHERE @doc126_apta=1;
SET @doc126_alta=ROW_COUNT();
SET @doc126_id=IF(@doc126_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc126_id,c.nombre FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc126_alta=1;
SET @doc126_columnas=ROW_COUNT();
SET @doc126_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc126_id,'","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_doc126_e2e_reemplazar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc126_id,1,'ALTA',@doc126_actor,@doc126_utc,'DOC126: fixture descartable manual; conservar efectos.',@doc126_doc
FROM DUAL WHERE @doc126_alta=1 AND @doc126_columnas=2;
SET @doc126_hist=ROW_COUNT();
UPDATE ra_integracion_fuente SET activo=1,revision=2,actualizado_por=@doc126_actor,actualizado_utc=@doc126_utc
WHERE fuente_id=@doc126_id AND activo=0 AND revision=1 AND @doc126_hist=1;
SET @doc126_activa=ROW_COUNT();
SET @doc126_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"","Columnas":["CLAVE","VALOR"],"ConexionRef":"DA","FuenteId":"',@doc126_id,'","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_doc126_e2e_reemplazar"}');
INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc126_id,2,'ACTIVAR',@doc126_actor,@doc126_utc,'DOC126: activacion manual de fixture autorizado.',@doc126_doc
FROM DUAL WHERE @doc126_activa=1;
SET @doc126_hist_activa=ROW_COUNT();
SET @doc126_fuente_reemplazar=IF(@doc126_alta=1 AND @doc126_columnas=2 AND @doc126_hist=1 AND @doc126_activa=1 AND @doc126_hist_activa=1,@doc126_id,NULL);

SELECT IF(@doc126_fuente_agregar IS NOT NULL AND @doc126_fuente_reemplazar IS NOT NULL,
 'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
 @doc126_fuente_agregar AS fuente_agregar,@doc126_fuente_reemplazar AS fuente_reemplazar;
-- SOLO ante LISTO_PARA_COMMIT y cero errores: COMMIT manual en esta misma sesion.
-- Otro resultado o error: ROLLBACK manual. No hay COMMIT automatico.
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,activo,revision FROM ra_integracion_fuente
WHERE fuente_id IN (@doc126_fuente_agregar,@doc126_fuente_reemplazar) ORDER BY fuente_id;

-- DOC124. APLICACION MANUAL en RA, MySQL5.1/InnoDB.
-- Configura DA/imp_02_contratos exclusiva de empresa2, ya comprobada.
-- EFECTO PERMANENTE: alta de fuente, cuatro columnas, activacion e historial.
-- No publica proveedores, no concede permisos ni modifica tablas de negocio.
-- Ejecutar sin continuar ante errores; ante cualquier error: ROLLBACK y detener.
-- COMPLETAR actor: ID real del operador registrado en RA, no una identidad ficticia.
-- NULL deja el archivo sin escrituras. Revisar resultado final antes de confirmar.

SET @doc124_actor=NULL;
SET @doc124_fuente_id=NULL;
SET @doc124_instante=UTC_TIMESTAMP();
START TRANSACTION;
SET @doc124_fuente_apta=(
 (SELECT COUNT(*) FROM empresa_gestion_documental)=1
 AND (SELECT COUNT(*) FROM empresa_gestion_documental WHERE ID_EMPRESA=2)=1
 AND @doc124_actor REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM remit_dest_interno
      WHERE id_Remit_Dest_Int=@doc124_actor
        AND Empresa_Gestion_Documental_id_empresa=2 AND Estado_Usuario=1)=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES
      WHERE TABLE_SCHEMA=DATABASE() AND ENGINE='InnoDB'
        AND TABLE_NAME IN ('ra_integracion_fuente','ra_integracion_fuente_columna',
                           'ra_integracion_fuente_historial'))=3
 AND (SELECT COUNT(*) FROM ra_integracion_fuente
      WHERE conexion_ref='DA' AND tabla_nombre='imp_02_contratos')=0
);

INSERT INTO ra_integracion_fuente
 (ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,
  activo,revision,creado_por,creado_utc)
SELECT '2','DA','imp_02_contratos','CONEXION','',0,1,@doc124_actor,@doc124_instante
FROM DUAL WHERE @doc124_fuente_apta=1;
SET @doc124_fuente_alta=ROW_COUNT();
SET @doc124_fuente_id=IF(@doc124_fuente_alta=1,LAST_INSERT_ID(),NULL);

INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre)
SELECT @doc124_fuente_id,c.nombre FROM (
 SELECT 'FECHA' AS nombre UNION ALL SELECT 'NCONTRATO'
 UNION ALL SELECT 'NIT' UNION ALL SELECT 'NOMBRE'
) c WHERE @doc124_fuente_alta=1;
SET @doc124_fuente_columnas=ROW_COUNT();

-- Mismo DocumentoFuenteImportacion que el repositorio existente: alta inactiva.
SET @doc124_fuente_doc=CONCAT('{"Activo":false,"ColumnaEmpresa":"",',
 '"Columnas":["FECHA","NCONTRATO","NIT","NOMBRE"],"ConexionRef":"DA",',
 '"FuenteId":"',@doc124_fuente_id,
 '","ModoAislamiento":"CONEXION","Revision":1,"Tabla":"imp_02_contratos"}');
INSERT INTO ra_integracion_fuente_historial
 (fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc124_fuente_id,1,'ALTA',@doc124_actor,@doc124_instante,
 'DOC124: alta manual CONTRATOS; empresa unica2 verificada por SELECT.',
 @doc124_fuente_doc
FROM DUAL WHERE @doc124_fuente_alta=1 AND @doc124_fuente_columnas=4;
SET @doc124_fuente_hist_alta=ROW_COUNT();

UPDATE ra_integracion_fuente
SET activo=1,revision=2,actualizado_por=@doc124_actor,actualizado_utc=@doc124_instante
WHERE fuente_id=@doc124_fuente_id AND ambito_ref='2' AND conexion_ref='DA'
  AND tabla_nombre='imp_02_contratos' AND activo=0 AND revision=1
  AND @doc124_fuente_hist_alta=1 AND @doc124_fuente_columnas=4;
SET @doc124_fuente_activada=ROW_COUNT();
SET @doc124_fuente_doc=CONCAT('{"Activo":true,"ColumnaEmpresa":"",',
 '"Columnas":["FECHA","NCONTRATO","NIT","NOMBRE"],"ConexionRef":"DA",',
 '"FuenteId":"',@doc124_fuente_id,
 '","ModoAislamiento":"CONEXION","Revision":2,"Tabla":"imp_02_contratos"}');
INSERT INTO ra_integracion_fuente_historial
 (fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc)
SELECT @doc124_fuente_id,2,'ACTIVAR',@doc124_actor,@doc124_instante,
 'DOC124: activacion manual explicita de CONTRATOS para empresa2.',
 @doc124_fuente_doc
FROM DUAL WHERE @doc124_fuente_activada=1;
SET @doc124_fuente_hist_activar=ROW_COUNT();

SELECT IF(@doc124_fuente_alta=1 AND @doc124_fuente_columnas=4
          AND @doc124_fuente_hist_alta=1 AND @doc124_fuente_activada=1
          AND @doc124_fuente_hist_activar=1,'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
       @doc124_fuente_id AS fuente_id;
-- SOLO si LISTO_PARA_COMMIT y no hubo errores, ejecutar manualmente COMMIT.
-- Cualquier otro resultado: ROLLBACK. No reintentar si la fuente ya existe.
-- No hay COMMIT automatico para evitar confirmar un lote parcial en clientes
-- que continuen despues de un error. Mantener abierta esta misma sesion SQL.

SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,
       columna_empresa,activo,revision
FROM ra_integracion_fuente WHERE fuente_id=@doc124_fuente_id;
SELECT revision,accion,actor_ref FROM ra_integracion_fuente_historial
WHERE fuente_id=@doc124_fuente_id ORDER BY revision;

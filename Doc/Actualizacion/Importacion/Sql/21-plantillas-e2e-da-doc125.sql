-- DOC125 | DA | SOLO APLICACION MANUAL | NO EJECUTADO
-- Requiere SQL19 y SQL20 confirmados/verificados. Reutiliza la conexion del modulo.
-- COMPLETAR el ID real de imp01_xmlconexion; NULL impide DML.
-- Da de alta tres plantillas y tres filas SINTETICAS iniciales en las tablas de fixture.
-- No modifica plantillas o datos anteriores ni crea el destino del modo CREAR.
SET @doc125_conexion=NULL;
SET @doc125_plantilla_crear=NULL;
SET @doc125_plantilla_agregar=NULL;
SET @doc125_plantilla_reemplazar=NULL;
START TRANSACTION;
SET @doc125_apta=(
 @doc125_conexion REGEXP '^[1-9][0-9]*$'
 AND (SELECT COUNT(*) FROM imp01_xmlconexion WHERE ID_XMLCONEXION=@doc125_conexion
      AND LOWER(DBMS_BD)='mysql' AND BINARY NOMBRE_DB_DEFAULT=BINARY DATABASE())=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND ENGINE='InnoDB'
      AND TABLE_NAME IN ('imp01_plantillaimp','imp01_campos_plantilla','imp_02_doc125_e2e_agregar','imp_02_doc125_e2e_reemplazar'))=4
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_doc125_e2e_crear')=0
 AND (SELECT COUNT(*) FROM imp01_plantillaimp WHERE NOMBRE_PLANTILLA IN
      ('imp_02_doc125_e2e_crear','imp_02_doc125_e2e_agregar','imp_02_doc125_e2e_reemplazar'))=0
 AND (SELECT COUNT(*) FROM imp_02_doc125_e2e_agregar)=0
 AND (SELECT COUNT(*) FROM imp_02_doc125_e2e_reemplazar)=0
);
-- Plantilla crear; CREAR permanece como intencion sin tabla fisica.
INSERT INTO imp01_plantillaimp
 (IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,RUTA_ARCHIVO_FUENTE,
  ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA)
SELECT @doc125_conexion,'imp_02_doc125_e2e_crear','12.0 xlsx','Datos',CONCAT('web/',REPLACE(UUID(),'-',''),'.xlsx'),
 0,0,1,0,'CLAVE',1 FROM DUAL WHERE @doc125_apta=1;
SET @doc125_alta=ROW_COUNT();
SET @doc125_id=IF(@doc125_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO imp01_campos_plantilla
 (IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,
  REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER)
SELECT @doc125_id,c.nombre,c.nombre,'VARCHAR(250)',0,0,0,'',0
FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c WHERE @doc125_alta=1;
SET @doc125_campos=ROW_COUNT();
UPDATE imp01_plantillaimp SET ESTADO_PLANTILLA=1 WHERE ID_PLANTILLAIMP=@doc125_id
 AND ESTADO_PLANTILLA=0 AND REVISION_CARGA=1 AND @doc125_alta=1 AND @doc125_campos=2;
SET @doc125_activada=ROW_COUNT();
SET @doc125_plantilla_crear=IF(@doc125_alta=1 AND @doc125_campos=2 AND @doc125_activada=1,@doc125_id,NULL);

-- Plantilla agregar; CREAR permanece como intencion sin tabla fisica.
INSERT INTO imp01_plantillaimp
 (IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,RUTA_ARCHIVO_FUENTE,
  ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA)
SELECT @doc125_conexion,'imp_02_doc125_e2e_agregar','12.0 xlsx','Datos',CONCAT('web/',REPLACE(UUID(),'-',''),'.xlsx'),
 0,0,1,0,'CLAVE',1 FROM DUAL WHERE @doc125_apta=1;
SET @doc125_alta=ROW_COUNT();
SET @doc125_id=IF(@doc125_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO imp01_campos_plantilla
 (IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,
  REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER)
SELECT @doc125_id,c.nombre,c.nombre,'VARCHAR(250)',0,0,0,'',0
FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c WHERE @doc125_alta=1;
SET @doc125_campos=ROW_COUNT();
UPDATE imp01_plantillaimp SET ESTADO_PLANTILLA=1 WHERE ID_PLANTILLAIMP=@doc125_id
 AND ESTADO_PLANTILLA=0 AND REVISION_CARGA=1 AND @doc125_alta=1 AND @doc125_campos=2;
SET @doc125_activada=ROW_COUNT();
SET @doc125_plantilla_agregar=IF(@doc125_alta=1 AND @doc125_campos=2 AND @doc125_activada=1,@doc125_id,NULL);

-- Plantilla reemplazar; CREAR permanece como intencion sin tabla fisica.
INSERT INTO imp01_plantillaimp
 (IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,RUTA_ARCHIVO_FUENTE,
  ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA)
SELECT @doc125_conexion,'imp_02_doc125_e2e_reemplazar','12.0 xlsx','Datos',CONCAT('web/',REPLACE(UUID(),'-',''),'.xlsx'),
 0,1,0,0,'CLAVE',1 FROM DUAL WHERE @doc125_apta=1;
SET @doc125_alta=ROW_COUNT();
SET @doc125_id=IF(@doc125_alta=1,LAST_INSERT_ID(),NULL);
INSERT INTO imp01_campos_plantilla
 (IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,
  REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER)
SELECT @doc125_id,c.nombre,c.nombre,'VARCHAR(250)',0,0,0,'',0
FROM (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c WHERE @doc125_alta=1;
SET @doc125_campos=ROW_COUNT();
UPDATE imp01_plantillaimp SET ESTADO_PLANTILLA=1 WHERE ID_PLANTILLAIMP=@doc125_id
 AND ESTADO_PLANTILLA=0 AND REVISION_CARGA=1 AND @doc125_alta=1 AND @doc125_campos=2;
SET @doc125_activada=ROW_COUNT();
SET @doc125_plantilla_reemplazar=IF(@doc125_alta=1 AND @doc125_campos=2 AND @doc125_activada=1,@doc125_id,NULL);

SET @doc125_listo=(@doc125_plantilla_crear IS NOT NULL AND @doc125_plantilla_agregar IS NOT NULL AND @doc125_plantilla_reemplazar IS NOT NULL);
INSERT INTO imp_02_doc125_e2e_agregar(CLAVE,VALOR)
SELECT 'DOC125_BASE_AG','DOC125_BASE_VALOR_AG' FROM DUAL WHERE @doc125_listo=1;
SET @doc125_base_ag=ROW_COUNT();
INSERT INTO imp_02_doc125_e2e_reemplazar(CLAVE,VALOR)
SELECT c.clave,c.valor FROM (SELECT 'DOC125_BASE_RE_1' AS clave,'DOC125_BASE_VALOR_RE_1' AS valor
 UNION ALL SELECT 'DOC125_BASE_RE_2','DOC125_BASE_VALOR_RE_2') c WHERE @doc125_listo=1;
SET @doc125_base_re=ROW_COUNT();
SELECT IF(@doc125_listo=1 AND @doc125_base_ag=1 AND @doc125_base_re=2,
 'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
 @doc125_plantilla_crear AS plantilla_crear,@doc125_plantilla_agregar AS plantilla_agregar,
 @doc125_plantilla_reemplazar AS plantilla_reemplazar;
-- SOLO ante LISTO_PARA_COMMIT y cero errores: COMMIT manual en la misma sesion.
-- Otro resultado o error: ROLLBACK manual. No hay COMMIT automatico.
SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,CAMPO_UNICO,REVISION_CARGA,ESTADO_PLANTILLA,
 ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD FROM imp01_plantillaimp
WHERE ID_PLANTILLAIMP IN (@doc125_plantilla_crear,@doc125_plantilla_agregar,@doc125_plantilla_reemplazar)
ORDER BY ID_PLANTILLAIMP;

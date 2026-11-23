-- DOC124. APLICACION MANUAL en DA del modulo11, referencia6 verificada.
-- Una plantilla nueva para la tabla EXISTENTE; modo AGREGAR, cuatro campos.
-- No crea ni modifica imp_02_contratos, no importa filas ni cambia consumidores.
-- Requiere SQL15 confirmado/verificado en RA y preflight de identidad de conexion.
-- Ejecutar sin continuar ante errores. Ante error: ROLLBACK y detener.

START TRANSACTION;
SET @doc124_plantilla_apta=(
 (SELECT COUNT(*) FROM imp01_xmlconexion WHERE ID_XMLCONEXION=6
    AND LOWER(DBMS_BD)='mysql' AND BINARY NOMBRE_DB_DEFAULT=BINARY DATABASE())=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES
      WHERE TABLE_SCHEMA=DATABASE() AND ENGINE='InnoDB'
        AND TABLE_TYPE='BASE TABLE'
        AND TABLE_NAME IN ('imp01_plantillaimp','imp01_campos_plantilla','imp_02_contratos'))=3
 AND (SELECT COUNT(*) FROM imp01_plantillaimp
      WHERE LOWER(NOMBRE_PLANTILLA) IN ('contratos','imp_02_contratos'))=0
 AND (SELECT COUNT(*) FROM information_schema.COLUMNS
      WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_contratos')=4
 AND (SELECT COUNT(*) FROM information_schema.COLUMNS
      WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_contratos'
        AND COLUMN_NAME IN ('FECHA','NCONTRATO','NIT','NOMBRE')
        AND COLUMN_TYPE='varchar(250)' AND CHARACTER_SET_NAME='latin1'
        AND COLLATION_NAME='latin1_swedish_ci' AND EXTRA=''
        AND (COLUMN_NAME<>'NCONTRATO' OR IS_NULLABLE='NO'))=4
 AND EXISTS(SELECT INDEX_NAME FROM information_schema.STATISTICS
            WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_contratos'
            GROUP BY INDEX_NAME HAVING COUNT(*)=1 AND MIN(NON_UNIQUE)=0
              AND MIN(COLUMN_NAME)='NCONTRATO' AND SUM(SUB_PART IS NOT NULL)=0)
);
INSERT INTO imp01_plantillaimp
 (IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,
  RUTA_ARCHIVO_FUENTE,ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,
  ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA)
SELECT 6,'imp_02_contratos','12.0 xlsx','Hoja1',
       CONCAT('web/',REPLACE(UUID(),'-',''),'.xlsx'),0,0,1,0,'NCONTRATO',1
FROM DUAL WHERE @doc124_plantilla_apta=1;
SET @doc124_plantilla_alta=ROW_COUNT();
SET @doc124_plantilla_id=IF(@doc124_plantilla_alta=1,LAST_INSERT_ID(),NULL);

-- FECHA es VARCHAR(250) fisico; conservar texto, no convertir a DATE.
INSERT INTO imp01_campos_plantilla
 (IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,
  REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER)
SELECT @doc124_plantilla_id,c.nombre,c.nombre,'VARCHAR(250)',0,0,0,'',0
FROM (SELECT 'FECHA' AS nombre UNION ALL SELECT 'NCONTRATO'
      UNION ALL SELECT 'NIT' UNION ALL SELECT 'NOMBRE') c
WHERE @doc124_plantilla_alta=1;
SET @doc124_plantilla_campos=ROW_COUNT();
UPDATE imp01_plantillaimp SET ESTADO_PLANTILLA=1
WHERE ID_PLANTILLAIMP=@doc124_plantilla_id AND ESTADO_PLANTILLA=0
  AND REVISION_CARGA=1 AND @doc124_plantilla_alta=1 AND @doc124_plantilla_campos=4;
SET @doc124_plantilla_activada=ROW_COUNT();

SELECT IF(@doc124_plantilla_alta=1 AND @doc124_plantilla_campos=4
          AND @doc124_plantilla_activada=1,'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
       @doc124_plantilla_id AS plantilla_id;
-- SOLO si LISTO_PARA_COMMIT y no hubo errores: ejecutar manualmente COMMIT
-- en esta misma sesion. Cualquier otro resultado: ROLLBACK y revisar.
-- Si ya existe una plantilla, no la editar ni repetir esta alta automaticamente.

SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,CAMPO_UNICO,REVISION_CARGA,
       ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA
FROM imp01_plantillaimp WHERE ID_PLANTILLAIMP=@doc124_plantilla_id;

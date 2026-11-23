-- DOC-124. PLANTILLA DEL ARCHIVO RELACION ORDENES 2025.
-- Creada por el agente a solicitud expresa del usuario mediante el repositorio
-- existente: ID27, conexion6, revision1, siete campos. NO repetir el alta.
-- Este archivo documenta el alta equivalente y permite aplicarla manualmente
-- en un ambiente compatible donde aun no exista. No ejecuta DDL ni importa filas.
-- Ejecutar en DA del modulo; revisar conexion6 antes. No cambia EGRESOS/CREDITOS.
-- El libro compatible conserva Hoja1 y mueve encabezados de fila2 a fila1 en una
-- COPIA; el original tiene una fila inicial de titulo, no admitida como encabezado.
-- El limite20000 se configura en ImportacionExcel.MaxFilasDatos, no por SQL.

SELECT ID_XMLCONEXION,NOMBRE_CONEXION,DBMS_BD
FROM imp01_xmlconexion WHERE ID_XMLCONEXION=6;

SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,CAMPO_UNICO,REVISION_CARGA,
       IMP01_XMLCONEXION_ID_XMLCONEXION,ESTADO_PLANTILLA
FROM imp01_plantillaimp WHERE BINARY NOMBRE_PLANTILLA=BINARY 'imp_02_ordenes_2025';

START TRANSACTION;
SET @doc124_ordenes_existe=(SELECT COUNT(*) FROM imp01_plantillaimp
                           WHERE BINARY NOMBRE_PLANTILLA=BINARY 'imp_02_ordenes_2025');
SET @doc124_ordenes_conexion=(SELECT COUNT(*) FROM imp01_xmlconexion
                             WHERE ID_XMLCONEXION=6 AND LOWER(DBMS_BD)='mysql'
                               AND BINARY NOMBRE_DB_DEFAULT=BINARY DATABASE());
SET @doc124_ordenes_tabla_existe=(SELECT COUNT(*) FROM information_schema.TABLES
                                WHERE TABLE_SCHEMA=DATABASE()
                                  AND TABLE_NAME='imp_02_ordenes_2025');
INSERT INTO imp01_plantillaimp
 (IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,
  RUTA_ARCHIVO_FUENTE,ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,
  ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA)
SELECT 6,'imp_02_ordenes_2025','12.0 xlsx','Hoja1',
       CONCAT('web/',REPLACE(UUID(),'-',''),'.xlsx'),0,0,1,1,'NROORDEN',1
FROM DUAL
WHERE @doc124_ordenes_conexion=1 AND @doc124_ordenes_existe=0
  AND @doc124_ordenes_tabla_existe=0;

SET @doc124_ordenes_creada=ROW_COUNT();
SET @doc124_ordenes_id=IF(@doc124_ordenes_creada=1,LAST_INSERT_ID(),NULL);

INSERT INTO imp01_campos_plantilla
 (IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,
  REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER)
SELECT @doc124_ordenes_id,m.fuente,m.destino,m.tipo,0,0,0,'',0
FROM (
 SELECT 'NOMBRE' AS fuente,'NOMBRE' AS destino,'VARCHAR(128)' AS tipo
 UNION ALL SELECT 'NIT','NIT','VARCHAR(32)'
 UNION ALL SELECT 'NROORDEN','NROORDEN','VARCHAR(32)'
 UNION ALL SELECT 'FECHARECEP','FECHARECEP','DATE'
 UNION ALL SELECT 'FECHAENTREGA','FECHAENTREGA','DATE'
 UNION ALL SELECT 'VIN','VIN','VARCHAR(32)'
 UNION ALL SELECT 'PLACA','PLACA','VARCHAR(16)'
) m WHERE @doc124_ordenes_creada=1;
COMMIT;

SELECT p.ID_PLANTILLAIMP,p.IMP01_XMLCONEXION_ID_XMLCONEXION,p.NOMBRE_PLANTILLA,
       p.CAMPO_UNICO,p.REVISION_CARGA,p.ESTADO_PLANTILLA,
       c.CAMPO_FUENTE,c.CAMPO_DESTINO,c.TIPO_CAMPO
FROM imp01_plantillaimp p INNER JOIN imp01_campos_plantilla c
  ON c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=p.ID_PLANTILLAIMP
WHERE BINARY p.NOMBRE_PLANTILLA=BINARY 'imp_02_ordenes_2025'
ORDER BY c.ID_CAMPOS_PLANTILLA;

-- Debe continuar ausente: crear la plantilla no crea la tabla de negocio.
SELECT COUNT(*) AS tabla_fisica_existente FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND BINARY TABLE_NAME=BINARY 'imp_02_ordenes_2025';

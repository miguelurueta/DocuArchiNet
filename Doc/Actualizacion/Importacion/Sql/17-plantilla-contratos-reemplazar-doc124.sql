-- DOC124. APLICACION MANUAL en DA, SOLO DESPUES del PASS real AGREGAR.
-- Cambia la UNICA plantilla creada por16 de AGREGAR/revision1 a REEMPLAZAR/revision2.
-- No ejecutar junto con15/16. Antes, cerrar la ventana AGREGAR y sus sesiones.
-- No sustituye datos de CONTRATOS: cambia configuracion de la siguiente prueba.
-- Las preparaciones antiguas conservaran su snapshot; no se reutilizan sus IDs.
-- Completar con el ID devuelto por16 y usado en el PASS AGREGAR; NULL no modifica.

SET @doc124_plantilla_esperada=NULL;
START TRANSACTION;
SET @doc124_modo_id=(SELECT IF(COUNT(*)=1,MIN(ID_PLANTILLAIMP),NULL)
 FROM imp01_plantillaimp WHERE LOWER(NOMBRE_PLANTILLA) IN ('contratos','imp_02_contratos'));
SET @doc124_modo_campos=(SELECT COUNT(*) FROM imp01_campos_plantilla
 WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=@doc124_modo_id
   AND BINARY CAMPO_FUENTE=BINARY CAMPO_DESTINO
   AND CAMPO_DESTINO IN ('FECHA','NCONTRATO','NIT','NOMBRE')
   AND BINARY TIPO_CAMPO=BINARY 'VARCHAR(250)'
   AND REMPLAZA_COMA=0 AND REMPLAZA_PUNTO=0 AND OPCION_SPLITER=0
   AND CARACTER_SPLITER='' AND NUMERO_MATRIZ_EXPLITER=0);

UPDATE imp01_plantillaimp
SET ELIMINA_INSERTA_RECORD=1,INSERTA_RECORD=0,REVISION_CARGA=2
WHERE ID_PLANTILLAIMP=@doc124_modo_id AND ID_PLANTILLAIMP=@doc124_plantilla_esperada
  AND IMP01_XMLCONEXION_ID_XMLCONEXION=6
  AND BINARY NOMBRE_PLANTILLA=BINARY 'imp_02_contratos'
  AND BINARY CAMPO_UNICO=BINARY 'NCONTRATO'
  AND BINARY LIBRO_EXCEL=BINARY 'Hoja1' AND CONEXION_FUENTE='12.0 xlsx'
  AND REVISION_CARGA=1 AND ESTADO_PLANTILLA=1
  AND ELIMINA_TABLA=0 AND ELIMINA_INSERTA_RECORD=0 AND INSERTA_RECORD=1
  AND @doc124_modo_campos=4
  AND (SELECT COUNT(*) FROM imp01_campos_plantilla
       WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=@doc124_modo_id)=4
  AND (SELECT COUNT(DISTINCT CAMPO_DESTINO) FROM imp01_campos_plantilla
       WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=@doc124_modo_id)=4;
SET @doc124_modo_cambiado=ROW_COUNT();
SELECT IF(@doc124_modo_cambiado=1,'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
       @doc124_modo_id AS plantilla_id;
-- SOLO si LISTO_PARA_COMMIT y no hubo errores: COMMIT manual en esta sesion.
-- Otro resultado o error: ROLLBACK. No fuerza revisiones ni modifica mapeos.
-- Tras confirmar, releer plantilla y huella con el repositorio existente;
-- la ventana REEMPLAZAR requiere un scope nuevo con revision2 y hash nuevo.

SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,REVISION_CARGA,
       ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD
FROM imp01_plantillaimp WHERE ID_PLANTILLAIMP=@doc124_modo_id;

-- DOC127 SQL32 | DA | APLICACION MANUAL | ENTREGADO, NO APLICADO POR EL AGENTE.
-- MySQL5.1.37. Reutiliza conexion6 y contrato58/59; conserva sus registros.
-- Requiere SQL30 en DA y SQL31 en RA aplicados/confirmados previamente.
-- Efecto maximo: 8 cabeceras nuevas +16 campos; activa SOLO esas8 cabeceras.
-- Cero CREATE/ALTER/DROP, cambios de permisos, proveedores o filas de negocio.
-- NOMBRE_PLANTILLA es UNIQUE: XLS usa dos tablas DOC127 propias, no duplica58/59.
-- No editar plantillas57/58/59/60. Los seis destinos CREAR quedan AUSENTES.
-- Precondicion: ventana administrativa sin DDL u otras altas concurrentes;
-- revisar28 y confirmar politicas RA CONEXION de las dos tablas XLS de SQL30/31.
-- Ejecutar completo, sin continuar ante errores, en UNA sesion DA dedicada.
-- Completar NULL/0 en una COPIA LOCAL despues de confirmar empresa2/DA11/ref6.
SET @doc127_empresa_confirmada=NULL;
SET @doc127_modulo_confirmado=NULL;
SET @doc127_conexion=NULL;
SET @doc127_preflight_confirmado=0;
SET @doc127_lote=REPLACE(UUID(),'-','');
SET @doc127_ruta_prefijo=CONCAT('web/',@doc127_lote,'_');

-- SERIALIZABLE solo para la proxima transaccion. InnoDB protege rangos de
-- catalogo mientras el operador revisa; COMMIT/ROLLBACK libera los bloqueos.
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
START TRANSACTION;
SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,CONEXION_FUENTE,REVISION_CARGA
FROM imp01_plantillaimp
WHERE IMP01_XMLCONEXION_ID_XMLCONEXION=@doc127_conexion
  AND NOMBRE_PLANTILLA IN ('imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar',
    'imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls',
    'imp_02_doc127_e2e_crear_xls_1000','imp_02_doc127_e2e_crear_xls_5000',
    'imp_02_doc127_e2e_crear_xls_10000','imp_02_doc127_e2e_crear_xlsx_1000',
    'imp_02_doc127_e2e_crear_xlsx_5000','imp_02_doc127_e2e_crear_xlsx_10000')
ORDER BY ID_PLANTILLAIMP FOR UPDATE;
SELECT ID_CAMPOS_PLANTILLA,IMP01_PLANTILLAIMP_ID_PLANTILLAIMP
FROM imp01_campos_plantilla
WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP IN (58,59)
ORDER BY IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,ID_CAMPOS_PLANTILLA FOR UPDATE;

SET @doc127_apta=(
 @doc127_empresa_confirmada=2 AND @doc127_modulo_confirmado=11
 AND @doc127_conexion=6 AND @doc127_preflight_confirmado=1
 AND (SELECT COUNT(*) FROM imp01_xmlconexion WHERE ID_XMLCONEXION=@doc127_conexion
      AND LOWER(DBMS_BD)='mysql' AND BINARY NOMBRE_DB_DEFAULT=BINARY DATABASE())=1
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND ENGINE='InnoDB' AND TABLE_TYPE='BASE TABLE' AND TABLE_NAME IN
      ('imp01_plantillaimp','imp01_campos_plantilla',
       'imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar',
       'imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls'))=6
 AND (SELECT COUNT(*) FROM imp_02_doc127_e2e_agregar_xls)=0
 AND (SELECT COUNT(*) FROM imp_02_doc127_e2e_reemplazar_xls)=0
 AND (SELECT COUNT(*) FROM imp01_plantillaimp
      WHERE IMP01_XMLCONEXION_ID_XMLCONEXION=@doc127_conexion
      AND CONEXION_FUENTE='12.0 xlsx' AND LIBRO_EXCEL='Datos'
      AND ESTADO_PLANTILLA=1 AND REVISION_CARGA=1 AND CAMPO_UNICO='CLAVE'
      AND ELIMINA_TABLA=0 AND
      ((ID_PLANTILLAIMP=58 AND NOMBRE_PLANTILLA='imp_02_doc126_e2e_agregar'
        AND ELIMINA_INSERTA_RECORD=0 AND INSERTA_RECORD=1)
       OR (ID_PLANTILLAIMP=59 AND NOMBRE_PLANTILLA='imp_02_doc126_e2e_reemplazar'
        AND ELIMINA_INSERTA_RECORD=1 AND INSERTA_RECORD=0)))=2
 AND (SELECT COUNT(*) FROM imp01_campos_plantilla
      WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP IN (58,59))=4
 AND (SELECT COUNT(DISTINCT IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_DESTINO)
      FROM imp01_campos_plantilla
      WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP IN (58,59)
      AND CAMPO_FUENTE=CAMPO_DESTINO AND CAMPO_DESTINO IN ('CLAVE','VALOR')
      AND TIPO_CAMPO='VARCHAR(250)' AND REMPLAZA_COMA=0 AND REMPLAZA_PUNTO=0
      AND OPCION_SPLITER=0 AND NUMERO_MATRIZ_EXPLITER=0)=4
 AND (SELECT COUNT(*) FROM imp01_plantillaimp
      WHERE NOMBRE_PLANTILLA IN
      ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls'))=0
 AND (SELECT COUNT(*) FROM imp01_plantillaimp WHERE NOMBRE_PLANTILLA IN
      ('imp_02_doc127_e2e_crear_xls_1000','imp_02_doc127_e2e_crear_xls_5000',
       'imp_02_doc127_e2e_crear_xls_10000','imp_02_doc127_e2e_crear_xlsx_1000',
       'imp_02_doc127_e2e_crear_xlsx_5000','imp_02_doc127_e2e_crear_xlsx_10000'))=0
 AND (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
      AND TABLE_NAME IN
      ('imp_02_doc127_e2e_crear_xls_1000','imp_02_doc127_e2e_crear_xls_5000',
       'imp_02_doc127_e2e_crear_xls_10000','imp_02_doc127_e2e_crear_xlsx_1000',
       'imp_02_doc127_e2e_crear_xlsx_5000','imp_02_doc127_e2e_crear_xlsx_10000'))=0
 AND (SELECT COUNT(*) FROM imp01_plantillaimp
      WHERE LEFT(RUTA_ARCHIVO_FUENTE,CHAR_LENGTH(@doc127_ruta_prefijo))=@doc127_ruta_prefijo)=0
);

INSERT INTO imp01_plantillaimp
 (IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,
  RUTA_ARCHIVO_FUENTE,ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,
  ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA)
SELECT @doc127_conexion,p.tabla,p.formato,'Datos',
 CONCAT(@doc127_ruta_prefijo,p.n,IF(p.formato='8.0 xls','.xls','.xlsx')),
 0,p.reemplazar,1-p.reemplazar,0,'CLAVE',1
FROM (
 SELECT 1 AS n,'imp_02_doc127_e2e_agregar_xls' AS tabla,'8.0 xls' AS formato,0 AS reemplazar
 UNION ALL SELECT 2,'imp_02_doc127_e2e_reemplazar_xls','8.0 xls',1
 UNION ALL SELECT 3,'imp_02_doc127_e2e_crear_xls_1000','8.0 xls',0
 UNION ALL SELECT 4,'imp_02_doc127_e2e_crear_xls_5000','8.0 xls',0
 UNION ALL SELECT 5,'imp_02_doc127_e2e_crear_xls_10000','8.0 xls',0
 UNION ALL SELECT 6,'imp_02_doc127_e2e_crear_xlsx_1000','12.0 xlsx',0
 UNION ALL SELECT 7,'imp_02_doc127_e2e_crear_xlsx_5000','12.0 xlsx',0
 UNION ALL SELECT 8,'imp_02_doc127_e2e_crear_xlsx_10000','12.0 xlsx',0
) p WHERE @doc127_apta=1;
SET @doc127_cabeceras=ROW_COUNT(),@doc127_avisos_cabeceras=@@warning_count;

INSERT INTO imp01_campos_plantilla
 (IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,
  REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER)
SELECT p.ID_PLANTILLAIMP,c.nombre,c.nombre,'VARCHAR(250)',0,0,0,'',0
FROM imp01_plantillaimp p
CROSS JOIN (SELECT 'CLAVE' AS nombre UNION ALL SELECT 'VALOR') c
WHERE @doc127_apta=1 AND @doc127_cabeceras=8 AND @doc127_avisos_cabeceras=0
 AND p.IMP01_XMLCONEXION_ID_XMLCONEXION=@doc127_conexion
 AND LEFT(p.RUTA_ARCHIVO_FUENTE,CHAR_LENGTH(@doc127_ruta_prefijo))=@doc127_ruta_prefijo
 AND p.ESTADO_PLANTILLA=0 AND p.REVISION_CARGA=1;
SET @doc127_campos=ROW_COUNT(),@doc127_avisos_campos=@@warning_count;

UPDATE imp01_plantillaimp SET ESTADO_PLANTILLA=1
WHERE @doc127_apta=1 AND @doc127_cabeceras=8 AND @doc127_campos=16
 AND @doc127_avisos_cabeceras=0 AND @doc127_avisos_campos=0
 AND IMP01_XMLCONEXION_ID_XMLCONEXION=@doc127_conexion
 AND LEFT(RUTA_ARCHIVO_FUENTE,CHAR_LENGTH(@doc127_ruta_prefijo))=@doc127_ruta_prefijo
 AND ESTADO_PLANTILLA=0 AND REVISION_CARGA=1;
SET @doc127_activadas=ROW_COUNT(),@doc127_avisos_activadas=@@warning_count;

SELECT IF(@doc127_apta=1 AND @doc127_cabeceras=8 AND @doc127_campos=16
 AND @doc127_activadas=8 AND @doc127_avisos_cabeceras=0
 AND @doc127_avisos_campos=0 AND @doc127_avisos_activadas=0,
 'LISTO_PARA_COMMIT','REVISAR_Y_ROLLBACK') AS resultado,
 @doc127_cabeceras AS cabeceras,@doc127_campos AS campos,@doc127_activadas AS activadas;
SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,CONEXION_FUENTE,CAMPO_UNICO,
 REVISION_CARGA,ESTADO_PLANTILLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD
FROM imp01_plantillaimp
WHERE IMP01_XMLCONEXION_ID_XMLCONEXION=@doc127_conexion
 AND LEFT(RUTA_ARCHIVO_FUENTE,CHAR_LENGTH(@doc127_ruta_prefijo))=@doc127_ruta_prefijo
ORDER BY ID_PLANTILLAIMP;

-- NO HAY COMMIT AUTOMATICO.
-- Solo si LISTO_PARA_COMMIT, 8/16/8 y CERO errores/avisos: COMMIT manual
-- en ESTA MISMA sesion. Otro resultado o error: ROLLBACK manual y revisar.
-- Conservar los8 IDs y confirmar aplicacion/COMMIT. Despues, agente verifica
-- desde otra conexion mediante SELECT. No repetir este archivo si ya se aplico.
-- Estas altas no autorizan E2E ni crean las seis tablas fisicas de CREAR.

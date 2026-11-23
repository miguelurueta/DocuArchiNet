-- DOC-124. CONTRATOS existente: precondiciones para AGREGAR/REEMPLAZAR.
-- SOLO SELECT. Ejecutar manualmente por bloques RA/DA; no aplica configuracion.
-- No demuestra pertenencia empresarial: la confirma el responsable de los datos.
-- No repetir el ALTER del SQL08 ni activar fuentes para conseguir un PASS.

-- RA: politica propia y posibles politicas contradictorias de otros ambitos.
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,
       columna_empresa,activo,revision
FROM ra_integracion_fuente
WHERE BINARY conexion_ref=BINARY 'DA'
  AND BINARY tabla_nombre=BINARY 'imp_02_contratos'
ORDER BY ambito_ref,fuente_id;

SELECT c.fuente_id,c.nombre
FROM ra_integracion_fuente_columna c
INNER JOIN ra_integracion_fuente f ON f.fuente_id=c.fuente_id
WHERE BINARY f.conexion_ref=BINARY 'DA'
  AND BINARY f.tabla_nombre=BINARY 'imp_02_contratos'
ORDER BY c.fuente_id,c.nombre;

-- DA: tabla existente; nunca crear otra tabla como fixture.
SELECT TABLE_NAME,ENGINE,TABLE_TYPE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE BINARY TABLE_SCHEMA=BINARY DATABASE()
  AND BINARY TABLE_NAME=BINARY 'imp_02_contratos';

SELECT COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT,
       CHARACTER_SET_NAME,COLLATION_NAME,EXTRA
FROM information_schema.COLUMNS
WHERE BINARY TABLE_SCHEMA=BINARY DATABASE()
  AND BINARY TABLE_NAME=BINARY 'imp_02_contratos'
ORDER BY ORDINAL_POSITION;

SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE BINARY TABLE_SCHEMA=BINARY DATABASE()
  AND BINARY TABLE_NAME=BINARY 'imp_02_contratos'
ORDER BY INDEX_NAME,SEQ_IN_INDEX;

-- Solo conteos: ninguna clave ni nombre de tercero en la evidencia publica.
SELECT COUNT(*) AS filas,
       COALESCE(SUM(NCONTRATO IS NULL),0) AS claves_nulas,
       COALESCE(SUM(NCONTRATO IS NOT NULL AND TRIM(NCONTRATO)=''),0) AS claves_vacias
FROM imp_02_contratos;

SELECT COUNT(*) AS grupos_duplicados
FROM (SELECT NCONTRATO FROM imp_02_contratos
      WHERE NCONTRATO IS NOT NULL GROUP BY NCONTRATO HAVING COUNT(*)>1) d;

-- La referencia historica no autoriza cambiar el destino del modulo.
-- Comparar en privado servidor/base con los recursos vigentes; no imprimir secretos.
SELECT ID_XMLCONEXION,NOMBRE_CONEXION,DBMS_BD,
       BINARY NOMBRE_DB_DEFAULT=BINARY DATABASE() AS misma_base_actual
FROM imp01_xmlconexion WHERE ID_XMLCONEXION=6;

SELECT ID_PLANTILLAIMP,IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,
       CAMPO_UNICO,REVISION_CARGA,LIBRO_EXCEL,CONEXION_FUENTE,
       ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA
FROM imp01_plantillaimp
WHERE LOWER(NOMBRE_PLANTILLA) IN ('contratos','imp_02_contratos')
ORDER BY ID_PLANTILLAIMP;

SELECT c.ID_CAMPOS_PLANTILLA,c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,
       c.CAMPO_FUENTE,c.CAMPO_DESTINO,c.TIPO_CAMPO,
       c.REMPLAZA_COMA,c.REMPLAZA_PUNTO,c.OPCION_SPLITER,
       c.CARACTER_SPLITER,c.NUMERO_MATRIZ_EXPLITER
FROM imp01_campos_plantilla c
INNER JOIN imp01_plantillaimp p
        ON p.ID_PLANTILLAIMP=c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP
WHERE LOWER(p.NOMBRE_PLANTILLA) IN ('contratos','imp_02_contratos')
ORDER BY c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,c.ID_CAMPOS_PLANTILLA;

-- Revisar identidad/unicidad del catalogo antes de proponer altas de plantillas.
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE BINARY TABLE_SCHEMA=BINARY DATABASE()
  AND TABLE_NAME IN ('imp01_plantillaimp','imp01_campos_plantilla')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;

-- Resultado esperado antes de probar un exito:
-- * InnoDB, NCONTRATO NOT NULL, UNIQUE completo, sin reparacion de negocio.
-- * Politica activa explicita para la empresa autorizada, sin contradiccion.
--   No inferir CONEXION porque no exista una columna de empresa.
-- * Plantilla activa de imp_02_contratos, conexion validada, clave NCONTRATO.
-- * Campos FECHA/NCONTRATO/NIT/NOMBRE VARCHAR(250), no convertir FECHA a DATE.
-- * AGREGAR: flags 0/0/1; REEMPLAZAR: flags 0/1/0; revision/huella nuevas
--   si se cambia el modo. No cambiar flags desde Preparar ni desde la E2E.
-- Si falta configuracion, generar un SQL especifico separado para aplicacion
-- manual despues de confirmar aislamiento. Este archivo no habilita el destino.

-- DOC127 SQL33 | SOLO SELECT | DESPUES de confirmar SQL30/31/32 y sus COMMIT.
-- Los bloques RA/DA se ejecutan POR SEPARADO en el destino indicado.
-- No ejecutar SQL30/31/32 otra vez para comprobar su aplicacion.

-- A. DA de empresa2/modulo11, referencia6, comprobadas previamente.
SELECT 'DOC127_POST_RECURSOS_DA' AS control,DATABASE() AS base_actual,VERSION() AS version;
-- Esperadas: 8 nuevas (2 XLS sobre tablas DOC127 propias y6 CREAR) y4 anteriores.
-- Conservar IDs/atributos anteriores; cada nueva tiene revision1 y estado1.
SELECT ID_PLANTILLAIMP,IMP01_XMLCONEXION_ID_XMLCONEXION AS conexion_id,
 NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,CAMPO_UNICO,
 REVISION_CARGA,ESTADO_PLANTILLA,ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD
FROM imp01_plantillaimp
WHERE ID_PLANTILLAIMP IN (57,58,59,60)
 OR (IMP01_XMLCONEXION_ID_XMLCONEXION=6 AND
    (NOMBRE_PLANTILLA IN
     ('imp_02_doc127_e2e_crear_xls_1000','imp_02_doc127_e2e_crear_xls_5000',
      'imp_02_doc127_e2e_crear_xls_10000','imp_02_doc127_e2e_crear_xlsx_1000',
      'imp_02_doc127_e2e_crear_xlsx_5000','imp_02_doc127_e2e_crear_xlsx_10000')
     OR (NOMBRE_PLANTILLA IN ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
         AND CONEXION_FUENTE='8.0 xls')))
ORDER BY ID_PLANTILLAIMP;

-- Exactamente2 campos CLAVE/VALOR por plantilla nueva, VARCHAR(250), No/No/No/0.
-- Comparar los campos anteriores con su corte; no convertir ni reparar sus datos.
SELECT p.ID_PLANTILLAIMP,p.NOMBRE_PLANTILLA,p.CONEXION_FUENTE,
 c.ID_CAMPOS_PLANTILLA,c.CAMPO_FUENTE,c.CAMPO_DESTINO,c.TIPO_CAMPO,
 c.REMPLAZA_COMA,c.REMPLAZA_PUNTO,c.OPCION_SPLITER,c.CARACTER_SPLITER,
 c.NUMERO_MATRIZ_EXPLITER
FROM imp01_plantillaimp p JOIN imp01_campos_plantilla c
 ON c.IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=p.ID_PLANTILLAIMP
WHERE p.ID_PLANTILLAIMP IN (57,58,59,60)
 OR (p.IMP01_XMLCONEXION_ID_XMLCONEXION=6 AND
    (p.NOMBRE_PLANTILLA IN
     ('imp_02_doc127_e2e_crear_xls_1000','imp_02_doc127_e2e_crear_xls_5000',
      'imp_02_doc127_e2e_crear_xls_10000','imp_02_doc127_e2e_crear_xlsx_1000',
      'imp_02_doc127_e2e_crear_xlsx_5000','imp_02_doc127_e2e_crear_xlsx_10000')
     OR (p.NOMBRE_PLANTILLA IN ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
         AND p.CONEXION_FUENTE='8.0 xls')))
ORDER BY p.ID_PLANTILLAIMP,c.ID_CAMPOS_PLANTILLA;

-- Debe devolver CERO filas antes de las pruebas funcionales CREAR.
SELECT TABLE_NAME,ENGINE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp_02_doc127_e2e_crear_xls_1000','imp_02_doc127_e2e_crear_xls_5000',
  'imp_02_doc127_e2e_crear_xls_10000','imp_02_doc127_e2e_crear_xlsx_1000',
  'imp_02_doc127_e2e_crear_xlsx_5000','imp_02_doc127_e2e_crear_xlsx_10000');

-- SQL30/31/32 no escriben en estas tablas. Corte de preparacion: 1002/1000 filas.
-- Los nuevos fixtures XLS deben conservar motor, tipos e indices completos de30.
SELECT TABLE_NAME,ENGINE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls');
SELECT TABLE_NAME,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,CHARACTER_SET_NAME,COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY TABLE_NAME,ORDINAL_POSITION;
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp01_plantillaimp','imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;
SELECT 'imp_02_doc126_e2e_agregar' AS tabla,COUNT(*) AS filas
FROM imp_02_doc126_e2e_agregar
UNION ALL
SELECT 'imp_02_doc126_e2e_reemplazar',COUNT(*) FROM imp_02_doc126_e2e_reemplazar
UNION ALL
SELECT 'imp_02_doc127_e2e_agregar_xls',COUNT(*) FROM imp_02_doc127_e2e_agregar_xls
UNION ALL
SELECT 'imp_02_doc127_e2e_reemplazar_xls',COUNT(*) FROM imp_02_doc127_e2e_reemplazar_xls;

-- B. RA: dos politicas anteriores preservadas y dos nuevas XLS de empresa2.
SELECT 'DOC127_POST_RECURSOS_RA' AS control,DATABASE() AS base_actual;
SELECT fuente_id,ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,
 columna_empresa,activo,revision
FROM ra_integracion_fuente
WHERE conexion_ref='DA' AND tabla_nombre IN
 ('imp_02_doc126_e2e_agregar','imp_02_doc126_e2e_reemplazar',
  'imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY fuente_id;
SELECT f.fuente_id,c.nombre
FROM ra_integracion_fuente f JOIN ra_integracion_fuente_columna c
 ON c.fuente_id=f.fuente_id
WHERE f.conexion_ref='DA' AND f.tabla_nombre IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY f.fuente_id,c.nombre;
SELECT f.fuente_id,h.revision,h.accion,h.actor_ref,h.fecha_utc,h.estado_doc
FROM ra_integracion_fuente f JOIN ra_integracion_fuente_historial h
 ON h.fuente_id=f.fuente_id
WHERE f.conexion_ref='DA' AND f.tabla_nombre IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY f.fuente_id,h.revision;
-- Esperadas cuatro politicas CONEXION activas de empresa2. Anteriores intactas;
-- nuevas revision2, dos columnas autorizadas e historial ALTA/ACTIVAR cada una.
-- La E2E requiere ademas cuentas, huellas completas, archivos y ventana propios.

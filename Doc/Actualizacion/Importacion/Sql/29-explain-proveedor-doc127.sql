-- DOC127 | EXPLAIN PARA EJECUCION MANUAL | MySQL 5.1.37.
-- No lo ejecuta el agente ni la E2E. No contiene DDL/DML ni ANALYZE TABLE.
-- Destino: DA de conexion_ref DA, empresa2, modulo11, referencia6 confirmadas.
-- Fuente inspeccionada: proveedor1/version1 PUBLICADA y activo, tabla EGRESOS,
-- clave CONSECUTIVO/TEXTO, sin columna empresarial (politica CONEXION).
-- La tabla no se modifica. Se conserva el alias historico BENFICIARIO.
-- Antes: bloque RA de28 debe confirmar el mismo contrato/politica vigente.
-- Si difiere, no adaptar libremente ni ejecutar este plan: entregar otro archivo.
-- La clave siguiente es SINTETICA y textual, como el parametro del consumidor.
-- No representa un dato real ni acredita por si sola cardinalidad/rendimiento.

-- Primero, en DA: metadatos del predicado y sus indices completos.
SELECT COLUMN_NAME,COLUMN_TYPE,CHARACTER_SET_NAME,COLLATION_NAME,IS_NULLABLE
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_egresos'
  AND COLUMN_NAME IN ('CONSECUTIVO','NIT','BENEFICIARIO')
ORDER BY ORDINAL_POSITION;
SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_egresos'
ORDER BY INDEX_NAME,SEQ_IN_INDEX;

-- Consulta exacta generada por MySqlProveedorDatosIndices.ConsultarDatos:
-- misma proyeccion, igualdad y LIMIT2; texto parametrizado sustituido por literal.
EXPLAIN SELECT `NIT` AS `NIT`,`BENEFICIARIO` AS `BENFICIARIO`
FROM `imp_02_egresos`
WHERE `CONSECUTIVO`='DOC127_CLAVE_SINTETICA'
LIMIT 2;

-- Entregar key/possible_keys/type/rows/Extra para revision junto a28.
-- El plan estimado no mide tiempo real ni valida claves existentes/ausentes.
-- Si no hay indice utilizable, registrar la brecha. No crearlo ni limpiar datos.
-- Otros proveedores, claves numericas o aislamiento COLUMNA requieren su consulta
-- concreta con filtro de empresa; este caso no acredita esas variantes.

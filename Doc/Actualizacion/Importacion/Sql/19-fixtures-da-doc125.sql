-- DOC125 | APLICACION MANUAL EN DA | NO EJECUTADO POR EL AGENTE
-- Instala dos tablas nuevas descartables. No modifica tablas de negocio existentes.
-- Primero ejecutar SQL18 y revisar base/capacidades/permisos/ausencia de relaciones.
-- Comprobar esta consulta ANTES de iniciar el bloque DDL. Las tres deben estar ausentes.
-- Si existe cualquiera, detener y revisar: no DROP, IF NOT EXISTS ni reutilizacion ciega.
-- DDL tiene commit implicito: un ROLLBACK no deshace un CREATE ya terminado.
SELECT DATABASE() AS base_da, TABLE_NAME, ENGINE, TABLE_COMMENT
FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()
AND TABLE_NAME IN ('imp_02_doc125_e2e_agregar','imp_02_doc125_e2e_reemplazar','imp_02_doc125_e2e_crear');

CREATE TABLE imp_02_doc125_e2e_agregar (
 CLAVE VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 VALOR VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 UNIQUE KEY uq_carga_clave (CLAVE),
 UNIQUE KEY uq_doc125_valor (VALOR)
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci
COMMENT='DOC125 fixture manual; conservar efectos y resultados';

CREATE TABLE imp_02_doc125_e2e_reemplazar (
 CLAVE VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 VALOR VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 UNIQUE KEY uq_carga_clave (CLAVE),
 UNIQUE KEY uq_doc125_valor (VALOR)
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci
COMMENT='DOC125 fixture manual; conservar efectos y resultados';

-- imp_02_doc125_e2e_crear queda AUSENTE: la creara la operacion funcional autorizada.
-- El segundo UNIQUE permite probar rollback real tras DELETE por VALOR repetido.
SELECT TABLE_NAME,ENGINE,TABLE_COLLATION,TABLE_COMMENT FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
('imp_02_doc125_e2e_agregar','imp_02_doc125_e2e_reemplazar','imp_02_doc125_e2e_crear') ORDER BY TABLE_NAME;
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE()
AND TABLE_NAME IN ('imp_02_doc125_e2e_agregar','imp_02_doc125_e2e_reemplazar')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;

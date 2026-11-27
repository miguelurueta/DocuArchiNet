-- DOC127 SQL30 | DA | APLICACION MANUAL | NO APLICADO POR EL AGENTE.
-- Revisar28, confirmar empresa2/DA11/ref6 y respaldo antes de ejecutar.
-- Reutiliza el esquema ya probado de SQL23; NOMBRE_PLANTILLA es UNIQUE.
-- Crea SOLO dos tablas XLS nuevas, vacias, para preservar las plantillas DOC126.
-- Cero INSERT de datos, cambios de permisos, fuentes, plantillas o tablas previas.
-- MySQL5.1: cada DDL tiene commit implicito; ROLLBACK no revierte un CREATE.
-- Ejecutar en ventana administrativa sin otras altas/DDL concurrentes.

-- Comprobar que devuelve CERO filas ANTES de ejecutar cualquier CREATE.
-- Si existe cualquiera: detener y revisar; no repetir, borrar ni IF NOT EXISTS.
SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp01_plantillaimp'
ORDER BY INDEX_NAME,SEQ_IN_INDEX;
SELECT DATABASE() AS base_da,TABLE_NAME,ENGINE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls');

CREATE TABLE imp_02_doc127_e2e_agregar_xls (
 CLAVE VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 VALOR VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 UNIQUE KEY uq_carga_clave (CLAVE),
 UNIQUE KEY uq_doc127_valor (VALOR)
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci
COMMENT='DOC127 fixture XLS manual; conservar efectos y resultados';

CREATE TABLE imp_02_doc127_e2e_reemplazar_xls (
 CLAVE VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 VALOR VARCHAR(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
 UNIQUE KEY uq_carga_clave (CLAVE),
 UNIQUE KEY uq_doc127_valor (VALOR)
) ENGINE=InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci
COMMENT='DOC127 fixture XLS manual; conservar efectos y resultados';

SELECT TABLE_NAME,ENGINE,TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls');
SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('imp_02_doc127_e2e_agregar_xls','imp_02_doc127_e2e_reemplazar_xls')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;
-- Confirmar ambas altas. Una aplicacion parcial se registra y se corrige con
-- otro archivo, sin limpiar ni reaplicar este lote. No se crearon destinos CREAR.

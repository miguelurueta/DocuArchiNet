-- DOC-121 | DESTINO DA | GENERADOR SELECT, NO APLICA DDL
-- Casos historicos: egresos/CONSECUTIVO y contratos/NCONTRATO.
-- Produce propuestas desde el tipo/charset/collation/default/comentario ACTUALES.
-- No cambia BENEFICIARIO ni ninguna otra columna. No limpia datos.
-- Antes de copiar el ALTER a un NUEVO archivo SQL especifico:
-- 1. Confirmar tabla base InnoDB y fuente exclusiva por CONEXION.
-- 2. Ejecutar los dos controles propuestos y exigir cero incidencias.
-- 3. Revisar 00: PK/UNIQUE equivalente completo, no prefijo. Si existe, no ADD.
-- 4. Revisar longitud en bytes, defaults/comentario y escritores concurrentes.
-- 5. El usuario revisa y ejecuta manualmente el nuevo archivo especifico.
-- Fuente compartida: NO usar esta propuesta; generar otro archivo que preserve
-- columna empresarial requerida y UNIQUE(empresa,clave), segun autoridad vigente.
-- La consulta devuelve texto SQL; no PREPARE/EXECUTE ni instalacion indirecta.
SELECT c.TABLE_NAME, c.COLUMN_NAME, c.COLUMN_TYPE, c.IS_NULLABLE,
       c.CHARACTER_SET_NAME, c.COLLATION_NAME,
       'REQUIERE_REVISION_DE_DATOS_INDICES_Y_AISLAMIENTO' AS estado_propuesta,
       CONCAT('SELECT COUNT(*) AS claves_nulas_o_vacias FROM `',
         REPLACE(DATABASE(), '`', '``'), '`.`', c.TABLE_NAME,
         '` WHERE `', c.COLUMN_NAME, '` IS NULL OR TRIM(`', c.COLUMN_NAME,
         '`) = ', QUOTE(''), ';') AS control_nulos_vacios,
       CONCAT('SELECT COUNT(*) AS grupos_duplicados FROM (SELECT `',
         c.COLUMN_NAME, '` FROM `', REPLACE(DATABASE(), '`', '``'), '`.`',
         c.TABLE_NAME, '` GROUP BY `', c.COLUMN_NAME,
         '` HAVING COUNT(*) > 1) AS duplicados;') AS control_duplicados,
       CONCAT('ALTER TABLE `', REPLACE(DATABASE(), '`', '``'), '`.`',
         c.TABLE_NAME, '` MODIFY COLUMN `', c.COLUMN_NAME, '` ', c.COLUMN_TYPE,
         IF(c.CHARACTER_SET_NAME IS NULL, '', CONCAT(' CHARACTER SET ',
           c.CHARACTER_SET_NAME, ' COLLATE ', c.COLLATION_NAME)),
         ' NOT NULL',
         IF(c.COLUMN_DEFAULT IS NULL, '', CONCAT(' DEFAULT ', QUOTE(c.COLUMN_DEFAULT))),
         IF(c.COLUMN_COMMENT = '', '', CONCAT(' COMMENT ', QUOTE(c.COLUMN_COMMENT))),
         ', ADD UNIQUE INDEX `ux_doc121_', c.COLUMN_NAME, '` (`', c.COLUMN_NAME,
         '`);') AS propuesta_ddl_no_ejecutada
FROM information_schema.COLUMNS c
INNER JOIN information_schema.TABLES t
  ON t.TABLE_SCHEMA = c.TABLE_SCHEMA AND t.TABLE_NAME = c.TABLE_NAME
WHERE c.TABLE_SCHEMA = DATABASE() AND t.ENGINE = 'InnoDB'
  AND c.EXTRA = '' AND c.DATA_TYPE IN ('char', 'varchar', 'tinyint', 'smallint',
      'mediumint', 'int', 'bigint', 'decimal', 'numeric', 'date')
  AND ((c.TABLE_NAME = 'imp_02_egresos' AND c.COLUMN_NAME = 'CONSECUTIVO')
    OR (c.TABLE_NAME = 'imp_02_contratos' AND c.COLUMN_NAME = 'NCONTRATO'))
ORDER BY c.TABLE_NAME;

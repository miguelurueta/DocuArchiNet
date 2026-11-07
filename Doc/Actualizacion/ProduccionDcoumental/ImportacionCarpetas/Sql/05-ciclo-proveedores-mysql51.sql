-- DOC-110: ampliacion de SQL 04 ya instalado. MySQL 5.1 / InnoDB.
-- Aplicacion MANUAL por el usuario, UNA VEZ, en la misma base RA.
-- No registra proveedores, no concede permisos, no publica filas existentes.
-- Instalacion nueva: 01 -> 04 -> 05. Existente con 04: solo 05.
ALTER TABLE ra_integracion_proveedor
  ADD COLUMN revision INT UNSIGNED NOT NULL DEFAULT 1,
  ADD COLUMN actualizado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NULL,
  ADD COLUMN actualizado_utc DATETIME NULL;

ALTER TABLE ra_integracion_proveedor_version
  ADD COLUMN estado VARCHAR(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL DEFAULT 'BORRADOR',
  ADD COLUMN revision INT UNSIGNED NOT NULL DEFAULT 1,
  ADD COLUMN actualizado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NULL,
  ADD COLUMN actualizado_utc DATETIME NULL,
  ADD COLUMN publicado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NULL,
  ADD COLUMN publicado_utc DATETIME NULL,
  ADD COLUMN contrato_hash BINARY(32) NULL,
  ADD KEY ix_ra_proveedor_version_estado (proveedor_id,estado,numero);

-- DOC-110 AMP-02. MySQL 5.1 / InnoDB. Aplicacion MANUAL UNA VEZ en RA.
-- Requiere instalacion previa 04/05/06. No modifica DA, SII ni datos de negocio.
-- Sin registros de fuentes, asignaciones de permiso ni publicacion automatica.
CREATE TABLE ra_integracion_fuente (
  fuente_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  conexion_ref VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  tabla_nombre VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  modo_aislamiento VARCHAR(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  columna_empresa VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  activo TINYINT UNSIGNED NOT NULL DEFAULT 0,
  revision INT UNSIGNED NOT NULL DEFAULT 1,
  creado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  creado_utc DATETIME NOT NULL,
  actualizado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NULL,
  actualizado_utc DATETIME NULL,
  PRIMARY KEY (fuente_id),
  UNIQUE KEY uq_ra_fuente_identidad (ambito_ref,conexion_ref,tabla_nombre),
  KEY ix_ra_fuente_ambito_activo (ambito_ref,activo,fuente_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_fuente_columna (
  columna_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  fuente_id INT UNSIGNED NOT NULL,
  nombre VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (columna_id),
  UNIQUE KEY uq_ra_fuente_columna (fuente_id,nombre),
  CONSTRAINT fk_ra_fuente_columna FOREIGN KEY (fuente_id)
    REFERENCES ra_integracion_fuente (fuente_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_fuente_historial (
  historial_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  fuente_id INT UNSIGNED NOT NULL,
  revision INT UNSIGNED NOT NULL,
  accion VARCHAR(24) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  actor_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  fecha_utc DATETIME NOT NULL,
  motivo VARCHAR(500) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  estado_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (historial_id),
  UNIQUE KEY uq_ra_fuente_hist_revision (fuente_id,revision),
  CONSTRAINT fk_ra_fuente_historial FOREIGN KEY (fuente_id)
    REFERENCES ra_integracion_fuente (fuente_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

ALTER TABLE remit_dest_interno_perfil_produccion
  ADD COLUMN ADMINISTRAR_FUENTES_IMPORTACION TINYINT UNSIGNED NOT NULL DEFAULT 0;

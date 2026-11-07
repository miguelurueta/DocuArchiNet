-- DOC-110 AMP-01. MySQL 5.1 / InnoDB.
-- Ejecutar UNA VEZ en el origen RA donde reside ra_integracion_perfil.
-- Instalacion nueva: SQL 01, luego este archivo. Existente: solo este archivo.
-- Sin datos de negocio, credenciales, seeds ni cambios en SII.
CREATE TABLE ra_integracion_proveedor (
  proveedor_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  codigo VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  nombre VARCHAR(160) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  activo TINYINT UNSIGNED NOT NULL DEFAULT 0,
  creado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  creado_utc DATETIME NOT NULL,
  PRIMARY KEY (proveedor_id),
  UNIQUE KEY uq_ra_proveedor_ambito_codigo (ambito_ref,codigo),
  KEY ix_ra_proveedor_ambito_activo (ambito_ref,activo,proveedor_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_proveedor_version (
  version_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  proveedor_id INT UNSIGNED NOT NULL,
  numero INT UNSIGNED NOT NULL,
  tipo VARCHAR(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  conexion_ref VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  tabla_nombre VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  clave_columna VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  clave_tipo VARCHAR(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  empresa_columna VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  esquema_config SMALLINT UNSIGNED NOT NULL,
  propiedades_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  creado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  creado_utc DATETIME NOT NULL,
  PRIMARY KEY (version_id),
  UNIQUE KEY uq_ra_proveedor_version_numero (proveedor_id,numero),
  CONSTRAINT fk_ra_proveedor_version_proveedor FOREIGN KEY (proveedor_id)
    REFERENCES ra_integracion_proveedor (proveedor_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

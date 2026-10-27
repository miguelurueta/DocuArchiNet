CREATE TABLE ra_integracion_perfil (
  perfil_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  codigo VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  nombre VARCHAR(160) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  descripcion TEXT CHARACTER SET utf8 COLLATE utf8_bin NULL,
  activo TINYINT UNSIGNED NOT NULL,
  version_publicada_id INT UNSIGNED NULL,
  creado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  actualizado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  creado_utc DATETIME NOT NULL,
  actualizado_utc DATETIME NOT NULL,
  revision INT UNSIGNED NOT NULL,
  PRIMARY KEY (perfil_id),
  UNIQUE KEY uq_ra_integracion_perfil_ambito_codigo (ambito_ref,codigo),
  KEY ix_ra_integracion_perfil_listado (ambito_ref,activo,perfil_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_perfil_version (
  version_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  perfil_id INT UNSIGNED NOT NULL,
  numero INT UNSIGNED NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  revision INT UNSIGNED NOT NULL,
  nombre_visible VARCHAR(160) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  gabinete_id INT UNSIGNED NULL,
  modo_integridad VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL
    DEFAULT 'COMPLETO_OBLIGATORIO',
  ambito_integridad VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NULL,
  estrategia_carpetas VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NULL,
  politica_subcarpetas VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NULL,
  esquema_config SMALLINT UNSIGNED NOT NULL,
  configuracion_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  config_hash BINARY(32) NULL,
  creado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  publicado_por VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NULL,
  creado_utc DATETIME NOT NULL,
  publicado_utc DATETIME NULL,
  PRIMARY KEY (version_id),
  UNIQUE KEY uq_ra_integracion_perfil_version_numero (perfil_id,numero),
  KEY ix_ra_integracion_perfil_version_estado (perfil_id,estado,numero),
  CONSTRAINT fk_ra_integracion_version_perfil FOREIGN KEY (perfil_id)
    REFERENCES ra_integracion_perfil (perfil_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_perfil_regla (
  regla_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  version_id INT UNSIGNED NOT NULL,
  destino_tipo VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  destino_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  fuente_tipo VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  etapa_resolucion VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  obligatoria TINYINT UNSIGNED NOT NULL,
  orden INT UNSIGNED NOT NULL,
  regla_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (regla_id),
  UNIQUE KEY uq_ra_integracion_regla_destino (version_id,destino_tipo,destino_ref),
  KEY ix_ra_integracion_regla_orden (version_id,orden),
  CONSTRAINT fk_ra_integracion_regla_version FOREIGN KEY (version_id)
    REFERENCES ra_integracion_perfil_version (version_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

ALTER TABLE remit_dest_interno_perfil_produccion
  ADD COLUMN ADMINISTRAR_PERFILES_IMPORTACION TINYINT UNSIGNED NOT NULL DEFAULT 0;

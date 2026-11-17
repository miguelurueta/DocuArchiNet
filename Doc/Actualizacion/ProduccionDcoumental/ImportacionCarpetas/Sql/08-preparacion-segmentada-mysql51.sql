-- DOC-120 / contrato08. Aplicacion MANUAL, con ventana DDL propia.
-- MySQL5.1/InnoDB. Verificar esquema y respaldo antes de aplicar.
-- No reinstalar tablas existentes ni ejecutar un downgrade destructivo.
-- Este archivo no habilita jobs, limpieza ni permisos de usuario.
CREATE TABLE ra_integracion_preparacion (
  preparacion_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  usuario_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  nivel_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  version_id INT UNSIGNED NOT NULL,
  clave VARCHAR(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  cabecera_hash BINARY(32) NOT NULL,
  contexto_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  config_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  total_raices INT UNSIGNED NOT NULL,
  total_archivos INT UNSIGNED NOT NULL,
  total_bytes BIGINT UNSIGNED NOT NULL,
  partes_raices INT UNSIGNED NOT NULL,
  partes_archivos INT UNSIGNED NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  revision INT UNSIGNED NOT NULL,
  plan_id INT UNSIGNED NULL,
  creado_utc DATETIME NOT NULL,
  expira_utc DATETIME NOT NULL,
  PRIMARY KEY (preparacion_id),
  UNIQUE KEY uq_preparacion_clave (ambito_ref,usuario_ref,clave),
  UNIQUE KEY uq_preparacion_plan (plan_id),
  KEY ix_preparacion_contexto (ambito_ref,usuario_ref,creado_utc),
  KEY ix_preparacion_estado (estado,expira_utc),
  KEY ix_preparacion_version (version_id),
  CONSTRAINT fk_preparacion_version FOREIGN KEY (version_id)
    REFERENCES ra_integracion_perfil_version (version_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT fk_preparacion_plan FOREIGN KEY (plan_id)
    REFERENCES ra_integracion_plan (plan_id) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_preparacion_parte (
  preparacion_id INT UNSIGNED NOT NULL,
  tipo VARCHAR(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  numero INT UNSIGNED NOT NULL,
  contenido_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  contenido_hash BINARY(32) NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  sello_catalogos VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NULL,
  PRIMARY KEY (preparacion_id,tipo,numero),
  KEY ix_preparacion_parte_estado (preparacion_id,estado),
  CONSTRAINT fk_preparacion_parte FOREIGN KEY (preparacion_id)
    REFERENCES ra_integracion_preparacion (preparacion_id) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_preparacion_resultado (
  preparacion_id INT UNSIGNED NOT NULL,
  tipo VARCHAR(24) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  numero INT UNSIGNED NOT NULL,
  contenido_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  contenido_hash BINARY(32) NOT NULL,
  PRIMARY KEY (preparacion_id,tipo,numero),
  CONSTRAINT fk_preparacion_resultado FOREIGN KEY (preparacion_id)
    REFERENCES ra_integracion_preparacion (preparacion_id) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

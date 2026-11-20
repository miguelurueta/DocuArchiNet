-- DOC-121 | DESTINO DA DE LOS DATOS | EJECUCION MANUAL | UNA SOLA VEZ
-- Precondicion: 00 acredita ausencia de esta tabla y disponibilidad de InnoDB.
-- Sin IF NOT EXISTS: una tabla preexistente se compara, nunca se da por valida.
-- Estados/invariantes se validaran en aplicacion; no CHECK ni JSON nativo.
-- Preparacion nullable en RECIBIDA. LONGTEXT sin DEFAULT, fechas UTC por app.
CREATE TABLE imp01_carga_ejecucion (
  id_carga CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  id_plantilla INT UNSIGNED NOT NULL,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  usuario_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  conexion_ref VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  tabla_destino VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  archivo_nombre VARCHAR(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  archivo_ref VARCHAR(120) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  archivo_bytes BIGINT UNSIGNED NOT NULL,
  archivo_hash BINARY(32) NOT NULL,
  modo VARCHAR(16) CHARACTER SET ascii COLLATE ascii_bin NULL,
  revision_plantilla INT UNSIGNED NULL,
  configuracion_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NULL,
  configuracion_hash BINARY(32) NULL,
  esquema_hash BINARY(32) NULL,
  estado VARCHAR(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  filas_previstas INT UNSIGNED NULL,
  filas_confirmadas INT UNSIGNED NOT NULL DEFAULT 0,
  codigo_incidencia VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NULL,
  creado_utc DATETIME NOT NULL,
  actualizado_utc DATETIME NOT NULL,
  confirmado_utc DATETIME NULL,
  PRIMARY KEY (id_carga),
  KEY ix_carga_propietario_fecha (ambito_ref, usuario_ref, creado_utc, id_carga),
  KEY ix_carga_plantilla_fecha (id_plantilla, creado_utc, id_carga),
  KEY ix_carga_destino_estado (ambito_ref, conexion_ref, tabla_destino, estado)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

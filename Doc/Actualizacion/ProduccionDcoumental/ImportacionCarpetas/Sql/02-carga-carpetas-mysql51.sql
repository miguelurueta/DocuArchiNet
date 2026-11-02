-- DOC-111. Aplicacion manual: leer 02-aplicacion-carga-carpetas.md.
-- MySQL 5.1 / InnoDB. Seleccionar y verificar antes el esquema documental.
-- No ejecutar si existe alguna entidad destino o la columna de permiso.
-- DDL con commit implicito: no envolver en una transaccion como recuperacion.

CREATE TABLE ra_integracion_plan (
  plan_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  usuario_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  nivel_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  version_id INT UNSIGNED NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  revision INT UNSIGNED NOT NULL,
  esquema_doc SMALLINT UNSIGNED NOT NULL,
  contexto_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  config_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  manifiesto_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  resultado_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  plan_hash BINARY(32) NOT NULL,
  creado_utc DATETIME NOT NULL,
  expira_utc DATETIME NOT NULL,
  PRIMARY KEY (plan_id),
  KEY ix_integracion_plan_contexto (ambito_ref,usuario_ref,creado_utc),
  KEY ix_integracion_plan_version (version_id),
  CONSTRAINT fk_integracion_plan_version FOREIGN KEY (version_id)
    REFERENCES ra_integracion_perfil_version (version_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_lote (
  lote_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  plan_id INT UNSIGNED NOT NULL,
  ambito_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  solicitante_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  nivel_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  clave_confirmacion VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  solicitud_hash BINARY(32) NOT NULL,
  version_id INT UNSIGNED NOT NULL,
  modo_integridad VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  ambito_integridad VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NULL,
  contexto_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  config_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  detencion_solicitada TINYINT UNSIGNED NOT NULL,
  total_raices INT UNSIGNED NOT NULL,
  total_archivos INT UNSIGNED NOT NULL,
  total_bytes BIGINT UNSIGNED NOT NULL,
  revision INT UNSIGNED NOT NULL,
  creado_utc DATETIME NOT NULL,
  actualizado_utc DATETIME NOT NULL,
  PRIMARY KEY (lote_id),
  UNIQUE KEY uq_integracion_lote_plan (plan_id),
  UNIQUE KEY uq_integracion_lote_confirmacion (ambito_ref,solicitante_ref,clave_confirmacion),
  KEY ix_integracion_lote_contexto (ambito_ref,solicitante_ref,creado_utc),
  KEY ix_integracion_lote_estado (estado,actualizado_utc),
  KEY ix_integracion_lote_version (version_id),
  CONSTRAINT fk_integracion_lote_plan FOREIGN KEY (plan_id)
    REFERENCES ra_integracion_plan (plan_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT fk_integracion_lote_version FOREIGN KEY (version_id)
    REFERENCES ra_integracion_perfil_version (version_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_raiz (
  raiz_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  lote_id INT UNSIGNED NOT NULL,
  cliente_raiz_id VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  datos_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  identidad_expediente_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NULL,
  valores_efectivos_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NULL,
  intento_activo_id INT UNSIGNED NULL,
  revision INT UNSIGNED NOT NULL,
  creado_utc DATETIME NOT NULL,
  actualizado_utc DATETIME NOT NULL,
  PRIMARY KEY (raiz_id),
  UNIQUE KEY uq_integracion_raiz_cliente (lote_id,cliente_raiz_id),
  KEY ix_integracion_raiz_estado (lote_id,estado),
  CONSTRAINT fk_integracion_raiz_lote FOREIGN KEY (lote_id)
    REFERENCES ra_integracion_lote (lote_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_archivo (
  archivo_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  raiz_id INT UNSIGNED NOT NULL,
  cliente_archivo_id VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  origen_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  ruta_hash BINARY(32) NOT NULL,
  bytes_previstos BIGINT UNSIGNED NOT NULL,
  bytes_recibidos BIGINT UNSIGNED NULL,
  contenido_hash BINARY(32) NULL,
  staging_ref CHAR(32) CHARACTER SET ascii COLLATE ascii_bin NULL,
  estado_recepcion VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  estado_efecto VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  plan_valores_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  valores_efectivos_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NULL,
  resultado_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NULL,
  intento_activo_id INT UNSIGNED NULL,
  revision INT UNSIGNED NOT NULL,
  creado_utc DATETIME NOT NULL,
  actualizado_utc DATETIME NOT NULL,
  PRIMARY KEY (archivo_id),
  UNIQUE KEY uq_integracion_archivo_cliente (raiz_id,cliente_archivo_id),
  UNIQUE KEY uq_integracion_archivo_ruta (raiz_id,ruta_hash),
  KEY ix_integracion_archivo_estado (raiz_id,estado_recepcion,estado_efecto),
  CONSTRAINT fk_integracion_archivo_raiz FOREIGN KEY (raiz_id)
    REFERENCES ra_integracion_raiz (raiz_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_intento (
  intento_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  lote_id INT UNSIGNED NOT NULL,
  raiz_id INT UNSIGNED NULL,
  archivo_id INT UNSIGNED NULL,
  fase VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  estado VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  ejecutor_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  token_reclamacion CHAR(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  solicitud_hash BINARY(32) NOT NULL,
  inicio_utc DATETIME NOT NULL,
  ultimo_contacto_utc DATETIME NOT NULL,
  fin_utc DATETIME NULL,
  evidencia_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NULL,
  PRIMARY KEY (intento_id),
  KEY ix_integracion_intento_lote (lote_id,estado,inicio_utc),
  KEY ix_integracion_intento_raiz (raiz_id,fase),
  KEY ix_integracion_intento_archivo (archivo_id,fase),
  CONSTRAINT fk_integracion_intento_lote FOREIGN KEY (lote_id)
    REFERENCES ra_integracion_lote (lote_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT fk_integracion_intento_raiz FOREIGN KEY (raiz_id)
    REFERENCES ra_integracion_raiz (raiz_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT fk_integracion_intento_archivo FOREIGN KEY (archivo_id)
    REFERENCES ra_integracion_archivo (archivo_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_evento (
  evento_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  lote_id INT UNSIGNED NOT NULL,
  intento_id INT UNSIGNED NULL,
  recurso_tipo VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  recurso_id INT UNSIGNED NULL,
  accion VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  codigo VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  actor_ref VARCHAR(128) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  ocurrido_utc DATETIME NOT NULL,
  detalle_doc LONGTEXT CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (evento_id),
  KEY ix_integracion_evento_lote (lote_id,ocurrido_utc,evento_id),
  KEY ix_integracion_evento_intento (intento_id),
  CONSTRAINT fk_integracion_evento_lote FOREIGN KEY (lote_id)
    REFERENCES ra_integracion_lote (lote_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT fk_integracion_evento_intento FOREIGN KEY (intento_id)
    REFERENCES ra_integracion_intento (intento_id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

ALTER TABLE remit_dest_interno_perfil_produccion
  ADD COLUMN IMPORTAR_CARPETAS TINYINT UNSIGNED NOT NULL DEFAULT 0;

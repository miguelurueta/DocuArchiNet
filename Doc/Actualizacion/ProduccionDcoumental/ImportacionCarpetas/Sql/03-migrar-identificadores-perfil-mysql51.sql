CREATE TABLE ra_integracion_perfil_ai (
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

CREATE TABLE ra_integracion_perfil_version_ai (
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
  KEY ix_ra_integracion_perfil_version_estado (perfil_id,estado,numero)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_perfil_regla_ai (
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
  KEY ix_ra_integracion_regla_orden (version_id,orden)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_mapa_perfil (
  nuevo_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  anterior_id CHAR(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (nuevo_id),
  UNIQUE KEY uq_ra_mapa_perfil_anterior (anterior_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_mapa_version (
  nuevo_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  anterior_id CHAR(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (nuevo_id),
  UNIQUE KEY uq_ra_mapa_version_anterior (anterior_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_mapa_regla (
  nuevo_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
  anterior_id CHAR(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (nuevo_id),
  UNIQUE KEY uq_ra_mapa_regla_anterior (anterior_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

CREATE TABLE ra_integracion_migracion_huella (
  version_id INT UNSIGNED NOT NULL,
  hash_anterior BINARY(32) NULL,
  hash_nuevo BINARY(32) NULL,
  verificado_utc DATETIME NOT NULL,
  PRIMARY KEY (version_id)
) ENGINE=InnoDB DEFAULT CHARACTER SET ascii COLLATE ascii_bin;

INSERT INTO ra_integracion_mapa_perfil (anterior_id) SELECT perfil_id FROM ra_integracion_perfil ORDER BY perfil_id;

INSERT INTO ra_integracion_mapa_version (anterior_id) SELECT version_id FROM ra_integracion_perfil_version ORDER BY version_id;

INSERT INTO ra_integracion_mapa_regla (anterior_id) SELECT regla_id FROM ra_integracion_perfil_regla ORDER BY regla_id;

INSERT INTO ra_integracion_perfil_ai (perfil_id,ambito_ref,codigo,nombre,descripcion,activo,version_publicada_id,creado_por,actualizado_por,creado_utc,actualizado_utc,revision)
SELECT mp.nuevo_id,s.ambito_ref,s.codigo,s.nombre,s.descripcion,s.activo,pub.nuevo_id,s.creado_por,s.actualizado_por,s.creado_utc,s.actualizado_utc,s.revision
FROM ra_integracion_perfil s
INNER JOIN ra_integracion_mapa_perfil mp ON mp.anterior_id=s.perfil_id
LEFT JOIN ra_integracion_mapa_version pub ON pub.anterior_id=s.version_publicada_id;

INSERT INTO ra_integracion_perfil_version_ai (version_id,perfil_id,numero,estado,revision,nombre_visible,gabinete_id,modo_integridad,ambito_integridad,estrategia_carpetas,politica_subcarpetas,esquema_config,configuracion_doc,config_hash,creado_por,publicado_por,creado_utc,publicado_utc)
SELECT mv.nuevo_id,mp.nuevo_id,s.numero,s.estado,s.revision,s.nombre_visible,s.gabinete_id,s.modo_integridad,s.ambito_integridad,s.estrategia_carpetas,s.politica_subcarpetas,s.esquema_config,s.configuracion_doc,s.config_hash,s.creado_por,s.publicado_por,s.creado_utc,s.publicado_utc
FROM ra_integracion_perfil_version s
INNER JOIN ra_integracion_mapa_perfil mp ON mp.anterior_id=s.perfil_id
INNER JOIN ra_integracion_mapa_version mv ON mv.anterior_id=s.version_id;

INSERT INTO ra_integracion_perfil_regla_ai (regla_id,version_id,destino_tipo,destino_ref,fuente_tipo,etapa_resolucion,obligatoria,orden,regla_doc)
SELECT mr.nuevo_id,mv.nuevo_id,s.destino_tipo,s.destino_ref,s.fuente_tipo,s.etapa_resolucion,s.obligatoria,s.orden,s.regla_doc
FROM ra_integracion_perfil_regla s
INNER JOIN ra_integracion_mapa_version mv ON mv.anterior_id=s.version_id
INNER JOIN ra_integracion_mapa_regla mr ON mr.anterior_id=s.regla_id;

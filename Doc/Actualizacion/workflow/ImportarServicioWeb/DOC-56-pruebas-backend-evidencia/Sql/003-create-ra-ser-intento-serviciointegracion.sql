-- MySQL 5.1. Puede iniciarse desde Workflow, pero crea la tabla exclusivamente
-- en la base DocuArchi que contiene ra_ser_serviciointegracion.
-- No crea llaves foraneas entre bases: padre e hija quedan en el mismo esquema.
-- IntentId y ClientItemId son correlaciones con Workflow y no tienen FK cruzada.
-- No usa procedimientos almacenados.

SET @doc56_service_schema = (
  SELECT MIN(t.TABLE_SCHEMA)
  FROM information_schema.TABLES t
  WHERE t.TABLE_NAME = 'ra_ser_serviciointegracion'
  HAVING COUNT(*) = 1
);

SET @doc56_service_engine = (
  SELECT t.ENGINE
  FROM information_schema.TABLES t
  WHERE t.TABLE_SCHEMA = @doc56_service_schema
    AND t.TABLE_NAME = 'ra_ser_serviciointegracion'
  LIMIT 1
);

SET @doc56_service_id_type = (
  SELECT c.COLUMN_TYPE
  FROM information_schema.COLUMNS c
  WHERE c.TABLE_SCHEMA = @doc56_service_schema
    AND c.TABLE_NAME = 'ra_ser_serviciointegracion'
    AND c.COLUMN_NAME = 'Id_ser_servicioIntegracion'
  LIMIT 1
);

SET @doc56_schema_escaped = REPLACE(@doc56_service_schema, '`', '``');

SET @doc56_create_attempt_table = IF(
  @doc56_service_schema IS NULL
  OR UPPER(COALESCE(@doc56_service_engine, '')) <> 'INNODB'
  OR @doc56_service_id_type IS NULL,
  'SELECT * FROM `DOC56_SERVICE_CATALOG_MISSING_AMBIGUOUS_OR_NOT_INNODB`',
  CONCAT(
    'CREATE TABLE IF NOT EXISTS `', @doc56_schema_escaped, '`.`ra_ser_intento_serviciointegracion` (',
    'Id_ser_intentoServicioIntegracion BIGINT NOT NULL AUTO_INCREMENT,',
    'Id_ser_servicioIntegracion ', @doc56_service_id_type, ' NOT NULL,',
    'NombreServicioSnapshot VARCHAR(40) NOT NULL,',
    'Operacion VARCHAR(60) NOT NULL,',
    'Exitoso TINYINT(1) NOT NULL,',
    'CodigoError VARCHAR(100) NULL,',
    'CategoriaError VARCHAR(40) NULL,',
    'CodigoDependencia VARCHAR(120) NULL,',
    'MensajeDiagnostico VARCHAR(1000) NULL,',
    'EstadoHttp SMALLINT NULL,',
    'Reintentable TINYINT(1) NOT NULL DEFAULT 0,',
    'IntentId VARCHAR(32) NULL,',
    'ClientItemId VARCHAR(128) NULL,',
    'OperationId VARCHAR(128) NULL,',
    'CorrelationId VARCHAR(128) NOT NULL,',
    'FechaInicioUtc DATETIME NOT NULL,',
    'FechaFinUtc DATETIME NOT NULL,',
    'DuracionMs BIGINT NOT NULL,',
    'PRIMARY KEY (Id_ser_intentoServicioIntegracion),',
    'KEY ix_ra_ser_intento_disponibilidad (Id_ser_servicioIntegracion, Operacion, FechaInicioUtc, Exitoso),',
    'KEY ix_ra_ser_intento_error (Id_ser_servicioIntegracion, CodigoError, FechaInicioUtc),',
    'KEY ix_ra_ser_intento_correlacion (CorrelationId),',
    'KEY ix_ra_ser_intento_intencion (IntentId, ClientItemId),',
    'CONSTRAINT fk_ra_ser_intento_servicio ',
    'FOREIGN KEY (Id_ser_servicioIntegracion) ',
    'REFERENCES `', @doc56_schema_escaped, '`.`ra_ser_serviciointegracion` (Id_ser_servicioIntegracion) ',
    'ON UPDATE RESTRICT ON DELETE RESTRICT',
    ') ENGINE=InnoDB DEFAULT CHARSET=latin1'
  )
);

PREPARE doc56_create_attempt_statement FROM @doc56_create_attempt_table;
EXECUTE doc56_create_attempt_statement;
DEALLOCATE PREPARE doc56_create_attempt_statement;

SET @doc56_create_attempt_table = NULL;
SET @doc56_service_id_type = NULL;
SET @doc56_service_engine = NULL;
SET @doc56_schema_escaped = NULL;
SET @doc56_service_schema = NULL;

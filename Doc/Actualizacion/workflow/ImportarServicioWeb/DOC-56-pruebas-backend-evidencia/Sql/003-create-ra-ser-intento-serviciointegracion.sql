-- Ejecutar exclusivamente sobre la base de datos del modulo DocuArchi.
-- intent_id y client_item_id son referencias de correlacion entre modulos;
-- deliberadamente no crean una FK hacia la base de datos Workflow.
CREATE TABLE ra_ser_intento_serviciointegracion (
  Id_ser_intentoServicioIntegracion BIGINT NOT NULL AUTO_INCREMENT,
  Id_ser_servicioIntegracion INT(10) NOT NULL,
  NombreServicioSnapshot VARCHAR(40) NOT NULL,
  Operacion VARCHAR(60) NOT NULL,
  Exitoso TINYINT(1) NOT NULL,
  CodigoError VARCHAR(100) NULL,
  CategoriaError VARCHAR(40) NULL,
  CodigoDependencia VARCHAR(120) NULL,
  MensajeDiagnostico VARCHAR(1000) NULL,
  EstadoHttp SMALLINT NULL,
  Reintentable TINYINT(1) NOT NULL DEFAULT 0,
  IntentId VARCHAR(32) NULL,
  ClientItemId VARCHAR(128) NULL,
  OperationId VARCHAR(128) NULL,
  CorrelationId VARCHAR(128) NOT NULL,
  FechaInicioUtc DATETIME NOT NULL,
  FechaFinUtc DATETIME NOT NULL,
  DuracionMs BIGINT NOT NULL,
  PRIMARY KEY (Id_ser_intentoServicioIntegracion),
  CONSTRAINT fk_ra_ser_intento_servicio
    FOREIGN KEY (Id_ser_servicioIntegracion)
    REFERENCES ra_ser_serviciointegracion (Id_ser_servicioIntegracion)
    ON UPDATE RESTRICT
    ON DELETE RESTRICT,
  KEY ix_ra_ser_intento_disponibilidad
    (Id_ser_servicioIntegracion, Operacion, FechaInicioUtc, Exitoso),
  KEY ix_ra_ser_intento_error
    (Id_ser_servicioIntegracion, CodigoError, FechaInicioUtc),
  KEY ix_ra_ser_intento_correlacion
    (CorrelationId),
  KEY ix_ra_ser_intento_intencion
    (IntentId, ClientItemId)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

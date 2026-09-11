-- Ejecutar exclusivamente sobre la base de datos del modulo DocuArchi.
-- Todos los identificadores son snapshots opcionales: otros proveedores pueden
-- no usar tarea Workflow, radicado ni codigo de barras.
ALTER TABLE ra_ser_intento_serviciointegracion
  ADD COLUMN TaskId BIGINT NULL AFTER Reintentable,
  ADD COLUMN Radicado VARCHAR(40) NULL AFTER TaskId,
  ADD COLUMN CodigoBarras VARCHAR(20) NULL AFTER Radicado,
  ADD COLUMN ReferenciaProveedor VARCHAR(120) NULL AFTER CodigoBarras,
  ADD KEY ix_ra_ser_intento_tarea (TaskId, FechaInicioUtc),
  ADD KEY ix_ra_ser_intento_radicado (Radicado, FechaInicioUtc),
  ADD KEY ix_ra_ser_intento_codigo_barras (Id_ser_servicioIntegracion, CodigoBarras, FechaInicioUtc),
  ADD KEY ix_ra_ser_intento_referencia_proveedor (Id_ser_servicioIntegracion, ReferenciaProveedor, FechaInicioUtc);

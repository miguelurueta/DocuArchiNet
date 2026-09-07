# Contrato y mapping

La identidad de elemento es `providerId + externalKey + targetTaskId`. La intención conserva operación, correlación, usuario/grupo, tarea/ruta/trámite, proveedor, requisitos, elementos, estado, versión y fechas UTC.

Individual y múltiple usan el mismo DTO. `externalKey` permanece opaca: la semántica SII corresponde al adaptador futuro.

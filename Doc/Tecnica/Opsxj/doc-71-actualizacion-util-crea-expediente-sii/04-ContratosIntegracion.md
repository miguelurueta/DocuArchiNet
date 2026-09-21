# ACTUALIZACION-UTIL-CREA-EXPEDIENTE-SII

- Ticket: DOC-71
- Cambio OpenSpec: doc-71-actualizacion-util-crea-expediente-sii
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

- No se agregan endpoints ni autoridad suministrada por el cliente.
- `DestinationMode` admite `WithoutExpedient`; los efectos exclusivos de expediente usan
  `NotApplicable`/`NoAplica`.
- `ExpedientId` permanece nulo en modo `SinExpediente`.
- La autenticación y autorización existentes se conservan; el modo se resuelve en servidor.
- No hay DDL nuevo: los estados se persisten por nombre en columnas textuales existentes.
- La compatibilidad es aditiva para respuestas históricas y conserva la rama `GestionarExpediente`.

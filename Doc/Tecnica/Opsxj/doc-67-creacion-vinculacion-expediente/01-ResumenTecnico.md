# CREACION-VINCULACION-EXPEDIENTE

- Ticket: DOC-67
- Cambio OpenSpec: doc-67-creacion-vinculacion-expediente
- Clasificacion: cross_cutting (Transversal)
## Objetivo

La importación SII queda completada con expediente obligatorio, almacenamiento secuencial, descubrimiento por `ENLASE`, vínculo verificado, caché documental, índices SQL/XML y reconciliación persistente.

## Alcance y compatibilidad

- [x] Borde afectado: `WebServiceImportarServicioWebModern.asmx.vb`, bajo el gate existente.
- [x] Servicios modernos: intención, coordinadores de expediente/documentos, repositorios y adaptadores físicos.
- [x] Plataforma E2E: escenarios DOC-56 existentes; no se creó runner ni escenario paralelo.
- [x] Legacy preservado: `ClassAlmacenamiento`, `ClassGaExpediente`, ASMX existentes e `Integracionccv/` conservan sus huellas.
- [x] Reversa: apagar el gate restaura el recorrido legacy; no se borran expedientes, documentos, relaciones ni evidencia ya confirmada.

La matriz completa función–destino–prueba está en [06-MatrizMigracionLegacy.md](06-MatrizMigracionLegacy.md) y la línea base protegida en [07-LineaBaseLegacy.md](07-LineaBaseLegacy.md).

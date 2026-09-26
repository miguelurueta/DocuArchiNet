# RECONCILIACION-PERSISTENCIA-SERVICIO-ENLACE-SII

- Ticket: DOC-81
- Cambio OpenSpec: doc-81-reconciliacion-persistencia-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Implementar la preparación, persistencia idempotente y reconciliación autoritativa de anexos SII para actividades ENLASE, reutilizando el núcleo moderno y encapsulando la función legacy sin asignar, cerrar ni avanzar la tarea.

## Alcance y compatibilidad

- [x] Backend afectado: ASMX moderno, DTO/modelo, preflight, intención, orquestador, proveedor SII, adaptador ENLASE y repositorios de estado/reconciliación.
- [x] Persistencia: capacidad y referencia confiable en intención; identidad externa y resultado por elemento; verificación `logdocuarchi` + gabinete físico.
- [x] Compatibilidad: constancias conservan adaptador, coordinadores y semántica DOC-67; el ASMX legacy conserva su llamada histórica por parámetro opcional.
- [x] Sin superficies UI modificadas; el modal existente consume los mismos envelopes modernos.
- [x] Reversa: desactivar gate, retirar composición/paso ENLASE y reconciliar intenciones abiertas; no eliminar documentos confirmados.

Detalle completo: `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-81-preparacion-persistencia-reconciliacion/`.
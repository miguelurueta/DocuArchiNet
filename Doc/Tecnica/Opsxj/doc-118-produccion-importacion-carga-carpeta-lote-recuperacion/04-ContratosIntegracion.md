# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado y preparación local en curso; sin despliegue ni integración real nueva.

## Contratos e integraciones

Reutilizar AvanzarLote, SolicitarDetencion, ConsultarLote/consultas existentes, ReconciliarRecurso y CerrarReservaSinInicio. No se inventa ReanudarLote. Mantener contrato de solicitud, identidad, revisión y snapshot. Replay confirmado devuelve los mismos IDs; resultado incierto no autoriza otra ejecución. Separar el estado durable EN_CURSO del sobre público INCIERTO y la confirmación de cierre preinicio del éxito documental.

COORD-02 exige dos clientes/sesiones independientes; COORD-03 exige llamada legacy realmente activa al detener. La integración debe demostrar su intervalo activo sin cambiar almacenamiento compartido. Los reportes DOC115/117 conservan su ambiente, SHA y autorización; no conceden autorización ni excepción instrumental a DOC-118. Preparar observación viable y recursos nuevos antes de pedir autorización específica. Las consultas de control RA/DA son SELECT y no deben alterar reclamaciones ni evidencias.


Extensión local de herramientas: `tools/e2e/tests/support/doc118-active-call.cjs` coordina un único avance activo con SAME_COMMAND/OTHER_FILE o STOP. Correlaciona observación externa y PID/hilo, libera ante fallo y no reintenta. Los contratos ASMX no cambian. El plan de integración fija topes HTTP por operación y SELECT separados; aún falta enlazar y calificar el observador real y seleccionar descriptores de ambiente/cuenta.

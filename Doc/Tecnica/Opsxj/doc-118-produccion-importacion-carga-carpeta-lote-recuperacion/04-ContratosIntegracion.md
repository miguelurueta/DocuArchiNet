# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado y preparación local en curso; sin despliegue ni integración real nueva.

## Contratos e integraciones

Reutilizar AvanzarLote, SolicitarDetencion, ConsultarLote/consultas existentes, ReconciliarRecurso y CerrarReservaSinInicio. No se inventa ReanudarLote. Mantener contrato de solicitud, identidad, revisión y snapshot. Replay confirmado devuelve los mismos IDs; resultado incierto no autoriza otra ejecución. Separar el estado durable EN_CURSO del sobre público INCIERTO y la confirmación de cierre preinicio del éxito documental.

COORD-02 exige dos clientes/sesiones independientes; COORD-03 exige llamada legacy realmente activa al detener. La integración debe demostrar su intervalo activo sin cambiar almacenamiento compartido. Los reportes DOC115/117 conservan su ambiente, SHA y autorización; no conceden autorización ni excepción instrumental a DOC-118. Preparar observación viable y recursos nuevos antes de pedir autorización específica. Las consultas de control RA/DA son SELECT y no deben alterar reclamaciones ni evidencias.


Extensión local de herramientas: `tools/e2e/tests/support/doc118-active-call.cjs` coordina un único avance activo con SAME_COMMAND/OTHER_FILE o STOP. Correlaciona observación externa y PID/hilo, libera ante fallo y no reintenta. Los contratos ASMX no cambian. El plan de integración fija topes HTTP por operación y SELECT separados; aún falta enlazar y calificar el observador real y comprobar la vigencia de los descriptores de ambiente/cuenta localizados.

La búsqueda puntual posterior localizó la suite `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs` y el perfil local ignorado `tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json`. El runner existente dio `DOC111_CHECK=PASS; REAL_EXECUTION=false`. Ambiente/nivel/versión/DSN y la referencia histórica usuario141/empresa2 ya están identificados en el plan de integración; falta comprobar vigencia con autorización específica, no solicitarlos de nuevo al usuario. Se reutilizan openSession/createDriver/createSelectControls existentes, conservando recursos históricos intactos.


Enlace implementado: `doc118-session-bridge.cjs` reutiliza openSession/createDriver/createSelectControls. Consume una sola ventana por par, vincula autorización a runId/recursos/snapshot/comando y aplica presupuestos antes de despacho. COORD02 exige revisión vigente del segundo archivo y no acredita CONFLICTO_REVISION como exclusión. COORD03 utiliza SolicitarDetencion; el driver de lectura la rechaza. Los callbacks del observador externo y la preparación autorizada de recursos siguen a cargo del conductor exterior; no existe CLI autónomo de aceptación completa.

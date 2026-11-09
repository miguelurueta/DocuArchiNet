# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: revisión local V2 consolidada en RV-08;25/35 tareas, aceptación real y calificación del runtime abiertas.

## Contratos e integraciones

Contrato adicional RV-06:POST RecuperarDocumentoV2 recibe únicamente loteId/intentoId/revisionLoteEsperada/revisionArchivoEsperada por el guard operativo Gestión/CSRF/autoridad RECONCILIAR. Cierre sin invocación devuelve HTTP200/ok=true, estado RECHAZADO, código EJECUCION_CERRADA_SIN_INVOCACION, cierreSinInvocacion=true y documentoConfirmado=false, sin IDs documentales. Confirmación positiva expone solo IDs verificados. No se aceptan pruebas/rutas/proveedor del cliente. El driver DOC111 aún no admite esta operación y el almacenamiento habitual sigue V1. Resolver esos enlaces bajo alcance local concreto antes de la autorización real descrita en `doc118-plan-real-v2-20261109.md`; no existe todavía CLI ejecutable de ese plan.

Reutilizar AvanzarLote, SolicitarDetencion, ConsultarLote/consultas existentes, ReconciliarRecurso y CerrarReservaSinInicio. No se inventa ReanudarLote. Mantener contrato de solicitud, identidad, revisión y snapshot. Replay confirmado devuelve los mismos IDs; resultado incierto no autoriza otra ejecución. Separar el estado durable EN_CURSO del sobre público INCIERTO y la confirmación de cierre preinicio del éxito documental.

COORD-02 exige dos clientes/sesiones independientes; COORD-03 exige llamada legacy realmente activa al detener. La integración debe demostrar su intervalo activo sin cambiar almacenamiento compartido. Los reportes DOC115/117 conservan su ambiente, SHA y autorización; no conceden autorización ni excepción instrumental a DOC-118. Preparar observación viable y recursos nuevos antes de pedir autorización específica. Las consultas de control RA/DA son SELECT y no deben alterar reclamaciones ni evidencias.


Extensión local de herramientas: `tools/e2e/tests/support/doc118-active-call.cjs` coordina un único avance activo con SAME_COMMAND/OTHER_FILE o STOP. Correlaciona observación externa y PID/hilo, libera ante fallo y no reintenta. Los contratos ASMX no cambian. El plan de integración fija topes HTTP por operación y SELECT separados; aún falta enlazar y calificar el observador real y comprobar la vigencia de los descriptores de ambiente/cuenta localizados.

La búsqueda puntual posterior localizó la suite `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs` y el perfil local ignorado `tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json`. El runner existente dio `DOC111_CHECK=PASS; REAL_EXECUTION=false`. Ambiente/nivel/versión/DSN y la referencia histórica usuario141/empresa2 ya están identificados en el plan de integración; falta comprobar vigencia con autorización específica, no solicitarlos de nuevo al usuario. Se reutilizan openSession/createDriver/createSelectControls existentes, conservando recursos históricos intactos.


Enlace implementado: `doc118-session-bridge.cjs` reutiliza openSession/createDriver/createSelectControls. Consume una sola ventana por par, vincula autorización a runId/recursos/snapshot/comando y aplica presupuestos antes de despacho. COORD02 exige revisión vigente del segundo archivo y no acredita CONFLICTO_REVISION como exclusión. COORD03 utiliza SolicitarDetencion; el driver de lectura la rechaza. Los callbacks del observador externo y la preparación autorizada de recursos siguen a cargo del conductor exterior; no existe CLI autónomo de aceptación completa.


La consola usa una cola local de fases permitidas y hashes de implementación. Los secretos solo viven en el proceso, se pasan efímeramente al helper compartido y se limpian al cerrar. No hay endpoint nuevo de producto, otro login ni SQL arbitrario desde la cola. PREFLIGHT es readOnly y usa HTML/token reales sin subrecursos de producción ni selección visual del árbol. El ledger lleva697 HTTP/5 aperturas y bloquea reinicios; no están completadas las fases documentales del ejecutor. La pareja se puede vincular al segundo caso sin login nuevo, revalidando autoridad por caso.


La ampliación está autorizada; el preflight pasó con17 SELECT. La consola y el worker terminaron, y el finally no fue reconocido. El candidato externo no ejecutó ARM/Freeze/Thaw; no habilita fases documentales por su attach/source binding.


Las fases documentales ya están compuestas en el ejecutor, con orden/consumo únicos, sello de dependencias y marcador exclusivo de corrida. El retorno se corta entre Playwright y el driver después del checkpoint; no se afirma pérdida en el cable. La disponibilidad exige hashes, procesos y heartbeat vigentes antes de secretos. La evidencia final requiere revisión y los guards bloquean otra pareja bajo los topes agotados.


## Corte posterior a la preparación del lote24

PREPARE02 admite la recuperación cerrada del lote24 mediante el mismo cliente y controles SELECT. Exige plan40, IDs/nombres/valores/contexto exactos, ausencia de cualquier ALMACENAR previo y ausencia física; no repite preparación/recepción/creación/materialización. FINISH02 reutiliza la revisión B del snapshot durable inalterado tras el replay. No cambian contratos ASMX. Véase `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-preparacion-recuperacion-20261108.md`.

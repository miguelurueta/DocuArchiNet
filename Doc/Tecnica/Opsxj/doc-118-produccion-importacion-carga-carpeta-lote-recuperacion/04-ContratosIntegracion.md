# DOC-118 — coordinación y recuperación

## Captura revisada lanzada — 2026-11-10

El usuario pidió «lanza» después de la parada sin login o efectos. Se preservó el intento anterior y se abrió la consola en el escritorio normal con observador2 enlazado;al registrar espera APPLICATION_USER. Ambas HEAD se conservan dentro del máximo:acumulado1580 HTTP/172 SELECT/11 sesiones.269/269 Node y32/38 tareas;sin nuevo PASS de integración. Lote25 confirmado/protegido y archivo anterior intacto. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-captura-revisada-20261110.json`.

## Lectura durable resuelta;captura de integración por revisar

Lote25:documento16235/inventario23339 físicamente confirmados y archivoB sin efecto, mediante25 SELECT nuevos y sin replay. La consola se hizo visible fuera del escritorio aislado. La ventana nueva consumió solo1 HEAD;se detuvo antes de login/fases, con SDK/secretos cerrados. Acumulado1579 HTTP/172 SELECT/11 sesiones.268/268 Node candidatos;32/38 tareas. Falta aclarar la captura de cuatro entradas antes de otra ejecución. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-integracion-continuacion-20261110.json`.

## Continuación técnica activa — 2026-11-10

Jira En curso y mismo cambio OpenSpec reactivado, conservando el archivo administrativo intacto. Se acreditan5.2 por resultados/procedencia explícitos y5.4 por sincronización comprobada;5.3 pasó OpenSpec estricto/refinement;32/38, con seis comprobaciones reales pendientes. La auditoría SELECT del lote25 se retoma con contadores intactos y sin replay; requiere dos entradas enmascaradas.22/22 pruebas focales; almacenamiento/SII/binarios intactos. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-continuacion-tareas-20261110.json`.

## Cierre administrativo con salvedades — 2026-11-10

Jira confirmó el cierre administrativo en Listo (comentario24490), aceptado por el usuario conservando las nueve tareas pendientes. 29/38;260/260 Node históricos y observador4.1 calificado, sin aceptación completa de COORD02/03 ni QA/E2E. Lote25 retenido con almacenamiento UNKNOWN, sin replay ni limpieza. Cambio archivado en `openspec/changes/archive/2026-11-10-doc-118-produccion-importacion-carga-carpeta-lote-recuperacion`; sin sincronizar specs ni fusionar la rama. Las compuertas ordinarias OPSXJ siguen sin aprobarse; no se presenta como cierre técnico completo. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-cierre-administrativo-20261110.json`.

## Corte local de pérdida de acuses V2

Completada6.11/D-18/RQ-18: decorador de pruebas de pérdida de un acuse nativo válido y conductor acotado Q1/Q2/Q3, fuera del proyecto productivo.247/247 Node (42 comportamiento/cliente/driver/conductor y205 herramientas);157 casos V2 internos del harness, incluidos sin sumarlos a Node. Progreso28/38, mismas10 tareas reales/finales pendientes. P3 preparado y probado como paquete local; enlace/lector del host de pruebas aún sin instalar/calificar. P4/P5 abiertos. Sin modificar almacenamiento compartido/InvocarAlmacenamiento/SII ni ejecutar HTTP/SQL/login reales. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-acuses-v2-local-20261109.json`.

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificacion: cross_cutting
- Estado: enlace/driver V2 implementados localmente;27/37 tareas, aceptación real y calificación del runtime abiertas.

## Contratos e integraciones

Corte6.9/6.10:el driver compartido ya admite RecuperarDocumentoV2, valida exactamente cuatro referencias y14 propiedades públicas correlacionadas y usa el mismo safePost/CSRF/origen, con cero retries y prohibición readOnly. El enlace candidato usa revisión de lote de servidor y persistencia V2 para intentos nuevos; no cambia el contrato HTTP de avance ni el escritor compartido. La revisión RV-08 que sigue conserva su base histórica; el estado vigente de P1/P2 está en `doc118-plan-real-v2-enlace-20261109.md`, con P3/P4/P5 abiertos y sin autorización real.

### Corte anterior RV-08

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

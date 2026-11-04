<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->

# Diseño DOC-115

## Contexto y alcance diferencial

Base inspeccionada: origin/main 8bf61815f15357947d7d3308fefb5192626988a8, worktree feature/DOC-115. DOC-111 implementa el recorrido; DOC-112 documenta contratos y DOC-114 verifica planes. Este cambio acepta y endurece 07-A/07-B, no construye otro motor. Se revisaron guía 00, prompt 03/07, diseños 01/02/03 y mapa design.md, fuentes VB y pruebas existentes. La presencia de código no garantiza cada carrera ni su despliegue actual.

## Decisiones aprobadas por el usuario — 2026-11-03

### D-01 — Reutilizar una sola fundación operativa en 07-A y 07-B

Origen: RQ-01. Evidencia: MySqlImportacionCarpetasRepository.vb; Sql/02-carga-carpetas-mysql51.sql.

El esquema ya está en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`: seis tablas InnoDB, plan/lote/raíz/archivo/intento/evento, diez FK e índices únicos por plan y clave de confirmación (ámbito, solicitante, clave). No existe un Sql/ operativo paralelo en raíz. Conservarlo y su procedimiento 02-aplicacion; solo una brecha estructural probada justifica incremento separado, nunca reaplicar DDL sobre instalación existente. El repositorio único y sus parciales sirven a 06–11. El mapa normativo es [Diseno/design.md](../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/design.md); actualizar allí el inventario, no duplicar otro catálogo contractual.

### D-02 — Confirmar bajo exclusión transaccional y unicidad de ámbito

Origen: RQ-02. Evidencia: MySqlImportacionCarpetasRepository.vb: ConfirmarPlan, Repeticion.

ConfirmarPlan bloquea el plan, calcula solicitud_hash de plan/revisión/huella y consulta clave/plan dentro de Operar antes de insertar inventario y evento. Verificar tanto mismo plan como dos planes distintos con una misma clave de ámbito: el índice único debe impedir dos lotes y el perdedor debe obtener un conflicto seguro. Operar traduce 1062/1205/1213 a CONFLICTO_REVISION. Si una carrera válida termina en bloqueo/deadlock, conservar esa respuesta y consultar/repetir el mismo comando después de verificar estado; la convergencia se verifica por identidad persistida. No prometer dos respuestas HTTP exitosas inmediatas por ausencia de deadlock en una simulación. Una repetición con payload idéntico devuelve identidad y estado persistidos sin recrear inventario ni expirar retrospectivamente un plan confirmado. Un rollback de BD se prueba sin confundir huecos AUTO_INCREMENT con pérdida de integridad.

### D-03 — Conservar la autoridad y el contrato multipart cerrado

Origen: RQ-03. Evidencia: ImportacionCarpetasRequestGuard.vb: LeerMultipart; ServicioRecepcionImportacion.ObtenerRecepcion.

ASMX ConfirmarPlan conserva {planId, revisionEsperada, planHashEsperado, claveConfirmacion}. ASHX conserva exactamente loteId/archivoId/claveIntento y un archivo; no acepta gabinete, expediente, nivel ni estados nuevos. ServicioRecepcionImportacion resuelve identidad, ámbito, nivel y sello desde recursos persistidos y reautoriza antes del commit. ImportacionCarpetasRequestGuard valida POST/origen/token/cabeceras antes de Form/Files. Rechazo HTTP puede ocurrir en IIS antes del handler: clasificar capa y no exigir un JSON del producto donde el servidor corta el cuerpo.

### D-04 — Mantener staging privado y cuotas efectivas sin ampliar configuración global

Origen: RQ-04. Evidencia: AlmacenStagingImportacion.vb: Recibir, CopiarAcotado, BytesRetenidos; LimitesImportacionCarpetas.DesdeTransporte.

AlmacenStagingImportacion usa App_Data/ImportacionCarpetas, identidades técnicas, .part/.bin, CreateNew, contención y rechazo de enlaces. Lee hasta un byte extra, compara longitud prevista, calcula SHA-256 y valida PDF antes del rename. El nombre original solo se valida como metadato. Caracterizar fallo de cuota considerando .part y .bin retenidos y reenvío del mismo contenido. DesdeTransporte configura MaximoBytesLote=ULong.MaxValue y retención TimeSpan.MaxValue: NO existe hoy una cuota agregada finita de producción ni una política nueva aprobada. Probar la guarda agregada con límites inyectados explícitos; en E2E probar límites efectivos de archivo/perfil/servidor y registrar lo no configurable. No añadir appSettings ni elevar IIS/httpRuntime. PDF es el formato caracterizado; TIF debe rechazarse aunque una extensión aparezca en la selección.

### D-05 — Recuperar por evidencia persistida y física sin transacción disco/BD ficticia

Origen: RQ-05. Evidencia: ServicioRecepcionImportacion.vb: Recibir, ReconciliarRecepcion; MySqlImportacionCarpetasRepository.RegistrarFalloRecepcion.

El rename y el commit MySQL son operaciones separadas. Si existe .bin íntegro y el intento propio queda incierto, reconciliar verificando identidad, tamaño, formato y hash; nunca consumir el original como copia legacy. Si falta/varía el temporal de una fila RECIBIDO, bloquear la reutilización y conservar evidencia. El replay compara contenido con el retenido y CONTENIDO_DISTINTO no degrada el commit anterior. .part no acredita recepción. Para corte antes de un completo, consultar estado/intentohistórico antes de usar una nueva clave: la misma clave de intento RECHAZADO no se reutiliza y una incertidumbre no se libera por tiempo. No exponer ReconciliarRecepcion como endpoint de prueba; es método interno y se valida por integración del servicio. La vía pública existente solo se usa si su contrato autoriza esa fase.

### D-06 — Mantener revisión y reclamación persistida como exclusión real

Origen: RQ-06. Evidencia: MySqlImportacionCarpetasRepository.vb: ReservarIntento, ConfirmarRecepcion; AlmacenStagingImportacion.BloquearLote.

ReservarIntento y ConfirmarRecepcion combinan revisión, token y comprobación de pertenencia; BloquearLote usa exclusión física FileShare.None. El competidor puede recibir RECEPCION_OCUPADA, OPERACION_EN_CURSO o CONFLICTO_REVISION según el punto probado; éxito final exige una sola recepción verificable, no un código genérico cualquiera. El handler implementa IRequiresSessionState y ASMX EnableSession: dos HTTP simultáneos de una sesión pueden serializarse. Por eso 07-A usa conexiones/transacciones independientes para probar la carrera real; las HTTP registran esa limitación y solo prueban concurrencia efectiva si el entorno permite contextos autorizados independientes para el mismo recurso sin cambiar autoridad. No cambiar el comportamiento de sesión global para fabricar la carrera.

### D-07 — Congelar inventario y barrera sin invocar efectos documentales

Origen: RQ-07. Evidencia: CoordinadorImportacionCarpetas.Barreras.vb; ServicioRecepcionImportacion.vb.

Congelar snapshot, nivel, inventario original, modo y ámbito en lote. Completo EXPEDIENTE requiere todos sus archivos; LOTE requiere todo el inventario; parcial conserva NO_APLICA y rechazados. Reutilizar lógica de barrera como aserción sin llamar AvanzarLote, creadores ni adaptadores documentales. El receptor devuelve estadoRecepcion separado de estadoEfecto y no entrega stagingRef/ruta por HTTP. 09 consume referencia controlada y copia de trabajo; 11 es propietario de limpieza. No modificar la función de almacenamiento compleja ni reabrir aislamiento XML descartado.

### D-08 — Extender exclusivamente la infraestructura de pruebas existente

Origen: RQ-08. Evidencia: tools/e2e/tests/support/authenticated-workflow-session.cjs; doc111-importacion-carpetas.cjs; doc114-plan-importacion.cjs.

Inventariar package.json, playwright.config.cjs, AGENT-RUNBOOK, support, scripts y fixtures. Reutilizar authenticated-workflow-session.cjs (acceso Producción ya ampliado), doc111-importacion-carpetas.cjs y controles SELECT de doc114-plan-importacion.cjs donde su contrato lo permita. No llamar el flujo DOC-111 completo porque avanza documentos. El script run-doc111-persistence.ps1 activa temporalmente el perfil 24: no ejecutarlo en DOC-115 por inferencia ni copiarlo entero. Extender sus utilidades/harness con selección explícita de casos y recursos aprobados, sin activación/permisos implícitos. Rutas nuevas implementadas en este corte local: tools/e2e/tests/doc115-recepcion-importacion.spec.cjs; tools/e2e/tests/support/doc115-recepcion-importacion.cjs; tools/e2e/scripts/run-doc115-recepcion-importacion.cjs; tools/e2e/fixtures/doc115-recepcion-cases.json; tests/doc115-recepcion-contracts.test.cjs. Un runner delgado delega autenticación/autorización. Conservar reporters, workers, reintentos y saneamiento de trazas/capturas; no guardar cookies ni cuentas.

### D-09 — Cerrar cada fase con evidencia y aprobación explícita del alcance

Origen: RQ-09. Evidencia: Validacion/doc111-matriz-final-20261102.md; tools/e2e/AGENT-RUNBOOK.md.

RECIB-01 doble confirmación/payload distinto; RECIB-02 truncado/ID cruzado/ruta/cuota/recepción válida/replay y contenido cambiado; RECIB-03 desconexión/consulta/reenvío/respuesta perdida. Complementar 07-A con rollback real, esquema y reclamación concurrente. Cada caso registra EJECUTADO_OK/FALLIDO/NO_EJECUTADO, comando, SHA del código y backend servido, recursos/IDs y huellas SELECT antes/después. DOC-111 es antecedente y sus dobles no acreditan transacciones MySQL ni cortes de red DOC-115. No hay autorización nueva para ambiente, cuentas ni datos: responsable de la ejecución es el equipo del ambiente y la ejecución real queda NO_EJECUTADO hasta autorización específica. 07-A se cierra con repositorios/SQL; 07-B con RECIB reales, sin exigir efectos documentales a ninguna fase. Suspender ante resultado incierto, conservar IDs/staging y consultar antes de repetir. No borrar lotes ni recursos conservados.

## Contratos y errores

Respuesta común RespuestaPerfilImportacion: ok, codigo, mensaje, operacionId, datos. Confirmación devuelve loteId/planId/nivelRef/estado/revision/totales. Recepción devuelve loteId/archivoId/estadoRecepcion/estadoEfecto/bytesRecibidos/contenidoHash/revision. IDs y bytes se transportan en cadenas decimales según el DTO vigente; revisión UInt32. No admitir estructuras de estado nuevas desde cliente. Errores existentes que se caracterizan: SOLICITUD_INVALIDA, ACCESO_DENEGADO, CONFLICTO_REVISION, CAPACIDAD_NO_DISPONIBLE, LIMITE_EXCEDIDO, ARCHIVO_RECHAZADO, FORMATO_NO_SOPORTADO, CONTENIDO_DISTINTO, RECEPCION_OCUPADA, OPERACION_EN_CURSO y RESULTADO_INCIERTO; usar EstadoHttp vigente sin inventar códigos alternos.

## Orden de entrega y verificación

1. Inventario/contratos y SQL existente; harness local sin secretos.
2. 07-A repositorios e integración transaccional autorizada independiente de UI.
3. 07-B protección/recepción, RECIB-01/02/03 y regresión pertinente.
4. Evidencia por fase, documentación común y validación del cambio.

MSBuild real de GestionDocumental-Docuarchi.net.vbproj si cambia VB/proyecto; node --check en archivos JavaScript modificados; harness Framework y suites Node pertinentes. No compilar ni ejecutar pruebas de integración por editar documentación. Rollout sin DDL automático: revisar estado instalado; incremento separado solo si existe brecha. Rollback de código no elimina datos confirmados ni staging; recuperación explícita conserva recursos.

## Corte de implementación local — 2026-11-03

El usuario aprobó el refinamiento. La extensión compartida se limita a aceptar scopes RECIB-01/02/03 en createPreparationClient, conservando el contrato PLAN. La suite de recepción usa controles y autenticación existentes. RECIB-03 corta una petición HTTP incompleta y descarta deliberadamente el resultado de otra recepción confirmada; esta segunda parte prueba pérdida del resultado en cliente y no acredita caída del servidor ni respuesta perdida en red. No se ejecutaron estos escenarios reales. Ver Validacion/doc115-implementacion-local-20261103.md y la matriz por fase.

## Preparación 07-A posterior a RECIB — 2026-11-04

Se reutiliza el runner de persistencia con un modo autorizado separado, sin activar perfiles ni exigir tablas vacías. El mapa común registra la parcial de test y el soporte PowerShell. Tres planes/dos lotes máximos y un recibo permiten probar transacciones en conexiones independientes y cuatro rollbacks controlados. Se usa contexto de fixture autorizado y comprobaciones SELECT de autoridad/nivel/publicación, sin dependencia de 06/UI. No hay escritores SQL nuevos: la persistencia usa el repositorio existente. Ver Validacion/doc115-07a-preparacion-20261104.md para presupuesto, recuperación y límites.

## Resultado de implementación y validación — 2026-11-04

Las 28 tareas se completaron. 07-A verifica transacciones en conexiones independientes, unicidad, exclusión y cuatro rollbacks reales con el repositorio único. RECIB-01/02/03 cuentan con resultados HTTP/SELECT saneados y huellas por escenario. No se modificaron el producto VB, proyecto, esquema ni configuración. El diagnóstico de ODBC conserva pasos/códigos cerrados mediante tools/e2e/scripts/support/doc115-readonly-failure.ps1. Los cortes históricos anteriores se conservan; el informe vigente está en Validacion/doc115-07a-resultados.md y doc115-07b-resultados.md. La pérdida de resultado RECIB-03 corresponde al descarte en cliente y la cuota finita se caracteriza localmente por inyección.

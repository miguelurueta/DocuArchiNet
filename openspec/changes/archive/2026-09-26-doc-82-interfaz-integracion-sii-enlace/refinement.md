<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-82-interfaz-integracion-sii-enlace

## Fuente y alcance

- Ticket: `DOC-82` — INTERFAZ-INTEGRACION-SII-ENLACE.
- Cambio OPSXJ/OpenSpec: `doc-82-interfaz-integracion-sii-enlace`.
- Perfil tecnológico: ASP.NET Web Forms/VB.NET, JavaScript ES5 aditivo, ASMX moderno, CSS adaptable y plataforma E2E Playwright existente.
- Alcance: interfaz moderna para consultar, previsualizar, preparar e importar uno o varios anexos SII durante la preasignación `ENLASE`, refrescar la lista documental y conservar la asignación como acción explícita con revalidación final en servidor.
- Fuera de alcance: crear endpoints backend, asignar automáticamente, retirar el recorrido legacy, modificar `ClassAlmacenamiento` o cambiar los contratos publicados por DOC-80/DOC-81.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx`: administrador documental ENLASE, disparador `a_adj_service_web`, lista `GridView_list_documento_relacion_wf`, botón `Buttonaceptar` y modal moderno compartido.
- `workflow/Webworkflow.aspx.vb`: registro de assets/bootstrap moderno y `Buttonaceptar_Click`, que llama `Verfica_existencia_tipo_documental_obligatorio_digitalizado` antes de asignar.
- `js/workflow/Webworkflow.js`: recorrido legacy `ActivaListaAnexosIntegracionSII` y llamadas AJAX antiguas, conservadas para alternancia y caracterización.
- `js/workflow/importar-servicio-web/`: API, core, registro de proveedor, UI, preview, preparación, intención, progreso, reconciliación, lista documental, recuperación y guard de contexto reutilizables.
- `js/workflow/importar-servicio-web/sii/`: adaptador, mapper y lista actuales, cuya presentación está orientada a constancias y requiere una variante por capacidad para anexos.
- DOC-80 publicó `ResolveCapabilities`, `QueryItems`, `GetPreview` y streaming seguro para `ANEXOS_RADICADO_ENLASE`; DOC-81 publicó preflight, intención, ejecución, consulta y reconciliación.
- No existe un contrato moderno `ValidateAssignment`; la autoridad implementada permanece en `Buttonaceptar_Click`. La UI no inferirá cumplimiento documental a partir del éxito de importación.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Reutilizar un único modal y núcleo moderno, activándolo desde `a_adj_service_web` con proveedor `INTEGRACIONSII` y capacidad `ANEXOS_RADICADO_ENLASE`; no crear otra aplicación ni otro proveedor. | `Webworkflow.aspx`; `RegisterImportarServicioWebModernBootstrap`; `ImportarServicioWebUi.initialize`; `ImportarServicioWebProviderRegistry.create` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Capturar tarea, radicado confiable, proveedor y capacidad desde atributos del disparador y propagar la capacidad en todas las solicitudes; el guard debe impedir que una respuesta de una tarea actualice otra. | `requestContext`; `taskId`; `ImportarServicioWebTaskContextGuard.create` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Resolver la presentación por capacidad: anexos muestran identidad y metadatos publicados por DOC-80, soportan cero/uno/múltiples, selección individual/total y deshabilitan elementos no importables sin alterar la tabla de constancias. | `ImportarServicioWebSiiAdapter.create/renderItems`; mapper DOC-80; `updateSelectionState` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Reutilizar preview, preflight y preparación modernos; la tipología se predetermina únicamente cuando el catálogo backend trae una resolución inequívoca y toda selección múltiple crea una sola intención. | `ImportarServicioWebPreview.create`; `ImportarServicioWebPreparation`; `ImportarServicioWebIntentClient.confirm`; contratos DOC-81 | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Bloquear cierre normal y acciones incompatibles mientras la ejecución no tenga resultado; cerrar tras éxito completo solamente después del refresco documental y mantener abierto ante fallo, parcial o incierto. | `setExecutionCloseLock`; `executeCreatedIntent`; `closeAfterResult`; `ImportarServicioWebRecovery` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Sincronizar solo documentos confirmados de la tarea original, deduplicados por `DocumentId`, mediante el refresco Web Forms existente; nunca insertar una proyección en otra tarea. | `ImportarServicioWebDocumentListAdapter.collect/synchronize`; `refreshDocumentListPartial`; `GridView_list_documento_relacion_wf` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Conservar `Asignar` como acción explícita y autoritativa: la UI no habilita por inferencia ni asigna automáticamente; `Buttonaceptar_Click` vuelve a validar documentos obligatorios y rechaza la operación si faltan. | `Webworkflow.aspx.vb::Buttonaceptar_Click`; `ClassWorkflowDigitalizacion.Verfica_existencia_tipo_documental_obligatorio_digitalizado` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Mantener modal contenido en viewport, scroll vertical del cuerpo y horizontal de tabla, foco inicial/restaurado, trampa de foco, teclado y regiones `aria-live`, sin desfigurar la página anfitriona al cerrar. | `Styles/importar-servicio-web-modern.css`; `ImportarServicioWebUi.open/close/onKeydown`; markup del modal | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Mantener el recorrido legacy bajo alternancia, cubrir la integración con pruebas focales y extender el escenario E2E ENLASE existente; una corrida autenticada o mutadora requiere autorización independiente. | `WorkflowCentroTrabajoModernActive`; `tools/e2e`; `AGENT-RUNBOOK.md`; pruebas `importar-servicio-web-*` | D-09 | RQ-09 | Origen: D-09, RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El disparador ENLASE abre la experiencia moderna con la capacidad correcta sin duplicar modal/proveedor. | Dada una preasignación ENLASE válida, al activar servicio web se consulta `ANEXOS_RADICADO_ENLASE`; fuera del gate continúa disponible el legacy. | Registro duplicado o sustitución irreversible del legacy. |
| RQ-02 | Todas las operaciones quedan ligadas a la tarea original y rechazan contexto cambiado. | Si cambia la tarea durante consulta o escritura, no se proyectan resultados y se informa conflicto seguro. | Respuesta tardía contaminando otra tarea. |
| RQ-03 | La lista representa cero, uno o múltiples anexos con acciones coherentes. | Seleccionar todos afecta solo importables; deseleccionar limpia la selección; importados/no disponibles no se preparan. | Regresión visual de constancias o columnas falsas. |
| RQ-04 | Preview y preparación consumen únicamente contratos publicados y una selección produce una intención. | Preview conserva selección/foco; catálogo inequívoco predetermina; catálogo ambiguo exige elección; N anexos seleccionados crean una intención con N elementos. | Una ejecución por fila o tipología inventada en cliente. |
| RQ-05 | El cierre respeta el estado autoritativo de ejecución. | Una ejecución en curso bloquea X/Escape/backdrop; éxito completo refresca y cierra; fallo/parcial/incierto permanece visible y recuperable. | Ocultar errores o cerrar antes del refresco. |
| RQ-06 | La lista documental refleja resultados confirmados sin duplicados. | Solo `Disponible/Importada`, `DocumentId > 0` y tarea coincidente se sincronizan; duplicados se ignoran y luego se ejecuta el refresco Web Forms. | Duplicación o documento en tarea equivocada. |
| RQ-07 | Importar nunca equivale a asignar y la decisión final permanece en servidor. | Tras importar, la tarea no cambia; al pulsar Asignar, `Buttonaceptar_Click` revalida obligatorios y solo continúa cuando responde `YES`. | La UI no muestra una garantía preventiva inexistente. |
| RQ-08 | El modal es usable con teclado y viewports representativos. | El diálogo queda dentro del viewport, la tabla desplaza internamente, el foco no escapa y vuelve al disparador al cerrar. | CSS global afectando Webworkflow. |
| RQ-09 | La entrega es reversible y verificable. | Suites focales pasan; E2E reutiliza login/perfiles/evidencia; gate queda restaurado; no se afirma una corrida no ejecutada. | Mutación real sin autorización o retirada prematura del legacy. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox conserva tamaño, área/archivos, origen y verificación observable.
3. La UI consume exclusivamente contratos DOC-80/DOC-81; una necesidad backend nueva detiene el punto afectado y exige otro refinamiento.
4. Las consultas E2E de control son solo `SELECT`; credenciales, cookies, rutas físicas y excepciones crudas no forman parte de la evidencia.

## Resultado del refinamiento

- Estado: aprobado para implementar la interfaz DOC-82 con la decisión aceptada de revalidación autoritativa en `Buttonaceptar_Click`.
- La aprobación no autoriza por sí sola una corrida E2E autenticada, mutadora ni la activación del gate.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-82 --sync`.

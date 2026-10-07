<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-89-correcion-guardar-documento-escaner-enlace

## Fuente y alcance

- Ticket: `DOC-89` — CORRECION-GUARDAR-DOCUMENTO-ESCANER-ENLACE.
- Cambio OpenSpec: `doc-89-correcion-guardar-documento-escaner-enlace`.
- Fuente Jira: `specs/correcion-guardar-documento-escaner-enlace/jira-context.md`.
- Perfil tecnologico: ASP.NET Web Forms sobre .NET Framework 4.6.1, Microsoft AJAX, JavaScript legacy y CSS.
- Corte minimo: presentacion del progreso de `ButtonAlmacenar` en `workflow/Webworkflow.aspx`; no se cambia captura, transporte, almacenamiento ni proyeccion documental.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx:90-131` registra `InitializeRequest`; la rama de las lineas 123-129 agrupa `ButtonAlmacenar` y `Button_guardar_desicion` y llama `posicion_update_pogres_modal('progres_bar')`.
- `js/workflow/Webworkflow.js:6634-6671` implementa `posicion_update_pogres_modal`: agrega `overlay_`, fuerza ancho completo, muestra el nodo y lo posiciona en `(0,0)`.
- `Styles/Aplicaction.css:1291-1301` define `.overlay_` con ancho y alto completos y fondo translucido.
- `Styles/workflow-centro-trabajo-moderno.css:1820-1842` convierte `#progres_bar.ctw-loading-indicator` en una tarjeta blanca. Al coexistir ambas clases, la tarjeta ocupa toda la superficie.
- `workflow/Webworkflow.aspx:4110-4112` emite `#progres_bar` con `ctw-loading-indicator` en la experiencia moderna oficial.
- `workflow/Webworkflow.aspx.vb:4807-4827` ejecuta una sola llamada a `ClassAlmacenamiento.UploadSaveFileScan`, marca `Hidden_result_load_` y proyecta los datos de la fila; este contrato no necesita cambios.
- `workflow/Webworkflow.aspx:132-180,340-348` retira `overlay_` al finalizar y, si `Hidden_result_load_` vale `YES`, llama una vez a `insert_row_documento_relacionado`.
- `js/workflow/WebFormEscan.js:579-605` enruta por `Hidden21`: `1` usa `ButtonAlmacenar`, `2` añade al documento, `3` migracion, `4` radicacion simple y `5` reemplazo de version. Es un componente compartido y queda intacto.
- `Resources/online_demo_operation.js:203,329,339` contiene el transporte Dynamsoft y su continuacion legacy para `-2003`; no se mezcla esa deuda tecnica con la correccion visual.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Separar en `InitializeRequest` la presentacion de `ButtonAlmacenar`: mostrar `#progres_bar` como indicador compacto sin llamar `posicion_update_pogres_modal` ni agregar `overlay_`. | `workflow/Webworkflow.aspx:96-131`; `js/workflow/Webworkflow.js:6634-6671` | D-01 | RQ-01, RQ-02 | Origen: D-01, RQ-01, RQ-02 |
| D-02 | Conservar sin cambios la rama de `Button_guardar_desicion`, que continua usando `posicion_update_pogres_modal` y su overlay legacy. | `workflow/Webworkflow.aspx:123-129,178-180` | D-02 | RQ-03 | Origen: D-02, RQ-03 |
| D-03 | No modificar `WebFormEscan`, Dynamsoft, `ClassAlmacenamiento`, `ButtonAlmacenar_Click`, hidden fields ni `insert_row_documento_relacionado`; la correccion es exclusivamente visual en el contenedor Workflow. | `js/workflow/WebFormEscan.js:579-605`; `workflow/Webworkflow.aspx.vb:4807-4827`; `workflow/Webworkflow.aspx:340-348` | D-03 | RQ-04, RQ-05 | Origen: D-03, RQ-04, RQ-05 |
| D-04 | Limpiar al finalizar cualquier estilo transitorio introducido para el indicador compacto, tanto en exito como en rechazo, sin recarga, `DataBind`, temporizador ni segundo intento de almacenamiento. | `workflow/Webworkflow.aspx:132-180`; `#progres_bar` | D-04 | RQ-02, RQ-06 | Origen: D-04, RQ-02, RQ-06 |
| D-05 | Mantener la experiencia moderna como unica presentacion oficial; no introducir `WorkflowCentroTrabajoModernActive`, listas de usuarios/grupos ni rutas visuales paralelas. | `workflow/Webworkflow.aspx:4110`; `Styles/workflow-centro-trabajo-moderno.css:1820-1842`; `AGENTS.md` | D-05 | RQ-07 | Origen: D-05, RQ-07 |
| D-06 | Proteger la frontera compartida con pruebas estructurales que inventarien `Hidden21`, verifiquen una sola persistencia/proyeccion y demuestren que los selectores globales y callbacks no cambian. | `js/workflow/WebFormEscan.js:579-605`; `Resources/online_demo_operation.js`; `tools/e2e/tests` | D-06 | RQ-03, RQ-04, RQ-05 | Origen: D-06, RQ-03, RQ-04, RQ-05 |
| D-07 | Completar build, validacion OpenSpec, QA reproducible y E2E real reutilizando la plataforma existente; la E2E mutante solo se ejecuta con autorizacion explicita y evidencia saneada. | `GestionDocumental-Docuarchi.net.sln`; `tools/e2e/AGENT-RUNBOOK.md`; `tools/e2e` | D-07 | RQ-08 | Origen: D-07, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | `ButtonAlmacenar` sigue iniciando un async postback dentro de `UpdatePanel_boton_tool`. | WHEN el iframe activa `window.parent.ButtonAlmacenar.click()` THEN no ocurre navegacion completa, recarga ni `PostBackTrigger`. | Se conserva el ciclo Microsoft AJAX existente. |
| RQ-02 | Durante ese async postback la interfaz permanece perceptible y el progreso es compacto, accesible y no ocupa el viewport. | WHEN inicia `ButtonAlmacenar` THEN `#progres_bar` se muestra sin `overlay_`, sin ancho/alto completo y con `role=status`; WHEN termina THEN queda oculto y sin estilos residuales. | La correccion no cambia el resto de indicadores. |
| RQ-03 | `Button_guardar_desicion` conserva exactamente su presentacion anterior. | WHEN inicia esa operacion THEN continua llamando `posicion_update_pogres_modal`; WHEN termina THEN retira `overlay_`. | Evita regresion en carga tradicional y otras decisiones de guardado. |
| RQ-04 | El escaner compartido conserva todas las continuaciones por `Hidden21`. | WHEN se inspeccionan y prueban TRAMITE/TRAMITE_ADJUNTOWORKFLOW, añadir, MIGRACION, TRAMITE SIMPLE y REMPLAZAVERSION THEN sus botones/callbacks son los mismos. | No se tocan `WebFormEscan.js`, Dynamsoft ni estilos globales. |
| RQ-05 | Una aceptacion produce como maximo una llamada a `UploadSaveFileScan` y un nodo nuevo. | WHEN el almacenamiento responde `YES` THEN `Hidden_result_load_` se consume una vez e `insert_row_documento_relacionado` se invoca una vez; doble clic no agrega una segunda escritura. | No se agregan retries, timers, reload ni `DataBind`. |
| RQ-06 | Exito y error finalizan limpiamente. | WHEN termina o falla el async postback THEN el indicador se oculta, no queda overlay/bloqueo y se conserva el mensaje de error existente. | No se ocultan excepciones ni se cambia el contrato servidor. |
| RQ-07 | La solucion funciona en la experiencia moderna oficial sin mecanismos de activacion retirados. | WHEN se inspecciona el diff THEN no aparecen feature flags, gates, usuarios/grupos ni duplicacion de la ruta visual. | La clase moderna existente se reutiliza de forma acotada. |
| RQ-08 | La entrega tiene evidencia reproducible y reversa local. | WHEN concluye la implementacion THEN pasan pruebas focales/regresion, MSBuild, `git diff --check`, OpenSpec estricto, QA y E2E autorizada; la reversa elimina solo la rama visual especifica. | Sin autorizacion E2E se registra bloqueo y no se simula evidencia real. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` se desarrolla en `design.md`, se refleja en `spec.md` y tiene tareas con `Origen: D-XX, RQ-XX`.
2. Ninguna tarea se marca completa sin evidencia verificable.
3. Un cambio en `WebFormEscan`, Dynamsoft, `ClassAlmacenamiento`, `.overlay_` global o `ctw-loading-indicator` global amplia el alcance y exige nueva aprobacion.
4. Antes de E2E autenticada se lee `tools/e2e/AGENT-RUNBOOK.md`; no se guardan secretos y los controles de datos son solo `SELECT`.

## Resultado del refinamiento

- Estado: aprobado para planificacion atomica e implementacion quirurgica.
- La alternativa aprobada tiene una sola frontera productiva: la presentacion cliente de `ButtonAlmacenar` en `workflow/Webworkflow.aspx`.
- Los hallazgos `OnHttpUploadSuccess`, continuacion `-2003` y typo `heigth` quedan expresamente fuera de DOC-89.

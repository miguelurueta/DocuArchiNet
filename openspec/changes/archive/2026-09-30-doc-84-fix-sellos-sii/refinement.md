<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-84-fix-sellos-sii

## Fuente y alcance

- Ticket: `DOC-84` — FIX-SELLOS-SII
- Cambio OpenSpec: `doc-84-fix-sellos-sii`
- Fuente Jira: `specs/fix-sellos-sii/jira-context.md`
- Perfil tecnológico: `legacy-webforms-vb`
- Alcance: proyección inmediata de sellos/constancias en la lista Workflow; ENLASE es invariante protegido.

## Contexto inspeccionado

- `LegacyImportDocumentStorageAdapter.Almacenar` recibe `idImagen` e `imagen`, pero descarta `imagen` al construir el resultado.
- `LegacyEnlaseImportDocumentStorageAdapter.CrearProyeccion` ya implementa el patrón tipado exclusivo de ENLASE y no se modifica.
- `ImportServiceOrchestrator.MapItem`, `ServicioReconciliacionImportacion.ProjectExecutionResult`, `progress-adapter` y `reconciliation.js` solo conocen `EnlaseProjection`.
- `createLegacyGridAppender` fabrica para `wf` una cadena parcial con gabinete, radicado y formato vacíos e icono genérico.
- `importar-servicio-web-ui.js` conserva `refreshDocumentListPartial`, `Button_actualiza_trevie_seleccion.click()` y `PageRequestManager` como fallback de sellos.
- El commit `0e8199c8` introdujo la proyección completa ENLASE y dejó explícitamente parcial la rama Workflow; DOC-84 corrige solo esa asimetría.
- La evidencia DOC-83 demuestra que ENLASE funciona con ocho campos, destino `rad`, deduplicación y preservación efímera; no concede autorización para nuevas E2E.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Crear modelo/DTO Workflow propios y construir los ocho campos desde `idImagen`, `imagen` y el comando ya validado, sin segunda consulta. | `LegacyImportDocumentStorageAdapter.Almacenar`; `ImportarServicioWebModels.vb`; `ImportarServicioWebDtos.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Propagar y preservar `WorkflowProjection` solo para ítems confirmados con coincidencia estricta de capacidad, identidad, documento y tarea. | `StoreImportExecutionStep`; `ImportServiceOrchestrator.MapItem`; `ServicioReconciliacionImportacion.ProjectExecutionResult` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Crear un appender Workflow exclusivo que valide, escape, deduplique, inserte en `wf` y compruebe la fila; ENLASE permanece intacto. | `importar-servicio-web-document-list-adapter.js`; `GredviewControl.js:insert_row_documento_relacionado` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Despachar por capacidad y eliminar del cierre moderno cualquier recarga/postback; mantener abierto el modal ante fallo de proyección. | `importar-servicio-web-ui.js:refreshDocumentListPartial/closeAfterResult` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Registrar el módulo antes de la UI y renovar versiones públicas de todos los scripts modificados. | `GestionDocumental-Docuarchi.net.vbproj`; `Webworkflow.aspx.vb:RegisterImportarServicioWebModernAssets` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Probar contrato backend/frontend, ausencia de recarga y regresión ENLASE; documentar E2E como bloqueada sin autorización. | `tests/importar-servicio-web-*`; documentación DOC-81/DOC-83 | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Predeterminar una única tipología equivalente a Constancia de Inscripción antes de otras tipologías obligatorias, tolerando tildes, mayúsculas y errores ortográficos menores. | `importar-servicio-web-preparation.js:defaultDocumentType` | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Proyección Workflow con ocho campos correctos. | Almacenamiento `YES` produce campos completos; `DBT` cae a `extension`; faltantes fallan cerrado. | No modificar almacenamiento legacy ni consultar de nuevo. |
| RQ-02 | `WorkflowProjection` llega a la respuesta confirmada y sobrevive a la reconciliación inmediata. | Coincidencia estricta conserva; cualquier diferencia descarta. | No persistir proyección ni contaminar otra tarea. |
| RQ-03 | Una fila completa, operable y no duplicada aparece en `GridView_list_documento_relacion_wf`. | Destino `wf`, ocho campos, escape y deduplicación por documento. | No reutilizar DTO/appender ENLASE. |
| RQ-04 | El cierre no ejecuta recarga y solo ocurre tras comprobar la fila. | Fallo deja modal abierto; éxito no usa postback ni `DataBind`. | Documento puede persistir aunque la UI falle; se informa sin fingir éxito. |
| RQ-05 | Assets registrados en orden y con versión nueva. | Módulo Workflow antes de UI; versiones verificadas por prueba. | Evitar caché de implementaciones previas. |
| RQ-06 | ENLASE permanece idéntico y las pruebas locales son determinísticas. | Suites ENLASE pasan; pruebas negativas detectan cruces y recargas. | E2E requiere autorización separada. |
| RQ-07 | Sellos propone automáticamente Constancia de Inscripción cuando existe una coincidencia autorizada inequívoca. | La opción queda seleccionada para el lote completo; ambigüedad conserva selección manual. | No inventar identificadores ni elegir entre dos coincidencias equivalentes. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en `spec.md` y vinculada a tareas con `Origen: D-XX, RQ-XX`.
2. Las tareas de validación y documentación conservan origen explícito.
3. No se modifica `LegacyEnlaseImportDocumentStorageAdapter`, `ProyeccionDocumentoEnlaseImportacion`, `ImportEnlaseDocumentProjectionDto`, `MapEnlaseProjection`, `enlaseData` ni el destino `rad`.
4. No se ejecuta E2E autenticada, carga o mutación externa sin autorización explícita conforme a `AGENTS.md` y al runbook.

## Resultado del refinamiento

- Estado: aprobado tras inspección de código, commit causal, prompts F05–F08 y evidencia DOC-83.
- Perfil: `legacy-webforms-vb`.
- Siguiente comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-84 --sync --tech-profile legacy-webforms-vb`.

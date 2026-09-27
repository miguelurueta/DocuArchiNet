# Inventario técnico y trazabilidad

## Componentes DOC-82

| Tipo | Ruta del archivo | Clase/interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Dependencias |
|---|---|---|---|---|---|---|---|
| JavaScript | `js/workflow/importar-servicio-web/importar-servicio-web-ui.js` | módulo IIFE | `initialize` | `options: Object` | control/Object o null | Compone modal, API, adapters, guard y eventos | DOM, API moderna |
| JavaScript | misma | módulo IIFE | `requestContext` | `control: Object` | Object | Lee tarea/radicado/proveedor/capacidad activos | trigger WebForms |
| JavaScript | misma | módulo IIFE | `openPreparation` | `control: Object, keys: Array<String>, trigger: Element` | `undefined` | Catálogo, tipología y preflight | preparation/requirements |
| JavaScript | misma | módulo IIFE | `executeCreatedIntent` | `control: Object, intent: Object` | Promise implícita | Ejecuta, reconcilia y proyecta | intent client, progress, guard |
| JavaScript | misma | módulo IIFE | `settleAfterResult` | `control, execution, authoritative: Object` | Promise | Decide permanencia/cierre después de resultado | document list |
| JavaScript | `js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-adapter.js` | módulo IIFE | `create` | `options: Object` | adapter/Object | Fuerza capacidad ENLASE y delega API/mapping | adapter SII, list |
| JavaScript | `js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-list.js` | módulo IIFE | `render` | `container: Element, data: Object` | `undefined` | Tabla 0/1/N y selección | DOM |
| JavaScript | `js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-assignment-bridge.js` | módulo IIFE | `create` | `options: Object` | bridge/Object | Expone controles a bloquear; no asigna | Webworkflow DOM |
| JavaScript | `js/workflow/importar-servicio-web/importar-servicio-web-task-context-guard.js` | módulo IIFE | `create` | `options: Object` | guard/Object | Captura contexto y bloquea conflicto de tarea | CustomEvent |
| WebForms | `workflow/Webworkflow.aspx.vb` | `Webworkflow` | `Buttonaceptar_Click` | `sender: Object, e: EventArgs` | `System.Void` | Entrada autoritativa de asignación | sesión, clases legacy |
| VB | `workflow/ClassWorkflowDigitalizacion.vb` | `ClassWorkflowDigitalizacion` | `Verfica_existencia_tipo_documental_obligatorio_digitalizado` | `Radicado: String, nombre_gabinete: String, id_tramite: Integer` | `String` | Revalida obligatorios; solo `YES` permite continuar | lista de chequeo/gabinete |

## Endpoints consumidos

Todos pertenecen a `webservice/WebServiceImportarServicioWebModern.asmx`, usan POST JSON, requieren sesión Workflow y validan tarea/proveedor/capacidad. DOC-82 consume sin modificar: `ResolveCapabilities`, `QueryItems`, `GetPreview`, `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`. Los DTO exactos están inventariados en DOC-80/DOC-81; la capacidad es siempre `ANEXOS_RADICADO_ENLASE`.

| Grupo DTO | Propiedades relevantes para DOC-82 | Validación |
|---|---|---|
| Contexto | TaskId, Radicado, ProviderId, Capability | tarea positiva; contexto coincide con trigger capturado |
| Item consultado | ExternalKey, DisplayName, ContentType, ImportStatus, AllowedActions | clave no vacía; acciones gobiernan preview/import |
| Item preparado | ExternalKey, DocumentTypeId/Name, TargetTaskId, FileName | tipología válida y tarea original |
| Intención/ejecución | IntentId, VersionToken, Items | una intención; token de versión; N Items |
| Reconciliación | Status, Items, DocumentId, PersistenceKnown, ReachedPhase | solo persistencia confirmada se proyecta |

## Diagramas

| ID | Archivo | Propósito |
|---|---|---|
| DOC82-D01 | `Diagramas/01-componentes.puml` | Componentes y dependencias reales |
| DOC82-D02 | `Diagramas/02-interfaz-secuencia.puml` | Secuencia consulta→preparación→ejecución→respuesta |
| DOC82-D03 | `Diagramas/03-estados.puml` | Máquina visible y cierre |
| DOC82-D04 | `Diagramas/04-asignacion-actividad.puml` | Revalidación autoritativa al asignar |

Elementos no verificables como símbolos: navegador/usuario/SII (externos) y estados visibles (conceptuales). No se identificaron componentes DOC-82 adicionales pendientes de revisión; la lógica backend consumida conserva su documentación propia DOC-80/DOC-81.

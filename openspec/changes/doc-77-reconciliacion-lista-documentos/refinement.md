<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-77-reconciliacion-lista-documentos

## Fuente y alcance

- Ticket: `DOC-77` — RECONCILIACION-LISTA-DOCUMENTOS.
- Dependencias: frontend F01-F05 implementado hasta DOC-76, endpoints `GetImportIntent`/`ReconcileImportIntent` y fixtures B05.
- Perfil tecnológico: JavaScript ES5 compatible con WebForms/ASMX, lista documental legacy existente y pruebas Node `node:test`.
- Alcance: reconciliar el resultado autoritativo, reflejar estados por elemento y agregar una sola vez los documentos confirmados a la lista de la tarea visible.
- Fuera de alcance: almacenamiento, ASMX, consultas directas a SII, interpretación de `dato_lista`, una segunda lista documental y cambios a funciones legacy.

## Contexto inspeccionado

- `js/workflow/importar-servicio-web/importar-servicio-web-api.js`: única puerta frontend para `GetImportIntent` y `ReconcileImportIntent`.
- `js/workflow/importar-servicio-web/importar-servicio-web-progress-adapter.js`: adapta el resultado de `ExecuteImportIntent`, conserva `TaskId`/`DocumentId` y no consulta SII.
- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`: mantiene modal, filtros, selección, foco y composición del recorrido moderno.
- `js/workflow/documentos-relacionados-visual.js`: capa visual existente de la lista documental que debe preservarse.
- `js/workflow/Webworkflow.js` y `workflow/Webworkflow.aspx`: consumidores existentes de `insert_row_documento_relacionado(...)`; se inspeccionan, no se modifican para interpretar contratos modernos.
- `Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v1/`: casos completado, parcial, incierto, duplicado y tarea incorrecta publicados por B05.
- `Doc/Actualizacion/workflow/ImportarServicioWeb/PromptBackend/05-reconciliacion-lista-documentos.md`: contrato autoritativo y límites de compatibilidad.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La reconciliación consume `GetImportIntent` y `ReconcileImportIntent` exclusivamente mediante `importar-servicio-web-api.js`, sin consultar SII ni ejecutar persistencia. | `importar-servicio-web-api.js`; fixtures `reconciliation-v1` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Los estados backend se proyectan conservadoramente; timeout o respuesta ausente quedan en `Verificando` y nunca producen disponibilidad/importación optimista. | fixtures `completed.json`, `partial.json`, `uncertain.json` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Solo un item `Disponible` con `DocumentId` puede alimentar la lista, y únicamente cuando su `TaskId` coincide estrictamente con la tarea visible. | `wrong-task.json`; `importar-servicio-web-progress-adapter.js` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La actualización final recorre `response.Items` una vez y deduplica por `DocumentId`, sin depender del orden visual ni insertar durante la espera global. | `duplicated-document.json`; `partial.json` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Un adaptador visual encapsula el puente con la lista existente; si faltan datos seguros para el contrato visual solicita refresco autoritativo completo y no inventa `dato_lista`. | `Webworkflow.js`; `documentos-relacionados-visual.js` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Volver desde el documento importado conserva filtros y scroll, limpia selección y restaura foco predecible; cerrar y reabrir obtiene estado persistido. | `importar-servicio-web-ui.js`; `workflow/Webworkflow.aspx` | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El frontend consulta/reconcilia una intención mediante la API moderna y recibe items estructurados. | WHEN el resultado es incierto o se reabre autorizadamente THEN se realiza una consulta por intención, tarea, proveedor e identidad externa sin acceso directo a SII. | Mantiene al backend como fuente de verdad y evita un segundo orquestador. |
| RQ-02 | Cada elemento muestra `Disponible`, `Verificando`, `ResultadoIncierto`, `Inconsistente`, `Completado`, `Parcial`, `Detenido` o `Fallido` según respuesta real. | WHEN hay timeout, ausencia o estado desconocido THEN el elemento permanece verificando/no confirmado y no se marca importado. | Evita falsos positivos y conserva compatibilidad con estados futuros. |
| RQ-03 | Un documento confirmado aparece solo en la lista de su tarea. | WHEN `TaskId` difiere de la tarea visible THEN no se inserta ni se modifica la lista actual; WHEN coincide y hay `DocumentId` autorizado THEN se habilita su visualización. | Impide contaminación entre tareas y exposición no autorizada. |
| RQ-04 | Una respuesta múltiple actualiza en lote y cada `DocumentId` aparece como máximo una vez. | WHEN dos items refieren el mismo documento THEN el adaptador emite una sola actualización; el orden de items no cambia el resultado. | Evita duplicados y acoplamiento al orden visual. |
| RQ-05 | La lista existente se actualiza mediante un contrato seguro o un refresco completo. | WHEN los datos modernos son suficientes THEN el adaptador invoca el punto visual existente; WHEN no lo son THEN refresca autoritativamente y no interpreta `dato_lista`. | Preserva funciones legacy y evita fabricar campos delimitados. |
| RQ-06 | La navegación conserva el contexto visual y la reapertura refleja persistencia. | WHEN el usuario abre y vuelve del documento THEN conserva filtros/scroll, limpia selección y recupera foco; WHEN reabre el modal THEN ve el estado backend persistido. | Reduce regresión de usabilidad sin mantener estado documental paralelo. |

## Reglas de trazabilidad obligatorias

Cada decisión se desarrolla en `design.md`, se expresa en `spec.md` y aparece en `tasks.md` con `Origen: D-XX, RQ-XX`. Pruebas, documentación y validación conservan el mismo origen.

## Resultado del refinamiento

- Estado: aprobado.
- Dependencias F01-F05 y B05 verificadas en código, contratos y fixtures existentes.
- El diseño privilegia rechazo seguro y refresco autoritativo frente a inserciones optimistas o contratos legacy inventados.

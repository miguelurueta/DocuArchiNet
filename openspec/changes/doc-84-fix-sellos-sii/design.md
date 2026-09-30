<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-84 corrige la proyección inmediata de sellos o constancias importados desde SII en `GridView_list_documento_relacion_wf`. La persistencia actual usa `LegacyImportDocumentStorageAdapter`, pero la frontera cliente comparte `createLegacyGridAppender` con ENLASE y, para destino `wf`, fabrica una cadena parcial con gabinete, radicado y formato vacíos e icono genérico. Después, la UI puede recurrir a `Button_actualiza_trevie_seleccion` y un postback parcial para reconstruir la lista.

ENLASE ya posee un recorrido estabilizado e independiente (`LegacyEnlaseImportDocumentStorageAdapter` → `ProyeccionDocumentoEnlaseImportacion` → `ImportEnlaseDocumentProjectionDto` → destino `rad`). Ese recorrido es un invariante protegido y no se modifica en DOC-84.

## Goals / Non-Goals

**Goals**

- Construir una proyección Workflow tipada con los ocho campos del contrato histórico usando únicamente el resultado del almacenamiento confirmado.
- Propagar la proyección efímera por ejecución, respuesta, progreso y reconciliación sin persistir `dato_lista`.
- Insertar una sola fila completa en `GridView_list_documento_relacion_wf` mediante JavaScript y destino `wf`.
- Fallar cerrado ante proyección incompleta, tarea distinta, resultado incierto o fila no comprobable.
- Eliminar del cierre moderno de sellos todo postback o recarga parcial/completa de la lista.
- Mantener intactos el código, DTO, mapeo, destino y criterios de confirmación de ENLASE.

**Non-Goals**

- Modificar `AlmacenaDocumentoTareaWorkflow`, `ClassAlmacenamiento`, `insert_row_documento_relacionado` o sus consumidores legacy.
- Agregar consultas para volver a leer el documento almacenado.
- Persistir la proyección visual o una cadena delimitada.
- Crear otra lista, GridView o raíz frontend.
- Ejecutar E2E autenticada sin autorización explícita de ambiente, cuentas y datos descartables.

## Decisions

### D-01 — Proyección Workflow independiente y cerrada

Se agrega `ProyeccionDocumentoWorkflowImportacion` al modelo interno y `ImportWorkflowDocumentProjectionDto` al contrato público, ambos independientes de los tipos ENLASE. `LegacyImportDocumentStorageAdapter` construye la proyección después de `AlmacenaDocumentoTareaWorkflow` con `idImagen`, `stru_datos_image_lista` y el comando ya validado.

Campos: gabinete, identificador, radicado, tipo físico, tipología, tarea, estado de firma e icono. `DBT` tiene prioridad y `extension` es el único fallback físico. Gabinete, radicado y tipología pueden usar los valores seguros del comando. El icono solo admite el valor retornado o un fallback explícito derivado del formato conocido. Si falta un campo obligatorio, el resultado no se presenta como proyectable.

### D-02 — Propagación tipada y preservación efímera

`ResultadoFaseImportacion` y `ResultadoElementoImportacion` transportan la proyección Workflow. El orquestador la copia al ítem y la expone como `WorkflowProjection` solo para estados confirmados. `ServicioReconciliacionImportacion.ProjectExecutionResult` conserva la proyección de ejecución cuando coinciden capacidad no ENLASE, `ClientItemId`, `ExternalKey`, documento y tarea, y cuando los ocho campos son válidos. La proyección no se persiste ni se reconstruye desde un snapshot posterior.

### D-03 — Appender Workflow exclusivo

Se crea `importar-servicio-web-workflow-document-list-adapter.js`. Valida los ocho campos, identidad de documento y tarea visible, elimina `|` de valores y escapa HTML únicamente al construir la cadena legacy. Deduplica por `id_wf`, llama una sola vez `insert_row_documento_relacionado(legacyData, "wf", 1)` y comprueba que la fila exista después de insertar.

El appender ENLASE, `enlaseData`, `EnlaseProjection` y destino `rad` permanecen sin cambios funcionales.

### D-04 — Despacho explícito y ausencia de recarga

La UI selecciona el appender según la capacidad: ENLASE usa su appender vigente y sellos usa el nuevo appender Workflow. `document-list-adapter` transporta ambas proyecciones sin mezclarlas. El cierre de sellos no llama `refreshDocumentListPartial`, `Button_actualiza_trevie_seleccion.click()`, `PageRequestManager`, `DataBind`, postback ni recarga de página. Si la proyección o su comprobación falla, el modal permanece abierto con un mensaje seguro.

### D-05 — Integración y caché controladas

El nuevo módulo se registra en `GestionDocumental-Docuarchi.net.vbproj` y se carga antes de `importar-servicio-web-ui.js`. Todos los scripts modificados reciben una versión pública nueva y coherente para invalidar caché.

### D-06 — Evidencia determinística y compatibilidad

Las pruebas cubren backend, mapeos, reconciliación, appender Workflow, deduplicación, lote, tarea visible, ausencia de recarga y aislamiento ENLASE. Se actualiza el paquete canónico `DOC-84-correccion-proyeccion-sellos-workflow`. Las pruebas locales no usan autenticación, red ni mutación externa; cualquier E2E queda registrada como bloqueada hasta autorización expresa.

### D-07 — Tipología predeterminada orientada a constancias

La preparación selecciona primero una única opción autorizada cuyo nombre sea equivalente a `Constancia de Inscripción`, antes de aplicar el fallback de tipología obligatoria. La comparación ignora mayúsculas, tildes, separadores y tolera hasta dos ediciones por cada palabra conceptual (`CONSTANCIA` e `INSCRIPCION`), cubriendo errores menores como `Contancia` o `Inscrpcion`. Si más de una opción satisface el concepto, no se elige ninguna y el usuario conserva la decisión manual. La selección siempre transporta el identificador y nombre exactos del catálogo.

## Risks / Trade-offs

- `stru_datos_image_lista` puede omitir `DBT`; se acepta `extension` porque proviene de la misma escritura confirmada y no implica una consulta adicional.
- Un icono ausente puede impedir la proyección. El fallback debe ser una tabla cerrada por formato; no se permite un icono genérico indiscriminado.
- La reconciliación autoritativa no persiste datos visuales. La preservación se limita a la respuesta de ejecución actual y exige coincidencia estricta para evitar contaminación entre ítems o tareas.
- La inserción legacy interpreta HTML. El escape se realiza una sola vez en la frontera JavaScript y se elimina el delimitador para mantener exactamente ocho campos.
- Fallar cerrado puede dejar el modal abierto aunque el documento esté persistido; es preferible a mostrar una fila incompleta e inoperable.
- La coincidencia tolerante podría encontrar más de una variante; en ese caso no se predetermina ninguna para evitar una clasificación incorrecta.

## Migration Plan

1. Caracterizar y mantener verdes las pruebas ENLASE existentes.
2. Agregar modelo, DTO y construcción Workflow sin alterar contratos existentes.
3. Propagar y preservar la proyección efímera en backend y adaptadores frontend.
4. Registrar el appender Workflow, separar el despacho y retirar el fallback de recarga.
5. Renovar versiones de assets y ejecutar pruebas focales, regresión ENLASE, compilación y OpenSpec estricto.
6. Documentar evidencia y limitaciones. No ejecutar E2E autenticada sin autorización.

## Rollback

Revertir conjuntamente el DTO/modelo Workflow, su propagación, el nuevo módulo JavaScript y el despacho de UI. El rollback no requiere revertir ni modificar ENLASE, almacenamiento legacy, esquema de base de datos o datos persistidos.

## Open Questions

- La aceptación visual autenticada requiere ambiente, cuenta y tarea descartable expresamente autorizados; queda fuera de la validación local de esta implementación.

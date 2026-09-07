<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-54-reconciliacion-lista-documetos

## Fuente y alcance

- Ticket: `DOC-54` — RECONCILIACION-LISTA-DOCUMETOS.
- Perfil: backend VB.NET/WebForms legacy con servicios modernos aditivos.
- Alcance: reconstruir desde persistencia el resultado de una intención DOC-52/DOC-53 y publicar Get/Reconcile seguros, sin alterar escritura legacy.

## Contexto inspeccionado

- Contratos/modelos: `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb` y `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb`.
- Lectura actual: `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`.
- Get y mapping actuales: `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`.
- Fuentes modernas: `workflow_import_intent`, `workflow_import_intent_item` y `workflow_import_intent_transition`.
- `insert_row_documento_relacionado`, `dato_lista`, `AlmacenaDocumentoTareaWorkflow` y cachés SII son fronteras legacy inmutables.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Un servicio único coordina Get/Reconcile y revalida usuario, tarea e intención. | validador y contexto persistido | D-01 | RQ-01 | 2.1, 4.1 |
| D-02 | Un repositorio especializado concentra lecturas parametrizadas de intención, items, documento y relación. | repositorio/fábricas de datos existentes | D-02 | RQ-02 | 1.1, 3.1 |
| D-03 | La búsqueda focal usa intención, proveedor e identidad externa sin cruzar tarea. | claves persistidas del item | D-03 | RQ-03 | 1.2, 3.2 |
| D-04 | Solo una relación documental única con la tarea original produce `Disponible`. | document_id y target_task_id | D-04 | RQ-04 | 2.2, 3.3 |
| D-05 | Un mapper puro aplica una tabla total de fase/consistencia a estado visible. | enum y DTO existentes | D-05 | RQ-05 | 2.3, 3.4 |
| D-06 | Timeout o efecto no demostrable queda `Verificando`/`ResultadoIncierto`, nunca `Disponible`. | PersistenceKnown y estados DOC-53 | D-06 | RQ-06 | 2.4, 3.5 |
| D-07 | El contrato v1 expone mínimos de lista sin ruta, excepción, secreto ni `dato_lista`. | DTO v1 y restricciones Jira | D-07 | RQ-07 | 1.3, 2.5, 3.6 |
| D-08 | Confirmados se deduplican por documento/tarea; anomalías conservan correlación. | DocumentId y CorrelationId | D-08 | RQ-08 | 2.6, 3.7 |
| D-09 | Entrega aditiva sin modificar ASMX, JS, escritura, almacenamiento ni Backend 06. | restricciones Jira | D-09 | RQ-09 | 1.4, 3.8, 4.2-4.4 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Get/Reconcile rechazan contexto ajeno sin filtrar existencia. | Usuario/tarea distintos retornan error seguro y cero items. | Evita enumeración. |
| RQ-02 | La instantánea se reconstruye solo desde persistencia. | Tras perder respuesta retorna última verdad confirmada. | Sin Session/navegador. |
| RQ-03 | La consulta focal queda dentro de su intención. | External key ajena no se retorna. | Evita cruces. |
| RQ-04 | Solo relación única/correcta habilita documento. | Ausencia, duplicado o cruce queda inconsistente. | No contamina otra lista. |
| RQ-05 | Cada fase/consistencia tiene un estado frontend único. | Toda fase válida obtiene salida contractual. | Evita éxito ambiguo. |
| RQ-06 | Incertidumbre no se convierte en éxito. | Timeout/lectura parcial queda incierto. | Evita reintento duplicado. |
| RQ-07 | DTO mínimo y saneado. | No serializa rutas, SQL, excepciones ni dato_lista. | Compatibilidad v1. |
| RQ-08 | Un confirmado por documento/tarea y anomalía trazable. | Duplicados se deduplican o marcan inconsistentes. | Soporte conserva correlación. |
| RQ-09 | Legacy intacto y documentación canónica. | Diff limitado a adiciones modernas. | Rollback por retiro. |

## Resultado del refinamiento

- Estado: aprobado para planificación e implementación.
- Documentación canónica: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-54-reconciliacion-lista-documentos/`.
- No se autoriza DDL, base real, E2E autenticado, carga ni gates en esta fase.

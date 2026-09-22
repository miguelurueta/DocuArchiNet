<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-76-progreso-integracion-sii

## Fuente y alcance

- Ticket: `DOC-76` — PROGRESO-INTEGRACION-SII.
- Contrato normativo: `Doc/Actualizacion/workflow/ImportarServicioWeb/CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`, secciones 1, 4 y 5.
- Perfil tecnológico: JavaScript ES5 compatible con WebForms/ASMX, CSS propio del feature y pruebas Node `node:test`.
- Alcance: ejecutar una intención ya creada, representar la espera global indeterminada y presentar el resultado autoritativo por elemento.
- Fuera de alcance: almacenamiento, reintentos, cancelación, polling, porcentajes y cualquier cambio al recorrido legacy.

## Contexto inspeccionado

- `js/workflow/importar-servicio-web/importar-servicio-web-api.js`: publica `executeImportIntent` y `getImportIntent` sobre el transporte ASMX.
- `js/workflow/importar-servicio-web/importar-servicio-web-intent-client.js`: preflight y creación idempotente de una intención; no ejecuta aún la intención.
- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`: controla modal, listado, preview y preparación con listeners registrados, sin handlers inline.
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`: contratos de ejecución, consulta e item.
- `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`: ejecución única, agregado global y snapshot por elemento.
- `js/java_general/JSProgresBar.js`: infraestructura legacy inspeccionada solo para demostrar aislamiento.
- `Styles/importar-servicio-web-modern.css` y el `.vbproj`: puntos canónicos para estilos y registro.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | El frontend invoca exactamente una vez `ExecuteImportIntent` por intención, independientemente de la cardinalidad, y nunca ejecuta fases por elemento. | `importar-servicio-web-api.js`; `ImportServiceOrchestrator.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El adaptador transforma únicamente `response.Items` y aplica el mapeo normativo de fase backend a estado visible, conservando clave, fase, documento y mensaje seguro. | `ImportarServicioWebDtos.vb`; contrato compartido §4 | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La vista muestra una espera global indeterminada accesible y, al terminar, resultados independientes y un resumen sin falso éxito total. | contrato compartido §1; `importar-servicio-web-ui.js` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | `GetImportIntent` solo se usa por timeout, pérdida de respuesta o reapertura autorizada; no hay polling ni temporizadores. | contrato compartido §3 y §5; API ASMX | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Cerrar la vista no cancela ni revierte la operación y esta entrega no presenta reintento de fallidos. | Jira DOC-76; `ExecuteImportIntentRequestDto` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Los módulos modernos quedan aislados de `JSProgresBar`, códigos legacy y almacenamiento; se registran y prueban por invariancia. | `JSProgresBar.js`; contrato compartido §2 y §5 | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Individual y múltiple realizan una sola ejecución síncrona de la intención completa. | WHEN se confirma una intención de uno o varios elementos THEN se registra una sola invocación con su `IntentId` y `VersionToken`. | Evita duplicados y conserva al backend como único ejecutor. |
| RQ-02 | Cada item final conserva clave, fase backend, estado visible, mensaje seguro y metadatos aplicables. | WHEN llega `Items` THEN cada fase se mapea según el contrato y estados desconocidos no inventan éxito. | Previene interpretaciones optimistas y fuga de códigos legacy. |
| RQ-03 | Mientras la promesa está en curso se ve una espera indeterminada; al resolver se ven todos los resultados y conteos. | WHEN termina THEN el resumen cuenta importadas, omitidas, fallidas y no procesadas, y solo declara éxito total si todas son importadas. | Accesibilidad y resultados parciales explícitos. |
| RQ-04 | No existen consultas repetitivas durante la llamada síncrona. | WHEN está en curso THEN `getImportIntent` no se invoca; WHEN hay recuperación autorizada THEN se realiza una sola consulta explícita. | Reduce carga y evita un segundo orquestador. |
| RQ-05 | El cierre no ofrece cancelación ni reintento. | WHEN se cierra THEN la promesa continúa sin `StopRequested`, cancelación ni reversión, y no aparece “Reintentar fallidos”. | Respeta la semántica backend. |
| RQ-06 | El feature moderno no usa ni altera infraestructura legacy. | WHEN se prueban los módulos THEN no usan `JSProgresBar`, códigos legacy ni almacenamiento. | El gate apagado conserva el flujo legacy; rollback por archivos aditivos. |

## Reglas de trazabilidad obligatorias

Cada decisión se desarrolla en `design.md`, se expresa en `spec.md` y aparece en `tasks.md` con `Origen: D-XX, RQ-XX`. Pruebas, documentación y validación conservan el mismo origen.

## Resultado del refinamiento

- Estado: aprobado.
- Dependencia B04 verificada mediante DTO, endpoint ASMX y `ImportServiceOrchestrator` existentes.
- El cambio puede pasar a implementación después de validar la sincronización OpenSpec.

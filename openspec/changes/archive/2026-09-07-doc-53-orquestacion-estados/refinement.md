<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-53-orquestacion-estados

## Fuente y alcance

- Ticket: `DOC-53` — ORQUESTACION-ESTADOS.
- Perfil: backend VB.NET/WebForms legacy con núcleo moderno aditivo.
- Alcance: ejecutar y consultar intenciones DOC-52 usando el cliente DOC-51, sin sustituir la coreografía vigente.

## Contexto inspeccionado

- Contratos/estados en `DTOs/Workflow/ImportarServicioWeb/` y `Modelo/Workflow/ImportarServicioWeb/`.
- Persistencia en `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`.
- Secuencia observada en `Exploracion/03-exploracion-backend-importar-servicio-web.md` y `04-radiografia-backend-actual.md`.
- Caja negra `workflow/ClassAlmacenamiento.vb::AlmacenaDocumentoTareaWorkflow` y patrón de adaptadores de `Infrastructure/Workflow/Terminar/`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Un orquestador servidor carga, autoriza y ejecuta; UI/ASMX legacy no coordinan fases nuevas. | DTO Execute/Get; servicios DOC-52 | D-01 | RQ-01 | 1.1, 3.1 |
| D-02 | Los items se procesan en secuencia y cada transición usa versión optimista. | `FaseImportacionServicio`; repositorio | D-02 | RQ-02 | 1.2, 2.1, 3.2 |
| D-03 | La máquina de estados es pura, rechaza saltos y genera auditoría saneada. | enum existente; adaptadores Terminar | D-03 | RQ-03 | 2.2, 3.3 |
| D-04 | Se conserva el orden expediente, índices, almacenamiento y caché con transacciones locales independientes. | radiografía y secuencias actuales | D-04 | RQ-04 | 2.3, 3.4 |
| D-05 | El adaptador nuevo es el único consumidor de `AlmacenaDocumentoTareaWorkflow` y no modifica la caja negra. | firma de 16 argumentos | D-05 | RQ-05 | 2.4, 3.5 |
| D-06 | Un efecto no demostrable queda `ResultadoIncierto` y no se reintenta hasta reconciliar. | no existe transacción distribuida | D-06 | RQ-06 | 2.5, 3.6 |
| D-07 | La detención ocurre entre unidades, conserva confirmados y no inicia pendientes. | cardinalidad y estados existentes | D-07 | RQ-07 | 2.6, 3.7 |
| D-08 | Execute/Get v1 exponen fase, `persistenceKnown`, retryable, correlación y versión. | DTOs y resultado por item | D-08 | RQ-08 | 1.3, 2.7, 3.8 |
| D-09 | Entrega aditiva sin tocar ASMX, JS, Integracionccv, ServiciosIntegracion o almacenamiento legacy. | restricciones Jira | D-09 | RQ-09 | 2.8, 4.1-4.3 |

## Requisitos verificables

| ID | Resultado observable | Escenario | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Execute inicia solo una intención autorizada y operable. | Rechazo no invoca efectos. | Autoridad permanece en servidor. |
| RQ-02 | Items secuenciales con token de versión. | N+1 espera terminal de N. | Evita doble ejecución. |
| RQ-03 | Solo transiciones declaradas y auditables. | Salto inválido no muta. | Auditoría saneada. |
| RQ-04 | Última fase confirmada siempre consultable. | Fallo por fase queda coherente. | Sin rollback global. |
| RQ-05 | Adaptador caracteriza la llamada legacy. | Argumentos y resultado tipados. | Sesión queda encapsulada. |
| RQ-06 | Incertidumbre bloquea reintento mutador. | Exige reconciliación. | Evita duplicados. |
| RQ-07 | Detención no revierte ni inicia más items. | Pendientes quedan detenidos. | Reanudación controlada. |
| RQ-08 | Get devuelve instantánea persistida v1. | No depende de Session. | Mapping compatible. |
| RQ-09 | Legacy permanece intacto. | Diff y suite prueban aislamiento. | Rollback retira lo nuevo. |

## Resultado del refinamiento

- Estado: aprobado para implementación.
- No se autoriza DDL, base real, E2E autenticado, carga ni activación de gates en esta fase.

<!-- opsxj:refinement version=1 state=approved -->
# Refinamiento - doc-69-lista-preview-incripciones

## Fuente y alcance
- Ticket: `DOC-69` — LISTA-PREVIEW-INCRIPCIONES.
- Fuente: `specs/lista-preview-incripciones/jira-context.md` y prompt backend 10.
- Dependencias: DOC-67/DOC-68 archivados. Perfil: WebForms/ASMX, VB.NET 4.6.1, MySQL 5.1 y `node:test`.

## Contexto inspeccionado
- `WebServiceImportarServicioWebModern.GetPreview` valida gate, sesión, tarea y proveedor.
- `GetPreviewMetadataAsync` descarga hoy el anexo completo, descarta los bytes y usa `ExternalKey` como descriptor.
- `SiiPreviewResponseFactory` valida expiración, tipos, tamaño y disposición, pero no crea autoridad de canje.
- `WorkflowPreviewSessionContextGate` resuelve identidad y conexiones desde sesión autenticada.
- Los handlers legacy aceptan rutas del cliente y no son reutilizables.
- La sesión es `InProc`; memoria/sesión no soportan múltiples nodos.

## Decisiones aprobadas
| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Snapshot temporal compartido en Workflow; nunca memoria, sesión o ruta pública. | `GetPreviewMetadataAsync`; connection factory | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Descriptor aleatorio; persistir SHA-256 y ligar usuario, tarea, proveedor y `ExternalKey`. | DTO/context gate | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | `HEAD` no consume; `GET` reclama una vez mediante transición atómica. | patrones de transacción modernos | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Allowlist, 10 MiB, nombre seguro y headers defensivos; inline solo permitido. | factory/configuración | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Descargar SII una vez al crear snapshot; handler nunca compone cliente SII. | cliente SII | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | TTL y limpieza idempotente; ausente/vencido/ajeno/consumido son opacos. | repositorios/errores seguros | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Handler reutiliza gate/contexto modernos; gate apagado corta antes de I/O. | context gate/feature gate | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Pruebas, amenazas, MSBuild y E2E autorizada con controles `SELECT`. | `tests/`; `tools/e2e` | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables
| ID | Resultado observable | Aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Snapshot compartido sin efectos documentales. | Disponible entre nodos. | BLOB acotado y temporal. |
| RQ-02 | Descriptor opaco y contextual. | Alterado o ajeno se rechaza igual. | No registrar descriptor. |
| RQ-03 | HEAD no consume; un GET gana. | Carrera: un éxito. | Reclamar antes de emitir. |
| RQ-04 | Respuesta segura y acotada. | Tipo/tamaño inválido no transmite. | Autoridad solo servidor. |
| RQ-05 | Una descarga SII por descriptor. | HEAD/GET: cero SII. | Descarga inicial inevitable. |
| RQ-06 | Expiración sin filtrar existencia. | Respuesta pública uniforme. | Diagnóstico solo saneado. |
| RQ-07 | Gate/contexto modernos. | Rechazo antes de I/O. | Legacy intacto. |
| RQ-08 | Evidencia reproducible. | Suites, build y E2E pasan. | E2E requiere autorización. |

## Resultado del refinamiento
- Estado: `approved`.
- D-01 a D-08 están trazadas a RQ-01 a RQ-08.

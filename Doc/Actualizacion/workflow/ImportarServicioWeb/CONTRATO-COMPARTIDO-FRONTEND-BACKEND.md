# Contrato compartido frontend/backend de ImportarServicioWeb

Este documento es normativo para los prompts de `Prompt/` y `PromptBackend/`. Si un prompt contradice esta frontera, prevalece este contrato y el prompt debe corregirse antes de implementar.

## 1. Propiedad de la ejecución

| Recorrido | Propietario de la secuencia | Papel de `JSProgresBar` |
|---|---|---|
| Legacy, gate apagado | Navegador y endpoints ASMX existentes | Ejecutor existente, sin cambios |
| Moderno, gate encendido | `ImportServiceOrchestrator` en backend | Adaptador de presentación de eventos/estados confirmados; no inicia efectos por elemento |

La implementación moderna no puede tener dos ejecutores. El frontend crea o solicita la ejecución de una intención y consulta/recibe su estado; únicamente el backend decide e invoca las fases mutadoras en orden secuencial.

## 2. Invariantes de coexistencia y almacenamiento

- La modernización es paralela y aditiva; no reemplaza ni redirige silenciosamente endpoints, clases o recorridos vigentes.
- `WorkflowCentroTrabajoModernActive = false` conserva íntegramente el recorrido legacy.
- `AlmacenaDocumentoTareaWorkflow(...)` y `ClassAlmacenamiento` son infraestructura existente reutilizada como caja negra.
- Ningún prompt frontend o backend autoriza modificar su firma, implementación, efectos o consumidores vigentes.
- El backend moderno adapta externamente su comando normalizado a los argumentos existentes de almacenamiento.
- “Implementación paralela” no significa ejecución concurrente: los elementos modernos se procesan secuencialmente.

## 3. Contrato canónico y bloqueo de dependencias

Backend 01 debe publicar, antes de que un consumidor frontend implemente integración real, un artefacto versionado con:

- URI o método ASMX moderno, verbo y versión de cada operación;
- DTO de solicitud y respuesta, campos requeridos/opcionales y nulabilidad;
- `schemaVersion`, `operationId`, `providerId`, `externalKey`, `taskId` y `correlationId`;
- códigos HTTP/transportes y códigos funcionales seguros;
- autorización, idempotencia, concurrencia, timeout y compatibilidad;
- ejemplos saneados y fixtures compartidos.

Operaciones lógicas mínimas:

| Operación | Mutación | Resultado principal |
|---|---:|---|
| `ResolveCapabilities` | No | proveedor, capacidades y contexto permitido |
| `QueryItems` | No | elementos externos normalizados |
| `GetPreview` | No | descriptor/stream temporal autorizado |
| `PreflightImport` | No | plan de efectos y requisitos validados |
| `CreateImportIntent` | Sí, solo intención | intención idempotente |
| `ExecuteImportIntent` | Sí | aceptación de ejecución; el servidor conserva la secuencia |
| `GetImportIntent` | No | estado global y por elemento |
| `ReconcileImportIntent` | No | resultado persistido y documentos confirmados |

Un prompt frontend queda bloqueado para integración productiva si la operación que consume aún no tiene contrato backend publicado. Puede construir UI con adaptadores falsos locales, pero no inventar respuestas productivas.

## 4. Estados y mapeo normativo

| Estado/fase backend | Estado visible frontend |
|---|---|
| Creada o Validada | Disponible/Preparando según contexto |
| RecursoObtenido, ExpedientePreparado, DocumentoAlmacenado, ÍndicesActualizados o CachéActualizado | Procesando |
| ResultadoIncierto | Verificando |
| Reconciliada o Completada con documento confirmado | Importada |
| RequiereDecision | Requiere decisión |
| FallidaAntesDePersistir | Fallida |
| Parcial | Resultado individual confirmado; resumen global Parcial |
| Detenida antes del elemento | No procesada |
| Omitida por regla confirmada | Omitida |

El frontend no deriva `Importada` de una respuesta optimista: requiere reconciliación y relación documental con la tarea original.

## 5. Propiedad de la compatibilidad legacy

- El adaptador backend ASMX es el único propietario de traducir `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` hacia o desde el resultado estructurado.
- El frontend moderno nunca interpreta esos códigos.
- `ImportarServicioWebProgressAdapter` transforma estados estructurados en presentación y eventos compatibles de UI.
- Los consumidores legacy conservan su traducción y comportamiento actuales sin modificación.

## 6. Preview mediado

`GetPreview` recibe `taskId`, `providerId`, `externalKey` y, cuando aplique, `operationId`. El backend:

- revalida usuario, tarea, proveedor e identidad externa;
- obtiene el recurso mediante el cliente tipado del proveedor;
- valida tipo, tamaño y disposición;
- devuelve stream mediado o descriptor temporal de vida corta;
- usa encabezados seguros y diferencia inline de attachment;
- nunca entrega como autoridad la URL externa, token, ruta física o respuesta cruda.

El preview es de solo lectura y no cambia tarea, estado, intención, documento, expediente, índices, caché ni auditoría funcional.

## 7. Gate compartido

El gate canónico es `WorkflowCentroTrabajoModernActive`:

- se evalúa en presentación y en la frontera de endpoints modernos;
- apagado: la UI moderna queda oculta, los endpoints modernos responden `FEATURE_DISABLED` sin efectos y el legacy continúa intacto;
- encendido para usuario/grupo autorizado: habilita exclusivamente la ruta moderna paralela;
- nunca se activa automáticamente desde código o pruebas;
- después de una corrida autorizada queda en `false`, con usuarios y grupos vacíos.

## 8. Orden cruzado obligatorio

| Etapa | Backend | Frontend desbloqueado |
|---:|---|---|
| 1 | B01 contratos/contexto/registro | F01 núcleo con integración contractual |
| 2 | B02 transporte + B06 consulta/preview SII aplicable | F02 consulta y F03 preview |
| 3 | B03 preflight/intención | F04 preparación |
| 4 | B04 orquestación | F05 progreso/presentación |
| 5 | B05 reconciliación | F06 reconciliación |
| 6 | B01+B03+B04+B05 completos | F07 contexto y recuperación |
| 7 | B07 pruebas backend + F01–F07 | F08 validación integral y gate |

Cada cambio OpenSpec debe declarar qué versión del contrato compartido consume y qué dependencia previa está verificada.

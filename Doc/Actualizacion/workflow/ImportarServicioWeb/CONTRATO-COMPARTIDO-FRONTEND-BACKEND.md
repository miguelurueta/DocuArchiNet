# Contrato compartido frontend/backend de ImportarServicioWeb

Este documento es normativo para los prompts de `Prompt/` y `PromptBackend/`. Si un prompt contradice esta frontera, prevalece este contrato y el prompt debe corregirse antes de implementar.

## 1. Propiedad de la ejecución

| Recorrido | Propietario de la secuencia | Papel de `JSProgresBar` |
|---|---|---|
| Legacy, gate apagado | Navegador y endpoints ASMX existentes | Ejecutor existente, sin cambios |
| Moderno, gate encendido | `ImportServiceOrchestrator` en backend | No participa. El feature usa un indicador indeterminado local y presenta resultados finales; no integra `JSProgresBar` |

La implementación moderna no puede tener dos ejecutores. El frontend crea o solicita la ejecución de una intención y consulta/recibe su estado; únicamente el backend decide e invoca las fases mutadoras en orden secuencial.

Para la primera entrega frontend, todos los elementos seleccionados pertenecen a una sola intención y se envían en una sola llamada síncrona a `ExecuteImportIntent`. Mientras la llamada está pendiente, la UI muestra espera global indeterminada; no inventa porcentajes ni progreso individual. Al recibir la respuesta, recorre `Items`, presenta el resultado de cada elemento y agrega a la lista documental todos los elementos confirmados, deduplicados por `DocumentId` y únicamente si `TaskId` coincide con la tarea visible.

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
| `ExecuteImportIntent` | Sí | ejecución síncrona de la intención completa y resultado estructurado por elemento |
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
- `ImportarServicioWebProgressAdapter` transforma la respuesta final y los snapshots estructurados en presentación; no presupone eventos push, progreso intermedio ni integración con `JSProgresBar`.
- Los consumidores legacy conservan su traducción y comportamiento actuales sin modificación.

## 6. Preview mediado

`GetPreview` recibe `taskId`, `providerId`, `externalKey` y, cuando aplique, `operationId`. El backend:

- revalida usuario, tarea, proveedor e identidad externa;
- obtiene el recurso mediante el cliente tipado del proveedor;
- valida tipo, tamaño y disposición;
- devuelve stream mediado o descriptor temporal de vida corta;
- usa encabezados seguros y diferencia inline de attachment;
- nunca entrega como autoridad la URL externa, token, ruta física o respuesta cruda.

`GetPreview` devuelve metadatos y un `DescriptorId` opaco. El contenido se obtiene mediante un handler de streaming separado que vuelve a validar sesión, tarea, proveedor, identidad, expiración, tipo y tamaño; nunca se transportan bytes como JSON/base64. `HEAD` no descarga contenido externo y cada consumo evita repetir innecesariamente la descarga SII.

El preview es de solo lectura y no cambia tarea, estado, intención, documento, expediente, índices, caché ni auditoría funcional.

## 6.1 Listado, catálogo y preflight enriquecidos

- `QueryItems` realiza una sola consulta SII por solicitud y enriquece localmente cada item con metadatos presentables, estado y acciones.
- El catálogo de tipologías proviene de configuración local autoritativa de tarea/trámite; nunca de SII ni del cliente.
- `PreflightImport` puede describir destino lógico, requisitos y efectos previstos, pero no expone `ExpedientId` ni afirma que los efectos físicos ya ocurrieron.
- Catálogo y preflight no realizan llamadas externas SII ni mutaciones.

## 7. Gate compartido

El gate canónico es `WorkflowCentroTrabajoModernActive`:

- se evalúa en presentación y en la frontera de endpoints modernos;
- la frontera de cada endpoint valida conjuntamente el booleano, el usuario y el grupo autorizados; el ocultamiento de la UI no reemplaza esta autorización;
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

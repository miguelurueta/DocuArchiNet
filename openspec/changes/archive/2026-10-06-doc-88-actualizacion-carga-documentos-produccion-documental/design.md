<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-88 corrige el error de referencia nula observado al cargar documentos desde Producción Documental. El recorrido atraviesa JavaScript legacy, servicios ASMX, el handler compartido y `ClassAlmacenamiento`; la evidencia estática muestra consumidores que confían en resultados parciales y una proyección visual con un campo ajeno al evento.

La implementación partió de una caracterización reproducible. La E2E real confirmó la línea exacta en el handler: `radicado_radicacion` es opcional y `PRODUCCION` no lo envía, pero se aplicaba `.Trim()` directamente sobre el valor nulo antes de invocar `UploadSaveFile`.

## Goals / Non-Goals

### Goals

- Rechazar contextos incompletos antes de desreferenciar datos.
- Conservar autorización, contratos HTTP y comportamiento de los eventos compartidos.
- Separar confirmación de persistencia de actualización visual.
- Evitar repeticiones automáticas de almacenamiento ante doble activación, error confirmado, timeout, respuesta perdida o falla visual.
- Entregar errores funcionales saneados y evidencia verificable.

### Non-Goals

- Reescribir globalmente `UploadSaveFile` o el cargador compartido.
- Cambiar DOC-85, Workflow, SII, ENLASE, versiones, PQRS o digitalización.
- Incorporar un feature gate, recarga de página, postback o espera artificial.
- Introducir scripts SQL como requisito operativo de deduplicación.
- Crear tablas, columnas, tokens persistentes o consultas de existencia para esta corrección.

## Decisions

### D-01 — Caracterización antes de corrección

Se crearán pruebas o un arnés aislado para `ServiceSolicitaCargarDocumentoExpediente`, `Service_parameter_upload("PRODUCCION")`, `FileUploadHandler_.ashx` y `PreAlmacenaDocumentoProduccion`/`AlmacenamientoDocumentoProduccionDocumental`. La implementación se limitará a la primera frontera reproducida y a defensas locales comprobables.

### D-02 — Contexto autorizado y salidas materializadas

El servidor resolverá el expediente desde la sesión autenticada y comprobará usuario, nivel, expediente, gabinete, clasificación, tipología y configuración. Antes de consumir una salida `ByRef` se validará referencia, cantidad, identidad y campos obligatorios; `YES` no constituye esa validación.

### D-03 — Compatibilidad por evento

Las firmas y campos de `UploadFilesResult` se conservan. Una modificación en el handler o su cliente compartido tendrá una guardia explícita para `PRODUCCION`; las demás ramas seguirán su recorrido actual y tendrán pruebas de invariancia.

### D-04 — Persistencia separada de proyección

El resultado persistido se confirmará antes de construir la fila. `_RegistraArchivoInterfaz` usará datos reales del resultado de producción y dejará de usar `id_imageinsert_row_documento_relacionado`. Una falla visual no autorizará otra escritura.

### D-05 — Errores funcionales saneados

Las fronteras devolverán códigos `PRODUCCION_CARGA_*` por etapa y no expondrán `ex.Message`. Los `Catch` JavaScript del recorrido leerán `ex.message`. Logs y evidencia excluirán credenciales, cookies, tokens, cadenas de conexión, contenido y rutas sensibles.

### D-06 — Resultado de almacenamiento como autoridad y no repetición automática

La corrección reutilizará el contrato existente de `Almacenamiento`: un resultado distinto de `YES` es terminal y se devuelve como rechazo saneado, sin consultar existencia ni volver a almacenar. Cuando el resultado sea `YES`, la proyección usará únicamente los identificadores ya devueltos por esa llamada. Si después del envío no puede confirmarse la respuesta, o si falla la proyección visual, el cliente informará resultado incierto o falla de proyección y no ejecutará automáticamente una segunda escritura. El doble clic se contendrá solo mientras la solicitud actual esté activa. No se agregan tabla, columna, token persistente ni lógica transversal de deduplicación.

### D-07 — Verificación integral

Se cubrirán casos válidos, contexto ausente, salida vacía, archivo ausente, éxito parcial, doble envío y otros eventos mediante caracterización local. La compilación y suites compartidas son obligatorias. DOC-88 conserva un preview autenticado no mutante y agrega una ejecución autenticada independiente: esta última requiere autorización explícita de ejecución y recurso descartable, activa `Guardar` exactamente una vez, valida la respuesta real, exige proyección visual y confirma por `SELECT` que la persistencia cambió. Un rechazo de almacenamiento es terminal y nunca provoca un segundo envío.

## Risks / Trade-offs

- Una validación global podría cambiar consumidores ajenos; se mitiga delimitando `PRODUCCION`.
- Sin una identidad persistida no puede demostrarse idempotencia fuerte entre solicitudes independientes; se acepta expresamente este límite para preservar el alcance quirúrgico y se prohíbe el reintento automático.
- Persistencia exitosa con respuesta perdida queda como resultado incierto; la aplicación no consulta existencia ni supone que deba repetir la escritura.
- Las fallas adicionales pueden ocurrir en preparación y almacenamiento; se mitigan con validaciones y códigos por etapa, mientras la causa reproducida queda corregida en la lectura segura del parámetro opcional.

## Migration Plan

1. Caracterizar el defecto sin mutar datos externos.
2. Implementar la defensa mínima en la frontera confirmada.
3. Aplicar correcciones de proyección, error y repetición automática limitadas a `PRODUCCION`.
4. Ejecutar pruebas focales, inventario de eventos, suites compartidas y compilación.
5. Ejecutar primero el preview E2E autenticado no mutante y después, con autorización mutante independiente, una carga real sobre el archivo descartable.
6. Revertir el commit focal si cualquier consumidor compartido cambia su contrato.

## Open Questions

- No queda una pregunta pendiente sobre la frontera original: fue confirmada mediante E2E real en `FileUploadHandler_.ashx.vb`.
- No queda una decisión pendiente de identidad persistente: por indicación funcional se conserva el almacenamiento existente, no se consulta si el documento existe y no se repite automáticamente una escritura con resultado fallido o incierto.

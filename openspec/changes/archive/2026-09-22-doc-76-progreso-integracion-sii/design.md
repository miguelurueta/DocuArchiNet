<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
# Diseño técnico — DOC-76

## Contexto

La preparación de DOC-75 termina con una intención creada. El backend ofrece `ExecuteImportIntent` como única operación mutadora y `GetImportIntent` como lectura. DOC-76 incorpora un adaptador de ejecución/estado y una vista accesible. El recorrido legacy queda fuera de la dependencia moderna.

## Decisiones

### D-01 — Una ejecución por intención

El adaptador acepta `IntentId` y `VersionToken`, construye un único request y mantiene una promesa en vuelo por intención para impedir doble confirmación. No itera solicitudes por elemento ni llama SII. Individual y múltiple solo difieren en la colección ya asociada a la intención.

### D-02 — Estado autoritativo por elemento

La salida contiene estado global, lista/mapa estable por `ExternalKey` y resumen. Cada item conserva los campos contractuales disponibles. La fase se mapea conforme al contrato: preparatorias/proceso → Procesando; incierto → Verificando; reconciliada/completada confirmada → Importada; decisión/falla/detenida/omitida → estado normativo. Una fase desconocida nunca se presenta como importada.

### D-03 — Vista indeterminada y resultado honesto

La vista crea DOM con listeners y atributos accesibles (`role=status`, `aria-live`). Durante la promesa solo anuncia espera global, sin valor porcentual. Al concluir, renderiza un renglón por item y conteos. Cualquier estado diferente de Importada produce resumen parcial/no exitoso.

### D-04 — Recuperación explícita, nunca polling

El flujo normal adapta directamente `ExecuteImportIntent`. Una operación separada puede efectuar una sola llamada a `GetImportIntent` únicamente por timeout, pérdida de respuesta o reapertura autorizada. No usa intervalos, timeouts de progreso ni recursión.

### D-05 — Cierre sin cancelación y sin reintento

El cierre solo desmonta u oculta presentación; no aborta fetch, no envía `StopRequested`, no revierte y no altera el resultado futuro. No se agrega “Reintentar fallidos”.

### D-06 — Aislamiento legacy y composición aditiva

Los archivos nuevos se registran en el `.vbproj` y la UI existente los compone. Los estilos se agregan solo a `Styles/importar-servicio-web-modern.css`. Ningún módulo moderno importa, copia o invoca `JSProgresBar`; tampoco interpreta códigos legacy ni toca almacenamiento.

## Contratos

- Entrada: `{ IntentId, VersionToken, ProviderId, TaskId, OperationId?, CorrelationId? }`.
- Respuesta: `IntentId`, `Status`, `VersionToken`, `Items[]` y `Error` opcional.
- Salida: snapshot con `phase`, `status`, `items`, `summary`, `isComplete` e `isTotalSuccess`.
- Vista: `renderPending`, `renderResult` y `renderFailure`; no handlers inline ni eventos ficticios.

## Riesgos y mitigaciones

- Doble clic: promesa en vuelo y control deshabilitado.
- Fase futura: clasificación conservadora, nunca éxito total.
- Pérdida de respuesta: recuperación explícita de una sola lectura.
- Cierre durante ejecución: separar vida de solicitud y vida visual.
- Regresión legacy: archivos aditivos y prueba de invariancia.

## Validación y reversión

Se ejecutarán las tres pruebas canónicas DOC-76, suites focales y build MSBuild aplicable. La validación manual comprobará individual, múltiple, cierre y resultado parcial. La reversión retira módulos, registro y estilos aditivos; no requiere migración de datos.

# crea-componente-appprogressbatch Specification

## Purpose
Define the shared AppProgressBatch UI component contract, lifecycle, cancellation, result handling, queue preview, accessibility, and validation expectations for generic sequential batch processes.
## Requirements
### Requirement: Shared AppProgressBatch Component
El sistema SHALL proveer un componente shared `AppProgressBatch` en `src/app/Components/UI/AppProgressBatch/` para orquestar procesos batch secuenciales genericos sin dependencia de dominio.

#### Scenario: Public component files exist
- **WHEN** se implementa SCRUMCORE-263
- **THEN** existen `AppProgressBatch.tsx`, `AppProgressBatch.types.ts`, `AppProgressBatch.module.css`, `AppProgressBatch.test.tsx`, `README.md` e `index.ts` dentro de `src/app/Components/UI/AppProgressBatch/`
- **AND** `src/app/Components/UI/index.ts` exporta `./AppProgressBatch`

#### Scenario: Component does not depend on business modules
- **WHEN** se revisan imports de `AppProgressBatch`
- **THEN** no importa services, endpoints, storage documental, workflow, firmas, documentos, indices, upload ni modulos consumidores
- **AND** no usa jQuery, Bootstrap manual, variables globales, funciones globales legacy ni `name_service`

### Requirement: Generic Typed Contract
`AppProgressBatch` SHALL expose a generic typed contract driven by `items` and `processItem`.

#### Scenario: Generic item processing contract
- **WHEN** un consumidor renderiza `AppProgressBatch<TItem>`
- **THEN** puede pasar `items: TItem[]`
- **AND** debe pasar `processItem(item, context)`
- **AND** `context` expone `index`, `total`, `signal`, `setCurrentLabel`, `setItemProgress` y `setPhase`

#### Scenario: Public result and summary types
- **WHEN** se importan tipos desde `src/app/Components/UI`
- **THEN** estan disponibles `AppProgressBatchLifecycle`, `AppProgressBatchItemResult`, `AppProgressBatchItemContext`, `AppProgressBatchSummary` y `AppProgressBatchProps`
- **AND** no se introduce `any` nuevo en el contrato publico

### Requirement: Controlled Modal Rendering
El componente SHALL renderizarse de forma controlada mediante `open` y `onOpenChange`, usando `AppModal` como contenedor principal.

#### Scenario: Modal opens and closes through controlled props
- **WHEN** `open` es `true`
- **THEN** el modal es visible con titulo `title ?? processName ?? "Proceso por lotes"`
- **WHEN** el usuario cierra en estado no activo
- **THEN** el componente llama `onOpenChange(false)`

#### Scenario: Shared UI components are used
- **WHEN** se renderizan acciones del batch
- **THEN** las acciones principales/secundarias usan `AppButton`
- **AND** el modal usa `AppModal`
- **AND** no se crea un modal paralelo nuevo

### Requirement: Sequential Execution
El componente SHALL procesar items en orden, uno a la vez, sin ejecuciones concurrentes.

#### Scenario: Items process in order
- **GIVEN** `items` contiene tres elementos
- **WHEN** el usuario inicia el proceso o `autoStart=true`
- **THEN** `processItem` se llama para el item 0, luego item 1, luego item 2
- **AND** nunca se ejecutan dos items simultaneamente

#### Scenario: Active run blocks duplicate start
- **GIVEN** el lifecycle es `running`, `paused` o `cancelling`
- **WHEN** el usuario intenta iniciar otra vez o cambia `items`
- **THEN** no se crea una segunda corrida
- **AND** no se llama `processItem` en paralelo

### Requirement: Lifecycle State Machine
El componente SHALL modelar explicitamente el lifecycle `idle | running | paused | cancelling | completed | error`.

#### Scenario: Successful lifecycle
- **WHEN** inicia una corrida con items validos
- **THEN** el lifecycle pasa de `idle` a `running`
- **AND** al finalizar todos los items sin cancelacion ni error fatal pasa a `completed`

#### Scenario: Controlled error lifecycle
- **WHEN** `processItem` retorna `{ status: "controlled-error", message }`
- **THEN** el lifecycle pasa a `paused`
- **AND** se muestra una decision de continuar o cancelar segun `canContinue`

#### Scenario: Fatal lifecycle
- **WHEN** `processItem` retorna `fatal-error`, lanza una excepcion o retorna un resultado invalido
- **THEN** el lifecycle pasa a `error`
- **AND** no se procesan items pendientes

### Requirement: Progress Reporting
El componente SHALL mostrar progreso global, contador, item actual, fase actual y progreso del item actual.

#### Scenario: Global progress uses processed over total
- **GIVEN** total de 4 items
- **WHEN** 2 items ya cerraron con resultado terminal
- **THEN** el progreso global representa `2 / 4 * 100`
- **AND** el contador visible indica `2 de 4` o equivalente

#### Scenario: Item progress is normalized
- **WHEN** `processItem` llama `setItemProgress(-20)`
- **THEN** el progreso visible del item se normaliza a `0`
- **WHEN** `processItem` llama `setItemProgress(140)`
- **THEN** el progreso visible del item se normaliza a `100`

#### Scenario: Phase and label can update during processing
- **WHEN** `processItem` llama `setCurrentLabel("Documento A")` y `setPhase("Validando")`
- **THEN** la UI refleja label y fase actuales sin mezclarlo con progreso global

### Requirement: Empty List Handling
El componente SHALL manejar lista vacia sin ejecutar `processItem`.

#### Scenario: Empty items show message and complete with zero summary
- **GIVEN** `items` es una lista vacia
- **WHEN** el componente esta abierto
- **THEN** no crea `AbortController`
- **AND** no llama `processItem`
- **AND** muestra `emptyMessage` o mensaje por defecto
- **AND** emite `onComplete` con `total: 0`, `processed: 0` y `cancelled: false`

### Requirement: Result Policy
El componente SHALL interpretar resultados tipados y actualizar resumen de forma consistente.

#### Scenario: Success increments success
- **WHEN** `processItem` retorna `{ status: "success" }`
- **THEN** el item cuenta como procesado
- **AND** incrementa `success`
- **AND** avanza al siguiente item

#### Scenario: Warning continues
- **WHEN** `processItem` retorna `{ status: "warning", message }`
- **THEN** el item cuenta como procesado
- **AND** incrementa `warnings`
- **AND** avanza al siguiente item sin pausar
- **AND** la advertencia queda visible en el resumen

#### Scenario: Skipped continues
- **WHEN** `processItem` retorna `{ status: "skipped", message }`
- **THEN** el item cuenta como procesado
- **AND** incrementa `skipped`
- **AND** avanza al siguiente item sin pausar

#### Scenario: Controlled error continue
- **WHEN** `processItem` retorna `controlled-error` con `canContinue !== false`
- **AND** el usuario elige continuar
- **THEN** incrementa `controlledErrors`
- **AND** avanza al siguiente item

#### Scenario: Controlled error cancel
- **WHEN** `processItem` retorna `controlled-error`
- **AND** el usuario elige cancelar
- **THEN** aborta la corrida
- **AND** no procesa items pendientes
- **AND** emite `onCancel` con resumen parcial cancelado

#### Scenario: Fatal error stops
- **WHEN** `processItem` retorna `fatal-error`
- **THEN** incrementa `fatalErrors`
- **AND** emite `onError`
- **AND** no procesa items pendientes

### Requirement: Cancellation Policy
El componente SHALL implementar cancelacion segura mediante `AbortController`.

#### Scenario: Cancel during running aborts active signal
- **GIVEN** el lifecycle es `running`
- **WHEN** el usuario cancela
- **THEN** el componente pasa a `cancelling`
- **AND** llama `abortController.abort()`
- **AND** emite `onCancel(summary)`
- **AND** no emite `onComplete` como exito total

#### Scenario: Close during active process follows cancellation policy
- **GIVEN** el lifecycle es `running` o `paused`
- **WHEN** el usuario intenta cerrar el modal
- **THEN** si `confirmOnCancel=true`, se pide confirmacion antes de cancelar
- **AND** si `confirmOnCancel=false`, se cancela directamente
- **AND** nunca se cierra silenciosamente perdiendo estado

### Requirement: Stale Run Protection
El componente SHALL aislar cada corrida con un `runId` interno e ignorar resultados tardios.

#### Scenario: Late result after cancellation is ignored
- **GIVEN** una corrida activa fue cancelada
- **WHEN** `processItem` resuelve tarde
- **THEN** ese resultado no actualiza lifecycle, resumen ni UI de la corrida vigente

#### Scenario: Unmount cleanup prevents state update
- **GIVEN** una corrida activa
- **WHEN** el componente se desmonta
- **THEN** invalida la corrida activa
- **AND** limpia el `AbortController`
- **AND** no actualiza UI despues de unmount

### Requirement: AutoStart Policy
El componente SHALL soportar `autoStart` sin duplicar ejecuciones.

#### Scenario: AutoStart starts once per opening
- **GIVEN** `autoStart=true`, `open=true` e items disponibles
- **WHEN** el componente se renderiza o re-renderiza
- **THEN** inicia una sola corrida
- **AND** React StrictMode no duplica `processItem`

#### Scenario: Manual start when autoStart is false
- **GIVEN** `autoStart=false`
- **WHEN** el componente esta `idle` con items
- **THEN** muestra accion `Iniciar`
- **AND** no llama `processItem` hasta que el usuario inicia

### Requirement: Enterprise UI and Accessibility
La UI SHALL mantener composicion enterprise estable, accesible y responsive dentro del modal.

#### Scenario: Footer actions match lifecycle
- **WHEN** lifecycle es `idle` con items
- **THEN** muestra `Iniciar` y `Cerrar`
- **WHEN** lifecycle es `running`
- **THEN** muestra `Cancelar`
- **WHEN** lifecycle es `paused`
- **THEN** muestra `Continuar` si aplica y `Cancelar`
- **WHEN** lifecycle es `cancelling`
- **THEN** acciones quedan deshabilitadas o en loading
- **WHEN** lifecycle es `completed` o `error`
- **THEN** muestra `Cerrar`

#### Scenario: Progress and messages are accessible
- **WHEN** se muestran barras de progreso, errores, advertencias o lista vacia
- **THEN** existen textos visibles o labels accesibles equivalentes
- **AND** el usuario no depende solo del color para entender el estado

#### Scenario: Long labels do not break layout
- **WHEN** el label del item actual es largo
- **THEN** la UI aplica wrap controlado o truncado con `title`/tooltip equivalente
- **AND** el modal no cambia bruscamente de alto entre fases

### Requirement: Documentation and Validation
La entrega SHALL documentar el componente, evidencias, limites de alcance y documentacion enterprise del ticket.

#### Scenario: README documents usage
- **WHEN** se revisa `src/app/Components/UI/AppProgressBatch/README.md`
- **THEN** incluye objetivo, props, tipos, ejemplo basico, ejemplo con error controlado, ejemplo con cancelacion, limites de dominio y relacion futura con `AppUploadDocumental`

#### Scenario: Enterprise architecture documentation exists
- **WHEN** se revisa `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Arquitectura.md`
- **THEN** documenta objetivo, contexto legacy, alcance/no alcance, dependencias permitidas, separacion de estados, lifecycle, cancelacion, stale results, seguridad, restricciones y decisiones arquitectonicas

#### Scenario: Enterprise implementation documentation exists
- **WHEN** se revisa `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Implementacion-Detallada.md`
- **THEN** documenta estructura de archivos, API publica, tipos, defaults, flujo de ejecucion, politica de resultados, lista vacia, autoStart, mutex, guard runtime, uso de `AppModal`/`AppButton`, estados visuales y ejemplos de uso

#### Scenario: Enterprise testing documentation exists
- **WHEN** se revisa `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Pruebas.md`
- **THEN** contiene matriz de pruebas unitarias e integracion, comandos ejecutados, resultados, evidencia de validacion, pruebas manuales recomendadas y riesgos no cubiertos

#### Scenario: Enterprise metadata documentation exists
- **WHEN** se revisa `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Metadata.md`
- **THEN** contiene Jira, rama, commits, PR futuro, archivos creados/modificados, estado de tareas, decisiones, riesgos residuales y confirmaciones de no backend, no endpoints, no dominio, no `any`, cancelacion con `AbortController` y stale results ignorados

#### Scenario: Verification evidence is recorded
- **WHEN** se cierre el ticket
- **THEN** se registran comandos ejecutados y resultado de tests
- **AND** se reporta explicitamente si build o lint no se ejecutan
- **AND** se confirma que backend, endpoints y consumidores no fueron modificados

## 1. Refinement

- [x] 1.1 Consultar Jira mediante `opsxj:new SCRUMCORE-263`.
- [x] 1.2 Crear artefactos OpenSpec iniciales desde Jira.
- [x] 1.3 Revisar contexto arquitectonico existente de `docs/Architecture/AppProgressBatch`.
- [x] 1.4 Revisar contratos actuales de `AppModal`, `AppButton` y barrel shared `src/app/Components/UI/index.ts`.
- [x] 1.5 Corregir nombres generados automaticamente: usar `AppProgressBatch`, no `AppAppprogressbatch`.
- [x] 1.6 Refinar `proposal.md`, `design.md`, `spec.md` y `tasks.md` antes de publish.
- [x] 1.7 Validar OpenSpec refinado con `npx.cmd openspec validate scrumcore-263-crea-componente-appprogressbatch --strict`.

## 2. Arquitectura y estructura

- [ ] 2.1 Crear carpeta `src/app/Components/UI/AppProgressBatch/`.
- [ ] 2.2 Crear `AppProgressBatch.types.ts` con tipos publicos requeridos.
- [ ] 2.3 Crear `AppProgressBatch.tsx`.
- [ ] 2.4 Crear `AppProgressBatch.module.css`.
- [ ] 2.5 Crear `AppProgressBatch.test.tsx`.
- [ ] 2.6 Crear `README.md`.
- [ ] 2.7 Crear `index.ts` local.
- [ ] 2.8 Actualizar `src/app/Components/UI/index.ts` con `export * from "./AppProgressBatch";`.
- [ ] 2.9 Mantener utilidades privadas dentro de la carpeta si se requieren, sin exportarlas desde el barrel publico.
- [ ] 2.10 No crear ni modificar services, endpoints, backend ni modulos consumidores.

## 3. Contrato publico y tipos

- [ ] 3.1 Definir `AppProgressBatchLifecycle = "idle" | "running" | "paused" | "cancelling" | "completed" | "error"`.
- [ ] 3.2 Definir `AppProgressBatchItemResult` con `success`, `warning`, `skipped`, `controlled-error`, `fatal-error`.
- [ ] 3.3 Definir `AppProgressBatchSummary` con total, processed, success, warnings, skipped, controlledErrors, fatalErrors y cancelled.
- [ ] 3.4 Definir `AppProgressBatchItemContext` con `index`, `total`, `signal`, `setCurrentLabel`, `setItemProgress`, `setPhase`.
- [ ] 3.5 Definir `AppProgressBatchProps<TItem>` con contrato generico, callbacks y opciones requeridas.
- [ ] 3.6 No introducir `any`; usar genericos y `unknown` donde aplique.
- [ ] 3.7 Exportar componente y tipos desde `AppProgressBatch/index.ts`.

## 4. Maquina de ejecucion

- [ ] 4.1 Implementar estado interno separado de UI/execution/summary.
- [ ] 4.2 Implementar defaults centralizados: titulo, mensaje lista vacia, mensaje confirmacion cancelacion y mensajes base.
- [ ] 4.3 Implementar `runId` interno por corrida.
- [ ] 4.4 Implementar `AbortController` por corrida y cleanup en cierre/unmount.
- [ ] 4.5 Implementar mutex para impedir doble ejecucion con `running`, `paused` o `cancelling`.
- [ ] 4.6 Implementar inicio manual cuando `autoStart=false`.
- [ ] 4.7 Implementar `autoStart=true` una sola vez por apertura e items disponibles, cuidando StrictMode.
- [ ] 4.8 Congelar la corrida activa para que cambios de `items` no inicien batch paralelo.
- [ ] 4.9 Ignorar resultados stale cuando `runId` no coincide.

## 5. Politica de procesamiento

- [ ] 5.1 Procesar items secuencialmente en orden.
- [ ] 5.2 Construir `AppProgressBatchItemContext` por item.
- [ ] 5.3 Permitir actualizacion de label con `setCurrentLabel`.
- [ ] 5.4 Permitir actualizacion de fase con `setPhase`.
- [ ] 5.5 Permitir actualizacion de progreso del item con `setItemProgress`.
- [ ] 5.6 Normalizar progreso de item a rango 0-100.
- [ ] 5.7 Calcular progreso global con `processed / total * 100`.
- [ ] 5.8 Mantener separado `itemPercent` de `globalPercent`.
- [ ] 5.9 Implementar `getItemLabel` para label inicial.

## 6. Resultados y errores

- [ ] 6.1 Implementar `success`: procesado + success + avanzar.
- [ ] 6.2 Implementar `warning`: procesado + warnings + avanzar sin pausa.
- [ ] 6.3 Implementar `skipped`: procesado + skipped + avanzar sin pausa.
- [ ] 6.4 Implementar `controlled-error`: pasar a `paused` y mostrar decision.
- [ ] 6.5 Implementar continuar despues de `controlled-error`: incrementar controlledErrors y avanzar.
- [ ] 6.6 Implementar cancelar despues de `controlled-error`: abortar y emitir `onCancel`.
- [ ] 6.7 Implementar `fatal-error`: incrementar fatalErrors, pasar a `error`, emitir `onError` y detener.
- [ ] 6.8 Tratar excepciones de `processItem` como error fatal.
- [ ] 6.9 Crear guard privado `isValidBatchItemResult(value: unknown)`.
- [ ] 6.10 Tratar resultado invalido como fatal error y emitir `onError`.
- [ ] 6.11 Mantener resumen parcial consistente en error/cancelacion.

## 7. Cancelacion y cierre

- [ ] 7.1 Implementar cancelacion desde boton `Cancelar`.
- [ ] 7.2 Llamar `abortController.abort()` al cancelar.
- [ ] 7.3 Detener items pendientes despues de cancelar.
- [ ] 7.4 Emitir `onCancel(summary)` sin emitir `onComplete` como exito total.
- [ ] 7.5 Aplicar la misma politica cuando se intenta cerrar el modal durante `running` o `paused`.
- [ ] 7.6 Si `confirmOnCancel=true`, pedir confirmacion antes de cancelar.
- [ ] 7.7 Si `confirmOnCancel=false`, cancelar directamente.
- [ ] 7.8 No cerrar silenciosamente durante ejecucion.
- [ ] 7.9 Si se cancela durante `paused`, aplicar el mismo flujo de cancelacion.

## 8. Lista vacia y finalizacion

- [ ] 8.1 Si `items.length === 0`, no crear `AbortController`.
- [ ] 8.2 Si `items.length === 0`, no llamar `processItem`.
- [ ] 8.3 Mostrar `emptyMessage` o default.
- [ ] 8.4 Emitir `onComplete` con resumen total cero.
- [ ] 8.5 Al completar todos los items, pasar a `completed`.
- [ ] 8.6 Emitir `onComplete(summary)`.
- [ ] 8.7 Si `closeOnComplete=true`, llamar `onOpenChange(false)` despues de `onComplete`.
- [ ] 8.8 No ocultar errores ni cancelaciones con `closeOnComplete`.

## 9. UI y accesibilidad

- [ ] 9.1 Usar `AppModal` con `maskClosable={false}`.
- [ ] 9.2 Usar `hideFooter` y footer custom interno si se requieren multiples acciones por lifecycle.
- [ ] 9.3 Usar `AppButton` para iniciar, cancelar, continuar, cerrar y confirmar cancelacion.
- [ ] 9.4 Usar `Progress` de Ant Design para progreso global e item actual si no hay wrapper local.
- [ ] 9.5 Usar `Alert` de Ant Design para errores, advertencias y lista vacia si no hay wrapper local.
- [ ] 9.6 Mostrar titulo, nombre de proceso, item actual, fase, contador y barras de progreso.
- [ ] 9.7 Renderizar footer por lifecycle: idle, running, paused, cancelling, completed, error.
- [ ] 9.8 Garantizar textos/labels accesibles para barras y mensajes.
- [ ] 9.9 Manejar labels largos con wrap/truncado y `title`/tooltip.
- [ ] 9.10 Mantener composicion visual estable sin saltos bruscos de alto.
- [ ] 9.11 Respetar cierre por teclado bajo la politica de cancelacion.

## 10. Documentacion

- [ ] 10.1 Crear `src/app/Components/UI/AppProgressBatch/README.md`.
- [ ] 10.2 Documentar objetivo y limites de dominio.
- [ ] 10.3 Documentar props y tipos publicos.
- [ ] 10.4 Agregar ejemplo basico.
- [ ] 10.5 Agregar ejemplo con error controlado.
- [ ] 10.6 Agregar ejemplo con cancelacion.
- [ ] 10.7 Documentar relacion futura con `AppUploadDocumental` sin integrarlo en este ticket.
- [ ] 10.8 Mantener alineacion con `docs/Architecture/AppProgressBatch/AppProgressBatch-Requisitos.md`.
- [ ] 10.9 Mantener alineacion con `docs/Architecture/AppProgressBatch/Legacy-Gap-Analysis.md`.
- [ ] 10.10 Crear `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Arquitectura.md`.
- [ ] 10.11 En arquitectura enterprise, documentar objetivo, contexto legacy, alcance/no alcance y dependencias permitidas.
- [ ] 10.12 En arquitectura enterprise, documentar separacion UI state, execution state y consumer state.
- [ ] 10.13 En arquitectura enterprise, documentar lifecycle, run isolation, cancelacion, stale results, seguridad y restricciones.
- [ ] 10.14 Crear `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Implementacion-Detallada.md`.
- [ ] 10.15 En implementacion detallada, documentar estructura de archivos, API publica, tipos, defaults y exports.
- [ ] 10.16 En implementacion detallada, documentar flujo de ejecucion, politica de resultados, errores, lista vacia, autoStart, mutex y guard runtime.
- [ ] 10.17 En implementacion detallada, documentar uso de `AppModal`, `AppButton`, `Progress`, `Alert`, estados visuales y ejemplos de uso.
- [ ] 10.18 Crear `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Pruebas.md`.
- [ ] 10.19 En pruebas enterprise, documentar matriz de casos unitarios/integracion, comandos ejecutados, resultados y evidencia.
- [ ] 10.20 En pruebas enterprise, documentar pruebas manuales recomendadas, build/lint si aplica y riesgos no cubiertos.
- [ ] 10.21 Crear `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Metadata.md`.
- [ ] 10.22 En metadata enterprise, documentar Jira, rama, commits, PR futuro, archivos creados/modificados, estado de tareas y decisiones.
- [ ] 10.23 En metadata enterprise, documentar riesgos residuales y confirmaciones de no backend, no endpoints, no dominio, no `any`, cancelacion con `AbortController` y stale results ignorados.

## 11. Pruebas unitarias e integracion

- [ ] 11.1 Test: render controlado con `open`.
- [ ] 11.2 Test: usa `AppModal` y respeta cierre.
- [ ] 11.3 Test: lista vacia no llama `processItem`.
- [ ] 11.4 Test: muestra `emptyMessage`.
- [ ] 11.5 Test: `autoStart=true` inicia una sola vez.
- [ ] 11.6 Test: `autoStart=false` no inicia automaticamente.
- [ ] 11.7 Test: ejecuta items en orden.
- [ ] 11.8 Test: `success` incrementa exitos.
- [ ] 11.9 Test: `warning` incrementa advertencias y continua.
- [ ] 11.10 Test: `skipped` incrementa omitidos y continua.
- [ ] 11.11 Test: `controlled-error` pausa.
- [ ] 11.12 Test: continuar despues de `controlled-error` procesa siguiente item.
- [ ] 11.13 Test: cancelar despues de `controlled-error` no procesa pendientes.
- [ ] 11.14 Test: `fatal-error` detiene y emite `onError`.
- [ ] 11.15 Test: excepcion de `processItem` detiene y emite `onError`.
- [ ] 11.16 Test: cancelacion llama `AbortController.abort`.
- [ ] 11.17 Test: resultados tardios tras cancelacion no actualizan resumen activo.
- [ ] 11.18 Test: `setItemProgress` normaliza menores a 0 y mayores a 100.
- [ ] 11.19 Test: `getItemLabel` define label inicial.
- [ ] 11.20 Test: resultado invalido de `processItem` se trata como fatal.
- [ ] 11.21 Test: exports publicos desde `index.ts`.
- [ ] 11.22 Test integracion: proceso completo de 3 items exitosos.
- [ ] 11.23 Test integracion: mezcla `success`, `warning`, `skipped`.
- [ ] 11.24 Test integracion: cierre durante ejecucion con `confirmOnCancel`.
- [ ] 11.25 Test integracion: `closeOnComplete` cierra despues del final exitoso.

## 12. Validacion y cierre

- [ ] 12.1 Ejecutar `npm test -- src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx` o equivalente Vitest.
- [ ] 12.2 Ejecutar `npx.cmd tsc --noEmit --pretty false`.
- [ ] 12.3 Ejecutar `npx.cmd openspec validate scrumcore-263-crea-componente-appprogressbatch --strict`.
- [ ] 12.4 Ejecutar `git diff --check`.
- [ ] 12.5 Ejecutar build/lint si el alcance y tiempo lo permiten; si no, documentarlo explicitamente.
- [ ] 12.6 Confirmar backend NO modificado.
- [ ] 12.7 Confirmar endpoints NO modificados.
- [ ] 12.8 Confirmar consumidores de negocio NO modificados.
- [ ] 12.9 Confirmar `AppUpload` NO modificado.
- [ ] 12.10 Confirmar `any` nuevo NO introducido.
- [ ] 12.11 Confirmar cancelacion con `AbortController`.
- [ ] 12.12 Confirmar resultados stale ignorados.
- [ ] 12.13 Commit de refinamiento/publish de artefactos OpenSpec.
- [ ] 12.14 Push de `feature/SCRUMCORE-263`.

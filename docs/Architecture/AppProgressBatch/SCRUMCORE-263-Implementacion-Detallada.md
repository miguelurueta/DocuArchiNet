# SCRUMCORE-263 - Implementacion Detallada

## Estructura

```txt
src/app/Components/UI/AppProgressBatch/
├─ AppProgressBatch.tsx
├─ AppProgressBatch.types.ts
├─ AppProgressBatch.module.css
├─ AppProgressBatch.test.tsx
├─ README.md
└─ index.ts
```

Tambien se actualiza:

```txt
src/app/Components/UI/index.ts
```

## API publica

El consumidor importa:

```ts
import { AppProgressBatch } from "src/app/Components/UI";
```

Tipos exportados:

- `AppProgressBatchLifecycle`
- `AppProgressBatchItemResult`
- `AppProgressBatchItemContext`
- `AppProgressBatchSummary`
- `AppProgressBatchProps`

## Defaults

- `DEFAULT_TITLE = "Proceso por lotes"`
- `DEFAULT_EMPTY_MESSAGE = "No hay elementos para procesar."`
- `DEFAULT_CANCEL_CONFIRM_MESSAGE = "Hay un proceso en curso. Desea cancelarlo?"`
- Mensajes base para preparacion, completado, cancelado, fatal e invalid result.

## Flujo de ejecucion

1. El consumidor abre el modal.
2. Si no hay items, se muestra mensaje y se emite resumen cero.
3. Si hay items y el usuario inicia o `autoStart=true`, se crea `runId` y `AbortController`.
4. Se congela la lista activa para evitar reentradas por cambios de props.
5. Se procesa un item a la vez.
6. Se entrega contexto con `index`, `total`, `signal`, `setCurrentLabel`, `setItemProgress` y `setPhase`.
7. Se valida el resultado runtime.
8. Se actualiza resumen y progreso.
9. Se completa, pausa, cancela o detiene segun resultado.

## Politica de resultados

- `success`: incrementa `processed` y `success`.
- `warning`: incrementa `processed` y `warnings`; continua.
- `skipped`: incrementa `processed` y `skipped`; continua.
- `controlled-error`: pasa a `paused`; continuar incrementa `processed` y `controlledErrors`; cancelar emite resumen cancelado.
- `fatal-error`: incrementa `fatalErrors`, pasa a `error`, emite `onError` y detiene.

## Errores

`processItem` puede fallar de tres formas:

- lanzar excepcion;
- retornar `fatal-error`;
- retornar resultado invalido.

Los tres casos detienen la corrida y emiten `onError`. El resumen parcial se conserva.

## Lista vacia

Lista vacia no crea `AbortController`, no llama `processItem` y emite:

```ts
{
  total: 0,
  processed: 0,
  success: 0,
  warnings: 0,
  skipped: 0,
  controlledErrors: 0,
  fatalErrors: 0,
  cancelled: false,
}
```

## AutoStart y mutex

`autoStart=true` inicia una sola vez por apertura con items disponibles. Un ref interno evita duplicacion por rerenders o StrictMode. Mientras el lifecycle sea `running`, `paused` o `cancelling`, no se inicia otra corrida.

## Guard runtime

`isValidBatchItemResult(value: unknown)` protege al componente de consumidores que retornen formas no validas. Un resultado invalido se convierte en error fatal operacional.

## UI

`AppProgressBatch` usa:

- `AppModal` con `maskClosable={false}` y `hideFooter`.
- Footer custom interno por lifecycle.
- `AppButton` para iniciar, cancelar, continuar, cerrar y confirmar.
- `Progress` de Ant Design para progreso global e item.
- `Alert` de Ant Design para mensajes y errores.

## Estados visuales

- `idle`: muestra accion iniciar y cerrar.
- `running`: muestra item actual, fase, progreso y cancelar.
- `paused`: muestra error controlado con continuar/cancelar.
- `cancelling`: muestra accion de cancelacion en curso.
- `completed`: muestra resumen final.
- `error`: muestra error fatal y resumen parcial.

## Ejemplo resumido

```tsx
<AppProgressBatch
  open={open}
  items={items}
  onOpenChange={setOpen}
  processItem={async (item, context) => {
    context.setPhase("Procesando");
    context.setItemProgress(50);
    await runOperation(item, context.signal);
    context.setItemProgress(100);
    return { status: "success" };
  }}
/>
```

## Preview de items en cola

Antes de iniciar, `AppProgressBatch` muestra un preview generico de los items en cola.

El preview:

- usa `getItemLabel` si el consumidor lo provee;
- usa labels genericos `Elemento N` si no existe `getItemLabel`;
- muestra hasta seis items visibles;
- resume elementos adicionales con un contador `+N elementos adicionales`;
- no conoce dominio documental, upload ni metadatos de negocio.

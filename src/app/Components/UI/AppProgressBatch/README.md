# AppProgressBatch

`AppProgressBatch` es un componente shared para orquestar procesos batch secuenciales desde UI React. Recibe una lista generica de items y delega la operacion concreta al consumidor mediante `processItem(item, context)`.

No conoce dominio de negocio, no llama endpoints, no importa services y no reemplaza logicamente a ningun modulo consumidor. Su responsabilidad es presentar progreso, ciclo de vida, cancelacion segura y resumen operacional.

## Archivos

```txt
src/app/Components/UI/AppProgressBatch/
├─ AppProgressBatch.tsx
├─ AppProgressBatch.types.ts
├─ AppProgressBatch.module.css
├─ AppProgressBatch.test.tsx
├─ README.md
└─ index.ts
```

## Props principales

- `open`: controla visibilidad del modal.
- `items`: lista generica de elementos a procesar.
- `onOpenChange`: callback de apertura/cierre.
- `processItem`: funcion async inyectada por el consumidor.
- `title`: titulo del modal.
- `processName`: nombre funcional del proceso.
- `autoStart`: inicia automaticamente una sola vez por apertura.
- `confirmOnCancel`: exige confirmacion al cancelar o cerrar durante ejecucion.
- `emptyMessage`: mensaje para lista vacia.
- `closeOnComplete`: cierra al completar exitosamente.
- `getItemLabel`: resuelve label inicial del item actual y los labels del preview de items en cola.
- `onComplete`: recibe resumen final.
- `onCancel`: recibe resumen parcial cancelado.
- `onError`: recibe errores fatales o resultados invalidos.

## Tipos publicos

El barrel local y el barrel shared exportan:

```ts
AppProgressBatch
AppProgressBatchLifecycle
AppProgressBatchItemResult
AppProgressBatchItemContext
AppProgressBatchSummary
AppProgressBatchProps
```

## Ejemplo basico

```tsx
<AppProgressBatch
  open={open}
  items={documentos}
  title="Procesar documentos"
  processName="Validacion documental"
  onOpenChange={setOpen}
  getItemLabel={(documento) => documento.nombre}
  processItem={async (documento, context) => {
    context.setPhase("Validando");
    context.setItemProgress(40);

    await validarDocumento(documento, context.signal);

    context.setPhase("Finalizando");
    context.setItemProgress(100);

    return { status: "success" };
  }}
  onComplete={(summary) => {
    console.info(summary);
  }}
/>
```

## Error controlado

```tsx
processItem={async (item, context) => {
  context.setPhase("Validando reglas");

  if (item.requiereRevision) {
    return {
      status: "controlled-error",
      message: "El item requiere revision manual antes de continuar.",
      canContinue: true,
    };
  }

  return { status: "success" };
}}
```

Cuando retorna `controlled-error`, el componente pasa a `paused`. El usuario puede continuar o cancelar. Si continua, el item cuenta como procesado con `controlledErrors + 1`.

## Cancelacion

```tsx
processItem={async (item, context) => {
  const response = await fetch(item.url, { signal: context.signal });

  if (!response.ok) {
    return { status: "fatal-error", message: "No fue posible procesar el item." };
  }

  return { status: "success" };
}}
```

La cancelacion llama `AbortController.abort()` y entrega la senal al consumidor. El componente no asume rollback de backend ni confirma efectos de negocio.

## Resultados soportados

- `success`: procesado exitoso.
- `warning`: procesado con advertencia no bloqueante.
- `skipped`: procesado omitido.
- `controlled-error`: pausa y pide decision.
- `fatal-error`: detiene el batch y emite `onError`.

## Relacion con AppUploadDocumental

Este ticket no integra `AppProgressBatch` con `AppUploadDocumental`. La relacion esperada es futura: `AppUploadDocumental` u otro consumidor podria inyectar su propio `processItem` para operaciones de carga, validacion o reemplazo documental, manteniendo toda la logica de negocio fuera del componente shared.

## Preview de items en cola

Antes de iniciar, el modal muestra una vista previa compacta de los items en cola usando `getItemLabel`. Esa lista es generica del componente shared y no conoce metadatos documentales, upload ni servicios de negocio.

## Limites

- No ejecuta APIs por si mismo.
- No persiste datos.
- No conoce documentos, workflow, firmas, indices ni upload.
- No usa `any`, jQuery, Bootstrap manual, variables globales ni IDs DOM fijos.
- No procesa items en paralelo.

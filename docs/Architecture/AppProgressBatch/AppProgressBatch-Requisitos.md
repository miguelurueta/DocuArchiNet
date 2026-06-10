# Modelo de requisitos: AppProgressBatch

## Objetivo

Definir un componente reutilizable para ejecutar procesos batch secuenciales con progreso global, cancelacion y manejo de errores controlados. El componente no debe conocer dominios concretos como carga de archivos, workflow, firmas, indices o documentos.

## Contexto legacy

El sistema legacy usa `JSProgresBar.js` como orquestador visual de procesos por lote. Ese archivo:

- recibe una lista de items en `OptionItemSelect`;
- muestra un modal con barra de progreso;
- procesa item por item;
- actualiza porcentaje, contador y nombre del proceso;
- permite cancelar;
- permite continuar o cancelar ante errores controlados;
- invoca operaciones concretas por `name_service`.

La migracion debe conservar el comportamiento util, pero eliminar el acoplamiento a strings de servicio, funciones globales, jQuery, Bootstrap manual y estado global.

## Alcance

Incluye:

- componente UI reusable;
- ejecucion secuencial de items;
- progreso global `actual / total`;
- estado de elemento actual;
- cancelacion por `AbortController`;
- errores controlados con opcion continuar/cancelar;
- resumen final;
- API generica basada en `processItem`.

No incluye:

- subida de archivos;
- conocimiento de endpoints;
- reglas de negocio documental;
- validacion de extensiones o tamanos;
- carga de tipologias;
- registro en almacenamiento documental.

## Ubicacion propuesta

```txt
src/app/Components/UI/AppProgressBatch/
├─ AppProgressBatch.tsx
├─ AppProgressBatch.types.ts
├─ AppProgressBatch.module.css
├─ AppProgressBatch.test.tsx
└─ index.ts
```

## Contrato propuesto

```ts
export type AppProgressBatchItemResult =
  | { status: "success" }
  | { status: "skipped"; message?: string }
  | { status: "warning"; message: string }
  | { status: "controlled-error"; message: string; canContinue?: boolean }
  | { status: "fatal-error"; message: string };

export type AppProgressBatchSummary = {
  total: number;
  processed: number;
  success: number;
  skipped: number;
  warnings: number;
  controlledErrors: number;
  fatalErrors: number;
  cancelled: boolean;
};

export type AppProgressBatchProps<TItem> = {
  open: boolean;
  title?: string;
  processName?: string;
  items: TItem[];
  autoStart?: boolean;
  confirmOnCancel?: boolean;
  emptyMessage?: string;
  closeOnComplete?: boolean;
  onOpenChange: (open: boolean) => void;
  processItem: (
    item: TItem,
    context: {
      index: number;
      total: number;
      signal: AbortSignal;
      setCurrentLabel: (label: string) => void;
      setItemProgress: (percent: number) => void;
    },
  ) => Promise<AppProgressBatchItemResult>;
  onComplete?: (summary: AppProgressBatchSummary) => void;
  onCancel?: (summary: AppProgressBatchSummary) => void;
  onError?: (error: unknown) => void;
};
```

## Requisitos funcionales

### RF-PB-01 Renderizado controlado

El componente debe abrirse y cerrarse mediante props controladas `open` y `onOpenChange`.

### RF-PB-02 Procesamiento secuencial

El componente debe procesar los items en orden, uno a la vez, usando la funcion `processItem`.

### RF-PB-02A Lista vacia

Si `items` esta vacio, el componente no debe iniciar procesamiento y debe reportar un mensaje configurable mediante `emptyMessage`.

Equivalencia legacy:

```txt
No has seleccionado todos los items.
```

### RF-PB-03 Progreso global

El componente debe mostrar:

- porcentaje global;
- contador `x de y`;
- nombre del proceso;
- etiqueta del item actual.

### RF-PB-04 Progreso interno de item

El componente debe permitir que `processItem` reporte progreso interno del item actual mediante `setItemProgress`.

### RF-PB-05 Cancelacion

El componente debe permitir cancelar el proceso batch. Al cancelar:

- debe abortar el item en curso mediante `AbortSignal`;
- debe detener los items pendientes;
- debe emitir `onCancel`;
- debe cerrar o mantenerse abierto segun definicion UX final.

### RF-PB-06 Confirmacion de cancelacion

Cuando `confirmOnCancel` este habilitado, el componente debe pedir confirmacion antes de cancelar.

### RF-PB-07 Errores controlados

Si `processItem` retorna `controlled-error`, el componente debe:

- registrar el error;
- mostrar el mensaje;
- permitir continuar cuando `canContinue` sea verdadero;
- permitir cancelar el batch.

### RF-PB-07A Advertencias no bloqueantes

Si `processItem` retorna `warning`, el componente debe registrar la advertencia y continuar con el siguiente item sin pedir confirmacion.

Equivalencia legacy:

```txt
CTRL
```

### RF-PB-07B Items omitidos

Si `processItem` retorna `skipped`, el componente debe contar el item como procesado no exitoso y continuar.

Este estado cubre casos donde el item no se puede procesar pero no debe detener el lote.

### RF-PB-08 Errores fatales

Si `processItem` retorna `fatal-error` o lanza una excepcion no controlada, el componente debe detener el proceso y emitir `onError`.

### RF-PB-09 Resumen final

Al finalizar debe emitir `onComplete` con:

- total;
- procesados;
- exitosos;
- omitidos;
- advertencias;
- errores controlados;
- errores fatales;
- estado de cancelacion.

### RF-PB-10 Reutilizacion

El componente debe funcionar con cualquier tipo de item generico `TItem`.

### RF-PB-11 Estados de ciclo de vida

El componente debe modelar explicitamente su ciclo de vida:

- `idle`;
- `running`;
- `paused`;
- `cancelling`;
- `completed`;
- `error`.

Esto evita depender de banderas numericas como `estado_control`.

### RF-PB-12 Bloqueo de cierre durante ejecucion

Cuando el proceso este en estado `running` o `paused`, el cierre debe pasar por la misma politica de cancelacion configurada para el boton cancelar.

### RF-PB-13 Etiqueta de fase del item

`processItem` debe poder actualizar una etiqueta de fase del item actual, por ejemplo `Cargando`, `Guardando`, `Validando` o `Completando`.

Esto permite migrar la semantica visual legacy sin acoplarla a uploads.

## Requisitos no funcionales

### RNF-PB-01 Independencia de dominio

No debe importar servicios de almacenamiento, workflow, documentos, firmas ni modulos de negocio.

### RNF-PB-02 Sin dependencias legacy

No debe usar jQuery, Bootstrap manual, variables globales ni HTML construido por strings.

### RNF-PB-03 Accesibilidad

Debe usar componentes accesibles del sistema o Ant Design para dialogos, botones y progreso.

### RNF-PB-04 Testabilidad

Debe tener pruebas para:

- procesamiento secuencial;
- cancelacion;
- error controlado con continuar;
- error fatal;
- advertencia no bloqueante;
- item omitido;
- lista vacia;
- resumen final.

### RNF-PB-05 Estabilidad visual

La UI no debe depender del largo del texto para cambiar de tamano de forma abrupta. Los textos largos deben truncarse o envolver sin romper layout.

## Equivalencia legacy

```txt
JSProgresBar.LoadJSProgresBar
→ AppProgressBatch open + autoStart

_GeneraProcesingProgres
→ processItem secuencial

_SetPorcentProgres
→ estado interno de progreso

estado_control = 0
→ cancelacion con AbortController

estado_control = 2
→ pausa por confirmacion

CTRL / CTRLRETURN
→ warning / controlled-error

name_service
→ NO migra; se reemplaza por processItem

OptionItemSelect.length == 0
→ emptyMessage y no iniciar proceso

estado_control = 1
→ running

estado_control = 2
→ paused
```

## Casos de uso previstos

- carga documental por lote;
- eliminacion de documentos;
- actualizacion de indices;
- firma digital batch;
- vinculacion de documentos a expediente;
- procesos administrativos secuenciales.

## Criterios de aceptacion

- Dado un arreglo de 3 items, cuando inicia el proceso, entonces ejecuta `processItem` 3 veces en orden.
- Dado un item con error controlado y `canContinue=true`, cuando el usuario elige continuar, entonces el batch procesa el siguiente item.
- Dado un item con error fatal, cuando ocurre el error, entonces el batch se detiene y emite `onError`.
- Dado un item con `warning`, cuando se procesa, entonces el batch registra la advertencia y continua.
- Dado un item `skipped`, cuando se procesa, entonces el batch lo cuenta como omitido y continua.
- Dado un arreglo vacio, cuando inicia el proceso, entonces no llama `processItem` y muestra `emptyMessage`.
- Dado un proceso en curso, cuando el usuario cancela, entonces no se procesan items pendientes y se emite `onCancel`.
- Dado cualquier tipo de item, el componente no requiere campos especificos del dominio.

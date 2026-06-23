## Context

SCRUMCORE-263 crea `AppProgressBatch`, un componente shared para ejecutar procesos batch secuenciales genericos desde React. El ticket parte del comportamiento reutilizable de `docs/Architecture/AppProgressBatch/legacy/JSProgresBar.legacy.js`, pero la implementacion nueva no debe copiar codigo legacy ni arrastrar dependencias de jQuery, Bootstrap manual, funciones globales, IDs fijos, `name_service` o estado global.

El componente vive en `src/app/Components/UI/AppProgressBatch/` y debe alinearse con:

- `docs/Architecture/AppProgressBatch/AppProgressBatch-Requisitos.md`
- `docs/Architecture/AppProgressBatch/Legacy-Gap-Analysis.md`
- `docs/Architecture/AppProgressBatch/diagrams/01-class-diagram.md`
- `docs/Architecture/AppProgressBatch/diagrams/02-use-cases.md`
- `docs/Architecture/AppProgressBatch/diagrams/03-state-diagram.md`
- `docs/Architecture/AppProgressBatch/diagrams/04-component-diagram.md`
- `docs/Architecture/AppProgressBatch/diagrams/05-sequence-diagram.md`

## Goals / Non-Goals

**Goals**

- Implementar un componente reusable sin dominio para procesos batch secuenciales.
- Exponer API generica tipada con `processItem(item, context)`.
- Modelar progreso global, progreso por item, item actual, fase, lifecycle y resumen final.
- Soportar cancelacion con `AbortController`, errores controlados, advertencias, omitidos, errores fatales y lista vacia.
- Bloquear doble ejecucion concurrente y cierre silencioso durante ejecucion.
- Ignorar resultados stale al cancelar, cerrar, desmontar o iniciar otra corrida.
- Usar `AppModal` como contenedor y `AppButton` para acciones.
- Cubrir el contrato con tests unitarios/integracion y README local.

**Non-Goals**

- No implementar upload, almacenamiento documental, workflow, firmas, indices ni consumidores concretos.
- No llamar APIs ni importar services de negocio.
- No modificar backend.
- No modificar `AppUpload` ni `AppUploadDocumental`.
- No crear modal nuevo si `AppModal` cubre el caso.
- No introducir dependencias nuevas salvo evidencia tecnica indispensable.

## Decisions

1. **Ubicacion shared**
   - Crear exactamente `src/app/Components/UI/AppProgressBatch/`.
   - Agregar `index.ts` local y export en `src/app/Components/UI/index.ts`.
   - Mantener utilidades privadas dentro de la carpeta si se requieren, sin exportarlas desde el barrel publico.

2. **Contrato generico**
   - `AppProgressBatchProps<TItem>` recibe `items: TItem[]` y `processItem`.
   - El componente nunca decide que servicio ejecutar; el consumidor encapsula operaciones de negocio.
   - No se usa `any`; para validaciones runtime se usa `unknown`.

3. **Separacion de estados**
   - UI state: apertura, item actual, label, fase, porcentajes, mensaje visible, resumen y confirmacion.
   - Execution state: lifecycle, runId, indice actual, `AbortController`, conteos y resultado pendiente.
   - Consumer state: items de negocio, `processItem` y callbacks `onComplete`, `onCancel`, `onError`.

4. **Lifecycle explicito**
   - Estados publicos: `idle | running | paused | cancelling | completed | error`.
   - `controlled-error` lleva el componente a `paused`.
   - `fatal-error` o excepcion lleva a `error`.
   - Cancelar desde `running` o `paused` lleva a `cancelling` y luego deja resumen cancelado visible.

5. **Run isolation**
   - Cada corrida crea un `runId` interno.
   - Resultados asincronos con `runId` obsoleto se ignoran.
   - El unmount, cancelacion y cierre invalidan la corrida activa.

6. **Cancelacion**
   - Cada corrida crea un `AbortController`.
   - `context.signal` se entrega a `processItem`.
   - Cancelar llama `abortController.abort()`, detiene items pendientes y emite `onCancel(summary)`.
   - No se asume que el backend soporte cancelacion; solo se propaga `AbortSignal`.

7. **Resultados tipados**
   - `success`: cuenta procesado y exitoso.
   - `warning`: cuenta procesado y advertencia, continua sin pausa.
   - `skipped`: cuenta procesado y omitido, continua sin pausa.
   - `controlled-error`: pausa; continuar registra error controlado y avanza; cancelar aborta.
   - `fatal-error`: cuenta fatal, emite `onError` y detiene.

8. **Guard runtime**
   - Implementar `isValidBatchItemResult(value: unknown): value is AppProgressBatchItemResult`.
   - Resultado invalido se trata como fatal controlado y dispara `onError`.
   - El componente no debe confiar ciegamente en consumidores externos.

9. **AutoStart y mutex**
   - `autoStart=true` inicia una sola vez por apertura cuando hay items.
   - React StrictMode no debe duplicar corridas.
   - Mientras `running`, `paused` o `cancelling`, no se permite iniciar otra corrida ni aceptar cambios de `items` como batch nuevo.

10. **Lista vacia**
    - Si `items.length === 0`, no crear `AbortController` ni llamar `processItem`.
    - Mostrar `emptyMessage` o default.
    - Emitir `onComplete` con resumen total cero.

11. **UI enterprise**
    - Usar `AppModal` con `maskClosable={false}`, `hideFooter` y footer custom interno.
    - Usar `AppButton` para iniciar, cancelar, continuar, cerrar y confirmar cancelacion.
    - Usar `Progress` de Ant Design para barras de progreso y `Alert` para mensajes, si no existe wrapper local equivalente.
    - Mantener layout estable entre estados, sin saltos bruscos de altura.
    - Reservar zona de mensajes y hacer wrap/truncado controlado para labels largos.

12. **Documentacion**
    - Crear `README.md` en la carpeta del componente.
    - Mantener alineados los documentos de arquitectura existentes.
    - Registrar evidencias de tests, build/lint si se ejecutan y riesgos residuales.

## Component Shape

```txt
src/app/Components/UI/AppProgressBatch/
├── AppProgressBatch.tsx
├── AppProgressBatch.types.ts
├── AppProgressBatch.module.css
├── AppProgressBatch.test.tsx
├── README.md
└── index.ts
```

Utilidades privadas opcionales:

```txt
src/app/Components/UI/AppProgressBatch/AppProgressBatch.utils.ts
```

## Public API

El barrel local debe exponer:

```ts
export { AppProgressBatch } from "./AppProgressBatch";
export type {
  AppProgressBatchItemContext,
  AppProgressBatchItemResult,
  AppProgressBatchLifecycle,
  AppProgressBatchProps,
  AppProgressBatchSummary,
} from "./AppProgressBatch.types";
```

El barrel shared debe agregar:

```ts
export * from "./AppProgressBatch";
```

## Risks / Trade-offs

- **StrictMode puede duplicar efectos**: usar guard interno por apertura/run para que `autoStart` no duplique procesos.
- **Cancelacion no garantiza rollback de negocio**: documentar que el componente solo aborta mediante `AbortSignal`; el consumidor define compensaciones.
- **Progress muy frecuente puede causar renders excesivos**: normalizar porcentajes y evitar updates redundantes; no introducir throttling/dependencias salvo evidencia.
- **Resultados invalidos del consumidor**: validar runtime y tratar como fatal.
- **Cambios de props durante ejecucion**: congelar la corrida activa; no iniciar corrida paralela ni recontar con nuevos `items`.
- **Accesibilidad de estados dinamicos**: usar texto visible y labels en barras; evitar depender solo del color.

## Migration Plan

1. Crear carpeta y tipos publicos del componente.
2. Implementar estado inicial, defaults centralizados y utilidades privadas.
3. Implementar maquina de ejecucion secuencial con `runId`, mutex y `AbortController`.
4. Implementar politica de resultados, lista vacia y callbacks.
5. Implementar UI con `AppModal`, `AppButton`, `Progress`, `Alert` y estilos CSS module.
6. Exportar desde barrel local y shared.
7. Crear README con ejemplos basico, error controlado y cancelacion.
8. Agregar tests unitarios e integracion del contrato.
9. Ejecutar validaciones y documentar evidencia.

## Open Questions

- Confirmar si el cierre automatico con `closeOnComplete=true` debe ser inmediato o diferido. Decision provisional: cerrar inmediatamente despues de emitir `onComplete`, sin ocultar errores ni cancelaciones.
- Confirmar si el resumen cancelado debe permanecer visible por defecto. Decision provisional: si el consumidor no cierra el modal, mostrar resumen parcial cancelado.
- Confirmar si `controlled-error` con `canContinue === false` debe permitir solo cancelar. Decision provisional: no mostrar accion continuar y dejar cancelar/cerrar bajo politica de cancelacion.

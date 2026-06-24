# Jira Context - SCRUMCORE-263

## Summary

CREA-COMPONENTE-APPPROGRESSBATCH

## Description

> # PROMPT ARQUITECTONICO - AppProgressBatch
> 
> ## Rol esperado
> 
> Arquitecto frontend senior
> 
> React 19, TypeScript estricto, componentes UI enterprise, state orchestration, accesibilidad, procesos batch reutilizables, cancelacion controlada, UX transaccional, testing frontend.
> 
> ## Objetivo
> 
> Implementar `AppProgressBatch` como un componente reusable de UI para ejecutar procesos secuenciales por lotes donde:
> 
> - el componente reciba una lista generica de items;
> - el consumidor inyecte la operacion concreta mediante `processItem`;
> - el componente muestre progreso global;
> - el componente muestre item actual y fase actual;
> - el componente soporte cancelacion segura;
> - el componente soporte errores controlados con continuar/cancelar;
> - el componente soporte advertencias no bloqueantes;
> - el componente soporte items omitidos;
> - el componente emita un resumen final tipado;
> - el componente no conozca ningun dominio concreto.
> 
> El componente debe reemplazar conceptualmente el comportamiento reutilizable de `JSProgresBar.js`, pero sin migrar sus dependencias legacy.
> 
> ## IMPORTANTE
> 
> Este ticket NO debe:
> 
> - implementar carga de archivos;
> - implementar almacenamiento documental;
> - llamar endpoints;
> - importar servicios de negocio;
> - acoplarse a workflow, documentos, firmas, indices o upload;
> - usar jQuery;
> - construir HTML por strings;
> - usar Bootstrap manual;
> - usar variables globales;
> - usar `name_service`;
> - usar funciones globales legacy;
> - usar `setInterval(..., 1)` para esperar decisiones del usuario;
> - introducir `any` nuevo;
> - cambiar `AppUpload`;
> - modificar backend;
> - modificar arquitectura de modulos consumidores.
> 
> Este ticket SI debe:
> 
> - crear un componente shared reusable;
> - exponer contrato generico por props;
> - ejecutar items secuencialmente;
> - mostrar progreso global y progreso del item actual;
> - permitir que `processItem` actualice label, fase y progreso del item;
> - implementar cancelacion mediante `AbortController`;
> - modelar ciclo de vida explicito;
> - bloquear cierre accidental durante ejecucion;
> - manejar lista vacia;
> - manejar resultados `success`, `warning`, `skipped`, `controlled-error`, `fatal-error`;
> - emitir `onComplete`, `onCancel`, `onError`;
> - tener pruebas unitarias e integracion de comportamiento;
> - documentar uso esperado y limites.
> 
> ## Dependencia
> 
> - React 19.
> - TypeScript estricto.
> - Sistema UI compartido existente.
> - Ant Design si ya esta disponible para `Modal`, `Progress`, `Button`, `Alert` o equivalentes del design system.
> - Convenciones actuales de componentes shared en `src/app/Components/UI`.
> 
> ## Contexto existente
> 
> El legacy `JSProgresBar.js` implementa:
> 
> - procesamiento secuencial de `OptionItemSelect`;
> - progreso `x de y`;
> - barra de porcentaje;
> - cancelacion;
> - pausa por confirmacion;
> - manejo de codigos `YES`, `CTRL`, `CTRLRETURN`;
> - ejecucion de operaciones por `name_service`;
> - modales construidos manualmente con Bootstrap y jQuery;
> - variable global `_JSProgresBar`;
> - estado numerico `estado_control`.
> 
> Problemas actuales del legacy:
> 
> - acoplamiento a operaciones concretas;
> - acoplamiento a funciones globales;
> - estado global mutable;
> - UI no React;
> - dificil testabilidad;
> - manejo de pausa/cancelacion basado en polling;
> - resultados expresados como strings ambiguos;
> - dependencia visual de IDs fijos.
> 
> ## Estado actual
> 
> No existe un componente shared moderno que:
> 
> - ejecute procesos batch secuenciales genericos;
> - permita inyectar operacion por item;
> - exponga progreso global reusable;
> - maneje cancelacion y confirmacion sin dominio;
> - emita resumen final tipado;
> - pueda ser usado por `AppUploadDocumental` u otros procesos futuros.
> 
> ## Ubicacion esperada
> 
> Componente:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/AppProgressBatch.tsx
> ```
> 
> Tipos:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/AppProgressBatch.types.ts
> ```
> 
> Estilos:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/AppProgressBatch.module.css
> ```
> 
> Tests:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx
> ```
> 
> Export:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/index.ts
> ```
> 
> Barrel shared obligatorio:
> 
> ```txt
> src/app/Components/UI/index.ts
> ```
> 
> Documentacion:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/README.md
> ```
> 
> ## Componentes UI concretos obligatorios
> 
> Usar los componentes shared existentes:
> 
> ```txt
> src/app/Components/UI/AppModal/AppModal.tsx
> src/app/Components/UI/AppButton/AppButton.tsx
> ```
> 
> Uso esperado:
> 
> - `AppModal` como contenedor modal principal.
> - `AppButton` para acciones primarias y secundarias.
> - `Progress` de Ant Design para barras de progreso, salvo wrapper local equivalente.
> - `Alert` de Ant Design para errores, advertencias y lista vacia, salvo wrapper local equivalente.
> - HTML semantico simple o `Typography` para textos.
> 
> No crear un modal nuevo si `AppModal` cubre el caso.
> 
> `AppModal` debe configurarse con:
> 
> - `open={open}`;
> - `title={title ?? processName ?? "Proceso por lotes"}`;
> - `onClose` apuntando a la politica de cierre/cancelacion;
> - `maskClosable={false}`;
> - `closeOnEscape` solo si respeta la politica de cancelacion;
> - `hideFooter` cuando el footer sea custom dentro del componente.
> 
> `AppButton` debe usarse para:
> 
> - iniciar;
> - cancelar;
> - continuar;
> - cerrar;
> - confirmar cancelacion.
> 
> No usar botones Ant Design directos salvo que `AppButton` no soporte el caso.
> 
> ## Estructura de archivos obligatoria
> 
> Crear exactamente:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/
> ├─ AppProgressBatch.tsx
> ├─ AppProgressBatch.types.ts
> ├─ AppProgressBatch.module.css
> ├─ AppProgressBatch.test.tsx
> ├─ README.md
> └─ index.ts
> ```
> 
> Tambien actualizar:
> 
> ```txt
> src/app/Components/UI/index.ts
> ```
> 
> Si la logica interna crece, se permite crear archivos privados dentro de la misma carpeta, por ejemplo:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/AppProgressBatch.utils.ts
> ```
> 
> No exportar utilidades privadas desde el barrel publico.
> 
> ## Referencias de arquitectura obligatorias
> 
> Leer y respetar:
> 
> ```txt
> docs/Architecture/AppProgressBatch/AppProgressBatch-Requisitos.md
> docs/Architecture/AppProgressBatch/Legacy-Gap-Analysis.md
> docs/Architecture/AppProgressBatch/diagrams/01-class-diagram.md
> docs/Architecture/AppProgressBatch/diagrams/02-use-cases.md
> docs/Architecture/AppProgressBatch/diagrams/03-state-diagram.md
> docs/Architecture/AppProgressBatch/diagrams/04-component-diagram.md
> docs/Architecture/AppProgressBatch/diagrams/05-sequence-diagram.md
> docs/Architecture/AppProgressBatch/legacy/JSProgresBar.legacy.js
> ```
> 
> La referencia legacy es solo evidencia. No copiar codigo legacy.
> 
> ## Restricciones obligatorias
> 
> - NO usar `any` nuevo.
> - NO acoplar a dominio.
> - NO llamar APIs.
> - NO usar variables globales.
> - NO depender de IDs DOM fijos.
> - NO manipular DOM manualmente salvo foco controlado si es imprescindible.
> - NO usar jQuery.
> - NO usar Bootstrap manual.
> - NO usar timers para simular estado de pausa.
> - NO permitir doble ejecucion concurrente del mismo batch.
> - NO cerrar silenciosamente durante ejecucion.
> - NO tragar errores fatales.
> - NO mezclar warning con error fatal.
> - NO asumir que cancelacion backend existe; el componente solo propaga `AbortSignal`.
> 
> ## Regla arquitectonica obligatoria
> 
> `AppProgressBatch` debe ser un componente de orquestacion UI generica.
> 
> La operacion concreta vive fuera del componente y se inyecta por:
> 
> ```ts
> processItem(item, context)
> ```
> 
> El componente nunca debe decidir internamente que servicio ejecutar.
> 
> ## Regla de separacion de responsabilidades
> 
> Debe diferenciarse explicitamente:
> 
> ### UI State
> 
> - abierto/cerrado;
> - item actual;
> - label actual;
> - fase actual;
> - porcentaje global;
> - porcentaje del item actual;
> - dialogo de confirmacion;
> - mensaje visible;
> - resumen final.
> 
> ### Execution State
> 
> - lifecycle `idle | running | paused | cancelling | completed | error`;
> - `AbortController`;
> - indice actual;
> - conteos acumulados;
> - resultado del item actual.
> 
> ### Consumer State
> 
> - items de negocio;
> - implementacion de `processItem`;
> - reaccion a `onComplete`, `onCancel`, `onError`.
> 
> El componente no debe persistir informacion de negocio.
> 
> ## Regla de source-of-truth obligatoria
> 
> El source-of-truth del avance batch es el estado interno del componente durante una ejecucion activa.
> 
> El source-of-truth del resultado de negocio pertenece al consumidor.
> 
> `AppProgressBatch` solo reporta resumen operacional, no confirma efectos de negocio que no puede verificar.
> 
> ## Regla de atomicidad UX obligatoria
> 
> Un item solo puede contarse como exitoso cuando `processItem` retorna:
> 
> ```ts
> { status: "success" }
> ```
> 
> Antes de eso:
> 
> - no incrementar `success`;
> - no avanzar resumen definitivo;
> - no emitir `onComplete`;
> - no cerrar como exitoso.
> 
> ## Regla anti-concurrencia obligatoria
> 
> Mientras `status` sea `running`, `paused` o `cancelling`:
> 
> - no iniciar otra ejecucion;
> - no aceptar cambios de `items` como nuevo batch activo;
> - no cerrar sin pasar por politica de cancelacion;
> - no ejecutar `processItem` simultaneamente para dos items.
> 
> ## Regla anti-stale obligatoria
> 
> Cada ejecucion debe tener un `runId` interno.
> 
> Si:
> 
> - cambia `open`;
> - el componente se desmonta;
> - el usuario cancela;
> - inicia una ejecucion nueva despues de finalizar otra;
> 
> entonces:
> 
> - resultados asincronos de ejecuciones anteriores deben ignorarse;
> - no deben contaminar el resumen vigente;
> - no deben actualizar UI de una corrida ya cancelada/desmontada.
> 
> ## Contrato frontend obligatorio
> 
> Tipos minimos:
> 
> ```ts
> export type AppProgressBatchLifecycle =
>   | "idle"
>   | "running"
>   | "paused"
>   | "cancelling"
>   | "completed"
>   | "error";
> 
> export type AppProgressBatchItemResult =
>   | { status: "success" }
>   | { status: "warning"; message: string }
>   | { status: "skipped"; message?: string }
>   | { status: "controlled-error"; message: string; canContinue?: boolean }
>   | { status: "fatal-error"; message: string };
> 
> export type AppProgressBatchSummary = {
>   total: number;
>   processed: number;
>   success: number;
>   warnings: number;
>   skipped: number;
>   controlledErrors: number;
>   fatalErrors: number;
>   cancelled: boolean;
> };
> 
> export type AppProgressBatchItemContext = {
>   index: number;
>   total: number;
>   signal: AbortSignal;
>   setCurrentLabel: (label: string) => void;
>   setItemProgress: (percent: number) => void;
>   setPhase: (phase: string) => void;
> };
> 
> export type AppProgressBatchProps<TItem> = {
>   open: boolean;
>   items: TItem[];
>   onOpenChange: (open: boolean) => void;
>   processItem: (
>     item: TItem,
>     context: AppProgressBatchItemContext,
>   ) => Promise<AppProgressBatchItemResult>;
>   title?: string;
>   processName?: string;
>   autoStart?: boolean;
>   confirmOnCancel?: boolean;
>   emptyMessage?: string;
>   closeOnComplete?: boolean;
>   getItemLabel?: (item: TItem, index: number) => string;
>   onComplete?: (summary: AppProgressBatchSummary) => void;
>   onCancel?: (summary: AppProgressBatchSummary) => void;
>   onError?: (error: unknown) => void;
> };
> ```
> 
> No usar `any`; si se requiere tipo generico, usar `unknown` o genericos acotados.
> 
> ## API de exports obligatoria
> 
> `src/app/Components/UI/AppProgressBatch/index.ts` debe exportar:
> 
> ```ts
> export { AppProgressBatch } from "./AppProgressBatch";
> export type {
>   AppProgressBatchItemContext,
>   AppProgressBatchItemResult,
>   AppProgressBatchLifecycle,
>   AppProgressBatchProps,
>   AppProgressBatchSummary,
> } from "./AppProgressBatch.types";
> ```
> 
> `src/app/Components/UI/index.ts` debe agregar:
> 
> ```ts
> export * from "./AppProgressBatch";
> ```
> 
> La API publica debe quedar disponible desde:
> 
> ```ts
> import { AppProgressBatch } from "src/app/Components/UI";
> ```
> 
> ## Flujo obligatorio completo
> 
> 1. Consumidor renderiza `AppProgressBatch` con `open=true`, `items` y `processItem`.
> 2. Si `items.length === 0`, no ejecutar `processItem`.
> 3. Mostrar `emptyMessage` y emitir `onComplete` con resumen en cero o resumen total cero.
> 4. Si hay items, preparar nueva ejecucion con `runId` y `AbortController`.
> 5. Pasar lifecycle a `running`.
> 6. Tomar primer item.
> 7. Resolver label inicial con `getItemLabel` si existe.
> 8. Construir `AppProgressBatchItemContext`.
> 9. Ejecutar `await processItem(item, context)`.
> 10. Permitir que el proceso actualice label, fase y progreso del item.
> 11. Normalizar porcentaje de item entre 0 y 100.
> 12. Si retorna `success`, incrementar exitos y avanzar.
> 13. Si retorna `warning`, registrar advertencia, incrementar procesados y avanzar.
> 14. Si retorna `skipped`, registrar omitido, incrementar procesados y avanzar.
> 15. Si retorna `controlled-error`, pasar a `paused`, mostrar mensaje y pedir decision.
> 16. Si usuario continua, registrar error controlado y avanzar.
> 17. Si usuario cancela, pasar a `cancelling`, abortar controller y emitir `onCancel`.
> 18. Si retorna `fatal-error`, pasar a `error`, emitir `onError` y detener.
> 19. Si `processItem` lanza excepcion, tratar como error fatal.
> 20. Al procesar ultimo item sin cancelacion/fatal, pasar a `completed`.
> 21. Emitir `onComplete(summary)`.
> 22. Si `closeOnComplete` esta activo, llamar `onOpenChange(false)`.
> 23. Limpiar controller en desmontaje o cierre.
> 
> ## Politica de resultados obligatoria
> 
> ### `success`
> 
> - cuenta como procesado;
> - incrementa `success`;
> - avanza al siguiente item.
> 
> ### `warning`
> 
> - cuenta como procesado;
> - incrementa `warnings`;
> - no pausa;
> - avanza al siguiente item;
> - debe quedar visible en resumen.
> 
> ### `skipped`
> 
> - cuenta como procesado;
> - incrementa `skipped`;
> - no pausa;
> - avanza al siguiente item.
> 
> ### `controlled-error`
> 
> - cuenta como controlado solo cuando el usuario decide continuar o cancelar;
> - pausa el proceso;
> - muestra mensaje;
> - permite continuar si `canContinue !== false`;
> - permite cancelar.
> 
> ### `fatal-error`
> 
> - detiene el proceso;
> - incrementa `fatalErrors`;
> - emite `onError`;
> - no procesa pendientes.
> 
> ## Guard runtime obligatorio
> 
> Crear una funcion privada para validar resultados de `processItem`:
> 
> ```ts
> function isValidBatchItemResult(value: unknown): value is AppProgressBatchItemResult
> ```
> 
> Si el resultado no es valido:
> 
> - tratar como `fatal-error`;
> - construir mensaje controlado;
> - emitir `onError`.
> 
> Esto evita asumir que el consumidor siempre cumple contrato.
> 
> ## Politica de cancelacion obligatoria
> 
> Cancelacion debe:
> 
> - pasar lifecycle a `cancelling`;
> - llamar `abortController.abort()`;
> - impedir nuevos items;
> - ignorar resultados tardios;
> - emitir `onCancel(summary)`;
> - no emitir `onComplete` como exito total si fue cancelado.
> 
> Si se cancela durante `paused`, debe comportarse igual.
> 
> Si se intenta cerrar el modal mientras `running` o `paused`:
> 
> - si `confirmOnCancel=true`, pedir confirmacion;
> - si `confirmOnCancel=false`, cancelar directamente;
> - nunca cerrar de forma silenciosa perdiendo estado.
> 
> ## Mutex de ejecucion obligatorio
> 
> Debe existir un bloqueo interno que impida:
> 
> - doble click sobre iniciar;
> - `autoStart` duplicado por rerender;
> - reinicio mientras `running`, `paused` o `cancelling`.
> 
> Si `autoStart=true`, iniciar una sola vez por apertura y lista de items estable.
> 
> La implementacion debe cuidar React StrictMode para no duplicar ejecuciones en desarrollo.
> 
> ## Politica de lista vacia
> 
> Si `items.length === 0`:
> 
> - mostrar `emptyMessage` o mensaje por defecto;
> - no crear `AbortController`;
> - no llamar `processItem`;
> - emitir resumen:
> 
> ```ts
> {
>   total: 0,
>   processed: 0,
>   success: 0,
>   warnings: 0,
>   skipped: 0,
>   controlledErrors: 0,
>   fatalErrors: 0,
>   cancelled: false
> }
> ```
> 
> ## Politica de autoStart obligatoria
> 
> Si `autoStart=true`:
> 
> - iniciar cuando `open=true` y lifecycle sea `idle`;
> - no iniciar si `items.length === 0`;
> - no reiniciar por rerender;
> - reiniciar solo despues de cerrar/resetear y abrir nuevamente.
> 
> Si `autoStart=false`:
> 
> - mostrar accion `Iniciar` cuando lifecycle sea `idle` y existan items;
> - esa accion debe usar `AppButton`;
> - no iniciar automaticamente por cambios de props.
> 
> ## Politica visual obligatoria
> 
> UI minima:
> 
> - titulo;
> - nombre de proceso;
> - barra de progreso global;
> - contador `x de y`;
> - label de item actual;
> - fase actual;
> - progreso del item actual si existe;
> - boton cancelar durante ejecucion;
> - decision continuar/cancelar en error controlado;
> - resumen final.
> 
> No usar texto excesivo dentro de botones. Usar iconos si el patron local lo permite.
> 
> ## Layout visual obligatorio
> 
> El modal debe tener una composicion estable:
> 
> ```txt
> Header
> ├─ titulo del modal
> └─ nombre del proceso si es distinto del titulo
> 
> Body
> ├─ bloque de estado actual
> │  ├─ label del item actual
> │  ├─ fase actual
> │  └─ contador x de y
> ├─ progreso global
> ├─ progreso del item actual
> ├─ mensaje de error, warning o lista vacia
> └─ resumen final cuando aplique
> 
> Footer
> ├─ Iniciar en idle
> ├─ Cancelar durante running
> ├─ Continuar y Cancelar en controlled-error
> └─ Cerrar cuando completed/error sin proceso activo
> ```
> 
> Reglas visuales:
> 
> - El modal no debe cambiar bruscamente de alto entre fases.
> - La zona de mensajes debe reservar espacio razonable.
> - Labels largos deben truncarse con tooltip o hacer wrap controlado.
> - Las barras de progreso deben tener `aria-label` o texto accesible equivalente.
> - El contador debe mostrarse como `N de Total`.
> - En lista vacia, mostrar mensaje funcional y boton cerrar.
> - En resumen final, mostrar total, exitos, advertencias, omitidos, errores controlados, errores fatales y cancelado.
> 
> ## Politica de footer obligatoria
> 
> El footer debe cambiar segun lifecycle:
> 
> | Estado | Acciones |
> | --- | --- |
> | `idle` con items | Iniciar, Cerrar |
> | `idle` con lista vacia | Cerrar |
> | `running` | Cancelar |
> | `paused` por error controlado | Continuar, Cancelar |
> | `cancelling` | Acciones deshabilitadas o loading |
> | `completed` | Cerrar |
> | `error` | Cerrar |
> 
> Si `closeOnComplete=true`, puede cerrar automaticamente despues de emitir `onComplete`, pero no debe ocultar errores.
> 
> ## Maquina de estados interna obligatoria
> 
> Implementar estado interno minimo:
> 
> ```ts
> type InternalBatchState = {
>   lifecycle: AppProgressBatchLifecycle;
>   runId: number;
>   currentIndex: number;
>   currentLabel: string;
>   currentPhase: string;
>   globalPercent: number;
>   itemPercent: number;
>   message: string | null;
>   summary: AppProgressBatchSummary;
> };
> ```
> 
> Puede ajustarse el nombre, pero debe existir separacion equivalente.
> 
> Calculo de progreso global:
> 
> ```txt
> processed / total * 100
> ```
> 
> Durante item activo, el progreso global puede considerar solo items cerrados. El progreso del item se muestra aparte.
> 
> No mezclar `itemPercent` con `globalPercent`.
> 
> ## Accesibilidad y UX
> 
> - Dialog accesible con titulo.
> - Botones con `aria-label` cuando corresponda.
> - Focus estable al abrir.
> - Focus estable al mostrar confirmacion.
> - Estados de progreso legibles por screen reader si el componente base lo permite.
> - Errores visibles y accionables.
> - No flicker severo.
> - Textos largos truncados o con wrap controlado.
> - Cierre por teclado debe respetar politica de cancelacion.
> 
> ## Textos por defecto obligatorios
> 
> Definir defaults centralizados:
> 
> ```ts
> const DEFAULT_TITLE = "Proceso por lotes";
> const DEFAULT_EMPTY_MESSAGE = "No hay elementos para procesar.";
> const DEFAULT_CANCEL_CONFIRM_MESSAGE = "Hay un proceso en curso. Desea cancelarlo?";
> ```
> 
> Los textos pueden ajustarse a convenciones del producto, pero no deben quedar hardcodeados dispersos.
> 
> Mensajes minimos:
> 
> - lista vacia;
> - cancelacion en curso;
> - error controlado;
> - error fatal;
> - proceso completado.
> 
> ## Reglas de performance
> 
> - Procesar un item a la vez.
> - Evitar re-render completo costoso en cada tick si `setItemProgress` se llama muchas veces.
> - Normalizar progreso y evitar updates redundantes si el porcentaje no cambia.
> - Memoizar callbacks internos cuando sea razonable.
> - Limpiar `AbortController` en unmount.
> - Evitar timers permanentes.
> 
> Implementar throttling simple de progreso solo si los tests o comportamiento real muestran demasiados renders. No introducir dependencias nuevas para esto.
> 
> ## Reglas de seguridad
> 
> - No loguear payloads sensibles recibidos por items.
> - No exponer contenido de items en errores salvo label provisto por consumidor.
> - No persistir datos de negocio.
> - No almacenar items en variables globales.
> 
> ## Manejo de errores obligatorio
> 
> ### Caso A: `processItem` lanza excepcion
> 
> - detener batch;
> - lifecycle `error`;
> - emitir `onError`;
> - mantener resumen parcial.
> 
> ### Caso B: resultado invalido
> 
> Si `processItem` retorna un objeto sin `status` valido:
> 
> - tratar como fatal error;
> - emitir error controlado al consumidor;
> - no continuar silenciosamente.
> 
> ### Caso C: cancelacion durante item activo
> 
> - llamar `abort`;
> - ignorar resultado tardio;
> - emitir `onCancel`.
> 
> ### Caso D: error controlado sin posibilidad de continuar
> 
> Si `canContinue === false`:
> 
> - mostrar mensaje;
> - permitir cancelar/cerrar segun politica;
> - no avanzar automaticamente.
> 
> ### Caso E: lista vacia
> 
> - no llamar `processItem`;
> - mostrar mensaje;
> - emitir resumen total cero.
> 
> ## Riesgos a evitar
> 
> - doble ejecucion concurrente;
> - cierre silencioso durante ejecucion;
> - summary inconsistente;
> - mezclar advertencia con error fatal;
> - seguir procesando despues de cancelar;
> - actualizar UI despues de unmount;
> - requerir campos especificos de negocio;
> - crear un componente que solo sirva para upload;
> - reproducir `name_service` del legacy;
> - introducir estado global.
> 
> ## Pruebas unitarias obligatorias
> 
> - render controlado con `open`.
> - usa `AppModal` y respeta `onClose`.
> - no ejecuta `processItem` con lista vacia.
> - muestra `emptyMessage`.
> - `autoStart=true` inicia una sola vez.
> - `autoStart=false` no inicia automaticamente.
> - ejecuta items en orden.
> - `success` incrementa exitos.
> - `warning` incrementa advertencias y continua.
> - `skipped` incrementa omitidos y continua.
> - `controlled-error` pausa.
> - continuar despues de `controlled-error` procesa siguiente item.
> - cancelar despues de `controlled-error` no procesa pendientes.
> - `fatal-error` detiene y emite `onError`.
> - excepcion de `processItem` detiene y emite `onError`.
> - cancelacion llama `AbortController.abort`.
> - resultados tardios tras cancelacion no actualizan resumen activo.
> - `setItemProgress` normaliza valores menores a 0 y mayores a 100.
> - `getItemLabel` define label inicial.
> - resultado invalido de `processItem` se trata como fatal error.
> - exporta tipos y componente desde `index.ts`.
> 
> ## Pruebas de integracion obligatorias
> 
> - proceso completo de 3 items exitosos.
> - mezcla success + warning + skipped.
> - error controlado con continuar.
> - error controlado con cancelar.
> - intento de cierre durante ejecucion con `confirmOnCancel`.
> - `closeOnComplete` cierra despues de final exitoso.
> - cambio de props durante ejecucion no inicia corrida paralela.
> - cierre de modal durante ejecucion dispara politica de cancelacion.
> - lifecycle `completed` muestra resumen final.
> 
> ## Pruebas de interaccion en navegador obligatorias
> 
> - abrir modal y arrancar proceso.
> - cancelar con boton.
> - cancelar con cierre modal si aplica.
> - continuar ante error controlado.
> - revisar foco al abrir confirmacion.
> - revisar que contador y barras se actualicen.
> - textos largos no rompen layout.
> 
> ## Pruebas E2E recomendadas
> 
> - flujo batch exitoso.
> - flujo cancelado a mitad.
> - flujo con error controlado y retry desde consumidor.
> - flujo con lista vacia.
> 
> ## Pruebas de regresion obligatorias
> 
> - AppUpload intacto.
> - AppUploadDocumental aun no requerido para este ticket.
> - componentes UI shared no afectados.
> - `src/app/Components/UI/index.ts` exporta sin romper imports existentes.
> - build sin errores.
> - lint sin errores nuevos.
> - sin errores de consola en tests.
> 
> ## Criterios de aceptacion
> 
> - `AppProgressBatch` existe en `src/app/Components/UI/AppProgressBatch`.
> - Exporta componente y tipos.
> - Esta exportado desde `src/app/Components/UI/index.ts`.
> - Usa `AppModal` y `AppButton`.
> - No tiene dependencia de dominio.
> - No usa `any` nuevo.
> - Procesa items secuencialmente.
> - Soporta lista vacia.
> - Soporta cancelacion con `AbortController`.
> - Soporta `success`, `warning`, `skipped`, `controlled-error`, `fatal-error`.
> - Emite resumen final consistente.
> - No procesa items pendientes tras cancelacion o fatal error.
> - Ignora resultados stale.
> - Tiene pruebas unitarias e integracion relevantes.
> - Tiene README de uso.
> - El preview visual del componente es estable en estados `idle`, `running`, `paused`, `completed` y `error`.
> 
> ## Documentacion obligatoria
> 
> Actualizar o crear:
> 
> ```txt
> src/app/Components/UI/AppProgressBatch/README.md
> ```
> 
> Debe incluir:
> 
> - objetivo;
> - props;
> - tipos;
> - ejemplo basico;
> - ejemplo con error controlado;
> - ejemplo con cancelacion;
> - advertencia de que no debe conocer dominio;
> - relacion con `AppUploadDocumental`.
> 
> Mantener alineados:
> 
> ```txt
> docs/Architecture/AppProgressBatch/AppProgressBatch-Requisitos.md
> docs/Architecture/AppProgressBatch/Legacy-Gap-Analysis.md
> ```
> 
> ## Entrega esperada
> 
> - Diff de archivos frontend tocados.
> - Lista de archivos creados.
> - Resumen tecnico del componente.
> - Evidencia de tests ejecutados.
> - Comando exacto de tests.
> - Resultado de build/lint si se ejecuta.
> - Confirmacion explicita:
>   - backend NO modificado;
>   - endpoints NO modificados;
>   - componente reusable sin dominio;
>   - `any` nuevo NO introducido;
>   - cancelacion con `AbortController`;
>   - resultados stale ignorados.
> 
> ## Comandos de verificacion esperados
> 
> Ejecutar como minimo:
> 
> ```txt
> npm test -- src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx
> ```
> 
> Si el proyecto usa Vitest directo y el comando anterior no aplica, usar el comando equivalente existente del repo y registrar evidencia.
> 
> Ejecutar build/lint solo si el alcance y tiempo lo permiten. Si no se ejecutan, reportarlo explicitamente.
> 
> ## Instruccion final
> 
> Implementar `AppProgressBatch` como componente shared enterprise para orquestar procesos batch secuenciales genericos, reemplazando el comportamiento reutilizable de `JSProgresBar.js` mediante un contrato React tipado, cancelacion segura, control de ciclo de vida explicito, manejo de errores controlados, advertencias, omitidos, resumen final y separacion absoluta de cualquier dominio de negocio.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: APPPROGRESSBATCH, COMPONENTE, CREA

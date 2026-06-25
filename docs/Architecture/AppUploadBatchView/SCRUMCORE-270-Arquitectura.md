# SCRUMCORE-270 - Arquitectura Enterprise AppUploadBatchView

## Resumen Ejecutivo

`AppUploadBatchView` introduce una vista shared, reusable y tipada para representar experiencias de carga por lote sin acoplarse a ningun dominio de negocio. La vista moderniza la ergonomia util observada en el legacy `FileUploadHandler.js`, pero elimina dependencias legacy como jQuery, Bootstrap manual, IDs dinamicos, callbacks por string, estilos inline y HTML construido manualmente.

La implementacion queda ubicada en `src/app/Components/UI/AppUploadBatchView` y se exporta desde el barrel shared `src/app/Components/UI/index.ts`. Su responsabilidad es exclusivamente visual y de orquestacion de eventos UI: renderizar cola de archivos, archivo activo, preview, acciones, estados, errores, advertencias, metadata inyectada y resumen. El consumidor conserva el source-of-truth de negocio, validaciones, persistencia, progreso real y llamadas a servicios.

## Objetivo Arquitectonico

Crear una vista base enterprise para que futuras experiencias de carga puedan compartir layout, accesibilidad y patrones de interaccion sin duplicar UI ni mezclar reglas de negocio. El componente queda preparado para ser usado por especializaciones como `AppUploadDocumental`, anexos, evidencias, reemplazo PDF, imagenes o importaciones, siempre mediante props, callbacks y slots.

## Alcance Implementado

- Componente shared reusable `AppUploadBatchView`.
- Contratos TypeScript genericos sin `any`.
- Composicion de `AppUpload` como selector de archivos.
- Lista compacta de archivos con nombre, tamano, estado, progreso, fase, warning, error y acciones.
- Resolucion de archivo activo por `selectedUid`, bandera `selected` o primer archivo disponible.
- Preview default para PDF, imagenes y fallback para otros formatos.
- Soporte de `previewUrl` externo y object URL local con cleanup.
- Acciones globales: agregar, guardar todos, limpiar todos.
- Acciones por archivo: seleccionar/ver, guardar individual, eliminar.
- Slots de extension: metadata, preview, nombre de archivo y footer adicional.
- Calculo de resumen operacional cuando el consumidor no entrega `summary`.
- Layout responsive desktop/mobile con CSS module.
- Accesibilidad basica mediante roles, labels, `aria-live` y foco visible.
- README de uso del componente.
- Tests unitarios e integracion enfocados.

## Fuera de Alcance

- No se implementa almacenamiento documental.
- No se llaman endpoints.
- No se implementa upload por chunks.
- No se cargan tipologias, TRD, expedientes, workflow, radicados o gabinetes.
- No se modifica `AppUpload`.
- No se modifica backend.
- No se agrega logica de negocio documental.
- No se persisten archivos, metadata ni progreso.
- No se introduce una pantalla final de producto; el componente queda disponible como shared UI.

## Principios de Diseno

### Separacion de Responsabilidades

`AppUploadBatchView` renderiza y emite eventos. El consumidor administra:

- lista canonica de archivos;
- metadata especializada;
- validaciones;
- progreso real;
- llamadas a backend;
- reglas de habilitacion;
- seleccion persistente;
- almacenamiento y errores de negocio.

### Source of Truth

La vista no muta `files`. La lista llega como `ReadonlyArray<AppUploadBatchFileItem<TMetadata>>`; cualquier cambio se solicita al consumidor por callback. Esto evita estados duplicados y permite que el componente sea reutilizable en flujos controlados.

### Composicion Sobre Acoplamiento

La vista compone `AppUpload` y `AppButton`, y usa slots para permitir especializacion sin condicionar el componente base. No conoce nombres de campos documentales ni servicios especificos.

### Contrato Estable

El contrato publico se concentra en `AppUploadBatchView.types.ts`. La API soporta metadata generica mediante `TMetadata = unknown`, lo que permite especializaciones sin introducir `any`.

## Componentes y Dependencias

| Dependencia | Uso |
| --- | --- |
| `AppUpload` | Selector de archivos controlado por el consumidor. |
| `AppButton` | Acciones globales y acciones por archivo. |
| `Progress` de Ant Design | Indicador de progreso por archivo cuando aplica. |
| CSS module local | Layout responsive, estados visuales, truncado y accesibilidad visual. |
| APIs nativas `URL.createObjectURL` / `URL.revokeObjectURL` | Preview local temporal cuando no existe `previewUrl`. |

No se usan jQuery, Bootstrap manual, HTML por strings, IDs fijos ni estilos inline.

## Modelo de Responsabilidades

```txt
Consumidor
  - mantiene files y metadata
  - valida reglas de negocio
  - ejecuta servicios/backend
  - decide habilitaciones
  - reacciona a callbacks

AppUploadBatchView
  - renderiza layout enterprise
  - compone AppUpload
  - emite eventos UI
  - calcula summary fallback
  - renderiza preview local
  - limpia object URLs
  - expone slots visuales

AppUpload
  - resuelve interaccion nativa de seleccion
  - entrega File/originFile al wrapper
```

## Flujo Arquitectonico

1. El consumidor entrega `files`, flags de habilitacion, callbacks y slots.
2. La vista calcula el resumen si no se entrega `summary`.
3. La vista resuelve el archivo activo.
4. `AppUpload` funciona como entrada de seleccion; la vista intercepta la seleccion y emite `onFilesSelected`.
5. La cola se renderiza con estados y acciones disponibles.
6. El preview muestra el archivo activo o un estado vacio.
7. Las acciones no alteran estado local de negocio; llaman callbacks.
8. Si se genera object URL local, se revoca al cambiar el archivo o desmontar.

## Decisiones Tecnicas Relevantes

- `files` se define como `ReadonlyArray` para reforzar uso controlado.
- `TMetadata = unknown` permite metadata generica sin degradar TypeScript.
- `beforeUpload` de `AppUpload` se usa para emitir una seleccion unica y evitar que `AppUpload` mantenga una lista visual duplicada.
- `value={[]}` en `AppUpload` conserva el source-of-truth en el consumidor.
- `previewUrl` tiene prioridad sobre object URL local para soportar URLs ya gestionadas por el consumidor.
- El resumen fallback agrupa `uploading`, `completing` y `storing` como actividad operacional en curso.
- Las acciones destructivas se bloquean para items `done`, items `disabled`, estado global `disabled` o `loading`.
- Los nombres largos se trunccan visualmente y conservan `title`.
- La vista reserva layout estable para preview y lista, sin cards anidadas.

## Accesibilidad

- Raiz semantica con `section` y `aria-label`.
- Toolbar con `aria-label`.
- Lista con `role="list"` y filas con `role="listitem"`.
- Botones iconograficos con texto accesible via `aria-label`.
- Resumen con `aria-live="polite"`.
- Preview con region `aside` y `aria-label`.
- Fila seleccionable con `aria-pressed`.
- Foco visible en botones y filas.
- Estados no dependen solo de color: tambien hay texto de estado, warning o error.

## Seguridad y Privacidad

- No se loguean archivos ni metadata.
- No se persisten object URLs.
- No se envian archivos a backend desde la vista.
- No se exponen campos de negocio en errores.
- No se crean variables globales.
- No se manipula el DOM manualmente.

## Riesgos Controlados

| Riesgo | Control aplicado |
| --- | --- |
| Duplicar source-of-truth con `AppUpload` | `value={[]}` y callbacks controlados. |
| Fuga de object URLs | Cleanup con `URL.revokeObjectURL`. |
| Acoplamiento documental | Metadata generica y slots. |
| Layout roto por nombres largos | Truncado visual con `title`. |
| Acciones peligrosas durante carga | Flags globales, `loading`, `disabled` e item `disabled`. |
| Regresion en `AppUpload` | Se compone sin modificar su fuente. |

## Estado Arquitectonico Final

La arquitectura cumple el alcance del ticket: una vista base enterprise, reusable, sin dominio y preparada para que `AppUploadDocumental` u otros flujos implementen reglas especificas encima sin modificar el shared UI.

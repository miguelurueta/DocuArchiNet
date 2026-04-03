## Context

`AppTable` hoy es un wrapper base de AG Grid con overlays, selección y render dinámico de acciones, pero todavía no distingue entre paginación local, paginación servidor o ausencia total de paginación. Tras `SCRUMCORE-39` y `SCRUMCORE-40`, el proyecto ya dispone de un `AppTableQueryState` reusable y de un `AppTableQueryWrapper` para la capa visual; ahora hace falta que el renderer base soporte explícitamente los modos `none`, `client` y `server`.

El objetivo no es meter la barra tipo Gmail dentro de `AppTable`. Esa experiencia ya quedó ubicada en el wrapper externo. El cambio aquí debe limitarse a:

- exponer un contrato claro de `paginationMode`
- activar o desactivar la paginación nativa de AG Grid según el modo
- aplicar `quickFilterText` únicamente en modos locales
- preservar el comportamiento previo cuando no se informe `paginationMode`

La principal restricción es no romper pantallas existentes que hoy consumen `AppTable` sin conocer estos modos nuevos. El componente debe seguir actuando como renderer base, no como controlador de la consulta.

## Goals / Non-Goals

**Goals:**
- Agregar `paginationMode?: "none" | "client" | "server"` a `AppTable`.
- Agregar `quickFilterText?: string` y `clientPaginationPageSize?: number`.
- Centralizar la lógica de modo en `AppTable` y `useAgGridBaseConfig`.
- Permitir quick filter local en `client` y `none`, ignorándolo en `server`.
- Preservar completamente la API previa cuando no se informen las nuevas props.

**Non-Goals:**
- No embutir la barra Gmail ni navegación externa dentro de `AppTable`.
- No introducir lógica de backend o `queryState` en el grid base.
- No migrar todavía módulos consumidores a `server mode`.
- No recalcular `total` ni partir filas localmente fuera del comportamiento nativo de AG Grid.

## Decisions

### 1. `paginationMode` será explícito y `none` preservará el comportamiento previo

`AppTable` expondrá `paginationMode` como prop nueva con tres valores:

- `none`
- `client`
- `server`

Cuando la prop no exista, el componente debe conservar el comportamiento anterior: sin paginación nativa, mismo layout, mismos overlays y misma selección. Esto evita regresiones silenciosas en consumidores existentes.

**Alternativas consideradas**
- Inferir el modo desde `total` o `quickFilterText`: se descarta porque vuelve ambigua la API.
- Hacer `client` el default nuevo: se descarta porque rompería tablas actuales.

### 2. `client mode` delega la paginación a AG Grid

En `client mode`, `AppTable` activará la paginación nativa de AG Grid con un `paginationPageSize` fijo y documentado, derivado de `clientPaginationPageSize` o del default `25`.

**Alternativas consideradas**
- Paginar manualmente las filas antes del grid: se descarta porque AG Grid ya resuelve ese comportamiento.
- Reutilizar la barra externa en `client mode`: se descarta para esta fase porque duplicaría controles.

### 3. `server mode` mantiene al grid como renderer puro de la página actual

En `server mode`, `AppTable` no debe activar `pagination = true`. Las filas recibidas ya representan la página actual proveniente de backend y la navegación queda completamente a cargo del wrapper externo.

Esto alinea el renderer con la arquitectura tipo Gmail definida para `workflowInboxgestion` y evita mezclar paginación cliente y servidor a la vez.

**Alternativas consideradas**
- Activar la paginación del grid también en `server mode`: se descarta porque partiría dos veces el dataset.

### 4. `quickFilterText` solo aplica en modos locales

`quickFilterText` se aplicará en `client` y `none`, pero no en `server`. En `server mode` la búsqueda pertenece al request backend y no debe alterar localmente la página recibida.

**Alternativas consideradas**
- Ignorar `quickFilterText` por completo hasta tickets posteriores: se descarta porque el requerimiento ya fija soporte local reutilizable.
- Permitirlo también en `server mode`: se descarta porque rompería coherencia entre total, resultados y request backend.

### 5. `useAgGridBaseConfig` seguirá siendo un compositor de opciones de grid

La composición de `pagination` y `paginationPageSize` vivirá principalmente en `useAgGridBaseConfig`, mientras que `AppTable` decidirá cuándo aplicar `quickFilterText` y cómo preservar la compatibilidad hacia atrás.

**Alternativas consideradas**
- Mover toda la lógica al componente: se descarta porque ya existe un hook de configuración base.
- Poner `quickFilterText` dentro del hook base: se descarta para no mezclar estado del grid con transporte de props del componente.

## Risks / Trade-offs

- [Risk] Romper tablas existentes al introducir nuevas props de paginación.  
  Mitigation: mantener comportamiento previo cuando `paginationMode` sea `undefined`.

- [Risk] Que `quickFilterText` altere resultados en `server mode`.  
  Mitigation: aplicarlo solo en `client` y `none`.

- [Risk] Sobrecargar `useAgGridBaseConfig` con lógica que no pertenece al grid base.  
  Mitigation: limitar el hook a configuración de AG Grid y dejar el gating de `quickFilterText` en `AppTable`.

- [Risk] Que el default de `clientPaginationPageSize` varíe por consumidor.  
  Mitigation: fijar `25` como default único documentado y probado.

## Migration Plan

1. Extender `AppTable.types.ts` con el contrato de `paginationMode`, `quickFilterText` y `clientPaginationPageSize`.
2. Ajustar `useAgGridBaseConfig` para soportar `pagination` y `paginationPageSize` por modo.
3. Ajustar `AppTable.tsx` para aplicar `quickFilterText` solo en modos locales y preservar compatibilidad previa.
4. Cubrir `none`, `client`, `server` y compatibilidad hacia atrás con pruebas focalizadas.
5. Dejar listo `AppTable` para que `SCRUMCORE-42` y `SCRUMCORE-43` lo consuman sin redefinir paginación.

Rollback: al ser un cambio encapsulado en `AppTable`, puede revertirse retirando las props nuevas y restaurando el hook base si aparece alguna regresión.

## Open Questions

- Si conviene exponer `clientPaginationPageSize` también en `AppTableQueryWrapper` más adelante o mantenerlo exclusivamente en el renderer base.
- Si el comportamiento de `quickFilterText` en `none` debe seguir siendo activo por defecto o si algún consumidor futuro podría pedir deshabilitarlo explícitamente.

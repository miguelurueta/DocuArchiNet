## Context

`GestionCorrespondencia` es el primer módulo real que debe adoptar la arquitectura completa construida en los tickets anteriores:

- `AppTableQueryState`
- `AppTableQueryWrapper`
- `AppTable` con `paginationMode="server"`
- mapper server reusable para la consulta backend

El backend de `workflowInboxgestion` ya quedó preparado con claims reales, `Pagination.Total` real y request compatible con búsqueda server. Por eso esta fase ya no debe resolver infraestructura nueva; debe migrar la pantalla a las piezas compartidas sin reintroducir wiring ad hoc.

Hoy el módulo mantiene estado y layout propios para:

- búsqueda
- page size
- refresh
- bloque visual de paginación

Además, sigue usando un mapper específico del módulo y un hook con shape previo. El riesgo principal es mezclar la nueva infraestructura reusable con controles viejos o romper comportamiento existente del drawer, la subruta `respuesta`, `MenuActions`, columnas fijas y estados vacíos/cargando.

## Goals / Non-Goals

**Goals:**
- Migrar `GestionCorrespondencia` a `AppTableQueryWrapper`.
- Hacer que `useGestionCorrespondenciaTable` use `AppTableQueryState` como única fuente de verdad.
- Renderizar `AppTable` en `server mode`.
- Preservar acciones dinámicas, `MenuActions`, `Pinned/LockPinned`, loading y empty state.
- Mantener estable el patrón actual de `GestionCorrespondenciaRoutePage`, `Outlet` y drawer.

**Non-Goals:**
- No migrar otros módulos.
- No rediseñar el dominio funcional de correspondencia.
- No crear infraestructura paralela a `AppTable`.
- No resolver aquí la generalización final de filtros de dominio como `category` más allá de la decisión explícita para este módulo.

## Decisions

### 1. `useGestionCorrespondenciaTable` pasa a ser un adaptador de la infraestructura shared

El hook del módulo debe dejar de manejar estados sueltos (`search`, `pageSize`, etc.) y pasar a exponer:

- `queryState`
- `onQueryChange`
- `refetch`
- datos finales de tabla (`rows`, `columns`, `total`, `loading`, `error`, `isEmpty`)

Esto deja al hook como adaptador del módulo sobre la capa shared, no como una segunda fuente de verdad.

**Alternativas consideradas**
- Mantener estados locales y sincronizarlos con el query state: se descarta porque duplicaría lógica.
- Resolver el query state solo en la página: se descarta porque el hook del módulo debe seguir encapsulando la integración con backend.

### 2. `GestionCorrespondencia.tsx` se convierte en composición pura

La página principal debe usar:

- `AppTableQueryWrapper`
- `AppTable`

y dejar de renderizar una barra manual paralela para búsqueda, refresh y page size.

Esto reduce wiring repetido y deja el módulo alineado con la capa reusable para futuras tablas.

**Alternativas consideradas**
- Conservar la barra actual y solo usar parte del wrapper: se descarta porque mezclaría dos patrones.

### 3. `GestionCorrespondenciaRoutePage.tsx` se preserva como frontera de carga/ruta

La migración no debe absorber toda la lógica de route-level loading/error dentro de la pantalla principal. El `RoutePage` ya existe para proteger la navegación del módulo y debe mantenerse.

**Alternativas consideradas**
- Mover el loading inicial completamente a `GestionCorrespondencia`: se descarta porque rompería el patrón actual del módulo.

### 4. El filtro `category` no se elimina silenciosamente

El módulo hoy expone un control `category`. En esta fase debe tratarse de forma explícita:

- o se preserva fuera del query state reusable base mientras no participe del request server final
- o se integra de forma formal si ya existe soporte backend real

No puede desaparecer silenciosamente durante la migración.

## Risks / Trade-offs

- [Risk] Romper `Outlet + Drawer` o la subruta `respuesta`.  
  Mitigation: no mover la responsabilidad de ruta fuera de `GestionCorrespondenciaRoutePage` y cubrir regresión de navegación.

- [Risk] Perder acciones dinámicas o columnas fijas al tocar la conversión de filas/columnas.  
  Mitigation: mantener los mismos adapters de columnas/rows y validar `MenuActions` y `Pinned/LockPinned`.

- [Risk] Reintroducir estado paralelo entre wrapper, página y hook.  
  Mitigation: hacer que el hook exponga `queryState` y `onQueryChange` como fuente única.

- [Risk] El filtro `category` quede en un limbo funcional.  
  Mitigation: mantenerlo explícitamente preservado o excluirlo formalmente sin borrarlo de forma implícita.

## Migration Plan

1. Revisar el estado actual de `GestionCorrespondencia`, `GestionCorrespondenciaRoutePage` y su hook.
2. Migrar `useGestionCorrespondenciaTable` para usar `AppTableQueryState` y `useDynamicUiTableQuery` shared.
3. Reemplazar la barra manual de `GestionCorrespondencia` por `AppTableQueryWrapper`.
4. Renderizar `AppTable` en `paginationMode="server"` usando el query state del hook.
5. Validar regresiones sobre drawer, subruta `respuesta`, acciones dinámicas, columnas fijas y empty/loading states.

Rollback: al estar contenido en un solo módulo, la migración puede revertirse restaurando la barra actual y el hook previo si aparece una regresión crítica.

## Open Questions

- Si `category` debe seguir siendo solo visual/preservado o si ya hay soporte backend suficiente para integrarlo al request del módulo.
- Si conviene exponer `hasLoadedOnce` todavía desde el hook o si el patrón final del `RoutePage` ya lo vuelve innecesario tras la migración.

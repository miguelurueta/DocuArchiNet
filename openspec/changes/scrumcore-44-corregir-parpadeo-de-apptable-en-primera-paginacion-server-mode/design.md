## Context

En `GestionCorrespondencia` se detectó un parpadeo visible en la primera paginación cuando `AppTable` opera en `server mode`.

El síntoma no proviene del renderer aislado, sino de la transición de datos en la capa shared:

- cambia `page`
- cambia el `queryKey` de React Query
- mientras llega la nueva página, `useDynamicUiTableQuery` entrega un fallback vacío
- el host recibe temporalmente `rows = []`, `columns = []` y `total = 0`
- el grid reacciona a ese estado intermedio y el usuario percibe un flash vacío

El problema se hizo visible en `GestionCorrespondencia`, pero la causa es reusable y puede afectar cualquier tabla en `server mode`.

## Goals / Non-Goals

**Goals:**
- eliminar el parpadeo en la primera paginación server
- corregir la causa en la capa shared de consulta
- dejar cubierta la regresión con pruebas automáticas
- mantener compatibilidad con empty state real y client mode

**Non-Goals:**
- no rediseñar `AppTable`
- no cambiar backend
- no mezclar esta corrección con búsqueda avanzada o filtros de dominio
- no introducir lógica específica solo para `GestionCorrespondencia`

## Decisions

### 1. La corrección principal vive en `useDynamicUiTableQuery`

La capa shared de consulta debe conservar los datos previos durante el refetch de una nueva página server.

Se adopta:

- `placeholderData: (previousData) => previousData`

en el `useQuery` de `useDynamicUiTableQuery`.

Con esto:

- mientras llega la nueva página, se mantienen filas y total previos
- desaparece la transición artificial a `rows = []`
- el host ya no entrega un estado vacío intermedio al grid

### 2. `AppTable` no es la única capa responsable

`AppTable` puede endurecerse más adelante para suavizar overlays durante refetch con datos visibles, pero esa medida es complementaria.

La decisión para este ticket es:

- corregir primero la fuente del problema en la query shared
- no acoplar la solución a un renderer particular

### 3. El comportamiento correcto en transición es conservar la página anterior

Durante el cambio de una página server a otra:

- la página anterior puede mantenerse visible
- el estado de `loading` puede permanecer activo
- el total no debe caer a `0`
- la nueva página reemplaza el contenido únicamente cuando la respuesta llega

Esto es preferible al flash vacío porque mantiene continuidad visual.

## Risks / Trade-offs

- [Risk] Enmascarar un empty state real conservando datos anteriores demasiado tiempo.  
  Mitigation: la preservación solo aplica durante el refetch; el empty state real sigue dependiendo de la nueva respuesta.

- [Risk] Introducir comportamiento inesperado en client mode.  
  Mitigation: la corrección se limita a la capa shared de server query y se valida sin tocar la lógica de paginación cliente.

- [Risk] Mover el problema al renderer sin resolver la causa.  
  Mitigation: la corrección principal se mantiene en `useDynamicUiTableQuery`.

## Migration Plan

1. Ajustar `useDynamicUiTableQuery` para preservar datos previos durante refetch.
2. Agregar una prueba específica que valide la conservación de filas y total mientras llega una nueva página.
3. Validar `GestionCorrespondencia` con pruebas del hook y del módulo.
4. Evaluar en un ticket posterior si `AppTable` necesita endurecimiento visual adicional.

## Open Questions

- Si más adelante conviene complementar la solución con una regla visual en `AppTable` para no mostrar overlay agresivo cuando ya existen filas visibles y solo hay refetch.

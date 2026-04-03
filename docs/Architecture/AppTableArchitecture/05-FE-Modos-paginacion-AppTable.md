# Ticket 05 FE

## Titulo

Agregar modos de paginacion `client/server/none` en `AppTable`

## Objetivo

Extender `AppTable` para soportar tres modos de paginación reutilizables:

- `none`
- `client`
- `server`

permitiendo coexistencia entre:

- tablas sin paginación
- tablas con paginación cliente de AG Grid
- tablas con paginación servidor controlada externamente

sin romper la API actual del componente.

## Problema actual

- `AppTable` no distingue modos de paginación
- no existe una forma reusable de alternar entre client pagination y server pagination
- el quick filter local no está claramente delimitado por modo

## Alcance

- agregar `paginationMode`
- agregar `quickFilterText`
- agregar `clientPaginationPageSize`
- preservar compatibilidad hacia atrás
- centralizar la lógica de modo en `AppTable` y `useAgGridBaseConfig`

## No alcance

- no embutir la barra Gmail dentro de `AppTable`
- no migrar aún una pantalla final
- no agregar lógica de query state o backend dentro de `AppTable`

## Dependencias

- Ticket 04 FE completado

## Archivos frontend esperados

- `src/app/Components/UI/AppTable/AppTable.tsx`
- `src/app/Components/UI/AppTable/AppTable.types.ts`
- `src/app/Components/UI/AppTable/hooks/useAgGridBaseConfig.ts`
- `src/app/Components/UI/AppTable/tests/`

## Contratos esperados

```ts
type AppTablePaginationMode = "none" | "client" | "server";
```

Nuevas props mínimas:

```ts
paginationMode?: AppTablePaginationMode;
quickFilterText?: string;
clientPaginationPageSize?: number;
```

## Regla de compatibilidad

Si `paginationMode` no se informa, `AppTable` debe conservar exactamente el comportamiento previo del componente:

- sin paginación nativa del grid
- mismo `domLayout`
- mismos overlays
- misma selección
- mismo render de filas

## Reglas de implementación

### Modo `none`

- sin paginación interna
- sin corte de filas
- renderiza todas las filas recibidas
- `pagination = false`

### Modo `client`

- activar paginación nativa de AG Grid
- `pagination = true`
- usar `clientPaginationPageSize`
- si `clientPaginationPageSize` no viene, usar default fijo documentado:
  - `25`
- `quickFilterText` aplica como filtro local

### Modo `server`

- no activar paginación interna del grid como fuente principal de navegación
- `pagination = false`
- asumir que las filas recibidas ya representan la página actual
- no cortar filas localmente
- `quickFilterText` no debe alterar resultados ni activar filtrado local

## Reglas adicionales

- `clientPaginationPageSize` solo aplica en `client`
- en otros modos debe ignorarse sin romper render
- `quickFilterText` aplica en:
  - `client`
  - `none`
- `quickFilterText` no aplica en:
  - `server`
- `AppTable` debe garantizar que solo un modo de paginación esté activo a la vez

## Responsabilidad por archivo

### `AppTable.types.ts`

- centralizar contratos nuevos de paginación

### `useAgGridBaseConfig.ts`

- componer configuración base del grid según `paginationMode`
- evitar duplicación de lógica
- no absorber lógica de backend ni de query state

### `AppTable.tsx`

- aplicar el modo configurado
- aplicar `quickFilterText` cuando corresponda
- mantener compatibilidad hacia atrás
- no mezclar lógica de backend o pantalla

## Regla técnica de AG Grid

- `client mode`
  - `pagination = true`
- `server mode`
  - `pagination = false`
- `none mode`
  - `pagination = false`

La navegación de `server mode` debe quedar completamente a cargo del wrapper externo.

## Riesgos a evitar

- activar paginación cliente y servidor a la vez
- romper pantallas actuales sin `paginationMode`
- hacer que `quickFilterText` cambie resultados de `server mode`
- esconder lógica de modo en múltiples capas
- sobrecargar `useAgGridBaseConfig` con lógica que pertenece a `AppTable`

## Pruebas obligatorias

- `none mode`
- `client mode`
- `server mode`
- aplicación correcta de `clientPaginationPageSize`
- `quickFilterText` en `client`
- `quickFilterText` en `none`
- `quickFilterText` ignorado en `server`
- compatibilidad hacia atrás sin prop nueva

## Criterios de aceptación

- `AppTable` soporta los tres modos
- se mantiene la API previa
- quick filter local queda disponible en modos locales sin invadir `server mode`
- la lógica de paginación queda centralizada y reusable
- el wrapper externo puede controlar completamente `server mode`

## Instrucción final

Antes de implementar:

- validar comportamiento actual de `AppTable`
- validar `useAgGridBaseConfig`
- validar cómo aplicar `quickFilterText` en AG Grid sin afectar `server mode`

Luego:

- implementar con TypeScript estricto
- mantener separación de capas
- preservar compatibilidad hacia atrás

Finalmente reportar:

- decisiones por modo
- default final de `clientPaginationPageSize`
- estrategia de aplicación de `quickFilterText`
- compatibilidad preservada con usos actuales
